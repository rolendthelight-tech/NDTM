using System;
using System.ComponentModel;
using System.Windows.Forms;
using AT.Toolbox.Constants;

namespace AT.Toolbox.Help
{
  public class HelpMessageFilter : IMessageFilter
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(HelpMessageFilter).Name);

    private bool m_ctl_down = false;
    private bool m_shift_down;

    #region IMessageFilter Members

    protected static Component ResolveComponent(IntPtr hwnd, out IHelpSupported parent)
    {
      parent = null;

      try
      {
        Control ctl = Control.FromHandle(hwnd);

        if (null == ctl)
          return null;

        while (null != ctl.Parent)
        {
          if (ctl is IHelpSupported)
          {
            parent = ctl as IHelpSupported;
            break;
          }

          if (null != ctl.Parent)
            ctl = ctl.Parent;
        }

        if (null == parent)
          return null;

        foreach (Control ctl2 in ctl.Controls)
        {
          Component cmp = HelpResolvers.ResolveHelpTarget(ctl2);

          if (null == cmp)
            continue;

          return cmp;
        }
      }
      catch (Exception ex)
      {
        Log.Error("ResolveComponent(): exception", ex);
      }

      return null;
    }

    bool IMessageFilter.PreFilterMessage(ref Message m)
    {
      //Use a switch so we can trap other messages in the future.

      switch ((MessageCodes) m.Msg)
      {
        case MessageCodes.WmKeyUp: // WM_KEYDOWN

          if ((int) m.WParam == (int) Keys.ControlKey || (int) m.WParam == (int) Keys.Control)
            m_ctl_down = false;

          if ((int) m.WParam == (int) Keys.ShiftKey || (int) m.WParam == (int) Keys.Shift)
            m_shift_down = false;


          if ((int) m.WParam == (int) Keys.F1)
          {
            IHelpSupported parent;
            Component cmp = ResolveComponent(m.HWnd, out parent);

            if (null == parent)
              return true;

            if (m_ctl_down)
              ApplicationHelp.ShowFullHelp(parent, cmp);
            else
              ApplicationHelp.GetContextHelp(parent, cmp);

            if (ApplicationHelp.Preferences.DebugMode && m_shift_down)
              Clipboard.SetText(ApplicationHelp.GetHelpIDs(parent, cmp));
          }
          break;
        case MessageCodes.WmKeyDown:
          {
            if ((int) m.WParam == (int) Keys.ControlKey || (int) m.WParam == (int) Keys.Control)
              m_ctl_down = true;

            if ((int) m.WParam == (int) Keys.ShiftKey || (int) m.WParam == (int) Keys.Shift)
              m_shift_down = true;
            break;
          }
      }
      return false;
    }

    #endregion
  }
}