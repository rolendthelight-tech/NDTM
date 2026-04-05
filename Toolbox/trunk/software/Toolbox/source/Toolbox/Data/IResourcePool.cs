using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Common;

namespace Toolbox.Data
{
	/// <summary>
	/// Пул ресурсов, предоставляющий ссылки на ресурс.
	/// Для уничтожения пула (и освобождения ресурсов) следует вызвать <see cref="IDisposable.Dispose()"/> 1 раз.
	/// </summary>
	/// <typeparam name="TResource">Тип ресурса.</typeparam>
	public interface IResourcePool<out TResource> : IDisposable
		where TResource : class, IDisposable
	{
		/// <summary>
		/// Создаёт запрашивателя ресурсов из пула.
		/// </summary>
		/// <param name="selector">Функция выбора читаемой части ресурса.</param>
		/// <param name="reserveResourceCount">Количество резервируемых ресурсов. (Левая граница — количество резервируемых сразу, правая — количество резервируемых сразу или позже по мере освобождения резерва в пуле.)</param>
		/// <param name="cancel">Токен отмены.</param>
		/// <returns>Запрашиватель на ресурс.</returns>
		/// <exception cref="T:System.OperationCanceledException">Если операция отменена.</exception>
		/// <exception cref="T:System.ArgumentNullException">Если какой-либо параметр <code>null</code>.</exception>
		/// <exception cref="T:Toolbox.Data.ResourcePoolEndedException">Если создание запрашивателей ресурсов прекращено.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Если пул освобождён.</exception>
		[NotNull]
		IResourceAcquirer<TReadonlyResource> CreateAcquirer<TReadonlyResource>([NotNull] Func<TResource, TReadonlyResource> selector, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> reserveResourceCount, CancellationToken cancel);

		/// <summary>
		/// Прекращает создание запрашивателей ресурсов, освобождает ресурсы по мере снятия резервирования ресурсов запрашивателями.
		/// Не ожидает возвращения ресурсов в пул и освобождения ресурсов.
		/// </summary>
		/// <exception cref="T:Toolbox.Data.ResourcePoolEndedException">Если создание запрашивателей ресурсов прекращено.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Если пул освобождён.</exception>
		void EndCreate();

		/// <summary>
		/// Прекращено ли создание запрашивателей ресурсов.
		/// </summary>
		/// <exception cref="T:System.ObjectDisposedException">Если пул освобождён.</exception>
		bool Ended { get; }
	}
}
