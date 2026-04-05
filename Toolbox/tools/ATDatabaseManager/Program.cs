using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Dialogs;
using ATDatabaseManager.Properties;
using AT.Toolbox.MSSQL.DAL.RecordBinding;
using AT.Toolbox.MSSQL.DAL.RecordBinding.Sql;

namespace ATDatabaseManager
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
      RecordManager.SetWriteLogStrategy(new ATWriteLogStrategy());
      RecordManager.SetBinderFactory(new SqlRecordBinderFactory());
      AppWrapper.Run<MainForm>(null, delegate
      {
        RecordManager.AutoLoadAspects();
      }, true);
    }
  }
}
