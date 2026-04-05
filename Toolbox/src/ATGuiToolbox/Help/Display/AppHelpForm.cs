using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using System.Collections.Generic;
using AT.Toolbox.Base;
using AT.Toolbox.Properties;
using AT.Toolbox.Dialogs;

namespace AT.Toolbox.Help
{
  public partial class AppHelpForm : LocalizableForm
  {
    private HelpFile m_file;
    public string InitialURL;

    List<string> backwardBuffer = new List<string>();
    List<string> forwardBuffer = new List<string>();
    string current_page;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HelpFile File
    {
      get { return m_file; }
      set
      {
        m_help_browser.OnHelpNavigate += new EventHandler<ParamEventArgs<string>>(HandleHelpNavigation);
        m_help_navigator.Nodes = value.Nodes;
        m_file = value;
      }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      this.Text = Resources.HELP_BROWSER_TITLE;
      m_cmd_about.Caption = Resources.ABOUT_TITLE;
      m_cmd_settings.Caption = Resources.SETTINGS;
    }

    private void HandleHelpNavigation(object sender, ParamEventArgs<string> e)
    {
      if (null == m_file)
        return;

      string val = e.Value.Replace("/", "");
      
      forwardBuffer.Clear();
      if (current_page != null)
      {
        backwardBuffer.Add(current_page);
      }
      current_page = val;
      this.ChangeCurrentPage(val);
    }

    public AppHelpForm()
    {
      InitializeComponent();
      m_help_navigator.OnHelpOpen += HandleHelpNavigation;
    }

    private void HandleFormShown(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(InitialURL))
        m_help_browser.Open(InitialURL);
    }

    private void ChangeCurrentPage(string val)
    {
       
      if (val.Contains("#"))
      {
        string[] strings = val.Split("#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        m_file.Deflate(strings[0], ApplicationSettings.Instance.TempPath);

        string str = Path.Combine(ApplicationSettings.Instance.TempPath, "index.html") + "#" + strings[1];
        m_help_browser.Open(str);
      }
      else
      {
        m_file.Deflate(val, ApplicationSettings.Instance.TempPath);
        string str = Path.Combine(ApplicationSettings.Instance.TempPath, "index.html");
        m_help_browser.Open(str);
      }
      m_cmd_back.Enabled = backwardBuffer.Count > 0;
      m_cmd_forward.Enabled = forwardBuffer.Count > 0;
    }

    private void m_cmd_back_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (backwardBuffer.Count > 0)
      {
        string val = backwardBuffer[backwardBuffer.Count - 1];
        backwardBuffer.RemoveAt(backwardBuffer.Count - 1);
        forwardBuffer.Add(current_page);
        current_page = val;
        this.ChangeCurrentPage(val);
      }
    }

    private void m_cmd_forward_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (forwardBuffer.Count > 0)
      {
        string val = forwardBuffer[forwardBuffer.Count - 1];
        forwardBuffer.RemoveAt(forwardBuffer.Count - 1);
        backwardBuffer.Add(current_page);
        current_page = val;
        this.ChangeCurrentPage(val);
      }
    }

    private void m_cmd_settings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SettingsForm sf = new SettingsForm();
      sf.ShowDialog(this);
    }

    private void m_cmd_about_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AboutBox box = new AboutBox();
      box.ShowDialog(this);
    }
  }
}