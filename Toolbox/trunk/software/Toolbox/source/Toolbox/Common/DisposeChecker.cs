using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	/// <summary>
	/// Проверяет своевременные правильные вызовы <see cref="System.IDisposable.Dispose()"/>.
	/// </summary>
	public class DisposeChecker : IDisposable
	{
		[CanBeNull] private readonly string m_creation_stack_trace;
		[NotNull] private readonly object m_obj;
		[CanBeNull] private readonly FinalizeChecker m_finalize_checker;
		[NotNull] private readonly string m_type_name;
		private bool m_disposed;
		private bool m_dispose_repeated;
		[CanBeNull] private string m_dispose_stack_trace;
		private DisposeContext m_context;
		private bool m_accessed_in_background_thread;

		/// <summary>
		/// Создание объекта для проверки.
		/// </summary>
		/// <param name="obj">Проверяемый контейнер.</param>
		private DisposeChecker([NotNull] object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			SetStackTrace(ref this.m_creation_stack_trace);
			SetFinalizeCheckerTrace(ref this.m_finalize_checker);

			this.m_obj = obj;
			this.m_type_name = this.m_obj.GetType().Name;
			this.m_disposed = false;
			this.m_dispose_repeated = false;
			Access();
			Thread.MemoryBarrier();
		}

		/// <summary>
		/// Создание объекта для проверки.
		/// </summary>
		/// <param name="obj">Проверяемый контейнер.</param>
		[NotNull]
		public static DisposeChecker Create([NotNull] object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			return new DisposeChecker(obj);
		}

		/// <summary>
		/// Создание контекста вызова <see cref="DisposeInContext()"/>.
		/// </summary>
		/// <returns>Контекст.</returns>
		[NotNull]
		public IDisposable CreateContext()
		{
			Access();
			return new DisposeContext(this);
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.ObjectDisposedException"/>, если объект освобождён.
		/// </summary>
		/// <exception cref="T:System.ObjectDisposedException">Если объект освобождён.</exception>
		public void ThrowIfDisposed()
		{
			Access();
			if (this.m_disposed)
				throw new ObjectDisposedException(this.m_type_name);
		}

		/// <summary>
		/// Бросает исключение <see cref="T:System.ObjectDisposedException"/>, если объект освобождён.
		/// </summary>
		/// <exception cref="T:System.ObjectDisposedException">Если объект освобождён.</exception>
		/// <param name="objectName">Имя объекта для исключения.</param>
		public void ThrowIfDisposed([NotNull] string objectName)
		{
			if (objectName == null) throw new ArgumentNullException("objectName");

			Access();
			if (this.m_disposed)
				throw new ObjectDisposedException(objectName);
		}

		[Conditional("DEBUG_FULL")]
		private void Access()
		{
			if (Thread.CurrentThread.IsBackground)
				this.m_accessed_in_background_thread = true;
		}

		[Conditional("DEBUG_FULL")]
		private void SetStackTrace([CanBeNull] ref string stackTrace)
		{
			if (stackTrace != null) throw new ArgumentException("Is not null", "stackTrace");

			var st = Environment.StackTrace;
			if (st == null)
				throw new ApplicationException("Environment.StackTrace вернул null");
			stackTrace = st;
		}

		[Conditional("DEBUG_FULL")]
		private void SetFinalizeCheckerTrace([CanBeNull] ref FinalizeChecker finalizeChecker)
		{
			if (finalizeChecker != null) throw new ArgumentException("Is not null", "finalizeChecker");

			var fc = FinalizeChecker.Create(this);
			if (fc == null)
				throw new ApplicationException("FinalizeChecker.Create вернул null");
			finalizeChecker = fc;
		}

		[Obsolete("Это свойство предназначено для отладки", true)]
		public bool Disposed
		{
			get { return this.m_disposed; }
		}

		[Obsolete("Это свойство предназначено для отладки", true)]
		public bool Repeated
		{
			get { return this.m_dispose_repeated; }
		}

		[NotNull]
		[Obsolete("Это свойство предназначено для отладки", true)]
		public object Object
		{
			get { return this.m_obj; }
		}

		[CanBeNull]
		[Obsolete("Это свойство предназначено для отладки", true)]
		public string CreationStackTrace
		{
			get { return this.m_creation_stack_trace; }
		}

		[CanBeNull]
		[Obsolete("Это свойство предназначено для отладки", true)]
		public DisposeChecker Checker
		{
			get { return this.m_finalize_checker == null ? null : this.m_finalize_checker.DisposeChecker; }
		}

		[Obsolete("Это свойство предназначено для отладки", true)]
		public bool AccessedInBackgroundThread
		{
			get { return this.m_accessed_in_background_thread; }
		}

		[Conditional("DEBUG_FULL")]
		private void Repeate()
		{
			Debugger.Break();
		}

		[Conditional("DEBUG_FULL")]
		private void Finalize2()
		{
			Debugger.Break();
		}

		#region Implementation of IDisposable

		private void DisposeImpl()
		{
			if (this.m_disposed)
			{
				this.m_dispose_repeated = true;
				Repeate();
			}
			else
			{
				this.m_disposed = true;
				DisposeFinalizeChecker();
				SetStackTrace(ref this.m_dispose_stack_trace);
			}
			GC.KeepAlive(this);
		}

		[Conditional("DEBUG_FULL")]
		private void DisposeFinalizeChecker()
		{
			if (this.m_finalize_checker == null)
				throw new ApplicationException("Не согласованы инициализация и освобождение объекта FinalizeChecker");

			this.m_finalize_checker.Dispose();
		}

		/// <summary>
		/// Вызов <see cref="Dispose()"/> в контексте (рекомендуется при отсутствии наследников и базового виртуального метода <see cref="System.IDisposable.Dispose()"/>).
		/// </summary>
		public void Dispose()
		{
			Thread.MemoryBarrier();
			Access();
			var context = this.m_context;

			if (context != null)
				throw new InvalidOperationException("Контекст есть");

			DisposeImpl();
		}

		/// <summary>
		/// Вызов <see cref="Dispose()"/> в контексте (рекомендуется для проверки вызова базового виртуального метода <see cref="System.IDisposable.Dispose()"/> при переопределении в наследниках).
		/// </summary>
		public void DisposeInContext()
		{
			Access();
			var context = this.m_context;

			if (context == null)
				throw new InvalidOperationException("Контекста нет");

			context.Check();
			DisposeImpl();
		}

		#endregion

		private class FinalizeChecker : IDisposable
		{
			[NotNull] private readonly DisposeChecker m_dispose_checker;
			private bool m_disposed;

			private FinalizeChecker([NotNull] DisposeChecker disposeChecker)
			{
				if (disposeChecker == null) throw new ArgumentNullException("disposeChecker");

				this.m_dispose_checker = disposeChecker;
			}

			[NotNull]
			public static FinalizeChecker Create([NotNull] DisposeChecker disposeChecker)
			{
				if (disposeChecker == null) throw new ArgumentNullException("disposeChecker");

				return new FinalizeChecker(disposeChecker);
			}

			~FinalizeChecker()
			{
				Dispose();
				Finalize2();
			}

			[NotNull]
			[Obsolete("Это свойство предназначено для отладки", true)]
			public DisposeChecker DisposeChecker
			{
				get { return this.m_dispose_checker; }
			}

			private void Finalize2()
			{
				this.m_dispose_checker.Finalize2();
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				var old_disposed = this.m_disposed;
				this.m_disposed = true;
				if (!old_disposed)
				{
					GC.SuppressFinalize(this);
				}
				GC.KeepAlive(this);
			}

			#endregion
		}

		private class DisposeContext : IDisposable
		{
			[NotNull] private readonly DisposeChecker m_checker;
			private bool m_disposed;
			private bool m_check1;
			private bool m_check2_or_more;

			public DisposeContext([NotNull] DisposeChecker checker)
			{
				if (checker == null) throw new ArgumentNullException("checker");

				this.m_checker = checker;
				this.m_disposed = false;
				this.m_check1 = false;
				this.m_check2_or_more = false;

				var old = Interlocked.CompareExchange(ref this.m_checker.m_context, this, null);

				if (old != null)
					throw new InvalidOperationException("Контекст уже есть");
			}

			public void Check()
			{
				ThrowIfDisposed();

				if (this.m_check1)
				{
					this.m_check2_or_more = true;
					throw new InvalidOperationException("Повторный вызов");
				}
				else
				{
					this.m_check1 = true;
				}
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				var old_disposed = this.m_disposed;
				this.m_disposed = true;

				if (!old_disposed)
				{
					var old = Interlocked.CompareExchange(ref this.m_checker.m_context, null, this);

					if (old != this)
						throw new InvalidOperationException("Контекст уже не тот");

					if (!this.m_check1)
						throw new ApplicationException("Не было вызова");
					if (this.m_check2_or_more)
						throw new ApplicationException("Было более одного вызова");
				}
			}

			private void ThrowIfDisposed()
			{
				if (this.m_disposed)
					throw new ObjectDisposedException(this.GetType().Name);
			}

			#endregion
		}
	}
}
