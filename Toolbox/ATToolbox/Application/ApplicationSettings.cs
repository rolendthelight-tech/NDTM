using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using AT.Toolbox.Extensions;
using AT.Toolbox.Files;
using AT.Toolbox.Log;
using AT.Toolbox.Properties;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace AT.Toolbox
{
  [DataContract]
  public class ApplicationSettings : ConfigurationSection
  {
    /// <summary>
    /// Временная папка для работы
    /// </summary>
    public string TempPath { get; set; }

    /// <summary>
    /// Флаг разрешения запуска нескольких экземпляров приложения.
    /// </summary>
    [DataMember]
    public bool AllowOnlyOneInstance { get; set; }

    /// <summary>
    /// Флаг автоперезапуска в случае критической ошибки
    /// </summary>
    [DataMember]
    public bool AutoRestartOnCriticalFailure { get; set; }

    public bool CustomDrawingSupported { get; set; }

    string m_default_locale = "EN";

    public ApplicationSettings()
    {
      AllowOnlyOneInstance = false;
    }

    [DataMember]
    public string DefaultLocale
    {
      get { return m_default_locale; }
      set
      {
        if (value == m_default_locale)
          return;

        m_default_locale = value;
      }
    }

    [DataMember]
    public bool CloseOnCriticalError { get; set; }

    public static ApplicationSettings Instance
    {
      get { return AppManager.Configurator.GetSection<ApplicationSettings>(); }
    }
  }
}