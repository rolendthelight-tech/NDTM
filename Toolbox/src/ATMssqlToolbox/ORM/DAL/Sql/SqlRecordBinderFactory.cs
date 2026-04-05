using System;
using System.Collections.Generic;
using System.Text;
using AT.Toolbox.MSSQL.ORM.DAL.Wcf;
using AT.Toolbox.MSSQL.UI;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Sql
{
  public class SqlRecordBinderFactory : IRecordBinderFactory
  {
    readonly RecordServiceClientStub m_record_service = new RecordServiceClientStub();

    #region IRecordBinderFactory Members

    public string GetTransactionStackId(IRecordBinder binder)
    {
      return ((SqlRecordBinder)binder).Identifier;
    }

    public BindingResult GetResult(IServiceRecord record, BindingAction action)
    {
      return m_record_service.GetResult(record, action);
    }

    public IEnumerable<IBindable> GetObjectList(string tableName, QueryObject query)
    {
      List<IBindable> ret = new List<IBindable>();

      InFilter flt = query.RootFilter as InFilter;

      if (flt != null 
        && RecordManager.GetItem(tableName, null)._KeyFieldName == flt.FieldName)
      {
        foreach (var val in flt.Values)
        {
          ret.Add(RecordManager.GetItem(tableName, val));
        }
      }
      else
      {
        using (IBindable item = RecordManager.GetItem(tableName, null))
        {
          item.InitReader(query);

          while (item.Next())
          {
            ret.Add((IBindable)item.Clone());
          }
        }
      }

      return ret;
    }

    public IRecordBinder CreateRecordBinder(Dictionary<BindingAction, string> procedureActions)
    {
      SqlRecordBinder bdr = new SqlRecordBinder();
      bdr.ApplyProcedureActions(procedureActions);
      return bdr;
    }

    public IQueryBuilder CreateQueryBuilder()
    {
      return new SqlQueryBuilder();
    }

    public Form CreateConfiguratorForm()
    {
      return new Form();
    }

    public bool CheckActuality<TType>()
      where TType : class, IBindable, new()
    {
      return RecordCache.ValidateType<TType>(null);
    }

    public bool RequireInitialize
    {
      get { return string.IsNullOrEmpty(SqlRecordBinder.ConnectionString); }
    }

    public string GetCurrentUserName()
    {
      return string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName).ToLower();
    }

    public string GetComputerName()
    {
      return Environment.MachineName;
    }

    public string GetApplicationName()
    {
      return ApplicationInfo.MainAttributes.ProductName;
    }

    public void FillDataTable(string tableName, DataTable table, QueryObject qo)
    {
      SqlQueryBuilder qb = new SqlQueryBuilder();
      qb.MainTable = tableName;
      qo.BuildQuery(qb);
      table.TableName = tableName;
      using (SqlConnection conn = new SqlConnection(SqlRecordBinder.ConnectionString))
      {
        conn.Open();
        table.Clear();

        SqlDataAdapter adapter = new SqlDataAdapter();

        adapter.SelectCommand = new SqlCommand(qb.GetQueryString(), conn);
        foreach (KeyValuePair<int, object> kv in qb.GetParameters())
        {
          adapter.SelectCommand.Parameters.Add(new SqlParameter("_" + kv.Key, kv.Value));
        }

        adapter.FillSchema(table, SchemaType.Source);
        adapter.Fill(table);
      }
    }

    #endregion
  }
}
