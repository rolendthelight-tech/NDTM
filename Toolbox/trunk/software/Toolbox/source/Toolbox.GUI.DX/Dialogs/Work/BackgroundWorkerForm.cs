using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using DevExpress.XtraEditors;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.GUI.Base;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Properties;

namespace Toolbox.GUI.DX.Dialogs.Work
{
  /// <summary>
  /// Диалог для отображения прогресса фоновой обработки задачи
  /// </summary>
	[Obsolete("Используйте AppManager.TaskManager.Run(work, parameters)", false)]
  public partial class BackgroundWorkerForm : LocalizableForm
  {
	  [NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BackgroundWorkerForm));

    public BackgroundWorkerForm()
    {
      InitializeComponent();
    }


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Задача для выполнения
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBackgroundWork Work { get; set; }

    public int InfoLabelWidth{ get { return m_label_state.Width;}}
    public Font InfoLabelFont{ get { return m_label_state.Font;}}

    #endregion

    public event EventHandler<RunWorkerCompletedEventArgs> Completed;

    #region Private Methods -----------------------------------------------------------------------------------------------------

    private void HandleCancel(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Abort;
      m_worker.CancelAsync();
    }

    private void RunBackgroundWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        Thread.CurrentThread.CurrentUICulture = AppSwitchablePool.CurrentLocale;
        Work.Run(sender as BackgroundWorker);
      }
      catch (Exception ex)
      {
        //По идее, здесь должно быть 
        // AppManager.Notificator.Log("AppFramework", new Info(ex))
        // но у GuiToolbox нет ссылки на ERMS.Core
        _log.Error("RunBackgroundWork(): exception", ex);

        this.DialogResult = DialogResult.Cancel;
      }
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

	  private void HandleProgressChange([NotNull] object sender, [NotNull] ProgressChangedEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

	    try
	    {
		    if (null != e.UserState)
			    m_label_state.Text = e.UserState.ToString();

		    if (Work.IsMarquee)
		    {
			    m_marquee_bar.EditValue = ((int)m_marquee_bar.EditValue) + 1;
			    m_marquee_bar.Invalidate();
		    }
		    else
		    {
			    SetPercentageWithCorrection(e.ProgressPercentage);
			    m_progress_bar.Invalidate();
		    }
	    }
	    catch (Exception ex)
	    {
		    _log.Error("HandleProgressChange(): exception", ex);
	    }

	    m_label_state.Invalidate();
    }

    private void HandleWorkComnpleted([NotNull] object sender, [NotNull] RunWorkerCompletedEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			try
      {
        m_marquee_bar.Properties.Stopped = true;
				m_progress_bar.Position = m_progress_bar.Properties.Maximum;

	      if (this.Work != null)
	      {
		      this.Work.PropertyChanged -= this.HandlePropertyChanged;

		      if (Work.CloseOnFinish)
		      {
			      DialogResult = DialogResult.OK;
			      Close();
		      }
		      else
		      {
			      m_cancel_btn.Enabled = true;
			      m_cancel_btn.Text = Resources.OK;
		      }
	      }
      }
      finally
      {
        if (null != Completed)
          Completed( Work, e );
      }
    }

	  private void HandleFormShown(object sender, EventArgs e)
	  {
		  if (null != Work)
		  {
			  m_picture_box.Image = Work.Icon;
			  m_picture_box.Invalidate();
			  Text = Work.Name;
			  m_group.Text = Work.Name;
			  m_cancel_btn.Enabled = Work.CanCancel;

			  if (Work.IsMarquee)
			  {
				  m_progress_bar.Hide();
				  m_marquee_bar.Show();
			  }
			  else
			  {
				  m_marquee_bar.Hide();
				  m_progress_bar.Show();
			  }

			  Work.PropertyChanged += this.HandlePropertyChanged;

			  m_worker.RunWorkerAsync();
		  }
		  else
		  {
			  DialogResult = DialogResult.Abort;
			  Close();
		  }
	  }

	  private void HandlePropertyChanged([NotNull] object obj, [NotNull] PropertyChangedEventArgs args)
    {
	    if (obj == null) throw new ArgumentNullException("obj");
	    if (args == null) throw new ArgumentNullException("args");

			if (this.InvokeRequired)
      {
        this.BeginInvoke(new PropertyChangedEventHandler(this.HandlePropertyChanged), obj, args);
      }
      else
      {
        var work = obj as IBackgroundWork;

	      if (work != null)
	      {
		      this.ChangeIsMarqee(work.IsMarquee);
		      this.Text = work.Name;
		      m_cancel_btn.Enabled = work.CanCancel;
		      m_picture_box.Image = work.Icon;
	      }
      }
    }

	  void ChangeIsMarqee(bool value)
    {
      if (value)
      {
        m_progress_bar.Hide();
        m_marquee_bar.Show();
      }
      else
      {
        m_marquee_bar.Hide();
        m_progress_bar.Show();
      }
    }

    #endregion

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_cancel_btn.Text = Resources.CANCEL;
    }
  }
}