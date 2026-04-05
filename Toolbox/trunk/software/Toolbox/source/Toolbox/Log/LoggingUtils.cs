using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Xml;
using JetBrains.Annotations;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Toolbox.Application;
using Toolbox.Properties;

namespace Toolbox.Log
{
  public static class LoggingUtils
  {
    // Две перегруженные версии InitializeLogging: без параметров (содержимое конфига берём из ресурсов),
    // с параметром — явная передача содержимого конфига.
    // Константы для работы с ClientInfo.
    public const string UserNameKey = "userName";
    public const string AppNameKey = "appName";
    public const string MachineKey = "machineName";

    public static string _configDir;

    public static log4net.ILog InitializeLogging([CanBeNull] string[] commandLineArgs)
    {
      return InitializeLogging(Resources.Log4netConfig, commandLineArgs);
    }

    public static log4net.ILog InitializeLogging([NotNull] string log4netConfig, [CanBeNull] string[] commandLineArgs)
    {
	    if (log4netConfig == null) throw new ArgumentNullException("log4netConfig");

	    LoggingUtils.InitializeInner(log4netConfig);
      var log = log4net.LogManager.GetLogger(typeof(ApplicationInfo).Name);

      AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
      {
        var ex = e.ExceptionObject as Exception;

        if (ex != null)
          LogManager.GetLogger("AppFramework").Fatal("AppDomain.CurrentDomain.UnhandledException(): ", ex);
        else if (e.ExceptionObject != null)
					LogManager.GetLogger("AppFramework").FatalFormat("AppDomain.CurrentDomain.UnhandledException(): {0}", e.ExceptionObject);
				else
					LogManager.GetLogger("AppFramework").Fatal("AppDomain.CurrentDomain.UnhandledException(): null");
      };

      LoggingUtils.WriteStartLogEntry(log, ApplicationInfo.MainAttributes.ProductName, commandLineArgs);

      return log;
    }

    public static void WriteStartLogEntry([NotNull] log4net.ILog log, [CanBeNull] string appName, [CanBeNull] string[] args)
    {
	    if (log == null) throw new ArgumentNullException("log");

	    AssemblyName assembly_name = (Assembly.GetEntryAssembly() ?? AppDomain.CurrentDomain.GetAssemblies()[0]).GetName();

			log.InfoFormat(@"{0} ({1}) started at {2:yyyy-MM-dd HH:mm:ss} UTC", appName, assembly_name.FullName, DateTime.UtcNow);
      var identity = WindowsIdentity.GetCurrent();
      string identityStr = identity != null ? identity.Name : "unknown";
      log.InfoFormat("Started under {0} account on {1}", identityStr, Environment.MachineName);

      if (args != null)
        log.InfoFormat("Command line arguments ({1}) are:{2}{0}", string.Join("\n\t", args), args.Length, args.Length > 0 ? "\n\t" : "");
      else
        log.InfoFormat("Command line arguments is null");
		}

    private static void InitializeInner([NotNull] string log4netConfig)
    {
	    if (log4netConfig == null) throw new ArgumentNullException("log4netConfig");

	    var log4netConfigFilePath = Path.Combine(ConfigDirectory, ApplicationInfo.MainAttributes.ProductTitle + ".log.config");
      PrepareConfigFile(log4netConfigFilePath, log4netConfig);
      ConfigureLog4net(log4netConfigFilePath);
    }

		private static void PrepareConfigFile([NotNull] [PathReference] string log4netConfigFilePath, [NotNull] string log4netConfig)
    {
	    if (log4netConfigFilePath == null) throw new ArgumentNullException("log4netConfigFilePath");
	    if (log4netConfig == null) throw new ArgumentNullException("log4netConfig");

	    if (!File.Exists(log4netConfigFilePath))
      {
        string dir = Path.GetDirectoryName(log4netConfigFilePath);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        File.WriteAllText(log4netConfigFilePath, log4netConfig);
      }
    }

		private static void ConfigureLog4net([NotNull] [PathReference] string log4netConfigFilePath)
	  {
		  if (log4netConfigFilePath == null) throw new ArgumentNullException("log4netConfigFilePath");

		  try
      {
        string relativeAppDataDir = Path.Combine(
          ApplicationInfo.MainAttributes.Company ?? "",
          ApplicationInfo.MainAttributes.ProductName);

        GlobalContext.Properties["appDataDir"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), relativeAppDataDir);
        GlobalContext.Properties[AppNameKey] = ApplicationInfo.MainAttributes.ProductTitle;

        var log4netConfig = new FileInfo(log4netConfigFilePath);
        if (log4netConfig.Exists)
        {
          Trace.WriteLine(string.Format("{0}: using configuration file '{1}'", ApplicationInfo.MainAttributes.ProductName, log4netConfigFilePath));
          XmlConfigurator.ConfigureAndWatch(log4netConfig);
        }
        else
        {
          Trace.WriteLine(string.Format("{0}: log4net configuration file not found, configuring by default", ApplicationInfo.MainAttributes.ProductName));
          var appender = new OutputDebugStringAppender
          {
						Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline%exception")
          };

          BasicConfigurator.Configure(appender);
        }

	      using (var process = Process.GetCurrentProcess())
	      {
		      GlobalContext.Properties["pid"] = process.Id;
		      GlobalContext.Properties[UserNameKey] = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
		      GlobalContext.Properties[MachineKey] = Environment.MachineName;
		      GlobalContext.Properties["startTimeUtc"] = process.StartTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
		      GlobalContext.Properties["appDomainId"] = AppDomain.CurrentDomain.Id;
	      }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(string.Format("{0}: ERROR - failed to initialize logging, exception: {1}", ApplicationInfo.MainAttributes.ProductName, ex));
      }
	  }

	  [NotNull] private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(LoggingUtils));

	  [NotNull]
	  public static string ConfigDirectory
    {
      get
      {
        if (_configDir == null)
        {
          string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

          try
          {
            string configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (File.Exists(configFilePath))
            {
              Trace.WriteLine(string.Format("ConfigDirectory.get(): application configuration file found at '{0}'", configFilePath));
              var doc = new XmlDocument();
              doc.Load(configFilePath);
              XmlNode node =
                  doc.SelectSingleNode("/configuration/appSettings/add[@key='ConfigDir']/@value");
              if (node != null)
              {
                _configDir = node.InnerText;
                if (!Path.IsPathRooted(_configDir))
                  _configDir = Path.Combine(appDir, _configDir);

                Trace.WriteLine(string.Format("ConfigDirectory.get(): path to configuration dir is set to '{0}'", _configDir));
              }
              else
                Trace.WriteLine("ConfigDirectory.get(): appSettings section doesn't contain ConfigDir parameter");
            }
            else
              Trace.WriteLine(string.Format("ConfigDirectory.get(): configuration file '{0}' not found", configFilePath));
          }
          catch (Exception ex)
          {
            Trace.WriteLine("ConfigDirectory.get(): exception: " + ex);
          }

          if (string.IsNullOrEmpty(_configDir))
          {
            _configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
																		 ApplicationInfo.MainAttributes.Company, ApplicationInfo.MainAttributes.ProductName, "Config");
            Trace.WriteLine(string.Format("ConfigDirectory.get(): configuration dir is set by default to '{0}'", _configDir));
          }

          if (!Directory.Exists(_configDir))
            Directory.CreateDirectory(_configDir);
        }

        return _configDir;
      }
    }
  }
}