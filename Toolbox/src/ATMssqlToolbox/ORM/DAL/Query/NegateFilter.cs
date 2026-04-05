using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class NegateFilter : FilterBase
  {
    public NegateFilter(FilterBase filter)
    {
      this.NegatedFilter = filter;
    }
    
    public FilterBase NegatedFilter { get; private set; }

    public override bool Enabled
    {
      get
      {
        return this.NegatedFilter != null && this.NegatedFilter.Enabled;
      }
    }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      if (this.NegatedFilter != null && this.NegatedFilter.Enabled)
      {
        this.NegatedFilter.Query = this.Query;
        if (string.IsNullOrEmpty(this.NegatedFilter.TableName))
        {
          this.NegatedFilter.TableName = builder.MainTable;
        }

        builder.Negate();
        if (this.NegatedFilter is GroupFilter)
        {
          this.NegatedFilter.BuildFilter(builder);
        }
        else
        {
          builder.StartBlock();
          this.NegatedFilter.BuildFilter(builder);
          builder.EndBlock();
        }
      }
    }

    public override FilterBase Clone(QueryObject newQuery)
    {
      NegateFilter ret = base.Clone(newQuery) as NegateFilter;

      ret.NegatedFilter = this.NegatedFilter != null ? this.NegatedFilter.Clone(newQuery) : null;

      return ret;
    }
  }
}
