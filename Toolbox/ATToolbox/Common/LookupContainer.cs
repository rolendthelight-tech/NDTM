using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using log4net;
using AT.Toolbox.Properties;

namespace AT.Toolbox
{
  /// <summary>
  /// Статический контейнер для реализации паттерна Dependency Lookup
  /// </summary>
  public static class LookupContainer
  {
    private static readonly Dictionary<Type, ILookupInfo> _lookups = new Dictionary<Type, ILookupInfo>();
    private static readonly LockSource _lock = new LockSource();
    private static readonly ILog _log = LogManager.GetLogger("LookupContainer");

    /// <summary>
    /// Показывает, нужно ли бросать исключение, если тип не настроен
    /// </summary>
    public static bool ThrowIfNotSet { get; set; }

    /// <summary>
    /// Показывает, нужно ли искать наследуемые типы
    /// </summary>
    public static bool SearchDescendants { get; set; }

    /// <summary>
    /// Настраивает тип для создания экземпляра
    /// </summary>
    /// <typeparam name="T">Настраиваемый тип</typeparam>
    /// <param name="creator">Метод, вызываемый при создании объекта</param>
    /// <param name="singleton">Показывает, является ли объект Singleton</param>
    /// <returns>True, если настройка была добавлена; Fase, если настройка была заменена</returns>
    public static bool Setup<T>(Func<T> creator, bool singleton)
      where T : class
    {
      LookupInfo<T> lookup = new LookupInfo<T>(creator, singleton);

      using (_lock.GetWriteLock())
      {
        bool ret = !_lookups.ContainsKey(typeof(T));
        _lookups[typeof(T)] = lookup;
        return ret;
      }
    }

    /// <summary>
    /// Настраивает тип для создания экземпляра
    /// </summary>
    /// <typeparam name="T">Настраиваемый тип</typeparam>
    /// <param name="creator">Метод, вызываемый при создании объекта</param>
    /// <param name="singleton">Показывает, является ли объект Singleton</param>
    /// <returns>True, если настройка была добавлена; Fase, если настройка была заменена</returns>
    public static bool Setup<T>(Func<object[], T> creator, bool singleton)
      where T : class
    {
      LookupInfo<T> lookup = new LookupInfo<T>(creator, singleton);

      using (_lock.GetWriteLock())
      {
        bool ret = !_lookups.ContainsKey(typeof(T));
        _lookups[typeof(T)] = lookup;
        return ret;
      }
    }

    /// <summary>
    /// Настраивает множество типов
    /// </summary>
    /// <param name="info">Список типов и порождающих их делегатов</param>
    /// <param name="singletons">Типы, которые должны порождаться как Singleton</param>
    public static void SetupMultiple(Dictionary<Type, Delegate> info, HashSet<Type> singletons)
    {
      using (_lock.GetWriteLock())
      {
        List<Type> added = new List<Type>();

        try
        {
          foreach (var kv in info)
          {
            Type lookupType = typeof(LookupInfo<>).MakeGenericType(kv.Key);

            bool is_new = !_lookups.ContainsKey(kv.Key);

            _lookups[kv.Key] = (ILookupInfo)Activator.CreateInstance(lookupType,
              kv.Value, singletons.Contains(kv.Key));

            if (is_new)
              added.Add(kv.Key);
          }
        }
        catch (Exception ex)
        {
          foreach (var type in added)
            _lookups.Remove(type);

          throw ex;
        }
      }
    }

    /// <summary>
    /// Иницализирует новый экземпляр настроенного типа
    /// </summary>
    /// <typeparam name="T">Тип, экземпляр которого нужно вернуть</typeparam>
    /// <param name="parameters">Параметры, передаваемые методу создания</param>
    /// <returns>Экземпляр типа</returns>
    public static T GetInstance<T>(params object[] parameters)
       where T : class
    {
      using (_lock.GetReadLock())
      {
        ILookupInfo li;
        if (!_lookups.TryGetValue(typeof(T), out li))
        {
          if (SearchDescendants)
          {
            List<ILookupInfo> found = new List<ILookupInfo>();

            foreach (var kv in _lookups)
            {
              if (typeof(T).IsAssignableFrom(kv.Key))
                found.Add(kv.Value);
            }

            if (found.Count == 1)
              return (T)found[0].GetInstance(new object[0]);
          }

          if (ThrowIfNotSet)
            throw new DependenceNotInitializedException(string.Format(Resources.LOOKUP_NOT_SET, typeof(T)));
          else
          {
            try
            {
              return (T)Activator.CreateInstance(typeof(T), parameters);
            }
            catch (Exception ex)
            {
              throw new DependenceNotInitializedException(string.Format(Resources.LOOKUP_NOT_SET, typeof(T)), ex);
            }
          }
        }
        else
          return (T)li.GetInstance(parameters);
      }
    }

    /// <summary>
    /// Иницализирует новый экземпляр настроенного типа без параметров
    /// </summary>
    /// <typeparam name="T">Тип, экземпляр которого нужно вернуть</typeparam>
    /// <returns>Экземпляр типа</returns>
    public static T GetInstance<T>()
      where T : class
    {
      return GetInstance<T>(new object[0]);
    }

    public static bool TryGetInstance<T>(out T result)
       where T : class
    {
      using (_lock.GetReadLock())
      {
        result = null;

        ILookupInfo creator;

        if (_lookups.TryGetValue(typeof(T), out creator))
        {
          try
          {
            result = creator.GetInstance(new object[0]) as T;
          }
          catch (Exception ex)
          {
            _log.Error("TryGetInstance(): exception", ex);
          }

          if (result != null)
            return true;
        }
        else if (SearchDescendants)
        {
          List<ILookupInfo> found = new List<ILookupInfo>();
          foreach (var kv in _lookups)
          {
            if (typeof(T).IsAssignableFrom(kv.Key))
              found.Add(kv.Value);
          }
          if (found.Count == 1)
          {
            try
            {
              result = found[0].GetInstance(new object[0]) as T;
            }
            catch (Exception ex)
            {
              _log.Error("TryGetInstance(): exception", ex);
            }

            if (result != null)
              return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Проверяет, установлен ли тип
    /// </summary>
    /// <typeparam name="T">Проверяемый тип</typeparam>
    /// <returns>True, если тип</returns>
    public static bool CanInstantiate<T>()
    {
      using (_lock.GetReadLock())
      {
        bool ret = _lookups.ContainsKey(typeof(T));
        if (!ret && SearchDescendants)
        {
          List<Type> found = new List<Type>();
          foreach (var kv in _lookups)
          {
            if (typeof(T).IsAssignableFrom(kv.Key))
              found.Add(kv.Key);
          }
          return found.Count == 1;
        }
        return ret;
      }
    }

    #region Nested types

    private interface ILookupInfo
    {
      object GetInstance(object[] parameters);
    }

    /// <summary>
    /// Настроечный объект для LookupContainer
    /// </summary>
    /// <typeparam name="T">Метод создания нового объекта</typeparam>
    private class LookupInfo<T> : ILookupInfo
      where T : class
    {
      private T m_instance;
      private bool m_once_called;
      private readonly Func<object[], T> m_creator;
      private readonly object m_lock = new object();
      private readonly bool m_parametrized;

      /// <summary>
      /// Инициализирует новый экземпляр настроечного объекта
      /// </summary>
      /// <param name="creator">Метод создания объекта</param>
      /// <param name="singleton">Показывает, что объект создаётся только при первом обращении</param>
      public LookupInfo(Func<T> creator, bool singleton)
      {
        if (creator == null)
          throw new ArgumentNullException("creator");

        m_creator = (parms) => creator();
        this.Singleton = singleton;
        m_parametrized = false;
      }

      public LookupInfo(Func<object[], T> creator, bool singleton)
      {
        if (creator == null)
          throw new ArgumentNullException("creator");

        m_creator = creator;
        this.Singleton = singleton;
        m_parametrized = true;
      }

      /// <summary>
      /// Показывает, что объект создаётся только при первом обращении
      /// </summary>
      public bool Singleton { get; set; }

      internal T GetInstance(object[] parameters)
      {
        if (!m_parametrized && parameters != null && parameters.Length > 0)
          throw new ArgumentException(string.Format(Resources.LOOKUP_NO_PARAMS, typeof(T)), "parameters");

        if (this.Singleton)
        {
          lock (m_lock)
          {
            if (!m_once_called)
            {
              m_instance = m_creator(parameters);
              m_once_called = true;
            }
          }

          return m_instance;
        }
        else
          return m_creator(parameters);
      }

      #region ILookupInfo Members

      object ILookupInfo.GetInstance(object[] parameters)
      {
        return this.GetInstance(parameters);
      }

      #endregion
    }

    #endregion
  }

  [Serializable]
  public class DependenceNotInitializedException : InvalidOperationException
  {
    public DependenceNotInitializedException() : base() { }

    public DependenceNotInitializedException(string message) : base(message) { }

    public DependenceNotInitializedException(string message, Exception innerException)
      : base(message, innerException) { }

    protected DependenceNotInitializedException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
  }
}
