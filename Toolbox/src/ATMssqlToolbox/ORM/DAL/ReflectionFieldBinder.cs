using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using AT.Toolbox.MSSQL.Properties;
using AT.Toolbox.Log;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public class ReflectionFieldBinder<TType> : IFieldBinder
    where TType : class, IBindable, new()
  {
    TType currentObject;
    bool use_m_prefix;

    public ReflectionFieldBinder(TType currentObject, bool use_m_prefix)
    {
      if (currentObject == null)
      {
        throw new ArgumentNullException("currentObject");
      }
      if (currentObject.GetType() != typeof(TType))
      {
        throw new ArgumentException(string.Format(Resources.ReflectionBuilder_TypeMismatch, typeof(TType), currentObject.GetType()));
      }
      this.currentObject = currentObject;
      this.use_m_prefix = use_m_prefix;
    }

    #region IFieldBinder Members

    public object GetFieldValue(string fieldName)
    {
      if (this.use_m_prefix)
      {
        fieldName = "m_" + fieldName;
      }
      return typeof(TType).GetField(fieldName
          , BindingFlags.Public
          | BindingFlags.NonPublic
          | BindingFlags.Instance).GetValue(currentObject);
    }

    public void SetFieldValue(string fieldName, object newValue)
    {
      if (this.use_m_prefix)
      {
        fieldName = "m_" + fieldName;
      }
      typeof(TType).GetField(fieldName
          , BindingFlags.Public
          | BindingFlags.NonPublic
          | BindingFlags.Instance).SetValue(currentObject, newValue);
    }

    public IFieldBinder Clone(IBindable newObject)
    {
      ReflectionFieldBinder<TType> ret = new ReflectionFieldBinder<TType>((TType)newObject, this.use_m_prefix);
      return ret;
    }

    public bool ReadDataFields(string[] fieldNames, IRecordBinder recordBinder)
    {
      bool ret = true;
      for (int i = 0; i < fieldNames.Length; i++)
      {
        string fieldName = this.use_m_prefix ? "m_" + fieldNames[i] : fieldNames[i];
        try
        {
          object value = recordBinder.GetFieldValue(i);
          FieldInfo field =  typeof(TType).GetField(fieldName
              , BindingFlags.Public
              | BindingFlags.NonPublic
              | BindingFlags.Instance);
          if (value != null && field.FieldType.IsGenericType
              && field.FieldType.GetGenericArguments()[0].IsEnum)
          {
            value = Enum.ToObject(field.FieldType.GetGenericArguments()[0], value);
          }
          field.SetValue(currentObject, value);
        }
        catch (Exception ex)
        {
          ret = false;
        }
      }
      return ret;
    }

    #endregion
  }
}
