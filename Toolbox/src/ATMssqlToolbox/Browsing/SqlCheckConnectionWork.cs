using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using AT.Toolbox.Misc;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для проверки соединения с сервером
  /// </summary>
  public class SqlCheckConnectionWork : IBackgroundWork
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlCheckConnectionWork).Name);

    protected bool m_success;

    #region IBackgroundWork Members

    public bool CloseOnFinish { get; set; }

    public bool CanCancel
    {
      get { return true;}
    }

    public bool IsMarquee
    {
      get { return true; }
    }

    public object Result
    {
      get { return null; }
    }

    public Bitmap Icon
    {
      get { return Properties.Resources.p_48_server_search; }
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

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      Thread th = new Thread(ThreadRoutine);
      
      try
      {
        worker.ReportProgress(0, Properties.Resources.SQL_CHECKING_SERVER);
        
        th.Start();

        while( !th.Join(100) )
        {
          if (!worker.CancellationPending) 
            continue;

          m_success = false;
          th.Abort(); 
          break;
        }
      }
      catch (Exception ex )
      {
        Log.Error("Run(): exception", ex);
        m_success = false;
      }
      finally
      {
        if( th.IsAlive )
          th.Abort();
      }

      if( m_success )
        worker.ReportProgress(100, Properties.Resources.SQL_CHECKING_SUCCESS);
      else 
        worker.ReportProgress(100, Properties.Resources.SQL_CHECKING_FAILURE);
    }

    #endregion

    protected void ThreadRoutine()
    {
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      try
      {
        //SmoUtil.ProcessServer(new SqlConnection(this.ConnectionString), new BackgroundWorker(),
        //  delegate(Server server)
        //  {
        //    server.Databases.GetEnumerator();
        //    m_success = true;
        //  });
        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
        {
          connection.Open();
          var dt = connection.GetSchema("Databases");
          if (dt != null)
            m_success = true;
          connection.Close();
        }
      }
      catch (ThreadAbortException) { }
      catch (Exception ex)
      {
        //Log.Error(Properties.Resources.SQL_CHECKING_CONNECTION_FAIL, ex); // TODO: MessageService
        Log.Error("ThreadRoutine(): exception", ex);
        m_success = false;
      }
    }

    public bool Sucess
    {
      get { return m_success; }
    }
  }
}