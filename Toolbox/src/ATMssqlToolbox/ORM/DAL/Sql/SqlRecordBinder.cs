using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using AT.Toolbox.MSSQL.Properties;
using System.ComponentModel;
using System.Collections;
using AT.Toolbox.Log;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Sql
{
  public sealed class SqlRecordBinder : IRecordBinder
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlRecordBinder).Name);

    #region Fields

    private IDataReader innerReader;
    private bool disposed;
    private readonly string connectionString;
    private readonly Dictionary<BindingAction, string> procedureActions = new Dictionary<BindingAction, string>();

    private static readonly object locker = new object();
    private static readonly Dictionary<string, SqlTransactionStack> transactions = new Dictionary<string, SqlTransactionStack>();

    #endregion

    #region Constructors

    public SqlRecordBinder(string connectionString)
    {
      this.connectionString = connectionString;
    }

    public SqlRecordBinder()
    {
      this.connectionString = SqlRecordBinder.ConnectionString;
    }

    #endregion

    #region Properties

    private bool Disposed
    {
      get { return disposed; }
    }

    private bool IsInTransaction
    {
      get
      {
        lock (locker)
        {
          return transactions.ContainsKey(RecordManager.GetTransactionStackId(this));
        }
      }
    }

    public string Identifier
    {
      get { return connectionString; }
    }

    #endregion

    #region IRecordBinder Members

    public void ApplyProcedureActions(Dictionary<BindingAction, string> actions)
    {
      this.procedureActions.Clear();

      foreach (KeyValuePair<BindingAction, string> kv in actions)
      {
        if (!string.IsNullOrEmpty(kv.Value))
        {
          if (this.procedureActions.ContainsValue(kv.Value))
          {
            RecordManager.WriteLog(new Info(string.Format("Procedure {0} is already used.", kv.Value), InfoLevel.Warning));
          }
          this.procedureActions.Add(kv.Key, kv.Value);
        }
      }
    }

    public object GetFieldValue(int fieldNumber)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      if (innerReader == null)
      {
        throw new InvalidOperationException(Resources.Sql_IndexerUninitialized);
      }
      object ret = innerReader[fieldNumber];
      return ret != Convert.DBNull ? ret : null;
    }

    public string[] OpenReader(string tableName, Dictionary<string, object> keyFieldValues)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      string procedureName;

      if (procedureActions.TryGetValue(BindingAction.Get, out procedureName))
      {
        this.CloseReader();
        this.innerReader = this.CreateDataReaderByProc(procedureName, keyFieldValues);

        this.AddReaderToStack();

        string[] ret = new string[this.innerReader.FieldCount];
        for (int i = 0; i < ret.Length; i++)
        {
          ret[i] = this.innerReader.GetName(i);
        }
        return ret;
      }
      else
      {
        return this.OpenReader(tableName, QueryObject.CreateFromFieldValues(keyFieldValues));
      }
    }

    private void AddReaderToStack()
    {
      string id = RecordManager.GetTransactionStackId(this);
      if (!string.IsNullOrEmpty(id)
       && transactions.ContainsKey(id))
      {
        var tstack = transactions[id];
        tstack.OpenedReaders.Add(innerReader);
      }
    }

    public string[] OpenReader(string tableName, QueryObject query)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      Dictionary<int, object> parameters = new Dictionary<int, object>();

      string queryString = this.GetSelectQuery(tableName, query, parameters);

      SqlTransaction tts;
      SqlConnection connection = this.GetCurrentConnection(out tts);

      try
      {
        SqlCommand command = new SqlCommand(queryString, connection, tts);

        foreach (KeyValuePair<int, object> kv in parameters)
        {
          command.Parameters.Add(new SqlParameter("_" + kv.Key.ToString(), kv.Value));
        }

        command.CommandType = CommandType.Text;

        this.CloseReader();
        innerReader = command.ExecuteReader(tts != null ? CommandBehavior.Default : CommandBehavior.CloseConnection);

        this.AddReaderToStack();

        if (query.Aggregation.Count != 1)
        {
          string[] ret = new string[this.innerReader.FieldCount];
          for (int i = 0; i < ret.Length; i++)
          {
            ret[i] = this.innerReader.GetName(i);
          }
          return ret;
        }
        else
        {
          IEnumerator<KeyValuePair<string, AggregateOperation>> en = query.Aggregation.GetEnumerator();
          en.MoveNext();
          return new string[] { en.Current.Key };
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("OpenReader({0}): exception", tableName), ex);
        RecordManager.WriteLog(Resources.Sql_InvalidQuery + queryString + Environment.NewLine + ex.Message, InfoLevel.Error);
        if (tts == null)
        {
          connection.Close();
        }
        throw;
      }
    }

    public bool Next()
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      if (innerReader == null)
      {
        throw new InvalidOperationException(Resources.Sql_NextWithoutInitList);
      }
      bool canReadMore = innerReader.Read();
      if (!canReadMore)
      {
        innerReader.Close();
        innerReader = null;
      }
      return canReadMore;
    }

    public void CloseReader()
    {
      if (this.innerReader != null && !this.innerReader.IsClosed)
      {
        lock (transactions)
        {
          var id = RecordManager.GetTransactionStackId(this);
          if (!string.IsNullOrEmpty(id)
            && transactions.ContainsKey(id))
          {
            var tstack = transactions[id];

            int idx_2_close = -1;
            for (int i = 0; i < tstack.OpenedReaders.Count; i++)
            {
              if (ReferenceEquals(this.innerReader, tstack.OpenedReaders[i]))
              {
                idx_2_close = i;
                break;
              }
            }

            if (idx_2_close >= 0)
            {
              if (!tstack.OpenedReaders[idx_2_close].IsClosed)
              {
                tstack.OpenedReaders[idx_2_close].Close();
              }
              if (idx_2_close < tstack.OpenedReaders.Count)
                tstack.OpenedReaders.RemoveAt(idx_2_close);
            }
          }
        }

        innerReader.Close();
        innerReader = null;
      }
    }

    public bool Insert(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      string procedureName;

      if (procedureActions.TryGetValue(BindingAction.Insert, out procedureName))
      {
        return this.InsertByProc(procedureName, fieldValues, outputFieldNames);
      }
      else
      {
        return this.InsertByQuery(tableName, fieldValues, outputFieldNames);
      }
    }

    public bool Update(string tableName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      string procedureName;

      if (procedureActions.TryGetValue(BindingAction.Update, out procedureName))
      {
        return this.UpdateByProc(procedureName, fieldValues, keyFieldValues);
      }
      else
      {
        return this.UpdateRecordset(tableName, QueryObject.CreateFromFieldValues(keyFieldValues), fieldValues);
      }
    }

    public bool Delete(string tableName, Dictionary<string, object> keyFieldValues)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      string procedureName;

      if (procedureActions.TryGetValue(BindingAction.Delete, out procedureName))
      {
        return this.DeleteByProc(procedureName, keyFieldValues);
      }
      else
      {
        return this.DeleteRecordSet(tableName, QueryObject.CreateFromFieldValues(keyFieldValues));
      }
    }

    public bool UpdateRecordset(string tableName, QueryObject query, Dictionary<string, object> values)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      Dictionary<int, object> parameters = new Dictionary<int, object>();

      string querystr = this.GetUpdateQuery(tableName, query, values, parameters);

      SqlTransaction tts;
      SqlConnection connection = this.GetCurrentConnection(out tts);
      try
      {
        SqlCommand command = new SqlCommand(querystr, connection, tts);

        foreach (KeyValuePair<int, object> kv in parameters)
        {
          command.Parameters.Add(new SqlParameter("_" + kv.Key.ToString(), kv.Value ?? Convert.DBNull));
        }
        foreach (KeyValuePair<string, object> kv in values)
        {
          command.Parameters.Add(new SqlParameter(kv.Key, kv.Value ?? Convert.DBNull));
        }

        command.CommandType = CommandType.Text;

        return command.ExecuteNonQuery() > 0;
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("UpdateRecordset({0}): exception, query = '{1}'", tableName, querystr), ex);
        RecordManager.WriteLog(Resources.Sql_InvalidQuery + querystr, InfoLevel.Error);
        throw;
      }
      finally
      {
        if (tts == null)
          connection.Close();
      }
    }

    public bool DeleteRecordSet(string tableName, QueryObject query)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      Dictionary<int, object> parameters = new Dictionary<int, object>();

      string queryString = this.GetDeleteQuery(tableName, query, parameters);

      SqlTransaction tts;
      SqlConnection connection = this.GetCurrentConnection(out tts);
      try
      {
        SqlCommand command = new SqlCommand(queryString, connection, tts);

        foreach (KeyValuePair<int, object> kv in parameters)
        {
          command.Parameters.Add(new SqlParameter("_" + kv.Key.ToString(), kv.Value ?? Convert.DBNull));
        }

        command.CommandType = CommandType.Text;

        return command.ExecuteNonQuery() > 0;
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("DeleteRecordSet({0}): exception, query = '{1}'", tableName, queryString), ex);
        RecordManager.WriteLog(Resources.Sql_InvalidQuery + queryString, InfoLevel.Error);
        throw;
      }
      finally
      {
        if (tts == null)
          connection.Close();
      }
    }

    #endregion

    #region Special methods

    public static void FillDatabaseList(string server, IList destination)
    {
      SqlConnection connection = new SqlConnection("packet size=4096;data source=" + server
          + ";integrated security=SSPI;persist security info=False;initial catalog=master");
      connection.Open();

      SqlCommand command = new SqlCommand("sp_databases", connection);
      command.CommandType = CommandType.StoredProcedure;

      IDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
      while (dr.Read())
      {
        destination.Add(dr[0]);
      }
      dr.Close();
    }

    public static int ExecuteSqlUpdate(string commands)
    {
      int rowCount = 0;
      using (SqlConnection connection = new SqlConnection(ConnectionString))
      {
        connection.Open();
        SqlTransaction tts = connection.BeginTransaction();
        foreach (string commandText in commands.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
        {
          SqlCommand command = new SqlCommand(commandText, connection, tts);
          command.CommandType = CommandType.Text;
          rowCount += command.ExecuteNonQuery();
        }
        tts.Commit();
      }
      return rowCount;
    }

    #endregion

    #region Query builder

    public string GetSelectQuery(string tableName, QueryObject query, Dictionary<int, object> parameters)
    {
      SqlQueryBuilder builder = new SqlQueryBuilder();
      builder.MainTable = tableName;

      query.BuildQuery(builder);

      foreach (KeyValuePair<int, object> kv in builder.GetParameters())
      {
        parameters.Add(kv.Key, kv.Value ?? Convert.DBNull);
      }

      return builder.GetQueryString();
    }

    public string GetInsertQuery(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("INSERT INTO [dbo].[" + tableName + "] (");

      bool first = true;

      foreach (KeyValuePair<string, object> kv in fieldValues)
      {
        if (kv.Value == null)
          continue;

        if (Array.Exists<string>(outputFieldNames, (p) => p == kv.Key))
          continue;

        if (first)
        {
          first = false;
        }
        else
        {
          sb.Append(", ");
        }
        sb.Append("[" + kv.Key + "]");
      }
      first = true;

      sb.Append(")");
      sb.AppendLine("VALUES (");
      foreach (KeyValuePair<string, object> kv in fieldValues)
      {
        if (kv.Value == null)
          continue;

        if (Array.Exists<string>(outputFieldNames, (p) => p == kv.Key))
          continue;

        if (first)
        {
          first = false;
        }
        else
        {
          sb.Append(", ");
        }
        sb.Append("@" + kv.Key);
      }
      sb.Append(")");
      foreach (string identityField in outputFieldNames)
      {
        sb.AppendLine("SELECT @" + identityField + "=SCOPE_IDENTITY()");
        break;
      }
      return sb.ToString();
    }

    public string GetUpdateQuery(string tableName, QueryObject query, Dictionary<string, object> values, Dictionary<int, object> parameters)
    {
      string querystr = "UPDATE [dbo].[" + tableName + "] SET ";

      bool first = true;
      foreach (string parameter in values.Keys)
      {
        if (first)
        {
          first = false;
        }
        else
        {
          querystr += ", ";
        }
        querystr += "[" + parameter + "] = @" + parameter;
      }

      querystr += this.GenerateCommandText(tableName, query, parameters);

      return querystr;
    }

    public string GetDeleteQuery(string tableName, QueryObject query, Dictionary<int, object> parameters)
    {
      return "DELETE FROM [dbo].[" + tableName + "]"
           + this.GenerateCommandText(tableName, query, parameters);
    }

    public string GetCountQuery(string tableName, QueryObject query, Dictionary<int, object> parameters)
    {
      return "SELECT COUNT(*) FROM [dbo].[" + tableName + "]"
           + this.GenerateCommandText(tableName, query, parameters);
    }

    private string GenerateCommandText(string tableName, QueryObject query, Dictionary<int, object> parameters)
    {
      SqlQueryBuilder builder = new SqlQueryBuilder();
      query.BuildPredicates(builder);

      foreach (KeyValuePair<int, object> kv in builder.GetParameters())
      {
        parameters.Add(kv.Key, kv.Value);
      }

      return builder.GetQueryString();
    }

    #endregion

    #region Transactions

    public static string ConnectionString { get; set; }

    void IRecordBinder.BeginTransaction(IsolationLevel iso)
    {
      lock (locker)
      {
        SqlTransactionStack tstack;

        var id = RecordManager.GetTransactionStackId(this);
        if (!transactions.TryGetValue(id, out tstack))
        {
          SqlConnection globalConnection = new SqlConnection(this.connectionString);
          globalConnection.Open();
          tstack = new SqlTransactionStack(globalConnection.BeginTransaction(iso));
          transactions.Add(id, tstack);
        }
        else
        {
          tstack.IncreaseLevel();
        }
      }
    }

    void IRecordBinder.CommitTransaction()
    {
      lock (locker)
      {
        SqlTransactionStack tstack;
        var id = RecordManager.GetTransactionStackId(this);
        if (transactions.TryGetValue(id, out tstack))
        {
          if (!tstack.DecreaseLevel())
          {
            foreach (var reader in tstack.OpenedReaders)
            {
              if (!reader.IsClosed)
                reader.Close();
            }

            tstack.OpenedReaders.Clear();
            
            tstack.Transaction.Commit();
            tstack.Connection.Close();
            transactions.Remove(id);
          }
        }
        else
        {
          throw new InvalidOperationException();
        }
      }
    }

    void IRecordBinder.RollbackTransaction()
    {
      lock (locker)
      {
        SqlTransactionStack tstack;
        var id = RecordManager.GetTransactionStackId(this);
        if (transactions.TryGetValue(id, out tstack))
        {
          if (!tstack.DecreaseLevel())
          {
            foreach (var reader in tstack.OpenedReaders)
            {
              if (!reader.IsClosed)
                reader.Close();
            }

            tstack.OpenedReaders.Clear();

            tstack.Transaction.Rollback();
            tstack.Connection.Close();
            transactions.Remove(id);
          }
        }
        else
        {
          throw new InvalidOperationException();
        }
      }
    }

    private SqlConnection GetCurrentConnection(out SqlTransaction transaction)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.GetType().ToString());
      }
      lock (locker)
      {
        SqlTransactionStack tstack;
        if (transactions.TryGetValue(this.connectionString, out tstack))
        {
          transaction = tstack.Transaction;
          return tstack.Connection;
        }
        else
        {
          transaction = null;
          SqlConnection connection = new SqlConnection(this.connectionString);
          connection.Open();
          return connection;
        }
      }
    }

    #region Transaction stack

    private class SqlTransactionStack
    {
      private volatile int m_depth;
      private SqlTransaction m_tts;
      private SqlConnection m_conn;
      private readonly List<IDataReader> m_readers = new List<IDataReader>();

      public SqlTransactionStack(SqlTransaction transaction)
      {
        this.m_tts = transaction;
        this.m_conn = transaction.Connection;
        this.m_depth = 1;
      }

      public List<IDataReader> OpenedReaders
      {
        get { return m_readers; }
      }

      public SqlTransaction Transaction
      {
        get { return m_tts; }
      }

      public SqlConnection Connection
      {
        get { return m_conn; }
      }

      public void IncreaseLevel()
      {
        m_depth++;
      }

      public bool DecreaseLevel()
      {
        m_depth--;
        return m_depth > 0;
      }
    }

    #endregion

    #endregion

    #region Procedure binding

    private SqlCommand PrepareCommand(string procedureName, Dictionary<string, object> parameters, string[] outputFieldNames)
    {
      outputFieldNames = outputFieldNames ?? new string[0];

      SqlTransaction tts = null;
      SqlConnection connection = this.GetCurrentConnection(out tts);

      SqlCommand command = new SqlCommand(procedureName, connection, tts);
      command.CommandType = CommandType.StoredProcedure;
      if (parameters != null)
      {
        foreach (KeyValuePair<string, object> currentEntity in parameters)
        {
          SqlParameter currentParameter = new SqlParameter(currentEntity.Key, currentEntity.Value);
          if (Array.Exists<string>(outputFieldNames, (p) => p == currentEntity.Key))
          {
            currentParameter.Direction = ParameterDirection.Output;
          }
          command.Parameters.Add(currentParameter);
        }
      }
      return command;
    }

    private SqlDataReader CreateDataReaderByProc(string procedureName, Dictionary<string, object> parameters)
    {
      SqlCommand command = PrepareCommand(procedureName, parameters, null);
      try
      {
        return command.ExecuteReader(this.IsInTransaction ? CommandBehavior.Default : CommandBehavior.CloseConnection);
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("CreateDataReaderByProc({0}): exception", procedureName), ex);

        if (!this.IsInTransaction)
        {
          command.Connection.Close();
        }
        throw;
      }
    }

    private bool InsertByProc(string procedureName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      SqlCommand command = PrepareCommand(procedureName, fieldValues, outputFieldNames);
      try
      {
        int ret = command.ExecuteNonQuery();

        if (ret != 0)
        {
          foreach (string identityField in outputFieldNames)
          {
            fieldValues[identityField] = command.Parameters[identityField].Value;
          }
        }
        return ret != 0;
      }
      finally
      {
        if (!this.IsInTransaction)
        {
          command.Connection.Close();
        }
      }
    }

    private bool UpdateByProc(string procedureName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues)
    {
      SqlCommand cmd = this.PrepareCommand(procedureName, fieldValues, null);

      foreach (KeyValuePair<string, object> kv in keyFieldValues)
      {
        cmd.Parameters.Add(new SqlParameter("_orig_" + kv.Key, kv.Value));
      }

      try
      {
        return cmd.ExecuteNonQuery() != 0;
      }
      finally
      {
        if (!this.IsInTransaction)
        {
          cmd.Connection.Close();
        }
      }
    }

    private bool DeleteByProc(string procedureName, Dictionary<string, object> keyFieldValues)
    {
      SqlCommand command = PrepareCommand(procedureName, keyFieldValues, null);
      try
      {
        return command.ExecuteNonQuery() != 0;
      }
      finally
      {
        if (!this.IsInTransaction)
        {
          command.Connection.Close();
        }
      }
    }

    #endregion

    #region Query builder binding

    private bool InsertByQuery(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      string querystr = this.GetInsertQuery(tableName, fieldValues, outputFieldNames);
      SqlTransaction tts;
      SqlConnection connection = this.GetCurrentConnection(out tts);
      try
      {
        SqlCommand command = new SqlCommand(querystr, connection, tts);

        foreach (KeyValuePair<string, object> currentEntity in fieldValues)
        {
          SqlParameter currentParameter = new SqlParameter(currentEntity.Key, currentEntity.Value ?? Convert.DBNull);
          if (Array.Exists<string>(outputFieldNames, (p) => p == currentEntity.Key))
          {
            currentParameter.Direction = ParameterDirection.Output;
          }
          if (currentEntity.Value == null)
          {
            currentParameter.SqlDbType = SqlDbType.Variant;
          }
          command.Parameters.Add(currentParameter);
        }

        int ret = command.ExecuteNonQuery();
        if (ret != 0)
        {
          foreach (string identityField in outputFieldNames)
          {
            fieldValues[identityField] = command.Parameters[identityField].Value;
          }
        }
        return ret != 0;
      }
      finally
      {
        if (tts == null)
        {
          connection.Close();
        }
      }
    }

    #endregion

    #region IDisposable Members

    ~SqlRecordBinder()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.CloseReader();
      }
      else
      {
        try
        {
          this.CloseReader();
        }
        catch (Exception ex)
        {
          Log.Error(string.Format("Dispose({0}): exception", disposing), ex);
        }
      }
      disposed = true;
    }

    #endregion
  }
}
