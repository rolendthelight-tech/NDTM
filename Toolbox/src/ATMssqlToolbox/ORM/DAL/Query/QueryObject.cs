using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Объектная модель запроса, независимого от типа источника данных.
  /// За преобразование ее в конкретный запрос отвечает класс, реализующий IQueryBuilder
  /// </summary>
  [Serializable]
  public class QueryObject
  {
    Dictionary<string, AggregateOperation> aggregation = new Dictionary<string, AggregateOperation>();
    SortFieldCollection ordering = new SortFieldCollection();
    FilterBase rootFilter;

    public FilterBase RootFilter
    {
      get { return rootFilter; }
      set
      {
        rootFilter = value;
        if (value != null)
        {
          rootFilter.Query = this;
        }
      }
    }

    public Dictionary<string, AggregateOperation> Aggregation
    {
      get
      {
        return aggregation;
      }
    }

    public bool Distinct { get; set; }

    public int Top { get; set; }

    public SortFieldCollection Ordering
    {
      get
      {
        return ordering;
      }
    }

    public void AddFilter(FilterBase filter)
    {
      if (filter == null)
      {
        throw new ArgumentException("filter");
      }

      if (this.RootFilter == null)
      {
        this.RootFilter = filter;
      }
      else
      {
        GroupFilter group = this.RootFilter as GroupFilter;
        if (group != null)
        {
          group.Add(filter);
        }
        else
        {
          group = new GroupFilter(true);
          group.Add(this.RootFilter);
          group.Add(filter);
          this.RootFilter = group;
        }
      }
    }

    public void AddAnyFilter(FilterBase fb, bool conjunction)
    {
      if (fb == null)
        return;
      
      GroupFilter rootGroup = this.RootFilter as GroupFilter;

      if (rootGroup != null)
      {
        rootGroup.Add(fb);
        rootGroup.Conjunction = conjunction;
      }
      else if (this.RootFilter == null)
      {
        this.rootFilter = fb;
      }
      else
      {
        FilterBase oldRoot = this.RootFilter;
        rootGroup = new GroupFilter(conjunction);
        rootGroup.Add(fb);
        rootGroup.Add(oldRoot);
        this.rootFilter = rootGroup;
      }
    }

    public void AddFilterExpression(string fieldName, FilterOperation operation, object value)
    {
      if (string.IsNullOrEmpty(fieldName))
        throw new ArgumentException();
      
      ExpressionFilter expr = new ExpressionFilter(fieldName, operation);
      expr.Value = value;

      if (this.RootFilter == null)
      {
        this.RootFilter = expr;
        return;
      }
      else
      {
        ExpressionFilter expr2 = this.rootFilter as ExpressionFilter;
        if (expr2 != null && expr.FieldName == expr2.FieldName)
        {
          this.RootFilter = expr;
          return;
        }
      }

      GroupFilter group = this.RootFilter as GroupFilter;
      if (group != null)
      {
        group.Conjunction = true;
        foreach (FilterBase filter in group)
        {
          ExpressionFilter expr2 = filter as ExpressionFilter;
          if (expr2 != null && expr2.FieldName == fieldName)
          {
            expr2.Value = value;
            expr2.Expression = operation;
            return;
          }
        }
        group.Add(expr);
        return;
      }
      else
      {
        group = new GroupFilter(true);
        group.Add(this.RootFilter);
        group.Add(expr);
        this.RootFilter = group;
      }
    }

    public void RemoveFilterExpression(string fieldName)
    {
      ExpressionFilter filter = this.RootFilter as ExpressionFilter;

      if (filter != null && filter.FieldName == fieldName)
      {
        this.RootFilter = null;
      }
      else
      {
        GroupFilter group = this.RootFilter as GroupFilter;

        foreach (FilterBase flt in group)
        {
          filter = flt as ExpressionFilter;
          if (filter.FieldName == fieldName && filter.Expression == FilterOperation.Equals)
          {
            break;
          }
        }
        if (filter != null)
        {
          group.Remove(filter);
        }
      }
    }

    public void AddFilterExpression(string fieldName, object value)
    {
      if (value != null)
      {
        this.AddFilterExpression(fieldName, FilterOperation.Equals, value);
      }
      else
      {
        this.AddFilter(new NullFilter() { FieldName = fieldName });
      }
    }

    public void BuildPredicates(IQueryBuilder builder)
    {
      if (this.RootFilter != null && this.RootFilter.Enabled)
      {
        builder.BeginFilter();
        this.RootFilter.BuildFilter(builder);
      }
    }

    public void BuildQuery(IQueryBuilder builder)
    {
      Projection prj = new Projection(this.aggregation)
      {
        Distinct = this.Distinct,
        TableName = builder.MainTable,
        Top = this.Top
      };
      builder.BuildProjection(prj);
      this.BuildPredicates(builder);
      int aggCounter = 0;
      foreach (KeyValuePair<string, AggregateOperation> kv in this.aggregation)
      {
        if (kv.Value != AggregateOperation.None)
          aggCounter++;
      }

      if (aggCounter > 0)
      {
        foreach (KeyValuePair<string, ListSortDirection> kv in this.Ordering)
        {
          this.aggregation[kv.Key] = AggregateOperation.Group;
        }
      }

      List<string> groupFileds = new List<string>();
      foreach (KeyValuePair<string, AggregateOperation> kv in aggregation)
      {
        if (kv.Value == AggregateOperation.Group)
        {
          groupFileds.Add(kv.Key);
        }
      }
      if (groupFileds.Count > 0)
      {
        builder.BuildGroupBy(groupFileds);
      }

      if (ordering.Count > 0)
      {
        builder.BuildOrderBy(ordering);
      }
    }

    public QueryObject Clone()
    {
      QueryObject ret = new QueryObject();
      ret.aggregation = new Dictionary<string, AggregateOperation>();
      ret.ordering = new SortFieldCollection();

      foreach (KeyValuePair<string, AggregateOperation> kv in this.aggregation)
      {
        ret.aggregation.Add(kv.Key, kv.Value);
      }

      foreach (KeyValuePair<string, ListSortDirection> kv in this.ordering)
      {
        ret.ordering.Add(kv.Key, kv.Value);
      }

      if (this.rootFilter != null)
      {
        ret.rootFilter = this.rootFilter.Clone(ret);
      }
      return ret;
    }

    public static QueryObject CreateFromFieldValues(Dictionary<string, object> fieldValues)
    {
      QueryObject ret = new QueryObject();
      GroupFilter group = new GroupFilter(true);
      ret.RootFilter = group;
      foreach (KeyValuePair<string, object> kv in fieldValues)
      {
        if (kv.Value != null)
        {
          group.Add(new ExpressionFilter(kv.Key, FilterOperation.Equals, kv.Value));
        }
        else
        {
          group.Add(new NullFilter() { FieldName = kv.Key });
        }
      }

      return ret;
    }
  }
}
