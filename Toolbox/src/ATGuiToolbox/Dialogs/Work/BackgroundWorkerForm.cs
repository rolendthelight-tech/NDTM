using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AT.Toolbox.Base;
using AT.Toolbox.Misc;
using AT.Toolbox.Properties;
using System.Threading;

using DevExpress.XtraEditors;



namespace AT.Toolbox.Dialogs
{
  /// <summary>
  /// ƒиалог дл€ отображени€ прогресса фоновой обработки задачи
  /// </summary>
  public partial class BackgroundWorkerForm : LocalizableForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BackgroundWorkerForm).Name);

    public BackgroundWorkerForm()
    {
      InitializeComponent();
    }

    private DevExpress.XtraEditors.MarqueeProgressBarControl m_marquee_progress = new MarqueeProgressBarControl(  );
    private DevExpress.XtraEditors.ProgressBarControl m_progress = new ProgressBarControl(  );

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// «адача дл€ выполнени€
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBackgroundWork Work { get; set; }
    
    public int InfoLabelWidth{ get { return m_info_label.Width;}}
    public Font InfoLabelFont{ get { return m_info_label.Font;}}


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
        //ѕо идее, здесь должно быть 
        // AppManager.Notificator.Log("AppFramework", new Info(ex))
        // но у GuiToolbox нет ссылки на ERMS.Core
        Log.Error("RunBackgroundWork(): exception", ex);
        
        this.DialogResult = DialogResult.Cancel;
      }
    }

    private void HandleProgressChange(object sender, ProgressChangedEventArgs e)
    {
      if (null != e.UserState)
        m_info_label.Text = e.UserState.ToString();

      try
      {
        if( Work.IsMarquee )
        {
          m_marquee_progress.EditValue = ( (int) m_marquee_progress.EditValue ) + 1;
          m_marquee_progress.Refresh( );
        }
        else
        {
          m_progress.Position = e.ProgressPercentage;
          m_progress.Refresh( );
        }
      }
      catch (Exception ex)
      {
        Log.Error("HandleProgressChange(): exception", ex);
      }

      m_info_label.Refresh();
    }

    private void HandleWorkComnpleted(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        m_marquee_progress.Properties.Stopped = true;
        m_progress.Position = 100;

        if (this.Work == null)
          return;

        this.Work.PropertyChanged -= this.HandlePropertyChanged;

        if( Work.CloseOnFinish )
        {
          DialogResult = DialogResult.OK;
          Close( );
        }
        else
        {
          m_cancel_btn.Enabled = true;
          m_cancel_btn.Text = Resources.OK;
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
        m_picture_box.Refresh( );
        Text = Work.Name;
        m_group.Text = Work.Name;
        m_cancel_btn.Enabled = Work.CanCancel;

        if (Work.IsMarquee)
        {
          panelControl1.Controls.Add( m_marquee_progress );
          m_marquee_progress.Dock = DockStyle.Fill;
        }
        else
        {
          panelControl1.Controls.Add(m_progress);
          m_progress.Dock = DockStyle.Fill;
        }

        Work.PropertyChanged += this.HandlePropertyChanged;

        m_worker.RunWorkerAsync();

        return;
      }

      DialogResult = DialogResult.Abort;
      Close();
    }

    private void HandlePropertyChanged(object obj, PropertyChangedEventArgs args)
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new PropertyChangedEventHandler(this.HandlePropertyChanged), obj, args);
        return;
      }
      else
      {
        var work = obj as IBackgroundWork;

        if (work == null)
          return;

        this.ChangeIsMarqee(work.IsMarquee);
        this.Text = work.Name;
        m_cancel_btn.Enabled = work.CanCancel;
        m_picture_box.Image = work.Icon;
      }
    }

    void ChangeIsMarqee(bool value)
    {
      panelControl1.Controls.Clear();

      if (value)
      {
        if (panelControl1.Controls.Contains(m_progress))
          panelControl1.Controls.Remove(m_progress);

        panelControl1.Controls.Add(m_marquee_progress);
        m_marquee_progress.Dock = DockStyle.Fill;
      }
      else
      {
        if (panelControl1.Controls.Contains(m_marquee_progress))
          panelControl1.Controls.Remove(m_marquee_progress);

        panelControl1.Controls.Add(m_progress);
        m_progress.Dock = DockStyle.Fill;
      }
    }

    #endregion

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_cancel_btn.Text = Resources.CANCEL;
    }
  }
}