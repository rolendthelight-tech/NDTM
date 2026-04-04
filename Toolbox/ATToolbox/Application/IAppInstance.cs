using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;
using AT.Toolbox.Properties;

namespace AT.Toolbox
{
  /// <summary>
  /// Приложение в целом
  /// </summary>
  public interface IAppInstance
  {
    /// <summary>
    /// Объект синхронизации приложения
    /// </summary>
    ISynchronizeInvoke Invoker { get; }

    /// <summary>
    /// Главный поток приложения, в который прокидывает сообщения объект синхронизации
    /// </summary>
    Thread MainThread { get; }

    /// <summary>
    /// Путь к директории, из которой стартовало приложение
    /// </summary>
    string StartupPath { get; }

    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    CommandLineArgs CommandArgs { get; }

    /// <summary>
    /// Находится ли приложение в состоянии перезагрузки
    /// </summary>
    bool Restarting { get; }

    /// <summary>
    /// Находится ли приложение в состоянии завершения
    /// </summary>
    bool Terminating { get; }

    /// <summary>
    /// Имеет ли текущий пользователь права администратора
    /// </summary>
    bool IsUserAnAdmin { get; }

    /// <summary>
    /// Создание объекта блокировки приложения для предотвращения запуска нескольких копий
    /// </summary>
    void CreateBlockingMutex();

    /// <summary>
    /// Уничтожение объекта блокировки приложения
    /// </summary>
    void ReleaseBlockingMutex();

    /// <summary>
    /// Перезапуск приложения
    /// </summary>
    void Restart();

    /// <summary>
    /// Завершение приложения
    /// </summary>
    void Exit();
}

    

    

    

  /// <summary>
  /// Класс для контроля количества запущеных экземпляров приложения и перезапуска программы
  /// Для корректной работы Visual Studio Hosting Process должен быть выключен
  /// </summary>
  public class AppInstance : IAppInstance
  {
    /// <summary>
    /// ERMS.UI из дистрибутива ожидает вложенный тип AppInstance+UnhandledDelegate, не AT.Toolbox.UnhandledDelegate.
    /// </summary>
    public delegate bool UnhandledDelegate(Exception ex);

    #region Fields

    private volatile bool m_restarting;
    private volatile bool m_terminating;
    private readonly IAppInstanceView m_view;
    private readonly object m_lock = new object();
    private Mutex m_mutex;
    private readonly CommandLineArgs m_args;
    private Thread m_main_thread;

    #endregion

    static AppInstance()
    {
      // Static constructor to ensure CLR can find the CreateBlockingMutex and ReleaseBlockingMutex methods
      // This handles the case where older compiled ERMS.UI.dll calls these methods via reflection or IL
    }

    #region Constructor

    public AppInstance(IAppInstanceView view, string[] args)
    {
      m_view = view ?? throw new ArgumentNullException(nameof(view));

      m_args = new CommandLineArgs(args);
      s_currentInstance = this;  // Store as current instance for static access
    }

    #endregion

    #region IAppInstance Members

    public ISynchronizeInvoke Invoker
    {
      get { return m_view.Invoker; }
    }

    public Thread MainThread
    {
      get
      {
        if (m_main_thread != null)
          return m_main_thread;
        
        if (m_view.Invoker.InvokeRequired)
          return m_main_thread = (Thread)m_view.Invoker.Invoke(
            new Func<Thread>(APIHelper.GetCurrentThread), new object[0]);
        else
          return Thread.CurrentThread;
      }
    }

    public string StartupPath
    {
      get { return m_view.StartupPath; }
    }

    public CommandLineArgs CommandArgs
    {
      get { return m_args; }
    }

    public bool Restarting
    {
      get { return m_restarting; }
    }

    public bool Terminating
    {
      get { return m_terminating; }
    }

    public bool IsUserAnAdmin
    {
      get { return APIHelper.IsUserAnAdmin(); }
    }

    /// <summary>
    /// Префиксы исключений для классификации сборок. Экземплярное свойство требуется ERMS.UI (get_ExcludePrefixes).
    /// Backward-compatible instance getter that returns the shared static set.
    /// </summary>
    public HashSet<string> ExcludePrefixes
    {
      get
      {
        if (m_excludePrefixes == null)
          return new System.Collections.Generic.HashSet<string>();
        return m_excludePrefixes;
      }
    }

    public void CreateBlockingMutex()
    {
      try
      {
        // Check if we can invoke on the invoker (UI thread safety)
        if (m_view != null && m_view.Invoker != null && m_view.Invoker.InvokeRequired)
        {
          m_view.Invoker.Invoke(new Action(this.CreateBlockingMutex), new object[0]);
          return;
        }

        lock (m_lock)
        {
          if (m_mutex != null || m_terminating)
            return;

          var app_id = Process.GetCurrentProcess().MainModule.FileName.Replace('\\', '_').Replace(':', '_');

          m_mutex = new Mutex(true, app_id, out bool new_instance);

          if (!new_instance)
          {
            m_mutex.Close();
            m_mutex = null;

            if (ApplicationSettings.Instance.AllowOnlyOneInstance)
              throw new ApplicationException(Resources.CONFIG_NO_INSTANCE + Environment.NewLine + m_args);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Print("CreateBlockingMutex error: " + ex.Message);
        // Continue anyway - mutex creation is optional
      }
    }

    public void ReleaseBlockingMutex()
    {
      try
      {
        // Check if we can invoke on the invoker (UI thread safety)
        if (m_view != null && m_view.Invoker != null && m_view.Invoker.InvokeRequired)
        {
          m_view.Invoker.Invoke(new Action(this.ReleaseBlockingMutex), new object[0]);
          return;
        }

        lock (m_lock)
        {
          this.ReleaseMutexImpl();
        }
      }
      catch (Exception ex)
      {
        Debug.Print("ReleaseBlockingMutex error: " + ex.Message);
        // Continue anyway - mutex release is optional
      }
    }

    public void Restart()
    {
      lock (m_lock)
      {
        if (m_restarting)
          return;

        m_restarting = true;
        m_terminating = true;

        if (m_view.Invoker.InvokeRequired)
          m_view.Invoker.Invoke(new Action(this.ReleaseMutexImpl), new object[0]);
        else
          this.ReleaseMutexImpl();

        m_view.Restart(m_args);
      }
    }

    public void Exit()
    {
      lock (m_lock)
      {
        if (m_terminating)
          return;

        m_terminating = true;

        if (m_view.Invoker.InvokeRequired)
          m_view.Invoker.Invoke(new Action(this.ReleaseMutexImpl), new object[0]);
        else
          this.ReleaseMutexImpl();

        m_view.Exit();
      }
    }

    #endregion

    #region Implementation

    static void PrepareWorkingFolder()
    {
      //if (WorkingFolder != Path.GetTempPath())
      //{
      //  foreach (string s in Directory.GetDirectories(WorkingFolder))
      //  {
      //    try
      //    {
      //      Guid id = new Guid(Path.GetFileName(s));
      //      Directory.Delete(s, true);
      //    }
      //    catch (Exception ex)
      //    {
      //      _log.Error("PrepareWorkingFolder(): exception", ex);
      //    }
      //  }
      //}

      //if (null == WorkingFolder)
      //{
      //  Preferences.TempPath = Path.GetTempPath();
      //}
      //else
      //{
      //  //Создаём временный каталог
      //  string guid = Guid.NewGuid().ToString();
      //  Preferences.TempPath = Path.Combine(WorkingFolder, guid);

      //  if (Directory.Exists(Preferences.TempPath))
      //    Directory.Delete(Preferences.TempPath, true);

      //  Directory.CreateDirectory(Preferences.TempPath);
      //}
    }

    private static bool m_loadPlugins;
    private static IConfigFileFinder m_configFileFinder;
    private static System.Reflection.Assembly[] m_toolboxAssemblies = new System.Reflection.Assembly[0];
    private static AppInstance s_currentInstance;

    public static AppInstance CurrentInstance
    {
      get { return s_currentInstance; }
      set { s_currentInstance = value; }
    }

    public static bool LoadPlugins
    {
      get { return m_loadPlugins; }
      set { m_loadPlugins = value; }
    }

    public static IConfigFileFinder ConfigFileFinder
    {
      get { return m_configFileFinder; }
      set { m_configFileFinder = value; }
    }

    public static System.Reflection.Assembly[] ToolboxAssemblies
    {
      get { return m_toolboxAssemblies; }
      set { m_toolboxAssemblies = value ?? new System.Reflection.Assembly[0]; }
    }

    private static readonly System.Collections.Generic.HashSet<string> m_excludePrefixes = new System.Collections.Generic.HashSet<string>();

    // Backward-compatibility: older callers expected AppInstance.InitializeLogging(string, string[])
    // Delegate to LoggingUtils which contains the real implementation.
    public static log4net.ILog InitializeLogging(string repositoryName, string[] args)
    {
      try
      {
        // LoggingUtils exposes an overload with the same signature (string, string[])
        return AT.Toolbox.Log.LoggingUtils.InitializeLogging(repositoryName, args);
      }
      catch
      {
        try
        {
          return log4net.LogManager.GetLogger(string.IsNullOrEmpty(repositoryName) ? "AT.Toolbox" : repositoryName);
        }
        catch
        {
          return null;
        }
      }
    }

    // Backward-compatibility: older ERMS.UI.AppWrapper called this method
    public static void ReadRegistryAndCommandLineSettings(string[] _)
    {
      // This is a compatibility shim for old API. The functionality is no-op in current version.
      // The args are already processed by CommandLineArgs during AppInstance construction.
    }

    static void TopLevelExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
      try
      {
        //_log.Error("TopLevelExceptionHandler(): exception caught", e.ExceptionObject as Exception);

        //if (null == UnhandledExceptionDelegate || !UnhandledExceptionDelegate(e.ExceptionObject as Exception))
        //{
        //  Restart = Preferences.AutoRestartOnCriticalFailure;
        //  Terminate();
        //}
      }
      catch (Exception ex)
      {
        Debug.Print(ex.Message);
      }
    }

    private void ReleaseMutexImpl()
    {
      if (m_mutex == null)
        return;

      if (m_mutex.WaitOne(1))
        m_mutex.ReleaseMutex();

      m_mutex.Close();
      m_mutex = null;
    }

    private class APIHelper
    {
      [DllImport("shell32.dll")]
      public static extern bool IsUserAnAdmin();

      public static Thread GetCurrentThread()
      {
        return Thread.CurrentThread;
      }
    }

    #endregion
  }


  public interface IAppInstanceView : ISynchronizeProvider
  {
    string StartupPath { get; }

    void Restart(CommandLineArgs args);

    void Exit();
  }

  public class ProcessAppInstanceView : SynchronizeProviderStub, IAppInstanceView
  {
    private readonly Process m_process = Process.GetCurrentProcess();
    
    #region IAppInstanceView Members

    public string StartupPath
    {
      get { return Path.GetDirectoryName(m_process.MainModule.FileName); }
    }

    public void Restart(CommandLineArgs args)
    {
      Process.Start(m_process.MainModule.FileName, args.ToString());

      m_process.Kill();
    }

    public void Exit()
    {
      m_process.Kill();
    }

    #endregion
  }
}
