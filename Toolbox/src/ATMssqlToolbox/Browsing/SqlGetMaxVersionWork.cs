using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using AT.Toolbox.Misc;
using System.Threading;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для определения версии базы данных
  /// </summary>
  internal class SqlGetMaxVersionWork : IBackgroundWork
  {
    private readonly ISpecificDBRoutines m_routines;

    public SqlGetMaxVersionWork(ISpecificDBRoutines routines)
    {
      if (routines == null)
        throw new ArgumentNullException("routines");

      m_routines = routines;
    }
    
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
      get { return Properties.Resources.DB_BROWSER_MOULD; }
    }

    public float Weight
    {
      get { return 1; }
    }
    
    public string Version { get; set; }

    public string ConnectionString { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      this.Version = m_routines.GetMaxVersion(this.ConnectionString, worker);
    }
  }
}
