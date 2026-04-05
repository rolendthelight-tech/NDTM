using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using log4net;

namespace Toolbox.Common
{
  /// <summary>
  /// Контекст выполнения команд
  /// </summary>
  public interface IBackgroundContext
  {
    /// <summary>
    /// Требуется ли прервать выполнение команды
    /// </summary>
    bool Skip { get; }

    /// <summary>
    /// Обёртка над операциями, которые могут требовать синхронизации
    /// </summary>
    IOperationWrapper Invoker { get; }
  }

  /// <summary>
  /// Фоновый обработчик очереди команд
  /// </summary>
  public class BackgroundLoader : IDisposable
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(BackgroundLoader));

    private readonly EventWaitHandle m_load_signal;
    private readonly BackgroundContext m_context;
    private readonly List<Action<IBackgroundContext>> m_actions;
    private volatile bool m_stopped;

    /// <summary>
    /// Инициализация очереди
    /// </summary>
    /// <param name="invoker">Объект синхронизации</param>
    public BackgroundLoader(ISynchronizeInvoke invoker)
      : this(new SynchronizeOperationWrapper(invoker)) { }

    /// <summary>
    /// Инициализация очереди
    /// </summary>
    /// <param name="invoker">Обёртка над операциями</param>
    public BackgroundLoader(IOperationWrapper invoker)
    {
      m_context = new BackgroundContext(invoker);
      m_load_signal= new EventWaitHandle(false, EventResetMode.AutoReset);
      m_actions = new List<Action<IBackgroundContext>>();

      var th = new Thread(this.LoadingProcess)
        {
          Name = "BackgroundLoader",
        };
      th.Start();
    }

    /// <summary>
    /// Инициализация очереди без синхронизации
    /// </summary>
    public BackgroundLoader() : this(new EmptyOperationWrapper()) { }

    /// <summary>
    /// Если это свойство установить в <code>true</code>, выполняться будет не только последняя команда, а все.
    /// </summary>
    public bool RunAllActions { get; set; }

    /// <summary>
    /// Добавление команды в очередь выполнения
    /// </summary>
    /// <param name="action">Добавляемая команда</param>
    public void AddAction(Action<IBackgroundContext> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      if (m_stopped)
        throw new ObjectDisposedException("BackgroundLoader");

      lock (m_actions)
      {
        m_context.SkipField = true;
        m_actions.Add(action);
        m_load_signal.Set();
      }
    }

    /// <summary>
    /// Завершение работы очереди
    /// </summary>
    public void Dispose()
    {
      if (m_stopped)
        return;

      m_stopped = true;
      m_context.SkipField = true;
      m_load_signal.Set();
    }

    #region Implementation

    private void LoadingProcess()
    {
      while (m_load_signal.WaitOne(Timeout.Infinite))
      {
        if (m_stopped)
          break;

        Action<IBackgroundContext> action = null;

        lock (m_actions)
        {
          if (m_actions.Count == 0)
            continue;

          if (this.RunAllActions)
          {
            action = m_actions[0];
            m_actions.RemoveAt(0);
          }
          else
          {
            action = m_actions[m_actions.Count - 1];
            m_actions.Clear();
          }
        }

        if (action != null)
        {
          m_context.SkipField = m_stopped;

          try
          {
            action(m_context);
          }
          catch (Exception ex)
          {
            _log.Error("LoadingProcess: exception", ex);
          }
        }
      }

      m_load_signal.Close();
    }

    private class BackgroundContext : IBackgroundContext
    {
      private readonly IOperationWrapper m_invoker;

      public BackgroundContext(IOperationWrapper invoker)
      {
        if (invoker == null)
          throw new ArgumentNullException("invoker");

        m_invoker = invoker;
      }

      #region IBackgroundContext Members

      public volatile bool SkipField;

      public bool Skip
      {
        get { return this.SkipField; }
      }

      public IOperationWrapper Invoker
      {
        get { return m_invoker; }
      }

      #endregion

      public override string ToString()
      {
        return string.Format("Invoker: {0}, Skipped: {1}", this.Invoker, this.Skip);
      }
    }

    #endregion
  }
}
