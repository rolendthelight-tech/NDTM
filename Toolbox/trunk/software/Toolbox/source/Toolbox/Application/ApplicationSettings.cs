using System.Runtime.Serialization;
using Toolbox.Application.Services;
using Toolbox.Configuration;

namespace Toolbox.Application
{
  [DataContract]
  public class ApplicationSettings : ConfigurationSection
  {
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

    string m_default_locale = "Ru";

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