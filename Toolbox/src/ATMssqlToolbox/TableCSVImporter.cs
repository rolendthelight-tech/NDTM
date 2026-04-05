using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

using AT.Toolbox.Files;


namespace AT.Toolbox.MSSQL
{
  public class TableCSVImporter
  {
    public Dictionary<string, int> ColumnMapping = new Dictionary<string, int>();

    public void Import(string FileName, DataTable tbl, string Encoding, string Separator)
    {
      CsvFile file = new CsvFile();
      file.UsedEncoding = Encoding;
      file.Separator = Separator;

      List<string[]> table;

      file.ReadLines(FileName, out table);

      List<string> keys_to_remove = new List<string>();

      foreach (KeyValuePair<string, int> pair in ColumnMapping)
      {
        if (!tbl.Columns.Contains(pair.Key))
          keys_to_remove.Add(pair.Key);
      }

      foreach (string s in keys_to_remove)
        ColumnMapping.Remove(s);

      foreach (string[] strings in table)
      {
        DataRow rw = tbl.NewRow();

        foreach (KeyValuePair<string, int> pair in ColumnMapping)
        {
          if (pair.Value >= strings.Count())
            continue;

          if (string.IsNullOrEmpty(strings[pair.Value]))
            continue;

          if (strings[pair.Value].ToUpper() == "NULL"
            || string.IsNullOrEmpty(strings[pair.Value]))
          {
            rw[pair.Key] = Convert.DBNull;
          }
          else
          {
            TypeConverter conv = TypeDescriptor.GetConverter(rw.Table.Columns[pair.Key].DataType);
            rw[pair.Key] = conv.ConvertFromString(strings[pair.Value]);
          }
        }
        if (tbl.Columns.Contains("ObjectGuid") && !ColumnMapping.ContainsKey("ObjectGuid"))
        {
          rw["ObjectGuid"] = Guid.NewGuid();
        }

        tbl.Rows.Add(rw);
      }
    }
  }
}
