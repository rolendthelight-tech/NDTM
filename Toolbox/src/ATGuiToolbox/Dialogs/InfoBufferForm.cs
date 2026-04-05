using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Dialogs
{
  public partial class InfoListForm : DialogBase
  {
    public InfoListForm()
    {
      InitializeComponent();
    }

    public void Accept(InfoBuffer buffer)
    {
      m_lister.Accept(buffer);

      switch (m_lister.Level)
      {
        case InfoLevel.Debug:
          m_picture.Image = Resources.p_48_debug;
          m_label_summary.Text = "";
          break;

        case InfoLevel.Info:
          m_picture.Image = Resources.p_48_info;
          m_label_summary.Text = Resources.SUMMARY_INFO;
          break;

        case InfoLevel.Warning:
          m_picture.Image = Resources.p_48_warning;
          m_label_summary.Text = Resources.SUMMARY_WARNING;
          break;

        case InfoLevel.Error:
          m_picture.Image = Resources.p_48_error;
          m_label_summary.Text = Resources.SUMMARY_ERROR;
          break;
      }
    }

    public void SetSummary(string text)
    {
      m_label_summary.Text = text;
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_group_summary.Text = Resources.INFO_WND;
    }

    private void m_close_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      this.Close();
    }
  }
}
