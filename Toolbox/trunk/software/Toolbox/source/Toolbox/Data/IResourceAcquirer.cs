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
	/// Запрашиватель ресурсов из пула, предоставляющий ссылки на ресурс.
	/// Метод <see cref="IDisposable.Dispose()"/> завершает запрос ресурсов, аннулирует ссылки, возвращает ресурсы в пул, ожидает возвращения ресурсов в пул.
	/// </summary>
	/// <typeparam name="TResource">Тип ресурса.</typeparam>
	public interface IResourceAcquirer<out TResource> : IDisposable
	{
		/// <summary>
		/// Выделяет ресурс из пула.
		/// </summary>
		/// <param name="cancel">Токен отмены.</param>
		/// <returns>Ссылка на ресурс.</returns>
		/// <exception cref="T:System.OperationCanceledException">Если операция отменена.</exception>
		/// <exception cref="T:Toolbox.Data.ResourceAcquirerEndedException">Если запрос ресурсов прекращён.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Если запрашиватель освобождён.</exception>
		[NotNull]
		IResourceLink<TResource> AcquireLink(CancellationToken cancel);

		/// <summary>
		/// Создаёт пул ресурсов, создающихся на основе ресурсов этого пула.
		/// </summary>
		/// <typeparam name="TReferencedResource">Тип ресурса.</typeparam>
		/// <param name="resourceFactory">Метод создания ресурса.</param>
		/// <param name="maxResourceCount">Вместимость пула.</param>
		/// <param name="readyResourceCount">Диапазон количества выделенных ресурсов, готовых к получению.</param>
		/// <param name="parallelOptions">Настройки параллелизма выделения и освобождения ресурсов.</param>
		/// <returns>Пул ресурсов, создающихся из ресурсов из этого пула.</returns>
		/// <exception cref="T:System.ArgumentNullException">Если какой-либо параметр <code>null</code>.</exception>
		/// <exception cref="T:System.OperationCanceledException">Если операция отменена. (Отмена через <paramref name="parallelOptions"/>.)</exception>
		/// <exception cref="T:Toolbox.Data.ResourceAcquirerEndedException">Если запрос ресурсов прекращён.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Если запрашиватель освобождён.</exception>
		[NotNull]
		IResourcePool<TReferencedResource> CreateReferencePool<TReferencedResource>([NotNull] Func<TResource, CancellationToken, TReferencedResource> resourceFactory, int? maxResourceCount, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount, [NotNull] ReadonlyParallelOptions parallelOptions)
			where TReferencedResource : class, IDisposable;

		/// <summary>
		/// Прекращает запрос ресурсов, освобождает резервирование ресурсов для запрашивателя по мере возврата ресурсов.
		/// Не ожидает возвращения ресурсов в пул и освобождения ресурсов.
		/// </summary>
		/// <exception cref="T:Toolbox.Data.ResourceAcquirerEndedException">Если запрос ресурсов прекращён.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Если запрашиватель освобождён.</exception>
		void EndAcquire();

		/// <summary>
		/// Прекращён ли запрос ресурсов.
		/// </summary>
		/// <exception cref="T:System.ObjectDisposedException">Если запрашиватель освобождён.</exception>
		bool Ended { get; }
	}
}
