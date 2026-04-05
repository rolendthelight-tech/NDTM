using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using log4net;

namespace AT.Toolbox
{
  /// <summary>
  /// Интерфейс для пометки объектов, зависящих от инициализации приложения
  /// </summary>
  public interface IAppInstanceInitializer { }
  
  public class AppInstanceInitializer : IApplicationLoader
  {
    private static readonly ILog _log = LogManager.GetLogger("AppInstanceInitializer");

    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
      context.Container.SetService<IAppInstanceInitializer>(new Stub());

      context.Worker.ReportProgress(0, "Инициализация приложения");

      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        ApplicationInfo.MainAttributes.Company + "\\" + ApplicationInfo.MainAttributes.ProductName + "\\" +
        ApplicationInfo.MainAttributes.Version);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      foreach (string s in Directory.GetDirectories(path))
      {
        try
        {
          Guid id = new Guid(Path.GetFileName(s));
          Directory.Delete(s, true);
        }
        catch (Exception ex)
        {
          _log.Error("PrepareWorkingFolder(): exception", ex);
        }
      }

      string guid = Guid.NewGuid().ToString();
      ApplicationSettings.Instance.TempPath = Path.Combine(path, guid);

      if (Directory.Exists(ApplicationSettings.Instance.TempPath))
        Directory.Delete(ApplicationSettings.Instance.TempPath, true);

      Directory.CreateDirectory(ApplicationSettings.Instance.TempPath);

      this.UpdateAppInstance();

      //AppInstance_.WorkingFolder = path;
      //AppInstance_.ConfigFileFinder = new ConfigFileFinder(null, "Settings.xml");
      //AppInstance_.ReadRegistryAndCommandLineSettings(context.CommandArgs);

      //if (context.Invoker.InvokeRequired)
      //  context.Invoker.Invoke(new Action(AppInstance_.CreateBlockingMutex), new object[0]);
      //else
      //  AppInstance_.CreateBlockingMutex();
      //AppInstance_.SkipGlobalHandler = true;
      //AppManager.Instance.CreateBlockingMutex();

      //return AppInstance_.Init(context.CommandArgs, 
      this.Init(context.Invoker);

      return true;
    }

    private void Init(ISynchronizeInvoke invoker)
    {
      if (invoker.InvokeRequired)
        invoker.Invoke(new Action(() => AppSwitchablePool.RegisterThread(@"MAIN", Thread.CurrentThread)), new object[0]);
      else
        AppSwitchablePool.RegisterThread(@"MAIN", Thread.CurrentThread);

      if (invoker.InvokeRequired)
        invoker.Invoke(new Action(() =>
          AppSwitchablePool.SwitchLocale(new CultureInfo(ApplicationSettings.Instance.DefaultLocale))), new object[0]);
      else
        AppSwitchablePool.SwitchLocale(new CultureInfo(ApplicationSettings.Instance.DefaultLocale));

      AppManager.AssemblyClassifier.TreeBuilder.Collect();

      if (this.LoadPlugins)
      {
        if (string.IsNullOrEmpty(PluginFilenameRegex))
          AppManager.AssemblyClassifier.FindAndLoadPlugins();
        else
          AppManager.AssemblyClassifier.FindAndLoadPlugins(PluginFilenameRegex);
      }
    }

    public bool LoadPlugins { get; set; }

    public string PluginFilenameRegex { get; set; }

    protected virtual void UpdateAppInstance()
    {
      var exclude = AppManager.AssemblyClassifier.TreeBuilder.ExcludePrefixes;

      exclude.Add("DevExpress");
      exclude.Add("Microsoft");
      exclude.Add("System");
      exclude.Add("mscorlib");
      exclude.Add("WindowsBase");
    }

    #endregion

    #region IDependencyItem<Type> Members

    public Type Key
    {
      get { return typeof(IAppInstanceInitializer); }
    }

    public IList<Type> MandatoryDependencies
    {
      get { return new Type[0]; }
    }

    public IList<Type> OptionalDependencies
    {
      get { return new Type[0]; }
    }

    private class Stub : IAppInstanceInitializer { }

    #endregion
  }

  public abstract class ConfigurationSectionInitializer : IApplicationLoader
  {
    private readonly HashSet<Type> m_config_section_types = new HashSet<Type>();

    protected void AddSectionType<TSection>()
      where TSection : ConfigurationSection, new()
    {
      m_config_section_types.Add(typeof(TSection));
    }

    protected abstract void AddSectionTypes();

    #region IComponentLoader Members

    public bool Load(LoadingContext context)
    {
      this.AddSectionTypes();

      bool ret = true;

      foreach (var type in m_config_section_types)
      {
        InfoBuffer buffer = new InfoBuffer();

        if (!AppManager.Configurator.LoadSection(type, buffer))
        {
          ret = false;

          foreach (var info in buffer)
          {
            var cpy = context.Buffer.Add(type.ToString(), info.Message, info.Level);

            cpy.Details = info.Details;

            foreach (var inner in info.InnerMessages)
              cpy.InnerMessages.Add(inner);
          }
        }
        else
        {
          var method = typeof(IConfigurator).GetMethod("GetSection").MakeGenericMethod(type);
          ((ConfigurationSection)method.Invoke(AppManager.Configurator, null)).ApplySettings();
        }
      }

      if (ret)
        context.Container.SetService(AppManager.Configurator);

      return ret;
    }

    #endregion

    #region IDependencyItem<Type> Members

    public Type Key
    {
      get { return typeof(IConfigurator); }
    }

    public IList<Type> MandatoryDependencies
    {
      get { return new Type[0]; }
    }

    public IList<Type> OptionalDependencies
    {
      get { return new Type[] { typeof(IAppInstanceInitializer) }; }
    }

    #endregion
  }

}
