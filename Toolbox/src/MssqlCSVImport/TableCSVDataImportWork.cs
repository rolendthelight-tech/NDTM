using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.Misc;
using RegisterDataImport.Properties;
using System.ComponentModel;
using AT.Toolbox.MSSQL;
using System.Data;
using System.Data.SqlClient;

namespace AT.Toolbox.MSSQL
{
  public class TableCSVDataImportWork : IBackgroundWork
  {
    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return true; }
    }

    public bool CanCancel
    {
      get { return false; }
    }

    public System.Drawing.Bitmap Icon
    {
      get { return null; }
    }

    public bool IsMarquee
    {
      get { return false; }
    }

    public object Result
    {
      get { return null; }
    }

    public string Name
    {
      get { return Resources.ImportForm_Title; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public string ConnectionString { get; set; }

    public Dictionary<string, int> ColumnMapping { get; set; }

    public DataTable DataTable { get; set; }

    public string FileName { get; set; }

    public string Encoding { get; set; }

    public string Separator { get; set; }

    public void Run(BackgroundWorker worker)
    {
      this.PropertyChanged += delegate { };
      TableCSVImporter imp = new TableCSVImporter();
      imp.ColumnMapping = this.ColumnMapping;
      DataTable table = this.DataTable;

      worker.ReportProgress(0, string.Format(Resources.ImportForm_TableTitle, table.TableName));
      imp.Import(this.FileName, table, this.Encoding, this.Separator);

      string commandText = "INSERT INTO [" + table.TableName + "] (";
      bool first = true;
      foreach (DataColumn column in table.Columns)
      {
        if (column.AutoIncrement)
          continue;

        if (first)
        {
          first = false;
        }
        else
        {
          commandText += ", ";
        }
        commandText += "[" + column.ColumnName + "]";
      }
      commandText += ") VALUES (";

      first = true;
      foreach (DataColumn column in table.Columns)
      {
        if (column.AutoIncrement)
          continue;

        if (first)
        {
          first = false;
        }
        else
        {
          commandText += ", ";
        }
        commandText += "@" + column.ColumnName;
      }
      commandText += ")";

      using (SqlConnection conn = new SqlConnection(this.ConnectionString))
      {
        conn.Open();
        for (int i = 0; i< table.Rows.Count; i++)
        {
          DataRow row = table.Rows[i];
          if (row.RowState == DataRowState.Added)
          {
            SqlCommand cmd = new SqlCommand(commandText, conn);
            foreach (DataColumn column in table.Columns)
            {
              if (column.AutoIncrement)
                continue;

              SqlParameter parm = new SqlParameter(column.ColumnName, row[column.ColumnName]);
              parm.IsNullable = column.AllowDBNull;
              cmd.Parameters.Add(parm);
            }
            cmd.ExecuteNonQuery();
          }
          worker.ReportProgress(i * 100 / table.Rows.Count);
        }
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}
