using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
  /// <summary>
  ///   Расширение функциональности массивов
  /// </summary>
  public static class ArrayExtensions
  {
    /// <summary>
    ///   Получение части массива от start до end
    /// </summary>
    [NotNull]
		[Pure]
		public static T[] Slice<T>([NotNull] this T[] source,
                                int start,
                                int end)
    {
      if (null == source)
        throw new ArgumentNullException("source");

			if (end < start)
				throw new ArgumentOutOfRangeException("end", end, "< start");
			if (start < source.GetLowerBound(0))
				throw new ArgumentOutOfRangeException("start", start, "< source.GetLowerBound(0)");
			if (end > source.GetUpperBound(0))
				throw new ArgumentOutOfRangeException("end", end, "> source.GetUpperBound(0)");

      int len = end - start + 1;

      T[] res = new T[len];
      for (int i = 0; i < len; i++)
        res[i] = source[i + start];
      return res;
    }

    /// <summary>
    ///   Получение части массива от start до конца
    /// </summary>
    [NotNull]
		[Pure]
		public static T[] Slice<T>([NotNull] this T[] source,
                                int start)
    {
      if (null == source)
        throw new ArgumentNullException("source");

			return source.Slice(start, source.GetUpperBound(0));
    }

    /// <summary>
    ///   Возвращает инвертированный массив
    /// </summary>
    [NotNull]
		[Pure]
		public static T[] ReversedData<T>([NotNull] this T[] data)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      T[] reversed = new T[data.Length];
      data.CopyTo(reversed, 0);
      Array.Reverse(reversed);

      return reversed;
    }

    /// <summary>
    ///   Присваивание значения элементам массива от Start до End
    /// </summary>
    [NotNull]
		public static T[] SetValues<T>([NotNull] this T[] data,
                                    T value,
                                    int start,
                                    int end)
    {
      if (null == data)
        throw new ArgumentNullException("data");

			if (end < start)
				throw new ArgumentOutOfRangeException("end", end, "< start");
			if (start < data.GetLowerBound(0))
				throw new ArgumentOutOfRangeException("start", start, "< data.GetLowerBound(0)");
			if (end > data.GetUpperBound(0))
				throw new ArgumentOutOfRangeException("end", end, "> data.GetUpperBound(0)");

      for (int i = start; i <= end; i++)
        data[i] = value;

      return data;
    }

    /// <summary>
    ///   Дополнение нулями до необходимой длины. Если массив длиннее требуемой длины, генерируется исключение ArgumentException
    /// </summary>
    [NotNull]
		[Pure]
		public static T[] ZeroPadding<T>([NotNull] this T[] data,
                                      int length)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      if (length < data.Length)
				throw new ArgumentException("length < data.Length", "length");

      T[] padding = new T[length];
      Array.Copy(data, padding, data.Length);

      return padding;
    }

    /// <summary>
    ///   Конкатенация массивов
    /// </summary>
    [NotNull]
		[Pure]
		public static T[] Concatenate<T>([NotNull] this T[] data, [NotNull] T[] added)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      if (null == added)
        throw new ArgumentNullException("added");

      T[] copy = new T[data.Length + added.Length];
      Array.Copy(data, copy, data.Length);
			Array.Copy(added, added.GetLowerBound(0), copy, data.Length, added.Length);

      return copy;
    }

    /// <summary>
    /// Минимальное значение из диапазона данных.
    /// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Минимальное значение.</returns>
		[Pure]
		public static T Min<T>([NotNull] this T[] data,
                            int start,
                            int end) where T : IComparable
    {
      if (null == data)
        throw new ArgumentNullException("data");

			if (end < start)
				throw new ArgumentOutOfRangeException("end", end, "< start");
			if (start < data.GetLowerBound(0))
				throw new ArgumentOutOfRangeException("start", start, "< data.GetLowerBound(0)");
			if (end > data.GetUpperBound(0))
				throw new ArgumentOutOfRangeException("end", end, "> data.GetUpperBound(0)");

			if (data.Length <= 0)
				throw new InvalidOperationException("data.Length ≤ 0");

      T min = data[start];
			for (int i = start + 1; i <= end; i++)
      {
        if (data[i].CompareTo(min) < 0)
          min = data[i];
      }
      return min;
    }

    /// <summary>
    /// Минимальное значение из диапазона от start до конца.
    /// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Минимальное значение.</returns>
		[Pure]
		public static T Min<T>([NotNull] this T[] data,
                            int start) where T : IComparable
    {
      if (null == data)
        throw new ArgumentNullException("data");

			return Min(data, start, data.GetUpperBound(0));
    }

    /// <summary>
    /// Применение функции к каждому элементу массива.
    /// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Исходный массив.</returns>
		[NotNull]
    public static T[] ApplyFun<T>([NotNull] this T[] data, [NotNull] Func<T, T> procFunc)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      if (null == procFunc)
        throw new ArgumentNullException("procFunc");

			for (int i = data.GetLowerBound(0); i <= data.GetUpperBound(0); i++)
        data[i] = procFunc(data[i]);

      return data;
    }

    /// <summary>
    /// Поиск первого индекса, удовлетворяющего логической функции.
    /// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Индекс найденного элемента.</returns>
		[Pure]
		public static int IndexOf<T>([NotNull] this T[] data, [NotNull] Func<T, bool> selectFunc)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      if (null == selectFunc)
        throw new ArgumentNullException("selectFunc");

			for (int i = data.GetLowerBound(0); i <= data.GetUpperBound(0); i++)
      {
        if (selectFunc(data[i]))
          return i;
      }
			return data.GetLowerBound(0) - 1;
    }

    /// <summary>
		/// Поиск первого индекса с конца, удовлетворяющего логической функции.
    /// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Индекс найденного элемента.</returns>
		[Pure]
		public static int LastIndexOf<T>([NotNull] this T[] data, [NotNull] Func<T, bool> selectFunc)
    {
      if (null == data)
        throw new ArgumentNullException("data");

      if (null == selectFunc)
        throw new ArgumentNullException("selectFunc");

			for (int i = data.GetUpperBound(0); i >= data.GetLowerBound(0); i--)
      {
        if (selectFunc(data[i]))
          return i;
      }
			return data.GetLowerBound(0) - 1;
    }

		/// <summary>
		/// Возвращает пустой массив.
		/// </summary>
		/// <typeparam name="T">Тип элементов массива.</typeparam>
		/// <returns>Пустой массив.</returns>
	  [NotNull]
		[Pure]
		public static T[] Empty<T>()
	  {
		  return EmptyArrayStorage<T>.Instance;
	  }

		private static class EmptyArrayStorage<T>
		{
			[NotNull] public static readonly T[] Instance = (Enumerable.Empty<T>() as T[]) ?? new T[] { };
		}
  }
}
