using System;
using System.Collections.Generic;
using System.Linq;
using AT.Toolbox.MSSQL.Browsing;
using AT.Toolbox.Misc;
using AT.Toolbox.MSSQL.Properties;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Security.AccessControl;
using System.IO;
using System.Threading;
using System.Data.Linq;
using System.Globalization;
using ERMS.Core.DAL;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для восстановления базы данных
  /// </summary>
  internal class SqlRestoreWork : IBackgroundWork
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlRestoreWork).Name);

    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return false; }
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
      get { return Resources.p_48_database_restore; }
    }

    public string Name
    {
      get { return Resources.DB_BROWSER_RESTORE; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public string ConnectionString { get; set; }

    public string Path { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker wrk)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      wrk.ReportProgress(0, Resources.SQL_CHECK_SERVER);
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(this.ConnectionString);
      string database = builder.InitialCatalog;
      builder.InitialCatalog = "master";

      try
      {
        FileInfo fi = new FileInfo(this.Path);
        FileSecurity fSecurity = fi.GetAccessControl();
        string accountName = "NETWORK SERVICE";
        FileSystemRights rigths = FileSystemRights.FullControl;
        fSecurity.SetAccessRule(new FileSystemAccessRule(accountName, rigths, AccessControlType.Allow));
        fi.SetAccessControl(fSecurity);
      }
      catch (UnauthorizedAccessException ex)
      {
        Log.Warn("Run(): Нет прав на изменение прав на исходную директорию (возможно, это не потребуется): ", ex);
      }

      Dictionary<string, string> relocate;

      using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
      {
        conn.Open();

        wrk.ReportProgress(0, "Reading headers");
        relocate = this.GetRelocateInfo();
				using(var remains = new SqlConnection(this.ConnectionString))
				{
					KillAllProcesses(conn, database, new SqlConnection[] {remains,});
				}

      	string cmd_text = "RESTORE DATABASE " + DBObjectOneComponentName.EscapeBySquareBrackets(database) +
          " FROM DISK=" + DBObjectOneComponentName.EscapeByApostrophe(this.Path) + " WITH REPLACE, STATS=1";

        foreach (var kv in relocate)
          cmd_text += ", MOVE " + DBObjectOneComponentName.EscapeByApostrophe(kv.Key) + " TO " + DBObjectOneComponentName.EscapeByApostrophe(kv.Value);

				using (var cmd = new SqlCommand(cmd_text, conn)
					{
						CommandTimeout = int.MaxValue
					})
				{
					conn.FireInfoMessageEventOnUserErrors = true;
					int progress = 0;
					conn.InfoMessage += delegate(object sender, SqlInfoMessageEventArgs e)
						{
							string message = e.Message.Replace("percent", "%");
							wrk.ReportProgress(progress, message);

							foreach (SqlError err in e.Errors)
							{
								var sp = SqlErrorSimpleParser.Parse(err);

								if (sp.IsProgress)
									wrk.ReportProgress(progress = sp.Progress);

								switch (sp.Level)
								{
									case SqlErrorSimpleParser.ErrorLevel.Debug:
										Log.DebugFormat("Run(): {0}", err.Message);
										break;
									case SqlErrorSimpleParser.ErrorLevel.Info:
										Log.InfoFormat("Run(): {0}", err.Message);
										break;
									case SqlErrorSimpleParser.ErrorLevel.Warning:
										Log.WarnFormat("Run(): {0}", err.Message);
										break;
									default:
									case SqlErrorSimpleParser.ErrorLevel.Error:
										Log.ErrorFormat("Run(): {0}", err.Message);
										break;
									case SqlErrorSimpleParser.ErrorLevel.Fatal:
										Log.FatalFormat("Run(): {0}", err.Message);
										break;
								}
							}
						};
					wrk.ReportProgress(progress, "Restoring database");
					//KillAllProcesses(conn, database);
					cmd.ExecuteNonQuery();
				}
      }
    }

		private static void KillAllProcesses(SqlConnection connection, string database, IEnumerable<SqlConnection> remains)
    {
      string command_text;
      //Подготовка к KillAllProcesses
      int server_version_major = int.Parse(connection.ServerVersion.Split('.')[0]);

      command_text = string.Format(CultureInfo.InvariantCulture,
                                   server_version_major == 8
                                     ? "SELECT DISTINCT req_spid AS SPID FROM master.dbo.syslockinfo WHERE rsc_type = 2 AND rsc_dbid = DB_ID(N{0}) AND req_spid > 50 AND req_spid <> @@SPID"
                                     : "SELECT DISTINCT request_session_id AS SPID FROM master.sys.dm_tran_locks WHERE resource_type = 'DATABASE' AND resource_database_id = DB_ID(N{0}) AND request_session_id <> @@SPID",
                                   DBObjectOneComponentName.EscapeByApostrophe(database));
			bool to_kill = false;
			string sql_commands_kill = "USE [master]\r\n";
			using (SqlCommand command = connection.CreateCommand())
			{
				command.CommandText = command_text;
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						sql_commands_kill += string.Format(CultureInfo.InvariantCulture, "KILL {0}\r\n",
						                                   Convert.ToInt32(reader[0], CultureInfo.InvariantCulture));
						to_kill = true;
					}
					reader.Close();
				}
			}
			if (to_kill)
      {
        try
        {
        	//KillAllProcesses
					using (SqlCommand command = new SqlCommand(sql_commands_kill, connection))
					{
						command.ExecuteNonQuery();
					}
        	if (remains != null)
        		foreach (var dead_conn in remains)
        		{
        			if (dead_conn != null)
        				SqlConnection.ClearPool(dead_conn);
        		}
        }
        catch(Exception ex)
        {
					Log.Error(string.Format("KillAllProcesses(…, \"{0}\"): exception", database), ex);
        }
      }
    }

    private Dictionary<string, string> GetRelocateInfo()
    {
      try
      {
        var relocate = new Dictionary<string, string>();

        using (var db = new DataContext(this.ConnectionString))
        {
          var headers = db.ExecuteQuery<DatabaseFileEntry>("RESTORE FILELISTONLY FROM DISK="
            + DBObjectOneComponentName.EscapeByApostrophe(this.Path)).ToArray();

          var files = db.ExecuteQuery<DatabaseFileEntry>(@"SELECT 
	[name] AS LogicalName, 
	[physical_name] AS PhysicalName, 
	(CASE[type_desc] WHEN 'ROWS' THEN 'D' WHEN 'LOG' THEN 'L' WHEN 'FILESTREAM' THEN 'S' ELSE [type_desc] END) AS [Type]
FROM 
	[sys].[database_files]").ToLookup(file => file.Type);

          foreach (var header in headers)
          {
						if (files.Contains(header.Type))
						{
							var filegroup = files[header.Type];

							int file_count = filegroup.Count();
							if (file_count == 1)
							{
								relocate.Add(header.LogicalName, filegroup.Single().PhysicalName);
							}
							else
								Log.WarnFormat("GetRelocateInfo(): предупреждение: для типа файлов БД \"{0}\" найден не один ({1}) такой файл целевой БД, что может привести к ошибке (исходный файл: \"{2}\" (\"{3}\"))", header.Type, file_count, header.LogicalName, header.PhysicalName);
						}
						else
							Log.WarnFormat("GetRelocateInfo(): предупреждение: для типа файлов БД \"{0}\" не найден такой файл целевой БД, что может привести к ошибке (исходный файл: \"{1}\" (\"{2}\"))", header.Type, header.LogicalName, header.PhysicalName);
          }
        }
        return relocate;
      }
      catch (Exception ex)
      {
        Log.Error("GetRelocateInfo(): exception", ex);
        //Log.Log.GetLogger("Restore").Error(ex.Message, ex); // TODO: MessageService
        return new Dictionary<string, string>();
      }
    }

    #endregion
  }

  internal class DatabaseFileEntry
  {
    public string LogicalName { get; set; }

    public string PhysicalName { get; set; }

    public string Type { get; set; }
  }
}
