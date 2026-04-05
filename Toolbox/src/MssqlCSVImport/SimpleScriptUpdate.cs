using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.MSSQL;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using AT.Toolbox.Log;
using System.ComponentModel;

namespace RegisterDataImport
{
  [DisplayName("Выполнение скрипта")]
  public class SimpleScriptUpdate : IDataImporter
  {
    #region IDataImporter Members

    public bool Import(DatabaseBrowserControl control)
    {
      string connectionString = control.CheckConnectionString(control.CurrentDatabase);

      OpenFileDialog dlg = new OpenFileDialog();

      dlg.Filter = "Sql files (*.sql)|*.sql";

      int recordCount = 0;

      if (dlg.ShowDialog(control) == DialogResult.OK)
      {
        string[] commands = File.ReadAllText(dlg.FileName).Split(new string[]{"GO"}, StringSplitOptions.RemoveEmptyEntries);

        foreach (string cmd in commands)
        {
          try
          {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
              conn.Open();

              SqlCommand comm = new SqlCommand(cmd, conn);
              comm.CommandType = System.Data.CommandType.Text;

              int result = comm.ExecuteNonQuery();

              Log.GetLogger("Sql script update").Info("(" + result + ") rows affected");
            }
          }
          catch (Exception ex)
          {
            Log.GetLogger("Sql script update").Error(cmd, ex);
            recordCount++;
          }
        }

        return recordCount == 0;
      }
      
      return false;
    }

    #endregion
  }
}
