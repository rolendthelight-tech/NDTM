using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Reflection;
using JetBrains.Annotations;
using Toolbox.Threading;

namespace Toolbox.Data
{
  [Serializable]
  public abstract class VirtualizedCollection<TType> : IList<TType>, IList
  {
    #region Fields

    [NonSerialized]
    private LockSource m_lock = new LockSource();
    private volatile int m_count = -1;

    #endregion

    #region Serialization

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (m_lock == null)
        m_lock = new LockSource();

      m_count = -1;
    }

    #endregion

    #region Overridable

    protected abstract int GetItemCount();

    protected abstract TType GetItem(int index);

    protected virtual void SetItem(int index, TType item)
    {
      throw new NotSupportedException();
    }

    protected virtual void InsertItem(int index, TType item)
    {
      throw new NotSupportedException();
    }

    protected virtual void RemoveItem(int index)
    {
      throw new NotSupportedException();
    }

    protected virtual void ClearItems()
    {
      throw new NotSupportedException();
    }

    protected virtual bool IsReadOnlyCore
    {
      get { return true; }
    }

    protected virtual object SyncRootCore
    {
      get { return null; }
    }

    protected void RefreshCount()
    {
      using (m_lock.GetWriteLock())
      {
        m_count = this.GetItemCount();
      }
    }

    #endregion

    #region IList<TType> Members

    public int IndexOf(TType item)
    {
      using (m_lock.GetReadLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          if (Equals(this.GetItem(i), item))
            return i;
        }

        return -1;
      }
    }

    public void Insert(int index, TType item)
    {
      using (m_lock.GetWriteLock())
      {
        this.InsertItem(index, item);

        m_count = this.GetItemCount();
      }
    }

    public void RemoveAt(int index)
    {
      using (m_lock.GetWriteLock())
      {
        this.RemoveItem(index);

        m_count = this.GetItemCount();
      }
    }

    public TType this[int index]
    {
      get
      {
        return this.GetItem(index);
      }
      set
      {
        using (m_lock.GetWriteLock())
        {
          this.SetItem(index, value);

          m_count = this.GetItemCount();
        }
      }
    }

    #endregion

    #region ICollection<TType> Members

    public void Add(TType item)
    {
      using (m_lock.GetWriteLock())
      {
        this.InsertItem(this.Count, item);
      }
    }

    public void Clear()
    {
      using (m_lock.GetWriteLock())
      {
        this.ClearItems();

        m_count = 0;
      }
    }

    public bool Contains(TType item)
    {
      using (m_lock.GetReadLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          if (Equals(this.GetItem(i), item))
            return true;
        }

        return false;
      }
    }

    public void CopyTo(TType[] array, int arrayIndex)
    {
      using (m_lock.GetReadLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          if (i >= array.Length)
            break;

          array[i + arrayIndex] = this.GetItem(i);
        }
      }
    }

    public int Count
    {
      get
      {
        if (m_count == -1)
          m_count = this.GetItemCount();

        return m_count;
      }
    }

    public bool IsReadOnly
    {
      get { return this.IsReadOnlyCore; }
    }

    public bool Remove(TType item)
    {
      using (m_lock.GetWriteLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          if (Equals(this.GetItem(i), item))
          {
            this.RemoveItem(i);
            m_count = this.GetItemCount();
            return true;
          }
        }

        return false;
      }
    }

    #endregion

    #region IEnumerable<TType> Members

    public IEnumerator<TType> GetEnumerator()
    {
      using (m_lock.GetReadLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          yield return this.GetItem(i);
        }
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion

    #region IList Members

    int IList.Add(object value)
    {
      using (m_lock.GetWriteLock())
      {
        this.Add((TType)value);

        return m_count;
      }
    }

    bool IList.Contains(object value)
    {
      return this.Contains((TType)value);
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
      get { return this.IsReadOnlyCore; }
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

    void ICollection.CopyTo([NotNull] Array array, int index)
    {
	    if (array == null) throw new ArgumentNullException("array");

			var element_type = array.GetType().GetElementType();

      if (!element_type.IsAssignableFrom(typeof(TType)))
        throw new InvalidCastException();

      using (m_lock.GetReadLock())
      {
        for (int i = 0; i < this.Count; i++)
        {
          if (i >= array.Length)
            break;

          array.SetValue(this.GetItem(i), i + index);
        }
      }
    }

    bool ICollection.IsSynchronized
    {
      get { return this.SyncRootCore != null; }
    }

    object ICollection.SyncRoot
    {
      get { return this.SyncRootCore; }
    }

    #endregion
  }
}
