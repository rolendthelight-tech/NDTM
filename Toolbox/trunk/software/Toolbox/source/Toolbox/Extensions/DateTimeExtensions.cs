using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Tasks;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
  public static class DateTimeExtensions
  {
	  [NotNull]
		[Pure]
		public static string ToISO8601(this DateTime value)
    {
      return value.ToString("yyyyMMddTHHmmss").ThrowIfNull("string");
    }

		#region Average

		/// <summary>
		/// Среднее значение даты.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Среднее значение даты.</returns>
		[Pure]
		public static DateTime Average([NotNull] this IEnumerable<DateTime> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			DateTimeKind? first_kind = null;

			return new DateTime(
				(long) source
					       .ThrowIfDifference<DateTime, DateTimeKind>("Разное основание времени", "source", simpleSelector: d =>
						       {
							       if (first_kind == null)
								       first_kind = d.Kind;
							       return d.Kind;
						       })
					       .Average(d => (decimal) d.Ticks), first_kind.Value);
		}

		/// <summary>
		/// Среднее значение даты.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Среднее значение даты.</returns>
		[Pure]
		public static DateTime? Average([NotNull] this IEnumerable<DateTime?> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			ISuccessResult<DateTimeKind> first_kind = FailResult<DateTimeKind>.Create();

			var ticks = source
				.ThrowIfDifference<DateTime?, DateTimeKind>("Разное основание времени", "source", selector: d =>
					{
						if (d != null)
						{
							var kind = SuccessResult<DateTimeKind>.Create(d.Value.Kind);
							if (!first_kind.Success)
								first_kind = kind;
							return kind;
						}
						else
						{
							return FailResult<DateTimeKind>.Create();
						}
					})
				.Average(d => d == null ? (decimal?) null : d.Value.Ticks);

			return ticks == null ? (DateTime?) null : new DateTime((long) ticks.Value, first_kind.Result);
		}

		/// <summary>
		/// Среднее значение даты.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Среднее значение даты.</returns>
		[Pure]
		public static DateTime Average([NotNull] params DateTime[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Average();
		}

		/// <summary>
		/// Среднее значение даты.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Среднее значение даты.</returns>
		[Pure]
		public static DateTime? Average([NotNull] params DateTime?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Average();
		}

		#endregion Average

		#region Min

		/// <summary>
		/// Минимальная дата.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Минимальная дата.</returns>
		[Pure]
		public static DateTime Min([NotNull] params DateTime[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Min();
		}

		/// <summary>
		/// Минимальная дата.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Минимальная дата.</returns>
		[Pure]
		public static DateTime? Min([NotNull] params DateTime?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Min();
		}

		#endregion Min

		#region Max

		/// <summary>
		/// Максимальная дата.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Максимальная дата.</returns>
		[Pure]
		public static DateTime Max([NotNull] params DateTime[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Max();
		}

		/// <summary>
		/// Максимальная дата.
		/// </summary>
		/// <param name="source">Список дат.</param>
		/// <returns>Максимальная дата.</returns>
		[Pure]
		public static DateTime? Max([NotNull] params DateTime?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Max();
		}

		#endregion Max

		#region Add

		/// <summary>
	  /// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число недель к значению данного экземпляра согласно календарю из заданных региональных параметров.
	  /// </summary>
	  /// <returns>
	  /// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества недель, представленного параметром <paramref name="value"/>.
	  /// </returns>
	  /// <param name="source">Исходное значение.</param>
	  /// <param name="value">Число недель. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
		/// <param name="cultureInfo">Региональные параметры. Если указано значение <code>null</code>, то используются текущие параметры (<see cref="P:System.Globalization.CultureInfo.CurrentCulture"/>).</param>
	  /// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
	  /// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddWeeks(this DateTime source, int value, CultureInfo cultureInfo = null)
		{
			return (cultureInfo ?? CultureInfo.CurrentCulture).Calendar.AddWeeks(source, value);
		}

	  /// <summary>
	  /// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число дней к значению данного экземпляра.
	  /// </summary>
	  /// <returns>
	  /// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества дней, представленного параметром <paramref name="value"/>.
	  /// </returns>
	  /// <param name="source">Исходное значение.</param>
	  /// <param name="value">Число полных и неполных дней. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
	  /// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
	  /// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddDays(this DateTime source, decimal value)
		{
			return source.Add(TimeSpanExtensions.FromDays(value));
		}

		/// <summary>
		/// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число часов к значению данного экземпляра.
		/// </summary>
		/// <returns>
		/// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества часов, представленного параметром <paramref name="value"/>.
		/// </returns>
		/// <param name="source">Исходное значение.</param>
		/// <param name="value">Число полных и неполных часов. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
		/// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddHours(this DateTime source, decimal value)
		{
			return source.Add(TimeSpanExtensions.FromHours(value));
		}

		/// <summary>
		/// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число минут к значению данного экземпляра.
		/// </summary>
		/// <returns>
		/// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества минут, представленного параметром <paramref name="value"/>.
		/// </returns>
		/// <param name="source">Исходное значение.</param>
		/// <param name="value">Число полных и неполных минут. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
		/// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddMinutes(this DateTime source, decimal value)
		{
			return source.Add(TimeSpanExtensions.FromMinutes(value));
		}

		/// <summary>
		/// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число секунд к значению данного экземпляра.
		/// </summary>
		/// <returns>
		/// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества секунд, представленного параметром <paramref name="value"/>.
		/// </returns>
		/// <param name="source">Исходное значение.</param>
		/// <param name="value">Число полных и неполных секунд. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
		/// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddSeconds(this DateTime source, decimal value)
		{
			return source.Add(TimeSpanExtensions.FromSeconds(value));
		}

		/// <summary>
		/// Возвращает новый объект <see cref="T:System.DateTime"/>, добавляющий заданное число миллисекунд к значению данного экземпляра.
		/// </summary>
		/// <returns>
		/// Объект, значение которого равно сумме даты и времени, представленных текущим экземпляром, и количества миллисекунд, представленного параметром <paramref name="value"/>.
		/// </returns>
		/// <param name="source">Исходное значение.</param>
		/// <param name="value">Число полных и неполных миллисекунд. Параметр <paramref name="value"/> может быть положительным или отрицательным.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Результирующий объект <see cref="T:System.DateTime"/> меньше, чем <see cref="F:System.DateTime.MinValue"/> или больше, чем <see cref="F:System.DateTime.MaxValue"/>.</exception>
		/// <filterpriority>2</filterpriority>
		[Pure]
		public static DateTime AddMilliseconds(this DateTime source, decimal value)
		{
			return source.Add(TimeSpanExtensions.FromMilliseconds(value));
		}

		#endregion Add
	}
}
