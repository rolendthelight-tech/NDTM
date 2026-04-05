using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace AT.Toolbox.Log
{
  public class LoggingUtils
  {
    private static void PrepareConfigFile(string log4netConfigFilePath, Assembly log4netConfigResourceAssembly, string log4netConfigResourceName)
    {
      if (!File.Exists(log4netConfigFilePath))
      {
        using (Stream stream = log4netConfigResourceAssembly.GetManifestResourceStream(log4netConfigResourceName))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              string dir = Path.GetDirectoryName(log4netConfigFilePath);
              if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

              string contents = reader.ReadToEnd();
              File.WriteAllText(log4netConfigFilePath, contents);
            }
          }
        }
      }
    }

    public const string UserNameKey = "userName";
    public const string AppNameKey = "appName";
    public const string MachineKey = "machineName";

    private static void PrepareConfigFile(string log4netConfigFilePath, string log4netConfig)
    {
      if (!File.Exists(log4netConfigFilePath))
      {
        string dir = Path.GetDirectoryName(log4netConfigFilePath);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        File.WriteAllText(log4netConfigFilePath, log4netConfig);
      }
    }

    public static log4net.ILog InitializeLogging(string log4net_config, string[] command_line_args)
    {
      LoggingUtils.InitializeLogging2(log4net_config);
      var log = log4net.LogManager.GetLogger(typeof(AppManager).Name);

      AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
      {
        var ex = e.ExceptionObject as Exception;

        if (ex != null)
          LogManager.GetLogger("AppFramework").Fatal("AppDomain.CurrentDomain.UnhandledException(): " + ex);
        else if (e.ExceptionObject != null)
          LogManager.GetLogger("AppFramework").Fatal("AppDomain.CurrentDomain.UnhandledException(): " + e.ExceptionObject);
      };

      LoggingUtils.WriteStartLogEntry(log, ApplicationInfo.MainAttributes.ProductName, command_line_args);

      return log;
    }

    public static void InitializeLogging(string log4netConfigFilePath, Assembly log4netConfigResourceAssembly, string log4netConfigResourceName)
    {
      PrepareConfigFile(log4netConfigFilePath, log4netConfigResourceAssembly, log4netConfigResourceName);
      InitializeLogging(log4netConfigFilePath);
    }

    public static void InitializeLogging2(string log4netConfig)
    {
      var log4netConfigFilePath = Path.Combine(ConfigUtils.ConfigDirectory, ApplicationInfo.MainAttributes.ProductTitle + ".log.config");
      PrepareConfigFile(log4netConfigFilePath, log4netConfig);
      InitializeLogging(log4netConfigFilePath);
    }

    public static void WriteStartLogEntry(log4net.ILog log, string appName, string[] args)
    {
      AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();

      log.Info(string.Format(@"{0} ({1}) started at {2} UTC", appName, assemblyName.FullName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
      var identity = WindowsIdentity.GetCurrent();
      string identityStr = identity != null ? identity.Name : "unknown";
      log.InfoFormat("Started under {0} account on {1}", identityStr, Environment.MachineName);

      if (args != null)
        log.InfoFormat("Command line arguments ({1}) are:\n\t{0}", string.Join("\n\t", args), args.Length);
      else
        log.InfoFormat("Command line arguments is null");
    }

    public static void InitializeLogging(string log4netConfigFilePath)
    {
      try
      {
        string relativeAppDataDir = string.Format("{0}\\{1}", ApplicationInfo.MainAttributes.Company,
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
            Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline")
          };

          BasicConfigurator.Configure(appender);
        }

        GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
        GlobalContext.Properties[UserNameKey] = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
        GlobalContext.Properties[MachineKey] = Environment.MachineName;
        GlobalContext.Properties["startTimeUtc"] =
            Process.GetCurrentProcess().StartTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
        GlobalContext.Properties["appDomainId"] = AppDomain.CurrentDomain.Id;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(string.Format("{0}: ERROR - failed to initialize logging, exception: {1}", ApplicationInfo.MainAttributes.ProductName, ex));
      }
    }

    public static void InitializeLoggingForTests()
    {
      var appender = new TraceAppender { Layout = new PatternLayout("%date [%property{pid}:%thread] %-5level %logger - %message%newline") };
      BasicConfigurator.Configure(appender);
    }
  }

  public class ConfigUtils
  {
    private static readonly log4net.ILog Log = LogManager.GetLogger("ConfigUtils");

    public static string _configDir;
    public static string ConfigDirectory
    {
      get
      {
        if (_configDir == null)
        {
          string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

          try
          {
#if NETFRAMEWORK
            string configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
#else
            // In .NET 5+, use the entry assembly name with .config extension
            string assemblyPath = Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly().Location;
            string configFilePath = assemblyPath + ".config";
#endif
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
                                      @"AudiTech\" + ApplicationInfo.MainAttributes.ProductName + @"\Config");
            Trace.WriteLine(string.Format("ConfigDirectory.get(): configuration dir is set by default to '{0}'", _configDir));
          }

          if (!Directory.Exists(_configDir))
            Directory.CreateDirectory(_configDir);
        }

        return _configDir;
      }
    }

    public static void Save(string filePath, object config)
    {
      XmlWriter writer = null;
      try
      {
        var serializer = new XmlSerializer(config.GetType());
        var settings = new XmlWriterSettings { Indent = true };

        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        writer = XmlWriter.Create(filePath, settings);

        if (writer != null)
        {
          serializer.Serialize(writer, config);
          writer.Flush();
        }
      }
      catch (Exception ex)
      {
        Log.Error("Save(): exception", ex);
//        MessageBox.Show("Failed to save configuration", Application.ProductName, MessageBoxButtons.OK,
//                        MessageBoxIcon.Error);
      }
      finally
      {
        if (writer != null)
          writer.Close();
      }
    }

    public static TConfigType Load<TConfigType>(string filePath) where TConfigType : class
    {
      try
      {
        if (File.Exists(filePath))
        {
          var serializer = new XmlSerializer(typeof(TConfigType));
          using (Stream stream = File.OpenRead(filePath))
          {
            return serializer.Deserialize(stream) as TConfigType;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("Load({0}): exception", filePath), ex);
      }

      return null;
    }
  }
}
