using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.Application.Load;
using Toolbox.Application.Services;
using Toolbox.GUI.Application;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Dialogs.Work;
using Toolbox.GUI.DX.Properties;
using Toolbox.Log;
using log4net;

namespace Toolbox.GUI.DX.Application
{
	public class ApplicationStarterDX : ApplicationStarter
	{
		[NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(ApplicationStarterDX));
    private static readonly HashSet<Type> _supress_exceptions = new HashSet<Type>();

		public ApplicationStarterDX([NotNull] IFactory<Form> formFactory, [NotNull] IFactory<ILoadingQueue> queueFactory)
			: base(formFactory, queueFactory)
		{
			if (formFactory == null) throw new ArgumentNullException("formFactory");
			if (queueFactory == null) throw new ArgumentNullException("queueFactory");
		}

    public static HashSet<Type> SupressExceptions
    {
      get { return _supress_exceptions; }
    }

		protected override ApplicationStarter.FormWrapper GetFormWrapper([NotNull] Form mainForm, ApplicationStarter starter, string[] commandArgs)
		{
			if (mainForm == null) throw new ArgumentNullException("mainForm");

			return new FormWrapperDX(mainForm, this, commandArgs);
		}

		protected override MainFormView GetMainFormView([NotNull] Form mainForm)
		{
			if (mainForm == null) throw new ArgumentNullException("mainForm");

			return new MainFormViewDX(mainForm);
		}

		override protected void UIExceptionHandler([NotNull] ThreadExceptionEventArgs e)
		{
			if (e == null) throw new ArgumentNullException("e");

			try
			{
				if (e.Exception != null)
        {
					_log.Fatal("UIExceptionHandler(): UI exception", e.Exception);

          if (_supress_exceptions.Contains(e.Exception.GetType()))
            return;
        }
				else
					_log.Fatal("UIExceptionHandler(): UI exception");

				using (var box = new ErrorBox())
				{
					box.Title = e.Exception == null ? "null" : e.Exception.Message; // "Exception Details";
					string str;
					var exception = e.Exception;
					if (exception == null)
					{
						str = "null";
					}
					else
					{
						str = string.Format(
							"{0}: {1}{2}{3}: {4}{2}{5}: {6}{2}{2}{7}:{2}{8}",
							Properties.Resources.EXCEPTION_TYPE,
							exception.GetType(),
							Environment.NewLine,
							Properties.Resources.EXCEPTION_MESSAGE,
							exception.Message,
							Properties.Resources.EXCEPTION_SOURCE,
							exception.Source,
							Properties.Resources.EXCEPTION_CALLSTACK,
							exception.StackTrace);
					}

					box.Message = str;
					box.StartPosition = FormStartPosition.CenterScreen;

					box.ShowDialog();
				}

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
			catch (Exception ex)
			{
				_log.Fatal("UIExceptionHandler(): exception", ex);
			}
		}

		protected class FormWrapperDX : FormWrapper, IDisposable
		{
			public FormWrapperDX([NotNull] Form mainForm, [NotNull] ApplicationStarter starter, string[] commandArgs) : base(mainForm, starter, commandArgs)
			{
				if (mainForm == null) throw new ArgumentNullException("mainForm");
				if (starter == null) throw new ArgumentNullException("starter");
			}

			protected override ApplicationLoadingResult RunLoadingTask([NotNull] ApplicationLoadingTask task)
			{
				if (task == null) throw new ArgumentNullException("task");

				using (var frm = new SplashScreen() { IsMarquee = true, })
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
					else if (m_starter.AlternativeSplashScreen != null)
						frm.Image = m_starter.AlternativeSplashScreen;
					else
						frm.Image = Resources.DefaultSplash;

					var work = new ApplicationLoadingWork(task, m_main_form);

					frm.Work = work.Run;
					frm.ShowDialog(m_main_form);

					return work.Result;
				}
			}

			protected override bool ShowSettingsForm(InfoBuffer buffer)
			{
				using (var frm = new SettingsForm())
				{
					frm.ExternalBuffer = buffer;
					return frm.ShowDialog(m_main_form) == DialogResult.OK;
				}
			}
		}
	}
}
