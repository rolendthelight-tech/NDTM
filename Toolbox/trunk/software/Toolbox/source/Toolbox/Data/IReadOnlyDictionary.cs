using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Представляет универсальную коллекцию пар "ключ-значение", доступную только для чтения. (Заглушка до перехода на .NET 4.5.)
	/// </summary>
	/// <typeparam name="TKey">Тип ключей в словаре, доступном только для чтения.</typeparam>
	/// <typeparam name="TValue">Тип значений в словаре, доступном только для чтения.</typeparam>
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		/// <summary>
		/// Получает элемент, имеющий указанный ключ в доступном только для чтения словаре.
		/// </summary>
		/// <returns>
		/// Элемент, имеющий указанный ключ в доступном только для чтения словаре.
		/// </returns>
		/// <param name="key">Искомый ключ.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Свойство получено и параметр <paramref name="key"/> не найден.</exception>
		[Pure]
		TValue this[[NotNull] TKey key] { get; }

		/// <summary>
		/// Получает перечисляемую коллекция, содержащую ключи в словаре только для чтения.
		/// </summary>
		/// <returns>
		/// Перечисляемая коллекция, содержащая ключи в словаре только для чтения.
		/// </returns>
		[Pure]
		[NotNull]
		IEnumerable<TKey> Keys { get; }

		/// <summary>
		/// Получает перечисляемую коллекцию, содержащая значения в словаре только для чтения.
		/// </summary>
		/// <returns>
		/// Перечисляемая коллекция, содержащая значения в словаре только для чтения.
		/// </returns>
		[Pure]
		[NotNull]
		IEnumerable<TValue> Values { get; }

		/// <summary>
		/// Определяет, содержится ли в словаре, доступном только для чтения, элемент с указанным ключом.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если в доступном только для чтения словаре содержится элемент с указанным ключом; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="key">Искомый ключ.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception>
		[Pure]
		bool ContainsKey([NotNull] TKey key);

		/// <summary>
		/// Получает значение, связанное с указанным ключом.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если объект, который реализует интерфейс <see cref="IReadOnlyDictionary{TKey,TValue}"/>, содержит элемент, имеющий указанный ключ; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="key">Искомый ключ.</param>
		/// <param name="value">Этот метод возвращает значение, связанное с указанным ключом, если он найден; в противном случае — значение по умолчанию для данного типа параметра <paramref name="value"/>. Этот параметр передаётся без инициализации.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="key"/> имеет значение null.</exception>
		[Pure]
		bool TryGetValue([NotNull] TKey key, out TValue value);
	}
}
