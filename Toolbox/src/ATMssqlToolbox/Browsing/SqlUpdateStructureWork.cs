using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.Misc;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для обновления структуры базы данных
  /// </summary>
  internal class SqlUpdateStructureWork : IBackgroundWork
  {
    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return true; }
    }

    public bool IsMarquee
    {
      get { return false; }
    }

    public object Result
    {
      get { return null; }
    }


    public bool CanCancel
    {
      get { return false; }
    }

    public System.Drawing.Bitmap Icon
    {
      get { return Properties.Resources.p_48_database_script; }
    }

    public string Name
    {
      get { return Properties.Resources.DB_BROWSER_RESET_DB; }
    }

    public float Weight
    {
      get { return 0; }
    }

    public string ConnectionString { get; set; }

    public bool SoftUpdate { get; set; }

    public bool CompatibilityModeSql2005 { get; set; }
   
    public bool UseFileStreams { get; set; }

    public bool ContinueOnErrorInScripts { get; set; }

    public bool SkipFkTest { get; set; }

    public bool ReCreateTables { get; set; }

    public ISpecificDBRoutines Routines { get; set; }

    public DatabaseEntry Database { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      if (this.SoftUpdate)
      {
        this.Routines.UpdateStructure(this.ConnectionString, worker, this.SkipFkTest, this.CompatibilityModeSql2005, this.UseFileStreams, this.ContinueOnErrorInScripts, this.ReCreateTables);
      }
      else
      {
        this.Routines.InitStructure(this.ConnectionString, worker, this.CompatibilityModeSql2005, this.UseFileStreams, this.ContinueOnErrorInScripts);
      }
      if (this.Database != null)
      {
        this.Database.ApplySupportInfo(this.Routines.Supported(this.ConnectionString, worker));
      }
    }

    #endregion
  }
}
