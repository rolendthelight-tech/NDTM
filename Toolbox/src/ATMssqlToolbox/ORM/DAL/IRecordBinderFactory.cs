using System.Collections.Generic;
using System.Data;
using AT.Toolbox.MSSQL.ORM.DAL.Wcf;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public interface IRecordBinderFactory
  {
    IRecordBinder CreateRecordBinder(Dictionary<BindingAction, string> procedureActions);
    BindingResult GetResult(IServiceRecord record, BindingAction action);
    IQueryBuilder CreateQueryBuilder();
    IEnumerable<IBindable> GetObjectList(string tableName, QueryObject query);
    bool CheckActuality<TType>()
      where TType : class, IBindable, new();
    string GetCurrentUserName();
    string GetComputerName();
    string GetApplicationName();
    void FillDataTable(string tableName, DataTable table, QueryObject query);
    bool RequireInitialize { get; }
    string GetTransactionStackId(IRecordBinder binder);
  }
}