using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace AT.Toolbox.MSSQL.Browsing
{
  using System.Collections;
  using Constants;
  using DB;
  //using Microsoft.SqlServer.Management.Smo;
  using Properties;
  using System.Data;
  using System.ComponentModel;

  // Этот класс устарел. Функциональность перенесена в Moulding.DbMouldRoutines
  /*
  public class ATDatabaseRoutines : ISpecificDBRoutines
  {
    protected static DatabaseStructureData m_data;
    protected static DatabaseDicData m_dictionary_data;

    public string MetaDataFile{get; set; }

    protected static void UpdateSupported( ref Support Value, Support Desired )
    {
      if (Desired < Value)
        Value = Desired;
    }

    public Support Supported(SqlConnection Conn, out string Summary, out string VersionInfoEx)
    {
      Summary = "";
      VersionInfoEx = "";
      Support ret_val = Support.Full;
      Server myServer = null;

      try
      {
        if (null == m_data)
        {
          m_data = DatabaseStructureData.Load(MetaDataFile);
          m_dictionary_data = new DatabaseDicData(m_data);
        }

        ServerConnection con = new ServerConnection(Conn);
        myServer = new Server(con);

        Database database = myServer.Databases[Conn.Database];

        UpdateSupported(ref ret_val, CheckTables(database, ref VersionInfoEx, ret_val));

        UpdateSupported(ref ret_val, CheckViews(database, ref VersionInfoEx, ret_val));

        UpdateSupported(ref ret_val, CheckProcedures(database, ref VersionInfoEx, ret_val));

        if (ret_val != Support.Full)
          Summary = Resources.DB_BROWSER_SUPPORT_PROBLEMS;
      }
      catch (Exception)
      {
        return Support.Unknown;
      }
      finally
      {
        try
        {
          myServer.ConnectionContext.Disconnect();
        }
        catch { }
      }

      return ret_val;
    }

    protected Support CheckTables(Database database, ref string VersionInfoEx, Support ret_val)
    {
      DataTable tables = database.EnumObjects(DatabaseObjectTypes.Table, SortOrder.Name);

      foreach (DataRow dr in tables.Rows)
      {
        Table tbl = database.Tables[dr["Name"].ToString()];
        if (tbl == null || tbl.IsSystemObject)
          continue;

        ExtendedProperty p = tbl.ExtendedProperties["Version"];

        string version = null == p ? "0.0.0.0" : p.Value.ToString();

        if (!m_dictionary_data.Tables.ContainsKey(tbl.Name))
        {
          VersionInfoEx += string.Format( "\r\n" + Resources.TABLE + Resources.NOT_FOUND_IN_MD, tbl.Name );
          UpdateSupported( ref ret_val, Support.Partial );
          continue;
        }

        if (m_dictionary_data.Tables[tbl.Name].Version != version)
        {
          VersionInfoEx += string.Format("\r\n" + Resources.TABLE + Resources.NOT_WRONG_VERSION, tbl.Name, version, m_dictionary_data.Tables[tbl.Name].Version);
          UpdateSupported(ref ret_val, Support.None);
        }
      }

      foreach (KeyValuePair<string, DatabaseStructureEntry> pair in m_dictionary_data.Tables)
      {
        if( !database.Tables.Contains( pair.Key ) )
        {
          VersionInfoEx += string.Format("\r\n" + Resources.TABLE + Resources.NOT_FOUND_IN_DATABASE, pair.Key);
          UpdateSupported(ref ret_val, Support.None);
        }
      }

      return ret_val;
    }

    protected Support CheckViews(Database database, ref string VersionInfoEx, Support ret_val)
    {
      DataTable views = database.EnumObjects(DatabaseObjectTypes.View, SortOrder.Name);

      foreach (DataRow dr in views.Rows)
      {
        View view = database.Views[dr["Name"].ToString()];
        if (view == null || view.IsSystemObject)
          continue;

        ExtendedProperty p = view.ExtendedProperties["Version"];

        string version = null == p ? "0.0.0.0" : p.Value.ToString();

        if (!m_dictionary_data.Views.ContainsKey(view.Name))
        {
          VersionInfoEx += string.Format("\r\n" + Resources.VIEW + Resources.NOT_FOUND_IN_MD, view.Name);
          UpdateSupported(ref ret_val, Support.Partial);
          continue;
        }

        if (m_dictionary_data.Views[view.Name].Version != version)
        {
          VersionInfoEx += string.Format("\r\n" + Resources.VIEW + Resources.NOT_WRONG_VERSION, view.Name, version, m_dictionary_data.Views[view.Name].Version);
          UpdateSupported(ref ret_val, Support.None);
        }
      }

      foreach (KeyValuePair<string, DatabaseStructureEntry> pair in m_dictionary_data.Views)
      {
        if (!database.Views.Contains(pair.Key))
        {
          VersionInfoEx += string.Format("\r\n" + Resources.VIEW + Resources.NOT_FOUND_IN_DATABASE, pair.Key);
          UpdateSupported(ref ret_val, Support.None);
        }
      }

      return ret_val;
    }

    protected Support CheckProcedures(Database database, ref string VersionInfoEx, Support ret_val)
    {
      int count = 0;

      DataTable procs = database.EnumObjects(DatabaseObjectTypes.StoredProcedure, SortOrder.Name);

      foreach (DataRow dr in procs.Rows)
      {
        StoredProcedure sp = database.StoredProcedures[dr["Name"].ToString()];
        if (sp == null || sp.IsSystemObject)
          continue;

        ExtendedProperty p = sp.ExtendedProperties["Version"];

        string version = null == p ? "0.0.0.0" : p.Value.ToString();

        if (!m_dictionary_data.Procedures.ContainsKey(sp.Name))
        {
          VersionInfoEx += string.Format("\r\n" + Resources.PROCEDURE + Resources.NOT_FOUND_IN_MD, sp.Name);
          UpdateSupported(ref ret_val, Support.Partial);
          continue;
        }

        if (m_dictionary_data.Procedures[sp.Name].Version != version)
        {
          VersionInfoEx += string.Format("\r\n" + Resources.PROCEDURE + Resources.NOT_WRONG_VERSION, sp.Name, version, m_dictionary_data.Procedures[sp.Name].Version);
          UpdateSupported(ref ret_val, Support.None);
        }

        count++;
      }

      foreach (KeyValuePair<string, DatabaseStructureEntry> pair in m_dictionary_data.Procedures)
      {
        if (!database.StoredProcedures.Contains(pair.Key))
        {
          VersionInfoEx += string.Format("\r\n" + Resources.PROCEDURE + Resources.NOT_FOUND_IN_DATABASE, pair.Key);
          UpdateSupported(ref ret_val, Support.None);
        }
      }

      return ret_val;
    }

    public virtual void ReloadMetaData()
    {
      m_data = DatabaseStructureData.Load(MetaDataFile);
      m_dictionary_data = new DatabaseDicData(m_data);
    }


    public bool InitStructure(SqlConnection conn, BackgroundWorker worker)
    {
      if (null == m_data)
      {
        m_data = DatabaseStructureData.Load(MetaDataFile);
        m_dictionary_data = new DatabaseDicData(m_data);
      }

      if( conn.State != System.Data.ConnectionState.Open )
        conn.Open();

      foreach (DatabaseStructureEntry table in m_data.Tables)
      {
        foreach (string s in table.Script)
        {
          try
          {
            SqlCommand cmd = new SqlCommand(s, conn);
            cmd.ExecuteNonQuery();
          }
          catch (Exception) { }
        }
        SqlExtendedPropertyHelper.SetTableExtendedProperties(conn, table.Name, "Version", table.Version);
      }

      foreach (DatabaseStructureEntry entry in m_data.TablesExt)
      {
        foreach (string s in entry.Script)
        {
          if (s.ToUpper().Contains("PRIMARY KEY")) // Primary key is always inserted into script -- fixing double creation
            continue;

          try
          {
            SqlCommand cmd = new SqlCommand(s, conn);
            cmd.ExecuteNonQuery();
          }
          catch (Exception) { }
        }
      }

      foreach (DatabaseStructureEntry view in m_data.Views)
      {
        foreach (string s in view.Script)
        {

          try
          {
            SqlCommand cmd = new SqlCommand(s, conn);
            cmd.ExecuteNonQuery();
          }
          catch (Exception) { }
        }

        SqlExtendedPropertyHelper.SetViewExtendedProperties(conn, view.Name, "Version", view.Version);
      }

      foreach (DatabaseStructureEntry entry in m_data.ViewsExt)
      {
        foreach (string s in entry.Script)
        {

          try
          {
            SqlCommand cmd = new SqlCommand(s, conn);
            cmd.ExecuteNonQuery();
          }
          catch (Exception) { }
        }
      }

      foreach (DatabaseStructureEntry entry in m_data.Procedures)
      {
        foreach (string s in entry.Script)
        {

          try
          {
            SqlCommand cmd = new SqlCommand(s, conn);
            cmd.ExecuteNonQuery();
          }
          catch (Exception) { }
        }

        SqlExtendedPropertyHelper.SetProcedureExtendedProperties(conn, entry.Name, "Version", entry.Version);
      }

      return true;
    }

    public bool UpdateStructure(SqlConnection conn, BackgroundWorker worker)
    {
      return false;
    }

    public virtual bool ImportData(SqlConnection conn)
    {
      return false;
    }
  }
   * */
}
