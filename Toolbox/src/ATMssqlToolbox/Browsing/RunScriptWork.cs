using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AT.Toolbox.Misc;
using AT.Toolbox.MSSQL.Properties;
using System.IO;
using System.Data.SqlClient;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Фоновая задача для выполнения SQL-скрипта
  /// </summary>
  class RunScriptWork : IBackgroundWork
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

    public bool CanCancel
    {
      get { return false; }
    }

    public System.Drawing.Bitmap Icon
    {
      get { return Resources.p_48_database_script; }
    }

    public string Name
    {
      get { return Resources.DB_BROWSER_RUN_SCRIPT; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public object Result
    {
      get { return null; }
    }

    public string FileName { get; set; }

    public string ConnectionString { get; set; }

    public void Run(BackgroundWorker worker)
    {
      this.PropertyChanged += delegate { };

      var lines = File.ReadAllLines(this.FileName);

      List<string> commands = new List<string>();
      var builder = new StringBuilder();

      for (int i = 0; i < lines.Length; i++)
      {
        if ((lines[i] ?? "").Trim().ToUpper() == "GO")
        {
          commands.Add(builder.ToString().Trim());
          builder = new StringBuilder();
          continue;
        }

        builder.AppendLine(lines[i]);
      }

      if (builder.Length > 0)
        commands.Add(builder.ToString().Trim());

      using (var connection = new SqlConnection(this.ConnectionString))
      {
        connection.Open();

        //var tts = connection.BeginTransaction();

        for (int i = 0; i < commands.Count; i++)
        {
					using (var cmd = connection.CreateCommand())
					{
						cmd.CommandText = commands[i];
						//cmd.Transaction = tts;
						cmd.CommandTimeout = int.MaxValue;
						cmd.ExecuteNonQuery();
					}
        	worker.ReportProgress(i * 100 / commands.Count);
        }

        //tts.Commit();
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}
