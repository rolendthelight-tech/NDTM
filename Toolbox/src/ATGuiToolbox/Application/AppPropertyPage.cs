using System;
using System.Collections.Generic;
using System.Drawing;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Properties;
using AT.Toolbox.Settings;

namespace AT.Toolbox
{
  using Base;

  public partial class AppPropertyPage : PropertyPage 
  {
    private bool m_restart_required;

    private readonly SettingsBindingSource<ApplicationSettings> m_app_settings;
    private readonly SettingsBindingSource<ErrorBox.Settings> m_err_settings;

    public AppPropertyPage()
    {
      InitializeComponent();

      m_app_settings = new SettingsBindingSource<ApplicationSettings>(settingsBindingSource);
      m_err_settings = new SettingsBindingSource<ErrorBox.Settings>(errorBoxBindingSource);

      InitLocalization();
    }

    public override string Group
    {
      get { return Resources.SETTINGS_GENERIC; }
    }

    public override string PageName
    {
      get { return Resources.APPLICATION_SETTINGS; }
    }

    public override Image Image
    {
      get { return Resources.p_32_app_config; }
    }

    public override bool RestartRequired
    {
      get { return m_restart_required; }
    }


    public override void ApplySettings()
    {
      settingsBindingSource.EndEdit(  );
      errorBoxBindingSource.EndEdit();

      m_app_settings.AssignSettings();
      m_err_settings.AssignSettings();

      AppSwitchablePool.SwitchLocale(ApplicationSettings.Instance.DefaultLocale);
    }

    public override void GetCurrentSettings()
    {
      m_app_settings.GetCurrentSettings();
      OnPageSettings.Add(m_app_settings.EditingSection);

      m_err_settings.GetCurrentSettings();
      OnPageSettings.Add(m_err_settings.EditingSection);
    }

    private string GetLocale(string s)
    {
      return s == Resources.LOCALE_ENGLISH ? "En" : "Ru";
    }

    private object GetLocaleName(string locale)
    {
      return locale.ToLower() == "en" ? Resources.LOCALE_ENGLISH : Resources.LOCALE_RUSSIAN;
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_locale_label.Text = Resources.LOCALE;
      m_allow_one_instance.Text = Resources.ALLOW_ONE_INSTANCE;
      m_restart_on_critical.Text = Resources.RESTART_ON_CRITICAL;
      m_dont_close.Text = Resources.DONT_CLOSE;

      var items = new List<KeyValuePair<string, string>>();
      items.Add(new KeyValuePair<string, string>("EN", Resources.LOCALE_ENGLISH));
      items.Add(new KeyValuePair<string, string>("RU", Resources.LOCALE_RUSSIAN));
      m_locale.Properties.DataSource = items;
    }

    private void HandleLocaleParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
      if (null == e.Value)
        return;

      if (e.Value.ToString().ToLower() == "en" || e.Value.ToString().ToLower() == "ru" )
        return;

      e.Value = GetLocale(e.Value.ToString());
      e.Handled = true;
    }

    private void HandleLocaleFormatEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
      if (null == e.Value)
        return;

      e.Value = GetLocaleName(e.Value.ToString());
      e.Handled = true;
    }

    private void HandleAllowOneInstanceChanged(object sender, EventArgs e)
    {
      m_restart_required = true;
    }

    private void HandleRestartOnCriticalChanged(object sender, EventArgs e)
    {
      m_restart_required = true;
    }
  }
}