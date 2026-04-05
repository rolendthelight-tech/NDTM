using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Toolbox.Extensions;
using Toolbox.Properties;

namespace Toolbox.Application.Services
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
    #region Fields

    private volatile bool m_restarting;
    private volatile bool m_terminating;
    private readonly IAppInstanceView m_view;
    private readonly object m_lock = new object();
    private Mutex m_mutex;
    private readonly CommandLineArgs m_args;
    private Thread m_main_thread;

    #endregion

    #region Constructor

    public AppInstance(IAppInstanceView view, string[] args)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
      m_args = new CommandLineArgs(args);
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
            new Func<Thread>(APIHelper.GetCurrentThread), ArrayExtensions.Empty<object>());
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

    public void CreateBlockingMutex()
    {
      if (m_view.Invoker.InvokeRequired)
      {
        m_view.Invoker.Invoke(new Action(this.CreateBlockingMutex), ArrayExtensions.Empty<object>());
        return;
      }

      lock (m_lock)
      {
        if (m_mutex != null || m_terminating)
          return;

        bool new_instance;

        string app_id;
	      using (var process = Process.GetCurrentProcess())
	      {
		      app_id = process.MainModule.FileName.Replace(Path.DirectorySeparatorChar, '_').Replace(':', '_');
	      }

	      m_mutex = new Mutex(true, app_id, out new_instance);

        if (!new_instance)
        {
          m_mutex.Close();
          m_mutex = null;

          if (ApplicationSettings.Instance.AllowOnlyOneInstance)
            throw new ApplicationException(Resources.CONFIG_NO_INSTANCE + Environment.NewLine + m_args);
        }
      }
    }

    public void ReleaseBlockingMutex()
    {
      if (m_view.Invoker.InvokeRequired)
      {
        m_view.Invoker.Invoke(new Action(this.ReleaseBlockingMutex), ArrayExtensions.Empty<object>());
        return;
      }

      lock (m_lock)
      {
        this.ReleaseMutexImpl();
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
          m_view.Invoker.Invoke(new Action(this.ReleaseMutexImpl), ArrayExtensions.Empty<object>());
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
          m_view.Invoker.Invoke(new Action(this.ReleaseMutexImpl), ArrayExtensions.Empty<object>());
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

    static private class APIHelper
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
		private readonly string m_main_module_file_name = GetMainModuleFileName();

		private static string GetMainModuleFileName()
		{
			using (var process = Process.GetCurrentProcess())
			{
				return process.MainModule.FileName;
			}
		}

    #region IAppInstanceView Members

    public string StartupPath
    {
			get { return Path.GetDirectoryName(m_main_module_file_name); }
    }

    public void Restart(CommandLineArgs args)
    {
	    if (args == null) throw new ArgumentNullException("args");

			using(Process.Start(m_main_module_file_name, args.ToString())){}

      Environment.Exit(3);
    }

    public void Exit()
    {
			Environment.Exit(4);
		}

    #endregion
  }
}
