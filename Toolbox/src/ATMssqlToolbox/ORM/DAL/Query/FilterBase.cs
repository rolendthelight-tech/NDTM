using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public abstract class FilterBase
  {
    public string TableName { get; set; }

    public virtual QueryObject Query { get; set; }

    public virtual bool Enabled
    {
      get
      {
        return true;
      }
    }

    public void BuildFilter(IQueryBuilder builder)
    {
      if (string.IsNullOrEmpty(this.TableName))
      {
        this.TableName = builder.MainTable;
      }

      this.DoBuildFilter(builder);
    }

    public abstract void DoBuildFilter(IQueryBuilder builder);

    public virtual FilterBase Clone(QueryObject newQuery)
    {
      FilterBase ret = this.MemberwiseClone() as FilterBase;
      ret.Query = newQuery;
      return ret;
    }
  }
}
