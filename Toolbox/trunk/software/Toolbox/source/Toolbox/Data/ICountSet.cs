using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace Toolbox.Data
{
	/// <summary>
	/// Интерфейс множества с учётом количества вхождений элементов.
	/// </summary>
	/// <typeparam name="TKey">Тип элементов.</typeparam>
	public interface ICountSet<TKey> : ICollection, IReadonlyCountSet<TKey> //…
	{
		/// <summary>
		/// Добавляет элемент или увеличивает количество вхождений на 1.
		/// </summary>
		/// <param name="key">Элемент.</param>
		void Increment([NotNull] TKey key);

		/// <summary>
		/// Добавляет элемент или увеличивает количество вхождений на указанное количество.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <param name="count">Количество вхождений.</param>
		void Increment([NotNull] TKey key, int count);

		/// <summary>
		/// Удаляет элемент или уменьшает количество вхождений на 1.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <exception cref="T:System.InvalidOperationException">Если элемента нет.</exception>
		void Decrement([NotNull] TKey key);

		/// <summary>
		/// Удаляет элемент или уменьшает количество вхождений на указанное количество.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <param name="count">Количество вхождений.</param>
		/// <exception cref="T:System.InvalidOperationException">Если элемента нет.</exception>
		void Decrement([NotNull] TKey key, int count);

		/// <summary>
		/// Пытается добавить элемент или увеличить количество вхождений на 1.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <returns>Удалось ли добавить элемент или увеличить количество вхождений.</returns>
		bool TryIncrement([NotNull] TKey key);

		/// <summary>
		/// Пытается добавить элемент или увеличить количество вхождений на указанное количество.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <param name="count">Количество вхождений.</param>
		/// <returns>Удалось ли добавить элемент или увеличить количество вхождений.</returns>
		bool TryIncrement([NotNull] TKey key, int count);

		/// <summary>
		/// Пытается удалить элемент или уменьшить количество вхождений на 1.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <returns>Удалось ли удалить элемент или уменьшить количество вхождений (был ли элемент).</returns>
		bool TryDecrement([NotNull] TKey key);

		/// <summary>
		/// Пытается удалить элемент или уменьшить количество вхождений на указанное количество.
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <param name="count">Количество вхождений.</param>
		/// <returns>Удалось ли удалить элемент или уменьшить количество вхождений (был ли элемент).</returns>
		bool TryDecrement([NotNull] TKey key, int count);
	}
}