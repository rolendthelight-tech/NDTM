using System.Data.SqlClient;
using System.Windows.Forms;


namespace AT.Toolbox.MSSQL.Authentication
{
  public class SqlConnectionFactory
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlConnectionFactory).Name);

    static SqlConnection m_conn = null ;

    public static string UserName { get; set; }
   
    public static SqlConnection Current
    {
      get
      {
        if (null != m_conn)
          return m_conn;

        if (string.IsNullOrEmpty(DatabaseBrowserControl.Preferences.CurrentConnection))
          return null;

        SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(DatabaseBrowserControl.Preferences.CurrentConnection);

        if( !b.IntegratedSecurity )
        {
          if( string.IsNullOrEmpty( b.UserID ) )
          {
            SqlAuthenticationForm frm = new SqlAuthenticationForm(  );

            if (DialogResult.OK != frm.ShowDialog())
              return null;

            b.UserID = frm.UserName;
            b.Password = frm.Password;
          }
        }

        UserName = b.UserID;
        b.MultipleActiveResultSets = true;
        m_conn = new SqlConnection( b.ConnectionString );

        Log.Debug("Current.get(): " + b.ConnectionString );

        return m_conn;
      }
    }
  }
}
