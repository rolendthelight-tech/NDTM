using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.GUI.Tree
{
  /// <summary>
  /// Узел дерева, связанный с коллекцией данных
  /// </summary>
  class ListTreeNode : TreeNode
  {
    IBindingList m_list;
    Type m_item_type;
    private readonly bool m_skip_recreate;
    private readonly bool m_skip_menu_creation;

    public ListTreeNode(bool skipRecreate, bool skipMenuCreation)
    {
      m_skip_recreate = skipRecreate;
      m_skip_menu_creation = skipMenuCreation;
    }

    public IBindingList List
    {
      get { return m_list; }
      set
      {
        if (ReferenceEquals(m_list, value))
          return;

        m_item_type = null;
        if (m_list != null)
        {
          m_list.ListChanged -= OnListChanged;
        }
        m_list = value;
        base.ContextMenuStrip = null;
        if (value != null)
        {
          foreach (MethodInfo mi in m_list.GetType().GetMethods())
          {
            var parms = mi.GetParameters();
            if (mi.Name == "Add" && parms.Length == 1)
            {
              m_item_type = parms[0].ParameterType;
              break;
            }
          }
          value.ListChanged += OnListChanged;
        }
      }
    }

    public override ContextMenuStrip ContextMenuStrip
    {
      get
      {
        if (!m_skip_menu_creation)
          this.CreateMenu();

        return base.ContextMenuStrip;
      }
      set
      {
        base.ContextMenuStrip = value;
      }
    }

    private string GetCategory(Type type)
    {
      var att = type.GetCustomAttribute<CategoryAttribute>(true);

      if (att != null)
        return att.Category ?? "";

      return "";
    }

    private void CreateMenu()
    {
      if (base.ContextMenuStrip != null)
        return;

      if (m_list == null || m_item_type == null)
        return;

      base.ContextMenuStrip = new ContextMenuStrip();

      if (this.TreeView != null && this.TreeView.Container != null)
        this.TreeView.Container.Add(this.ContextMenuStrip);

      var types = this.GetKnownTypes(m_item_type);
      if (types.Count == 1)
      {
				base.ContextMenuStrip.Items.Add(CreateAddButton(types.First(), Toolbox_GUI_Resources.NEW + " "));
      }
      else if (types.Count > 1)
      {
				var group = new ToolStripMenuItem(Toolbox_GUI_Resources.NEW);
        base.ContextMenuStrip.Items.Add(group);

        foreach (var cat in types.ToLookup(t => this.GetCategory(t)))
        {
          if (group.DropDownItems.Count != 0)
            group.DropDownItems.Add(new ToolStripSeparator());

          if (!string.IsNullOrEmpty(cat.Key))
          {
            ToolStripLabel label = new ToolStripLabel(cat.Key);
            label.Font = new Font(label.Font, FontStyle.Bold | FontStyle.Underline);
            group.DropDownItems.Add(label);
          }

          foreach (var type in cat)
            group.DropDownItems.Add(this.CreateAddButton(type, ""));
        }
      }
      base.ContextMenuStrip.Items.Add(this.CreateClearButton());

      ListTreeNode parentNode = this.Parent as ListTreeNode;
      if (parentNode != null && parentNode.List != null)
      {
				base.ContextMenuStrip.Items.Add(Toolbox_GUI_Resources.DELETE, null, delegate
        {
          parentNode.List.Remove(this.Tag);
        });
      }
    }

	  [NotNull]
	  private HashSet<Type> GetKnownTypes([NotNull] Type baseType)
    {
		  if (baseType == null) throw new ArgumentNullException("baseType");

			var ret = new HashSet<Type>();

      if (!baseType.IsAbstract
        && baseType.GetConstructor(Type.EmptyTypes) != null)
        ret.Add(baseType);

      foreach (KnownTypeAttribute att in baseType.GetCustomAttributes(typeof(KnownTypeAttribute), true))
      {
        if (att.Type != null && !att.Type.IsAbstract
          && att.Type.GetConstructor(Type.EmptyTypes) != null
          && baseType.IsAssignableFrom(att.Type))
        {
          ret.Add(att.Type);
        }

        if (!string.IsNullOrEmpty(att.MethodName))
        {
          MethodInfo mi = baseType.GetMethod(att.MethodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            Type.DefaultBinder, Type.EmptyTypes, ArrayExtensions.Empty<ParameterModifier>());

          if (mi != null && typeof(IEnumerable<Type>).IsAssignableFrom(mi.ReturnType))
          {
            foreach (var type in (IEnumerable<Type>)mi.Invoke(null, null))
            {
              if (!type.IsAbstract 
                && type.GetConstructor(Type.EmptyTypes) != null
                && baseType.IsAssignableFrom(type))
                ret.Add(type);
            }
          }
        }
      }

      return ret;
    }

	  [NotNull]
	  private ToolStripMenuItem CreateClearButton()
    {
			var ret = new ToolStripMenuItem(Toolbox_GUI_Resources.CLEAR);

      ret.Click += delegate
      {
        m_list.Clear();
      };

      return ret;
    }

	  [NotNull]
	  private ToolStripMenuItem CreateAddButton([NotNull] Type itemType, [CanBeNull] string prefix)
    {
	    if (itemType == null) throw new ArgumentNullException("itemType");

			string type_name = itemType.Name;

      if (itemType.IsDefined(typeof(DisplayNameAttribute), true))
      {
        type_name = ((DisplayNameAttribute) itemType.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]).DisplayName;

        if (!string.IsNullOrEmpty(type_name))
        {
          if (!string.IsNullOrEmpty(prefix))
            type_name = type_name[0].ToString().ToLower() + type_name.Substring(1);
        }
        else
          type_name = itemType.Name;
      }

      var ret = new ToolStripMenuItem(prefix + type_name);

      ret.Click += delegate
      {
        m_list.Add(Activator.CreateInstance(itemType));
      };

      return ret;
    }

    private void OnListChanged([NotNull] object sender, [NotNull] ListChangedEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			switch (e.ListChangedType)
      {
        case ListChangedType.ItemAdded:
          this.AddItem(e.NewIndex);
          break;
        case ListChangedType.ItemChanged:
          this.ChangeItem(e.NewIndex, e.PropertyDescriptor);
          break;
        case ListChangedType.ItemDeleted:
          this.RemoveItem(e.NewIndex);
          break;
        case ListChangedType.Reset:
          this.Reset();
          break;
      }
    }

	  private void ChangeItem(int index, [CanBeNull] PropertyDescriptor pd)
    {
      var changee = this.Nodes[index];

      if (pd == null && !m_skip_recreate)
      {
        changee.ClearContent();
        changee.Remove();

        var ret = this.Nodes.DisplayEntry(m_list[index], index, m_skip_recreate, m_skip_menu_creation);

        if (ret.TreeView != null)
          ret.TreeView.SelectedNode = ret;
      }
      else
      {
        changee.Text = (changee.Tag ?? "").ToString();
      }
    }

    private void Reset()
    {
      object tag = this.Tag;
      IBindingList list = m_list;

      this.ClearContent();
      this.Nodes.Clear();

      this.List = list;
      this.ProcessChildren(tag, list, m_skip_recreate, m_skip_menu_creation);
    }

    private void AddItem(int index)
    {
      if (m_list == null || m_list.Count <= index || index < 0)
        return;

      object new_item = m_list[index];

      TreeNode new_node = this.Nodes.DisplayEntry(new_item, index, m_skip_recreate, m_skip_menu_creation);
      new_node.Expand();
      this.TreeView.SelectedNode = new_node;
    }

    private void RemoveItem(int index)
    {
      if (m_list == null || this.Nodes.Count <= index || index < 0)
        return;

      var removee = this.Nodes[index];

      removee.ClearContent();
      removee.Tag = null;
      removee.Remove();
    }
  }
}
