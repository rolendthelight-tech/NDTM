using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using JetBrains.Annotations;
using Toolbox.Extensions;
using Toolbox.GUI.Controls.Log;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Dialogs.EditForms;
using Toolbox.Application.Services;

namespace Toolbox.GUI.DX.Controls
{
  public partial class InfoTreeView : XtraUserControl
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

    public void Accept([NotNull] InfoBuffer buffer)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

	    if (buffer.Count == 0)
        return;

      m_level = m_entries.Accept(buffer);
      m_tree.ExpandAll();

      Form frm = this.FindForm();

      if (frm != null)
      {
        var old_height = this.Height;

        var new_height = Math.Min(m_entries.Entries.Count * 21 + 4, frm.Height);

        frm.Height += new_height - old_height;
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
	        using (var frm = new MemoEditForm())
	        {
		        frm.Icon = entry.GetLargeIcon();
		        frm.Text = entry.Level.GetLabel();
		        frm.Value = entry.Tag.ToString();
		        frm.Editable = false;

		        frm.ShowDialog(this);
	        }
        }
        else
        {
					using (var mb = new MessageBoxEx())
					{
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
							case InfoLevel.FatalError:
								mb.StandardIcon = MessageBoxEx.Icons.Error;
								break;
						}

						mb.ShowDialog(this);
					}
        }
      }
    }
  }
}
