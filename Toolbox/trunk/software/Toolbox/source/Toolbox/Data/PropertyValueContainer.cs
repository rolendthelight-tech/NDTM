using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Extensions;

namespace Toolbox.Data
{
  public class PropertyValueContainer
  {
    private readonly IPropertyTracker m_owner;
    private readonly Dictionary<string, object> m_values = new Dictionary<string,object>();
    private readonly SharedSource<string, ConvertionHelper> m_converters;

    public PropertyValueContainer([NotNull] IPropertyTracker owner)
    {
      if (owner == null)
        throw new ArgumentNullException("owner");

      m_owner = owner;
      m_converters = new SharedSource<string, ConvertionHelper>(this.CreateConvertionHelper);
    }

    public TType Get<TType>([NotNull] Expression<Func<TType>> property)
    {
	    if (property == null) throw new ArgumentNullException("property");

	    return this.GetReturnValue<TType>(this.GetPropertyName(property));
    }

	  public void Set<TType>([NotNull] Expression<Func<TType>> property, TType value)
    {
	    if (property == null) throw new ArgumentNullException("property");

	    string property_name = this.GetPropertyName(property);

      m_owner.RaisePropertyChanging(property_name);
      var oldValue = this.GetReturnValue<TType>(property_name);

      var history = m_owner.History;

      if (history != null)
      {
        history.AddCommand(m_owner.UpdatePropertyChangedCommand(new PropertyChangedCommand
        {
          Item = m_owner,
          NewValue = value,
          OldValue = oldValue,
          PropertyName = property_name
        }));
      }

      m_values[property_name] = value;
      m_owner.RaisePropertyChanged(property_name);
    }

    public object this[[NotNull] string propertyName] 
    {
      get
      {
	      if (propertyName == null) throw new ArgumentNullException("propertyName");

	      return m_values[propertyName];
      }
	    set
	    {
		    if (propertyName == null) throw new ArgumentNullException("propertyName");

		    m_values[propertyName] = m_converters.Lookup(propertyName).ConvertInvariant(value);
	    }
    }

    public string[] GetFieldNames()
    {
      return m_values.Keys.ToArray();
    }

    private TType GetReturnValue<TType>([NotNull] string propertyName)
    {
	    if (propertyName == null) throw new ArgumentNullException("propertyName");

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

    private TType ProcessPropertyType<TType>([NotNull] string propertyName, object ret)
    {
	    if (propertyName == null) throw new ArgumentNullException("propertyName");

	    if (ret == null && !typeof(TType).IsNullable())
        ret = default(TType);

      return (TType)m_converters.Lookup(propertyName).ConvertInvariant(ret);
    }

    private ConvertionHelper CreateConvertionHelper([NotNull] string propertyName)
    {
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Empty", "propertyName");

      var pd = TypeDescriptor.GetProperties(m_owner)[propertyName];

      if (pd == null)
				throw new ArgumentException("Not found", "propertyName");

      return new ConvertionHelper(pd.PropertyType, () => propertyName);
    }

    private string GetPropertyName<TType>([NotNull] Expression<Func<TType>> property)
    {
	    if (property == null) throw new ArgumentNullException("property");

	    return ((MemberExpression)property.Body).Member.Name;
    }
  }

  public interface IPropertyTracker : ICommandHistoryOwner
  {
    void RaisePropertyChanging(string propertyName);

    void RaisePropertyChanged(string propertyName);

    ICommand UpdatePropertyChangedCommand([NotNull] PropertyChangedCommand command);

    object this[[NotNull] string propertyName] { get; set; }
  }
}
