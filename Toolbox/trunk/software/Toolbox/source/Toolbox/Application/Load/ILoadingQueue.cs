using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Toolbox.Application.Load
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
    [NotNull]
    IApplicationLoader[] GetLoaders();
  }

  /// <summary>
  /// Расширяемый инициализатор очереди загрузки
  /// </summary>
  public class LoadingQueue : ILoadingQueue
  {
    #region ILoadingQueue Members

	  [NotNull]
	  public IApplicationLoader[] GetLoaders()
    {
      var list = new ApplicationLoaderList();

      this.FillLoaders(list.Add, list.Contains);

      if (this.AppInstanceRequired && !list.Contains(typeof(IAppInstanceInitializer)))
        list.Add(this.GetAppInstanceInitializer());

      return list.ToArray();
    }

    protected virtual void FillLoaders([NotNull] Action<IApplicationLoader> add, [NotNull] Func<Type, bool> contains)
    {
	    if (add == null) throw new ArgumentNullException("add");
	    if (contains == null) throw new ArgumentNullException("contains");
    }

	  [NotNull]
	  protected virtual AppInstanceInitializer GetAppInstanceInitializer()
    {
      return new AppInstanceInitializer();
    }

    protected virtual bool AppInstanceRequired
    {
      get { return true; }
    }

    #endregion

    private class ApplicationLoaderList : KeyedCollection<Type, IApplicationLoader>
    {
      protected override Type GetKeyForItem([NotNull] IApplicationLoader item)
      {
	      if (item == null) throw new ArgumentNullException("item");

	      return item.Key;
      }
    }
  }

  /// <summary>
  /// Инициализатор очереди загрузки, комбинирующий другие инициализаторы
  /// </summary>
  public sealed class LoadingQueueComposite : ILoadingQueue
  {
	  [NotNull] private readonly HashSet<ILoadingQueue> m_queues;

    public LoadingQueueComposite([NotNull] IEnumerable<ILoadingQueue> queues)
    {
      if (queues == null)
        throw new ArgumentNullException("queues");

      m_queues = new HashSet<ILoadingQueue>(queues.Where(q => q != null));
    }

    #region ILoadingQueue Members

	  [NotNull]
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
