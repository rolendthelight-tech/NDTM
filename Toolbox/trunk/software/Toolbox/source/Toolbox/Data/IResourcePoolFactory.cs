using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Tasks;

namespace Toolbox.Data
{
	/// <summary>
	/// Создатель пула ресурсов.
	/// </summary>
	/// <typeparam name="TResource">Тип ресурса.</typeparam>
	public interface IResourcePoolFactory<TResource>
		where TResource : class, IDisposable
	{
		/// <summary>
		/// Создаёт пул ресурсов.
		/// </summary>
		/// <param name="resourceFactory">Метод создания ресурса.</param>
		/// <param name="maxResourceCount">Вместимость пула.</param>
		/// <param name="readyResourceCount">Диапазон количества выделенных ресурсов, готовых к получению.</param>
		/// <param name="parallelOptions">Настройки параллелизма выделения и освобождения ресурсов.</param>
		/// <returns>Пул ресурсов.</returns>
		/// <exception cref="T:System.ArgumentNullException">Если какой-либо параметр <code>null</code>.</exception>
		/// <exception cref="T:System.OperationCanceledException">Если операция отменена. (Отмена через <paramref name="parallelOptions"/>.)</exception>
		[NotNull]
		IResourcePool<TResource> CreatePool([NotNull] Func<CancellationToken, TResource> resourceFactory, int? maxResourceCount, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount, [NotNull] ReadonlyParallelOptions parallelOptions);
	}
}
