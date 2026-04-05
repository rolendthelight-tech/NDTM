using System;
using System.Security;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Base
{
  public partial class LocalizableForm : XtraForm, ISwitchableLanguage
  {
    private readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LocalizableForm).Name);

    public LocalizableForm()
    {
      InitializeComponent();
      Terminating += delegate { };
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

    protected override void OnClosed(EventArgs e)
    {
      Terminating( this, EventArgs.Empty );
      base.OnClosed(e);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      InitLocalization();
    }

    protected Cursor m_cur = Cursors.Default;

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
          Cursor = m_cur ?? Cursors.Default;
      }
    }

    protected override void WndProc(ref Message msg)
    {
      try
      {
        base.WndProc(ref msg);
      }
      catch( SecurityException )
      {
         throw;
      }
      catch (Exception ex)
      {
        //TODO: íå î÷åíü êðàñèâî, íî íå õî÷åòñÿ äåëàòü ðåôåðåíñ â ýòîì ïðîåêòå íà ERMS.Security
        if (ex.GetType().Name == "SecurityControlException") 
          throw;

        Log.Error(string.Format("WndProc({0}): exception", msg), ex);
      }
    }

    private void LocalizableForm_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
    }

    public string HelpURI
    {
      get;
      set;
    }
  }
}