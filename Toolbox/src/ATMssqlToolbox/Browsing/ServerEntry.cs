using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using AT.Toolbox.Constants;
using System.Globalization;
using System;
using ERMS.Core.DAL;
using System.Runtime.Serialization;

namespace AT.Toolbox.MSSQL
{
  [DataContract]
  public class ServerEntry
  {
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServerEntry).Name);

    protected SqlConnectionStringBuilder m_builder = new SqlConnectionStringBuilder();
    private BindingList<DatabaseEntry> m_bases= new BindingList<DatabaseEntry>();

    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      if (m_bases == null)
        m_bases = new BindingList<DatabaseEntry>();

      if (m_builder == null)
        m_builder = new SqlConnectionStringBuilder();
    }

    [DataMember]
    [DefaultValue("")]
    public string ConnectionString
    {
      get
      {
        return m_builder.ConnectionString;
      }
      set
      {
        m_builder.ConnectionString = value;
      }
    }

    [DataMember]
    [DefaultValue("")]
    public string Name
    {
      get
      {
        return m_builder.DataSource;
      }
      set
      {
        m_builder.DataSource = value;
      }
    }

    [DataMember]
    public BindingList<DatabaseEntry> Bases
    {
      get { return m_bases; }
    }

    public DatabaseEntry FindOrCreateDatabase(string DatabaseName)
    {

      DatabaseEntry existing_db_entry = (from p in this.Bases
                                         where p.Name == DatabaseName
                                         select p).FirstOrDefault();

      if (null == existing_db_entry)
      {
        existing_db_entry = new DatabaseEntry();
        existing_db_entry.Name = DatabaseName;
        existing_db_entry.Supported = (int)Support.Full;

        this.Bases.Add(existing_db_entry);
      }

      return existing_db_entry;
    }

    //public void AddDatabase(string databaseName)
    //{
    //  SmoUtil.ProcessServer(new SqlConnection(this.ConnectionString), new BackgroundWorker(),
    //    delegate(Server server)
    //    {
    //      Database newDb = new Database(server, databaseName);
    //      newDb.Create();

    //      this.FindOrCreateDatabase(databaseName);
    //    });
    //}

    public void AddDatabase(string databaseName) // Ограничение на имя таблицы соответствует ограничению на имя файла в файловой системе
    {
      using (SqlConnection connection =
                 new SqlConnection(this.ConnectionString))
      {
        connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = string.Format(CultureInfo.InvariantCulture,
					                                    "USE [master]\r\nCREATE DATABASE {0}\r\n",
					                                    DBObjectOneComponentName.EscapeBySquareBrackets(databaseName));
					command.ExecuteNonQuery();
				}
      }
      this.FindOrCreateDatabase(databaseName);
    }

    public void RemoveDatabase(DatabaseEntry database_entry)
    {
    	//SqlConnection connection = new SqlConnection(this.ConnectionString);
    	BackgroundWorker worker = new BackgroundWorker();

    	worker.WorkerReportsProgress = true;
    	worker.ReportProgress(0, Properties.Resources.DB_GRABBER_INIT_CONNECTION);

    	using (var connection = new SqlConnection(this.ConnectionString))
    	{
    		string command_text;
    		connection.Open();
    		//Подготовка к KillAllProcesses
    		int server_version_major = int.Parse(connection.ServerVersion.Split('.')[0]);
    		string database_name_EscApost = DBObjectOneComponentName.EscapeByApostrophe(database_entry.Name);
    		string database_name_EscSquar = DBObjectOneComponentName.EscapeBySquareBrackets(database_entry.Name);

    		command_text = string.Format(CultureInfo.InvariantCulture,
    		                             server_version_major == 8
    		                             	? "SELECT DISTINCT req_spid AS SPID FROM master.dbo.syslockinfo WHERE rsc_type = 2 AND rsc_dbid = DB_ID(N{0}) AND req_spid > 50 AND req_spid <> @@SPID"
    		                             	: "SELECT DISTINCT request_session_id AS SPID FROM master.sys.dm_tran_locks WHERE resource_type = 'DATABASE' AND resource_database_id = DB_ID(N{0}) AND request_session_id <> @@SPID",
    		                             database_name_EscApost);
    		bool to_kill = false;
    		string sql_commands_kill = null;
    		using (SqlCommand command = connection.CreateCommand())
    		{
    			command.CommandText = command_text;
    			sql_commands_kill = "USE [master]\r\n";
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
    					SqlConnection.ClearPool(connection);
    				}
    			}
    			catch (Exception ex)
    			{
    				_log.Error(string.Format("RemoveDatabase(\"{0}\"): exception", database_entry.Name), ex);
    			}
    		}
    		//Drop
    		using (SqlCommand command = connection.CreateCommand())
    		{
    			command.CommandText = string.Format(
    				CultureInfo.InvariantCulture,
    				"USE [master]\r\nIF EXISTS (SELECT name FROM sys.databases WHERE name = N{0})\r\nDROP DATABASE {1}\r\n",
    				database_name_EscApost, database_name_EscSquar);
    			command.ExecuteNonQuery();
    		}
    	}
    	this.Bases.Remove(database_entry);
    }
  }
}