using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using ATDatabaseManager.Properties;
using System.Drawing;
using AT.Toolbox.MSSQL.DbMould;

namespace ATDatabaseManager
{
  public partial class BindableTree : TreeView
  {
    #region Events

    [Category("Data")]
    public event ObjectTextEventHandler ImageSelecting;
    [Category("Data")]
    public event TreeNodeEventHandler NodeCreated;
    [Category("Data")]
    public event EventHandler PropertiesCalled;
    [Category("Data")]
    public event EventHandler BoundObjectChanged;
    [Category("Drag Drop")]
    public event MoveEventHandler DragEnterChecking;
    [Category("Drag Drop")]
    public event MoveEventHandler DragDropPerforming;

    #endregion

    #region Constructors

    public BindableTree()
      : base()
    {
    }

    #endregion

    #region Methods

    public TreeNode FindNode(object tag)
    {
      return FindNode(tag, this.Nodes);
    }

    private TreeNode FindNode(object tag, TreeNodeCollection nodes)
    {
      foreach (TreeNode currentNode in nodes)
      {
        if (currentNode.Tag == tag)
        {
          return currentNode;
        }
      }
      foreach (TreeNode currentNode in nodes)
      {
        TreeNode ret = null;
        ret = FindNode(tag, currentNode.Nodes);
        if (ret != null)
        {
          return ret;
        }
      }
      return null;
    }

    /// <summary>
    /// Displays the structure of the specified object at the root of the tree
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public TreeNode BindObject(object data)
    {
      return this.DisplayEntity(data, this.Nodes, null, null);
    }

    /// <summary>
    /// Displays the specified object at the specified place of the tree
    /// </summary>
    /// <param name="data">object to display</param>
    /// <param name="nodes">where to place</param>
    /// <param name="ownerCollection">If specified object is a part of the collection, it should be specified</param>
    /// <param name="parent">owner of the collection</param>
    /// <returns></returns>
    public TreeNode DisplayEntity(object data, TreeNodeCollection nodes, IList ownerCollection, object parent)
    {
      if (data == null || nodes == null)
      {
        return null;
      }
      List<PropertyInfo> collectionProperties = new List<PropertyInfo>();
      Type objectType = data.GetType();
      foreach (PropertyInfo currentProperty in objectType.GetProperties())
      {
        if (currentProperty.GetIndexParameters().Length == 0 &&
            currentProperty.GetValue(data, null) is IList
            && !currentProperty.IsDefined(typeof(NoBindAttribute), true))
        {
          collectionProperties.Add(currentProperty);
        }
      }
      ObjectTextEventArgs imageNameArgs = new ObjectTextEventArgs(data);
      this.OnImageSelecting(imageNameArgs);
      TreeNode newNode = nodes.Add(data.ToString() + data.GetHashCode().ToString()
          , data.ToString()
          , imageNameArgs.Text
          , imageNameArgs.Text);
      newNode.Tag = data;
      newNode.ContextMenuStrip = this.SelectMenu(newNode, null, ownerCollection, parent);
      this.OnNodeCreated(new TreeNodeEventArgs(newNode));

      if (collectionProperties.Count > 0)
      {
        foreach (PropertyInfo currentProperty in collectionProperties)
        {
          bool createFolder = !currentProperty.IsDefined(typeof(SkipCreateNodeAttribute), true);
          DisplayNameAttribute[] displayName = currentProperty.GetCustomAttributes(typeof(DisplayNameAttribute), true) as DisplayNameAttribute[];
          string displayText = displayName.Length > 0 ? displayName[0].DisplayName : currentProperty.Name;

          IList collection = currentProperty.GetValue(data, null) as IList;
          imageNameArgs = new ObjectTextEventArgs(collection);
          this.OnImageSelecting(imageNameArgs);

          TreeNode collectionNode = createFolder ?
                                    newNode.Nodes.Add(currentProperty.Name
                                  , displayText
                                  , imageNameArgs.Text
                                  , imageNameArgs.Text)
                                  : newNode;
          if (createFolder)
          {
            collectionNode.Tag = collection;
            this.OnNodeCreated(new TreeNodeEventArgs(collectionNode));
          }
          collectionNode.ContextMenuStrip = this.SelectMenu(collectionNode, collection, ownerCollection, data);
          foreach (object item in collection)
          {
            this.DisplayEntity(item, collectionNode.Nodes, collection, data);
          }
        }
      }
      return newNode;
    }

    protected virtual void OnImageSelecting(ObjectTextEventArgs imageNameArgs)
    {
      if (ImageSelecting != null)
      {
        ImageSelecting(this, imageNameArgs);
      }
    }

    protected void OnNodeCreated(TreeNodeEventArgs e)
    {
      if (this.NodeCreated != null)
      {
        this.NodeCreated(this, e);
      }
    }

    private ContextMenuStrip SelectMenu(TreeNode node, IList innerCollection, IList ownerCollection, object parent)
    {
      object data = node.Tag;
      ContextMenuStrip menu = new ContextMenuStrip();
      if (data != null && !(data is IList))
      {
        ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete", null, new EventHandler(DeleteItem));
        deleteItem.Tag = ownerCollection;
        ToolStripMenuItem propertiesItem = new ToolStripMenuItem("Properties", null, new EventHandler(GetProperties));
        menu.Items.Add(propertiesItem);

        if (parent == null)
        {
          return null;
        }
        else if (innerCollection != null)
        {
          ToolStripItem constructItem = this.CreateConstructor(node, innerCollection, parent);
          if (constructItem != null)
          {
            menu.Items.Insert(0, constructItem);
            menu.Items.Insert(0, deleteItem);
          }
          else
          {
            menu.Items.Insert(0, deleteItem);
          }
        }
        else if (this.CreateConstructor(node, ownerCollection, parent) != null)
        {
          menu.Items.Insert(0, deleteItem);
        }
      }
      else
      {
        IList listData = node.Tag as IList;
        if (listData != null)
        {
          ToolStripItem constructItem = this.CreateConstructor(node, listData, parent);
          if (constructItem != null)
          {
            menu.Items.Add(constructItem);
          }
        }
      }
      return menu;
    }

    private ToolStripItem CreateConstructor(TreeNode node, IList listData, object parent)
    {
      Type listType = listData.GetType();
      Type itemType = null;
      if (listType.IsArray)
      {
        itemType = listType.GetElementType();
      }
      else
      {
        itemType = listType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance).GetParameters()[0].ParameterType;
      }
      bool requireObject = false;
      PropertyInfo enumProperty;

      MethodInfo constructMethod = ElementBinder.FindConstructMethod(itemType, out requireObject, out enumProperty);
      ConstructorInfo constructor = itemType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

      string displayText = "New ";
      DisplayNameAttribute[] attributes = itemType.GetCustomAttributes(typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];
      if (attributes.Length > 0)
      {
        displayText += attributes[0].DisplayName;
      }
      if (constructMethod != null)
      {
        ToolStripMenuItem groupItem = new ToolStripMenuItem(displayText);
        ParameterInfo[] parameters = constructMethod.GetParameters();
        foreach (object currentValue in Enum.GetValues(parameters[0].ParameterType))
        {
          object secondParameter = null;
          if (requireObject)
          {
            if (parent.GetType().IsSubclassOf(parameters[1].ParameterType)
                || parent.GetType() == parameters[1].ParameterType)
            {
              secondParameter = parent;
            }
            else if (listData.GetType().IsSubclassOf(parameters[1].ParameterType)
                || listData.GetType() == parameters[1].ParameterType)
            {
              secondParameter = listData;
            }
          }
          object[] parametersToUse = requireObject ? new object[] { currentValue, secondParameter } : new object[] { currentValue };
          object returnValue = itemType.InvokeMember(constructMethod.Name, BindingFlags.Public
                                                      | BindingFlags.Static
                                                      | BindingFlags.ExactBinding
                                                      | BindingFlags.InvokeMethod
                                                      , null
                                                      , null
                                                      , parametersToUse);
          if (returnValue != null)
          {
            ToolStripMenuItem enumItem = new ToolStripMenuItem(currentValue.ToString(), null, new EventHandler(ConstructItem));
            enumItem.Tag = new object[] { itemType, parametersToUse, listData, parent };
            groupItem.DropDownItems.Add(enumItem);
            returnValue = null;
          }
        }
        return groupItem;
      }
      else if (constructor != null)
      {
        ToolStripMenuItem newItem = new ToolStripMenuItem(displayText, null, new EventHandler(CreateItem));
        newItem.Tag = new object[] { itemType, listData, parent };
        return newItem;
      }
      else
      {
        return null;
      }
    }

    #endregion

    #region Menu item Handlers

    private void ConstructItem(object sender, EventArgs e)
    {
      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      if (menuItem != null)
      {
        try
        {
          bool flag;
          PropertyInfo enumProperty;
          object[] tag = menuItem.Tag as object[];
          MethodInfo constructMethod = ElementBinder.FindConstructMethod(tag[0] as Type, out flag, out enumProperty);
          object item = (tag[0] as Type).InvokeMember(constructMethod.Name, BindingFlags.Public
                                                      | BindingFlags.Static
                                                      | BindingFlags.ExactBinding
                                                      | BindingFlags.InvokeMethod
                                                      , null
                                                      , null
                                                      , tag[1] as object[]);
          IList listData = tag[2] as IList;
          if (listData != null)
          {
            listData.GetType().InvokeMember("Add", BindingFlags.InvokeMethod
                | BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.ExactBinding, null, listData, new object[] { item });
            this.SelectedNode = this.DisplayEntity(item, this.SelectedNode.Nodes, listData, tag[3]);
          }
          this.NotifyChanged();
        }
        catch (Exception ex)
        {
          if (ex.InnerException != null)
          {
            MessageBox.Show(ex.InnerException.Message, Resources.Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
          }
          else
          {
            MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void CreateItem(object sender, EventArgs e)
    {
      ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
      if (menuItem != null)
      {
        object[] constructorParameters = null;
        Type itemType = (menuItem.Tag as object[])[0] as Type;
        IList listData = (menuItem.Tag as object[])[1] as IList;
        if (this.SelectedNode.Tag != listData
            && itemType.GetConstructor(new Type[] { (menuItem.Tag as object[])[1].GetType() }) != null)
        {
          constructorParameters = new object[] { (menuItem.Tag as object[])[1] };
        }
        else if (itemType.GetConstructor(new Type[] { (menuItem.Tag as object[])[2].GetType() }) != null)
        {
          constructorParameters = new object[] { (menuItem.Tag as object[])[2] };
        }
        object newItem = itemType.InvokeMember("", BindingFlags.Public
            | BindingFlags.CreateInstance
            | BindingFlags.Instance
            | BindingFlags.ExactBinding, null, null, constructorParameters);
        if (listData != null)
        {
          listData.GetType().InvokeMember("Add", BindingFlags.InvokeMethod
              | BindingFlags.Public
              | BindingFlags.Instance
              | BindingFlags.ExactBinding, null, listData, new object[] { newItem });
          this.SelectedNode = this.DisplayEntity(newItem, this.SelectedNode.Nodes, listData, (menuItem.Tag as object[])[2]);
        }
        this.NotifyChanged();
      }
    }

    private void DeleteItem(object sender, EventArgs e)
    {
      object data = this.SelectedNode.Tag;
      IList ownerCollection = (sender as ToolStripItem).Tag as IList;
      if (data != null && ownerCollection != null)
      {
        try
        {
          this.SelectedNode.Remove();
          ownerCollection.GetType().InvokeMember("Remove", BindingFlags.ExactBinding
                                                      | BindingFlags.InvokeMethod
                                                      | BindingFlags.Public
                                                      | BindingFlags.Instance,
                                                      null, ownerCollection, new object[] { data });
        }
        catch (Exception ex)
        {
          if (ex.InnerException != null)
          {
            MessageBox.Show(ex.InnerException.Message, Resources.Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
          }
          else
          {
            MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
          }
        }
        this.NotifyChanged();
      }
    }

    private void NotifyChanged()
    {
      if (BoundObjectChanged != null)
      {
        BoundObjectChanged(this, new EventArgs());
      }
    }

    private void GetProperties(object sender, EventArgs e)
    {
      if (PropertiesCalled != null)
      {
        PropertiesCalled(sender, e);
      }
    }

    #endregion

    #region Common handlers

    void PerpareDrag(DragEventArgs drgevent)
    {
      TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
      TreeNode destination = this.GetNodeAt(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
      if (this.DragEnterChecking != null)
      {
        TreeNodeMoveEventArgs moveArgs = new TreeNodeMoveEventArgs(source, destination);
        this.DragEnterChecking(this, moveArgs);
        drgevent.Effect = moveArgs.Cancel ? DragDropEffects.None : DragDropEffects.Move;
        if (drgevent.Effect == DragDropEffects.Move)
        {
          base.OnDragEnter(drgevent);
        }
      }
      this.SelectedNode = destination != null ? destination : this.SelectedNode;
    }

    protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
    {
      this.SelectedNode = e.Node;
      base.OnNodeMouseClick(e);
    }

    protected override void OnItemDrag(ItemDragEventArgs e)
    {
      DoDragDrop(e.Item, DragDropEffects.Move);
      base.OnItemDrag(e);
    }

    protected override void OnDragOver(DragEventArgs drgevent)
    {
      this.PerpareDrag(drgevent);
      base.OnDragOver(drgevent);
    }

    protected override void OnDragDrop(DragEventArgs drgevent)
    {
      base.OnDragDrop(drgevent);
      if (drgevent.Effect == DragDropEffects.Move)
      {
        TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
        TreeNode destination = this.GetNodeAt(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
        if (this.DragDropPerforming != null)
        {
          this.DragDropPerforming(this, new TreeNodeMoveEventArgs(source, destination));
        }
      }
    }

    #endregion

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.ResumeLayout(false);
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public sealed class ConstructMethodAttribute : Attribute
  {
  }

  public static class ElementBinder
  {
    public static MethodInfo FindConstructMethod(Type itemType, out bool requireObject, out PropertyInfo enumProperty)
    {
      requireObject = false;
      enumProperty = null;
      foreach (MethodInfo method in itemType.GetMethods(BindingFlags.Public | BindingFlags.Static))
      {
        if (method.IsDefined(typeof(ConstructMethodAttribute), false))
        {
          ParameterInfo[] parameters = method.GetParameters();
          foreach (PropertyInfo proper in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
          {
            if (proper.PropertyType.IsEnum && proper.PropertyType == parameters[0].ParameterType)
            {
              enumProperty = proper;
              switch (parameters.Length)
              {
                case 1:
                  return method;
                case 2:
                  requireObject = true;
                  return method;
              }
            }
          }
        }
      }
      return null;
    }
  }

  public delegate void TreeNodeEventHandler(object sender, TreeNodeEventArgs e);

  public class TreeNodeEventArgs : EventArgs
  {
    TreeNode node;
    internal TreeNodeEventArgs(TreeNode node)
    {
      this.node = node;
    }

    public TreeNode Node
    {
      get
      {
        return node;
      }
    }
  }

  public delegate void ObjectTextEventHandler(object sender, ObjectTextEventArgs e);

  public class ObjectTextEventArgs : EventArgs
  {
    internal ObjectTextEventArgs(object tag)
      : base()
    {
      this.Text = string.Empty;
      this.Tag = tag;
    }

    public object Tag { get; private set; }

    public string Text { get; set; }
  }


  public delegate void MoveEventHandler(object sender, TreeNodeMoveEventArgs e);

  public class TreeNodeMoveEventArgs : CancelEventArgs
  {
    TreeNode source;
    TreeNode destination;

    internal TreeNodeMoveEventArgs(TreeNode source, TreeNode destination)
      : base(true)
    {
      this.source = source;
      this.destination = destination;
    }

    public TreeNode Source
    {
      get
      {
        return source;
      }
    }

    public TreeNode Destination
    {
      get
      {
        return destination;
      }
    }
  }
}
