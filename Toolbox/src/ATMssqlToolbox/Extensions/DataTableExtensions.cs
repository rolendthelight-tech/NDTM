using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AT.Toolbox.MSSQL.Extensions
{
  public static class DataTableExtensions
  {
    public static DataRow NewRowFromPattern(this DataTable tbl, DataRow pattern)
    {
      DataRow ret_val = tbl.NewRow( );

      foreach( DataColumn col in tbl.Columns )
      {
        if( tbl.PrimaryKey.Length > 0 && col == tbl.PrimaryKey[0] )
          continue;
        
        if( pattern.Table.Columns.Contains( col.ColumnName ) )
          ret_val[col] = pattern[col.ColumnName];
      }

      return ret_val;
    }
  }
}
