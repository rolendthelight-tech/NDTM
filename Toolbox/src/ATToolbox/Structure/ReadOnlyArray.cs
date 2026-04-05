using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AT.Toolbox
{
  /// <summary>
  /// Обёртка над массивом только для чтения
  /// </summary>
  /// <typeparam name="T">Тип элемента массива</typeparam>
  [Serializable]
  public sealed class ReadOnlyArray<T> : IEnumerable<T>, ICloneable
  {
    private readonly T[] m_array;

    private ReadOnlyArray(T[] sourceArray, bool copy)
    {
      if (sourceArray == null)
        throw new ArgumentNullException("sourceArray");

      if (copy)
      {
        m_array = new T[sourceArray.Length];
        Array.Copy(sourceArray, m_array, m_array.Length);
      }
      else
        m_array = sourceArray;
    }

    public ReadOnlyArray(T[] sourceArray) : this(sourceArray, true) { }

    /// <summary>
    /// Количество элементов в массиве
    /// </summary>
    public int Count
    {
      get { return m_array.Length; }
    }

    /// <summary>
    /// Доступ к элементу массива по индексу
    /// </summary>
    /// <param name="index">Индекс в массиве</param>
    /// <returns>Значение элемента массива</returns>
    public T this[int index]
    {
      get { return m_array[index]; }
    }

    /// <summary>
    /// Вычисляет индекс первого вхождения элемента в массив
    /// </summary>
    /// <param name="item">Искомый элемент</param>
    /// <returns>Индекс элемента, если элемент найден. Иначе, -1</returns>
    public int IndexOf(T item)
    {
      for (int i = 0; i < m_array.Length; i++)
      {
        if (Equals(item, m_array[i]))
          return i;
      }

      return -1;
    }

    public override string ToString()
    {
      return string.Format("{0}[{1}]", typeof(T).FullName, m_array.Length);
    }

    /// <summary>
    /// Сравнение обёрток массивов на эквивалентность
    /// </summary>
    /// <param name="obj">Сравниваемый объект</param>
    /// <returns>True, если сравниваемый объект является объектом ReadOnlyArray
    /// с таким же Generic-параметром, у которого все элементы эквивалентны
    /// элементам текущего экземпляра</returns>
    public override bool Equals(object obj)
    {
      var roa = obj as ReadOnlyArray<T>;

      if (roa == null)
        return false;

      if (ReferenceEquals(m_array, roa.m_array))
        return true;

      if (m_array.Length != roa.m_array.Length)
        return false;

      for (int i = 0; i < m_array.Length; i++)
      {
        if (!Equals(m_array[i], roa.m_array[i]))
          return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      var ret = m_array.Length;

      for (int i = 0; i < m_array.Length; i++)
      {
        var value = m_array[i];

        if (value != null)
          ret ^= value.GetHashCode();
      }

      return ret;
    }

    public static implicit operator ReadOnlyArray<T>(T[] array)
    {
      if (array == null)
        return null;

      return new ReadOnlyArray<T>(array, false);
    }

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      IEnumerable<T> en = m_array;

      return en.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return m_array.GetEnumerator();
    }

    #endregion

    #region ICloneable Members

    public ReadOnlyArray<T> Clone()
    {
      return new ReadOnlyArray<T>(m_array, false);
    }

    object ICloneable.Clone()
    {
      return new ReadOnlyArray<T>(m_array, false);
    }

    #endregion
  }
}
