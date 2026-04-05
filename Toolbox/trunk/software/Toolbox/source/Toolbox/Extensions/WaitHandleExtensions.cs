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
	/// Расширение функциональности объектов ожидания.
	/// </summary>
	public static class WaitHandleExtensions
	{
		/// <summary>
		/// Ожидает получения сигнала объектом ожидания <see cref="T:System.Threading.WaitHandle"/>.
		/// </summary>
		/// <param name="waitHandle">Объект ожидания.</param>
		/// <param name="cancellationToken">Токен отмены ожидания.</param>
		public static void Wait([NotNull] this WaitHandle waitHandle, CancellationToken cancellationToken)
		{
			if (waitHandle == null) throw new ArgumentNullException("waitHandle");

			cancellationToken.ThrowIfCancellationRequested();
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var number = WaitHandle.WaitAny(
					new[]
					{
						waitHandle,
						cancellationToken.WaitHandle,
					});
				switch (number)
				{
					case 0:
						break;
					case 1:
						cancellationToken.ThrowIfCancellationRequested();
						throw new ApplicationException("Ошибка ожидания");
					default:
						throw new ApplicationException("Неверный индекс объекта ожидания");
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
			else
			{
				waitHandle.WaitOne();
			}
		}

		/// <summary>
		/// Ожидает получения сигнала объектом ожидания <see cref="T:System.Threading.WaitHandle"/>.
		/// </summary>
		/// <param name="waitHandle">Объект ожидания.</param>
		/// <param name="timeout">Время ожидания.</param>
		/// <param name="cancellationToken">Токен отмены ожидания.</param>
		public static void Wait([NotNull] this WaitHandle waitHandle, TimeSpan timeout, CancellationToken cancellationToken)
		{
			if (waitHandle == null) throw new ArgumentNullException("waitHandle");

			cancellationToken.ThrowIfCancellationRequested();
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var number = WaitHandle.WaitAny(
					new[]
					{
						waitHandle,
						cancellationToken.WaitHandle,
					}, timeout);
				switch (number)
				{
					case 0:
						break;
					case 1:
						cancellationToken.ThrowIfCancellationRequested();
						throw new ApplicationException("Ошибка ожидания");
					default:
						throw new ApplicationException("Неверный индекс объекта ожидания");
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
			else
			{
				waitHandle.WaitOne(timeout);
			}
		}

		/// <summary>
		/// Ожидает получения сигнала объектом ожидания <see cref="T:System.Threading.WaitHandle"/>.
		/// </summary>
		/// <param name="waitHandle">Объект ожидания.</param>
		/// <param name="timeout">Время ожидания.</param>
		/// <param name="exitContext">Выходить ли из домена синхронизации в текущем контексте перед ожиданием (в синхронизированном контексте) с его последующим повторным получением.</param>
		/// <param name="cancellationToken">Токен отмены ожидания.</param>
		public static void Wait([NotNull] this WaitHandle waitHandle, TimeSpan timeout, bool exitContext, CancellationToken cancellationToken)
		{
			if (waitHandle == null) throw new ArgumentNullException("waitHandle");

			cancellationToken.ThrowIfCancellationRequested();
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var number = WaitHandle.WaitAny(
					new[]
					{
						waitHandle,
						cancellationToken.WaitHandle,
					}, timeout, exitContext);
				switch (number)
				{
					case 0:
						break;
					case 1:
						cancellationToken.ThrowIfCancellationRequested();
						throw new ApplicationException("Ошибка ожидания");
					default:
						throw new ApplicationException("Неверный индекс объекта ожидания");
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
			else
			{
				waitHandle.WaitOne(timeout, exitContext);
			}
		}
	}
}
