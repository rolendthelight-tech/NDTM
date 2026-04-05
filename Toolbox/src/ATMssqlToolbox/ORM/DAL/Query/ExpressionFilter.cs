using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class ExpressionFilter : FilterBase
  {
    public ExpressionFilter(string fieldName, FilterOperation expression, object value)
    {
      this.FieldName = fieldName;
      this.Expression = expression;
      this.Value = value;
    }

    public ExpressionFilter(string fieldName, FilterOperation expression)
    {
      this.FieldName = fieldName;
      this.Expression = expression;
    }

    public ExpressionFilter(string fieldName) : this(fieldName, FilterOperation.Equals) { }

    public FilterOperation Expression { get; set; }

    public string FieldName { get; private set; }

    public object Value { get; set; }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      builder.BuildExpression(this);
    }
  }
}
