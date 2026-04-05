using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Toolbox.Data;

namespace Toolbox.GUI.Tree
{
  /// <summary>
  /// Компонент для привязки дерева к иерархическому источнику данных
  /// </summary>
  public class TreeBindingController : Component
  {
    #region Fields

    object m_dataSource;

    static readonly Dictionary<TreeBindingController, TreeView> _trees 
      = new Dictionary<TreeBindingController, TreeView>();

    #endregion

    #region Constructors

    /// <summary>
    /// Инициализирует новый экземпляр контроллера дерева
    /// </summary>
    public TreeBindingController()
    {
      lock (_trees)
        _trees.Add(this, null);
    }

    /// <summary>
    /// Инициализирует новый экземпляр контроллера дерева
    /// </summary>
    /// <param name="container">Контейнер, в который добавляется компонент</param>
    public TreeBindingController(IContainer container)
      : this()
    {
      container.Add(this);
    }

    #endregion

    #region Events

    /// <summary>
    /// Происходит при выборе изображения для создаваемого узла
    /// </summary>
    public event EventHandler<ImageIdxEventArgs> ImageSelecting;

    /// <summary>
    /// Происходит при проверке допустимости перемещения узла
    /// </summary>
    public event EventHandler<TreeNodeMoveEventArgs> DragEnterChecking;

    /// <summary>
    /// Производит перемещение узла
    /// </summary>
    public event EventHandler<TreeNodeMoveEventArgs> DragDropPerforming;

    #endregion

    #region Properties

    /// <summary>
    /// Дерево, которое требуется отобразить
    /// </summary>
    public TreeView Tree
    {
      get
      {
        lock (_trees)
          return _trees[this]; 
      }
      set
      {
        lock (_trees)
        {
          TreeView old = _trees[this];

          if (old != null)
          {
            old.NodeMouseClick -= Tree_NodeMouseClick;
            old.KeyDown -= Tree_KeyDown;
            old.ItemDrag -= Tree_ItemDrag;
            old.DragOver -= Tree_DragOver;
            old.DragDrop -= Tree_DragDrop;
          }
          if (value != null)
          {
            value.NodeMouseClick += Tree_NodeMouseClick;
            value.KeyDown += Tree_KeyDown;
            value.ItemDrag += Tree_ItemDrag;
            value.DragOver += Tree_DragOver;
            value.DragDrop += Tree_DragDrop;
          }
          _trees[this] = value;
        }
      }
    }

    [DefaultValue(false)]
    public bool SkipSetItemHandling { get; set; }

    [DefaultValue(false)]
    public bool SkipMenuCreation { get; set; }

    /// <summary>
    /// Объект, отображаемый в дереве
    /// </summary>
    public object DataSource
    {
      get { return m_dataSource; }
      set
      {
        if (this.Tree == null)
          return;

        foreach (TreeNode node in this.Tree.Nodes)
          node.ClearContent();

        this.Tree.Nodes.Clear(); 
        m_dataSource = value;

        if (value == null)
          return;

        var retNode = this.Tree.Nodes.DisplayEntry(value, 0, this.SkipSetItemHandling, this.SkipMenuCreation);

        if (retNode != null)
        {
          retNode.Expand();
        }
      }
    }

    #endregion

    #region Tree handlers

    private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

	    TreeView tree = sender as TreeView;

      if (tree != null)
        tree.SelectedNode = e.Node;
    }

    private void Tree_KeyDown(object sender, KeyEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

	    TreeView tree = sender as TreeView;

      if (tree == null)
        return;

      if (e.KeyCode == Keys.Delete)
      {
        ListTreeNode node = tree.SelectedNode.Parent as ListTreeNode;

        if (node != null && node.List != null && tree.SelectedNode.Tag != null)
        {
          node.List.Remove(tree.SelectedNode.Tag);
        }
      }
      else if (e.KeyCode == Keys.Insert)
      {
        ListTreeNode node = tree.SelectedNode as ListTreeNode;

        if (node != null && node.List != null)
        {
          try
          {
            node.List.AddNew();
          }
          catch { }
        }
      }
      else if (e.KeyCode == Keys.F2)
      {
        if (tree.SelectedNode != null && tree.LabelEdit)
          tree.SelectedNode.BeginEdit();
      }
    }

    private void Tree_ItemDrag(object sender, ItemDragEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

	    TreeView tree = sender as TreeView;

      if (tree != null)
        tree.DoDragDrop(e.Item, DragDropEffects.Move);
    }

    private void Tree_DragOver(object sender, DragEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

	    TreeView tree = sender as TreeView;

      if (tree != null)
        this.PrepareDrag(tree, e);
    }

    private void Tree_DragDrop(object sender, DragEventArgs drgevent)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (drgevent == null) throw new ArgumentNullException("drgevent");

	    TreeView tree = sender as TreeView;

      if (tree != null && drgevent.Effect == DragDropEffects.Move)
      {
        TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
        if (source == null)
        {
          source = drgevent.Data.GetData(typeof(ListTreeNode)) as TreeNode;
        }
        TreeNode destination = tree.GetNodeAt(tree.PointToClient(new Point(drgevent.X, drgevent.Y)));

        TreeNodeMoveEventArgs moveArgs = new TreeNodeMoveEventArgs(source, destination);

        INodeMoveProvider mover = m_dataSource as INodeMoveProvider;
        if (mover != null)
        {
          IList list = null;
          if (destination is ListTreeNode)
          {
            list = (destination as ListTreeNode).List;
          }
          mover.Perform(source.Tag, destination.Tag, list);
        }

        if (this.DragDropPerforming != null)
        {
          this.DragDropPerforming(this, moveArgs);
        }
      }
    }

    private void PrepareDrag(TreeView tree, DragEventArgs drgevent)
    {
	    if (tree == null) throw new ArgumentNullException("tree");
	    if (drgevent == null) throw new ArgumentNullException("drgevent");

	    TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
      if (source == null)
      {
        source = drgevent.Data.GetData(typeof(ListTreeNode)) as TreeNode;
      }
      TreeNode destination = tree.GetNodeAt(tree.PointToClient(new Point(drgevent.X, drgevent.Y)));

      TreeNodeMoveEventArgs moveArgs = new TreeNodeMoveEventArgs(source, destination);

      INodeMoveProvider mover = m_dataSource as INodeMoveProvider;
      if (mover != null)
      {
        IList list = null;
        if (destination is ListTreeNode)
        {
          list = (destination as ListTreeNode).List;
        }
        moveArgs.Cancel = !mover.Check(source.Tag, destination.Tag, list);
      }

      if (this.DragEnterChecking != null)
      {
        this.DragEnterChecking(this, moveArgs);
      }

      drgevent.Effect = moveArgs.Cancel ? DragDropEffects.None : DragDropEffects.Move;

      tree.SelectedNode = destination ?? tree.SelectedNode;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Поиск узла по тэгу
    /// </summary>
    /// <param name="item">Тэг, связанный с узлом</param>
    /// <returns>Первый узел, связанный с указанным тэгом</returns>
    public TreeNode FindNode(object item)
    {
      if (item == null)
        return null;

      var tree = this.Tree;

      if (tree == null)
        return null;

      return this.FindNode(item, tree, tree.Nodes, true);
    }

    private TreeNode FindNode(object item, TreeView tree, TreeNodeCollection nodes, bool checkSelected)
    {
	    if (tree == null) throw new ArgumentNullException("tree");
	    if (nodes == null) throw new ArgumentNullException("nodes");

	    if (checkSelected
        && tree.SelectedNode != null
        && tree.SelectedNode.Tag == item)
        return tree.SelectedNode;

      foreach (TreeNode node in nodes)
      {
        if (node.Tag == item)
          return node;
        else
        {
          var ret = this.FindNode(item, tree, node.Nodes, false);

          if (ret != null)
            return ret;
        }
      }

      return null;
    }


    /// <summary>
    /// Очищает ресурсы и отвязывает дерево от контроллера
    /// </summary>
    /// <param name="disposing">Нужно ли освобождать управляемые ресурсы</param>
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      lock(_trees)
        _trees.Remove(this);
    }

    internal void SelectImage(ImageIdxEventArgs e)
    {
	    if (e == null) throw new ArgumentNullException("e");

	    if (this.ImageSelecting != null)
        this.ImageSelecting(this, e);
    }

	  internal static IEnumerable<TreeBindingController> GetControllersForTree(TreeView tree)
    {
      lock (_trees)
        return _trees.Where(kv => ReferenceEquals(kv.Value, tree)).Select(kv => kv.Key);
    }

    #endregion
  }
}
