using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using AT.Toolbox.MSSQL.DAL.RecordBinding;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Sql
{
  public class SqlQueryBuilder : IQueryBuilder
  {
    StringBuilder sb = new StringBuilder();
    Dictionary<int, object> paramValues = new Dictionary<int, object>();
    Random numGenerator = new Random();
    
    #region IQueryBuilder Members

    public string MainTable { get; set; }

    public void BuildProjection(Projection prj)
    {
      string table = prj.TableName;
      sb.Append("SELECT ");
      if (prj.Distinct)
      {
        sb.Append("DISTINCT ");
      }
      if (prj.Top > 0)
      {
        sb.Append("TOP " + prj.Top + " ");
      }
      int counter = 0;

      if (prj.Aggregation.Count == 0)
      {
        sb.Append("[" + table + "].* ");
      }
      else
      {
        foreach (KeyValuePair<string, AggregateOperation> kv in prj.Aggregation)
        {
          if (counter > 0)
          {
            sb.Append(", ");
          }
          if (kv.Value == AggregateOperation.None ||
            kv.Value == AggregateOperation.Group)
          {
            sb.Append("[" + table + "].[" + kv.Key + "]");
          }
          else if (kv.Key != "*")
          {
            sb.Append(kv.Value.ToString() + "([" + table + "].[" + kv.Key + "]) AS "
              + "[" + kv.Key + "]");
          }
          else
          {
            sb.Append(kv.Value.ToString() + "(*)");
          }
          counter++;
        }
      }
      sb.Append(" FROM [" + table + "]");
    }

    public void BeginFilter()
    {
      sb.Append(" WHERE ");
    }

    public void BuildExpression(ExpressionFilter filter)
    {
      sb.Append(" [" + filter.FieldName + "]" 
        + this.GetExpressionText(filter.Expression) + "@_" 
        + this.GetParameterNum(filter.Value));
    }

    public void BuildIn(InFilter inFilter)
    {
      sb.Append(" [" + inFilter.FieldName + "] IN (");
      List<string> parms = new List<string>();

      foreach (object value in inFilter.Values)
      {
        parms.Add("@_" + this.GetParameterNum(value));
      }
      sb.Append(string.Join(", ",parms.ToArray()) + ")");
    }

    public void BuildIsNull(NullFilter nullFilter)
    {
      sb.Append("[" + nullFilter.TableName + "].[" + nullFilter.FieldName + "] IS NULL ");
    }

    public void StartBlock()
    {
      sb.Append("(");
    }

    public void EndBlock()
    {
      sb.Append(")");
    }

    public void Negate()
    {
      sb.Append(" NOT ");
    }

    public void BuildSeparator(bool conjunction)
    {
      sb.Append(conjunction ? " AND " : " OR ");
    }

    public void Exists()
    {
      sb.Append(" EXISTS ");
    }

    public void BuildJoiner(Joiner joiner)
    {
      sb.Append("[" + joiner.TableName + "].[" + joiner.ParentField + "] = ["
        + joiner.ChildTable + "].[" + joiner.ChildField + "]");
    }

    public void BuildGroupBy(List<string> groups)
    {
      sb.Append(" GROUP BY ");
      for (int i = 0; i < groups.Count; i++)
      {
        if (i > 0)
        {
          sb.Append(", ");
        }
        sb.Append("[" + groups[i] + "]");
      }
    }

    public void BuildOrderBy(SortFieldCollection ordering)
    {
      sb.Append(" ORDER BY ");

      int counter = 0;

      foreach (KeyValuePair<string, ListSortDirection> kv in ordering)
      {
        if (counter > 0)
        {
          sb.Append(", ");
        }
        sb.Append("[" + kv.Key + "] " + (kv.Value == ListSortDirection.Ascending ? "ASC" : "DESC"));
        counter++;
      }
    }

    public object GetResult()
    {
      string ret = sb.ToString();
      foreach (KeyValuePair<int, object> kv in paramValues)
      {
        object value = kv.Value ?? " NULL ";
        if (value is string)
        {
          value = "\'" + value + "\'";
        }
        ret.Replace("@_" + kv.Key, value.ToString());
      }
      return ret;
    }

      public void BuildCustomText(string customSuffix)
      {
          sb.Append(" " + customSuffix);
      }

      #endregion

    private string GetExpressionText(FilterOperation expr)
    {
      switch (expr)
      {
        case FilterOperation.Equals: return " = ";
        case FilterOperation.Less: return " < ";
        case FilterOperation.More: return " > ";
        case FilterOperation.Like: return " LIKE ";
        case FilterOperation.NotEquals: return " <> ";
        case FilterOperation.Equals | FilterOperation.Less: return " <= ";
        case FilterOperation.More | FilterOperation.Equals: return " >= ";
      }
      throw new NotSupportedException();
    }

    private int GetParameterNum(object value)
    {
      int ret;
      do
      {
        ret = numGenerator.Next(1000);
      } while (paramValues.ContainsKey(ret));
      paramValues.Add(ret, value);
      return ret;
    }

    public string GetQueryString()
    {
      return sb.ToString();
    }

    public Dictionary<int, object> GetParameters()
    {
      return this.paramValues;
    }
  }
}
