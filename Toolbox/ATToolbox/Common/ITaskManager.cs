using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using log4net;

namespace AT.Toolbox
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
    void Start(IRunBase runBase, LaunchParameters parameters, Action onCompleted);

    ReadOnlyCollection<TaskInfo> GetRunningTasks();

    event EventHandler TaskListChanged;
  }

  public enum TaskStatus
  {
    [DisplayNameRes("PENDING", typeof(AT.Toolbox.Properties.Resources))]
    Pending,
    [DisplayNameRes("RUNNING", typeof(AT.Toolbox.Properties.Resources))]
    Running,
    [DisplayNameRes("CANCELLING", typeof(AT.Toolbox.Properties.Resources))]
    Cancelling,
    [DisplayNameRes("CANCELLED", typeof(AT.Toolbox.Properties.Resources))]
    Cancelled,
    [DisplayNameRes("COMPLETED", typeof(AT.Toolbox.Properties.Resources))]
    Completed,
    [DisplayNameRes("ERROR", typeof(AT.Toolbox.Properties.Resources))]
    Error
  }

  /// <summary>
  /// Обёртка над выполняющейся задачей
  /// </summary>
  public class TaskInfo : INotifyPropertyChanged
  {
    private static readonly ILog _log = LogManager.GetLogger("TaskInfo");
    
    private readonly LaunchParameters m_parameters;
    private readonly IRunBase m_task;
    private readonly LockSource m_state_lock = new LockSource();
    private readonly object m_task_lock = new object();
    private ProgressState m_current_state;
    private IAsyncResult m_async;
    private Thread m_running_thread;
    
    internal TaskInfo(IRunBase task, LaunchParameters parameters)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_task = task;
      m_parameters = parameters;
      this.TaskGuid = Guid.NewGuid();
    }

    /// <summary>
    /// Событие об изменении прогресса выполнения
    /// </summary>
    public event EventHandler ProgressChanged;

    /// <summary>
    /// Событие об изменении свойства (статуса)
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal Guid TaskGuid { get; private set; }

    /// <summary>
    /// Название выполняющейся задачи
    /// </summary>
    public string Name
    {
      get { return m_parameters.Name ?? m_task.ToString(); }
    }

    /// <summary>
    /// Статус выполняющейся задачи
    /// </summary>
    public TaskStatus Status { get; private set; }

    internal void Start()
    {
      lock (m_task_lock)
      {
        if (m_async != null)
          throw new InvalidOperationException("Task already executing");

        var action = new Action(this.Run);
        m_async = action.BeginInvoke(null, null);
      }
    }

    /// <summary>
    /// Получение настроек для диалога с индикатором прогресса
    /// </summary>
    /// <returns>Настройки индикатора прогресса</returns>
    public LaunchParameters GetLaunchParameters()
    {
      return new LaunchParameters
      {
        Name = m_parameters.Name,
        CanCancel = m_parameters.CanCancel,
        CloseOnFinish = m_parameters.CloseOnFinish,
        Icon = m_parameters.Icon,
        SupportsPercentNotification = m_parameters.SupportsPercentNotification,
        Weight = m_parameters.Weight,
        CanMinimize = m_parameters.CanMinimize
      };
    }

    /// <summary>
    /// Ожидание завершения задачи
    /// </summary>
    /// <param name="timeout">Период ожидания</param>
    /// <returns>True, если задача завершилась в течение указанного периода</returns>
    public bool Wait(int timeout)
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

    private void Run()
    {
      try
      {
        m_task.ProgressChanged += this.HandleProgressChanged;
        m_task.CancellationCheck += this.HandleCancellationCheck;

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
        m_task.CancellationCheck -= this.HandleCancellationCheck;

        this.ProgressChanged = null;

        lock (m_task_lock)
        {
          m_async = null;
          m_running_thread = null;
        }

        AppSwitchablePool.UnRegisterThread(Thread.CurrentThread);
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      using (m_state_lock.GetWriteLock())
      {
        var old_state = m_current_state;
        m_current_state = new ProgressState();
        m_current_state.Percentage = e.ProgressPercentage;

        if (e.UserState != null)
          m_current_state.Description = e.UserState.ToString();
        else
          m_current_state.Description = old_state.Description;
      }

      this.ProgressChanged.InvokeSynchronized(sender, e);
    }

    private void HandleCancellationCheck(object sender, CancelEventArgs e)
    {
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
    /// <returns>True, если диалог закрыт по завершению задачи. False, если
    /// диалог закрыт по отмене или сворачиванию</returns>
    bool ShowProgressIndicator(TaskInfo task);
  }

  public class TaskManager : ITaskManager
  {
    private readonly ITaskView m_view;
    private readonly List<TaskInfo> m_running_tasks = new List<TaskInfo>();
    private readonly LockSource m_lock = new LockSource();

    public TaskManager(ITaskView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    #region ITaskManager Members

    public bool Run(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      var task = new TaskInfo(runBase, parameters);
      this.AddTaskInfo(task);
      task.Start();

      bool completed = task.Wait((int)this.SyncTimeout.TotalMilliseconds);

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

    public void Start(IRunBase runBase, LaunchParameters parameters, Action onCompleted) 
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      var task = new TaskInfo(runBase, parameters);
      this.AddTaskInfo(task);
      task.Start();

      new Func<TaskInfo, bool>(this.WaitForCompletion).BeginInvoke(task, this.HandleRunBaseCompleted, onCompleted);
    }

    private void HandleRunBaseCompleted(IAsyncResult result)
    {
      var action = result.AsyncState as Action;

      if (action != null)
        new SynchronizeOperationWrapper(m_view.Invoker).Invoke(action);
    }

    private bool WaitForCompletion(TaskInfo task)
    {
      if (task.Wait(Timeout.Infinite))
        this.RemoveTask(task.TaskGuid);

      return task.Status == TaskStatus.Completed;
    }

    public TimeSpan SyncTimeout { get; set; }

    public ReadOnlyCollection<TaskInfo> GetRunningTasks()
    {
      using (m_lock.GetReadLock())
      {
        return new ReadOnlyCollection<TaskInfo>(m_running_tasks.ToArray());
      }
    }

    public event EventHandler TaskListChanged;

    #endregion

    private void AddTaskInfo(TaskInfo task)
    {
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

    public bool ShowProgressIndicator(TaskInfo task)
    {
      return true;
    }

    #endregion
  }
}
