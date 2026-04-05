using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Linq;
using AT.Toolbox.Misc;

namespace AT.Toolbox.MSSQL
{
  //using Microsoft.SqlServer.Management.Smo;
  using AT.Toolbox.MSSQL.Properties;
  using System.Data.Common;

  /// <summary>
  /// Служебная задача для загрузки списка баз данных на сервере
  /// </summary>
  internal class SqlGetBasesWork : IBackgroundWork
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlGetBasesWork).Name);

    /// <summary>
    /// Содержимое файла протокола
    /// </summary>
    public List<string> strings = new List<string>();

    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return true; }
    }

    public bool CanCancel
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
    
    public Bitmap Icon
    {
      get { return Properties.Resources.p_48_server_wait; }
    }

    public string Name
    {
      get { return Properties.Resources.SQL_CHECKING_SERVER; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public string ConnectionString { get; set; }

    public ISpecificDBRoutines Routines { get; set; }

    public List<DatabaseEntry> Entries { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      SqlConnection conn = new SqlConnection(ConnectionString);

      try
      {
        if (null == Routines)
          return;

        worker.ReportProgress(0, Properties.Resources.SQL_CHECKING_SERVER + " " + conn.DataSource);
        Thread.Sleep(100);
        
        if( worker.CancellationPending )
        {
          worker.ReportProgress(100, string.Format(Properties.Resources.SQL_CHECKING_SUCCESS, conn.DataSource));
          return;
        }

        worker.ReportProgress(10, string.Format(Properties.Resources.SQL_GETTING_BASES, conn.DataSource));

        List<string> databases = GetDatabaseList(conn, worker);

        if (null == databases)
          throw new ApplicationException(string.Format(Resources.SQL_GET_DATABASE_LIST_FAIL, this.ConnectionString));

        this.Entries = new List<DatabaseEntry>();

        worker.ReportProgress(40, Properties.Resources.SQL_CHECKING_VERSIONS + "  (1/" + databases.Count + ")");

        int counter = 1;

        foreach (string s in databases)
        {
          SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(this.ConnectionString);
          b.InitialCatalog = s;

          try
          {
            SupportInfo support_info = Routines.Supported(b.ConnectionString, worker);

            DatabaseEntry ent = new DatabaseEntry();
            ent.Name = s;
            ent.ApplySupportInfo(support_info);

            this.Entries.Add(ent);

            counter++;
            worker.ReportProgress(40 + (int)60 * counter / databases.Count
              , Properties.Resources.SQL_CHECKING_VERSIONS + "  (" + counter + "/" + databases.Count + ")");
          }
          catch (Exception ex)
          {
            Log.Error("Run(): exception", ex);
          }
        }
      }
      catch (Exception ex )
      {
        Log.Error("Run(): exception 2", ex);
        //m_log.Error(Properties.Resources.SQL_CHECKING_CONNECTION_FAIL, ex); // TODO: MessageService
        worker.ReportProgress(100, Properties.Resources.FAIL);
        return;
      }
      finally
      {
        if( ConnectionState.Open == conn.State )
          conn.Close(); 
      }

      worker.ReportProgress(100, Properties.Resources.OK);
    }

    #endregion

    //protected List<string> GetDatabaseList_orig(SqlConnection connection, BackgroundWorker w)
    //{
    //  try
    //  {
    //    List<string> ret = null;
    //    SmoUtil.ProcessServer(connection, w, delegate(Server server)
    //    {
    //      ret = (from Database p in server.Databases
    //             where !p.IsSystemObject
    //             select p.Name).ToList();
    //    });
    //    return ret;
    //  }
    //  catch (Exception e)
    //  {
    //    Log.Error("GetDatabaseList(): exception", e);
    //    //m_log.Error(string.Format(Resources.SQL_GET_DATABASE_LIST_FAIL, connection.ConnectionString), e); // TODO: MessageService
    //    return null;
    //  }
    //}

    protected List<string> GetDatabaseList(SqlConnection connection, BackgroundWorker w)
    {
      try
      {
        List<string> ret = new List<string>();
        var connection_state = connection.State;
        if (ConnectionState.Closed == connection_state) connection.Open();
        var dt = connection.GetSchema("Databases");
        if (ConnectionState.Closed == connection_state) connection.Close();

        List<string> system_dbid = new List<string>() { "master", "model", "msdb", "tempdb" };
        foreach (DataRow row in  dt.Rows)
        {
          if (!system_dbid.Exists(delegate(String str)
          {
            return (str == row["database_name"].ToString().ToLowerInvariant());
          }))
          ret.Add(row["database_name"].ToString());
        }
        
        return ret;
      }
      catch (Exception e)
      {
        Log.Error("GetDatabaseList(): exception", e);
        //m_log.Error(string.Format(Resources.SQL_GET_DATABASE_LIST_FAIL, connection.ConnectionString), e); // TODO: MessageService
        return null;
      }
    }
  }
}