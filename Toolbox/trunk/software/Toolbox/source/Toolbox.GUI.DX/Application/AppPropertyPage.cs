using System;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.GUI.Base;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Properties;

namespace Toolbox.GUI.DX.Application
{
	public partial class AppPropertyPage : PropertyPage
	{
		private const string LocaleEnglish = "En";
		private const string LocaleRussian = "Ru";

    private bool m_restart_required;

		[NotNull] private readonly SettingsBindingSource<ApplicationSettings> m_app_settings;
		[NotNull] private readonly SettingsBindingSource<ErrorBox.Settings> m_err_settings;

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

		private string GetLocale([NotNull] string s)
		{
			if (s == null) throw new ArgumentNullException("s");

			if (s == Resources.LOCALE_ENGLISH) return LocaleEnglish;
			else if (s == Resources.LOCALE_RUSSIAN) return LocaleRussian;
			else throw new NotImplementedException(s);
		}

		private object GetLocaleName(string locale)
		{
			if (locale == LocaleEnglish) return Resources.LOCALE_ENGLISH;
			else if (locale == LocaleRussian) return Resources.LOCALE_RUSSIAN;
			else throw new NotImplementedException(locale);
		}

		public override void PerformLocalization(object sender, EventArgs e)
    {
      m_locale_label.Text = Resources.LOCALE;
      m_allow_one_instance.Text = Resources.ALLOW_ONE_INSTANCE;
      m_restart_on_critical.Text = Resources.RESTART_ON_CRITICAL;
      m_close_on_critical_error.Text = Resources.CLOSE_ON_CRITICAL_ERROR;

      var items = new List<KeyValuePair<string, string>>
	      {
		      new KeyValuePair<string, string>(LocaleEnglish, Resources.LOCALE_ENGLISH),
		      new KeyValuePair<string, string>(LocaleRussian, Resources.LOCALE_RUSSIAN),
	      };
	    m_locale.Properties.DataSource = items;
    }

		private static HashSet<string> _langs = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase) { LocaleEnglish, LocaleRussian, };

    private void HandleLocaleParseEditValue([NotNull] object sender, [NotNull] DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			if (null == e.Value)
        return;

			if (_langs.Contains(e.Value.ToString()))
        return;

      e.Value = GetLocale(e.Value.ToString());
      e.Handled = true;
    }

    private void HandleLocaleFormatEditValue([NotNull] object sender, [NotNull] DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

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