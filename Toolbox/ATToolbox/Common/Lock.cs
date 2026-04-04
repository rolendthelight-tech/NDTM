using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AT.Toolbox
{
  /// <summary>
  /// Объект разделямой блокировки на базе ReaderWriterLockSlim,
  /// позволяющий использовать наглядный синтаксис using ( )
  /// </summary>
  /// <example>using (myLock.GetReadLock()) { /* reading actions */ }
  /// using (myLock.GetWriteLock()) { /* writing actions */ }</example>
  public sealed class LockSource
  {
    private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

    /// <summary>
    /// Устанавливает блокировку на чтение
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable GetReadLock()
    {
      return new ReadLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable GetWriteLock()
    {
      return new WriteLock(m_lock);
    }

    /// <summary>
    /// Устанавливает блокировку на чтение с возможностью перехода к блокировке на запись
    /// </summary>
    /// <returns>Дескриптор, позволяющий завершить блокировку</returns>
    public IDisposable GetUpgradeableLock()
    {
      return new UpgradeableLock(m_lock);
    }

    /// <summary>
    /// Завершает работу ReaderWriterLockSlim
    /// </summary>
    public void Close()
    {
      m_lock.Dispose();
    }

    private sealed class ReadLock : IDisposable
    {
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public ReadLock(ReaderWriterLockSlim source)
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
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public UpgradeableLock(ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        var read_held = source.IsReadLockHeld;
        var upgradeable_held = source.IsUpgradeableReadLockHeld;

        if (read_held && !upgradeable_held)
          throw new ArgumentException();

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
      private readonly ReaderWriterLockSlim m_lock;
      private readonly bool m_exit_required;

      public WriteLock(ReaderWriterLockSlim source)
      {
        if (source == null)
          throw new ArgumentNullException("source");

        if (source.IsReadLockHeld
          && !source.IsUpgradeableReadLockHeld)
          throw new ArgumentException();

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
    private int m_processor_count = Environment.ProcessorCount;

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
      Interlocked.Exchange(ref m_spin_lock, 0);
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
      private readonly SpinLock m_lock;

      public Locker(SpinLock spinLock)
      {
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
