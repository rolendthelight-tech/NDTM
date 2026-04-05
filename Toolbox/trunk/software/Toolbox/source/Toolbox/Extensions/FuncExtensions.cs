using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности делегатов, возвращающих значение.
	/// </summary>
	public static class FuncExtensions
	{
		/// <summary>
		/// Значение по умолчанию типа, возвращаемого функцией.
		/// </summary>
		/// <typeparam name="T">Тип, возвращаемый функцией.</typeparam>
		/// <param name="func">Функция, из которой только выводится возвращаемый тип.</param>
		/// <returns>Значение по умолчанию типа <see cref="T"/></returns>
		[Pure]
		public static T Default<T>([CanBeNull] this Func<T> func)
		{
			return default(T);
		}
	}
}
