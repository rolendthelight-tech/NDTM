using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Коллекция только для чтения. (Заглушка до перехода на .NET 4.5.)
	/// </summary>
	/// <typeparam name="T">Тип элементов.</typeparam>
	public interface IReadOnlyCollection<out T> : IEnumerable<T>
	{
		/// <summary>
		/// Получает число элементов, содержащихся в интерфейсе <see cref="T:Toolbox.Data.IReadOnlyCollection`1"/>.
		/// </summary>
		/// <returns>
		/// Число элементов, содержащихся в интерфейсе <see cref="T:Toolbox.Data.IReadOnlyCollection`1"/>.
		/// </returns>
		[Pure]
		int Count { get; }
	}
}
