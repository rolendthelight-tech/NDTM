using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using AT.Toolbox.Properties;

namespace AT.Toolbox
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

    protected TType _<TType>(Expression<Func<TType>> property)
    {
      return m_container.Get(property);
    }

    protected void _<TType>(Expression<Func<TType>> property, TType value)
    {
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

    protected bool AutoCheckLookupProperties(InfoBuffer buffer)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");
      
      bool ret = true;

      ILookupProvider lookupProvider = this as ILookupProvider;

      if (lookupProvider != null)
      {
        ContextImpl ctxt = new ContextImpl();
        ctxt.Instance = lookupProvider;
        
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

    #region IPropertyTracker Members

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
      get { return m_container[propertyName]; }
      set { m_container[propertyName] = value; }
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

      public object GetService(Type serviceType)
      {
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

    protected void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      foreach (var property in m_container.GetFieldNames())
      {
        info.AddValue(property, m_container[property]);
      }
    }

    protected EntryBase(SerializationInfo info, StreamingContext context)
      :this()
    {
      foreach (SerializationEntry se in info)
      {
        m_container[se.Name] = se.Value;
      }
    }

    #endregion
  }
}
