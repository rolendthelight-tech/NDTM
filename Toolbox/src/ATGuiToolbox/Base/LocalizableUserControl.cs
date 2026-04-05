using System;
using System.Windows.Forms;
using AT.Toolbox.Constants;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Base
{
  public partial class LocalizableUserControl : XtraUserControl, ISwitchableLanguage
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LocalizableUserControl));

    public LocalizableUserControl()
    {
      InitializeComponent();
      Terminating += delegate { };
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      InitLocalization();
    }

    protected override void WndProc(ref Message m)
    {
      try
      {
        base.WndProc( ref m );

        if( m.Msg == (int)MessageCodes.WmDestroy )
          OnControlDestroyed( );        
      }
      catch( Exception ex )
      {
        Log.Error("WndProc(): exception", ex);
      }
    }

    public virtual void OnControlDestroyed()
    {
      Terminating(this, EventArgs.Empty);
    }

    public void InitLocalization()
    {
      AppSwitchablePool.RegisterSwitchable(this);
      PerformLocalization(this, EventArgs.Empty);
    }

    public virtual void PerformLocalization(object sender, EventArgs e)
    {
    }

    public event EventHandler Terminating;

    protected Cursor m_cur = Cursor.Current;

    public new bool UseWaitCursor
    {
      get { return base.UseWaitCursor; }
      set
      {
        base.UseWaitCursor = value;

        if (value)
        {
          m_cur = Cursor.Current;
          Cursor = Cursors.WaitCursor;
        }
        else
          Cursor = m_cur;
      }
    }

    private void LocalizableUserControl_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
    }

    public string HelpURI
    {
      get; set;
    }
  }
}