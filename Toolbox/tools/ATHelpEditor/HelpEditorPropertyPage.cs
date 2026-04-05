using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using AT.Toolbox.Settings;
using ATHelpEditor.Properties;
using System.IO;

namespace ATHelpEditor
{
  public partial class HelpEditorPropertyPage : PropertyPage 
  {
    public HelpEditorPropertyPage()
    {
      InitializeComponent();
    }

    #region IPropertyPage Members

    public override string Group
    {
      get { return Resources.SETTINGS_PAGE_GROUP; }
    }

    public override string PageName
    {
      get { return Resources.SETTINGS_PAGE_NAME; }
    }

    public override Image Image
    {
      get { return Resources._24_book_blue_add; }
    }

    public override bool RestartRequired
    {
      get { return false; }
    }

    public override  Guid ID
    {
      get { return new Guid("B16FC4E6-E81B-45f6-BA17-07B900409AA0"); }
    }

    public override  Image GroupImage
    {
      get { return Resources._48_help; }
    }

    public override void ApplySettings()
    {
      Program.Preferences.TemplateFolder = (string)m_edit_template_folder.EditValue;
    }

    public override void GetCurrentSettings()
    {
      m_edit_template_folder.EditValue = Path.Combine(Application.StartupPath, Program.Preferences.TemplateFolder);
    }

    public void BeforeSettingsFormClosing() { }

    #endregion

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      m_label_template_folder.Text = Resources.SETTINGS_PAGE_PATH;
    }

    private void m_edit_template_folder_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      FolderBrowserDialog dlg = new FolderBrowserDialog();
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_edit_template_folder.EditValue = dlg.SelectedPath;
        this.Changed = true;
      }
    }
  }
}
