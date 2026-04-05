using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AT.Toolbox.Misc;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Dialogs
{
  /// <summary>
  /// Диалог при загрузке программы. Растягивается под картинку. В остальном аналогичен BackgroundWorkerForm
  /// </summary>
  public partial class SplashScreen : XtraForm
  {
    public SplashScreen()
    {
      InitializeComponent();

      m_version_info.Text = ApplicationInfo.MainAttributes.Version;
    }

    #region Public Properties ---------------------------------------------------------------------------------------------------

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBackgroundWork Work { get; set; }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image Image
    {
      get { return BackgroundImage; }
      set
      {
        if (null == value)
          return;

        BackgroundImage = value;

        Width = Image.Size.Width + 4;
        Height = Image.Size.Height + 4 + m_panel.Height;
      }
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    private void DoBackgroundWork(object sender, DoWorkEventArgs e)
    {
      if (null != Work)
        Work.Run(sender as BackgroundWorker);
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (null != e.UserState)
        m_operation_info.Text = e.UserState.ToString();

      m_progress_bar.Position = e.ProgressPercentage;

      m_operation_info.Refresh();
      m_progress_bar.Refresh();
    }

    private void HandleWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Close();
    }

    private void HandleFormShown(object sender, EventArgs e)
    {
      if (null != Work)
      {
        if (Work.IsMarquee)
        {
          var marquee = new MarqueeProgressBarControl();
          marquee.Size = m_progress_bar.Size;
          marquee.Location = m_progress_bar.Location;
          marquee.Properties.Stopped = false;
          marquee.Dock = m_progress_bar.Dock;
          m_progress_bar.Visible = false;
          m_progress_bar.Parent.Controls.Add(marquee);
          m_progress_bar.Parent.Controls.Remove(m_progress_bar);
          marquee.Visible = true;
          marquee.Refresh();
        }
        
        m_worker.RunWorkerAsync();
        return;
      }

      DialogResult = DialogResult.Abort;
      Close();
    }

    #endregion
  }
}