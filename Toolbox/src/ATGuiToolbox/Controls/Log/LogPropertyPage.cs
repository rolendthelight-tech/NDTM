using System;
using System.Drawing;
using System.Windows.Forms;
using AT.Toolbox.Log;
using AT.Toolbox.Properties;
using AT.Toolbox.Settings;

namespace AT.Toolbox.Controls
{
  using Base;
  using System.Collections.Generic;

  public partial class LogPropertyPage : PropertyPage 
  {
    private readonly SettingsBindingSource<LogListerSettings> m_log_settings;

    public LogPropertyPage()
    {
      InitializeComponent();

      m_log_settings = new SettingsBindingSource<LogListerSettings>(m_log_lister_settings);
    }

    #region IPropertyPage Members

    public override string Group
    {
      get { return Resources.SETTINGS_GENERIC; }
    }

    public override string PageName
    {
      get { return Resources.LOG; }
    }

    public override Image Image
    {
      get { return Resources.p_32_log_config; }
    }

    public override void ApplySettings()
    {
      try
      {
        
        // Настройки отображения
      
        m_log_lister_settings.EndEdit();

        m_log_settings.AssignSettings();
      }
      catch (Exception ex)
      {
        Log.Error("ApplySettings(): exception", ex);
      }
    }

    public override void GetCurrentSettings()
    {
      try
      {
        m_log_settings.GetCurrentSettings();
        this.OnPageSettings.Add(m_log_settings.EditingSection);
      }
      catch (Exception ex)
      {
        Log.Error("GetCurrentSettings(): exception", ex);
      }
    }
    
    #endregion

    
    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_display_group.Text = Resources.SETTINGS_LOG_DISPLAY;

   
      m_label_recent_count2.Text = Resources.SETTINGS_LOG_RECENT_COUNT;

      m_level_label.Text = Resources.LOG_MIN_LEVEL;
      m_debug_label.Text = Resources.SETTINGS_LOG_DISPLAY_DEBUG_COLOR;
      m_error_label.Text = Resources.SETTINGS_LOG_DISPLAY_ERROR_COLOR;
      m_info_label.Text = Resources.SETTINGS_LOG_DISPLAY_INFO_COLOR;
      m_warning_label.Text = Resources.SETTINGS_LOG_DISPLAY_WARN_COLOR;

      var labels = new List<KeyValuePair<InfoLevel, string>>();

      foreach (InfoLevel value in Enum.GetValues(typeof(InfoLevel)))
      {
        labels.Add(new KeyValuePair<InfoLevel, string>(value, value.GetLabel()));
      }

      m_level_edit.Properties.DataSource = labels;
    }

    private void m_log_level_edit_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
      if (null == e.Value)
        return;

      e.Handled = true;

      if( e.Value.ToString() == Resources.LOG_LEVEL_INFO ) 
          e.Value = "info";

      if (e.Value.ToString() == Resources.LOG_LEVEL_WARN)
        e.Value = "warn";

      if (e.Value.ToString() == Resources.LOG_LEVEL_ERROR)
        e.Value = "error";
      else 
        e.Value = "all";

    }

    private void m_log_level_edit_FormatEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
    {
      if (null == e.Value)
        return;

      e.Handled = true;

      switch (e.Value.ToString().ToLower())
      {
        case "info":
          e.Value = Resources.LOG_LEVEL_INFO;
          break;
        case "warn":
          e.Value = Resources.LOG_LEVEL_WARN;
          break;
        case "error":
          e.Value = Resources.LOG_LEVEL_ERROR;
          break;
        case "all":
          e.Value = Resources.LOG_LEVEL_ALL;
          break;
        default:
          e.Handled = false;
          break;
      }
    }

    private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
  }
}