using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Common;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Tasks
{
	/// <summary>
	/// Успешный результат.
	/// </summary>
	/// <typeparam name="TResult">Тип значения результата.</typeparam>
	public class SuccessResult<TResult> : ISuccessResult<TResult>, IEquatable<SuccessResult<TResult>>
	{
		private readonly TResult m_result;

		/// <summary>
		/// Успешный результат.
		/// </summary>
		/// <param name="result">Результат.</param>
		[Pure]
		public SuccessResult(TResult result)
		{
			this.m_result = result;
		}

		/// <summary>
		/// Успешный результат.
		/// </summary>
		/// <param name="result">Результат.</param>
		[NotNull]
		[Pure]
		public static SuccessResult<TResult> Create(TResult result)
		{
			return new SuccessResult<TResult>(result);
		}

		#region Implementation of IResultable<out TResult>

		public TResult Result
		{
			get { return m_result; }
		}

		#endregion

		#region Implementation of ISuccessable

		public bool Success { get { return true; } }

		#endregion

		#region Equality members

		public override bool Equals(object obj)
		{
			return Equals((SuccessResult<TResult>)obj);
		}

		public bool Equals(SuccessResult<TResult> other)
		{
			return other != null && this.GetType() == other.GetType() && EqualityComparer<TResult>.Default.Equals(m_result, other.m_result);
		}

		public override int GetHashCode()
		{
			return GetHashCodeHelper.CombineHashCodesCustom(2014972741, 1221590089, EqualityComparer<TResult>.Default.GetHashCode, m_result);
		}

		#endregion
	}
}
