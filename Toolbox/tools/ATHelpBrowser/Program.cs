using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using AT.Toolbox.Help;
using AT.Toolbox;
using AT.Toolbox.Base;
using System.IO;
using AT.Toolbox.Dialogs;
using ERMS.Core.Common;
using ERMS.UI;

namespace ATHelpBrowser
{
  static class Program
  {
    private static string[] _args;

    [DataContract]
    public class HelpBrowserSettings : ConfigurationSection
    {
      public override void ApplySettings()
      {
        ApplicationHelp.Preferences.HelpFilePath = (_args.Length > 0) ? _args[0] : null;
        AppInstance.MainForm = ApplicationHelp.GetHelpForm();
      }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
      _args = args;

      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        ApplicationInfo.MainAttributes.Company + "\\" + ApplicationInfo.MainAttributes.ProductName + "\\" +
        ApplicationInfo.MainAttributes.Version);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      AppInstance.ConfigFileFinder = new ConfigFileFinder(args, "Settings.xml");
      //AppInstance.ConfigFilePath = path + "\\settings.xml";
      AppInstance.WorkingFolder = path;

      AppWrapper.AddSectionType<HelpBrowserSettings>();
      AppWrapper.Run<LocalizableForm>(args, true);

      return 0;
    }
  }
}
