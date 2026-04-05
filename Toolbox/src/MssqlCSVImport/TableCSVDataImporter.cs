using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using AT.Toolbox.Dialogs;
using System.Windows.Forms;

namespace AT.Toolbox.MSSQL
{
  [DisplayName("Simple CSV import")]
  public class TableCSVDataImporter : IDataImporter
  {
    #region IDataImporter Members

    public bool Import(DatabaseBrowserControl control)
    {
      CSVImportForm form = new CSVImportForm();
      form.Encoding = "utf-8";
      string connectionString = control.CheckConnectionString(control.CurrentDatabase);
      try
      {
        form.SetupConnection(new SqlConnection(connectionString));
      }
      catch (Exception ex)
      {
        Log.Log.GetLogger("TableCSVDataImporter").Error(ex.Message, ex);
        return false;
      }
      if (form.ShowDialog(control) == System.Windows.Forms.DialogResult.OK)
      {
        TableCSVDataImportWork wrk = new TableCSVDataImportWork();
        wrk.ColumnMapping = form.ColumnMapping;
        wrk.DataTable = form.GetDataTable();
        wrk.FileName = form.FileName;
        wrk.Encoding = form.Encoding;
        wrk.Separator = form.Separator;
        wrk.ConnectionString = connectionString;

        BackgroundWorkerForm frm = new BackgroundWorkerForm();
        frm.Work = wrk;

        return frm.ShowDialog(control) == DialogResult.OK;
      }
      return false;
    }

    #endregion
  }
}
