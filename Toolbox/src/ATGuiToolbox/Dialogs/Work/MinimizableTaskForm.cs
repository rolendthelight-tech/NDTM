using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using AT.Toolbox;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Dialogs
{
  public partial class MinimizableTaskForm : LocalizableForm
  {
    private readonly TaskInfo m_task;
    private bool m_completed;
    private bool m_close_on_finish;
    
    public MinimizableTaskForm(TaskInfo task)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      m_task = task;
      
      InitializeComponent();

      var parms = m_task.GetLaunchParameters();

      m_main_group.Text = m_task.Name;
      m_cancel_button.Visible = parms.CanCancel;
      m_progress_bar.Visible = parms.SupportsPercentNotification;
      m_minimize_button.Visible = parms.CanMinimize;
      m_picture_box.Image = parms.Icon;
      m_close_on_finish = parms.CloseOnFinish;

      m_task.ProgressChanged += this.HandleProgressChanged;
    }

    public bool IsCompleted
    {
      get { return m_completed; }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      m_cancel_button.Text = Resources.CANCEL;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      var state = m_task.GetCurrentState();
      m_progress_bar.Position = state.Percentage;
      m_label_state.Text = state.Description;

      m_worker.RunWorkerAsync();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      m_task.ProgressChanged -= this.HandleProgressChanged;
      base.OnClosing(e);
    }

    private void m_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (m_close_on_finish)
        this.Close();
      else
      {
        m_cancel_button.Text = Resources.OK;
        m_cancel_button.Visible = true;
        m_cancel_button.Enabled = true;

        m_progress_bar.Position = 0;
        m_progress_bar.Visible = true;
      }
    }

    private void m_worker_DoWork(object sender, DoWorkEventArgs e)
    {
      m_completed = false;
      
      while (!m_task.Wait(1000))
      {
        if (m_task.Status == TaskStatus.Cancelling)
          return;
      }

      m_completed = true;
    }

    private void HandleProgressChanged(object sender, EventArgs e)
    {
      var state = m_task.GetCurrentState();
      m_progress_bar.Position = state.Percentage;
      m_label_state.Text = state.Description;
    }

    private void m_cancel_button_Click(object sender, EventArgs e)
    {
      if (m_cancel_button.Text == Resources.CANCEL)
      {
        m_task.Cancel();
        m_cancel_button.Enabled = false;
      }
      else if (m_cancel_button.Text == Resources.OK)
        this.Close();        
    }

    private void m_minimize_button_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      this.Close();
    }
  }
}