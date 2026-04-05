using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class EditableCollection<TType> : IEditableCollection, IList<TType>, IList
    where TType: class, IBindable, new()
  {
    private List<EditableItem> m_items = new List<EditableItem>();
    private List<TType> m_items_to_delete = new List<TType>();

    public EditableCollection() { }

    public bool Binding { get; set; }

    #region IList<TType> Members

    public int IndexOf(TType item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }
      
      for (int i = 0; i < m_items.Count; i++)
      {
        if (item.Equals(m_items[i].Item))
        {
          return i;
        }
      }
      return -1;
    }

    public void Insert(int index, TType item)
    {
      object identifier = item.GetIdentifier();

      if (identifier != null)
      {
        List<int> undo_remove_indexes = new List<int>();
        for (int i = 0; i < m_items_to_delete.Count; i++)
        {
          if (identifier.Equals(m_items_to_delete[i].GetIdentifier()))
          {
            undo_remove_indexes.Add(i);
          }
        }
        foreach (int i in undo_remove_indexes)
        {
          m_items_to_delete.RemoveAt(i);
        }
      }
      m_items.Insert(index, new EditableItem()
      {
        Added = !this.Binding && m_items_to_delete.Count == 0,
        Item = item
      });
    }

    public void RemoveAt(int index)
    {
      if (!m_items[index].Added)
      {
        m_items_to_delete.Add(m_items[index].Item);
      }
      m_items.RemoveAt(index);
    }

    public TType this[int index]
    {
      get
      {
        return m_items[index].Item;
      }
      set
      {
        this.RemoveAt(index);
        this.Insert(index, value);
      }
    }

    #endregion

    #region ICollection<TType> Members

    public void Add(TType item)
    {
      this.Insert(this.Count, item);
    }

    public void Clear()
    {
      foreach (EditableItem item in m_items)
      {
        if (!item.Added)
        {
          m_items_to_delete.Add(item.Item);
        }
      }
      m_items.Clear();
    }

    public bool Contains(TType item)
    {
      if (item == null)
        return false;

      foreach (EditableItem current in m_items)
      {
        if (item.Equals(current))
        {
          return true;
        }
      }
      return false;
    }

    public int Count
    {
      get { return m_items.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; ; }
    }

    public bool Remove(TType item)
    {
      int index = this.IndexOf(item);

      if (index >= 0)
      {
        this.RemoveAt(index);
        return true;
      }
      return false;
    }

    #endregion

    #region IEnumerable<TType> Members

    public IEnumerator<TType> GetEnumerator()
    {
      foreach (EditableItem current in m_items)
      {
        yield return current.Item;
      }
    }

    #endregion

    #region ICollection<TType> Members

    void ICollection<TType>.CopyTo(TType[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion

    [Serializable]
    private struct EditableItem
    {
      public bool Added;
      public TType Item;
    }

    #region IEditableCollection Members

    public bool IsAdded(int index)
    {
      return m_items[index].Added;
    }

    IBindable IEditableCollection.this[int index]
    {
      get { return this[index]; }
    }

    IBindable[] IEditableCollection.ItemsToDelete
    {
      get { return m_items_to_delete.ConvertAll<IBindable>(p => p).ToArray(); }
    }

    void IEditableCollection.Add(IBindable item)
    {
      this.Add((TType)item);
    }

    #endregion

    #region IList Members

    int IList.Add(object value)
    {
      this.Add((TType)value);
      return this.Count - 1;
    }

    bool IList.Contains(object value)
    {
      if (value is TType)
      {
        return this.Contains((TType)value);
      }
      return m_items.Where(item => item.Item.GetIdentifier() == value).Count() > 0;
    }

    int IList.IndexOf(object value)
    {
      return this.IndexOf((TType)value);
    }

    void IList.Insert(int index, object value)
    {
      this.Insert(index, (TType)value);
    }

    bool IList.IsFixedSize
    {
      get { return false; }
    }

    bool IList.IsReadOnly
    {
      get { return false; }
    }

    void IList.Remove(object value)
    {
      this.Remove((TType)value);
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        this[index] = (TType)value;
      }
    }

    #endregion

    #region ICollection Members

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get { return null; }
    }

    #endregion
  }

  internal interface IEditableCollection : IEnumerable
  {
    bool Binding { get; set; }
    int Count{get;}
    IBindable this[int index] { get; }
    bool IsAdded(int index);
    IBindable[] ItemsToDelete { get; }
    void Add(IBindable item);
  }
}
