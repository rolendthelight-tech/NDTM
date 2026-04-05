using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности <see cref="T:System.TimeSpan"/>.
	/// </summary>
	public static class TimeSpanExtensions
	{
		/// <summary>
		/// Нулевой интервал времени.
		/// </summary>
		public static TimeSpan Zero
		{
			get { return TimeSpan.Zero; }
		}

		private static readonly TimeSpan _epsilon = TimeSpan.FromTicks(1L);

		/// <summary>
		/// Наименьший интервал времени больше нулевого.
		/// </summary>
		[Obsolete("Предназначен для приблизительного сравнения значений, которые могут быть сохранены с большей точностью. Используйте метод EqualsApproximately вместо этого значения.")]
		public static TimeSpan Epsilon
		{
			get { return _epsilon; }
		}

		/// <summary>
		/// Сумма интервала времени и <see cref="_epsilon"/>.
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <returns>Сумма интервала времени и <see cref="_epsilon"/>.</returns>
		[Pure]
		private static TimeSpan AddEpsilon(this TimeSpan value)
		{
			return value + _epsilon;
		}

		/// <summary>
		/// Половина интервала времени.
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <returns>Половина интервала времени.</returns>
		[Pure]
		public static TimeSpan Half(this TimeSpan value)
		{
			return value.Divide(2);
		}

		#region RoundTicks

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="ticks">Исходное число.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		private static long RoundTicks(this decimal ticks, RoundMode roundMode)
		{
			return ticks.RoundInt64(roundMode);
		}

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="ticks">Исходное число.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		private static long RoundTicks(this double ticks, RoundMode roundMode)
		{
			return ticks.RoundInt64(roundMode);
		}

		#endregion RoundTicks

		#region Multiply

		/// <summary>
		/// Интервал времени, увеличенный в заданное число раз.
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="multiplier">Множитель.</param>
		/// <returns>Увеличенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, int multiplier)
		{
			return TimeSpan.FromTicks(value.Ticks * multiplier);
		}

		[Obsolete("Округление не требуется. Эта перегрузка функции работает как Multiply без типа округления.")]
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, int multiplier, RoundMode roundMode)
		{
			return value.Multiply(multiplier);
		}

		/// <summary>
		/// Интервал времени, увеличенный в заданное число раз.
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="multiplier">Множитель.</param>
		/// <returns>Увеличенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, long multiplier)
		{
			return TimeSpan.FromTicks(value.Ticks * multiplier);
		}

		[Obsolete("Округление не требуется. Эта перегрузка функции работает как Multiply без типа округления.")]
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, long multiplier, RoundMode roundMode)
		{
			return value.Multiply(multiplier);
		}

		/// <summary>
		/// Интервал времени, увеличенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="multiplier">Множитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Увеличенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, decimal multiplier, RoundMode roundMode = RoundMode.ToNearest)
		{
			return TimeSpan.FromTicks((value.Ticks*multiplier).RoundTicks(roundMode));
		}

		/// <summary>
		/// Интервал времени, увеличенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="multiplier">Множитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Увеличенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Multiply(this TimeSpan value, double multiplier, RoundMode roundMode = RoundMode.ToNearest)
		{
			return TimeSpan.FromTicks((value.Ticks * multiplier).RoundTicks(roundMode));
		}

		#endregion Multiply

		#region Divide

		/// <summary>
		/// Интервал времени, уменьшенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="divisor">Делитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Уменьшенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Divide(this TimeSpan value, int divisor, RoundMode roundMode = RoundMode.ToNearest)
		{
			return value.Divide((long) divisor, roundMode);
		}

		/// <summary>
		/// Интервал времени, уменьшенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="divisor">Делитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Уменьшенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Divide(this TimeSpan value, long divisor, RoundMode roundMode = RoundMode.ToNearest)
		{
			long remainder;
			var result = Math.DivRem(value.Ticks, divisor, out remainder);
			if (remainder != 0)
				switch (roundMode)
				{
					case RoundMode.ToGreat:
						if (divisor == 0)
							throw new ApplicationException("Ошибка в делении");
						else if (value.Ticks != 0)
						{
							if (!((value.Ticks > 0) ^ (divisor > 0)))
								result += 1;
						}
						break;
					case RoundMode.ToLeast:
						if (divisor == 0)
							throw new ApplicationException("Ошибка в делении");
						else if (value.Ticks != 0)
						{
							if ((value.Ticks > 0) ^ (divisor > 0))
								result -= 1;
						}
						break;
					case RoundMode.ToInfinity:
						if (divisor == 0)
							throw new ApplicationException("Ошибка в делении");
						else if (value.Ticks != 0)
						{
							if ((value.Ticks > 0) ^ (divisor > 0))
								result -= 1;
							else
								result += 1;
						}
						break;
					case RoundMode.ToZero:
						break;
					case RoundMode.ToNearest:
						if (Math.Abs(remainder) >= Math.Abs(((divisor/2) + (divisor%2))))
							if (divisor == 0)
								throw new ApplicationException("Ошибка в делении");
							else if (value.Ticks != 0)
							{
								if ((value.Ticks > 0) ^ (divisor > 0))
									result -= 1;
								else
									result += 1;
							}
						break;
					default:
						throw new NotImplementedException(roundMode.ToString());
				}
			return TimeSpan.FromTicks(result);
		}

		/// <summary>
		/// Интервал времени, уменьшенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="divisor">Делитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Уменьшенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Divide(this TimeSpan value, decimal divisor, RoundMode roundMode = RoundMode.ToNearest)
		{
			return TimeSpan.FromTicks((value.Ticks / divisor).RoundTicks(roundMode));
		}

		/// <summary>
		/// Интервал времени, уменьшенный в заданное число раз (округление в указанную сторону).
		/// </summary>
		/// <param name="value">Интервал времени.</param>
		/// <param name="divisor">Делитель.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Уменьшенный интервал времени.</returns>
		[Pure]
		public static TimeSpan Divide(this TimeSpan value, double divisor, RoundMode roundMode = RoundMode.ToNearest)
		{
			return TimeSpan.FromTicks((value.Ticks / divisor).RoundTicks(roundMode));
		}

		/// <summary>
		/// Отношение интервалов времени.
		/// </summary>
		/// <param name="value">Делимое.</param>
		/// <param name="divisor">Делитель.</param>
		/// <returns>Отношение интервалов времени.</returns>
		[Pure]
		public static decimal DivideDecimal(this TimeSpan value, TimeSpan divisor)
		{
			return ((decimal)value.Ticks) / ((decimal)divisor.Ticks);
		}

		/// <summary>
		/// Отношение интервалов времени.
		/// </summary>
		/// <param name="value">Делимое.</param>
		/// <param name="divisor">Делитель.</param>
		/// <returns>Отношение интервалов времени.</returns>
		[Pure]
		public static double Divide(this TimeSpan value, TimeSpan divisor)
		{
			return ((double)value.Ticks) / ((double)divisor.Ticks);
		}

		#endregion Divide

		#region Remainder

		/// <summary>
		/// Остаток деления интервалов времени.
		/// </summary>
		/// <param name="value">Делимое.</param>
		/// <param name="divisor">Делитель.</param>
		/// <returns>Остаток деления интервалов времени.</returns>
		[Pure]
		public static TimeSpan Remainder(this TimeSpan value, TimeSpan divisor)
		{
			long remainder = value.Ticks%divisor.Ticks;
			return TimeSpan.FromTicks(remainder);
		}

		#endregion Remainder

		#region DivRem

		/// <summary>
		/// Деление интервалов времени с остатком.
		/// </summary>
		/// <param name="value">Делимое.</param>
		/// <param name="divisor">Делитель.</param>
		/// <param name="remainder">Остаток.</param>
		/// <returns>Частное.</returns>
		[Pure]
		public static long DivRem(this TimeSpan value, TimeSpan divisor, out TimeSpan remainder)
		{
			long r;
			var q = Math.DivRem(value.Ticks, divisor.Ticks, out r);
			remainder = TimeSpan.FromTicks(r);
			return q;
		}

		#endregion DivRem

		#region TotalSeconds

		[Pure]
		public static decimal TotalSecondsDecimal(this TimeSpan value)
		{
			return value.Ticks / (decimal)TimeSpan.TicksPerSecond;
		}

		[Pure]
		public static decimal TotalMillisecondsDecimal(this TimeSpan value)
		{
			return value.Ticks / (decimal)TimeSpan.TicksPerMillisecond;
		}

		[Pure]
		public static decimal TotalMinutesDecimal(this TimeSpan value)
		{
			return value.Ticks / (decimal)TimeSpan.TicksPerMinute;
		}

		[Pure]
		public static decimal TotalHoursDecimal(this TimeSpan value)
		{
			return value.Ticks / (decimal)TimeSpan.TicksPerHour;
		}

		[Pure]
		public static decimal TotalDaysDecimal(this TimeSpan value)
		{
			return value.Ticks / (decimal)TimeSpan.TicksPerDay;
		}

		#endregion TotalSeconds

		#region FromSeconds

		/// <summary>
		/// Интервал времени, представляющий указанное количество секунд.
		/// </summary>
		/// <param name="value">Количество секунд.</param>
		/// <returns>Интервал времени.</returns>
		[Obsolete("Рекомендуется использовать метод, принимающий decimal во избежание лишних округлений")]
		[Pure]
		public static TimeSpan FromSeconds(double value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество секунд.
		/// </summary>
		/// <param name="value">Количество секунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromSeconds(decimal value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество секунд.
		/// </summary>
		/// <param name="value">Количество секунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromSeconds(int value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество секунд.
		/// </summary>
		/// <param name="value">Количество секунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromSeconds(long value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerSecond));
		}

		#endregion FromSeconds

		#region FromMilliseconds

		/// <summary>
		/// Интервал времени, представляющий указанное количество миллисекунд.
		/// </summary>
		/// <param name="value">Количество миллисекунд.</param>
		/// <returns>Интервал времени.</returns>
		[Obsolete("Рекомендуется использовать метод, принимающий decimal во избежание лишних округлений")]
		[Pure]
		public static TimeSpan FromMilliseconds(double value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество миллисекунд.
		/// </summary>
		/// <param name="value">Количество миллисекунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMilliseconds(decimal value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество миллисекунд.
		/// </summary>
		/// <param name="value">Количество миллисекунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMilliseconds(int value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество миллисекунд.
		/// </summary>
		/// <param name="value">Количество миллисекунд.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMilliseconds(long value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMillisecond));
		}

		#endregion FromMilliseconds

		#region FromMinutes

		/// <summary>
		/// Интервал времени, представляющий указанное количество минут.
		/// </summary>
		/// <param name="value">Количество минут.</param>
		/// <returns>Интервал времени.</returns>
		[Obsolete("Рекомендуется использовать метод, принимающий decimal во избежание лишних округлений")]
		[Pure]
		public static TimeSpan FromMinutes(double value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMinute));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество минут.
		/// </summary>
		/// <param name="value">Количество минут.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMinutes(decimal value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMinute));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество минут.
		/// </summary>
		/// <param name="value">Количество минут.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMinutes(int value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMinute));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество минут.
		/// </summary>
		/// <param name="value">Количество минут.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromMinutes(long value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerMinute));
		}

		#endregion FromMinutes

		#region FromHours

		/// <summary>
		/// Интервал времени, представляющий указанное количество часов.
		/// </summary>
		/// <param name="value">Количество часов.</param>
		/// <returns>Интервал времени.</returns>
		[Obsolete("Рекомендуется использовать метод, принимающий decimal во избежание лишних округлений")]
		[Pure]
		public static TimeSpan FromHours(double value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerHour));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество часов.
		/// </summary>
		/// <param name="value">Количество часов.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromHours(decimal value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerHour));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество часов.
		/// </summary>
		/// <param name="value">Количество часов.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromHours(int value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerHour));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество часов.
		/// </summary>
		/// <param name="value">Количество часов.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromHours(long value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerHour));
		}

		#endregion FromHours

		#region FromDays

		/// <summary>
		/// Интервал времени, представляющий указанное количество дней.
		/// </summary>
		/// <param name="value">Количество дней.</param>
		/// <returns>Интервал времени.</returns>
		[Obsolete("Рекомендуется использовать метод, принимающий decimal во избежание лишних округлений")]
		[Pure]
		public static TimeSpan FromDays(double value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerDay));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество дней.
		/// </summary>
		/// <param name="value">Количество дней.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromDays(decimal value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerDay));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество дней.
		/// </summary>
		/// <param name="value">Количество дней.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromDays(int value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerDay));
		}

		/// <summary>
		/// Интервал времени, представляющий указанное количество дней.
		/// </summary>
		/// <param name="value">Количество дней.</param>
		/// <returns>Интервал времени.</returns>
		[Pure]
		public static TimeSpan FromDays(long value)
		{
			return TimeSpan.FromTicks((long)(value * TimeSpan.TicksPerDay));
		}

		#endregion FromDays

		#region Round

		/// <summary>
		/// Округление интервала времени по заданному шагу.
		/// </summary>
		/// <param name="value">Делимое.</param>
		/// <param name="divisor">Делитель.</param>
		/// <returns>Округлённый интервал времени.</returns>
		[Pure]
		public static TimeSpan Round(this TimeSpan value, TimeSpan divisor)
		{
			return TimeSpan.FromTicks(value.Ticks - (value.Ticks%divisor.Ticks));
		}

		[Pure]
		public static TimeSpan Round(this TimeSpan value, int digits, MidpointRounding mode)
		{
			throw new NotImplementedException();
		}

		#endregion Round

		#region EqualsApproximately

		/// <summary>
		/// Приблизительное сравнение интервалов, сохранённых с большей точностью.
		/// </summary>
		/// <param name="value">Первый интервал времени.</param>
		/// <param name="other">Второй интервал времени.</param>
		/// <returns>Равны с точностью <see cref="_epsilon"/>.</returns>
		[Pure]
		public static bool EqualsApproximately(this TimeSpan value, TimeSpan other)
		{
			var value2 = value.AddEpsilon();
			var other2 = other.AddEpsilon();
			return (value <= other && other < value2) || (value < other2 && other2 <= value2);
		}

		[Pure]
		public static bool EqualsApproximately(this TimeSpan value, TimeSpan other, int digits, MidpointRounding mode)
		{
			throw new NotImplementedException();
		}

		#endregion EqualsApproximately

		#region Sum

		/// <summary>
		/// Сумма интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Сумма интервалов времени.</returns>
		[Pure]
		public static TimeSpan Sum([NotNull] this IEnumerable<TimeSpan> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return TimeSpan.FromTicks(source.Sum(t => t.Ticks));
		}

		/// <summary>
		/// Сумма интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Сумма интервалов времени.</returns>
		[Pure]
		public static TimeSpan? Sum([NotNull] this IEnumerable<TimeSpan?> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var ticks = source.Sum(t => t == null ? (long?) null : t.Value.Ticks);

			return ticks == null ? (TimeSpan?) null : TimeSpan.FromTicks(ticks.Value);
		}

		/// <summary>
		/// Сумма интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Сумма интервалов времени.</returns>
		[Pure]
		public static TimeSpan Sum([NotNull] params TimeSpan[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Sum();
		}

		/// <summary>
		/// Сумма интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Сумма интервалов времени.</returns>
		[Pure]
		public static TimeSpan? Sum([NotNull] params TimeSpan?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Sum();
		}

		#endregion Sum

		#region Average

		/// <summary>
		/// Среднее значение интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Среднее значение интервалов времени.</returns>
		[Pure]
		public static TimeSpan Average([NotNull] this IEnumerable<TimeSpan> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return TimeSpan.FromTicks((long) source.Average(t => (decimal) t.Ticks));
		}

		/// <summary>
		/// Среднее значение интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Среднее значение интервалов времени.</returns>
		[Pure]
		public static TimeSpan? Average([NotNull] this IEnumerable<TimeSpan?> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var ticks = source.Average(t => t == null ? (decimal?) null : t.Value.Ticks);

			return ticks == null ? (TimeSpan?) null : TimeSpan.FromTicks((long) ticks.Value);
		}

		/// <summary>
		/// Среднее значение интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Среднее значение интервалов времени.</returns>
		[Pure]
		public static TimeSpan Average([NotNull] params TimeSpan[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Average();
		}

		/// <summary>
		/// Среднее значение интервалов времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Среднее значение интервалов времени.</returns>
		[Pure]
		public static TimeSpan? Average([NotNull] params TimeSpan?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Average();
		}

		#endregion Average

		#region Min

		/// <summary>
		/// Минимальный интервал времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Минимальный интервал времени.</returns>
		[Pure]
		public static TimeSpan Min([NotNull] params TimeSpan[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Min();
		}

		/// <summary>
		/// Минимальный интервал времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Минимальный интервал времени.</returns>
		[Pure]
		public static TimeSpan? Min([NotNull] params TimeSpan?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Min();
		}

		#endregion Min

		#region Max

		/// <summary>
		/// Максимальный интервал времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Максимальный интервал времени.</returns>
		[Pure]
		public static TimeSpan Max([NotNull] params TimeSpan[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Max();
		}

		/// <summary>
		/// Максимальный интервал времени.
		/// </summary>
		/// <param name="source">Список интервалов времени.</param>
		/// <returns>Максимальный интервал времени.</returns>
		[Pure]
		public static TimeSpan? Max([NotNull] params TimeSpan?[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return source.Max();
		}

		#endregion Max
	}
}
