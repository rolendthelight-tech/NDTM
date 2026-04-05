using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	/// <summary>
	/// Интервал.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public interface IInterval<out TLeft, out TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
		/// <summary>
		/// Левая граница интервала. (Может отсутствовать.)
		/// </summary>
		[CanBeNull]
		TLeft Left { get; }

		/// <summary>
		/// Правая граница интервала. (Может отсутствовать.)
		/// </summary>
		[CanBeNull]
		TRight Right { get; }
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public interface ILeftBoundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>
		where TLeft : struct, IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал, обязательно неограниченный слева.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public interface ILeftUnboundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал, обязательно ограниченный справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public interface IRightBoundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : struct, IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал, обязательно неограниченный справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public interface IRightUnboundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public interface IBoundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>, ILeftBoundedInterval<TLeft, TRight>, IRightBoundedInterval<TLeft, TRight>
		where TLeft : struct, IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : struct, IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public interface IUnboundedInterval<out TLeft, out TRight> : IInterval<TLeft, TRight>, ILeftUnboundedInterval<TLeft, TRight>,
		IRightUnboundedInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
	}

	/// <summary>
	/// Интервал.
	/// </summary>
	/// <typeparam name="TRight">Тип левой и правой границ интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public interface IInterval<out T> : IInterval<T, T>
		where T : IComparable<T>, IEquatable<T>
	{
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="T">Тип левой и правой границ интервала.</typeparam>
	public interface IBoundedInterval<out T> : IBoundedInterval<T, T>, IInterval<T>
		where T : struct, IComparable<T>, IEquatable<T>
	{
	}

	/// <summary>
	/// Интервал, обязательно неограниченный слева и справа.
	/// </summary>
	/// <typeparam name="T">Тип левой и правой границ интервала.</typeparam>
	public interface IUnboundedInterval<out T> : IUnboundedInterval<T, T>, IInterval<T>
		where T : IComparable<T>, IEquatable<T>
	{
	}

	/// <summary>
	/// Интервал, необязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public struct UnboundedInterval<TLeft, TRight> : IInterval<TLeft, TRight>, IUnboundedInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
		private readonly TLeft m_left;
		private readonly TRight m_right;

		/// <summary>
		/// Создание интервала, необязательно ограниченного слева и справа.
		/// </summary>
		/// <param name="left">Левая граница (<c><value>null</value></c> если отсутствует).</param>
		/// <param name="right">Правая граница (<c><value>null</value></c> если отсутствует).</param>
		public UnboundedInterval(TLeft left, TRight right)
		{
			if (left != null && right != null)
				if (right.CompareTo(left) < 0)
					throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public TLeft Left
		{
			get { return this.m_left; }
		}

		public TRight Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	/// <summary>
	/// Интервал, необязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой и правой границ интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public struct UnboundedInterval<T> : IInterval<T>, IUnboundedInterval<T>
		where T : IComparable<T>, IEquatable<T>
	{
		private readonly T m_left;
		private readonly T m_right;

		/// <summary>
		/// Создание интервала, необязательно ограниченного слева и справа.
		/// </summary>
		/// <param name="left">Левая граница (<c><value>null</value></c> если отсутствует).</param>
		/// <param name="right">Правая граница (<c><value>null</value></c> если отсутствует).</param>
		public UnboundedInterval(T left, T right)
		{
			if (left != null && right != null)
				if (right.CompareTo(left) < 0)
					throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public T Left
		{
			get { return this.m_left; }
		}

		public T Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	public struct LeftBoundedInterval<TLeft, TRight> : ILeftBoundedInterval<TLeft, TRight>, IRightUnboundedInterval<TLeft, TRight>
		where TLeft : struct, IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
		private readonly TLeft m_left;
		private readonly TRight m_right;

		/// <summary>
		/// Создание интервала, обязательно ограниченного слева.
		/// </summary>
		/// <param name="left">Левая граница.</param>
		/// <param name="right">Правая граница (<c><value>null</value></c> если отсутствует).</param>
		public LeftBoundedInterval(TLeft left, TRight right)
		{
			if (right != null)
				if (right.CompareTo(left) < 0)
					throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public TLeft Left
		{
			get { return this.m_left; }
		}

		public TRight Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	/// <summary>
	/// Интервал, обязательно ограниченный справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала. (Может быть <see cref="T.System.Nullable`1"/>.)</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public struct RightBoundedInterval<TLeft, TRight> : IRightBoundedInterval<TLeft, TRight>, ILeftUnboundedInterval<TLeft, TRight>
		where TLeft : IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : struct, IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
		private readonly TLeft m_left;
		private readonly TRight m_right;

		/// <summary>
		/// Создание интервала, обязательно ограниченного справа.
		/// </summary>
		/// <param name="left">Левая граница (<c><value>null</value></c> если отсутствует).</param>
		/// <param name="right">Правая граница.</param>
		public RightBoundedInterval(TLeft left, TRight right)
		{
			if (left != null)
				if (right.CompareTo(left) < 0)
					throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public TLeft Left
		{
			get { return this.m_left; }
		}

		public TRight Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="TLeft">Тип левой границы интервала.</typeparam>
	/// <typeparam name="TRight">Тип правой границы интервала.</typeparam>
	public struct BoundedInterval<TLeft, TRight> : IBoundedInterval<TLeft, TRight>
		where TLeft : struct, IComparable<TLeft>, IEquatable<TLeft>, IComparable<TRight>, IEquatable<TRight>
		where TRight : struct, IComparable<TRight>, IEquatable<TRight>, IComparable<TLeft>, IEquatable<TLeft>
	{
		private readonly TLeft m_left;
		private readonly TRight m_right;

		/// <summary>
		/// Создание интервала, обязательно ограниченного слева и справа.
		/// </summary>
		/// <param name="left">Левая граница.</param>
		/// <param name="right">Правая граница.</param>
		public BoundedInterval(TLeft left, TRight right)
		{
			if (right.CompareTo(left) < 0)
				throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public TLeft Left
		{
			get { return this.m_left; }
		}

		public TRight Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	/// <summary>
	/// Интервал, обязательно ограниченный слева и справа.
	/// </summary>
	/// <typeparam name="T">Тип левой и правой границ интервала.</typeparam>
	public struct BoundedInterval<T> : IBoundedInterval<T>
		where T : struct, IComparable<T>, IEquatable<T>
	{
		private readonly T m_left;
		private readonly T m_right;

		/// <summary>
		/// Создание интервала, обязательно ограниченного слева и справа.
		/// </summary>
		/// <param name="left">Левая граница.</param>
		/// <param name="right">Правая граница.</param>
		public BoundedInterval(T left, T right)
		{
			if (right.CompareTo(left) < 0)
				throw new ArgumentOutOfRangeException("right", right, "< left");

			this.m_left = left;
			this.m_right = right;
		}

		#region Члены IInterval<T>

		public T Left
		{
			get { return this.m_left; }
		}

		public T Right
		{
			get { return this.m_right; }
		}

		#endregion
	}

	public interface INullableBase<out T>
	where T : struct
	{
		T Value { get; }
	}

	public interface IEquatableNullable<T> : INullableBase<T>, IEquatable<T>, IEquatable<T?>, IEquatable<IEquatableNullable<T>>
		where T : struct, IEquatable<T>
	{
	}

	public interface IComparableNullable<T> : INullableBase<T>, IComparable<T>, IComparable<T?>, IComparable<IComparableNullable<T>>
		where T : struct, IComparable<T>
	{
	}

	public interface IEquatableComparableNullable<T> : INullableBase<T>, IEquatableNullable<T>, IComparableNullable<T>, IEquatable<IEquatableComparableNullable<T>>, IComparable<IEquatableComparableNullable<T>>
		where T : struct, IEquatable<T>, IComparable<T>
	{
	}

	public class ComparableNullable<T> : IComparableNullable<T>, IEquatableNullable<T>, IEquatableComparableNullable<T>
		where T : struct, IComparable<T>, IEquatable<T>
	{
		private readonly T m_value;

		public ComparableNullable(T value)
		{
			this.m_value = value;
		}

		#region Члены INullableBase<T>

		public T Value
		{
			get { return this.m_value; }
		}

		#endregion

		#region Члены IComparable<int>

		public int CompareTo(T other)
		{
			return this.m_value.CompareTo(other);
		}

		#endregion

		#region Члены IComparable<int?>

		public int CompareTo(T? other)
		{
			if (other.HasValue)
				return this.m_value.CompareTo(other.Value);
			else
				return 1;
		}

		#endregion

		#region Члены IComparable<IComparableNullable<T>>

		public int CompareTo(IComparableNullable<T> other)
		{
			if (other != null)
				return this.m_value.CompareTo(other.Value);
			else
				return 1;
		}

		#endregion

		#region Implementation of IComparable<in IEquatableComparableNullable<T>>

		public int CompareTo(IEquatableComparableNullable<T> other)
		{
			return this.CompareTo((IComparableNullable<T>)other);
		}

		#endregion

		#region Члены IEquatable<int>

		public bool Equals(T other)
		{
			return this.m_value.Equals(other);
		}

		#endregion

		#region Члены IEquatable<int?>

		public bool Equals(T? other)
		{
			if (other.HasValue)
				return this.m_value.Equals(other.Value);
			else
				return false;
		}

		#endregion

		#region Члены IEquatable<IEquatableNullable<T>>

		public bool Equals(IEquatableNullable<T> other)
		{
			if (other != null)
				return this.m_value.Equals(other.Value);
			else
				return false;
		}

		#endregion

		#region Implementation of IEquatable<IEquatableComparableNullable<T>>

		public bool Equals(IEquatableComparableNullable<T> other)
		{
			return this.Equals((IEquatableNullable<T>)other);
		}

		#endregion

		[NotNull]
		public static implicit operator ComparableNullable<T>(T value)
		{
			return new ComparableNullable<T>(value);
		}
	}

	public struct ExtendedNumber<T> : IEquatable<ExtendedNumber<T>>, IComparable<ExtendedNumber<T>>, IEquatable<T>, IComparable<T>//, IEquatable<IEquatableNullable<T>>, IComparable<IComparableNullable<T>>
		where T : struct, IEquatable<T>, IComparable<T>
	{
		private readonly ExtensionType m_type;
		private readonly T m_value;

		public ExtendedNumber(T value)
		{
			this.m_value = value;
			this.m_type = ExtensionType.Regular;
		}

		public ExtendedNumber(ExtensionType type)
		{
			switch (type)
			{
				case ExtensionType.Regular:
				case ExtensionType.NegativeInfinity:
				case ExtensionType.PositiveInfinity:
					break;
				default:
					throw new NotImplementedException(type.ToString());
			}

			this.m_value = default(T);
			this.m_type = type;
		}

		public ExtensionType Type
		{
			get { return this.m_type; }
		}

		public T Value
		{
			get
			{
				switch (this.m_type)
				{
					case ExtensionType.Regular:
						return this.m_value;
					case ExtensionType.NegativeInfinity:
					case ExtensionType.PositiveInfinity:
						throw new InvalidOperationException("У особого типа нет значения");
					default:
						throw new NotImplementedException(this.m_type.ToString());
				}
			}
		}

		public static implicit operator ExtendedNumber<T>(T value)
		{
			return new ExtendedNumber<T>(value);
		}

		#region Члены IEquatable<ExtendedNumber<T>>

		public bool Equals(ExtendedNumber<T> other)
		{
			var type = this.Type;
			switch (type)
			{
				case ExtensionType.Regular:
					return type.Equals(other.Type) && this.Value.Equals(other.Value);
				case ExtensionType.NegativeInfinity:
				case ExtensionType.PositiveInfinity:
					return type.Equals(other.Type);
				default:
					throw new NotImplementedException(type.ToString());
			}
		}

		#endregion

		#region Члены IComparable<ExtendedNumber<T>>

		public int CompareTo(ExtendedNumber<T> other)
		{
			var type = this.Type;
			var other_type = other.Type;
			switch (type)
			{
				case ExtensionType.Regular:
					switch (other_type)
					{
						case ExtensionType.NegativeInfinity:
							return +1;
						case ExtensionType.PositiveInfinity:
							return -1;
						case ExtensionType.Regular:
							return this.Value.CompareTo(other.Value);
						default:
							throw new NotImplementedException(other_type.ToString());
					}
				case ExtensionType.NegativeInfinity:
					switch (other_type)
					{
						case ExtensionType.NegativeInfinity:
							return 0;
						case ExtensionType.Regular:
						case ExtensionType.PositiveInfinity:
							return -1;
						default:
							throw new NotImplementedException(other_type.ToString());
					}
				case ExtensionType.PositiveInfinity:
					switch (other_type)
					{
						case ExtensionType.PositiveInfinity:
							return 0;
						case ExtensionType.Regular:
						case ExtensionType.NegativeInfinity:
							return +1;
						default:
							throw new NotImplementedException(other_type.ToString());
					}
				default:
					throw new NotImplementedException(type.ToString());
			}
		}

		#endregion

		#region Члены IEquatable<T>

		public bool Equals(T other)
		{
			var type = this.Type;
			switch (type)
			{
				case ExtensionType.Regular:
					return this.Value.Equals(other);
				case ExtensionType.NegativeInfinity:
				case ExtensionType.PositiveInfinity:
					return false;
				default:
					throw new NotImplementedException(type.ToString());
			}
		}

		#endregion

		#region Члены IComparable<T>

		public int CompareTo(T other)
		{
			var type = this.Type;
			switch (type)
			{
				case ExtensionType.Regular:
					return this.Value.CompareTo(other);
				case ExtensionType.NegativeInfinity:
					return -1;
				case ExtensionType.PositiveInfinity:
					return +1;
				default:
					throw new NotImplementedException(type.ToString());
			}
		}

		#endregion

		public enum ExtensionType : byte
		{
			None,
			Regular,
			NegativeInfinity,
			PositiveInfinity,
		}
	}
}
