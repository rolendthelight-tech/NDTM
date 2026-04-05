using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Extensions;
using Toolbox.Threading;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Объявляет операции, требуемые для работы службы событий
  /// </summary>
  /// <typeparam name="TListener">Интерфейс подписчика</typeparam>
  public interface IEventDispatcher<TListener>
    where TListener : class
  {
    /// <summary>
    /// Выполняет указанное действие над всеми подписчиками
    /// </summary>
    /// <param name="sender">Издатель события</param>
    /// <param name="handler">Действие, которое требуется выполнить</param>
    void Invoke(object sender, Action<TListener> handler);

    /// <summary>
    /// Вызывает указанную функцию у всех подписчиков
    /// </summary>
    /// <typeparam name="TResult">Тип возвращаемого значения</typeparam>
    /// <param name="sender">Издатель события</param>
    /// <param name="handler">Вызываемая функция</param>
    /// <returns>Если подписчик один, то результат вызова этой функции у него.
    /// Иначе, значение по умолчанию для типа возвращаемого значения</returns>
    TResult Invoke<TResult>(object sender, Func<TListener, TResult> handler);

    /// <summary>
    /// Вызывает указанную функцию у всех подписчиков
    /// </summary>
    /// <typeparam name="TResult">Тип возвращаемого значения</typeparam>
    /// <param name="sender">Издатель события</param>
    /// <param name="handler">Вызываемая функция</param>
    /// <param name="defaultValue">Значение по умолчанию, возвращаемое, если поддисчик не один</param>
    /// <returns>Если подписчик один, то результат вызова этой функции у него.
    /// Иначе, переданное значение по умолчанию</returns>
    TResult Invoke<TResult>(object sender, Func<TListener, TResult> handler, TResult defaultValue);

    /// <summary>
    /// Добавление подписчика к списку
    /// </summary>
    /// <param name="listener">Подписчик, который требуется добавить</param>
    void AddListener(TListener listener);

    /// <summary>
    /// Удаление подписчика из списка
    /// </summary>
    /// <param name="listener">Подписчик, который требуется удалить</param>
    void RemoveListener(TListener listener);

    /// <summary>
    /// Количество подписчиков
    /// </summary>
    int Count { get; }
  }

  public interface IEventFactory
  {
    /// <summary>
    /// Возвращает диспетчер для указанного набора событий
    /// </summary>
    /// <typeparam name="TListener">Интерфейс, задающий набор событий</typeparam>
    /// <returns>Единственный диспетчер для каждого типа подписчика</returns>
    IEventDispatcher<TListener> GetDispatcher<TListener>()
      where TListener : class;
  }

  internal sealed class EventDispatcher<TListener> : IEventDispatcher<TListener>
    where TListener : class
  {
    private readonly HashSet<TListener> m_listeners = new HashSet<TListener>();
    private readonly LockSource m_lock = new LockSource();

    #region IEventDispatcher<TListener> Members

    public void Invoke(object sender, Action<TListener> handler)
    {
      if (handler == null)
				throw new ArgumentNullException("handler");

      using (m_lock.GetReadLock())
      {
        foreach (var listener in m_listeners)
        {
          if (ReferenceEquals(listener, sender))
            continue;

          handler(listener);
        }
      }
    }

    public TResult Invoke<TResult>(object sender, Func<TListener, TResult> handler, TResult defaultValue)
    {
      if (handler == null)
				throw new ArgumentNullException("handler");

      using (m_lock.GetReadLock())
      {
        var results = new HashSet<TResult>();

        foreach (var listener in m_listeners)
        {
          if (ReferenceEquals(listener, sender))
            continue;

          var current_res = handler(listener);

          if (m_listeners.Count == 1
            || !SystemExtensions.AreEqual(defaultValue, current_res, false))
          {
            results.Add(current_res);
          }
        }

        return results.Count == 1 ? results.Single() : defaultValue;
      }
    }

    public TType Invoke<TType>(object sender, Func<TListener, TType> handler)
    {
      return this.Invoke(sender, handler, default(TType));
    }

    public void AddListener(TListener listener)
    {
      if (listener == null)
        throw new ArgumentNullException("listener");

      using (m_lock.GetWriteLock())
      {
        m_listeners.Add(listener);
      }
    }

    public void RemoveListener(TListener listener)
    {
      if (listener == null)
        return;

      using (m_lock.GetWriteLock())
      {
        m_listeners.Remove(listener);
      }
    }

    public int Count
    {
      get { return m_listeners.Count; }
    }

    #endregion
  }

  public sealed class EventFactory : IEventFactory
  {
    private readonly Dictionary<Type, object> m_dispatchers = new Dictionary<Type, object>();
    private readonly LockSource m_lock = new LockSource();

    #region IEventFactory Members

    public IEventDispatcher<TListener> GetDispatcher<TListener>() where TListener : class
    {
      object ret = null;
      using (m_lock.GetReadLock())
      {
        if (m_dispatchers.TryGetValue(typeof(TListener), out ret)
          && ret is IEventDispatcher<TListener>)
          return (IEventDispatcher<TListener>)ret;
      }

      using (m_lock.GetWriteLock())
      {
        if (m_dispatchers.TryGetValue(typeof(TListener), out ret)
         && ret is IEventDispatcher<TListener>)
          return (IEventDispatcher<TListener>)ret;

        var dispatcher = new EventDispatcher<TListener>();
        m_dispatchers.Add(typeof(TListener), dispatcher);
        return dispatcher;
      }
    }

    #endregion
  }
}
