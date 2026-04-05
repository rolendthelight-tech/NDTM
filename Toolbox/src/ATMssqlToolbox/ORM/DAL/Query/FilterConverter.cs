using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using System.ComponentModel;
using System.Reflection;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public static class ClientCriteriaVisitorExtensions
  {
    public static QueryObject GetQueryObject(this IClientCriteriaVisitor item)
    {
      if (item == null)
        return new QueryObject();
      
      MethodInfo method = item.GetType().GetMethod("GetQuery", 
        BindingFlags.Public | BindingFlags.Instance);
      
      if (method == null)
        return new QueryObject();

      return (method.Invoke(item, null) as QueryObject) ?? new QueryObject();
    }

    public static QueryObject GetQueryObject(this CriteriaOperator op, Type destType)
    {
      if (ReferenceEquals(op, null))
        return new QueryObject();

      IClientCriteriaVisitor visitor = (IClientCriteriaVisitor)Activator.CreateInstance(typeof(FilterConverter<>).MakeGenericType(destType));
      op.Accept(visitor);
      return visitor.GetQueryObject();
    }
  }
  
  /// <summary>
  /// Класс, преобразующий DevExpress'овские фильтры в объектную модель запроса
  /// </summary>
  /// <typeparam name="TType"></typeparam>
  class FilterConverter<TType> : IClientCriteriaVisitor
    where TType : class, IBindable, new()
  {
    QueryObject query = new QueryObject();
    List<GroupFilter> m_group_stack = new List<GroupFilter>();

    PropertyDescriptorCollection m_properties = TypeDescriptor.GetProperties(typeof(TType));
    Dictionary<string, string> m_mapping = new Dictionary<string, string>();
    GroupFilter group = new GroupFilter(false);
    public FilterConverter()
    {
      foreach (PropertyDescriptor pd in m_properties)
      {
        string fieldName = RecordManager.GetFieldName(typeof(TType), pd.Name);
        if (!string.IsNullOrEmpty(fieldName))
        {
          m_mapping[fieldName] = pd.Name;
        }
      }
    }

    #region IClientCriteriaVisitor Members

    public object Visit(OperandProperty theOperand)
    {
      bool redirected;
      ExpressionFilter ret = new ExpressionFilter(RecordManager.GetFieldName(typeof(TType), theOperand.PropertyName, out redirected));

      if (!redirected)
        return ret;
      else
        return null;
    }

    public object Visit(AggregateOperand theOperand)
    {
      AggregateOperation operation = AggregateOperation.None;
      switch (theOperand.AggregateType)
      {
        case Aggregate.Avg:
          operation = AggregateOperation.Avg;
          break;

        case Aggregate.Count:
          operation = AggregateOperation.Count;
          break;

        case Aggregate.Max:
          operation = AggregateOperation.Max;
          break;

        case Aggregate.Min:
          operation = AggregateOperation.Min;
          break;

        case Aggregate.Sum:
          operation = AggregateOperation.Sum;
          break;

        default:
          throw new NotSupportedException();
      }
      if (!ReferenceEquals(theOperand.CollectionProperty, null))
      {
        query.Aggregation.Add(RecordManager.GetFieldName(typeof(TType), theOperand.CollectionProperty.PropertyName), operation);
      }
      return null;
    }

    #endregion

    #region ICriteriaVisitor Members

    public object Visit(FunctionOperator theOperator)
    {
      return null;
    }

    public object Visit(OperandValue theOperand)
    {
      return null;
    }

    public object Visit(GroupOperator theOperator)
    {
      var group = new GroupFilter(theOperator.OperatorType == GroupOperatorType.And);
      if (m_group_stack.Count == 0)
      {
        query.RootFilter = group;
      }
      else
      {
        m_group_stack[m_group_stack.Count - 1].Add(group);
      }

      m_group_stack.Add(group);
      try
      {
        foreach (CriteriaOperator curr_op in theOperator.Operands)
        {
          curr_op.Accept(this);
        }
      }
      finally
      {
        m_group_stack.Remove(group);
      }
      return null;
    }

    public object Visit(InOperator theOperator)
    {
      InFilter ret = new InFilter();

      OperandProperty prop = theOperator.LeftOperand as OperandProperty;
      if (ReferenceEquals(prop, null))
        return null;

      ret.FieldName = RecordManager.GetFieldName(typeof(TType), prop.PropertyName);

      foreach (CriteriaOperator op in theOperator.Operands)
      {
        var value = op as OperandValue;

        if (ReferenceEquals(op, null))
          ret.Values.Add(value.Value);
      }

      query.AddAnyFilter(ret, true);
      return ret;
    }

    public object Visit(UnaryOperator theOperator)
    {
      if (theOperator.OperatorType == UnaryOperatorType.IsNull)
      {
        OperandProperty property = theOperator.Operand as OperandProperty;

        if (!ReferenceEquals(property, null))
        {
          NullFilter flt = new NullFilter();
          flt.FieldName = RecordManager.GetFieldName(typeof(TType), property.PropertyName);
          this.query.AddFilter(flt);
        }
      }
      else if (theOperator.OperatorType == UnaryOperatorType.Not)
      {
        FilterConverter<TType> nestedFilter = new FilterConverter<TType>();
        theOperator.Operand.Accept(nestedFilter);//
        query.AddFilter(new NegateFilter(nestedFilter.query.RootFilter));
      }
      return null;
    }

    public object Visit(BinaryOperator theOperator)
    {
      ExpressionFilter filter = theOperator.LeftOperand.Accept(this) as ExpressionFilter;
      OperandValue value = theOperator.RightOperand as OperandValue;

      if (filter != null
        && !ReferenceEquals(value, null))
      {
        this.SetExpression(filter.FieldName, theOperator.OperatorType, value);
        return null;
      }

      if (ReferenceEquals(theOperator.RightOperand, null))
      {
        return null;
      }

      filter = theOperator.RightOperand.Accept(this) as ExpressionFilter;
      value = theOperator.LeftOperand as OperandValue;
      if (filter != null
        && !ReferenceEquals(value, null))
      {
        this.SetExpression(filter.FieldName, theOperator.OperatorType, value);
      }

      return null;
    }

    public object Visit(BetweenOperator theOperator)
    {
      try
      {
        GroupFilter ret = new GroupFilter(true);

        ret.Add( new ExpressionFilter(((OperandProperty)theOperator.Property).PropertyName,
          FilterOperation.More, ((OperandValue)theOperator.BeginExpression).Value));
        
        ret.Add(new ExpressionFilter(((OperandProperty)theOperator.Property).PropertyName,
          FilterOperation.Less, ((OperandValue)theOperator.EndExpression).Value));

        if (query.RootFilter == null)
          query.RootFilter = ret;
        //return ret;
        group = m_group_stack.Count > 0 ? m_group_stack[m_group_stack.Count - 1] : null;

        if (group != null)
        {
          group.Add(ret);
        }
        else
        {
          query.RootFilter = ret;
        }
      }
      catch (Exception ex)
      {
        return null;
      }
      return null;
    }

    #endregion

    private void SetExpression(string fieldName, BinaryOperatorType operatorType, OperandValue value)
    {
      Type propertyType = m_properties[m_mapping[fieldName]].PropertyType;
      if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        propertyType = propertyType.GetGenericArguments()[0];
      }

      if (propertyType.IsEnum && value.Value is string)
      {
        value.Value = Enum.Parse(propertyType, value.Value as string);
      }

      FilterOperation operation = FilterOperation.Equals;
      switch (operatorType)
      {
        case BinaryOperatorType.Equal:
          operation = FilterOperation.Equals;
          break;

        case BinaryOperatorType.Less:
          operation = FilterOperation.Less;
          break;

        case BinaryOperatorType.LessOrEqual:
          operation = FilterOperation.Less | FilterOperation.Equals;
          break;

        case BinaryOperatorType.NotEqual:
          operation = FilterOperation.NotEquals;
          break;

        case BinaryOperatorType.Greater:
          operation = FilterOperation.More;
          break;

        case BinaryOperatorType.GreaterOrEqual:
          operation = FilterOperation.More | FilterOperation.Equals;
          break;

        case BinaryOperatorType.Like:
          operation = FilterOperation.Like;
          break;

        default:
          throw new NotSupportedException();
      }

      /*GroupFilter*/ group = m_group_stack.Count > 0 ? m_group_stack[m_group_stack.Count - 1] : null;

      if (group != null)
      {
        group.Add(new ExpressionFilter(fieldName, operation, value.Value));
      }
      else
      {
        query.RootFilter = new ExpressionFilter(fieldName, operation, value.Value);
      }
    }

    public QueryObject GetQuery()
    {
      return query;
    }

		#region IClientCriteriaVisitor Members

		public object Visit(JoinOperand theOperand)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
