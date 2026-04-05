using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AT.Toolbox.MSSQL.Properties;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class SortFieldCollection : IEnumerable<KeyValuePair<string, ListSortDirection>>
  {
    List<KeyValuePair<string, ListSortDirection>> m_list = new List<KeyValuePair<string, ListSortDirection>>();

    public int Count
    {
      get { return m_list.Count; }
    }

    public void Add(string fieldName, ListSortDirection direction)
    {
      if (string.IsNullOrEmpty(fieldName))
        throw new ArgumentNullException("fieldName");

      foreach (KeyValuePair<string, ListSortDirection> kv in m_list)
      {
        if (kv.Key.ToUpper() == fieldName.ToUpper())
        {
          throw new ArgumentException(string.Format(Resources.QUERY_DUPLICATE_SORT_FIELD, fieldName));
        }
      }
      m_list.Add(new KeyValuePair<string, ListSortDirection>(fieldName, direction)); 
    }

    public void Clear()
    {
      m_list.Clear();
    }

    public KeyValuePair<string, ListSortDirection> this[int index]
    {
      get
      {
        return m_list[index];
      }
    }

    public ListSortDirection this[string fieldName]
    {
      get
      {
        if (string.IsNullOrEmpty(fieldName))
          throw new ArgumentNullException("fieldName");

        foreach (KeyValuePair<string, ListSortDirection> kv in m_list)
        {
          if (kv.Key.ToUpper() == fieldName.ToUpper())
          {
            return kv.Value;
          }
        }

        throw new KeyNotFoundException();
      }
      set
      {
        if (string.IsNullOrEmpty(fieldName))
          throw new ArgumentNullException("fieldName");

        for (int i = 0; i < m_list.Count; i++)
        {
          if (m_list[i].Key.ToUpper() == fieldName.ToUpper())
          {
            m_list[i] = new KeyValuePair<string, ListSortDirection>(fieldName, value);
            return;
          }
        }

        m_list.Add(new KeyValuePair<string, ListSortDirection>(fieldName, value));
      }
    }
    
    public IEnumerator<KeyValuePair<string, ListSortDirection>> GetEnumerator()
    {
      return m_list.GetEnumerator();
    }

    #region Члены IEnumerable

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}
