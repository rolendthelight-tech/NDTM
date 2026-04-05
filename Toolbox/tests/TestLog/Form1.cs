using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Error;
using AT.Toolbox.Log;
using AT.Toolbox.Misc;
using AT.Toolbox.Network;
using AT.Toolbox.Settings;

namespace TestLog
{
  using System.ComponentModel;
  using System.Threading;

  public partial class Form1 : MinimizeToTrayForm
  {
    private BackgroundWorker m_worker;
    private ILog log = AT.Toolbox.Log.Log.GetLogger("test");

    public Form1()
    {
      InitializeComponent();
    }

    private void HandleFormShown(object sender, EventArgs e)
    {
    }

    private void OnSettings(object sender, EventArgs e)
    {
      SettingsForm frm = new SettingsForm();
      frm.ShowDialog(this);
    }
    
    private void HandleRestoreCmd(object sender, EventArgs e)
    {
      MinimizedToTray = false;
    }

    private void HandleWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      while(! m_worker.CancellationPending )
      {
        Throw();
        Thread.Sleep(500);
      }
    }

    void Throw( )
    {
      try
      {
        InnerThrow1( );
      }
      catch (Exception ex)
      {
        log.Error( "Error", ex );
      }
    }

    void InnerThrow1( ) { InnerThrow2(  ); }

    void InnerThrow2() { throw new ApplicationException( "Test" ); }

    private void HandleStart(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_worker = new BackgroundWorker();
      m_worker.WorkerSupportsCancellation = true;
      m_worker.DoWork += HandleWork;
      m_worker.RunWorkerAsync();
    }

    private void HandleStop(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if( null != m_worker )
        m_worker.CancelAsync();
    }

    private void HandlePreferences(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SettingsForm frm = new SettingsForm();
      frm.ShowDialog(this);
    }

    private void HandleAbout(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AboutBox dlg = new AboutBox();
      dlg.ShowDialog(this);
    }

    private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AppInstance.ReqireRestart(  );
    }
  }
}
