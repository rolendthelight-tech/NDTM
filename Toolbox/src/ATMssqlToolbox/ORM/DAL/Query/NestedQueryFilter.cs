using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class NestedQueryFilter : Joiner
  {
    public NestedQueryFilter(string parentTable, string childTable, string parentField, string childField)
      : base(parentTable, childTable, parentField, childField) { }

    public NestedQueryFilter(QueryObject query, string childTable, string parentField, string childField)
      : base (query, childTable, parentField, childField) { }

    public FilterBase Filter { get; set; }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      Joiner joiner = new Joiner(this);
      Projection prj = new Projection()
      {
        Distinct = false,
        TableName = this.ChildTable
      };
      
      builder.Exists();
      builder.StartBlock();
      builder.BuildProjection(prj);//(new Dictionary<string, AggregateOperation>(), this.ChildTable, false);
      builder.BeginFilter();
      builder.BuildJoiner(joiner);

      if (this.Filter != null && this.Filter.Enabled)
      {
        this.Filter.TableName = this.ChildTable;
        builder.BuildSeparator(true);
        this.Filter.BuildFilter(builder);
      }
      builder.EndBlock();
    }

    public override FilterBase Clone(QueryObject newQuery)
    {
      NestedQueryFilter ret = base.Clone(newQuery) as NestedQueryFilter;

      ret.Filter = this.Filter != null ? this.Filter.Clone(newQuery) : null;

      return ret;
    }
  }
}
