using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Toolbox.Threading
{
	/// <summary>
	/// Класс проверки однопоточного доступа
	/// </summary>
	public class SingleThreadChecker
	{
		private readonly int m_tid;

		/// <summary>
		/// Создаёт объект проверки для текущего потока
		/// </summary>
		public SingleThreadChecker()
		{
			this.m_tid = Thread.CurrentThread.ManagedThreadId;
			Thread.MemoryBarrier();
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.InvalidOperationException"/>, если вызов произведён не из потока, в котором объект был создан
		/// </summary>
		public void ThrowIfAnotherThread()
		{
			if (this.m_tid != Thread.CurrentThread.ManagedThreadId)
				throw new InvalidOperationException("Вызов из другого потока запрещён");
		}

		/// <summary>
		/// Создаёт объект проверки для текущего потока
		/// </summary>
		[NotNull]
		public static SingleThreadChecker Create()
		{
			return new SingleThreadChecker();
		}
	}
}
