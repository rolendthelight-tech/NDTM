using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox
{
  public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
  {
    private readonly IDictionary<TKey, TValue> dictionary;

    [NonSerialized]
    private Object _syncRoot;
    
    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");

      this.dictionary = dictionary;
    }

    void IDictionary.Clear()
    {
      throw new NotSupportedException("Очистка не поддерживается");
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return ((IDictionary)dictionary).GetEnumerator();
    }

    public void Remove(object key)
    {
      throw new NotSupportedException("Удаление не поддерживается");
    }

    object IDictionary.this[object key]
    {
      get { return ((IDictionary)this.dictionary)[key]; }
      set { throw new NotSupportedException("Установка значения не поддерживается"); }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return this.dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)dictionary).GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException("Добавление не поддерживается");
    }

    public bool Contains(object key)
    {
      return ((IDictionary)this.dictionary).Contains(key);
    }

    public void Add(object key, object value)
    {
      throw new NotSupportedException("Добавление не поддерживается");
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new NotSupportedException("Очистка не поддерживается");
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return this.dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.Rank != 1)
      {
        throw new ArgumentException("Многомерные массивы не поддерживаются");
      }

      if (array.GetLowerBound(0) != 0)
      {
        throw new ArgumentException("Нижняя граница не равна нулю");
      }

      if (arrayIndex < 0)
      {
        throw new ArgumentOutOfRangeException("arrayIndex", @"Требуется неотрицательное число");
      }

      if (array.Length - arrayIndex < this.Count)
      {
        throw new ArgumentException("Остаток массива слишком мал");
      }

      this.dictionary.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException("Удаление не поддерживается");
    }

    public void CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.Rank != 1)
      {
        throw new ArgumentException("Многомерные массивы не поддерживаются");
      }

      if (array.GetLowerBound(0) != 0)
      {
        throw new ArgumentException("Нижняя граница не равна нулю");
      }

      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("index", @"Требуется неотрицательное число");
      }

      if (array.Length - index < this.Count)
      {
        throw new ArgumentException("Остаток массива слишком мал");
      }

      KeyValuePair<TKey, TValue>[] items = array as KeyValuePair<TKey, TValue>[];
            if (items != null)
            {
              this.dictionary.CopyTo(items, index);
            }
            else
            {
              // 
              // Catch the obvious case assignment will fail.
              // We can found all possible problems by doing the check though.
              // For example, if the element type of the Array is derived from T,
              // we can't figure out if we can successfully copy the element beforehand. 
              //
              Type targetType = array.GetType().GetElementType();
              Type sourceType = typeof(KeyValuePair<TKey, TValue>);
              if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
              {
                throw new ArgumentException("Неверный тип массива");
              }

              //
              // We can't cast array of value type to object[], so we don't support 
              // widening of primitive types here.
              // 
              object[] objects = array as object[];
              if (objects == null)
              {
                throw new ArgumentException("Неверный тип массива");
              }

              try
              {
                foreach (var kvp in this.dictionary)
                {
                  objects[index++] = kvp;
                }
              }
              catch (ArrayTypeMismatchException)
              {
                throw new ArgumentException("Неверный тип массива");
              }
            }
    }

    public int Count
    {
      get { return this.dictionary.Count; }
    }

    public object SyncRoot
    {
      get
      {
        if (_syncRoot == null)
        {
          ICollection c = this.dictionary as ICollection;
          if (c != null)
          {
            _syncRoot = c.SyncRoot;
          }
          else
          {
            System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
          }
        }
        return _syncRoot;
      }
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    ICollection IDictionary.Values
    {
      get { return ((IDictionary)this.dictionary).Values; }
    }

    bool IDictionary.IsReadOnly
    {
      get { return true; }
    }

    public bool IsFixedSize
    {
      get { return true; }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get { return true; }
    }

    public bool ContainsKey(TKey key)
    {
      return this.dictionary.ContainsKey(key);
    }

    public void Add(TKey key, TValue value)
    {
      throw new NotSupportedException("Добавление не поддерживается");
    }

    public bool Remove(TKey key)
    {
      throw new NotSupportedException("Удаление не поддерживается");
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return this.dictionary.TryGetValue(key, out value);
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      get { return this.dictionary[key]; }
      set { throw new NotSupportedException("Установка значения не поддерживается"); }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get { return this.dictionary.Keys; }
    }

    ICollection IDictionary.Keys
    {
      get { return ((IDictionary)this.dictionary).Keys; }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get { return this.dictionary.Values; }
    }
  }
}
