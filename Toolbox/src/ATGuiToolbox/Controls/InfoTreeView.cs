using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox;
using AT.Toolbox.Dialogs;

namespace AT.Toolbox.Controls
{
  public partial class InfoTreeView : DevExpress.XtraEditors.XtraUserControl
  {
    private InfoLevel m_level = InfoLevel.Debug;

    public InfoTreeView()
    {
      InitializeComponent();
    }

    public InfoLevel Level
    {
      get { return m_level; }
    }

    public void Accept(InfoBuffer buffer)
    {
      if (buffer == null || buffer.Count == 0)
        return;

      m_level = m_entries.Accept(buffer);
      m_tree.ExpandAll();

      Form frm = this.FindForm();

      if (frm != null)
      {
        var oldHeight = this.Height;

        var newHeight = Math.Min(m_entries.Entries.Count * 21 + 4, frm.Height);

        frm.Height += newHeight - oldHeight;
      }
    }

    private void m_details_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      LogEntry entry = null;
      try
      {
        entry = m_tree.GetDataRecordByNode(m_tree.FocusedNode) as LogEntry;
      }
      catch { }

      if (entry != null)
      {
        if (entry.Tag != null && !string.IsNullOrEmpty(entry.Tag.ToString()))
        {
          MemoEditForm frm = new MemoEditForm();
          frm.Icon = entry.GetLargeIcon();
          frm.Text = entry.Level.GetLabel();
          frm.Lines = Regex.Replace(entry.Tag.ToString(), @"(?>\n\r|\r\n?)", "\n", RegexOptions.Compiled | RegexOptions.Singleline)
              .Split('\n');

          frm.ShowDialog(this);
        }
        else 
        {
          MessageBoxEx mb = new MessageBoxEx();

          mb.Message = entry.Message;
          mb.Caption = entry.Level.GetLabel();
          mb.Buttons = MessageBoxButtons.OK;

          switch (entry.Level)
          {
            case InfoLevel.Info:
            case InfoLevel.Debug:
              mb.StandardIcon = MessageBoxEx.Icons.Information;
              break;

            case InfoLevel.Warning:
              mb.StandardIcon = MessageBoxEx.Icons.Warning;
              break;

            case InfoLevel.Error:
              mb.StandardIcon = MessageBoxEx.Icons.Error;
              break;
          }

          mb.ShowDialog(this);
        }
      }
    }
  }
}
