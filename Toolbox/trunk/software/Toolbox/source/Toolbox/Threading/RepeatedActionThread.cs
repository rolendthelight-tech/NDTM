using System;
using System.Threading;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Threading
{
  /// <summary>
  /// Позволяет повторять в фоне действие. 
	/// Если во время выполнения действия было сделано более одного запроса <see cref="RequestPerform()"/> на выполнение,
  ///  то далее действие будет выполнено только один раз.
  /// </summary>
  public class RepeatedActionThread : IDisposable
  {
    private readonly ILog _mLog = LogManager.GetLogger(typeof (RepeatedActionThread));

    /// <summary>
    /// Действие выполнилось
    /// </summary>
    public event EventHandler ActionPerformed;

    private readonly Action action;

    private State _state;

    private readonly object _stateLocker = new object();

    private readonly Thread thread;

    private bool _cancel;

    private readonly ManualResetEvent startPerformEvent = new ManualResetEvent(false);

    public RepeatedActionThread([NotNull] Action action, [CanBeNull] string threadName = null)
    {
	    if (action == null) throw new ArgumentNullException("action");

	    this.action = action;
      _state = State.Idle;

      string name = String.IsNullOrWhiteSpace(threadName) ? "RepeatedActionThread" : threadName;

      thread = new Thread(ProcessThread) {IsBackground = true, Priority = ThreadPriority.BelowNormal, Name = name};
      thread.Start();
    }

    /// <summary>
    /// Запросить выполнение действия
    /// </summary>
    public void RequestPerform()
    {
      lock (_stateLocker)
      {
        switch (_state)
        {
          case State.Idle:
            startPerformEvent.Set();
            break;

          case State.Performing:
            _state = State.WaitForPerforming;
            break;
        }
      }
    }

    private void ProcessThread()
    {
      while (!_cancel)
      {
        startPerformEvent.WaitOne();
        lock (_stateLocker)
        {
          _state = State.Performing;
          startPerformEvent.Reset();
        }
        try
        {
          //m_log.Info("RepeatedActionThread:: run action");
          if (!_cancel)
          {
            action();

            if (ActionPerformed != null)
            {
              ActionPerformed(this, EventArgs.Empty);
            }
          }
        }
        catch (Exception e)
        {
          _mLog.Error("Ошибка выполнения действия", e);
        }

        lock (_stateLocker)
        {
          if (!_cancel)
          {
            switch (_state)
            {
              case State.WaitForPerforming:
                startPerformEvent.Set();
                break;

              default:
                _state = State.Idle;
                break;
            }
          }
        }
      }
    }

    private enum State
    {
      Idle,
      Performing,
      WaitForPerforming
    }

    public void Dispose()
    {
      try
      {
        lock (_stateLocker)
        {
          //if (_state != State.Idle)
          {
            _cancel = true;
            startPerformEvent.Set();
          }
        }
      }
      catch (Exception e)
      {
				_mLog.Error("Dispose(): exception", e);
			}
    }
  }
}
