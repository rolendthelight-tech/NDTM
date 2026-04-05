using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AT.Toolbox.MSSQL.Extensions
{
  public static class DataRowExtensions
  {
    public static Dictionary<string, object> GetValuesAsDictionary(this DataRow rw )
    {
      Dictionary<string, object> ret_val = new Dictionary<string, object>();

      foreach (DataColumn column in rw.Table.Columns)
        ret_val.Add( column.ColumnName, rw[column] );
      
      return ret_val;
    }

    public static object PrimaryKeyValue(this DataRow rw, int index )
    {
      return rw[rw.Table.PrimaryKey[index]];
    }

    public static object PrimaryKeyValue(this DataRow rw)
    {
      return rw[rw.Table.PrimaryKey[0]];
    }
  }
}
