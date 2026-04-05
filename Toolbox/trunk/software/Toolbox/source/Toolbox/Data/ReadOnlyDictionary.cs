using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Обёртка только для чтения над словарём.
	/// </summary>
	/// <typeparam name="TKey">Тип ключа.</typeparam>
	/// <typeparam name="TValue">Тип значения.</typeparam>
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
  {
	  [NotNull] private readonly IDictionary<TKey, TValue> dictionary;

	  [CanBeNull, NonSerialized] private Object _syncRoot;

		/// <summary>
		/// Создание обёртки только для чтения над словарём.
		/// </summary>
		/// <param name="dictionary">Словарь с данными.</param>
		[Pure]
    public ReadOnlyDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");

      this.dictionary = dictionary;
    }

    void IDictionary.Clear()
    {
      throw new NotSupportedException("Очистка не поддерживается");
    }

	  [NotNull]
	  IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return ((IDictionary)dictionary).GetEnumerator();
    }

    public void Remove([NotNull] object key)
    {
	    if (key == null) throw new ArgumentNullException("key");

			throw new NotSupportedException("Удаление не поддерживается");
    }

	  object IDictionary.this[[NotNull] object key]
    {
      get
      {
	      if (key == null) throw new ArgumentNullException("key");

				return ((IDictionary)this.dictionary)[key];
      }
	    set
	    {
		    if (key == null) throw new ArgumentNullException("key");

				throw new NotSupportedException("Установка значения не поддерживается");
	    }
    }

	  [NotNull]
	  IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return this.dictionary.GetEnumerator();
    }

	  [NotNull]
	  IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)dictionary).GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException("Добавление не поддерживается");
    }

    public bool Contains([NotNull] object key)
    {
	    if (key == null) throw new ArgumentNullException("key");

			return ((IDictionary)this.dictionary).Contains(key);
    }

	  public void Add([NotNull] object key, object value)
	  {
		  if (key == null) throw new ArgumentNullException("key");

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

    public void CopyTo([NotNull] KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.Rank != 1)
      {
				throw new ArgumentException("Многомерные массивы не поддерживаются", "array");
      }

      if (array.GetLowerBound(0) != 0)
      {
				throw new ArgumentException("Нижняя граница не равна нулю", "array");
      }

      if (arrayIndex < 0)
      {
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Требуется неотрицательное число");
      }

      if (array.Length - arrayIndex < this.Count)
      {
				throw new ArgumentException("Остаток массива слишком мал", "array");
      }

      this.dictionary.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException("Удаление не поддерживается");
    }

    public void CopyTo([NotNull] Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if (array.Rank != 1)
      {
				throw new ArgumentException("Многомерные массивы не поддерживаются", "array");
      }

      if (array.GetLowerBound(0) != 0)
      {
				throw new ArgumentException("Нижняя граница не равна нулю", "array");
      }

      if (index < 0)
      {
				throw new ArgumentOutOfRangeException("index", index, "Требуется неотрицательное число");
      }

      if (array.Length - index < this.Count)
      {
				throw new ArgumentException("Остаток массива слишком мал", "array");
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
								throw new ArgumentException("Неверный тип массива", "array");
              }

              //
              // We can't cast array of value type to object[], so we don't support 
              // widening of primitive types here.
              // 
              object[] objects = array as object[];
              if (objects == null)
              {
								throw new ArgumentException("Неверный тип массива", "array");
              }

              try
              {
                foreach (var kvp in this.dictionary)
                {
                  objects[index++] = kvp;
                }
              }
              catch (ArrayTypeMismatchException ex)
              {
								throw new ArgumentException("Неверный тип массива", "array", ex);
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

	  [NotNull]
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

	  [NotNull]
	  ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get { return this.dictionary.Keys; }
    }

	  [NotNull]
	  ICollection IDictionary.Keys
    {
      get { return ((IDictionary)this.dictionary).Keys; }
    }

	  [NotNull]
	  ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get { return this.dictionary.Values; }
    }

		#region Члены IReadOnlyDictionary<TKey,TValue>

		TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
		{
			get { return ((IDictionary<TKey, TValue>)this)[key]; }
		}

		[NotNull]
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
		{
			get { return ((IDictionary<TKey, TValue>)this).Keys; }
		}

		[NotNull]
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
		{
			get { return ((IDictionary<TKey, TValue>)this).Values; }
		}

		#endregion
	}
}
