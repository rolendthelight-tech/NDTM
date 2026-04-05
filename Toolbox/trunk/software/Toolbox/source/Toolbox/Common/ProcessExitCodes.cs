using System;
using System.Threading;
using System.ComponentModel;

namespace Toolbox.Common
{
  [Flags]
  public enum ProcessExitCodes : int
  {
    //[DisplayNameRes(reso)]
    Success = 0,
    DefaultError = 1 << 0,
    ConstructorError = 1 << 0,
    ThreadError = 1 << 0,
    NotStarted = 1 << 0,
    StartError = 1 << 0,
    StopError = 1 << 0,
    UpdateError = 1 << 0,
    UnknownError = 1 << 0,

    [Obsolete("Этот элемент предназначен для возможности распознавания кода ошибки при прерывании консольной программы с помощью <Control>+<Break>. Не используйте этот код.")]
    ControlBreak = -1073741510,
  }

  public delegate void ExitCodeSetter(ProcessExitCodes code);

  public class ProcessExitCodeSupergrass : Component
  {
    protected readonly object m_sync = new object();
    private ProcessExitCodes m_process_exit_code = ProcessExitCodes.Success;
    private readonly ExitCodeSetter m_exit_code_setter;
    private readonly ManualResetEvent m_error_signal = new ManualResetEvent(false);
    private bool m_disposed = false;

    public ProcessExitCodeSupergrass(ExitCodeSetter exitCodeSetter)
    {
      this.m_exit_code_setter = exitCodeSetter;
    }

    public ProcessExitCodeSupergrass()
      : this(null)
    {
    }

    public virtual ProcessExitCodes ProcessExitCode
    {
      get
      {
        lock (this.m_sync)
        {
          return this.m_process_exit_code;
        }
      }
      set
      {
        lock (this.m_sync)
        {
          if (this.m_process_exit_code == ProcessExitCodes.Success) // Берёт только первую ошибку
          {
            ProcessExitCodes tmp = this.m_process_exit_code = value;
            try
            {
              if (this.m_exit_code_setter != null)
                this.m_exit_code_setter(tmp);
            }
            finally
            {
              if (!this.m_disposed)
              {
                if (tmp == ProcessExitCodes.Success)
                  this.m_error_signal.Reset();
                else
                  this.m_error_signal.Set();
              }
            }
          }
        }
      }
    }

    public WaitHandle GetErrorWaitHandle
    {
      get { return this.m_error_signal; }
    }

    protected override void Dispose(bool disposing)
    {
      lock (this.m_sync)
      {
        this.m_disposed = true;
        this.m_error_signal.Close();
      }
      base.Dispose(disposing);
    }
  }

  public class ProcessExitCodeMotherSupergrass : ProcessExitCodeSupergrass
  {
    private ProcessExitCodeMotherSupergrass m_parent = null;
    private readonly object m_child_sync = new object();
    private int m_count_child = 0;
    private readonly ManualResetEvent m_not_children_signal = new ManualResetEvent(true);
    private bool m_stop_children = false;

    private ProcessExitCodeMotherSupergrass(ProcessExitCodeMotherSupergrass parent, ExitCodeSetter exitCodeSetter)
      : base(exitCodeSetter)
    {
      this.m_parent = parent;
    }

    public ProcessExitCodeMotherSupergrass()
      : this(null, null)
    {
    }

    public override ProcessExitCodes ProcessExitCode
    {
      get { return base.ProcessExitCode; }
      set
      {
        base.ProcessExitCode = value;
        if (this.m_parent != null)
          this.m_parent.ProcessExitCode = value;
      }
    }

    private void ChildDead()
    {
      lock (this.m_child_sync)
      {
        if (this.m_count_child <= 0)
          throw new InvalidOperationException("--this.m_count_child");
        checked
        {
          --this.m_count_child;
        }
        if (this.m_count_child == 0)
          this.m_not_children_signal.Set();
      }
    }

    public ProcessExitCodeMotherSupergrass GetChild()
    {
      return this.GetChild(null);
    }

    public ProcessExitCodeMotherSupergrass GetChild(ExitCodeSetter exitCodeSetter)
    {
      lock (this.m_child_sync)
      {
        if (this.m_stop_children)
          throw new InvalidOperationException("GetChild(): stop");
        checked
        {
          ++this.m_count_child;
        }
        if (this.m_count_child == 1)
          this.m_not_children_signal.Reset();
      }
      return new ProcessExitCodeMotherSupergrass(this, exitCodeSetter);
    }

    public ProcessExitCodes GetFirstErrorCodeWithWait()
    {
      lock (this.m_child_sync)
      {
        this.m_stop_children = true;
      }
      WaitHandle.WaitAny(new WaitHandle[]
        {
          this.m_not_children_signal,
          this.GetErrorWaitHandle
        });
        return this.ProcessExitCode;
    }

    public WaitHandle GetChildrenWaitHandle
    {
      get
      {
        lock (this.m_child_sync)
        {
          this.m_stop_children = true;
        }
        return this.m_not_children_signal;
      }
    }

    protected override void Dispose(bool disposing)
    {
      lock (this.m_sync)
      {
        if (this.m_parent != null)
        {
          this.m_parent.ChildDead();
          this.m_parent = null;
        }
      }
      lock (this.m_child_sync)
      {
        this.m_not_children_signal.Close();
      }
      base.Dispose(disposing);
    }
  }

  public sealed class Disposer : Component
  {
    private readonly Action<bool> m_action = null;
    private readonly object m_sync = new object();
    private bool m_first = true;

    public Disposer(Action<bool> action)
    {
      this.m_action = action;
    }

    protected override void Dispose(bool disposing)
    {
      bool first;
      lock (this.m_sync)
      {
        first = this.m_first;
        if (this.m_first)
          this.m_first = false;
      }
      try
      {
        if (this.m_action != null)
          this.m_action(first);
      }
      finally
      {
        base.Dispose(disposing);
      }
    }
  }
}
