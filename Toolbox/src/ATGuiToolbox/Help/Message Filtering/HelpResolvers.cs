using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;

namespace AT.Toolbox.Help
{
  public class HelpControlResolver
  {
    public virtual Component ResolveHelpTarget(Control ctl)
    {
      return null;
    }
  }

  public static class HelpResolvers
  {
    private static Dictionary<Type, HelpControlResolver> m_resolvers;

    static HelpResolvers()
    {
      m_resolvers = new Dictionary<Type, HelpControlResolver>();

      m_resolvers.Add(typeof (BarDockControl), new BarDockHelpResolver());
      m_resolvers.Add(typeof (TreeList), new TreeListResolver());
    }

    public static Component ResolveHelpTarget(Control ctl)
    {
      Type t = ctl.GetType();

      if (m_resolvers.ContainsKey(t))
        return m_resolvers[t].ResolveHelpTarget(ctl);

      return DefaultResolveHelpTarget(ctl);
    }

    public static Component DefaultResolveHelpTarget(Control ctl)
    {
      //TODO: Implementation
      return null;
    }
  }
}