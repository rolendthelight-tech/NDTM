using System;
using System.Windows.Forms;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.Application.Services;
using Toolbox.GUI.Controls.Log;
using Toolbox.GUI.Dialogs;
using Toolbox.Log;

namespace Toolbox.GUI.Application
{
  /// <summary>
  /// Реализация всех интерфейсов представлений,
  /// используемых сервисами AppManager, на базе WindowsForms / DevExpress
  /// </summary>
  abstract public class MainFormView : IAppInstanceView, INotificationView, ITaskView, ILoginServiceView
  {
	  [NotNull] protected readonly Form m_main_form;

    public MainFormView([NotNull] Form mainForm)
    {
      if (mainForm == null)
        throw new ArgumentNullException("mainForm");

      m_main_form = mainForm;
    }

    #region IAppInstanceView Members

    public string StartupPath
    {
      get { return System.Windows.Forms.Application.StartupPath; }
    }

    public void Restart(CommandLineArgs args)
    {
      System.Windows.Forms.Application.Restart();
    }

    public void Exit()
    {
      System.Windows.Forms.Application.Exit();
    }

    #endregion

    #region ISynchronizeProvider Members

	  [NotNull]
	  public ISynchronizeInvoke Invoker
    {
      get { return m_main_form; }
    }

    #endregion

    #region INotificationView Members

    public int LoggingQueueSize
    {
      get { return LogListerSettings.Preferences.RecentLogCount; }
    }

		public virtual bool Alert([NotNull] Info info, bool confirm)
		{
			if (info == null) throw new ArgumentNullException("info");

			throw new NotImplementedException();
		}

	  public virtual bool Alert([CanBeNull] string summary, [NotNull] InfoBuffer buffer, bool confirm)
	  {
		  if (buffer == null) throw new ArgumentNullException("buffer");

		  throw new NotImplementedException();
	  }

	  #endregion

    #region ILoginServiceView Members

    public bool ShowAuthenticationForm([CanBeNull] out UserCredentials crd)
    {
      using (var frm = new LoginForm())
      {
        if (frm.ShowDialog(m_main_form) == DialogResult.OK)
        {
        	crd = new UserCredentials
        		{
        			Login = frm.Login,
        			Password = frm.Password,
        			SaveCredentials = frm.SaveCredentials
        		};

        	return true;
        }
        else
        {
          crd = null;
          return false;
        }
      }
    }

    #endregion

    #region ITaskView Members

    virtual public bool ShowProgressIndicator([NotNull] TaskInfo task)
    {
	    if (task == null) throw new ArgumentNullException("task");

	    throw new NotImplementedException();
    }

	  #endregion
  }
}
