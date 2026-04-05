using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// <see cref="T:System.Collections.Generic.ISet`1"/>.
	/// Предоставляет основной интерфейс для абстракции наборов только для чтения.
	/// </summary>
	/// <typeparam name="T">Тип элементов в наборе.</typeparam>
	public interface IReadOnlySet<T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		/// <summary>
		/// Определяет, является ли набор подмножеством заданной коллекции.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если текущий набор является подмножеством объекта <paramref name="other"/>; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool IsSubsetOf([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Определяет, является ли текущий набор надмножеством заданной коллекции.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если текущий набор является надмножеством объекта <paramref name="other"/>; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool IsSupersetOf([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Определяет, является ли текущий набор должным (строгим) подмножеством заданной коллекции.
		/// </summary>
		/// <returns>
		/// <code>true</code>, если текущий набор является надлежащим надмножеством объекта <paramref name="other"/>; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool IsProperSupersetOf([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Определяет, является ли текущий набор должным (строгим) подмножеством заданной коллекции.
		/// </summary>
		/// <returns>
		/// <code>true</code>, если текущий набор является надлежащим подмножеством объекта <paramref name="other"/>; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool IsProperSubsetOf([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Определяет, пересекаются ли текущий набор и указанная коллекция.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если в текущем наборе и объекте <paramref name="other"/> имеется по крайней мере один общий элемент; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool Overlaps([NotNull] IEnumerable<T> other);

		/// <summary>
		/// Определяет, содержат ли текущий набор и указанная коллекция одни и те же элементы.
		/// </summary>
		/// <returns>
		/// Значение <code>true</code>, если текущий набор равен объекту <paramref name="other"/>; в противном случае — значение <code>false</code>.
		/// </returns>
		/// <param name="other">Коллекция для сравнения с текущим набором.</param>
		/// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="other"/> имеет значение <code>null</code>.</exception>
		[Pure]
		bool SetEquals([NotNull] IEnumerable<T> other);
	}
}
