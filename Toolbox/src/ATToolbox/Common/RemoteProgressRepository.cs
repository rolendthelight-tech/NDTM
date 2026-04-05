using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using AT.Toolbox.Properties;

namespace AT.Toolbox
{
  /// <summary>
  /// Репозиторий задач с доступом к статусу их выполнения по Guid
  /// </summary>
  public sealed class RemoteProgressRepository 
  {
    private readonly LockSource m_lock = new LockSource();
    private readonly RunBaseWrapperCollection m_wrappers = new RunBaseWrapperCollection();
    private readonly Dictionary<Guid, Exception> m_task_errors = new Dictionary<Guid, Exception>();
    private TimeSpan m_exception_timeout = new TimeSpan(0, 1, 30);

    private static RemoteProgressRepository _instanse = new RemoteProgressRepository();

    private RemoteProgressRepository() { }

    public static RemoteProgressRepository Instanse 
    {
      get { return _instanse; }
    }

    /// <summary>
    /// Время ожидания перед удалением сообщения об ошибке из репозитория
    /// </summary>
    public TimeSpan ExceptionTimeout
    {
      get { return m_exception_timeout; }
      set
      {
        long totalMilliseconds = (long)value.TotalMilliseconds;

        if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
          throw new ArgumentOutOfRangeException();

        lock (m_task_errors)
          m_exception_timeout = value;
      }
    }

    /// <summary>
    /// Добавляет задачу в репозиторий и асинхронно запускает её
    /// </summary>
    /// <param name="runBase">Задача, которую требуется выполнить</param>
    /// <returns>Глобальный идентификатор, по которому 
    /// будет осуществляться доступ к задаче</returns>
    public Guid BeginRun(IRunBase runBase)
    {
      var wrapper = new RunBaseWrapper(runBase);

      using (m_lock.GetWriteLock())
      {
        m_wrappers.Add(wrapper);
      }

      ThreadPool.QueueUserWorkItem(this.WaitCallback, wrapper);

      return wrapper.Guid;
    }

    /// <summary>
    /// Добавляет в репозиторий задачу с явно указанным глобальным идентификатором
    /// </summary>
    /// <param name="runBase">Добавляемая задача</param>
    /// <param name="explicitWorkGuid">Идентификатор задачи</param>
    public void BeginRun(IRunBase runBase, Guid explicitWorkGuid)
    {
      var wrapper = new RunBaseWrapper(runBase, explicitWorkGuid);

      try
      {
        using (m_lock.GetWriteLock())
        {
          m_wrappers.Add(wrapper);
        }
      }
      catch
      {
        wrapper.Dispose();
        throw;
      }

      ThreadPool.QueueUserWorkItem(this.WaitCallback, wrapper);
    }

    /// <summary>
    /// Возвращает текущий статус задачи
    /// </summary>
    /// <param name="workGuid">Глобальный идентификатор задачи</param>
    /// <param name="waitPeriod">Время в миллисекундах, в течение которого ожидается смена статуса задачи</param>
    /// <returns>Если задача уже завершилась, возвращает null. Иначе, возвращает текущий статус задачи</returns>
    public ProgressState GetCurrentState(Guid workGuid, int waitPeriod)
    {
      this.CheckTask(workGuid);
      
      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(workGuid, out wrapper))
          return null;
      }

      return wrapper.GetCurrentState(waitPeriod);
    }

    /// <summary>
    /// Асинхронно отменяет выполнение задачи.
    /// </summary>
    /// <param name="workGuid">Глобальный идентификатор задачи</param>
    public void Cancel(Guid workGuid)
    {
      this.CheckTask(workGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(workGuid, out wrapper))
          return;
      }

      wrapper.Cancel();
    }

    /// <summary>
    /// Отменяет задачу путём прерывания потока выполнения
    /// </summary>
    /// <param name="workGuid">Глобальный идентификатор задачи</param>
    /// <param name="timeout">Время ожидания (в миллисекундах) завершения перед прерыванием потока.</param>
    public void Kill(Guid workGuid, int timeout)
    {
      this.CheckTask(workGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(workGuid, out wrapper))
          return;
      }

      // пробуем отменить задачу мягко
      wrapper.Cancel();

      // Если дано дополнительное время, ждём завершения в течение этого времени
      if (timeout > 0 && wrapper.WaitCompletion(timeout))
        return;

      // Принудительно завершаем задачу
      wrapper.Kill();
    }

    /// <summary>
    /// Ищет в репозитории уже выполняющуюся задачу, удовлетворяющую условию
    /// </summary>
    /// <param name="searchPredicate">Предикат, которому должна удовлетворять задача</param>
    /// <returns>Глобальный идентификатор первой найденной задачи,
    /// удовлетворяющей условию. Если ни одной задачи не найдено - возвращает пустой GUID</returns>
    public Guid FindRunningWork(Func<IRunBase, bool> searchPredicate)
    {
      if (searchPredicate == null)
        throw new ArgumentNullException("searchPredicate");

      using (m_lock.GetReadLock())
      {
        foreach (var wrapper in m_wrappers)
        {
          if (wrapper.Match(searchPredicate))
            return wrapper.Guid;
        }
      }

      return Guid.Empty;
    }

    /// <summary>
    /// Выполнение синхронизированной операции над репозиторием
    /// </summary>
    /// <param name="action">Операция, требующая синхронизации</param>
    public void Lock(Action action)
    {
      if (action == null)
        return;

      using (m_lock.GetWriteLock())
      {
        action();
      }
    }

    #region Implementation

    private void WaitCallback(object state)
    {
      var wrapper = (RunBaseWrapper)state;

      // Подготовка ограниченной области выполнения служащей цели отложить доставку
      // асинхронных исключений (в том числе ThreadAbortException)
      // если исключение возбуждается во время выполнения кода внутри блока finally,
      // что позволяет гарантированно выполнить закрытие обёртки над процессом и её удалению 
      // из репозитория
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        wrapper.Run();
      }
      catch(Exception ex)
      {
        if (!(ex is ThreadAbortException))
        {
          lock (m_task_errors)
          {
            m_task_errors[wrapper.Guid] = ex;
            ThreadPool.QueueUserWorkItem(this.ClearTaskErrors, wrapper.Guid);
          }
        }
      }
      finally
      {
        CompleteRunningTask(wrapper);
      }
    }

    private void ClearTaskErrors(object state)
    {
      Thread.Sleep(m_exception_timeout);

      lock (m_task_errors)
        m_task_errors.Remove((Guid)state);
    }

    private void CheckTask(Guid taskGuid)
    {
      lock (m_task_errors)
      {
        Exception ex;

        if (m_task_errors.TryGetValue(taskGuid, out ex))
        {
          m_task_errors.Remove(taskGuid);
          throw new WorkFaultException(ex);
        }
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    private void CompleteRunningTask(RunBaseWrapper wrapper)
    {
      wrapper.Complete();

      using (m_lock.GetWriteLock())
      {
        m_wrappers.Remove(wrapper.Guid);
        wrapper.Dispose();
      }
    }

    private class RunBaseWrapperCollection : KeyedCollection<Guid, RunBaseWrapper>
    {
      protected override Guid GetKeyForItem(RunBaseWrapper item)
      {
        return item.Guid;
      }

      public bool TryGetValue(Guid key, out RunBaseWrapper value)
      {
        return this.Dictionary.TryGetValue(key, out value);
      }
    }

    private class RunBaseWrapper : IDisposable
    {
      private readonly EventWaitHandle m_wait_handle;
      private readonly IRunBase m_run_base;
      private readonly Guid m_guid; 
      private volatile bool m_cancelled;
      private volatile bool m_disposed;
      private ProgressState m_state = new ProgressState();
      private Thread m_thread;
      private readonly CultureInfo m_culture = Thread.CurrentThread.CurrentUICulture;


      public RunBaseWrapper(IRunBase runBase) 
        : this(runBase, Guid.NewGuid())
      {
      }

      public RunBaseWrapper(IRunBase runBase, Guid guid)
      {
        if (guid.Equals(System.Guid.Empty))
          throw new ArgumentNullException("guid");
        
        m_guid = guid;

        if (runBase == null)
          throw new ArgumentNullException("runBase");

        m_run_base = runBase;
        m_run_base.ProgressChanged += this.HanldeProgressChanged;
        m_run_base.CancellationCheck += this.HandleCancellationCheck;

        m_wait_handle = new EventWaitHandle(false, EventResetMode.AutoReset);
      }

      public Guid Guid
      {
        get { return m_guid; }
      }

      public void Cancel()
      {
        m_cancelled = true;
      }

      public bool Match(Func<IRunBase, bool> predicate)
      {
        return predicate(m_run_base);
      }

      public void Run()
      {
        m_thread = Thread.CurrentThread;
        m_thread.CurrentUICulture = m_culture;
        m_run_base.Run();
      }

      public void Complete()
      {
        m_thread = null;
        // После завершения задачи статус нужно вернуть немедленно
        m_wait_handle.Set();
      }

      public void Kill()
      {
        var thread = m_thread;

        if (thread != null)
          m_thread.Abort();
      }

      /// <summary>
      /// Ожидание завершения работы потока.
      /// </summary>
      /// <param name="waitPeriod">Время ожидания в миллисекундах.</param>
      /// <returns>True - если выполение потока было завершено, иначе - False</returns>
      public bool WaitCompletion(int waitPeriod)
      {
        if (waitPeriod < 1 || m_thread == null)
          return m_thread == null;

        DateTime end_time = DateTime.Now.AddMilliseconds(waitPeriod);

        while (DateTime.Now < end_time)
        {
          lock (m_wait_handle)
          {
            if (m_disposed)
              return true;
            else
            {
              if (!m_wait_handle.WaitOne(end_time - DateTime.Now))
                break;

              if (m_thread == null)
                return true;
            }
          }
        }

        return m_thread == null;
      }

      public ProgressState GetCurrentState(int waitPeriod)
      {
        if (waitPeriod < 1)
          throw new ArgumentOutOfRangeException();

        // Если метод вызван для уже завершившейся задачи,
        // ждать нового оповещения о прогрессе бессмысленно.
        lock (m_wait_handle)
        {
          if (!m_disposed)
            m_wait_handle.WaitOne(waitPeriod);
        }
           
        return m_state;
      }

      private void HanldeProgressChanged(object sender, ProgressChangedEventArgs e)
      {
        m_state = new ProgressState
        {
          Percentage = e.ProgressPercentage,
          Description = e.UserState != null ? e.UserState.ToString() : null
        };

        // Здесь проверять WaitHandle на доступность не нужно,
        // т.к. текущий метод никогда не вызовется после завершения задачи
        m_wait_handle.Set();
      }

      private void HandleCancellationCheck(object sender, CancelEventArgs e)
      {
        e.Cancel = m_cancelled;
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
        if (!m_disposed)
        {
          if (disposing)
          {
            m_run_base.ProgressChanged -= this.HanldeProgressChanged;
            m_run_base.CancellationCheck -= this.HandleCancellationCheck;

            lock (m_wait_handle)
            {
              m_wait_handle.Close();
              m_disposed = true;
            }
          }
          else
          {
            m_disposed = true;
          }
        }
      }

      ~RunBaseWrapper()
      {
        Dispose(false);
      }
    }

    #endregion
  }

  [Serializable]
  public class WorkFaultException : ApplicationException
  {
    public WorkFaultException()
      : base(Resources.WORK_FAULT) { }

    public WorkFaultException(string message)
      : base(message) { }

    public WorkFaultException(string message, Exception innerException)
      : base(message, innerException) { }

    public WorkFaultException(Exception innerException)
      : base(Resources.WORK_FAULT, innerException) { }

    protected WorkFaultException(System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
}
