	using System.Collections;
	using System.Collections.Generic;
	using JetBrains.Annotations;

namespace Toolbox.Data
{
	/// <summary>
	/// <see cref="T:System.Collections.Generic.ISet`1"/>.
	/// Предоставляет основной интерфейс для абстракции наборов.
	/// </summary>
	/// <typeparam name="T">Тип элементов в наборе.</typeparam>
	public interface ISet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, IReadOnlySet<T>, System.Collections.Generic.ISet<T>
	{
		/// <summary>
		/// Добавляет элемент в текущий набор и возвращает значение, указывающее, что элемент был добавлен успешно.
		/// </summary>
		/// 
		/// <returns>
		/// Значение <code>true</code>, если элемент добавлен в набор; значение <code>false</code>, если элемент уже был в наборе.
		/// </returns>
		/// <param name="item">Элемент, добавляемый в набор.</param>
		new bool Add([NotNull] T item);

		/// <summary>
		/// Изменяет текущий набор, чтобы он содержал все элементы, которые имеются как в текущем наборе, так и в указанной коллекции.
		/// </summary>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		new void UnionWith([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Изменяет текущий набор, чтобы он содержал только элементы, которые также имеются в заданной коллекции.
		/// </summary>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		new void IntersectWith([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Удаляет все элементы указанной коллекции из текущего набора.
		/// </summary>
		/// <param name="other">Коллекция элементов, удаляемых из набора.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		new void ExceptWith([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Изменяет текущий набор, чтобы он содержал только элементы, которые имеются либо в текущем наборе, либо в указанной коллекции, но не одновременно в них обоих.
		/// </summary>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		new void SymmetricExceptWith([NotNull] IEnumerable<T> other);
	}
}
