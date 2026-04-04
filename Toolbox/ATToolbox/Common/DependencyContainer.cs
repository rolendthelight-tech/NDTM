using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox
{
  /// <summary>
  /// Контейнер компонентов
  /// </summary>
  public class DependencyContainer : IServiceProvider
  {
    private readonly Dictionary<Type, Func<object>> m_creators = new Dictionary<Type, Func<object>>();
    private readonly LockSource m_lock = new LockSource();
    private volatile bool m_search_descendants = true;

    /// <summary>
    /// Следует ли при поиске компонента проверять его подтипы
    /// </summary>
    public bool SearchDescendants
    {
      get { return m_search_descendants; }
      set
      {
        using (m_lock.GetWriteLock())
        {
          m_search_descendants = value;
        }
      }
    }

    #region IServiceProvider Members

    /// <summary>
    /// Возвращает экземпляр компонента по типу
    /// </summary>
    /// <param name="serviceType">Тип компонента</param>
    /// <returns>Экземпляр компонента запрошенного типа, если компонент зарегистрирован. 
    /// Иначе, возвращает null</returns>
    public object GetService(Type serviceType)
    {
      if (serviceType == null)
        throw new ArgumentNullException("serviceType");

      return (this.GetCreator(serviceType) ?? (() => null)).Invoke();
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <typeparam name="TService">Тип компонента</typeparam>
    /// <param name="instance">Экземпляр компонента</param>
    public void SetService<TService>(TService instance)
    {
      this.SetService(typeof(TService), instance);
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <typeparam name="TService">Тип компонента</typeparam>
    /// <param name="creator">Метод, порождающий компонент</param>
    public void SetService<TService>(Func<TService> creator)
    {
      this.SetService(typeof(TService), creator == null ?
        (Func<object>)null : (() => creator()) );
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <param name="type">Тип компонента</param>
    /// <param name="instance">Экземпляр компонента</param>
    public void SetService(Type type, object instance)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      using (m_lock.GetWriteLock())
      {
        if (instance == null)
          m_creators.Remove(type);
        else
        {
          if (!type.IsAssignableFrom(instance.GetType()))
            throw new ArgumentException();

          m_creators[type] = () => instance;
        }
      }
    }

    /// <summary>
    /// Регистрация компонента
    /// </summary>
    /// <param name="type">Тип компонента</param>
    /// <param name="creator">Метод, порождающий компонент</param>
    public void SetService(Type type, Func<object> creator)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      using (m_lock.GetWriteLock())
      {
        if (creator == null)
          m_creators.Remove(type);
        else
          m_creators[type] = creator;
      }
    }

    #endregion

    private Func<object> GetCreator(Type serviceType)
    {
      using (m_lock.GetReadLock())
      {
        Func<object> creator;

        if (!m_creators.TryGetValue(serviceType, out creator)
          && m_search_descendants)
        {
          foreach (var kv in m_creators)
          {
            if (serviceType.IsAssignableFrom(kv.Key))
            {
              if (creator == null)
                creator = kv.Value;
              else
              {
                creator = null;
                break;
              }
            }
          }
        }

        return creator;
      }
    }
  }
}
