using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Help;
using ATHelpEditor;
using AT.Toolbox.Settings;
using System.ComponentModel;
using AT.Toolbox.Dialogs;

namespace ATHelpEditor
{
  static class Program
  {
    [ATConfigSection(Name = "HelpEditor", XPath = "HELPEDITOR")]
    public class Settings : SettingBase
    {
      private BindingList<TextBlockStyle> textBlockStyles = new BindingList<TextBlockStyle>();
      
      [ATConfiguration(Name = "TemplateFolder", Default = @"HelpTmp")]
      public string TemplateFolder { get; set; }

      [ATConfigurationList(Name = "TextBlockStyles", ItemType = typeof(TextBlockStyle))]
      public BindingList<TextBlockStyle> TextBlockStyles
      {
        get { return textBlockStyles; }
      }
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    private static readonly ATConfigSection conf_section = new ATConfigSection { Name = @"HelpEditor", XPath = @"HELPEDITOR" };

    /// <summary>
    /// Настройки 
    /// </summary>
    public static Settings Preferences
    {
      get
      {
        if (null == AppInstance.Configurator[conf_section])
          AppInstance.Configurator[conf_section] = new Settings();

        return AppInstance.Configurator[conf_section] as Settings;
      }
    }
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      AppWrapper.Run<MainForm>(true);
    }
  }
}
