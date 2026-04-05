using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.MSSQL.Browsing;
using AT.Toolbox.Misc;
using AT.Toolbox.MSSQL.Properties;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Data;
using ERMS.Core.DAL;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Служебная задача для создания резервной копии базы данных
  /// </summary>
  internal class SqlBackupWork : IBackgroundWork
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlBackupWork).Name);

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
      get { return Resources.p_48_database_save; }
    }

    public string Name
    {
      get { return Resources.DB_BROWSER_BACKUP; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public string ConnectionString { get; set; }

    public string Path { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      worker.ReportProgress(0, Resources.SQL_CHECK_SERVER);

      try
      {
        DirectoryInfo fi = (new FileInfo(this.Path)).Directory;
        DirectorySecurity fSecurity = fi.GetAccessControl();
        string accountName = "NETWORK SERVICE";
        {
          bool b;
          fSecurity.ModifyAccessRule(AccessControlModification.Reset, new FileSystemAccessRule(accountName, FileSystemRights.FullControl,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None, AccessControlType.Allow), out b);
        }
        //fSecurity.SetAccessRule(new FileSystemAccessRule(accountName, FileSystemRights.FullControl,
        //  InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
        //  PropagationFlags.None, AccessControlType.Allow));

        //fSecurity.SetAccessRuleProtection(false, false);
        fi.SetAccessControl(fSecurity);
      }
      catch(UnauthorizedAccessException ex)
      {
        Log.Warn("Run(): Нет прав на изменение прав на целевую директорию (возможно, это не потребуется): ", ex);
      }

      using (SqlConnection connection = new SqlConnection(this.ConnectionString))
      {
        connection.Open();

        try
        {
          worker.ReportProgress(0, Resources.CLEAR_LOG);
          bool allow_truncate_only = false;
          int server_version_major = int.Parse(connection.ServerVersion.Split('.')[0]);
          allow_truncate_only = server_version_major == 9;
          byte db_recovery_model = 0;
          {
						using (SqlCommand get_recovery_model = new SqlCommand(
							@"select recovery_model
from sys.databases
where name = N" + DBObjectOneComponentName.EscapeByApostrophe(connection.Database), connection))
						{
							using (var dr = get_recovery_model.ExecuteReader(CommandBehavior.SingleRow))
							{
								if (dr.Read())
								{
									var model_str = dr["recovery_model"].ToString();
									byte model = 0;
									if (byte.TryParse(model_str, out model))
										db_recovery_model = model;
									else
										db_recovery_model = 0;
								}
							}
						}
          }
          if ((db_recovery_model == 1 || db_recovery_model == 2) && allow_truncate_only) // Модели восстановления FULL и BULK_LOGGED соответственно (SIMPLE = 3)
          {
            using(SqlCommand drop_log = new SqlCommand("BACKUP LOG " + DBObjectOneComponentName.EscapeBySquareBrackets(connection.Database) + " WITH TRUNCATE_ONLY", connection))
            {
            	drop_log.ExecuteNonQuery();
            }
          }
          else
          { // TODO: использовать переключение между моделью полного восстановления и моделью восстановления с неполным протоколированием
            //          * Переключение базы данных на модель полного восстановления:
            //USE master;
            //ALTER DATABASE имя_базы_данных SET RECOVERY FULL.
            //          * Переключение базы данных на модель восстановления с неполным протоколированием:
            //USE master;
            //ALTER DATABASE имя_базы_данных SET RECOVERY BULK_LOGGED.
          }
        }
        catch (Exception ex)
        {
          Log.Error("Run(): exception", ex);
        }

        worker.ReportProgress(0, this.Name);
        string cmd_text = "BACKUP DATABASE " + DBObjectOneComponentName.EscapeBySquareBrackets(connection.Database)
          + " TO DISK=" + DBObjectOneComponentName.EscapeByApostrophe(this.Path) + " WITH INIT, STATS = 1";

        connection.FireInfoMessageEventOnUserErrors = true;
        int progress = 0;
        connection.InfoMessage += delegate(object sender, SqlInfoMessageEventArgs e)
        {
          string message = e.Message.Replace("percent", "%");
          worker.ReportProgress(progress, message);

          foreach (SqlError err in e.Errors)
          {
            var sp = SqlErrorSimpleParser.Parse(err);

            if (sp.IsProgress)
              worker.ReportProgress(progress = sp.Progress);

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

				using (SqlCommand cmd = new SqlCommand(cmd_text, connection)
					{
						CommandTimeout = int.MaxValue
					})
				{
					cmd.ExecuteNonQuery();
				}
      }
    }

    #endregion
  }
}