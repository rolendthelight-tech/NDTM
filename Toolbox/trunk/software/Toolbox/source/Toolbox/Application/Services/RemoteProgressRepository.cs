using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Properties;
using Toolbox.Threading;
using Toolbox.Extensions;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Репозиторий задач с доступом к статусу их выполнения по Guid
  /// </summary>
  public sealed class RemoteProgressRepository 
  {
	  [NotNull] private readonly LockSource m_lock = new LockSource();
	  [NotNull] private readonly RunBaseWrapperCollection m_wrappers = new RunBaseWrapperCollection();
	  [NotNull] private readonly Dictionary<Guid, Exception> m_task_errors = new Dictionary<Guid, Exception>();
    private TimeSpan m_exception_timeout = new TimeSpan(0, 1, 30);

	  [NotNull] private static readonly RemoteProgressRepository _instanse = new RemoteProgressRepository();

    private RemoteProgressRepository() { }

	  [NotNull]
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
        long total_milliseconds = (long)value.TotalMilliseconds;

        if ((total_milliseconds < -1L) || (total_milliseconds > 0x7fffffffL))
					throw new ArgumentOutOfRangeException("value", value, "Отрицательное или слишком большое значение");

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
    public Guid BeginRun([NotNull] IRunBase runBase)
    {
	    if (runBase == null) throw new ArgumentNullException("runBase");

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
    /// <param name="explicitTaskGuid">Идентификатор задачи</param>
    public void BeginRun([NotNull] IRunBase runBase, Guid explicitTaskGuid)
    {
	    if (runBase == null) throw new ArgumentNullException("runBase");

	    var wrapper = new RunBaseWrapper(runBase, explicitTaskGuid);

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
    /// <param name="taskGuid">Глобальный идентификатор задачи</param>
    /// <param name="waitPeriod">Время, в течение которого ожидается смена статуса задачи</param>
    /// <returns>Если задача уже завершилась, возвращает <code>null</code>. Иначе, возвращает текущий статус задачи</returns>
    [CanBeNull]
    public ProgressState GetCurrentState(Guid taskGuid, TimeSpan waitPeriod)
    {
      this.CheckTask(taskGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(taskGuid, out wrapper))
          return null;
      }

      return wrapper.GetCurrentState(waitPeriod);
    }

    /// <summary>
    /// Определяет возможность отмены задачи
    /// </summary>
    /// <param name="taskGuid">Глобальный идентификатор задачи</param>
    /// <returns><code>true</code>, если задача есть в репозитории, реализует ICancelableRunBase,
    ///  и свойство ICancelableRunBase.CanCancel вернуло <code>true</code>. В остальных случаях возвращает <code>false</code></returns>
    public bool CanCancel(Guid taskGuid)
    {
      this.CheckTask(taskGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(taskGuid, out wrapper))
          return false;
      }

      return wrapper.CanCancel;
    }

    /// <summary>
    /// Получение нестандартного статуса выполняющейся задачи, реализующей <see cref="T:System.IServiceProvider"/>
    /// </summary>
    /// <typeparam name="TState">Тип нестандартного статуса</typeparam>
    /// <param name="taskGuid">Глобальный идентификатор задачи</param>
    /// <returns>Нестандартный статус задачи. Если задача завершена
    /// или не реализует <see cref="T:System.IServiceProvider"/>, возвращает <code>null</code></returns>
    [CanBeNull]
    public TState GetCustomState<TState>(Guid taskGuid)
      where TState : class
    {
      if (typeof(TState) == typeof(ProgressState))
        return this.GetCurrentState(taskGuid, TimeSpan.FromMilliseconds(1)) as TState;

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(taskGuid, out wrapper))
          return null;
      }

      return wrapper.GetCustomState<TState>();
    }

    /// <summary>
    /// Асинхронно отменяет выполнение задачи.
    /// </summary>
    /// <param name="taskGuid">Глобальный идентификатор задачи</param>
    public void Cancel(Guid taskGuid)
    {
      this.CheckTask(taskGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(taskGuid, out wrapper))
          return;
      }

      wrapper.Cancel();
    }

    /// <summary>
    /// Отменяет задачу путём прерывания потока выполнения
    /// </summary>
    /// <param name="taskGuid">Глобальный идентификатор задачи</param>
    /// <param name="timeout">Время ожидания завершения перед прерыванием потока.</param>
    public void Kill(Guid taskGuid, TimeSpan timeout)
    {
      this.CheckTask(taskGuid);

      RunBaseWrapper wrapper = null;

      using (m_lock.GetReadLock())
      {
        if (!m_wrappers.TryGetValue(taskGuid, out wrapper))
          return;
      }

      // пробуем отменить задачу мягко
      wrapper.Cancel();

      // Если дано дополнительное время, ждём завершения в течение этого времени
      if (timeout > TimeSpan.Zero && wrapper.WaitCompletion(timeout))
        return;

      // Принудительно завершаем задачу
      wrapper.Kill();
    }

    /// <summary>
    /// Ищет в репозитории уже выполняющуюся задачу, удовлетворяющую условию
    /// </summary>
    /// <param name="searchPredicate">Предикат, которому должна удовлетворять задача</param>
    /// <returns>Глобальный идентификатор первой найденной задачи,
    /// удовлетворяющей условию. Если ни одной задачи не найдено — возвращает пустой GUID</returns>
    public Guid FindRunningWork([NotNull] Func<IRunBase, bool> searchPredicate)
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
    public void Lock([NotNull] Action action)
    {
	    if (action == null) throw new ArgumentNullException("action");

	    using (m_lock.GetWriteLock())
      {
        action();
      }
    }

	  #region Implementation

    private void WaitCallback([NotNull] object state)
    {
	    if (state == null) throw new ArgumentNullException("state");

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
	    catch (ThreadAbortException)
	    {
		    throw;
	    }
	    catch (Exception ex)
	    {
		    lock (m_task_errors)
		    {
			    m_task_errors[wrapper.Guid] = ex;
			    ThreadPool.QueueUserWorkItem(this.ClearTaskErrors, wrapper.Guid);
		    }
	    }
	    finally
	    {
		    CompleteRunningTask(wrapper);
	    }
    }

    private void ClearTaskErrors([NotNull] object state)
    {
	    if (state == null) throw new ArgumentNullException("state");

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
          throw new TaskFaultException(ex);
        }
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    private void CompleteRunningTask([NotNull] RunBaseWrapper wrapper)
    {
	    if (wrapper == null) throw new ArgumentNullException("wrapper");

	    wrapper.Complete();

      using (m_lock.GetWriteLock())
      {
        m_wrappers.Remove(wrapper.Guid);
        wrapper.Dispose();
      }
    }

    private class RunBaseWrapperCollection : KeyedCollection<Guid, RunBaseWrapper>
    {
      protected override Guid GetKeyForItem([NotNull] RunBaseWrapper item)
      {
	      if (item == null) throw new ArgumentNullException("item");

	      return item.Guid;
      }

	    public bool TryGetValue(Guid key, [CanBeNull] out RunBaseWrapper value)
      {
        if (this.Dictionary == null)
        {
          value = null;
          return false;
        }

        return this.Dictionary.TryGetValue(key, out value);
      }
    }

    private class RunBaseWrapper : IDisposable
    {
	    [NotNull] private readonly EventWaitHandle m_wait_handle;
	    [NotNull] private readonly IRunBase m_run_base;
      private readonly Guid m_guid; 
      private volatile bool m_cancelled;
      private volatile bool m_disposed;
	    [NotNull] private ProgressState m_state = new ProgressState();
	    [CanBeNull] private Thread m_thread;
	    [NotNull] private readonly CultureInfo m_culture = Thread.CurrentThread.CurrentUICulture;

      public RunBaseWrapper([NotNull] IRunBase runBase) 
        : this(runBase, Guid.NewGuid())
      {
      }

      public RunBaseWrapper([NotNull] IRunBase runBase, Guid guid)
      {
        if (runBase == null)
          throw new ArgumentNullException("runBase");
        if (guid.Equals(System.Guid.Empty))
					throw new ArgumentException("Empty", "guid");

        m_guid = guid;

        m_run_base = runBase;
        m_run_base.ProgressChanged += this.HanldeProgressChanged;

        if (m_run_base is ICancelableRunBase)
          ((ICancelableRunBase) m_run_base).CancellationCheck += this.HandleCancellationCheck;

        m_wait_handle = new EventWaitHandle(false, EventResetMode.AutoReset);
      }

      public Guid Guid
      {
        get { return m_guid; }
      }

      public bool CanCancel
      {
        get { return m_run_base is ICancelableRunBase && ((ICancelableRunBase) m_run_base).CanCancel; }
      }

      public void Cancel()
      {
        m_cancelled = true;

        if (m_run_base is CancelableRunBase)
          ((CancelableRunBase)m_run_base).OnCancel();
      }

      public bool Match([NotNull] Func<IRunBase, bool> predicate)
      {
	      if (predicate == null) throw new ArgumentNullException("predicate");

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
          thread.Abort();
      }

      /// <summary>
      /// Ожидание завершения работы потока.
      /// </summary>
      /// <param name="waitPeriod">Время ожидания.</param>
      /// <returns><code>true</code> — если выполнение потока было завершено, иначе — <code>false</code></returns>
      public bool WaitCompletion(TimeSpan waitPeriod)
      {
        if (waitPeriod <= TimeSpan.Zero || m_thread == null)
          return m_thread == null;

        DateTime end_time = DateTime.Now.Add(waitPeriod);

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

	    [NotNull]
	    public ProgressState GetCurrentState(TimeSpan waitPeriod)
      {
        if (waitPeriod.TotalMillisecondsDecimal() < 1)
					throw new ArgumentOutOfRangeException("waitPeriod", waitPeriod, "< 1 мс");

        // Если метод вызван для уже завершившейся задачи,
        // ждать нового оповещения о прогрессе бессмысленно.
        lock (m_wait_handle)
        {
          if (!m_disposed)
            m_wait_handle.WaitOne(waitPeriod);
        }

        return m_state;
      }

      public TState GetCustomState<TState>()
        where TState : class
      {
        var srv = m_run_base as IServiceProvider;

        if (srv != null)
          return srv.GetService<TState>();

        return null;
      }

      private void HanldeProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
      {
	      if (e == null) throw new ArgumentNullException("e");

	      m_state = new ProgressState
        {
          Percentage = e.ProgressPercentage,
          Description = e.UserState != null ? e.UserState.ToString() : null
        };

        // Здесь проверять WaitHandle на доступность не нужно,
        // т. к. текущий метод никогда не вызовется после завершения задачи
        m_wait_handle.Set();
      }

      private void HandleCancellationCheck(object sender, [NotNull] CancelEventArgs e)
      {
	      if (e == null) throw new ArgumentNullException("e");

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

            if (m_run_base is ICancelableRunBase)
              ((ICancelableRunBase)m_run_base).CancellationCheck -= this.HandleCancellationCheck;

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
  public class TaskFaultException : ApplicationException
  {
    public TaskFaultException()
      : base(Resources.WORK_FAULT) { }

    public TaskFaultException([NotNull] string message)
      : base(message)
    {
	    if (message == null) throw new ArgumentNullException("message");
    }

	  public TaskFaultException([NotNull] string message, Exception innerException)
      : base(message, innerException)
    {
	    if (message == null) throw new ArgumentNullException("message");
    }

	  public TaskFaultException(Exception innerException)
      : base(Resources.WORK_FAULT, innerException) { }

    protected TaskFaultException([NotNull] System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
}
