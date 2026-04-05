using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности состояний асинхронной операции.
	/// </summary>
	public static class AsyncResultExtensions
	{
		/// <summary>
		/// Ожидает завершения асинхронной операции <see cref="T:System.IAsyncResult"/>.
		/// </summary>
		/// <param name="asyncResult">Состояние асинхронной операции.</param>
		/// <param name="cancellationToken">Токен отмены ожидания.</param>
		public static void Wait([NotNull] this IAsyncResult asyncResult, CancellationToken cancellationToken)
		{
			if (asyncResult == null) throw new ArgumentNullException("asyncResult");

			cancellationToken.ThrowIfCancellationRequested();
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.ThrowIfCancellationRequested();
				asyncResult.AsyncWaitHandle.Wait(cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();
			}
			else
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
		}
	}
}
