using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Toolbox.Threading
{
  /// <summary>
  /// Объект разделямой блокировки на базе ReaderWriterLockSlim,
  /// позволяющий использовать наглядный синтаксис using ( )
  /// </summary>
  /// <example>using (myLock.GetReadLock()) { /* reading actions */ }
  /// using (myLock.GetWriteLock()) { /* writing actions */ }</example>
  public sealed class LockSource
  {
	  [NotNull] private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private volatile bool m_closed;

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    [NotNull]
    public IDisposable GetReadLock()
    {
      return new ReadLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    [NotNull]
    public IDisposable GetWriteLock()
    {
      return new WriteLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    [NotNull]
    public IDisposable GetUpgradeableLock()
    {
      return new UpgradeableLock(m_lock);
    }

    /// <summary>
    /// Выполнение операции в контексте блокировки на чтение
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    public void RunInReadLock([NotNull] Action action, int millisecondsTimeout)
    {
	    if (action == null) throw new ArgumentNullException("action");

	    if (m_closed)
        return;

      var lock_required = !m_lock.IsWriteLockHeld
          && !m_lock.IsUpgradeableReadLockHeld && !m_lock.IsReadLockHeld;

      if (lock_required && !m_lock.TryEnterReadLock(millisecondsTimeout))
        return;
      else
      {
        try
        {
          action();
        }
        finally
        {
          if (lock_required)
            m_lock.ExitReadLock();
        }
      }
    }

    /// <summary>
    /// Выполнение операции в контексте блокировки на запись
    /// </summary>
    /// <param name="action">Выполняемая операция</param>
    /// <param name="millisecondsTimeout">Время, по истечении которого если не удалось захватить блокировку операция выполняться не будет</param>
    public void RunInWriteLock([NotNull] Action action, int millisecondsTimeout)
    {
	    if (action == null) throw new ArgumentNullException("action");

	    if (m_closed)
        return;

      if (m_lock.IsReadLockHeld
        && !m_lock.IsUpgradeableReadLockHeld)
				throw new InvalidOperationException("IsReadLockHeld");

      var lock_required = !m_lock.IsWriteLockHeld;

      if (lock_required && !m_lock.TryEnterWriteLock(millisecondsTimeout))
        return;
      else
      {
        try
        {
          action();
        }
        finally
        {
          if (lock_required)
            m_lock.ExitWriteLock();
        }
      }
    }

    /// <summary>
    /// Завершает работу ReaderWriterLockSlim
    /// </summary>
    public void Close()
    {
      m_closed = true;
      m_lock.Dispose();
    }

    private sealed class ReadLock : IDisposable
    {
	    [NotNull] private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReadLock([NotNull] ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        m_lock = source;

        m_exit_required = !m_lock.IsWriteLockHeld
          && !source.IsUpgradeableReadLockHeld && !m_lock.IsReadLockHeld;

        if (m_exit_required)
          m_lock.EnterReadLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitReadLock();
      }
    }

    private sealed class UpgradeableLock : IDisposable
    {
	    [NotNull] private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public UpgradeableLock([NotNull] ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        var read_held = source.IsReadLockHeld;
        var upgradeable_held = source.IsUpgradeableReadLockHeld;

        if (read_held && !upgradeable_held)
					throw new ArgumentException("Not upgradeable", "source");

        m_lock = source;

        m_exit_required = !read_held && !upgradeable_held;

        if (m_exit_required)
          m_lock.EnterUpgradeableReadLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitUpgradeableReadLock();
      }
    }

    private sealed class WriteLock : IDisposable
    {
	    [NotNull] private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public WriteLock([NotNull] ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        if (source.IsReadLockHeld
          && !source.IsUpgradeableReadLockHeld)
					throw new ArgumentException("Not writable", "source");

        m_lock = source;

        m_exit_required = !m_lock.IsWriteLockHeld;

        if (m_exit_required)
          m_lock.EnterWriteLock();
      }

      public void Dispose()
      {
        if (m_exit_required)
          m_lock.ExitWriteLock();
      }
    }
  }

  /// <summary>
  /// Легковесная не реентерабельная блокировка
  /// </summary>
  public sealed class SpinLock
  {
    private int m_spin_lock;
    private readonly int m_processor_count = Environment.ProcessorCount;

    private const int PROC_SPIN_COUNTER = 20;
    private const int PROC_QUICK_COUNTER = 10;
    private const int SINGLE_THREAD_TRESHOLD = 15;

    private void Enter()
    {
      int counter = 0;

      while (Interlocked.CompareExchange(ref m_spin_lock, 1, 0) == 1)
      {
        if ((counter < PROC_QUICK_COUNTER) && (m_processor_count > 1))
          Thread.SpinWait(PROC_SPIN_COUNTER * (counter));
        else if (counter < SINGLE_THREAD_TRESHOLD)
          Thread.Sleep(0);
        else
          Thread.Sleep(1);

        counter++;
      }
    }

    private void Exit()
    {
			if (Interlocked.CompareExchange(ref m_spin_lock, 0, 1) != 1)
			{
				throw new InvalidOperationException("Already exited");
			}
    }

    /// <summary>
    /// Устанавливает блокировку
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable GetLock()
    {
      return new Locker(this);
    }

    private class Locker : IDisposable
    {
	    [NotNull] private readonly SpinLock m_lock;

      public Locker([NotNull] SpinLock spinLock)
      {
	      if (spinLock == null) throw new ArgumentNullException("spinLock");

	      m_lock = spinLock;
        m_lock.Enter();
      }

      public void Dispose()
      {
        m_lock.Exit();
      }
    }
  }
}
