using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using AT.Toolbox.Base;
using System.IO;
using AT.Toolbox.MSSQL.Browsing;

namespace AT.Toolbox.MSSQL
{
  public partial class BuildMouldForm : LocalizableForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BuildMouldForm).Name);

    //DatabaseMould m_mould;
    string m_mould_path;

    public BuildMouldForm()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      this.Text = Properties.Resources.DB_BROWSER_MOULD;
      this.m_button_cancel.Text = Properties.Resources.CANCEL;
      this.m_label_new_version.Text = Properties.Resources.BUILD_MOULD_NEW_VERSION;
      this.m_label_old_version.Text = Properties.Resources.BUILD_MOULD_OLD_VERSION;
      this.m_label_old_mould.Text = Properties.Resources.BUILD_MOULD_OLD_MOULD;
      this.m_label_new_mould.Text = Properties.Resources.BUILD_MOULD_NEW_MOULD;
    }

    public string OldVersion
    {
      get { return m_old_version_edit.Text; }
      set { m_old_version_edit.Text = value; }
    }

    public string MouldPath
    {
      get { return m_mould_path; }
      set
      {
        m_mould_path = value;

        if (this.RoutineHandler != null && !string.IsNullOrEmpty(m_mould_path)
          && File.Exists(m_mould_path))
        {
          m_mould_path = Path.GetFullPath(m_mould_path);

          m_old_mould_edit.Text = this.RoutineHandler.GetMetadataInfo(m_mould_path);
        }
        else
        {
          m_old_mould_edit.Text = "";
        }
      }
    }

    public ISpecificDBRoutines RoutineHandler { get; set; }

    public string NewVersion
    {
      get { return m_new_version_edit.Text; }
      set { m_new_version_edit.Text = value; }
    }

    public string NewMould
    {
      get { return m_new_mould_edit.Text; }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);
      List<string> messageBuffer = new List<string>();
      bool cancelRequired = false;

      if (this.DialogResult == DialogResult.OK)
      {
        if (VersionHelper.CompareVersions(m_new_version_edit.Text, m_old_version_edit.Text) <= 0)
        {
          messageBuffer.Add(Properties.Resources.BUILD_MOULD_INCORRECT_VERSION);
        }
        if (string.IsNullOrEmpty(this.NewMould))
        {
          messageBuffer.Add(Properties.Resources.BUILD_MOULD_NO_PATH);
          cancelRequired = true;
        }

        if (messageBuffer.Count > 0)
        {
          if (MessageBox.Show(string.Join(Environment.NewLine, messageBuffer.ToArray()),
            Properties.Resources.DB_BROWSER_MOULD, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
            == DialogResult.Cancel || cancelRequired)
          {
            e.Cancel = true;
          }
        }
      }
    }

    private void m_new_version_edit_Validating(object sender, CancelEventArgs e)
    {
      try
      {
        VersionHelper.CompareVersions("0.0.0.0", m_new_version_edit.Text);
      }
      catch (Exception ex)
      {
        Log.Error("m_new_version_edit_Validating(): exception", ex);
        MessageBox.Show(ex.Message, Properties.Resources.DB_BROWSER_MOULD, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        e.Cancel = true;
      }
    }

    private void m_path_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      SaveFileDialog dlg = new SaveFileDialog();
      dlg.Filter = "Database moulds|*.mould";
      dlg.InitialDirectory = Path.GetDirectoryName(m_mould_path);
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        if (!dlg.FileName.ToLower().EndsWith(".mould"))
          dlg.FileName += ".mould";
        
        m_new_mould_edit.Text = dlg.FileName;
        m_new_mould_edit.ToolTip = dlg.FileName;
      }
    }
  }
}