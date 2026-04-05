using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности коллекций.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Добавляет элементы указанной коллекции в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <typeparam name="T">Тип значения.</typeparam>
		/// <param name="collection">Коллекция, в которую добавляются элементы.</param>
		/// <param name="range">Коллекция, элементы которой добавляются в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>. Коллекция не может быть задана значением null, но может содержать элементы null, если тип <typeparamref name="T"/> является ссылочным типом.</param>
		public static void AddRange<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> range)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			if (range == null) throw new ArgumentNullException("range");

			foreach (var item in range)
			{
				collection.Add(item);
			}
		}

		/// <summary>
		/// Удаляет указанный элемент из коллекции.
		/// </summary>
		/// <typeparam name="T">Тип элементов в коллекции.</typeparam>
		/// <param name="set">Коллекция.</param>
		/// <param name="item">Подлежащий удалению элемент.</param>
		public static void CheckedRemove<T>([NotNull] this ICollection<T> set, T item)
		{
			if (set == null) throw new ArgumentNullException("set");
			if (!set.Remove(item))
				throw new ArgumentException("Такого элемента не существует в коллекции", "item");
		}

    /// <summary>
    /// Преобразовать все элементы коллекции с помощью функции в многопоточном режиме. Порядок элементов сохраняется. 
    /// Выбор элемента для преобразования осуществляется после каждого преобразования в отличии от Parallel.ForEach.
    /// </summary>
    /// <typeparam name="T">Тип элемента входной коллекции</typeparam>
    /// <typeparam name="R">Тип элемента выходной коллекции</typeparam>
    /// <param name="collection">Входная коллекция</param>
    /// <param name="function">Функция преобразования элементов R=function(T).</param>
    /// <param name="parallelOptions">Параметры распараллеливания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="itemProcessed">Делегат для оповещения обработки элемента</param>
    /// <returns>Выходная коллекция</returns>
    public static ICollection<R> ParallelForEach<T, R>([NotNull] this ICollection<T> collection,
      [NotNull] Func<T, R> function, ParallelOptions parallelOptions = null,
      CancellationToken cancellationToken = new CancellationToken(), EventHandler itemProcessed = null)
    {
      if (collection == null) throw new ArgumentNullException("collection");

      if (collection.Count == 0)
        return new Collection<R>();

      var threadCount = parallelOptions != null ? parallelOptions.MaxDegreeOfParallelism : Environment.ProcessorCount;
      threadCount = Math.Min(threadCount, collection.Count);

      var tasks = new Task[threadCount];

      var results = new R[collection.Count];
      var index = 0;

      foreach (var item in collection)
      {
        var thread = index >= tasks.Length ? Task.WaitAny(tasks, cancellationToken) : index;

        var data = new Tuple<T, int>(item, index);

        tasks[thread] = Task.Factory.StartNew(() =>
        {
          results[data.Item2] = function(data.Item1);
          if (itemProcessed != null)
            itemProcessed(collection, new EventArgs());
        }, cancellationToken);

        index++;
      }
      Task.WaitAll(tasks, cancellationToken);

      return results;
    }
	}
}
