using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application.Load;
using Toolbox.Application.Services;
using Toolbox.GUI.Base;
using Toolbox.GUI.Properties;
using Toolbox.Log;
using log4net;

namespace Toolbox.GUI.Application
{
  abstract public class ApplicationStarter
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationStarter));
		protected static System.Windows.SplashScreen _splash;
    protected static string _splash_resource;

	  [NotNull] protected readonly IFactory<Form> m_form_factory;
	  [NotNull] private readonly IFactory<ILoadingQueue> m_queue_factory;
    private bool m_skip_save_layout;

    public ApplicationStarter([NotNull] IFactory<Form> formFactory, [NotNull] IFactory<ILoadingQueue> queueFactory)
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

    public Image AlternativeSplashScreen { get; set; }

		public int Run([CanBeNull] string[] commandArgs)
		{
			InitializeLogging(commandArgs);

			System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			System.Windows.Forms.Application.ThreadException += UIExceptionHandler;
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			using (var wrapper = GetFormWrapper(m_form_factory.Create(), this, commandArgs))
			{
				var view = GetMainFormView(wrapper.MainForm);

				AppManager.Instance = new AppInstance(view, commandArgs);
				AppManager.Notificator = new Notificator(view);
				AppManager.TaskManager = new TaskManager(view);
				AppManager.LoginService = new LoginService(view);

				try
				{
					System.Windows.Forms.Application.Run(wrapper.MainForm);
					_log.Info("Run(): end");
					return 0;
				}
				catch (Exception ex)
				{
					_log.Error("Run(): exception", ex);
					return 1;
				}
			}
		}

		protected virtual FormWrapper GetFormWrapper([NotNull] Form mainForm, ApplicationStarter starter, string[] commandArgs)
		{
			if (mainForm == null) throw new ArgumentNullException("mainForm");

			throw new NotImplementedException();
		}

	  [NotNull]
	  protected virtual MainFormView GetMainFormView([NotNull] Form mainForm)
		{
			if (mainForm == null) throw new ArgumentNullException("mainForm");

			throw new NotImplementedException();
		}

	  //TODO: исправить на нормальный запуск в консольном режиме
		public static void InitializeLogging([CanBeNull] string[] commandArgs)
		{
			LoggingUtils.InitializeLogging(commandArgs);
		}

    protected virtual void UIExceptionHandler([NotNull] ThreadExceptionEventArgs e)
    {
    	throw new NotImplementedException();
    }

    private void UIExceptionHandler(object sender, [NotNull] ThreadExceptionEventArgs e)
    {
      m_skip_save_layout = true;
      this.UIExceptionHandler(e);
    }

  	#region Inner

    abstract protected class FormWrapper : IDisposable
    {
	    [NotNull] protected readonly Form m_main_form;
	    [NotNull] protected readonly ApplicationStarter m_starter;
      private readonly string[] m_command_args;
      private bool m_exit_required = true;

      public FormWrapper([NotNull] Form mainForm, [NotNull] ApplicationStarter starter, string[] commandArgs)
      {
        if (mainForm == null) throw new ArgumentNullException("mainForm");
	      if (starter == null) throw new ArgumentNullException("starter");

	      m_main_form = mainForm;
        m_starter = starter;
        m_command_args = commandArgs;

        if (m_main_form.WindowState == FormWindowState.Normal)
          m_main_form.Shown += this.HandleMainFormLoad;
        else
          m_main_form.Load += this.HandleMainFormLoad;

        m_main_form.FormClosed += this.HandleClosed;
      }

	    [NotNull]
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

      private bool InitApplication([NotNull] ILoadingQueue loadingQueue)
      {
	      if (loadingQueue == null) throw new ArgumentNullException("loadingQueue");

				var container = new DependencyContainer();
        var task = new ApplicationLoadingTask(loadingQueue, container) { CommandArgs = m_command_args };
        var res = this.RunLoadingTask(task);
        if (res == null)
        {
          _log.Fatal("InitApplication(): Can't get result loading task");
          return false;
        }

        while (!res.Success)
        {
          if (!AppManager.Notificator.Confirm(Resources.INIT_FAIL_DESCRIPTION, res.Buffer))
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

        if (sl != null && !m_starter.m_skip_save_layout)
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

			virtual protected ApplicationLoadingResult RunLoadingTask([NotNull] ApplicationLoadingTask task)
			{
				if (task == null) throw new ArgumentNullException("task");

				throw new NotImplementedException();
			}

	    virtual protected bool ShowSettingsForm(InfoBuffer buffer)
      {
				throw new NotImplementedException();
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

    protected class ApplicationLoadingWork
    {
	    [NotNull] private readonly ApplicationLoadingTask m_task;
	    [NotNull] private readonly Form m_main_form;

      public ApplicationLoadingWork([NotNull] ApplicationLoadingTask task, [NotNull] Form mainForm)
      {
	      if (task == null) throw new ArgumentNullException("task");
	      if (mainForm == null) throw new ArgumentNullException("mainForm");

	      m_task = task;
        m_main_form = mainForm;
      }

      #region IBackgroundWork Members

      public ApplicationLoadingResult Result { get; private set; }

      public void Run([NotNull] BackgroundWorker worker)
      {
	      if (worker == null) throw new ArgumentNullException("worker");

				worker.ReportProgress(0, "Запуск…");
        this.Result = m_task.Run(m_main_form, worker);
      }

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
