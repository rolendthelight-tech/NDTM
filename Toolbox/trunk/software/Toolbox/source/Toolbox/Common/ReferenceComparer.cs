using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	/// <summary>
	/// Компаратор ссылок.
	/// (Подобие <see cref="System.Dynamic.Utils.ReferenceEqualityComparer`1"/>.)
	/// </summary>
	public sealed class ReferenceComparer : EqualityComparer<object>
	{
		[NotNull] private static readonly ReferenceComparer _cache = new ReferenceComparer();

		private ReferenceComparer()
		{
		}

		/// <summary>
		/// Получение компаратора ссылок.
		/// </summary>
		[NotNull]
		public static ReferenceComparer Get()
		{
			return _cache;
		}

		#region Overrides of EqualityComparer<object>

		public override bool Equals(object x, object y)
		{
			return object.ReferenceEquals(x, y);
		}

		public override int GetHashCode(object obj)
		{
			if (obj == null)
				return 0;
			else
				return RuntimeHelpers.GetHashCode(obj);
		}

		#endregion
	}
}