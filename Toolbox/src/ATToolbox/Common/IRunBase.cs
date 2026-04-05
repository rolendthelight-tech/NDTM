using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace AT.Toolbox
{
  /// <summary>
  /// Предоставляет возможность выполнения задачи
  /// как в пользовательском интерфейсе, так и через службу процессов
  /// </summary>
  public interface IRunBase
  {
    /// <summary>
    /// Возникает, когда задача запрашивает, была ли она прервана
    /// </summary>
    event CancelEventHandler CancellationCheck;

    /// <summary>
    /// Возникает, когда задача оповещает внешний объект о прогрессе операции
    /// </summary>
    event ProgressChangedEventHandler ProgressChanged;

    /// <summary>
    /// Запускает задачу на выполнение
    /// </summary>
    void Run();
  }

  /// <summary>
  /// Контекст выполнения задачи
  /// </summary>
  public interface IRunningContext
  {
    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    /// <param name="runBase">Выполняемая задача</param>
    /// <returns>True, если выполнение успешно. Иначе, false</returns>
    bool Run(IRunBase runBase, LaunchParameters parameters);
  }

  public class LaunchParameters
  {
    public LaunchParameters()
    {
      this.CloseOnFinish = true;
      this.Weight = 1;
    }
    
    public string Name { get; set; }

    public bool CloseOnFinish { get; set; }

    public bool CanCancel { get; set; }

    public bool CanMinimize { get; set; }

    public bool SupportsPercentNotification { get; set; }

    public Bitmap Icon { get; set; }

    public float Weight { get; set; }
  }

  /// <summary>
  /// Базовый класс для объектов, поддерживающих оповещение о прогрессе операции
  /// </summary>
  public abstract class ProgressIndicator
  {
    private volatile int m_progress;
    private string m_state;

    #region Events

    public event CancelEventHandler CancellationCheck;

    public event ProgressChangedEventHandler ProgressChanged;

    #endregion

    /// <summary>
    /// Проверяет, была ли задача отменена пользователем
    /// </summary>
    protected bool CancellationPending
    {
      get
      {
        if (this.CancellationCheck == null)
          return false;

        var ea = new CancelEventArgs(false);
        this.CancellationCheck(this, ea);
        return ea.Cancel;
      }
    }

    /// <summary>
    /// Оповещает о прогрессе операции
    /// </summary>
    /// <param name="percentage">Процент выплонения</param>
    /// <param name="state">Сообщение для оповещения</param>
    protected void ReportProgress(int percentage, string state)
    {
      m_progress = percentage;
      m_state = state;

      if (this.ProgressChanged != null)
        this.ProgressChanged(this, new ProgressChangedEventArgs(percentage, state));
    }

    /// <summary>
    /// Оповещает о прогрессе операции
    /// </summary>
    /// <param name="state">Сообщение для оповещения</param>
    protected void ReportProgress(string state)
    {
      if (m_state != state)
        this.ReportProgress(m_progress, state);
    }

    /// <summary>
    /// Оповещает о прогрессе операции
    /// </summary>
    /// <param name="percentage">Процент выплонения</param>
    protected void ReportProgress(int percentage)
    {
      if (m_progress != percentage)
        this.ReportProgress(percentage, null);
    }
  }

  /// <summary>
  /// Базовая реализация универсальной задачи
  /// </summary>
  public abstract class RunBase : ProgressIndicator, IRunBase
  {
    #region IRunBase Members

    public abstract void Run();

    #endregion
  }

  public class RunningContextStub : IRunningContext
  {
    #region IRunningContext Members

    public bool Run(IRunBase runBase, LaunchParameters parameters)
    {
      runBase.Run();
      return true;
    }

    #endregion
  }
}
