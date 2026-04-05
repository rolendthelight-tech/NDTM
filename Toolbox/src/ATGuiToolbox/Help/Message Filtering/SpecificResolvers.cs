using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;

namespace AT.Toolbox.Help
{
  public class TreeListResolver : HelpControlResolver
  {
    public override Component ResolveHelpTarget(Control ctl)
    {
      TreeList lst = ctl as TreeList;
      Point pt = Cursor.Position;

      TreeListHitInfo info = lst.CalcHitInfo(ctl.PointToClient(pt));

      if (null != info)
        return info.Column;

      return null;
    }
  }

  public class BarDockHelpResolver : HelpControlResolver
  {
    public override Component ResolveHelpTarget(Control ctl)
    {
      BarDockControl bd = ctl as BarDockControl;

      Point pt = Cursor.Position;

      foreach (BarItem itm in bd.Manager.Items)
      {
        foreach (BarItemLink lnk in itm.Links)
        {
          if (lnk.ScreenBounds.Contains(pt))
            return itm;
        }
      }
      return null;
    }
  }
}