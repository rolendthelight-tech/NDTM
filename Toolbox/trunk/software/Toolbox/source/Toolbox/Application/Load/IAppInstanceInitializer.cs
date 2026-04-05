using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using Toolbox.Application.Services;
using Toolbox.Configuration;
using Toolbox.Extensions;
using Toolbox.Log;
using log4net;

namespace Toolbox.Application.Load
{
  /// <summary>
  /// Интерфейс для пометки объектов, зависящих от инициализации приложения
  /// </summary>
  public interface IAppInstanceInitializer { }

  public class AppInstanceInitializer : IApplicationLoader
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(AppInstanceInitializer));

    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
	    if (context == null) throw new ArgumentNullException("context");

			context.Container.SetService<IAppInstanceInitializer>(new Stub());

      context.Worker.ReportProgress(0, "Инициализация приложения");

      this.UpdateAppInstance();

      this.Init(context.Invoker);

      return true;
    }

    private void Init(ISynchronizeInvoke invoker)
    {
	    if (invoker == null) throw new ArgumentNullException("invoker");

			if (invoker.InvokeRequired)
        invoker.Invoke(new Action(() => AppSwitchablePool.RegisterThread(@"MAIN", Thread.CurrentThread)), ArrayExtensions.Empty<object>());
      else
        AppSwitchablePool.RegisterThread(@"MAIN", Thread.CurrentThread);

      if (invoker.InvokeRequired)
        invoker.Invoke(new Action(() =>
          AppSwitchablePool.SwitchLocale(new CultureInfo(ApplicationSettings.Instance.DefaultLocale))), ArrayExtensions.Empty<object>());
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
      get { return Type.EmptyTypes; }
    }

    public IList<Type> OptionalDependencies
    {
      get { return Type.EmptyTypes; }
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
	    if (context == null) throw new ArgumentNullException("context");

			this.AddSectionTypes();

      bool ret = true;

      foreach (var type in m_config_section_types)
      {
        var buffer = new InfoBuffer();

        if (!AppManager.Configurator.LoadSection(type, buffer))
        {
          ret = false;

          foreach (var info in buffer)
          {
            var cpy = context.Buffer.Add(link: type.ToString(), message: info.Message, state: info.Level, details: info.Details);

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
      get { return Type.EmptyTypes; }
    }

    public IList<Type> OptionalDependencies
    {
      get { return new Type[] { typeof(IAppInstanceInitializer) }; }
    }

    #endregion
  }

}
