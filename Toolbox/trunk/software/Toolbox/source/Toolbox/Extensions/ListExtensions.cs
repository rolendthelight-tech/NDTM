using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	public static class ListExtensions
	{
		[NotNull]
		[Pure]
		public static IEnumerable<T> ReverseEnumerable<T>([NotNull] this IList<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			using (var enumerator = source.GetEnumerator())
			{
				for (int i = source.Count - 1; i >= 0; --i)
				{
					T value = source[i];
					if (enumerator.MoveNext())
					{
						yield return value;
					}
					else
					{
						throw new InvalidOperationException("Список изменён");
					}
				}
				if (enumerator.MoveNext())
				{
					throw new InvalidOperationException("Список изменён");
				}
				else
				{
					yield break;
				}
			}
		}

		[NotNull]
		[Pure]
		public static IEnumerable ReverseEnumerable([NotNull] this IList source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var enumerator = source.GetEnumerator();
			{
				for (int i = source.Count - 1; i >= 0; --i)
				{
					object value = source[i];
					if (enumerator.MoveNext())
					{
						yield return value;
					}
					else
					{
						throw new InvalidOperationException("Список изменён");
					}
				}
				if (enumerator.MoveNext())
				{
					throw new InvalidOperationException("Список изменён");
				}
				else
				{
					yield break;
				}
			}
		}

		[NotNull]
		[Pure]
		public static IEnumerable<T> ReverseEnumerable<T>([NotNull] this T[] source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return ((IList<T>) source).ReverseEnumerable();
		}
	}
}
