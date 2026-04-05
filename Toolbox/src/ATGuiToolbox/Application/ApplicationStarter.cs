using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Log;
using AT.Toolbox.Misc;
using log4net;

namespace AT.Toolbox
{
  public class ApplicationStarter
  {
    private static readonly ILog _log = LogManager.GetLogger("ApplicationStarter");
    private static System.Windows.SplashScreen _splash;
    private static string _splash_resource;

    private readonly IFactory<Form> m_form_factory;
    private readonly IFactory<ILoadingQueue> m_queue_factory;

    public ApplicationStarter(IFactory<Form> formFactory, IFactory<ILoadingQueue> queueFactory)
    {
      if (formFactory == null)
        throw new ArgumentNullException("formFactory");

      if (queueFactory == null)
        throw new ArgumentNullException("queueFactory");

      m_queue_factory = queueFactory;
      m_form_factory = formFactory;
    }

    public static void ShowSplashScreeen(string resource)
    {
      _splash = new System.Windows.SplashScreen(resource);
      _splash.Show(true);
      _splash_resource = resource;
    }

    public void Run(string[] commandArgs)
    {
      LoggingUtils.InitializeLogging(Properties.Resources.Log4netConfig, commandArgs);
      
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      Application.ThreadException += new ThreadExceptionEventHandler(UIExceptionHandler);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      using (var wrapper = new FormWrapper(m_form_factory.Create(), this, commandArgs))
      {
        var view = new MainFormView(wrapper.MainForm);

        AppManager.Instance = new AppInstance(view, commandArgs);
        AppManager.Notificator = new Notificator(view);
        AppManager.TaskManager = new TaskManager(view);
        // TODO: AppManager.LoginService

        try
        {
          Application.Run(wrapper.MainForm);
        }
        catch (Exception ex)
        {
          _log.Error("Run(): exception", ex);
        }
      }
    }

    private void UIExceptionHandler(object sender, ThreadExceptionEventArgs e)
    {
      ErrorBox box = new ErrorBox();

      box.Title = e.Exception.Message; // "Exception Details";

      string str = Environment.NewLine;
      str += Properties.Resources.EXCEPTION_SOURCE + @" " + e.Exception.Source + Environment.NewLine + Environment.NewLine;
      str += Properties.Resources.EXCEPTION_CALLSTACK + Environment.NewLine + e.Exception.StackTrace;

      box.Message = str;
      box.StartPosition = FormStartPosition.CenterScreen;

      box.ShowDialog();

      AppManager.Configurator.HandleError(e.Exception);
      AppManager.Configurator.SaveSettings();

      // В случае Stack Overflow нужно выходить всегда
      if (e.Exception is StackOverflowException)
        AppManager.Instance.Exit();

      if (ApplicationSettings.Instance.AutoRestartOnCriticalFailure)
        AppManager.Instance.Restart();

      if (ApplicationSettings.Instance.CloseOnCriticalError)
        AppManager.Instance.Exit();
    }

    #region Inner

    private class FormWrapper : IDisposable
    {
      private readonly Form m_main_form;
      private readonly ApplicationStarter m_starter;
      private readonly string[] m_command_args;
      private bool m_exit_required = true;

      public FormWrapper(Form mainForm, ApplicationStarter starter, string[] commandArgs)
      {
        if (mainForm == null)
          throw new ArgumentNullException("mainForm");

        m_main_form = mainForm;
        m_starter = starter;
        m_command_args = commandArgs;

        m_main_form.Load += this.HandleMainFormLoad;
        m_main_form.FormClosed += this.HandleClosed;
      }

      public Form MainForm
      {
        get { return m_main_form; }
      }

      private void HandleMainFormLoad(object sender, EventArgs e)
      {
        try
        {
          AppManager.Instance.CreateBlockingMutex();
        }
        catch (Exception ex)
        {
          AppManager.Notificator.ShowMessage(new Info(ex));
          AppManager.Instance.Exit();
          return;
        }

        var queue = this.GetLoadingQueue();

        if (queue == null)
          return;

        if (!this.InitApplication(queue))
        {
          m_exit_required = false;

          if (!AppManager.Instance.Restarting)
            AppManager.Instance.Exit();
        }
      }

      private bool InitApplication(ILoadingQueue loadingQueue)
      {
        var container = new DependencyContainer();
        var task = new ApplicationLoadingTask(loadingQueue, container) { CommandArgs = m_command_args };
        var res = this.RunLoadingTask(task);

        while (!res.Success)
        {
          if (!AppManager.Notificator.Confirm(Properties.Resources.INIT_FAIL_DESCRIPTION, res.Buffer))
            return false;

          if (!this.ShowSettingsForm(res.Buffer))
            return false;

          task = new ApplicationLoadingTask(res, container) { CommandArgs = m_command_args };
          res = this.RunLoadingTask(task);
        }

        this.ApplyLayout();

        return true;
      }

      private void HandleClosed(object sender, FormClosedEventArgs e)
      {
        ISaveLayout sl = m_main_form as ISaveLayout;

        if (sl != null)
          sl.SaveLayout();

        AppManager.Configurator.SaveSettings();
      }

      private ILoadingQueue GetLoadingQueue()
      {
        var queue1 = m_main_form as ILoadingQueue;
        var queue2 = m_starter.m_queue_factory.Create();

        if (queue1 == null)
          return queue2;

        if (queue2 == null)
          return queue1;

        return new LoadingQueueComposite(new ILoadingQueue[] { queue1, queue2 });
      }

      #region Task management

      private ApplicationLoadingResult RunLoadingTask(ApplicationLoadingTask task)
      {
        using (var frm = new SplashScreen())
        {
          if (_splash != null)
            _splash.Close(new TimeSpan());

          if (!string.IsNullOrEmpty(_splash_resource))
          {
            var asm = Assembly.GetEntryAssembly();
            var name = new AssemblyName(asm.FullName);
            var rm = new ResourceManager(name.Name + ".g", asm);

            using (var ms = rm.GetStream(_splash_resource.ToLowerInvariant()))
            {
              frm.Image = new Bitmap(Image.FromStream(ms));
            }
          }

          var work = new ApplicationLoadingWork(task, m_main_form);

          frm.Work = work;
          frm.ShowDialog(m_main_form);

          return work.Result;
        }
      }

      private bool ShowSettingsForm(InfoBuffer buffer)
      {
        using (var frm = new SettingsForm())
        {
          frm.ExternalBuffer = buffer;
          return frm.ShowDialog(m_main_form) == DialogResult.OK;
        }
      }

      private void ApplyLayout()
      {
        var layout = m_main_form as ISaveLayout;

        if (layout != null)
          layout.RestoreLayout();
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        m_main_form.Dispose();

        AppManager.Instance.ReleaseBlockingMutex();

        if (m_exit_required)
        {
          //if (AppInstance_.Restart)
          //  Application.Restart();
          //else
          //  Application.Exit();
          if (!AppManager.Instance.Restarting)
            AppManager.Instance.Exit();
        }
      }

      #endregion
    }

    private class ApplicationLoadingWork : IBackgroundWork
    {
      private readonly ApplicationLoadingTask m_task;
      private readonly Form m_main_form;

      public ApplicationLoadingWork(ApplicationLoadingTask task, Form mainForm)
      {
        m_task = task;
        m_main_form = mainForm;
      }
      
      #region IBackgroundWork Members

      public bool CloseOnFinish
      {
        get { return true; }
      }

      public bool IsMarquee
      {
        get { return true; }
      }

      public bool CanCancel
      {
        get { return false; }
      }

      public System.Drawing.Bitmap Icon
      {
        get { return null; }
      }

      public string Name
      {
        get { return "Загрузка приложения"; }
      }

      public float Weight
      {
        get { return 1; }
      }

      public ApplicationLoadingResult Result { get; private set; }

      public void Run(BackgroundWorker worker)
      {
        this.PropertyChanged += delegate { };

        worker.ReportProgress(0, "Запуск...");
        this.Result = m_task.Run(m_main_form, worker);
      }

      #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion
    }

    #endregion
  }

  public interface ISaveLayout
  {
    void SaveLayout();

    void RestoreLayout();
  }
}
