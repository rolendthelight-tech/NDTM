using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Список только для чтения. (Заглушка до перехода на .NET 4.5.)
	/// </summary>
	/// <typeparam name="T">Тип элементов.</typeparam>
	public interface IReadOnlyList<out T> : IReadOnlyCollection<T>, IEnumerable<T>
	{
		[Pure]
		T this[int index] { get; }
	}
}
