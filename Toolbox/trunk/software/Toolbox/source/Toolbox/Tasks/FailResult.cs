using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Tasks
{
	/// <summary>
	/// Неуспешный результат.
	/// </summary>
	/// <typeparam name="TResult">Тип значения результата.</typeparam>
	public class FailResult<TResult> : ISuccessResult<TResult>, IEquatable<FailResult<TResult>>
	{
		/// <summary>
		/// Неуспешный результат.
		/// </summary>
		[Pure]
		public FailResult()
		{
		}

		[NotNull] private static readonly FailResult<TResult> _value = new FailResult<TResult>();

		/// <summary>
		/// Неуспешный результат.
		/// </summary>
		[NotNull]
		[Pure]
		public static FailResult<TResult> Create()
		{
			return _value;
		}

		#region Implementation of IResultable<out TResult>

		public TResult Result
		{
			get { throw new InvalidOperationException("Результата нет"); }
		}

		#endregion

		#region Implementation of ISuccessable

		public bool Success { get { return false; } }

		#endregion

		#region Equality members

		public override bool Equals(object obj)
		{
			return Equals((FailResult<TResult>)obj);
		}

		public bool Equals(FailResult<TResult> other)
		{
			return other != null && this.GetType() == other.GetType();
		}

		public override int GetHashCode()
		{
			return 1580479969;
		}

		#endregion
	}
}
