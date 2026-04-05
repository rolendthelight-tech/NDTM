using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Data
{
	/// <summary>
	/// Ссылка на ресурс из пула.
	/// Для возвращения ресурса в пул следует вызвать <see cref="IDisposable.Dispose()"/> 1 раз.
	/// </summary>
	/// <typeparam name="TResource">Тип ресурса.</typeparam>
	public interface IResourceLink<out TResource> : IDisposable
	{
		/// <summary>
		/// Ресурс.
		/// Создаётся и уничтожается пулом.
		/// </summary>
		[NotNull]
		TResource Resource { get; }
	}
}
