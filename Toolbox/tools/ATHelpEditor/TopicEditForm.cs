using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using ATHelpEditor.Properties;

namespace ATHelpEditor
{
  public partial class TopicEditForm : LocalizableForm
  {
    public TopicEditForm()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      this.Text = Resources.TOPIC_EDITOR;
      m_label_topic_id.Text = Resources.TOPIC_EDITOR_ID;
      m_label_topic_name.Text = Resources.TOPIC_EDITOR_NAME;
      m_ok_btn.Text = Resources.BUTTON_OK;
      m_cancel_btn.Text = Resources.BUTTON_CANCEL;
    }

    public string TopicName
    {
      get
      {
        return null == m_topic_name_edit.EditValue ? "" : m_topic_name_edit.EditValue.ToString();
      }
      set
      {
        m_topic_name_edit.EditValue = value;
      }
    }

    public string TopicID
    {
      get 
      {
        return null == m_topic_ID_edit.EditValue ? "" : m_topic_ID_edit.EditValue.ToString();
      }
      set
      {
        m_topic_ID_edit.EditValue = value;
      }
    }
  }
}