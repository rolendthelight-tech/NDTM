using System;
using System.Collections.Generic;
using System.Text;
using AT.Toolbox.MSSQL.Properties;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public class DictionaryFieldBinder<TType> : IFieldBinder
    where TType : class, IBindable, new()
  {
    readonly ObjectTextStore[] fieldValues;
    readonly int[] hashCodes;

    public DictionaryFieldBinder(Dictionary<string, object> defaults)
    {
      if (defaults == null)
      {
        throw new ArgumentNullException("fieldTypes");
      }

      this.fieldValues = new ObjectTextStore[defaults.Count];
      this.hashCodes = new int[defaults.Count];

      int i = 0;
      foreach (KeyValuePair<string, object> kv in defaults)
      {
        this.fieldValues[i] = new ObjectTextStore()
        {
          Text = kv.Key,
          Value = kv.Value
        };
        this.hashCodes[i] = kv.Key.GetHashCode();
        i++;
      }
    }

    #region IFieldBinder<TType> Members

    public object GetFieldValue(string fieldName)
    {
      if (fieldName == null)
        throw new ArgumentNullException("fieldName");

      int fieldHashCode = fieldName.GetHashCode();

      for (int i = 0; i < this.fieldValues.Length; i++)
      {
        if (this.hashCodes[i] == fieldHashCode
          && this.fieldValues[i].Text.Equals(fieldName))
        {
          return fieldValues[i].Value;
        }
      }
      throw new KeyNotFoundException();
    }

    public void SetFieldValue(string fieldName, object newValue)
    {
      if (fieldName == null)
        throw new ArgumentNullException("fieldName");

      int fieldHashCode = fieldName.GetHashCode();

      for (int i = 0; i < this.fieldValues.Length; i++)
      {
        if (this.hashCodes[i] == fieldHashCode
          && this.fieldValues[i].Text.Equals(fieldName))
        {
          fieldValues[i].Value = newValue;
          return;
        }
      }
      throw new KeyNotFoundException();
    }

    public IFieldBinder Clone(IBindable newObject)
    {
      Dictionary<string, object> values = new Dictionary<string, object>();

      foreach (ObjectTextStore ots in this.fieldValues)
      {
        values.Add(ots.Text, ots.Value);
      }

      DictionaryFieldBinder<TType> ret = new DictionaryFieldBinder<TType>(values);
      return ret;
    }

    public bool ReadDataFields(string[] fieldNames, IRecordBinder recordBinder)
    {
      bool ret = true;
      for (int i = 0; i < fieldNames.Length; i++)
      {
        int fieldHashCode = fieldNames[i].GetHashCode();
        bool found = false;
        
        for (int j = 0; j < this.fieldValues.Length; j++)
        {
          if (this.hashCodes[j] == fieldHashCode
            && this.fieldValues[j].Text.Equals(fieldNames[i]))
          {
            fieldValues[j].Value = recordBinder.GetFieldValue(i);
            found = true;
            break;
          }
        }
        ret = ret && found;
      }
      return ret;
    }

    #endregion
  }
}