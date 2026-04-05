using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности дробных чисел.
	/// </summary>
	public static class FloatExtensions
	{
		#region AntiTruncate

		/// <summary>
		/// Округление до целого числа в бо́льшую по модулю сторону.
		/// </summary>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static decimal AntiTruncate(this decimal number)
		{
			return number < 0 ? Math.Floor(number) : Math.Ceiling(number);
		}

		/// <summary>
		/// Округление до целого числа в бо́льшую по модулю сторону.
		/// </summary>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static double AntiTruncate(this double number)
		{
			return number < 0 ? Math.Floor(number) : Math.Ceiling(number);
		}

		#endregion AntiTruncate

		#region Round

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="roundMode">Тип округления.</param>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static decimal Round(this decimal number, RoundMode roundMode)
		{
			switch (roundMode)
			{
				case RoundMode.ToGreat:
					return Math.Ceiling(number);
				case RoundMode.ToLeast:
					return Math.Floor(number);
				case RoundMode.ToInfinity:
					return number.AntiTruncate();
				case RoundMode.ToZero:
					return Math.Truncate(number);
				case RoundMode.ToNearest:
					return Math.Round(number);
				default:
					throw new NotImplementedException(roundMode.ToString());
			}
		}

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="number">Исходное число.</param>
		/// <param name="roundMode">Тип округления.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		private static double Round(this double number, RoundMode roundMode)
		{
			switch (roundMode)
			{
				case RoundMode.ToGreat:
					return Math.Ceiling(number);
				case RoundMode.ToLeast:
					return Math.Floor(number);
				case RoundMode.ToInfinity:
					return number.AntiTruncate();
				case RoundMode.ToZero:
					return Math.Truncate(number);
				case RoundMode.ToNearest:
					return Math.Round(number);
				default:
					throw new NotImplementedException(roundMode.ToString());
			}
		}

		#endregion Round

		#region RoundInt32

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="roundMode">Тип округления.</param>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static Int32 RoundInt32(this decimal number, RoundMode roundMode)
		{
			return (int) number.Round(roundMode);
		}

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="roundMode">Тип округления.</param>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static Int32 RoundInt32(this double number, RoundMode roundMode)
		{
			return (int) number.Round(roundMode);
		}

		#endregion RoundInt32

		#region RoundInt64

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="roundMode">Тип округления.</param>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static Int64 RoundInt64(this decimal number, RoundMode roundMode)
		{
			return (long) number.Round(roundMode);
		}

		/// <summary>
		/// Округление до целого числа.
		/// </summary>
		/// <param name="roundMode">Тип округления.</param>
		/// <param name="number">Исходное число.</param>
		/// <returns>Округлённое число.</returns>
		[Pure]
		public static Int64 RoundInt64(this double number, RoundMode roundMode)
		{
			return (long) number.Round(roundMode);
		}

		#endregion RoundInt64
	}

	/// <summary> Тип округления. </summary>
	public enum RoundMode
	{
		[Obsolete, Browsable(false)]
		None,
		/// <summary> В бо́льшую сторону. </summary>
		ToGreat,
		/// <summary> В меньшую сторону. </summary>
		ToLeast,
		/// <summary> В бо́льшую по модулю сторону. </summary>
		ToInfinity,
		/// <summary> В меньшую по модулю сторону. </summary>
		ToZero,
		/// <summary> К ближайшему. </summary>
		ToNearest,
	}
}
