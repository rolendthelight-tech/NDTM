using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class Joiner : FilterBase
  {
    public Joiner(string parentTable, string childTable, string parentField, string childField)
    {
      this.TableName = parentTable;
      this.ChildTable = childTable;
      this.ParentField = parentField;
      this.ChildField = childField;
    }

    public Joiner(QueryObject query, string childTable, string parentField, string childField)
    {
      this.Query = query;
      if (query.RootFilter != null)
      {
        this.TableName = query.RootFilter.TableName;
      }
      this.ChildTable = childTable;
      this.ParentField = parentField;
      this.ChildField = childField;
    }

    public Joiner(Joiner prototype)
    {
      this.Query = prototype.Query;
      this.TableName = prototype.TableName;
      this.ChildTable = prototype.ChildTable;
      this.ParentField = prototype.ParentField;
      this.ChildField = prototype.ChildField;
    }
    
    public string ChildTable { get; protected set; }

    public string ParentField { get; protected set; }

    public string ChildField { get; protected set; }

    public override void DoBuildFilter(IQueryBuilder builder)
    {
      builder.BuildJoiner(this);
    }
  }
}
