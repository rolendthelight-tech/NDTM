using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Data;
using Toolbox.Tasks;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности перечислений
	/// </summary>
	public static class EnumerableExtensions
  {
	  /// <summary>
		/// Бросает исключение <see cref="T:System.ArgumentNullException"/>, если передано значение <code>null</code>
	  /// </summary>
	  /// <typeparam name="T">Тип значения</typeparam>
	  /// <param name="value">Значение</param>
	  /// <param name="variableName">Имя переменной для исключения</param>
	  /// <returns>Исходное значение</returns>
	  [NotNull]
		[Pure]
	  public static T ThrowIfNull<T>([NotNull] this T value, [NotNull] string variableName) where T : class
	  {
		  if (variableName == null) throw new ArgumentNullException("variableName");
		  if (value == null)
		  {
			  throw new ArgumentNullException(variableName);
		  }

		  return value;
	  }

	  /// <summary>
	  /// Бросает исключение <see cref="T:System.ArgumentException"/>, если передано значение, совпадающее с предикатом
	  /// </summary>
	  /// <typeparam name="T">Тип значения</typeparam>
	  /// <param name="value">Значение</param>
		/// <param name="message">Сообщение для исключения</param>
	  /// <param name="variableName">Имя переменной для исключения</param>
	  /// <param name="predicate">Предикат</param>
	  /// <returns>Исходное значение</returns>
		[Pure]
		public static T ThrowIfMatch<T>(this T value, [NotNull] string message, [NotNull] string variableName, [NotNull] Func<T, bool> predicate)
		{
		  if (message == null) throw new ArgumentNullException("message");
		  if (variableName == null) throw new ArgumentNullException("variableName");
		  if (predicate == null) throw new ArgumentNullException("predicate");

			if (predicate(value))
			{
				throw new ArgumentException(message, variableName);
			}

		  return value;
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.ArgumentNullException"/>, если передано значение <code>null</code>, и <see cref="T:System.ArgumentException"/> — если пустая строка
		/// </summary>
		/// <param name="value">Значение</param>
		/// <param name="variableName">Имя переменной для исключения</param>
		/// <returns>Исходное значение</returns>
		[NotNull]
		[Pure]
		public static string ThrowIfNullOrEmpty([NotNull] this string value, [NotNull] string variableName)
		{
			if (variableName == null) throw new ArgumentNullException("variableName");

			if (value == null)
			{
				throw new ArgumentNullException(variableName);
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Is empty", variableName);
			}

			return value;
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.ArgumentNullException"/> при перечислении, если очередное значение равно <code>null</code>
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <param name="source">Исходное перечисление</param>
		/// <param name="variableName">Имя переменной для исключения</param>
		/// <returns>Исходное значение</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfAnyNull<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] string variableName) where TSource : class
		{
			source.ThrowIfNull("source");
			variableName.ThrowIfNull("variableName");

			return source.Select(elem => elem.ThrowIfNull(variableName));
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.ArgumentNullException"/> при перечислении, если очередное значение равно <code>null</code>, и <see cref="T:System.ArgumentException"/> — если пустая строка
		/// </summary>
		/// <param name="source">Исходное перечисление</param>
		/// <param name="variableName">Имя переменной для исключения</param>
		/// <returns>Исходное значение</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<string> ThrowIfAnyNullOrEmpty([NotNull] this IEnumerable<string> source, [NotNull] string variableName)
		{
			source.ThrowIfNull("source");
			variableName.ThrowIfNull("variableName");

			return source.Select(elem => elem.ThrowIfNullOrEmpty(variableName));
		}

	  /// <summary>
	  /// Бросает исключение <see cref="T:System.ArgumentException"/> при перечислении, если очередное значение совпадает с предикатом
	  /// </summary>
	  /// <param name="source">Исходное перечисление</param>
		/// <param name="message">Сообщение для исключения</param>
	  /// <param name="variableName">Имя переменной для исключения</param>
		/// <param name="predicate">Предикат</param>
	  /// <returns>Исходное значение</returns>
	  [NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfAnyMatch<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] string message, [NotNull] string variableName, [NotNull] Func<TSource, bool> predicate)
		{
		  source.ThrowIfNull("source");
			message.ThrowIfNull("message");
			variableName.ThrowIfNull("variableName");
			predicate.ThrowIfNull("predicate");

			return source.Select(elem => elem.ThrowIfMatch(message, variableName, predicate));
		}

		/// <summary>
		/// Бросает исключение при перечислении, если перечисление пустое
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <param name="source">Исходное перечисление</param>
		/// <param name="variableName">Имя переменной для исключения</param>
		/// <returns>Исходное значение</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfEmpty<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] string variableName)
	  {
		  source.ThrowIfNull("source");
			variableName.ThrowIfNull("variableName");

		  using (var enumerator = source.GetEnumerator())
		  {
			  if (!enumerator.MoveNext())
			  {
					throw new ArgumentException("Enumerable is empty", variableName);
			  }
			  else
			  {
				  yield return enumerator.Current;

				  while (enumerator.MoveNext())
				  {
					  yield return enumerator.Current;
				  }
			  }
		  }
	  }

		/// <summary>
		/// Бросает исключение при перечислении, если перечисление содержит разные элементы.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения.</typeparam>
		/// <param name="source">Исходное перечисление.</param>
		/// <param name="message">Сообщение для исключения.</param>
		/// <param name="variableName">Имя переменной для исключения.</param>
		/// <param name="predicate">Предикат</param>
		/// <returns>Исходное значение.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfDifference<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] string message, [NotNull] string variableName, [CanBeNull] Func<TSource, bool> predicate = null)
			where TSource : TKey
		{
			if (predicate == null)
				return source.ThrowIfDifference(
					message, variableName,
					a => (ISuccessResult<TKey>) SuccessResult<TSource>.Create(a),
					EqualityComparer<TKey>.Default);
			else
				return source.ThrowIfDifference(
					message, variableName,
					a =>
					predicate(a)
						? (ISuccessResult<TKey>) SuccessResult<TSource>.Create(a)
						: (ISuccessResult<TKey>) FailResult<TSource>.Create(),
					EqualityComparer<TKey>.Default);
		}

		/// <summary>
		/// Бросает исключение при перечислении, если перечисление содержит разные элементы.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения.</typeparam>
		/// <param name="source">Исходное перечисление.</param>
		/// <param name="message">Сообщение для исключения.</param>
		/// <param name="variableName">Имя переменной для исключения.</param>
		/// <param name="selector">Функция выбора значения для сравнения.</param>
		/// <returns>Исходное значение.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfDifference<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] string message, [NotNull] string variableName, [NotNull] Func<TSource, ISuccessResult<TKey>> selector)
		{
			return source.ThrowIfDifference(message, variableName, selector, EqualityComparer<TKey>.Default);
		}

		/// <summary>
		/// Бросает исключение при перечислении, если перечисление содержит разные элементы.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения.</typeparam>
		/// <param name="source">Исходное перечисление.</param>
		/// <param name="message">Сообщение для исключения.</param>
		/// <param name="variableName">Имя переменной для исключения.</param>
		/// <param name="selector">Функция выбора значения для сравнения.</param>
		/// <returns>Исходное значение.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfDifference<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] string message, [NotNull] string variableName, [NotNull] Func<TSource, TKey> simpleSelector)
		{
			simpleSelector.ThrowIfNull("simpleSelector");

			return source.ThrowIfDifference(message, variableName, a => SuccessResult<TKey>.Create(simpleSelector(a)), EqualityComparer<TKey>.Default);
		}

		/// <summary>
		/// Бросает исключение при перечислении, если перечисление содержит разные элементы.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения.</typeparam>
		/// <param name="source">Исходное перечисление.</param>
		/// <param name="message">Сообщение для исключения.</param>
		/// <param name="variableName">Имя переменной для исключения.</param>
		/// <param name="selector">Функция выбора значения для сравнения.</param>
		/// <param name="comparer">Компаратор.</param>
		/// <returns>Исходное значение.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> ThrowIfDifference<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] string message, [NotNull] string variableName, [NotNull] Func<TSource, ISuccessResult<TKey>> selector, [NotNull] IEqualityComparer<TKey> comparer)
		{
			source.ThrowIfNull("source");
			message.ThrowIfNull("message");
			variableName.ThrowIfNull("variableName");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");

			using (var source_iterator = source.GetEnumerator())
			{
				while (source_iterator.MoveNext())
				{
					var current = source_iterator.Current;
					yield return current;
					var key = selector(current);
					if (key.Success)
					{
						var first_key = key.Result;
						while (source_iterator.MoveNext())
						{
							current = source_iterator.Current;
							var next_key = selector(current);

							if (next_key.Success)
								if (!comparer.Equals(first_key, next_key.Result))
									throw new ArgumentException(message, variableName);

							yield return current;
						}
						break;
					}
				}
			}
		}

    /// <summary>
    /// Returns the minimal element of the given sequence, based on
    /// the given projection.
    /// </summary>
    /// <remarks>
    /// If more than one element has the minimal projected value, the first
    /// one encountered will be returned. This overload uses the default comparer
    /// for the projected type. This operator uses immediate execution, but
    /// only buffers a single result (the current minimal element).
    /// </remarks>
    /// <typeparam name="TSource">Type of the source sequence</typeparam>
    /// <typeparam name="TKey">Type of the projected element</typeparam>
    /// <param name="source">Source sequence</param>
    /// <param name="selector">Selector to use to pick the results to compare</param>
    /// <returns>The minimal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <code>null</code></exception>
    /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[Pure]
		public static TSource MinBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
				[NotNull] Func<TSource, TKey> selector)
    {
      return source.MinBy(selector, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Returns the minimal element of the given sequence, based on
    /// the given projection and the specified comparer for projected values.
    /// </summary>
    /// <remarks>
    /// If more than one element has the minimal projected value, the first
    /// one encountered will be returned. This overload uses the default comparer
    /// for the projected type. This operator uses immediate execution, but
    /// only buffers a single result (the current minimal element).
    /// </remarks>
    /// <typeparam name="TSource">Type of the source sequence</typeparam>
    /// <typeparam name="TKey">Type of the projected element</typeparam>
    /// <param name="source">Source sequence</param>
    /// <param name="selector">Selector to use to pick the results to compare</param>
    /// <param name="comparer">Comparer to use to compare projected values</param>
    /// <returns>The minimal element, according to the projection.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
		/// or <paramref name="comparer"/> is <code>null</code></exception>
    /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[Pure]
		public static TSource MinBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
				[NotNull] Func<TSource, TKey> selector, [NotNull] IComparer<TKey> comparer)
    {
      source.ThrowIfNull("source");
      selector.ThrowIfNull("selector");
      comparer.ThrowIfNull("comparer");
      using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
      {
        if (!sourceIterator.MoveNext())
        {
          throw new InvalidOperationException("Sequence was empty");
        }
        TSource min = sourceIterator.Current;
        TKey minKey = selector(min);
        while (sourceIterator.MoveNext())
        {
          TSource candidate = sourceIterator.Current;
          TKey candidateProjected = selector(candidate);
          if (comparer.Compare(candidateProjected, minKey) < 0)
          {
            min = candidate;
            minKey = candidateProjected;
          }
        }
        return min;
      }
    }

     /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <code>null</code></exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[Pure]
		public static TSource MaxBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
		                                           [NotNull] Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/>
        /// or <paramref name="comparer"/> is <code>null</code></exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[Pure]
		public static TSource MaxBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
						[NotNull] Func<TSource, TKey> selector, [NotNull] IComparer<TKey> comparer)
        {
            source.ThrowIfNull("source");
            selector.ThrowIfNull("selector");
            comparer.ThrowIfNull("comparer");
            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

		/// <summary>
		/// Возвращает максимальный элемент, если это возможно
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения</typeparam>
		/// <param name="source">Исходное перечисление</param>
		/// <param name="selector">Функция выбора значения для сравнения</param>
		/// <param name="value">Максимальный элемент</param>
		/// <returns>Успешность получения максимума</returns>
		[Pure]
		public static bool TryMaxBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
																							 [NotNull] Func<TSource, TKey> selector, out TSource value)
    {
      return source.TryMaxBy(selector, out value, Comparer<TKey>.Default);
    }

		/// <summary>
		/// Возвращает максимальный элемент, если это возможно
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения</typeparam>
		/// <param name="source">Исходное перечисление</param>
		/// <param name="selector">Функция выбора значения для сравнения</param>
		/// <param name="value">Максимальный элемент</param>
		/// <param name="comparer">Компаратор</param>
		/// <returns>Успешность получения максимума</returns>
		[Pure]
		public static bool TryMaxBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
																							 [NotNull] Func<TSource, TKey> selector, out TSource value, [NotNull] IComparer<TKey> comparer)
    {
      source.ThrowIfNull("source");
      selector.ThrowIfNull("selector");
      comparer.ThrowIfNull("comparer");
      using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
      {
        if (!sourceIterator.MoveNext())
        {
          value = default(TSource);
          return false;
        }
        TSource max = sourceIterator.Current;
        TKey maxKey = selector(max);
        while (sourceIterator.MoveNext())
        {
          TSource candidate = sourceIterator.Current;
          TKey candidateProjected = selector(candidate);
          if (comparer.Compare(candidateProjected, maxKey) > 0)
          {
            max = candidate;
            maxKey = candidateProjected;
          }
        }
        value = max;
        return true;
      }
    }

		/// <summary>
		/// Значение по умолчанию типа элементов последовательности.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <returns>Значение по умолчанию типа элементов последовательности.</returns>
		[Pure]
    public static TSource Default<TSource>([CanBeNull] this IEnumerable<TSource> source)
    {
      return default(TSource);
    }

		/// <summary>
		/// Фильтрует последовательность элементов на основе предиката, если условие истинно, иначе — не фильтрует
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <param name="source">Исходная последовательность</param>
		/// <param name="condition">Условие включения фильтрации</param>
		/// <param name="predicate">Предикат для фильтрации</param>
		/// <returns>Отфильтрованная последовательность, если условие истинно, иначе — исходная</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> WhereIf<TSource>([NotNull] this IEnumerable<TSource> source, bool condition, [NotNull] Func<TSource, bool> predicate)
		{
			source.ThrowIfNull("source");
			predicate.ThrowIfNull("predicate");
			if (condition)
				return source.Where(predicate);
			else
				return source;
		}

	  /// <summary>
		/// Фильтрует последовательность элементов на основе предиката, если условие истинно, иначе — не фильтрует
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <param name="source">Исходная последовательность</param>
		/// <param name="condition">Условие включения фильтрации</param>
		/// <param name="predicate">Предикат для фильтрации</param>
		/// <returns>Отфильтрованная последовательность, если условие истинно, иначе — исходная</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> WhereIf<TSource>([NotNull] this IEnumerable<TSource> source, bool condition, [NotNull] Func<TSource, int, bool> predicate)
		{
			source.ThrowIfNull("source");
			predicate.ThrowIfNull("predicate");
			if (condition)
				return source.Where(predicate);
			else
				return source;
		}

		/// <summary>
		/// Циклически сдвигает последовательность
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <param name="source">Исходная последовательность</param>
		/// <param name="index">Размер сдвига (последовательность не должна быть короче, чем сдвиг)</param>
		/// <returns>Циклически сдвинутая последовательность</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> Shift<TSource>([NotNull] this IEnumerable<TSource> source, int index)
		{
			source.ThrowIfNull("source");
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Меньше нуля");

			{
				bool reset_not_supported = false;
				using (var enumerator = source.GetEnumerator())
				{
					for (int i = index; i > 0; --i)
					{
						if (!enumerator.MoveNext())
							throw new InvalidOperationException("Последовательность короче сдвига");
					}
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
					try
					{
						enumerator.Reset();
					}
					catch (NotSupportedException)
					{
						reset_not_supported = true;
					}
					if (!reset_not_supported)
						for (int i = index; i > 0; --i)
						{
							if (!enumerator.MoveNext())
								throw new InvalidOperationException("Последовательность короче сдвига");
							else
								yield return enumerator.Current;
						}
				}
				if (reset_not_supported)
					using (var enumerator = source.GetEnumerator())
					{
						for (int i = index; i > 0; --i)
						{
							if (!enumerator.MoveNext())
								throw new InvalidOperationException("Последовательность короче сдвига");
							else
								yield return enumerator.Current;
						}
					}
			}
		}

		/// <summary>
		/// Возвращает первый элемент, удовлетворяющий условию, или максимальный (если условию ни один не удовлетворил), или полученный указанной функцией (если максимума нет)
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения</typeparam>
		/// <param name="source">Исходная последовательность</param>
		/// <param name="predicate">Предикат для фильтрации (не учитывается при поиске максимума)</param>
		/// <param name="selector">Функция выбора значения для сравнения</param>
		/// <param name="factory">Функция получения значения для пустой последовательности</param>
		/// <returns></returns>
		[Pure]
		public static TSource FirstOrMaxOrGet<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate, [NotNull] Func<TSource, TKey> selector, [NotNull] Func<TSource> factory)
		{
			return source.FirstOrMaxOrGet(predicate, selector, Comparer<TKey>.Default, factory);
		}

		/// <summary>
		/// Возвращает первый элемент, удовлетворяющий условию, или максимальный (если условию ни один не удовлетворил), или полученный указанной функцией (если максимума нет)
		/// </summary>
		/// <typeparam name="TSource">Тип элементов</typeparam>
		/// <typeparam name="TKey">Тип значения для сравнения</typeparam>
		/// <param name="source">Исходная последовательность</param>
		/// <param name="predicate">Предикат для фильтрации (не учитывается при поиске максимума)</param>
		/// <param name="selector">Функция выбора значения для сравнения</param>
		/// <param name="comparer">Компаратор</param>
		/// <param name="factory">Функция получения значения для пустой последовательности</param>
		/// <returns></returns>
		[Pure]
		public static TSource FirstOrMaxOrGet<TSource, TKey>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate, [NotNull] Func<TSource, TKey> selector, [NotNull] IComparer<TKey> comparer, [NotNull] Func<TSource> factory)
		{
			source.ThrowIfNull("source");
			predicate.ThrowIfNull("predicate");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");
			factory.ThrowIfNull("factory");

			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					return factory();
				}
				TSource max = sourceIterator.Current;
				if (predicate(max))
					return max;
				TKey maxKey = selector(max);
				while (sourceIterator.MoveNext())
				{
					TSource candidate = sourceIterator.Current;
					if (predicate(candidate))
						return candidate;
					TKey candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}
				return max;
			}
		}

		[Flags]
		public enum AllAnyEnum
		{
			/// <summary> Не существует и не все (множество не пустое, но соответствия нет). </summary>
			None = 0x0,
			/// <summary> Все. </summary>
			All = 0x1,
			/// <summary> Существует. </summary>
			Exists = 0x2,
		}

		/// <summary>
		/// Проверяет, соответствуют ли условию все или некоторые элементы.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="predicate">Предикат для проверки условия.</param>
		/// <returns>Набор флагов, содержащий <see cref="AllAnyEnum.All"/>, если все элементы соответствуют условию, <see cref="AllAnyEnum.Exists"/>, если некоторые элементы соответствуют условию.</returns>
		[Pure]
		public static AllAnyEnum AllAny<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			var result = AllAnyEnum.All;

			foreach (var item in source)
			{
				if (predicate(item))
				{
					result |= AllAnyEnum.Exists;
				}
				else
				{
					result &= ~AllAnyEnum.All;
				}
				if (result == AllAnyEnum.Exists)
					break; // Результат больше не изменится
			}
			return result;
		}

		/// <summary>
		/// Создаёт множество <see cref="T:System.Collections.Generic.HashSet`1"/> из объекта <see cref="T:System.Collections.Generic.IEnumerable`1"/>. 
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <returns>Объект <see cref="T:System.Collections.Generic.HashSet`1"/>, содержащий элементы из входной последовательности.</returns>
		[NotNull]
		[Pure]
		public static HashSet<TSource> ToHashSet<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return new HashSet<TSource>(source);
		}

		/// <summary>
		/// Создаёт множество <see cref="T:System.Collections.Generic.HashSet`1"/> из объекта <see cref="T:System.Collections.Generic.IEnumerable`1"/>. 
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="equalityComparer">Компаратор.</param>
		/// <returns>Объект <see cref="T:System.Collections.Generic.HashSet`1"/>, содержащий элементы из входной последовательности.</returns>
		[NotNull]
		[Pure]
		public static HashSet<TSource> ToHashSet<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] IEqualityComparer<TSource> equalityComparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");

			return new HashSet<TSource>(source, equalityComparer);
		}

		/// <summary>
		/// Фильтрует последовательность элементов, выводя только дубликаты элементов.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <returns>Отфильтрованная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> Duplicates<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			return source.Duplicates(EqualityComparer<TSource>.Default);
		}

		/// <summary>
		/// Фильтрует последовательность элементов, выводя только дубликаты элементов.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="comparer">Компаратор.</param>
		/// <returns>Отфильтрованная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> Duplicates<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] IEqualityComparer<TSource> comparer)
		{
			source.ThrowIfNull("source");
			comparer.ThrowIfNull("comparer");

			var set = new HashSet<TSource>(comparer);
			foreach (var item in source)
			{
				if (!set.Add(item))
				{
					yield return item;
				}
			}
		}

		[Pure]
		private static int? GetCollectionCount<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			source.ThrowIfNull("source");

			var enumerable_type = source.GetType();

			TypeFilter tf = (Type t1, object fc1) =>
				{
					if (fc1 != null) throw new ArgumentException("Not null", "fc1");

					var fc2 = typeof (ICollection<>);
					return fc2.IsGenericType && t1.IsGenericType && fc2 == t1.GetGenericTypeDefinition();
				};

			var implemented_collection_interfaces = enumerable_type.FindInterfaces(tf, null);

			Type implemented_collection_interface = implemented_collection_interfaces
				.ThrowIfAnyMatch("interface is generic type definition", "interface", ici => ici.IsGenericTypeDefinition)
				.FirstOrDefault();

			if (implemented_collection_interface == null)
				return null;
			else
			{
				var pi = implemented_collection_interface.GetProperty("Count", typeof (int));
				int count = (int) pi.GetValue(source, null);
				return count;
			}
		}

		/// <summary>
		/// Возвращает первый элемент последовательности, удовлетворяющий хотя бы одному указанному условию.
		/// Условия поиска упорядочены по убыванию приоритета. При нахождении элемента, удовлетворяющего первому условию, он возвращается сразу, прочим условиям — после окончания перечисления, чтобы убедиться, что остальные элементы не удовлетворяют первому условию.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="predicates">Условия поиска, упорядоченные по убыванию приоритета.</param>
		/// <returns></returns>
		[Pure]
		[NotNull]
		public static ElementNumber<TSource> First<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] params Func<TSource, bool>[] predicates)
		{
			source.ThrowIfNull("source");
			predicates.ThrowIfNull("predicates");
			predicates.ThrowIfAnyNull("predicate").ForAll();

			TSource elem = default(TSource);
			int elem_predicate_i = predicates.Length;

			if (elem_predicate_i != 0)
				foreach (var candidate in source)
				{
					for (int i = 0; i < elem_predicate_i; ++i)
					{
						var predicate = predicates[i];
						if (predicate(candidate))
						{
							elem = candidate;
							elem_predicate_i = i;
							if (i == 0)
								return new ElementNumber<TSource>(elem, elem_predicate_i);
							else
								break;
						}
					}
					if (elem_predicate_i == 0)
						throw new ApplicationException("Индекс достиг нуля");
				}

			if (elem_predicate_i < 0)
				throw new ApplicationException("Индекс меньше нуля");

			if (elem_predicate_i < predicates.Length)
				return new ElementNumber<TSource>(elem, elem_predicate_i);

			throw new InvalidOperationException("Элемент, соответствующий хотя бы одному условию, не найден");
		}

		/// <summary>
		/// Пронумерованный элемент.
		/// </summary>
		/// <typeparam name="T">Тип элемента.</typeparam>
		[CannotApplyEqualityOperatorAttribute]
		public class ElementNumber<T> : Tuple<T, int>
		{
			public ElementNumber(T element, int number)
				: base(element, number)
			{
			}

			/// <summary>
			/// Значение элемента.
			/// </summary>
			public T Element
			{
				get { return this.Item1; }
			}

			/// <summary>
			/// Номер элемента.
			/// </summary>
			public int Number
			{
				get { return this.Item2; }
			}
		}

		/// <summary>
		/// Перечисляет всё перечисление и возвращает <code>true</code>.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <returns>Возвращает <code>true</code> после перечисления.</returns>
		[Pure]
		public static bool ForAll<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			source.ThrowIfNull("source");

			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
				}
			}
			return true;
		}

		/// <summary>
		/// Фильтрует последовательность элементов и подавляет ошибки при перечислении, передавая возникшие исключения в предикат, определяющий нужно ли получать значение элемента, переход к которому вызвал ошибку.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="errorHandler">Предикат, определяющий нужно ли получать значение элемента, переход к которому вызвал ошибку. Если <c>null</c> — все ошибки будут подавлены, а соответствующие элементы — пропущены.</param>
		/// <returns>Отфильтрованная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IEnumerable<TSource> WhereNotError<TSource>([NotNull] this IEnumerable<TSource> source, [CanBeNull] Func<Exception, bool> errorHandler)
		{
			source.ThrowIfNull("source");

			if (errorHandler == null)
				errorHandler = WhereErrorHandler;

			using (var enumerator = source.GetEnumerator())
			{
				for (;;)
				{
					bool found;
					bool error;
					try
					{
						found = enumerator.MoveNext();
						error = false;
					}
					catch (Exception ex)
					{
						found = true;
						error = !errorHandler(ex);
					}
					if (!found)
						break;
					if (!error)
						yield return enumerator.Current;
				}
			}
		}

		/// <summary>
		/// Пример предиката, подавляющего ошибки, пропускающего соответствующие элементы.
		/// </summary>
		/// <param name="ex">Возникшее исключение.</param>
		/// <returns><c>false</c></returns>
		[Pure]
		private static bool WhereErrorHandler([NotNull] Exception ex)
		{
			return false;
		}

		/// <summary>
		/// Сравнивает последовательности с учётом порядка элементов.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="sources">Последовательности (не менее одной).</param>
		/// <returns>Эквивалентность последовательностей.</returns>
		public static bool SequencesEqual<TSource>([NotNull] this IEnumerable<TSource>[] sources)
		{
			return sources.SequencesEqual(EqualityComparer<TSource>.Default);
		}

		/// <summary>
		/// Сравнивает последовательности с учётом порядка элементов.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="sources">Последовательности (не менее одной).</param>
		/// <param name="comparer">Компаратор элементов.</param>
		/// <returns>Эквивалентность последовательностей.</returns>
		public static bool SequencesEqual<TSource>([NotNull] this IEnumerable<TSource>[] sources, [NotNull] IEqualityComparer<TSource> comparer)
		{
			sources.ThrowIfNull("sources");
			comparer.ThrowIfNull("comparer");

			if (sources.Length == 0)
				throw new InvalidOperationException("Нет перечислений");

			IEnumerator<TSource>[] es = new IEnumerator<TSource>[sources.Length];

			try
			{
				for (int i = 0; i < sources.Length; ++i)
				{
					var source = sources[i].ThrowIfNull("source");
					var e = source.GetEnumerator().ThrowIfNull("e");
					es[i] = e;
				}

				var es_others = es.Skip(1);
				var es_current_distinct = es.Select(e => e.Current).Distinct(comparer).Take(2).Skip(1);
				var e0 = es[0];
				while (e0.MoveNext())
				{
					if (!es_others.All(e => e.MoveNext()))
						return false;
					else if (es_current_distinct.Any())
						return false;
				}
				if (es_others.Any(e => e.MoveNext()))
					return false;
				else
					return true;
			}
			finally
			{
				for (int i = 0; i < es.Length; ++i)
				{
					var e = es[i];
					if (e != null)
					{
						e.Dispose();
						es[i] = null;
					}
				}
			}
		}

		/// <summary>
		/// Сравнивает две последовательности без учёта порядка элементов.
		/// По крайней мере одна из последовательностей должна быть конечна, иначе сравнение не завершится.
		/// Работает как <see cref="M:System.Linq.Enumerable.SequencesEqual`1(IEnumerable`1,IEnumerable`1)"/> сортированных последовательностей, но не требует <see cref="IComparer`1"/>, потребляет меньше памяти, если порядок элементов в последовательностях похож, не требует конечности всех последовательностей.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="first">Первая последовательность.</param>
		/// <param name="second">Вторая последовательность.</param>
		/// <returns>Эквивалентность последовательностей.</returns>
		public static bool SetEqual<TSource>([NotNull] this IEnumerable<TSource> first, [NotNull] IEnumerable<TSource> second)
		{
			return SetEqual<TSource>(first, second, EqualityComparer<TSource>.Default);
		}

		/// <summary>
		/// Сравнивает две последовательности без учёта порядка элементов.
		/// По крайней мере одна из последовательностей должна быть конечна, иначе сравнение не завершится.
		/// Работает как <see cref="M:System.Linq.Enumerable.SequencesEqual`1(IEnumerable`1,IEnumerable`1)"/> сортированных последовательностей, но не требует <see cref="IComparer`1"/>, потребляет меньше памяти, если порядок элементов в последовательностях похож, не требует конечности всех последовательностей.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="first">Первая последовательность.</param>
		/// <param name="second">Вторая последовательность.</param>
		/// <param name="comparer">Компаратор элементов.</param>
		/// <returns>Эквивалентность последовательностей.</returns>
		[NotNull]
		[Pure]
		public static bool SetEqual<TSource>([NotNull] this IEnumerable<TSource> first, [NotNull] IEnumerable<TSource> second, [NotNull] IEqualityComparer<TSource> comparer)
		{
			first.ThrowIfNull("first");
			second.ThrowIfNull("second");
			comparer.ThrowIfNull("comparer");

			using (IEnumerator<TSource> e1 = first.GetEnumerator())
			using (IEnumerator<TSource> e2 = second.GetEnumerator())
			{
				ICountSet<TSource> s1 = new CountSet<TSource>(comparer: comparer);
				ICountSet<TSource> s2 = new CountSet<TSource>(comparer: comparer);

				while (e1.MoveNext())
				{
					if (!(e2.MoveNext()))
						return false;
					else
					{
						var c1 = e1.Current;
						var c2 = e2.Current;
						if (s2.TryDecrement(c1))
						{
							s2.Increment(c2);
						}
						else
						{
							s1.Increment(c1);
							if (s1.TryDecrement(c2))
							{
							}
							else
							{
								s2.Increment(c2);
							}
						}
					}
				}
				if (e2.MoveNext())
					return false;
				else
				{
					IReadonlyCountSet<TSource> s1r = s1;
					IReadonlyCountSet<TSource> s2r = s2;
					var c1 = s1r.TotalCount;
					var c2 = s2r.TotalCount;
					if (c1 != 0)
						return false;
					if (c1 != c2)
						throw new ApplicationException("Ошибка подсчёта элементов");
					return true;
				}
			}
		}

		/// <summary>
		/// Сортирует последовательность элементов, выводя дубликаты элементов после остальных.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <returns>Отсортированная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IOrderedEnumerable<TSource> OrderByDuplicates<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			source.ThrowIfNull("source");

			return source.OrderByDuplicates(EqualityComparer<TSource>.Default);
		}

		/// <summary>
		/// Сортирует последовательность элементов, выводя дубликаты элементов после остальных.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная последовательность.</param>
		/// <param name="comparer">Компаратор.</param>
		/// <returns>Отсортированная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IOrderedEnumerable<TSource> OrderByDuplicates<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] IEqualityComparer<TSource> comparer)
		{
			source.ThrowIfNull("source");
			comparer.ThrowIfNull("comparer");

			return source.OrderBy(DuplicatesKeySelectorFactory(comparer));
		}

		/// <summary>
		/// Выполняет дополнительное упорядочение элементов последовательности, выводя дубликаты элементов после остальных.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная упорядоченная последовательность.</param>
		/// <returns>Отсортированная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IOrderedEnumerable<TSource> ThenByDuplicates<TSource>([NotNull] this IOrderedEnumerable<TSource> source)
		{
			source.ThrowIfNull("source");

			return source.ThenByDuplicates(EqualityComparer<TSource>.Default);
		}

		/// <summary>
		/// Выполняет дополнительное упорядочение элементов последовательности, выводя дубликаты элементов после остальных.
		/// </summary>
		/// <typeparam name="TSource">Тип элементов.</typeparam>
		/// <param name="source">Исходная упорядоченная последовательность.</param>
		/// <param name="comparer">Компаратор.</param>
		/// <returns>Отсортированная последовательность.</returns>
		[NotNull]
		[Pure]
		public static IOrderedEnumerable<TSource> ThenByDuplicates<TSource>([NotNull] this IOrderedEnumerable<TSource> source, [NotNull] IEqualityComparer<TSource> comparer)
		{
			source.ThrowIfNull("source");
			comparer.ThrowIfNull("comparer");

			return source.ThenBy(DuplicatesKeySelectorFactory(comparer));
		}

		[NotNull]
		private static Func<TSource, bool> DuplicatesKeySelectorFactory<TSource>([NotNull] IEqualityComparer<TSource> comparer)
		{
			if (comparer == null) throw new ArgumentNullException("comparer");

			var set = new HashSet<TSource>(comparer);
			return (item) => !set.Add(item);
		}
	}
}
