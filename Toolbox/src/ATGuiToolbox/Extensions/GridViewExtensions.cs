using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Extensions
{
  using System.Data;

  using DevExpress.XtraGrid.Views.Grid;
  
  public static class GridViewExtensions
  {
    public static int GetRowHandleByDataRow(this GridView view, DataRow rw)
    {
      for (int i = 0; i < view.RowCount; i++)
      {
        if (rw == view.GetDataRow(i))
          return i;
      }

      return -1;
    }

    public static int GetRowHandleByData(this GridView view, object Data)
    {
      for (int i = 0; i < view.RowCount; i++)
      {
        if (Data == view.GetRow(i))
          return i;
      }

      return -1;
    }
  }
}
