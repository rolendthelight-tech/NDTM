using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Properties;
using System.Runtime.Serialization;

namespace AT.Toolbox.Help
{
  public class ApplicationHelp
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ApplicationHelp).Name);

    [DataContract]
    public class Settings : ConfigurationSection
    {
      /// <summary>
      /// Путь к файлу протокола.
      /// </summary>
      [DataMember]
      public string HelpFilePath { get; set; }

      /// <summary>
      /// Уровень протоколирования
      /// </summary>
      [DataMember]
      public bool DebugMode { get; set; }
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    public static Settings Preferences
    {
      get
      {
        return AppManager.Configurator.GetSection<Settings>();
      }
    }

    private static HelpBrowser contextBrowser;
    private static HelpFile m_file;

    protected static bool m_filter_set = false;

    protected static void SetFilter()
    {
      if (m_filter_set)
        return;

      HelpMessageFilter filter = new HelpMessageFilter();
      Application.AddMessageFilter(filter);
    }

    protected static void RefreshFile()
    {
      if (Preferences.DebugMode && null != m_file)
      {
        m_file.Close();
        m_file = null;
      }

      if (null == m_file)
      {
        if (string.IsNullOrEmpty(Preferences.HelpFilePath))
        {
          string path = Path.Combine(Application.StartupPath, Application.ProductName) + ".athelp";
          if (File.Exists(path))
            Preferences.HelpFilePath = path;
          else
            throw new ArgumentException(Resources.APPHELP_NOHELPFILE, Preferences.HelpFilePath);
        }

        if (!File.Exists(Preferences.HelpFilePath))
          throw new FileNotFoundException(string.Format("{0} \"{1}\"", Resources.APPHELP_CANT_OPEN_HELPFILE, Preferences.HelpFilePath), Preferences.HelpFilePath);

        m_file = new HelpFile();
        m_file.Open(Preferences.HelpFilePath);
      }
    }

    public static string Locale { get; set; }

    public static HelpBrowser ContextBrowser
    {
      get { return contextBrowser; }
      set
      {
        if (null == value)
          throw new ArgumentNullException("value");

        contextBrowser = value;

        contextBrowser.OnHelpNavigate += HandleHelpNavigate;
      }
    }

    private static void HandleHelpNavigate(object sender, ParamEventArgs<string> e)
    {
      RefreshFile();

      if (null == m_file)
        return;

      string val = e.Value.Replace(@"/", @"");

      if (val.Contains(@"#"))
      {
        string[] strings = val.Split(@"#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        m_file.Deflate(strings[0], ApplicationSettings.Instance.TempPath);

        string str = Path.Combine(ApplicationSettings.Instance.TempPath, @"index.html") + @"#" + strings[1];
        contextBrowser.Open(str);
      }
      else
      {
        m_file.Deflate(val, ApplicationSettings.Instance.TempPath);

        string str = Path.Combine(ApplicationSettings.Instance.TempPath, @"index.html");
        contextBrowser.Open(str);
      }
    }

    public static AppHelpForm GetHelpForm()
    {
      RefreshFile();

      AppHelpForm frm = new AppHelpForm();
      frm.File = m_file;
      return frm;
    }

    /// <summary>
    /// Отображает файл справки в отдельном окне
    /// </summary>
    /// <returns>Возвращает true, если отображение не привело к исключению, иначе false</returns>
    public static bool ShowFullHelp()
    {
      try
      {
        GetHelpForm().Show();
        return true;
      }
      catch(Exception ex)
      {
        Log.Error("ShowFullHelp(): exception", ex);
        return false;
      }
    }

    protected static Dictionary<Component, string> m_map = new Dictionary<Component, string>();

    public static void AppendMap(HelpMapper mapper)
    {
      SetFilter();

      if (null == mapper.HelpMap)
        return;

      foreach (HelpEntry entry in mapper.HelpMap)
      {
        if (!m_map.ContainsKey(entry.Target))
          m_map.Add(entry.Target, mapper.MainTopic + @"#" + entry.HelpLink);
      }
    }

    public static void GetContextHelp(IHelpSupported ctl, Component cmp)
    {
      if (null == ContextBrowser)
        return;

      RefreshFile();

      if (null != cmp)
      {
        if (!m_map.ContainsKey(cmp))
          return;

        ContextBrowser.Open(@"athelp://" + m_map[cmp]);
      }
      else
        ContextBrowser.Open(@"athelp://" + ctl.HelpMap.MainTopic);
    }

    public static void ShowFullHelp(IHelpSupported parent, Component cmp)
    {
      RefreshFile();

      AppHelpForm frm = new AppHelpForm();
      frm.File = m_file;

      if (null != cmp)
      {
        if (!m_map.ContainsKey(cmp))
          return;

        frm.InitialURL = @"athelp://" + m_map[cmp];
      }
      else
        frm.InitialURL = @"athelp://" + parent.HelpMap.MainTopic;

      frm.ShowDialog();
    }

    public static string GetHelpIDs(IHelpSupported parent, Component cmp)
    {
      if (!m_map.ContainsKey(cmp))
        return @"";

      if (Preferences.DebugMode)
        parent.ShowToolTip(m_map[cmp]);

      return m_map[cmp];
    }
  }
}