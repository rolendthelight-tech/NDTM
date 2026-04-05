using System;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Properties;
using Toolbox.Log;
using log4net;

namespace Toolbox.GUI.DX.Dialogs.Work
{
  public partial class MinimizableTaskForm : LocalizableForm
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(MinimizableTaskForm));

	  [NotNull] private readonly TaskInfo m_task;
    private bool m_completed;
    //private bool m_user_closed;
    private DateTime m_begin_time;

    public MinimizableTaskForm([NotNull] TaskInfo task)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      m_task = task;

      InitializeComponent();

      m_main_group.Text = m_task.Name;
	    {
		    var can_cancel = m_task.CanCancel;
				m_cancel_button.Visible = can_cancel;
				m_cancel_button.Enabled = can_cancel;
			}
	    {
		    var percent_notification = m_task.PercentNotification;
				m_progress_bar.Visible = percent_notification;
				m_progress_bar.Enabled = percent_notification;
				m_marquee_bar.Visible = !percent_notification;
				m_marquee_bar.Enabled = !percent_notification;
			}
	    {
		    var can_minimize = m_task.CanMinimize;
				m_minimize_button.Visible = can_minimize;
				m_minimize_button.Enabled = can_minimize;
			}
	    m_picture_box.Image = m_task.Icon;

	    if (m_task.Icon == null)
	    {
		    m_picture_box.Visible = false;
		    var old_right = m_panel_state.Right;
        m_panel_state.Left = this.m_picture_box.Left;
				m_panel_state.Width = old_right - m_panel_state.Left;
	    }

	    m_task.ProgressChanged += this.HandleProgressChanged;
      m_task.CanCancelChanged += this.HandleCanCancelChanged;
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
			SetPercentage(state.Percentage);
      m_label_state.Text = state.Description;

      m_worker.RunWorkerAsync();
    }

    protected override void OnFormClosing([NotNull] FormClosingEventArgs e)
    {
	    if (e == null) throw new ArgumentNullException("e");

	    if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel
        && !m_task.CanCancel && !m_task.CanMinimize)
      {
        e.Cancel = true;
				_log.WarnFormat("OnFormClosing(): {0}", Resources.UNEXPECTED_CANCELLATION_BY_USER);
        return;
      }

      m_task.ProgressChanged -= this.HandleProgressChanged;
      m_task.CanCancelChanged -= this.HandleCanCancelChanged;
			base.OnFormClosing(e);
    }

    private void m_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (m_task.CloseOnFinish)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.Close();

        //if (!m_user_closed)
        //  ShowInfoBuffer(this.m_task);
      }
      else
      {
        m_cancel_button.Text = Resources.OK;
        m_cancel_button.Visible = true;
        m_cancel_button.Enabled = true;

				m_progress_bar.Position = m_progress_bar.Properties.Maximum;
        m_progress_bar.Visible = true;
      }
    }

    private void m_worker_DoWork(object sender, DoWorkEventArgs e)
    {
      m_completed = false;
      m_begin_time = DateTime.Now;

      while (!m_task.Wait(new TimeSpan(0, 0, 1)))
      {
        if (m_task.Status == TaskStatus.Cancelling)
          return;

        if (m_task.KillTimeout != null && DateTime.Now > m_begin_time + m_task.KillTimeout.Value)
        {
          this.UIThread(() =>
          {
            m_cancel_button.Text = Resources.KILL_TASK;
            m_cancel_button.Visible = true;
            m_cancel_button.Enabled = true;
          });
        }
      }

      m_completed = true;
    }

		private void SetProgress(double progress)
		{
			if (!(progress >= 0d && progress <= 1d))
				throw new ArgumentOutOfRangeException("progress", progress, "¬(progress ≥ 0 ∧ progress ≤ 1)");

			var progress_control = m_progress_bar;
			var progress_properties = progress_control.Properties;
			progress_control.Position = (int)((progress_properties.Maximum - progress_properties.Minimum) * progress) + progress_properties.Minimum;
		}

		private void SetProgressWithCorrection(double progress)
		{
		  try
		  {
			  SetProgress(progress);
		  }
		  catch (ArgumentOutOfRangeException ex)
		  {
			  if (progress < 0d)
				  SetProgress(0d);
			  else if (progress > 1d)
				  SetProgress(1d);
			  else
				  throw new ApplicationException("Ошибка расчётов", ex);
			  throw;
		  }
		}

	  private void SetPercentage(int percent)
		{
			if (!(percent >= 0 && percent <= 100))
				throw new ArgumentOutOfRangeException("percent", percent, "¬(percent ≥ 0 ∧ percent ≤ 100)");

			SetProgress(percent / 100.0);
		}

		private void SetPercentageWithCorrection(int percent)
	  {
		  try
		  {
				SetPercentage(percent);
		  }
		  catch (ArgumentOutOfRangeException ex)
		  {
				if (percent < 0)
					SetPercentage(0);
				else if (percent > 100)
					SetPercentage(100);
			  else
				  throw new ApplicationException("Ошибка расчётов", ex);
			  throw;
		  }
	  }

	  private void HandleProgressChanged(object sender, EventArgs e)
	  {
		  try
		  {
			  var state = m_task.GetCurrentState();
			  SetPercentageWithCorrection(state.Percentage);

			  if (m_task.Status != TaskStatus.Cancelling)
				  m_label_state.Text = state.Description;
		  }
		  catch (Exception ex)
		  {
			  _log.Error("HandleProgressChanged(): exception", ex);
		  }
	  }

	  private void HandleCanCancelChanged(object sender, EventArgs e)
	  {
		  try
		  {
			  m_cancel_button.Visible = m_task.CanCancel;
		  }
		  catch (Exception ex)
		  {
			  _log.Error("HandleCanCancelChanged(): exception", ex);
		  }
	  }

	  private void m_cancel_button_Click(object sender, EventArgs e)
		{
			if (m_cancel_button.Text == Resources.CANCEL)
			{
				m_task.Cancel();
				m_cancel_button.Enabled = false;
        m_label_state.Text = Resources.CANCELLING;
			}
			else if (m_cancel_button.Text == Resources.KILL_TASK)
			{
				if (AppManager.Notificator.Confirm(new Info(string.Format(
					Resources.KILL_TASK_CONFIRM, m_task.Name), InfoLevel.Warning)))
				{
					m_task.Kill();
					m_cancel_button.Visible = false;
				}
			}
			else if (m_cancel_button.Text == Resources.OK)
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else
			{
				_log.ErrorFormat("m_cancel_button_Click(): {0}", Resources.UNKNOWN_ACTION);
			}
		}

  	private void m_minimize_button_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      this.Close();
      //m_user_closed = true;
    }

    //private void ShowInfoBuffer([NotNull] TaskInfo task)
    //{
    //  if (task == null) throw new ArgumentNullException("task");

    //  var info_buffer = task.GetCustomState<InfoBuffer>();
    //  if (info_buffer == null)
    //    _log.DebugFormat("ShowInfoBuffer(): {0}", Resources.TASK_NO_RESULT);
    //  else
    //  {
    //    var timer = new Timer();

    //    timer.Tick += (sender, e) =>
    //    {
    //      var tm = (Timer)sender;
    //      tm.Stop();
    //      tm.Dispose();

    //      AppManager.Notificator.ShowBuffer(Resources.TASK_RESULT, info_buffer);
    //    };

    //    timer.Start();
    //  }
    //}
  }
}