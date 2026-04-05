using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AT.Toolbox
{
  /// <summary>
  /// Инициализатор очереди загрузки 
  /// </summary>
  public interface ILoadingQueue
  {
    /// <summary>
    /// Получение загрузчиков компонентов, которые требуется загрузить
    /// </summary>
    /// <returns>Массив загрузчиков компонентов с уникальным типом</returns>
    IApplicationLoader[] GetLoaders();
  }

  /// <summary>
  /// Расширяемый инициализатор очереди загрузки
  /// </summary>
  public class LoadingQueue : ILoadingQueue
  {
    #region ILoadingQueue Members

    public IApplicationLoader[] GetLoaders()
    {
      var list = new ApplicationLoaderList();

      this.FillLoaders(list.Add, list.Contains);

      return list.ToArray();
    }

    protected virtual void FillLoaders(Action<IApplicationLoader> add, Func<Type, bool> contains)
    {
    }

    #endregion

    private class ApplicationLoaderList : KeyedCollection<Type, IApplicationLoader>
    {
      protected override Type GetKeyForItem(IApplicationLoader item)
      {
        return item.Key;
      }
    }
  }

  /// <summary>
  /// Инициализатор очереди загрузки, комбинирующий другие инициализаторы
  /// </summary>
  public sealed class LoadingQueueComposite : ILoadingQueue
  {
    private readonly HashSet<ILoadingQueue> m_queues;

    public LoadingQueueComposite(IEnumerable<ILoadingQueue> queues)
    {
      if (queues == null)
        throw new ArgumentNullException("queues");

      m_queues = new HashSet<ILoadingQueue>(queues.Where(q => q != null));
    }
    
    #region ILoadingQueue Members

    public IApplicationLoader[] GetLoaders()
    {
      var keys = new HashSet<Type>();
      var loaders = new List<IApplicationLoader>();

      foreach (var queue in m_queues)
      {
        foreach (var loader in queue.GetLoaders())
        {
          if (keys.Add(loader.Key))
            loaders.Add(loader);
        }
      }

      return loaders.ToArray();
    }

    #endregion
  }

}
