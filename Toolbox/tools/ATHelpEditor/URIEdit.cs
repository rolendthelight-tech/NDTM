using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Help;
using ATHelpEditor;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using ATHelpEditor.Properties;

namespace ATHelpEditor
{
  public partial class URIEdit : LocalizableForm
  {
    BindingList<HelpFileNode> m_nodes;

    public BindingList<HelpFileNode> Nodes
    {
      get
      {
        return m_nodes;
      }
      set
      {
        m_nodes = value;
        helpNodeViewBindingSource.DataSource = m_nodes;
      }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      this.Text = Resources.URI_EDITOR;
      m_check_edit_copy.Text = Resources.URI_EDITOR_COPY;
      m_check_edit_resource.Text = Resources.URI_EDITOR_RESOURCE;
      m_check_edit_topic.Text = Resources.URI_EDITOR_TOPIC;
      colIndex.Caption = Resources.URI_EDITOR_INDEX;
      colName.Caption = Resources.URI_EDITOR_NAME;
      m_btn_ok.Text = Resources.BUTTON_OK;
      m_btn_cancel.Text = Resources.BUTTON_CANCEL;
    }

    public URIEdit()
    {
      InitializeComponent();
    }

    private void checkEdit2_CheckedChanged(object sender, EventArgs e)
    {

    }

    public string URI
    {
      get
      {
        if( m_check_edit_resource.Checked )
          return buttonEdit1.EditValue.ToString();
        
        HelpFileNode v = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;

        if (null != v)
        {
          if( v.IsAnchor )
            return "athelp://" + v.ParentID + "#" + v.ID ;
            
          return "athelp://" + v.ID;
        }

        return "";
      }
    }

    private void URIModeOn(object sender, EventArgs e)
    {
      buttonEdit1.Enabled = m_check_edit_resource.Checked;
      m_check_edit_copy.Enabled = m_check_edit_resource.Checked;

      m_check_edit_topic.Checked = !m_check_edit_resource.Checked;
      treeList1.Enabled = m_check_edit_topic.Checked;
    }

    private void OnHelpMode(object sender, EventArgs e)
    {
      if (m_check_edit_topic.Checked)
        m_check_edit_resource.Checked = false;
    }
  }
}