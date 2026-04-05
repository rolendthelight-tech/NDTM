using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class Projection : FilterBase
  {
    Dictionary<string, AggregateOperation> m_aggregation;

    internal Projection(Dictionary<string, AggregateOperation> aggregation)
    {
      m_aggregation = aggregation ?? new Dictionary<string, AggregateOperation>();
    }

    internal Projection() : this(null) { }

    public bool Distinct { get; set; }

    public int Top { get; set; }

    public Dictionary<string, AggregateOperation> Aggregation
    {
      get { return m_aggregation; }
    }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      builder.BuildProjection(this);
    }
  }
}
