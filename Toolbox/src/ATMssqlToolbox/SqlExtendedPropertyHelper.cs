using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AT.Toolbox.DB
{
  public static class SqlExtendedPropertyHelper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlExtendedPropertyHelper).Name);

    private const string scheme_tag = "user"; // For the times it'll be replaced with "SCHEME"

    /// <summary>
    /// Выполняет команду и разбирает значения
    /// </summary>
    /// <param name="Connection">Соединение с сервером</param>
    /// <param name="Command">Команда</param>
    /// <returns></returns>
    private static Dictionary<string, string> GetExtendedProperties(SqlCommand Command)
    {
      Dictionary<string, string> ret_val = new Dictionary<string, string>();

      if (Command.Connection.State != ConnectionState.Open)
        Command.Connection.Open();

      using (SqlDataReader reader = Command.ExecuteReader())
      {
        if (null == reader)
          return ret_val;

        while (reader.Read())
        {
          string property_name = reader["name"].ToString();
          string property_value = reader["value"].ToString();

          if (!ret_val.ContainsKey(property_name))
            ret_val.Add(property_name, property_value);
        }
      }

      return ret_val;
    }

    /// <summary>
    ///  Возвращает набор раширеных свойств столбца
    /// </summary>
    /// <param name="Connection">Соединение с сервером</param>
    /// <param name="Table">Название таблицы</param>
    /// <param name="Column">Название столбца</param>
    /// <returns></returns>
    public static Dictionary<string, string> GetExtendedColProperties(SqlConnection Connection, string Table,
                                                                      string Column)
    {
      try
      {
        const string cmd_text =
          @"SELECT name,value FROM ::fn_listextendedproperty(NULL,'{0}','dbo','TABLE','{1}','column','{2}')";

        SqlCommand cmd = new SqlCommand(string.Format(cmd_text, scheme_tag, Table, Column), Connection);

        return GetExtendedProperties(cmd);
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("GetExtendedColProperties({0}, {1}): exception", Table, Column), ex);
      }
      return new Dictionary<string, string>();
    }

    /// <summary>
    /// Возвращает набор расширеных свойств таблицы
    /// </summary>
    /// <param name="Connection">Соединение с сервером</param>
    /// <param name="Table">Название таблицы</param>
    /// <returns></returns>
    public static Dictionary<string, string> GetExtendedTableProperties(SqlConnection Connection, string Table)
    {
      try
      {
        const string cmd_text =
          @"SELECT name,value FROM ::fn_listextendedproperty(NULL,'{0}','dbo','TABLE','{1}', NULL, NULL')";

        SqlCommand cmd = new SqlCommand(string.Format(cmd_text, scheme_tag, Table), Connection);

        return GetExtendedProperties(cmd);
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("GetExtendedTableProperties({0}): exception", Table), ex);
      }
      return new Dictionary<string, string>();
    }

    /// <summary>
    /// Возвращает набор расширеных свойств хранимой процедуры
    /// </summary>
    /// <param name="Connection">Соединение с сервером</param>
    /// <param name="Procedure">Название процедуры</param>
    /// <returns></returns>
    public static Dictionary<string, string> GetExtendedProcedureProperties(SqlConnection Connection, string Procedure)
    {
      try
      {
        const string cmd_text =
          @"SELECT name,value FROM ::fn_listextendedproperty(NULL,'{0}','dbo','PROCEDURE','{1}', NULL, NULL')";

        SqlCommand cmd = new SqlCommand(string.Format(cmd_text, scheme_tag, Procedure), Connection);

        return GetExtendedProperties(cmd);
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("GetExtendedProcedureProperties({0}): exception", Procedure), ex);
      }
      return new Dictionary<string, string>();
    }


    public static void SetTableExtendedProperties(SqlConnection Connection, string Table, string Name, string Value)
    {
      try
      {
        SqlCommand cmd = new SqlCommand("sp_addextendedproperty", Connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
        cmd.Parameters.Add("@value", SqlDbType.Variant).Value = Value;
        cmd.Parameters.Add("@level0type", SqlDbType.NVarChar).Value = scheme_tag;
        cmd.Parameters.Add("@level0name", SqlDbType.NVarChar).Value = "dbo";
        cmd.Parameters.Add("@level1type", SqlDbType.NVarChar).Value = "TABLE";
        cmd.Parameters.Add("@level1name", SqlDbType.NVarChar).Value = Table;

        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("SetTableExtendedProperties({0}, {1}, {2}): exception", Table, Name, Value), ex);
      }
    }

    public static void SetViewExtendedProperties(SqlConnection Connection, string Table, string Name, string Value)
    {
      try
      {
        SqlCommand cmd = new SqlCommand("sp_addextendedproperty", Connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
        cmd.Parameters.Add("@value", SqlDbType.Variant).Value = Value;
        cmd.Parameters.Add("@level0type", SqlDbType.NVarChar).Value = scheme_tag;
        cmd.Parameters.Add("@level0name", SqlDbType.NVarChar).Value = "dbo";
        cmd.Parameters.Add("@level1type", SqlDbType.NVarChar).Value = "VIEW";
        cmd.Parameters.Add("@level1name", SqlDbType.NVarChar).Value = Table;

        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("SetViewExtendedProperties({0}, {1}, {2}): exception", Table, Name, Value), ex);
      }
    }

    public static void SetProcedureExtendedProperties(SqlConnection Connection, string Table, string Name, string Value)
    {
      try
      {
        SqlCommand cmd = new SqlCommand("sp_addextendedproperty", Connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = Name;
        cmd.Parameters.Add("@value", SqlDbType.Variant).Value = Value;
        cmd.Parameters.Add("@level0type", SqlDbType.NVarChar).Value = scheme_tag;
        cmd.Parameters.Add("@level0name", SqlDbType.NVarChar).Value = "dbo";
        cmd.Parameters.Add("@level1type", SqlDbType.NVarChar).Value = "PROCEDURE";
        cmd.Parameters.Add("@level1name", SqlDbType.NVarChar).Value = Table;

        cmd.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("SetProcedureExtendedProperties({0}, {1}, {2}): exception", Table, Name, Value), ex);
      }
    }
  }
}