using System;
using System.ComponentModel;
using System.Collections;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Набор таблиц модуля, который можно положить на форму
  /// </summary>
  [Serializable]
  [DefaultEvent("ErrorMessage")]
  public abstract class DbContext : DbContextBase, IListSource
  {
    private DbContextListSource listSource;

    protected DbContext()
    {
      RecordManager.Contexts.Add(this);

      if (RecordManager.Contexts.Count == 1)
      {
        RecordCache.UpdateReferences(this.GetType());
      }

      listSource = new DbContextListSource(this);
      this.ExactBinding = true;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      RecordManager.Contexts.Remove(this);
    }

    #region IListSource Members

    public bool ContainsListCollection
    {
      get { return true; }
    }

    public System.Collections.IList GetList()
    {
      return listSource;
    }

    #endregion

    #region Descriptor

    /// <summary>
    /// Вспомогательный класс для лучшей интеграции с дизайнером.
    /// Позволяет представить контекст как список из одного элемента
    /// </summary>
    private class DbContextListSource : IList
    {
      DbContext context;

      public DbContextListSource(DbContext context)
      {
        this.context = context;
      }

      #region IList Members

      int IList.Add(object value)
      {
        throw new NotSupportedException();
      }

      void IList.Clear()
      {
        throw new NotSupportedException();
      }

      bool IList.Contains(object value)
      {
        return value == this.context;
      }

      int IList.IndexOf(object value)
      {
        if (value == this.context)
        {
          return 1;
        }
        return -1;
      }

      void IList.Insert(int index, object value)
      {
        throw new NotSupportedException();
      }

      bool IList.IsFixedSize
      {
        get { return true; }
      }

      bool IList.IsReadOnly
      {
        get { return true; }
      }

      void IList.Remove(object value)
      {
        throw new NotSupportedException();
      }

      void IList.RemoveAt(int index)
      {
        throw new NotSupportedException();
      }

      object IList.this[int index]
      {
        get
        {
          if (index == 0)
          {
            return context;
          }
          return null;
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      #endregion

      #region ICollection Members

      void ICollection.CopyTo(Array array, int index)
      {
        array.SetValue(context, index);
      }

      int ICollection.Count
      {
        get { return 1; }
      }

      bool ICollection.IsSynchronized
      {
        get { return false; }
      }

      object ICollection.SyncRoot
      {
        get { return this; }
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
        yield return context;
      }

      #endregion
    }

    #endregion
  }
}
