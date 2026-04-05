using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	/// <summary>
	/// Расширение функциональности делегатов.
	/// </summary>
	public static class ActionExtensions
	{
		#region InvokeAll

		/// <summary>
		/// Последовательно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями, если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void InvokeAll([NotNull] this IEnumerable<Action> actions, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			var exceptions = actions
				.Select(action => action.InvokeException())
				.Where(ex => ex != null);

			var aggregate_exception = message == null ? new AggregateException(exceptions) : new AggregateException(message, exceptions);

			if (aggregate_exception.InnerExceptions.Any())
				throw aggregate_exception;
		}

		/// <summary>
		/// Последовательно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями (кроме <see cref="T:System.OperationCanceledException"/>), если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.OperationCanceledException">Если хотя бы 1 делегат бросил это исключение.</exception>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void InvokeAll([NotNull] this IEnumerable<Action<CancellationToken>> actions, CancellationToken cancellationToken, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			cancellationToken.ThrowIfCancellationRequested();

			var exceptions = actions
				.Select(action => action.InvokeException(cancellationToken))
				.Where(ex => ex != null);

			cancellationToken.ThrowIfCancellationRequested();

			var aggregate_exception = message == null ? new AggregateException(exceptions) : new AggregateException(message, exceptions);

			if (aggregate_exception.InnerExceptions.Any())
				throw aggregate_exception;
		}

		#endregion InvokeAll

		#region ParallelInvokeAll

		/// <summary>
		/// Параллельно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями, если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void ParallelInvokeAll([NotNull] this IEnumerable<Action> actions, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			var exceptions = actions
				.AsParallel()
				.AsOrdered()
				.Select(action => action.InvokeException())
				.Where(ex => ex != null);

			var aggregate_exception = message == null ? new AggregateException(exceptions) : new AggregateException(message, exceptions);

			if (aggregate_exception.InnerExceptions.Any())
				throw aggregate_exception;
		}

		/// <summary>
		/// Параллельно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями (кроме <see cref="T:System.OperationCanceledException"/>), если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.OperationCanceledException">Если хотя бы 1 делегат бросил это исключение.</exception>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void ParallelInvokeAll([NotNull] this IEnumerable<Action<CancellationToken>> actions, CancellationToken cancellationToken, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			cancellationToken.ThrowIfCancellationRequested();

			var exceptions = actions
				.AsParallel()
				.AsOrdered()
				.WithCancellation(cancellationToken)
				.Select(action => action.InvokeException(cancellationToken))
				.Where(ex => ex != null);

			cancellationToken.ThrowIfCancellationRequested();

			var aggregate_exception = message == null ? new AggregateException(exceptions) : new AggregateException(message, exceptions);

			if (aggregate_exception.InnerExceptions.Any())
				throw aggregate_exception;
		}

		#endregion ParallelInvokeAll

		#region InvokeException

		/// <summary>
		/// Вызывает делегат и возвращает исключение, которое он бросил, если он бросил, иначе — <code>null</code>.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="action">Вызываемый делегат.</param>
		/// <returns>Исключение, брошенное делегатом.</returns>
		[CanBeNull]
		public static Exception InvokeException([NotNull] this Action action)
		{
			if (action == null) throw new ArgumentNullException("action");

			try
			{
				action.Invoke();
			}
			catch (Exception ex)
			{
				return ex;
			}

			return null;
		}

		/// <summary>
		/// Вызывает делегат и возвращает исключение, которое он бросил, если он бросил не <see cref="T:System.OperationCanceledException"/>, иначе — <code>null</code>.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="action">Вызываемый делегат.</param>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <exception cref="T:System.OperationCanceledException">Если делегат бросил это исключение.</exception>
		/// <returns>Исключение, брошенное делегатом.</returns>
		[CanBeNull]
		public static Exception InvokeException([NotNull] this Action<CancellationToken> action, CancellationToken cancellationToken)
		{
			if (action == null) throw new ArgumentNullException("action");

			try
			{
				action.Invoke(cancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				if (ex.CancellationToken == cancellationToken)
					throw;
				else
					return ex;
			}
			catch (Exception ex)
			{
				return ex;
			}

			return null;
		}

		#endregion InvokeException

		#region InvokeAll

		/// <summary>
		/// Последовательно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями, если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void InvokeAll([NotNull] this Action[] actions, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			if (actions.Length != 0)
				((IEnumerable<Action>)actions).InvokeAll(message);
		}

		/// <summary>
		/// Последовательно вызывает все делегаты и бросает общее исключение <see cref="T:System.AggregateException"/> со всеми брошенными ими исключениями (кроме <see cref="T:System.OperationCanceledException"/>), если они были.
		/// Это не относится к исключению <see cref="T:System.Threading.ThreadAbortException"/>.
		/// </summary>
		/// <param name="actions">Вызываемые делегаты.</param>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <exception cref="T:System.OperationCanceledException">Если хотя бы 1 делегат бросил это исключение.</exception>
		/// <exception cref="T:System.AggregateException">Если хотя бы 1 делегат бросил исключение.</exception>
		public static void InvokeAll([NotNull] this Action<CancellationToken>[] actions, CancellationToken cancellationToken, [CanBeNull] string message = null)
		{
			if (actions == null) throw new ArgumentNullException("actions");

			if (actions.Length != 0)
				((IEnumerable<Action<CancellationToken>>)actions).InvokeAll(cancellationToken, message);
		}

		#endregion InvokeAll
	}
}
