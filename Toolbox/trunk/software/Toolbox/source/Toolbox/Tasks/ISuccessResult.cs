using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Tasks
{
	/// <summary>
	/// Интерфейс результата, который может быть успешным или неуспешным (у неуспешного результата не может быть значения).
	/// </summary>
	/// <typeparam name="TResult">Тип результата.</typeparam>
	public interface ISuccessResult<out TResult> : IResultable<TResult>, ISuccessable
	{
	}
}
