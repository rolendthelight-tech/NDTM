using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности генератора псевдослучайных чисел.
	/// </summary>
	public static class RandomExtensions
	{
		[ThreadStatic] [CanBeNull] private static byte[] _uint32_buffer_cache;
		[ThreadStatic] [CanBeNull] private static byte[] _uint64_buffer_cache;

		private static UInt16 JoiningUInt8(byte x, byte y)
		{
			return (UInt16)(((UInt16)(((UInt16)x) & (UInt16)(0xFFu))) | ((UInt16)(((UInt16)(((UInt16)y) & (UInt16)(0xFFu))) << 8)));
		}

		private static UInt32 JoiningUInt16(UInt16 x, UInt16 y)
		{
			return (UInt32)(((UInt32)(((UInt32)x) & (UInt32)(0xFFFFu))) | ((UInt32)(((UInt32)(((UInt32)y) & (UInt32)(0xFFFFu))) << 16)));
		}

		private static UInt64 JoiningUInt32(UInt32 x, UInt32 y)
		{
			return (UInt64)(((UInt64)(((UInt64)x) & (UInt64)(0xFFFFFFFFu))) | ((UInt64)(((UInt64)(((UInt64)y) & (UInt64)(0xFFFFFFFFu))) << 32)));
		}

		/// <summary>
		/// Возвращает случайное число из всего возможного диапазона <see cref="T:System.UInt32"/>.
		/// </summary>
		/// <param name="random">Генератор псевдослучайных чисел.</param>
		/// <returns>Псевдослучайное число.</returns>
		[CanBeNull]
		public static UInt32 NextUInt32([NotNull] this Random random)
		{
			if (random == null) throw new ArgumentNullException("random");

			var buffer = _uint32_buffer_cache ?? new byte[4];
			_uint32_buffer_cache = null;
			random.NextBytes(buffer);
			UInt32 number = JoiningUInt16(JoiningUInt8(buffer[0], buffer[1]), JoiningUInt8(buffer[2], buffer[3]));
			Array.Clear(buffer, buffer.GetLowerBound(0), buffer.Length);
			_uint32_buffer_cache = buffer;
			return number;
		}

		/// <summary>
		/// Возвращает случайное число из всего возможного диапазона <see cref="T:System.Int32"/>.
		/// </summary>
		/// <param name="random">Генератор псевдослучайных чисел.</param>
		/// <returns>Псевдослучайное число.</returns>
		[CanBeNull]
		public static Int32 NextInt32([NotNull] this Random random)
		{
			return unchecked((Int32)NextUInt32(random));
		}

		/// <summary>
		/// Возвращает случайное число из всего возможного диапазона <see cref="T:System.UInt64"/>.
		/// </summary>
		/// <param name="random">Генератор псевдослучайных чисел.</param>
		/// <returns>Псевдослучайное число.</returns>
		[CanBeNull]
		public static UInt64 NextUInt64([NotNull] this Random random)
		{
			if (random == null) throw new ArgumentNullException("random");

			var buffer = _uint64_buffer_cache ?? new byte[8];
			_uint64_buffer_cache = null;
			random.NextBytes(buffer);
			UInt64 number = JoiningUInt32(JoiningUInt16(JoiningUInt8(buffer[0], buffer[1]), JoiningUInt8(buffer[2], buffer[3])), JoiningUInt16(JoiningUInt8(buffer[4], buffer[5]), JoiningUInt8(buffer[6], buffer[7])));
			Array.Clear(buffer, buffer.GetLowerBound(0), buffer.Length);
			_uint64_buffer_cache = buffer;
			return number;
		}

		/// <summary>
		/// Возвращает случайное число из всего возможного диапазона <see cref="T:System.Int64"/>.
		/// </summary>
		/// <param name="random">Генератор псевдослучайных чисел.</param>
		/// <returns>Псевдослучайное число.</returns>
		[CanBeNull]
		public static Int64 NextInt64([NotNull] this Random random)
		{
			return unchecked((Int64)NextUInt64(random));
		}
	}
}
