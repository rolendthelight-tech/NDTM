using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Common.Constants;
using Toolbox.Extensions;

namespace Toolbox.Common
{
	/// <summary>
	/// Компаратор с селектором сравниваемой части.
	/// </summary>
	public sealed class SelectEqualityComparer<TElement, TValue> : EqualityComparer<TElement>
	{
		[NotNull] private readonly Func<TElement, TValue> m_selector;
		[NotNull] private readonly IEqualityComparer<TValue> m_equality_comparer;

		/// <summary>
		/// Создаёт компаратор с селектором сравниваемой части, сравниваемой компаратором по умолчанию.
		/// </summary>
		/// <param name="selector">Селектор сравниваемой части.</param>
		public SelectEqualityComparer([NotNull] Func<TElement, TValue> selector)
			: this(selector, EqualityComparer<TValue>.Default)
		{
		}

		/// <summary>
		/// Создаёт компаратор с селектором сравниваемой части.
		/// </summary>
		/// <param name="selector">Селектор сравниваемой части.</param>
		/// <param name="equalityComparer">Компаратор выбранной части.</param>
		public SelectEqualityComparer([NotNull] Func<TElement, TValue> selector, [NotNull] IEqualityComparer<TValue> equalityComparer)
		{
			if (selector == null) throw new ArgumentNullException("selector");
			if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");

			this.m_selector = selector;
			this.m_equality_comparer = equalityComparer;
		}

		#region Overrides of EqualityComparer<TElement>

		public override bool Equals(TElement x, TElement y)
		{
			return this.m_equality_comparer.Equals(this.m_selector(x), this.m_selector(y));
		}

		public override int GetHashCode(TElement obj)
		{
			return this.m_equality_comparer.GetHashCode(this.m_selector(obj));
		}

		#endregion
	}
}