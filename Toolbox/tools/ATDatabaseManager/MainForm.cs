using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Help;
using AT.Toolbox.MSSQL;
using AT.Toolbox.MSSQL.DbMould;
using ATDatabaseManager.Properties;
using DevExpress.XtraBars;

namespace ATDatabaseManager
{
  public partial class MainForm : LocalizableForm
  {
    public MainForm()
    {
      InitializeComponent();
      m_database_browser.RoutineHandler = new DbMouldRoutines();
      m_database_browser.RoutineHandler.MetaDataFile = DatabaseBrowserControl.Preferences.MouldFilePath;
      m_cmd_edit.Visibility = DatabaseBrowserControl.Preferences.DeveloperMode ? BarItemVisibility.Always : BarItemVisibility.Never;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      m_text_current_connection.Caption = m_database_browser.CurrentDatabase;
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      this.Text = Resources.DatabaseManager;
      m_cmd_settings.Caption = Resources.Settings;
      m_cmd_select_mould.Caption = Resources.SelectMould;
      m_cmd_save.Caption = Resources.SaveMould;
      m_cmd_edit.Caption = Resources.EditMould;
      m_cmd_about.Caption = Resources.About;
      m_cmd_help.Caption = Resources.Help;
      m_menu_help.Caption = Resources.Help;
    }

    private void m_cmd_settings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SettingsForm sf = new SettingsForm();

      sf.ShowDialog(this);
    }

    private void m_cmd_en_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AppSwitchablePool.SwitchLocale(new CultureInfo("EN"));
    }

    private void m_cmd_ru_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AppSwitchablePool.SwitchLocale(new CultureInfo("RU"));
    }

    private void m_database_browser_EditValueChanged(object sender, EventArgs e)
    {
      m_text_current_connection.Caption = m_database_browser.CurrentDatabase;
    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();

      dlg.Filter = "Xml files|*.xml";
      dlg.Title = Resources.SelectMould;
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_database_browser.RoutineHandler.MetaDataFile = dlg.FileName;
        m_database_browser.RoutineHandler.ReloadMetaData();
      }
    }

    private void m_cmd_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SaveFileDialog dlg = new SaveFileDialog();

      dlg.Filter = "Xml files|*.xml";
      dlg.Title = Resources.SaveMould;
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_database_browser.RoutineHandler.CurrentMould.Save(dlg.FileName);
      }
    }

    private void m_cmd_help_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      try
      {
        ApplicationHelp.Preferences.HelpFilePath = Path.Combine(Application.StartupPath, ApplicationHelp.Preferences.HelpFilePath);
        ApplicationHelp.ShowFullHelp();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, Resources.DatabaseManager, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void m_cmd_about_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AboutBox box = new AboutBox();
      box.ShowDialog(this);
    }

    private void m_cmd_edit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      MouldEditForm form = new MouldEditForm();
      form.Mould = m_database_browser.RoutineHandler.CurrentMould;
      form.MouldFilePath = m_database_browser.RoutineHandler.MetaDataFile;
      form.Show(this);
    }
  }
}
