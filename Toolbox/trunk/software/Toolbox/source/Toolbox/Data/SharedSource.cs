using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Toolbox.Data
{
  /// <summary>
  /// Контейнер для элементов с раздельной блокировкой
  /// </summary>
  /// <typeparam name="TKey">Тип ключа элемента</typeparam>
  /// <typeparam name="TValue">Тип элемента</typeparam>
  public class SharedSource<TKey, TValue> : IEnumerable<TValue>
  {
    private readonly Dictionary<TKey, TValue> m_data = new Dictionary<TKey, TValue>();
    private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
    private readonly Func<TKey, TValue> m_creator;

    /// <summary>
    /// Инициализирует новый контейнер
    /// </summary>
    /// <param name="creator">Объект, создающий новый экземпляр по ключу</param>
    public SharedSource(Func<TKey, TValue> creator)
    {
      if (creator == null)
        throw new ArgumentNullException("creator");

      m_creator = creator;
    }

    /// <summary>
    /// Ищет объект в хранилище, если не найден, создаёт новый
    /// </summary>
    /// <param name="key">Значение ключа</param>
    /// <returns>Найденный или новый элемент</returns>
    public TValue this[TKey key]
    {
      get { return this.Lookup(key); }
    }

    /// <summary>
    /// Ищет объект в хранилище, если не найден, создаёт новый
    /// </summary>
    /// <param name="key">Значение ключа</param>
    /// <returns>Найденный или новый элемент</returns>
    public TValue Lookup(TKey key)
    {
      if (key == null)
        throw new ArgumentNullException("key");

      TValue value;

      m_lock.EnterReadLock();
      try
      {
        if (m_data.TryGetValue(key, out value))
          return value;
      }
      finally
      {
        m_lock.ExitReadLock();
      }

      m_lock.EnterWriteLock();
      try
      {
        if (!m_data.TryGetValue(key, out value))
        {
          value = m_creator(key);
          m_data.Add(key, value);
        }

        return value;
      }
      finally
      {
        m_lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Очищает хранилище
    /// </summary>
    public void Clear()
    {
      m_lock.EnterWriteLock();
      try
      {
        m_data.Clear();
      }
      finally
      {
        m_lock.ExitWriteLock();
      }
    }

    #region IEnumerable<TValue> Members

    public IEnumerator<TValue> GetEnumerator()
    {
      m_lock.EnterReadLock();
      try
      {
        return ((IEnumerable<TValue>)m_data.Values.ToArray()).GetEnumerator();
      }
      finally
      {
        m_lock.ExitReadLock();
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}
