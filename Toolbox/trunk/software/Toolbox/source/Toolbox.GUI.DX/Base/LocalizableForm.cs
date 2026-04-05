using System;
using System.ComponentModel;
using System.Security;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.Extensions;

namespace Toolbox.GUI.DX.Base
{
  public partial class LocalizableForm : XtraForm, ISwitchableLanguage
  {
	  [NotNull] private readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LocalizableForm));

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

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      Terminating( this, EventArgs.Empty );
			base.OnFormClosed(e);
    }

		/// <summary>
		/// OnClosing заменён на OnFormClosing
		/// </summary>
		/// <param name="e">Аргумент</param>
		[Obsolete("Используйте OnFormClosing", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
		}

		/// <summary>
		/// OnClosed заменён на OnFormClosed
		/// </summary>
		/// <param name="e">Аргумент</param>
		[Obsolete("Используйте OnFormClosed", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}

		/// <summary>
		/// Closing заменён на FormClosing
		/// </summary>
		[Obsolete("Используйте FormClosing", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event CancelEventHandler Closing
		{
			add { base.Closing += value; }
			remove { base.Closing -= value; }
		}

		/// <summary>
		/// Closed заменён на FormClosed
		/// </summary>
		[Obsolete("Используйте FormClosed", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler Closed
		{
			add { base.Closed += value; }
			remove { base.Closed -= value; }
		}

  	protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      InitLocalization();
    }

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
        //TODO: не очень красиво, но не хочется делать референс в этом проекте на ERMS.Security
        if (ex.GetType().Name == "SecurityControlException") 
          throw;

				Log.ErrorFormat(ex, "WndProc({0}): exception", msg);
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