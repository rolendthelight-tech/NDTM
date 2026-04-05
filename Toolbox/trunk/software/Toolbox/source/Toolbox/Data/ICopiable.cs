using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Поддерживает копирование, при котором создаётся новый экземпляр класса с тем же значением, что и у существующего экземпляра.
	/// Аналог <see cref="T:System.ICloneable"/>.
	/// </summary>
	/// <typeparam name="T">Тип элемента.</typeparam>
	/// <filterpriority>2</filterpriority>
	public interface ICopiable<out T>
		where T : ICopiable<T>
	{
		/// <summary>
		/// Создаёт новый объект, являющийся копией текущего экземпляра.
		/// </summary>
		/// <returns>
		/// Новый объект, являющийся копией этого экземпляра.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		[NotNull]
		T Copy();
	}
}
