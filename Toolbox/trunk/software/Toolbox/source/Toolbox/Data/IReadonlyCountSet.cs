using System.Collections.Generic;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Интерфейс только для чтения множества с учётом количества вхождений элементов.
	/// </summary>
	/// <typeparam name="TKey">Тип элементов.</typeparam>
	public interface IReadonlyCountSet<TKey> : IEnumerable<KeyValuePair<TKey, int>>, IReadOnlyCollection<KeyValuePair<TKey, int>>
	{
		/// <summary>
		/// Общее количество элементов (включая повторы).
		/// </summary>
		[Pure]
		int TotalCount { get; }

		/// <summary>
		/// Возвращает количество вхождений элемента (0, если элемента нет).
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <returns>Количество вхождений элемента.</returns>
		[Pure]
		int this[[NotNull] TKey key] { get; }

		/// <summary>
		/// Определяет, есть ли элемент (количество вхождений положительно).
		/// </summary>
		/// <param name="key">Элемент.</param>
		/// <returns>Есть ли элемент.</returns>
		[Pure]
		bool Contains([NotNull] TKey key);

		/// <summary>
		/// Возвращает словарь, содержащий в качестве ключей элементы множества, а в качестве значений — количество вхождений соответствующего ключа в множество.
		/// </summary>
		/// <returns>Словарь.</returns>
		[Pure]
		[NotNull]
		IDictionary<TKey, int> ToDictionary();
	}
}