using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using AT.Toolbox.Base;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Help
{
  public partial class UserHelpNavigator : LocalizableUserControl
  {
    public UserHelpNavigator()
    {
      InitializeComponent();
      OnHelpOpen += delegate { };
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<HelpFileNode> Nodes
    {
      get { return helpFileNodeBindingSource.DataSource as BindingList<HelpFileNode>; }
      set { helpFileNodeBindingSource.DataSource = value; }
    }

    public event EventHandler<ParamEventArgs<string>> OnHelpOpen;

    public override void  PerformLocalization(object sender, EventArgs e)
    {
      this.m_tab_contents.Text = Resources.HELP_BROWSER_CONTENTS;
      this.m_tab_index.Text = Resources.HELP_BROWSER_INDEX;
    }

    private void HandleTreeDoubleClick(object sender, EventArgs e)
    {
      /*return;

      if (null == treeList1.FocusedNode)
        return;

      HelpFileNode nod = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;

      if (null == nod)
        return;

      if (!nod.IsAnchor)
        OnHelpOpen(this, new ParamEventArgs<string>(nod.ID));*/
    }

    private void HandleGridDoubleClick(object sender, EventArgs e)
    {
      if (gridView1.FocusedRowHandle < 0)
        return;

      object o = gridView1.GetRow(gridView1.FocusedRowHandle);

      if (null == o)
        return;

      HelpFileNode nod = o as HelpFileNode;

      if (null == nod)
        return;

      if (!nod.IsAnchor)
        OnHelpOpen(this, new ParamEventArgs<string>(nod.ID));
    }

    private void HadleTreeClick(object sender, EventArgs e)
    {
    }

    private void HandleTreeMouseClick(object sender, MouseEventArgs e)
    {
      if (treeList1.FocusedNode != treeList1.Selection[0])
        return;

      HelpFileNode nod = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;

      if (null == nod)
        return;

      if (!nod.IsAnchor)
        OnHelpOpen(this, new ParamEventArgs<string>(nod.ID));
      else
        OnHelpOpen(this, new ParamEventArgs<string>(nod.ParentID + "#" + nod.ID));
    }

    private void HandleTreeGetSelectedImage(object sender, GetSelectImageEventArgs e)
    {
      HelpFileNode nod = treeList1.GetDataRecordByNode(e.Node) as HelpFileNode;

      if (null == nod)
        return;

      if (!e.Node.HasChildren)
      {
        if (nod.IsAnchor)
          e.NodeImageIndex = 3;
        else
          e.NodeImageIndex = 2;

        return;
      }

      foreach (TreeListNode child in e.Node.Nodes)
      {
        HelpFileNode n = treeList1.GetDataRecordByNode(child) as HelpFileNode;

        if (null == n)
          break;

        if (n.IsAnchor)
        {
          e.NodeImageIndex = 2;
          return;
        }
      }

      if (e.Node.Expanded)
        e.NodeImageIndex = 1;
      else
        e.NodeImageIndex = 0;
    }
  }
}