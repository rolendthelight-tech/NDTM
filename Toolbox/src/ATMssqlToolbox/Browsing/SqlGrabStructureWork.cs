using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.Misc;
using AT.Toolbox.MSSQL.Properties;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace AT.Toolbox.MSSQL
{
  // Этот класс устарел и больше не используется. Вместо него используется SqlMouldWork
  /*class SqlGrabStructureWork : IBackgroundWork
  {
    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return true; }
    }

    public bool CanCancel
    {
      get { return false; }
    }

    public System.Drawing.Bitmap Icon
    {
      get { return Resources.p_48_database_grabber; }
    }

    public string Name
    {
      get { return Resources.DB_BROWSER_TEST; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public SqlConnection Connection { get; set; }

    public event EventHandler<ParamEventArgs<bool>> CanCancelChanged;

    public void Run(BackgroundWorker worker)
    {
      CanCancelChanged += delegate { };
      DbMould mould = DbMouldBuilder.BuildWithoutUpdate(this.Connection, worker);
      using (StreamWriter sw = new StreamWriter(DatabaseBrowserControl.Preferences.ConfigFilePath))
      {
        new XmlSerializer(typeof(DbMould)).Serialize(sw, mould);
      }
    }

    #endregion
  }*/
}
