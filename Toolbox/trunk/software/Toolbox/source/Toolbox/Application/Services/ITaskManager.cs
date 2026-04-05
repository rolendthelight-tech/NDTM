using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Extensions;
using Toolbox.Properties;
using Toolbox.Threading;
using log4net;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Менеджер задач
  /// </summary>
  public interface ITaskManager : IRunningContext
  {
    /// <summary>
    /// Период, в течение которого ожидается завершение задачи до открытия диалога
    /// с прогрессом выполнения
    /// </summary>
    TimeSpan SyncTimeout { get; set; }

    /// <summary>
    /// Запуск задачи без ожидания результата
    /// </summary>
    /// <param name="runBase">Задача для выполнения</param>
    /// <param name="parameters">Настройки запуска задачи</param>
    /// <param name="onCompleted">Действие, выполняемое по завершении задачи</param>
    /// <returns>Обёртка над переданной задачей</returns>
    [NotNull]
    TaskInfo Start([NotNull] IRunBase runBase, [NotNull] LaunchParameters parameters, [CanBeNull] Action onCompleted);

    /// <summary>
    /// Получение списка задач, выполняющихся в настоящий момент
    /// </summary>
    [NotNull]
    ReadOnlyCollection<TaskInfo> GetRunningTasks();

    /// <summary>
    /// Происходит запуске новой или завершении одной из выполняющихся задач
    /// </summary>
    event EventHandler TaskListChanged;
  }

  /// <summary>
  /// Параметры запуска задачи
  /// </summary>
  public class LaunchParameters
  {
    public LaunchParameters()
    {
      this.CloseOnFinish = true;
      this.KillTimeout = TimeSpan.FromSeconds(1.5);
      this.ShowBufferOnComplete = true;
    }

    /// <summary>
    /// Заголовок окна с индикатором прогресса
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Закрывать ли диалог с индикатором прогресса после завершения задачи
    /// </summary>
    public bool CloseOnFinish { get; set; }

    /// <summary>
    /// Показывать модальный диалог с индикатором прогресса при асинхронном запуске
    /// </summary>
    public bool Modal { get; set; }

    /// <summary>
    /// Значок на форме с индикатором прогресса
    /// </summary>
    public Bitmap Icon { get; set; }

    /// <summary>
    /// Время работы задачи перед возможностью прервать
    /// </summary>
    public TimeSpan KillTimeout { get; set; }

    /// <summary>
    /// Показывать ли окно с сообщениями, когда задача завершена
    /// </summary>
    public bool ShowBufferOnComplete { get; set; }
  }

  public enum TaskStatus
  {
		[DisplayNameFromResource(typeof(Resources), "PENDING")]
    Pending,
		[DisplayNameFromResource(typeof(Resources), "RUNNING")]
    Running,
		[DisplayNameFromResource(typeof(Resources), "CANCELLING")]
    Cancelling,
		[DisplayNameFromResource(typeof(Resources), "CANCELLED")]
    Cancelled,
		[DisplayNameFromResource(typeof(Resources), "COMPLETED")]
    Completed,
		[DisplayNameFromResource(typeof(Resources), "ERROR")]
    Error
  }

  /// <summary>
  /// Обёртка над выполняющейся задачей
  /// </summary>
  public class TaskInfo : INotifyPropertyChanged
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(TaskInfo));

	  [NotNull] private readonly IRunBase m_task;
    private readonly LockSource m_state_lock = new LockSource();
    private readonly object m_task_lock = new object();
    private ProgressState m_current_state;
    private IAsyncResult m_async;
    private Thread m_running_thread;

    private readonly string m_name;
    private readonly bool m_close_on_finish;
    private readonly bool m_can_minimize;
    private readonly bool m_percent_notification;
    private readonly bool m_show_buffer;
    private readonly Bitmap m_icon;
    private readonly TimeSpan? m_kill_timeout;

    internal TaskInfo([NotNull] IRunBase task, [NotNull] LaunchParameters parameters, bool canMinimize)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_task = task;

      this.TaskGuid = Guid.NewGuid();
      m_can_minimize = canMinimize;
      m_close_on_finish = parameters.CloseOnFinish;
      m_icon = parameters.Icon;
      m_name = string.IsNullOrEmpty(parameters.Name) ? task.ToString() : parameters.Name;
      m_percent_notification = m_task.GetType().IsDefined(typeof(PercentNotificationAttribute), false);
      m_show_buffer = parameters.ShowBufferOnComplete;

      if (!(task is ICancelableRunBase) && !canMinimize)
        m_kill_timeout = parameters.KillTimeout;

      var tvo = this.GetCustomState<TaskVisualizationOverride>();

      if (tvo == null)
        return;

      if (tvo.PercentNotification != null)
        m_percent_notification = tvo.PercentNotification.Value;

      if (tvo.Icon != null)
        m_icon = tvo.Icon;

      if (!string.IsNullOrEmpty(tvo.Name))
        m_name = tvo.Name;
    }

    /// <summary>
    /// Событие об изменении прогресса выполнения
    /// </summary>
    public event EventHandler ProgressChanged;

    /// <summary>
    /// Происходит при смене допустимости отмены задачи
    /// </summary>
    public event EventHandler CanCancelChanged
    {
      add
      {
        var cancelable = m_task as ICancelableRunBase;

        if (cancelable != null)
          cancelable.CanCancelChanged += value;
      }
      remove
      {
        var cancelable = m_task as ICancelableRunBase;

        if (cancelable != null)
          cancelable.CanCancelChanged -= value;
      }
    }

    /// <summary>
    /// Событие об изменении свойства (статуса)
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal readonly Guid TaskGuid;

    #region Dialog properties

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    public string Name
    {
      get { return m_name; }
    }

    /// <summary>
    /// Закрывать ли диалог с прогрессом после завершения
    /// </summary>
    public bool CloseOnFinish
    {
      get { return m_close_on_finish; }
    }

    /// <summary>
    /// Возможность сворачивания диалога задачи в панель
    /// </summary>
    public bool CanMinimize
    {
      get { return m_can_minimize; }
    }

    /// <summary>
    /// Поддержка оповещения о проценте выполнения
    /// </summary>
    public bool PercentNotification
    {
      get { return m_percent_notification; }
    }

    /// <summary>
    /// Значок на форме с индикатором прогресса
    /// </summary>
    public Bitmap Icon
    {
      get { return m_icon; }
    }

    /// <summary>
    /// Время работы задачи перед возможностью прервать
    /// </summary>
    public TimeSpan? KillTimeout
    {
      get { return m_kill_timeout; }
    }

    #endregion

    #region Status properties

    public bool CanCancel
    {
      get { return m_task is ICancelableRunBase && ((ICancelableRunBase) m_task).CanCancel; } 
    }

    /// <summary>
    /// Статус выполняющейся задачи
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// Процент выполнения задачи
    /// </summary>
    public int Percentage
    {
      get
      {
        if (m_percent_notification)
        {
          var tmp = m_current_state;

          if (tmp != null)
            return tmp.Percentage;
          else
            return 0;
        }
        else
          return 1;
      }
    }

    /// <summary>
    /// Описание статуса
    /// </summary>
    public string StatusDescription
    {
      get
      {
        var tmp = m_current_state;

        return tmp != null ? tmp.Description : null;
      }
    }

    #endregion

    internal void Start()
    {
      lock (m_task_lock)
      {
        if (m_async != null)
          throw new InvalidOperationException("Task already executing");

        var action = (Action)this.Run;
        m_async = action.BeginInvoke(null, null);
      }
    }

    /// <summary>
    /// Ожидание завершения задачи
    /// </summary>
    /// <param name="timeout">Период ожидания</param>
    /// <returns><code>true</code>, если задача завершилась в течение указанного периода</returns>
    public bool Wait(TimeSpan timeout)
    {
      IAsyncResult res = m_async;

      if (res == null || res.IsCompleted)
        return true;

      return res.AsyncWaitHandle.WaitOne(timeout);
    }

    /// <summary>
    /// Отмена выполняющейся задачи
    /// </summary>
    public void Cancel()
    {
      this.Status = TaskStatus.Cancelling;

      if (m_task is CancelableRunBase)
        ((CancelableRunBase)m_task).OnCancel();

      this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("Status"));
    }

    /// <summary>
    /// Прерывание выполняющейся задачи
    /// </summary>
    public void Kill()
    {
      lock (m_task_lock)
      {
        if (m_running_thread == null)
          return;

        m_running_thread.Abort();
        m_running_thread = null;
        m_async = null;
      }
    }

    /// <summary>
    /// Получение текущего статуса выполняемой задачи
    /// </summary>
    /// <returns>Статус</returns>
    [NotNull]
    public ProgressState GetCurrentState()
    {
      using (m_state_lock.GetReadLock())
      {
        var ret = new ProgressState();

        if (m_current_state != null)
        {
          ret.Description = m_current_state.Description;
          ret.Percentage = m_current_state.Percentage;
        }

        return ret;
      }
    }

    /// <summary>
    /// Получение нестандартного статуса задачи
    /// </summary>
    /// <typeparam name="TState">Тип нестандартного статуса задачи</typeparam>
		/// <returns>Если задача реализует <see cref="T:System.IServiceProvider"/>, нестандартный статус,
    /// полученнй через этот интерфейс. Иначе, возвращает <code>null</code></returns>
    [CanBeNull]
    public TState GetCustomState<TState>()
      where TState : class
    {
      if (typeof(TState) == typeof(ProgressState))
        return this.GetCurrentState() as TState;

      var srv = m_task as IServiceProvider;

      if (srv == null)
        return null;
      else
        return srv.GetService<TState>();
    }

    public override string ToString()
    {
      return m_name;
    }

    private void Run()
    {
      try
      {
        m_task.ProgressChanged += this.HandleProgressChanged;

        if (m_task is ICancelableRunBase)
          ((ICancelableRunBase)m_task).CancellationCheck += this.HandleCancellationCheck;

        this.Status = TaskStatus.Running;
        this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("Status"));

        AppSwitchablePool.RegisterThread(Thread.CurrentThread);

        lock (m_task_lock)
          m_running_thread = Thread.CurrentThread;

        m_task.Run();

        this.Status = this.Status == TaskStatus.Cancelling ? TaskStatus.Cancelled : TaskStatus.Completed;
        this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("Status"));
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);
        this.Status = TaskStatus.Error;
        this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("Status"));
      }
      finally
      {
        m_task.ProgressChanged -= this.HandleProgressChanged;

        if (m_task is ICancelableRunBase)
          ((ICancelableRunBase)m_task).CancellationCheck -= this.HandleCancellationCheck;

        this.ProgressChanged = null;

        lock (m_task_lock)
        {
          m_async = null;
          m_running_thread = null;
        }

        AppSwitchablePool.UnRegisterThread(Thread.CurrentThread);

        if (m_show_buffer)
        {
          var info_buffer = this.GetCustomState<InfoBuffer>();

          if (info_buffer != null)
          {
            ShowBufferFactory(info_buffer)
              .BeginInvoke(null, null);
          }
        }
      }
    }

	  [NotNull]
	  private static Action ShowBufferFactory([NotNull] InfoBuffer info_buffer)
	  {
		  if (info_buffer == null) throw new ArgumentNullException("info_buffer");

			return () => AppManager.Notificator.ShowBuffer(Resources.TASK_RESULT, info_buffer);
	  }

	  private void HandleProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
    {
	    if (e == null) throw new ArgumentNullException("e");

	    bool update_needed = false;

      using (m_state_lock.GetWriteLock())
      {
        var old_state = m_current_state;
        m_current_state = new ProgressState();
        m_current_state.Percentage = e.ProgressPercentage;

        if (e.UserState != null)
          m_current_state.Description = e.UserState.ToString();
        else if (old_state != null)
          m_current_state.Description = old_state.Description;

        if (old_state == null || old_state.Percentage != m_current_state.Percentage
          || m_current_state.Description != old_state.Description)
          update_needed = true;

        //if (old_state == null)
        //{
        //  this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs(""));
        //}
        //else
        //{
        //  if (m_current_state.Description != old_state.Description)
        //    this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("StatusDescription"));

        //  if (m_current_state.Percentage != old_state.Percentage)
        //    this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs("Percentage"));
        //}
      }

      this.ProgressChanged.InvokeSynchronized(sender, e);

      if (update_needed)
        this.PropertyChanged.InvokeSynchronized(this, new PropertyChangedEventArgs(""));
    }

    private void HandleCancellationCheck(object sender, [NotNull] CancelEventArgs e)
    {
	    if (e == null) throw new ArgumentNullException("e");

	    e.Cancel = this.Status == TaskStatus.Cancelling;
    }
  }

  /// <summary>
  /// Интерфейс для пользовательского интерфейса задачи
  /// </summary>
  public interface ITaskView : ISynchronizeProvider
  {
    /// <summary>
    /// Показывает диалог с индикатором прогресса выполнения
    /// </summary>
    /// <param name="task">Задача, для которой показывается диалог</param>
    /// <returns><code>true</code>, если диалог закрыт по завершению задачи. <code>false</code>, если
    /// диалог закрыт по отмене или сворачиванию</returns>
    bool ShowProgressIndicator([NotNull] TaskInfo task);
  }

  public class TaskManager : ITaskManager
  {
	  [NotNull] private readonly ITaskView m_view;
	  [NotNull] private readonly List<TaskInfo> m_running_tasks = new List<TaskInfo>();
	  [NotNull] private readonly LockSource m_lock = new LockSource();

    public TaskManager([NotNull] ITaskView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    #region ITaskManager Members

    public bool Run([NotNull] IRunBase runBase, [NotNull] LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      var task = new TaskInfo(runBase, parameters, false);
      this.AddTaskInfo(task);
      task.Start();

      bool completed = task.Wait(this.SyncTimeout);

      if (!completed)
        completed = m_view.ShowProgressIndicator(task);
      else
      {
        this.RemoveTask(task.TaskGuid);
        return task.Status == TaskStatus.Completed;
      }

      if (!completed)
      {
        new Func<TaskInfo, bool>(this.WaitForCompletion).BeginInvoke(task, null, null);
        return false;
      }
      else
      {
        this.RemoveTask(task.TaskGuid);
        return task.Status == TaskStatus.Completed;
      }
    }

	  [NotNull]
	  public TaskInfo Start([NotNull] IRunBase runBase, [NotNull] LaunchParameters parameters, [CanBeNull] Action onCompleted) 
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      var task = new TaskInfo(runBase, parameters, true);
      this.AddTaskInfo(task);
      task.Start();

      new Func<TaskInfo, bool>(this.WaitForCompletion).BeginInvoke(task, this.HandleRunBaseCompleted, onCompleted);

      if (parameters.Modal && !task.Wait(this.SyncTimeout))
        m_view.ShowProgressIndicator(task);

      return task;
    }

    private void HandleRunBaseCompleted([NotNull] IAsyncResult result)
    {
	    if (result == null) throw new ArgumentNullException("result");

	    var action = result.AsyncState as Action;

      if (action != null && this.EndWait(result))
        new SynchronizeOperationWrapper(m_view.Invoker).Invoke(action);
    }

    private bool EndWait([NotNull] IAsyncResult result)
    {
	    if (result == null) throw new ArgumentNullException("result");

	    var res = result as AsyncResult;

      if (res == null)
        return false;

      var func = res.AsyncDelegate as Func<TaskInfo, bool>;

      if (func == null)
        return false;

      return func.EndInvoke(result);
    }

    private bool WaitForCompletion([NotNull] TaskInfo task)
    {
	    if (task == null) throw new ArgumentNullException("task");

	    if (task.Wait(/*Timeout.InfiniteTimeSpan — в .NET 4.5*/new TimeSpan(0, 0, 0, 0, -1)))
        this.RemoveTask(task.TaskGuid);

      return task.Status == TaskStatus.Completed;
    }

    public TimeSpan SyncTimeout { get; set; }

	  [NotNull]
	  public ReadOnlyCollection<TaskInfo> GetRunningTasks()
    {
      using (m_lock.GetReadLock())
      {
        return new ReadOnlyCollection<TaskInfo>(m_running_tasks.ToArray());
      }
    }

    public event EventHandler TaskListChanged;

    #endregion

    private void AddTaskInfo([NotNull] TaskInfo task)
    {
	    if (task == null) throw new ArgumentNullException("task");

	    using (m_lock.GetWriteLock())
      {
        m_running_tasks.Add(task);
        this.OnTaskListChanged();
      }
    }

	  private void OnTaskListChanged()
    {
      if (this.TaskListChanged != null)
      {
        foreach (EventHandler handler in this.TaskListChanged.GetInvocationList())
        {
          var invoker = handler.Target as ISynchronizeInvoke ?? m_view.Invoker;

          if (invoker.InvokeRequired)
            invoker.BeginInvoke(handler, new object[] { this, EventArgs.Empty });
          else
            handler(this, EventArgs.Empty);
        }
      }
    }

    private void RemoveTask(Guid taskGuid)
    {
      using (m_lock.GetWriteLock())
      {
        var task = m_running_tasks.SingleOrDefault(t => t.TaskGuid == taskGuid);

        if (task != null)
        {
          m_running_tasks.Remove(task);
          this.OnTaskListChanged();
        }
      }
    }
  }

  internal class TaskViewStub : SynchronizeProviderStub, ITaskView
  {
    #region ITaskView Members

    public bool ShowProgressIndicator([NotNull] TaskInfo task)
    {
	    if (task == null) throw new ArgumentNullException("task");

	    task.Wait(/*Timeout.InfiniteTimeSpan — в .NET 4.5*/new TimeSpan(0, 0, 0, 0, -1));

      return task.Status == TaskStatus.Completed;
    }

    #endregion
  }
}
