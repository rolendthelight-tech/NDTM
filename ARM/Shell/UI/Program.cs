namespace Shell.UI
{
    using AT.Toolbox;
    using ERMS.Core.Common;
    using ERMS.Core.DAL;
    using ERMS.Core.DbMould;
    using GUOP;
    using log4net;
    using Shell.UI.Properties;
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Reflection;
    using TransportModel;

    internal static class Program
    {
        private static ILog Log;
        private static readonly string DiagnosticLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "diagnostic.log");
        private static void WriteDiagnostic(string message, Exception ex = null)
        {
            try
            {
                var text = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff} - {1}\r\n", DateTime.Now, message ?? "");
                if (ex != null)
                    text += ex.ToString() + "\r\n";
                File.AppendAllText(DiagnosticLog, text);
            }
            catch { }
        }
        private const string ConfigFileName = "ndtm.config";

        [STAThread]
        private static void Main()
        {
            try
            {
                MainCore();
            }
            catch (System.MissingMethodException mmex) when (mmex.Message.Contains("CreateBlockingMutex") || mmex.Message.Contains("ReleaseBlockingMutex"))
            {
                // Handle version mismatch with ERMS.UI.dll
                Console.Error.WriteLine("Version mismatch with ERMS.UI.dll: " + mmex.Message);
                MessageBox.Show(
                    "Обнаружена несовместимость версий компонентов приложения.\n" +
                    "Пожалуйста, перестройте решение (Clean + Build).\n\n" +
                    "Error: " + mmex.Message,
                    "Ошибка несовместимости",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Критическая ошибка при запуске приложения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private static void MainCore()
        {
            // Initialize logging with fallback approach - skip embedded resources due to build issues
            try
            {
                WriteDiagnostic("MainCore start");
                string logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                if (File.Exists(logConfigPath))
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(logConfigPath));
                }
                else
                {
                    log4net.Config.BasicConfigurator.Configure();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to initialize logging: " + ex.Message);
                WriteDiagnostic("Failed to initialize logging: " + ex.Message, ex);
                try
                {
                    log4net.Config.BasicConfigurator.Configure();
                }
                catch { }
            }
            Log = LogManager.GetLogger(typeof(Program).Name);
            string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlugIns");
            if (!Directory.Exists(pluginsDir))
            {
                Directory.CreateDirectory(pluginsDir);
            }

            foreach (string str in Directory.GetFiles(pluginsDir, "*.dll"))
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(str));
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Main(): Exception ", exception);
                    }
                }
            }
            AppInstance.LoadPlugins = true;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Global exception handlers to catch unhandled exceptions from WinForms message loop
            Application.ThreadException += (sender, e) =>
            {
                // Log unhandled exceptions from WinForms threads. Do not show modal dialogs to the user
                // to avoid interrupting application startup and background operations.
                Log.Error("Unhandled thread exception", e.Exception);
#if !DEBUG
                WriteDiagnostic("Unhandled thread exception: " + e.Exception.Message, e.Exception);
#endif
#if DEBUG
                // In debug builds show the dialog to help troubleshooting.
                try
                {
                    MessageBox.Show(
                        "Ошибка при сохранении настроек:\n" + e.Exception.Message,
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                catch { }
#endif
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error("Fatal unhandled exception", e.ExceptionObject as Exception);
                WriteDiagnostic("Fatal unhandled exception: " + (e.ExceptionObject as Exception)?.Message, e.ExceptionObject as Exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Ensure ndtm.config exists before creating the ConfigFileFinder.
            // If ConfigFileFinder is created before the file exists it may scan and record a null BasePath.
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                Log.InfoFormat("BaseDirectory: {0}", AppDomain.CurrentDomain.BaseDirectory);
                Log.InfoFormat("configPath: {0}", configPath);
                Log.InfoFormat("File exists: {0}", File.Exists(configPath));
                bool needInitializeConfig = !File.Exists(configPath) || IsMinimalConfig(configPath);
                if (needInitializeConfig)
                {
                    string settingsXml = ResolveTemplateConfigPath(AppDomain.CurrentDomain.BaseDirectory);
                    if (!string.IsNullOrEmpty(settingsXml) && File.Exists(settingsXml))
                    {
                        File.Copy(settingsXml, configPath, true);
                        Log.InfoFormat("Copied settings.xml to: {0}", configPath);
                    }
                    else
                    {
                        File.WriteAllText(configPath, "<CONFIG></CONFIG>");
                        Log.WarnFormat("settings.xml was not found. Created minimal ndtm.config at: {0}", configPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ensure ndtm.config exists", ex);
            }

            AppInstance.ConfigFileFinder = CreateSafeConfigFileFinder();
            RecordManager.Service = new RecordManagementService();
            FileEntityDescriptionSource.ColumnListSources.Add(new EntityInfoCoulmnListSource(GUOPContext.DataSourceName));
            string fieldDescriptionFile = Shell.UI.Properties.Settings.Default.FieldDescription;
            string fieldDescriptionPath = null;
            try
            {
                if (Path.IsPathRooted(fieldDescriptionFile))
                {
                    fieldDescriptionPath = fieldDescriptionFile;
                }
                else
                {
                    // First, check application base directory (output directory)
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string candidate = Path.Combine(baseDir, fieldDescriptionFile);
                    if (File.Exists(candidate))
                        fieldDescriptionPath = candidate;

                    // If not found, walk up a few directory levels to locate the source file (e.g. ../../database.ed when running from bin/Debug)
                    if (fieldDescriptionPath == null)
                    {
                        string up = baseDir;
                        for (int i = 0; i < 6; i++)
                        {
                            up = Path.GetFullPath(Path.Combine(up, ".."));
                            candidate = Path.Combine(up, fieldDescriptionFile);
                            if (File.Exists(candidate))
                            {
                                fieldDescriptionPath = candidate;
                                break;
                                        }
                                    }
                                }
                            }

                if (!string.IsNullOrEmpty(fieldDescriptionPath) && File.Exists(fieldDescriptionPath))
                {
                    try
                    {
                        FileEntityDescriptionSource.Fields[GUOPContext.DataSourceName] = EntityDescriptionContainer.LoadFromFile(fieldDescriptionPath);
                        Log.InfoFormat("Loaded field description from: {0}", fieldDescriptionPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to load field description file", ex);
                    }
                }
                else
                {
                    Log.WarnFormat("Field description file not found: {0}. Continuing without it.", fieldDescriptionFile);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while resolving field description file path", ex);
            }

            // Dynamic loading of ERMS.UI types to allow late binding and handle version mismatches
            try
            {
                Type formFactoryType = Type.GetType("ERMS.UI.FormFactory, ERMS.UI", true);
                PropertyInfo customizationsProperty = formFactoryType.GetProperty("Customizations", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                object customizations = customizationsProperty.GetValue(null, null);

                Type visumDatabaseType = Type.GetType("TransportModel.VisumDatabase, TransportModel", true);
                Type visumDatabaseFormFactoryType = typeof(VisumDatabaseFormFactory);
                object formFactory = Activator.CreateInstance(visumDatabaseFormFactoryType);

                var indexer = customizations.GetType().GetProperty("Item", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, new[] { typeof(Type) }, null);
                indexer.SetValue(customizations, formFactory, new object[] { visumDatabaseType });
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to configure FormFactory customizations: " + ex.Message);
            }

            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlugIns")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlugIns"));
            }

            // Dynamic loading of ERMS.UI.AppWrapper to allow late binding and handle version mismatches
            Type appWrapperType = Type.GetType("ERMS.UI.AppWrapper, ERMS.UI", true);

            // AddSectionType<T>() - generic method with no parameters
            MethodInfo addSectionTypeMethod = appWrapperType.GetMethod("AddSectionType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, Type.EmptyTypes, null);
            MethodInfo addSectionTypeGenericMethod = addSectionTypeMethod.MakeGenericMethod(typeof(ETLSettings));
            addSectionTypeGenericMethod.Invoke(null, null);

            addSectionTypeGenericMethod = addSectionTypeMethod.MakeGenericMethod(typeof(ETLShellSettings));
            addSectionTypeGenericMethod.Invoke(null, null);

            // SplashScreen property
            PropertyInfo splashScreenProperty = appWrapperType.GetProperty("SplashScreen", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            splashScreenProperty.SetValue(null, "Splash.jpg", null);

            bool appWrapperRunSucceeded = false;
            try
            {
                // Run<T>(bool) - generic method taking a bool parameter
                MethodInfo runMethod = appWrapperType.GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] { typeof(bool) }, null);
                MethodInfo runGenericMethod = runMethod.MakeGenericMethod(typeof(MainForm));
                runGenericMethod.Invoke(null, new object[] { true });
                appWrapperRunSucceeded = true;
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is System.MissingMethodException mmex && (mmex.Message.Contains("CreateBlockingMutex") || mmex.Message.Contains("ReleaseBlockingMutex") || mmex.Message.Contains("ExcludePrefixes")))
            {
                Log.Error("AppWrapper.Run encountered version mismatch with CreateBlockingMutex/ExcludePrefixes; retrying without initialization.", ex);
                try
                {
                    MethodInfo runMethod = appWrapperType.GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] { typeof(bool) }, null);
                    MethodInfo runGenericMethod = runMethod.MakeGenericMethod(typeof(MainForm));
                    runGenericMethod.Invoke(null, new object[] { false });
                    appWrapperRunSucceeded = true;
                }
                catch (Exception inner)
                {
                    Log.Error("AppWrapper.Run(requireInit=false) also failed.", inner);
                    // Don't throw - continue to fallback
                }
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is Exception)
            {
                Log.Error("AppWrapper.Run(requireInit=true) failed; retrying without initialization.", ex.InnerException);
                try
                {
                    MethodInfo runMethod = appWrapperType.GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] { typeof(bool) }, null);
                    MethodInfo runGenericMethod = runMethod.MakeGenericMethod(typeof(MainForm));
                    runGenericMethod.Invoke(null, new object[] { false });
                    appWrapperRunSucceeded = true;
                }
                catch (Exception inner)
                {
                    Log.Error("AppWrapper.Run(requireInit=false) also failed.", inner);
                    // Don't throw - continue to fallback
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error during AppWrapper.Run invocation.", ex);
                // Don't throw - continue to fallback
            }

            // Fallback: if AppWrapper failed, try to run the form directly
            if (!appWrapperRunSucceeded)
            {
                Log.Warn("AppWrapper.Run failed completely. Attempting direct MainForm initialization as fallback.");
                try
                {
                    MainForm form = new MainForm();
                    Application.Run(form);
                }
                catch (System.Resources.MissingManifestResourceException rmex)
                {
                    Log.Error("Direct MainForm initialization failed - missing resources. This may indicate a build issue.", rmex);
                    MessageBox.Show(
                        "Ошибка загрузки ресурсов приложения.\n\n" +
                        "Решение: Выполните 'Clean' и 'Rebuild' для проекта ARM.\n\n" +
                        "Error: " + rmex.Message,
                        "Ошибка ресурсов",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error("Direct MainForm initialization also failed.", ex);
                    throw;
                }
            }
        }

        public static ETLShellSettings Preferences =>
            ERMS.Core.Common.AppManager.Configurator.GetSection<ETLShellSettings>();

        private static void LogConfigFinderBasePath(ConfigFileFinder finder)
        {
            try
            {
                var basePathProperty = typeof(ConfigFileFinder).GetProperty("BasePath");
                if (basePathProperty != null)
                {
                    object basePath = basePathProperty.GetValue(finder, null);
                    Log.InfoFormat("ConfigFileFinder.BasePath: {0}", basePath ?? "<null>");
                }
                else
                {
                    Log.Warn("ConfigFileFinder.BasePath property not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to read ConfigFileFinder.BasePath", ex);
            }
        }

        private static bool IsMinimalConfig(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return true;
                }

                string configContent = File.ReadAllText(path).Trim();
                return string.Equals(configContent, "<CONFIG></CONFIG>", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to validate ndtm.config content", ex);
                return false;
            }
        }


        private static bool IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        private static string ResolveTemplateConfigPath(string startDirectory)
        {
            try
            {
                if (IsNullOrWhiteSpace(startDirectory))
                {
                    return null;
                }

                string current = startDirectory;
                for (int i = 0; i < 8; i++)
                {
                    string candidate = Path.Combine(current, "settings.xml");
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }

                    DirectoryInfo parentInfo = Directory.GetParent(current);
                    string parent = parentInfo?.FullName;
                    if (string.IsNullOrEmpty(parent) || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    current = parent;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to resolve settings.xml location", ex);
            }

            return null;
        }

        private static ConfigFileFinder CreateSafeConfigFileFinder()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentDir = Environment.CurrentDirectory;
            string startupPath = Application.StartupPath;
            string[] lookupPaths = new[] { baseDir, currentDir, startupPath };
            var finder = new ConfigFileFinder(lookupPaths, ConfigFileName);

            Log.InfoFormat("ConfigFileFinder lookup paths: {0}", string.Join("; ", lookupPaths));
            LogConfigFinderBasePath(finder);

            try
            {
                // Force finder initialization early so Path.Combine does not fail later during settings save.
                string outputPath = finder.OutputConfigFilePath;
                Log.InfoFormat("ConfigFileFinder.OutputConfigFilePath: {0}", outputPath);
            }
            catch (Exception ex)
            {
                Log.Error("ConfigFileFinder failed during initialization. Creating local fallback config.", ex);

                string fallbackDirectory = IsNullOrWhiteSpace(baseDir)
                    ? Directory.GetCurrentDirectory()
                    : baseDir;
                string fallbackPath = Path.Combine(fallbackDirectory, ConfigFileName);
                if (!File.Exists(fallbackPath))
                {
                    string settingsXml = ResolveTemplateConfigPath(fallbackDirectory);
                    if (!string.IsNullOrEmpty(settingsXml) && File.Exists(settingsXml))
                    {
                        File.Copy(settingsXml, fallbackPath);
                        Log.InfoFormat("Copied settings.xml to fallback config path: {0}", fallbackPath);
                    }
                    else
                    {
                        File.WriteAllText(fallbackPath, "<CONFIG></CONFIG>");
                        Log.WarnFormat("settings.xml was not found for fallback. Created minimal config at: {0}", fallbackPath);
                    }
                }

                finder = new ConfigFileFinder(new[] { fallbackDirectory }, ConfigFileName);
            }

            return finder;
        }
    }
}
