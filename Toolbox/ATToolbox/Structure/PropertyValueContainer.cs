using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AT.Toolbox
{
  public class PropertyValueContainer
  {
    private readonly IPropertyTracker m_owner;
    private readonly Dictionary<string, object> m_values = new Dictionary<string,object>();
    private readonly SharedSource<string, ConvertionHelper> m_converters;

    public PropertyValueContainer(IPropertyTracker owner)
    {
      if (owner == null)
        throw new ArgumentNullException("owner");

      m_owner = owner;
      m_converters = new SharedSource<string, ConvertionHelper>(this.CreateConvertionHelper);
    }

    public TType Get<TType>(Expression<Func<TType>> property)
    {
      return this.GetReturnValue<TType>(this.GetPropertyName(property));
    }

    public void Set<TType>(Expression<Func<TType>> property, TType value)
    {
      string propertyName = this.GetPropertyName(property);

      m_owner.RaisePropertyChanging(propertyName);
      var oldValue = this.GetReturnValue<TType>(propertyName);

      var history = m_owner.History;

      if (history != null)
      {
        history.AddCommand(new PropertyChangedCommand
        {
          Item = m_owner,
          NewValue = value,
          OldValue = oldValue,
          PropertyName = propertyName
        });
      }

      m_values[propertyName] = value;
      m_owner.RaisePropertyChanged(propertyName);
    }

    public object this[string propertyName] 
    {
      get
      {
        this.ValidatePropertyName(propertyName);
        return m_values[propertyName];
      }
      set
      {
        this.ValidatePropertyName(propertyName);
        m_values[propertyName] = m_converters.Lookup(propertyName).Convert(value);
      }
    }

    public string[] GetFieldNames()
    {
      return m_values.Keys.ToArray();
    }

    private TType GetReturnValue<TType>(string propertyName)
    {
      this.ValidatePropertyName(propertyName);

      object ret;

      if (m_values.TryGetValue(propertyName, out ret))
      {
        ret = this.ProcessPropertyType<TType>(propertyName, ret);
        m_values[propertyName] = ret;
      }
      else
        ret = default(TType);

      return (TType)ret;
    }

    private TType ProcessPropertyType<TType>(string propertyName, object ret)
    {
      this.ValidatePropertyName(propertyName);

      if (ret == null && !typeof(TType).IsNullable())
        ret = default(TType);

      return (TType)m_converters.Lookup(propertyName).Convert(ret);
    }

    private ConvertionHelper CreateConvertionHelper(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ArithmeticException("propertyName");

      var pd = TypeDescriptor.GetProperties(m_owner)[propertyName];

      if (pd == null)
        throw new ArgumentOutOfRangeException("propertyName");

      return new ConvertionHelper(pd.PropertyType, () => propertyName);
    }

    private string GetPropertyName<TType>(Expression<Func<TType>> property)
    {
      return ((MemberExpression)property.Body).Member.Name;
    }

    private void ValidatePropertyName(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ArgumentNullException("propertyName");
    }
  }



  public interface IPropertyTracker : ICommandHistoryOwner
  {
    void RaisePropertyChanging(string propertyName);

    void RaisePropertyChanged(string propertyName);

    object this[string propertyName] { get; set; }
  }
}
