using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AT.Toolbox.Constants;

namespace AT.Toolbox.Base
{
  public partial class MinimizeToTrayForm : LocalizableForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MinimizeToTrayForm).Name);

    protected NotifyIcon m_notifyIcon = new NotifyIcon();
    protected bool m_minimize_to_tray;

    public MinimizeToTrayForm()
    {
      InitializeComponent();

      OnRestore += delegate { };
      OnMinimize += delegate { };

      m_notifyIcon.DoubleClick += Restore;
    }

    private void Restore(object sender, EventArgs e)
    {
      MinimizedToTray = false;
    }


    public event EventHandler OnRestore;


    public event EventHandler OnMinimize;


    public Icon NotifyIcon
    {
      get { return m_notifyIcon.Icon; }
      set { m_notifyIcon.Icon = value; }
    }


    public bool MinimizeFormToTray
    {
      get { return m_minimize_to_tray; }
      set { m_minimize_to_tray = value; }
    }


    public ContextMenuStrip NotifyMenu
    {
      get { return m_notifyIcon.ContextMenuStrip; }
      set { m_notifyIcon.ContextMenuStrip = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected bool MinimizedToTray
    {
      get { return m_notifyIcon.Visible; }
      set
      {
        m_notifyIcon.Visible = value;
        Visible = !value;
        ShowInTaskbar = !value;

        if (value)
          OnRestore(this, EventArgs.Empty);
      }
    }

    public void ShowBaloon(string title, string message, int timeout, ToolTipIcon ico)
    {
      m_notifyIcon.ShowBalloonTip(timeout, title, message, ico);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      m_notifyIcon.Visible = false;
      base.OnClosing(e);
    }

    protected override void WndProc(ref Message m)
    {
      try
      {
        if (m.Msg == (int) MessageCodes.WmCommand)
        {
          if (m.WParam == (IntPtr) CommandCodes.ScMinimize)
          {
            if (m_minimize_to_tray)
              MinimizedToTray = true;

            OnMinimize(this, EventArgs.Empty);
          }
        }

        base.WndProc(ref m);
      }
      catch (Exception ex)
      {
        Log.Error("WndProc(): exception", ex);
      }
    }
  }
}