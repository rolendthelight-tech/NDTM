using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Misc;
using AT.Toolbox.Dialogs;
using System.ComponentModel;
using System.Drawing;

namespace AT.Toolbox
{
  public class BackgroundWorkRunningContext : IRunningContext
  {
    private readonly Form m_owner;

    public BackgroundWorkRunningContext(Form owner)
    {
      m_owner = owner;
    }

    public BackgroundWorkRunningContext() : this(AppManager.Instance.Invoker as Form) { }

    #region IRunningContext Members

    public bool Run(IRunBase runBase, LaunchParameters parameters)
    {
      using (var frm = new BackgroundWorkerForm())
      {
        frm.Work = new RunBaseWithParametersWork(runBase, parameters);

        return frm.ShowDialog(m_owner) == DialogResult.OK;
      }
    }

    #endregion
  }

  internal class RunBaseWithParametersWork : IBackgroundWork
  {
    private readonly IRunBase m_run_base;
    private readonly LaunchParameters m_parameters;

    public RunBaseWithParametersWork(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_run_base = runBase;
      m_parameters = parameters;
    }

    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return m_parameters.CloseOnFinish; }
    }

    public bool IsMarquee
    {
      get { return !m_parameters.SupportsPercentNotification; }
    }

    public bool CanCancel
    {
      get { return m_parameters.CanCancel; }
    }

    public Bitmap Icon
    {
      get { return m_parameters.Icon; }
    }

    public string Name
    {
      get { return m_parameters.Name ?? m_run_base.GetType().GetDisplayName(); }
    }

    public float Weight
    {
      get { return m_parameters.Weight; }
    }

    public void Run(BackgroundWorker worker)
    {
      var handler = new BackgroundWorkerWrapper(worker);

      m_run_base.CancellationCheck += handler.HandleCancellationCheck;
      m_run_base.ProgressChanged += handler.HandleProgressChanged;

      try
      {
        m_run_base.Run();
      }
      finally
      {
        m_run_base.CancellationCheck -= handler.HandleCancellationCheck;
        m_run_base.ProgressChanged -= handler.HandleProgressChanged;
      }
    }

    private class BackgroundWorkerWrapper
    {
      private BackgroundWorker m_worker;

      public BackgroundWorkerWrapper(BackgroundWorker worker)
      {
        if (worker == null)
          throw new ArgumentNullException("worker");

        m_worker = worker;
      }

      public void HandleCancellationCheck(object sender, CancelEventArgs e)
      {
        e.Cancel = m_worker.CancellationPending;
      }

      public void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
      {
        m_worker.ReportProgress(e.ProgressPercentage, e.UserState);
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add { }
      remove { }
    }

    #endregion
  }
}
