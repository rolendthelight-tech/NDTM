using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using AT.Toolbox.Misc;
using System.Threading;
using System.Collections.Generic;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для создания слепка базы данных
  /// </summary>
  internal class SqlBuildMouldWork : IBackgroundWork
  {
    private readonly ISpecificDBRoutines m_routines;

    public SqlBuildMouldWork(ISpecificDBRoutines routines)
    {
      if (routines == null)
        throw new ArgumentNullException("routines");

      m_routines = routines;
    }
    
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

    public Bitmap Icon
    {
      get { return Properties.Resources.p_48_database_script ; }
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

    public string MouldPath { get; set; }

    public DatabaseEntry Database { get; set; }

    public bool TestScriptObjects { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      var support = m_routines.Build(this.ConnectionString, this.Version, this.MouldPath, worker);

      if (support != null && this.Database != null)
        this.Database.ApplySupportInfo(support);

      //DatabaseMould mould = MouldBuilder.BuildWithVersionUpdate(new SqlConnection(this.ConnectionString), worker, this.Version);
      
      //if (mould != null)
      //{
      //  mould.Save(Path.Combine(Application.StartupPath, this.MouldPath));
      //  if (Path.GetFullPath(this.MouldPath) 
      //    == Path.GetFullPath(DatabaseBrowserControl.Preferences.MouldFilePath)
      //    && this.Database != null)
      //  {
      //    this.Database.ApplySupportInfo(mould.GetDatabaseDescription());
      //  }
      //}
    }

    #endregion
  }
}
