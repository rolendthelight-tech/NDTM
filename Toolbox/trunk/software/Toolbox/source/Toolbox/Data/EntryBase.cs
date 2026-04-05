using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Properties;
using Toolbox.Extensions;

namespace Toolbox.Data
{
  [DataContract]
  public abstract class EntryBase : IPropertyTracker, INotifyPropertyChanging, INotifyPropertyChanged
  {
    private PropertyValueContainer m_container;

    private void Initialize()
    {
      if (m_container == null)
        m_container = new PropertyValueContainer(this);
    }

    [OnDeserializing]
    private void OnDeserializing(StreamingContext sc)
    {
      this.Initialize();
    }

    protected EntryBase()
    {
      this.Initialize();
    }

    protected TType _<TType>([NotNull] Expression<Func<TType>> property)
    {
	    if (property == null) throw new ArgumentNullException("property");

	    return m_container.Get(property);
    }

	  protected void _<TType>([NotNull] Expression<Func<TType>> property, TType value)
	  {
		  if (property == null) throw new ArgumentNullException("property");

		  m_container.Set(property, value);
	  }

	  public override string ToString()
    {
      return this.GetDisplayName();
    }

    protected string GetDisplayName()
    {
      string ret = null;

      if (this.GetType().IsDefined(typeof(DisplayNameAttribute), true))
        ret = this.GetType().GetCustomAttribute<DisplayNameAttribute>(true).DisplayName;

      return string.IsNullOrEmpty(ret) ? base.ToString() : ret;
    }

    protected bool AutoCheckLookupProperties([NotNull] InfoBuffer buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      bool ret = true;

      ILookupProvider lookup_provider = this as ILookupProvider;

      if (lookup_provider != null)
      {
        var ctxt = new ContextImpl
	        {
		        Instance = lookup_provider
	        };

	      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this))
        {
          if (pd.Converter is LookupTypeConverter)
          {
            bool found = false;
            object value = pd.GetValue(this);
            ctxt.PropertyDescriptor = pd;

            foreach (var std_value in pd.Converter.GetStandardValues(ctxt))
            {
              if (value == null)
              {
                if (std_value == null)
                {
                  found = true;
                  break;
                }
              }
              else
              {
                if (value.Equals(std_value))
                {
                  found = true;
                  break;
                }
              }
            }

            if (!found)
            {
              if (value == null)
              {
                var error = pd.Attributes[typeof(EmptyErrorMessageAttribute)] as EmptyErrorMessageAttribute;

                if (error != null)
                  buffer.Add(error.ErrorMessage, error.Level);
                else
                  buffer.Add(string.Format(Resources.INVALID_LOOKUP_PROPERTY, this, pd.DisplayName), InfoLevel.Warning);
              }
              else
              {
                var error_message = pd.Attributes[typeof(ErrorMessageAttribute)] as ErrorMessageAttribute;

                if (error_message != null)
                  buffer.Add(error_message.ErrorMessage.Replace("[value]", value.ToString()), error_message.Level);
                else
                  buffer.Add(string.Format(Resources.INVALID_LOOKUP_PROPERTY, this, pd.DisplayName), InfoLevel.Warning);
                ret = false;
              }
              ret = false;
            }
          }
        }
      }

      return ret;
    }

    protected virtual ICommand UpdatePropertyChangedCommand(PropertyChangedCommand command)
    {
      return command;
    }

    #region IPropertyTracker Members

    ICommand IPropertyTracker.UpdatePropertyChangedCommand(PropertyChangedCommand command)
    {
      return this.UpdatePropertyChangedCommand(command);
    }

    void IPropertyTracker.RaisePropertyChanging(string propertyName)
    {
      if (this.PropertyChanging != null)
        this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
    }

    void IPropertyTracker.RaisePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    object IPropertyTracker.this[string propertyName]
    {
      get
      {
	      if (propertyName == null) throw new ArgumentNullException("propertyName");

	      return m_container[propertyName];
      }
	    set
	    {
		    if (propertyName == null) throw new ArgumentNullException("propertyName");

		    m_container[propertyName] = value;
	    }
    }

    #endregion

    private class ContextImpl : ITypeDescriptorContext
    {
      #region ITypeDescriptorContext Members

      public IContainer Container { get; set; }

      public object Instance { get; set; }

      public void OnComponentChanged()
      {
      }

      public bool OnComponentChanging()
      {
        return true;
      }

      public PropertyDescriptor PropertyDescriptor { get; set; }

      #endregion

      #region IServiceProvider Members

      public object GetService([NotNull] Type serviceType)
      {
	      if (serviceType == null) throw new ArgumentNullException("serviceType");

	      return Activator.CreateInstance(serviceType);
      }

	    #endregion
    }


    #region ICommandHistoryOwner Members

    [Browsable(false)]
    public abstract CommandHistory History { get; }

    #endregion

    #region INotifyPropertyChanging Members

    public event PropertyChangingEventHandler PropertyChanging;

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region ISerializable Members

    protected void GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
    {
	    if (info == null) throw new ArgumentNullException("info");

	    foreach (var property in m_container.GetFieldNames())
      {
        info.AddValue(property, m_container[property]);
      }
    }

	  protected EntryBase([NotNull] SerializationInfo info, StreamingContext context)
      :this()
	  {
		  if (info == null) throw new ArgumentNullException("info");

		  foreach (SerializationEntry se in info)
      {
        m_container[se.Name] = se.Value;
      }
	  }

	  #endregion
  }
}
