using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using Toolbox.Data;
using Toolbox.Extensions;
using log4net;

namespace Toolbox.GUI.Tree
{
  static class TreeExtensions
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(TreeExtensions));

    static readonly Dictionary<INotifyPropertyChanged, TreeNode> _change_handlers
      = new Dictionary<INotifyPropertyChanged, TreeNode>();

    public static TreeNode DisplayEntry(this TreeNodeCollection nodes, object component, int index, bool skipRecreate, bool skipMenuCreation)
    {
      if (component == null)
        return null;

      IBindingList list = component as IBindingList;

      if (list == null)
      {
        IListSource source = component as IListSource;

        if (source != null)
          list = source.GetList() as IBindingList;
      }

      var ret = list != null ? new ListTreeNode(skipRecreate, skipMenuCreation) : new TreeNode();
      nodes.Insert(index, ret);

      try
      {
        ret.Text = component.ToString();
      }
      catch (Exception ex)
      {
        _log.Error("DisplayEntry(): Exception", ex);

        if (component != null)
          ret.Text = component.GetType().GetDisplayName();
      }

      ret.Tag = component;

      ret.SetImage(component);

      if (list != null)
      {
        ((ListTreeNode)ret).List = list;
      }

      ret.ProcessChildren(component, list, skipRecreate, skipMenuCreation);

      ListTreeNode parentNode = ret.Parent as ListTreeNode;
      if (parentNode != null && parentNode.List != null && ret.ContextMenuStrip == null
        && !skipMenuCreation)
      {
        ret.ContextMenuStrip = new ContextMenuStrip();
        ret.ContextMenuStrip.Items.Add(Toolbox_GUI_Resources.DELETE, null, delegate
        {
          parentNode.List.Remove(component);
        });
      }

      ret.AddPropertyHandler(component);

      return ret;
    }

    internal static void ProcessChildren(this TreeNode node, object component, IBindingList list, bool skipRecreate, bool skipMenuCreation)
    {
      var pds = (from PropertyDescriptor pd in TypeDescriptor.GetProperties(component)
                 where typeof(IBindingList).IsAssignableFrom(pd.PropertyType)
                 && pd.Attributes[typeof(NoBindAttribute)] == null
                 select new { Name = pd.DisplayName, Value = (IBindingList)pd.GetValue(component) }).ToList();

      foreach (var current in pds)
      {
        ListTreeNode listNode = ReferenceEquals(current.Value, list) ?
        (ListTreeNode)node : new ListTreeNode(skipRecreate, skipMenuCreation);

        if (!ReferenceEquals(listNode, node))
        {
          node.Nodes.Add(listNode);
          listNode.Text = current.Name;
          listNode.Tag = current.Value;
          listNode.List = current.Value;
          listNode.SetImage(current.Value);
        }

        for (int i = 0; i < current.Value.Count; i++)
        {
          listNode.Nodes.DisplayEntry(current.Value[i], i, skipRecreate, skipMenuCreation);
        }
      }
    }

    internal static void SetImage(this TreeNode ret, object component)
    {
      ImageIdxEventArgs e = new ImageIdxEventArgs(component);
      foreach (var binder in TreeBindingController.GetControllersForTree(ret.TreeView))
      {
        binder.SelectImage(e);

        if (e.ImageIdx >= 0)
          break;
      }

      ret.SelectedImageIndex = e.ImageIdx;
      ret.ImageIndex = e.ImageIdx;
    }

    internal static void AddPropertyHandler(this TreeNode ret, object component)
    {
      INotifyPropertyChanged pc = component as INotifyPropertyChanged;
      if (pc != null)
        lock (_change_handlers)
        {
          if (!_change_handlers.ContainsKey(pc))
          {
            _change_handlers.Add(pc, ret);
            pc.PropertyChanged += PropertyChangedHandler;
          }
        }
    }

    private static void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
      INotifyPropertyChanged pc = sender as INotifyPropertyChanged;

      if (pc == null)
        return;

      lock (_change_handlers)
      {
        TreeNode node;

        if (_change_handlers.TryGetValue(pc, out node))
        {
          node.Text = pc.ToString();

          if (node.TreeView != null)
          {
            foreach (var ctrlr in TreeBindingController.GetControllersForTree(node.TreeView))
            {
              var args = new ImageIdxEventArgs(sender);

              ctrlr.SelectImage(args);

              node.ImageIndex = args.ImageIdx;
              node.SelectedImageIndex = args.ImageIdx;
            }
          }
        }
      }
    }

    public static void ClearContent(this TreeNode node)
    {
      foreach (TreeNode child in node.Nodes)
      {
        child.ClearContent();
      }

      ListTreeNode ltn = node as ListTreeNode;

      if (ltn != null)
      {
        ltn.List = null;
      }

      INotifyPropertyChanged pc = node.Tag as INotifyPropertyChanged;
      if (pc != null)
      {
        lock (_change_handlers)
        {
          _change_handlers.Remove(pc);
          pc.PropertyChanged -= PropertyChangedHandler;
        }
      }
    }
  }
}
