using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности множеств.
	/// </summary>
	public static class SetExtensions
	{
		/// <summary>
		/// Добавляет указанный элемент в набор.
		/// </summary>
		/// <typeparam name="T">Тип элементов в наборе.</typeparam>
		/// <param name="set">Набор.</param>
		/// <param name="item">Элемент, добавляемый в набор.</param>
		public static void CheckedAdd<T>([NotNull] this ISet<T> set, T item)
		{
			if (set == null) throw new ArgumentNullException("set");
			if (!set.Add(item))
				throw new ArgumentException("Такой элемент уже существует во множестве", "item");
		}

		/// <summary>
		/// Добавляет указанный элемент в набор.
		/// </summary>
		/// <typeparam name="T">Тип элементов в наборе.</typeparam>
		/// <param name="set">Набор.</param>
		/// <param name="item">Элемент, добавляемый в набор.</param>
		public static void CheckedAdd<T>([NotNull] this Toolbox.Data.ISet<T> set, T item)
		{
			if (set == null) throw new ArgumentNullException("set");
			if (!set.Add(item))
				throw new ArgumentException("Такой элемент уже существует во множестве", "item");
		}

		/// <summary>
		/// Удаляет указанный элемент из набора.
		/// </summary>
		/// <typeparam name="T">Тип элементов в наборе.</typeparam>
		/// <param name="set">Набор.</param>
		/// <param name="item">Подлежащий удалению элемент.</param>
		public static void CheckedRemove<T>([NotNull] this ISet<T> set, T item)
		{
			if (set == null) throw new ArgumentNullException("set");
			if (!set.Remove(item))
				throw new ArgumentException("Такого элемента не существует во множестве", "item");
		}

		/// <summary>
		/// Удаляет указанный элемент из набора.
		/// </summary>
		/// <typeparam name="T">Тип элементов в наборе.</typeparam>
		/// <param name="set">Набор.</param>
		/// <param name="item">Подлежащий удалению элемент.</param>
		public static void CheckedRemove<T>([NotNull] this Toolbox.Data.ISet<T> set, T item)
		{
			if (set == null) throw new ArgumentNullException("set");
			if (!set.Remove(item))
				throw new ArgumentException("Такого элемента не существует во множестве", "item");
		}
	}
}
