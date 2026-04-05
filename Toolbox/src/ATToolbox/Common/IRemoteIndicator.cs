using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;

namespace AT.Toolbox
{
  /// <summary>
  /// Индикатор прогресса удалённой операции. Этот интерфейс должен
  /// быть обёрткой над конкретными сервисами, так как для каждого
  /// вида сервиса задача должна запускаться со своими параметрами,
  /// о которых универсальный клиент ничего знать не должен
  /// </summary>
  public interface IRemoteIndicator
  {
    /// <summary>
    /// Запускает удалённую операцию на выполнение 
    /// и немедленно возвращает её идентификатор,
    /// не дожидаясь завершения операции
    /// </summary>
    /// <returns>Идентификатор удалённой операции</returns>
    Guid CreateWork();

    /// <summary>
    /// Получает текущий статус удалённо выполняемой операции
    /// </summary>
    /// <param name="workGuid">Идентификатор удалённой операции,
    /// полученный вызовом метода CreateWork()</param>
    /// <returns>Если операция завершилась, метод должен вернуть null.
    /// Иначе, метод должен вернуть текущий статус операции:
    /// процент выполнения и описание статуса</returns>
    ProgressState GetWorkStatus(Guid workGuid);

    /// <summary>
    /// Асинхронно отменяет выполняющуюся операцию.
    /// </summary>
    /// <param name="workGuid">Идентификатор удалённой операции,
    /// полученный вызовом метода CreateWork()</param>
    void Cancel(Guid workGuid);
  }

  /// <summary>
  /// Статус удалённой операции
  /// </summary>
  [DataContract]
  public sealed class ProgressState
  {
    /// <summary>
    /// Процент выполнения
    /// </summary>
    [DataMember]
    public int Percentage { get; set; }

    /// <summary>
    /// Описание статуса
    /// </summary>
    [DataMember]
    public string Description { get; set; }
  }

  /// <summary>
  /// Клиент удалённой задачи, получающий прогресс операции
  /// </summary>
  public class RemoteIndicatorClient
  {
    private readonly IRemoteIndicator m_indicator;

    public RemoteIndicatorClient(IRemoteIndicator indicator)
    {
      if (indicator == null)
        throw new ArgumentNullException("indicator");

      m_indicator = indicator;
      AfterCancelSleepTime = BeforeCancelSleepTime = 1000;
    }

    /// <summary>
    /// Время ожидания очередного статуса перед запросом к серверу до запроса на отмену
    /// </summary>
    public int BeforeCancelSleepTime { get; set; }

    /// <summary>
    /// Время ожидания очередного статуса перед запросом к серверу после запроса на отмену
    /// </summary>
    public int AfterCancelSleepTime { get; set; }

    public void Run(BackgroundWorker worker)
    {
      if (worker == null)
        throw new ArgumentNullException("worker");

      bool cancelled = worker.CancellationPending;

      if (cancelled)
        return;

      Guid workGuid = m_indicator.CreateWork();

      if (workGuid == Guid.Empty)
        return;

      ProgressState current_state = null;

      while (true)
      {
        var sleep_time = cancelled ? this.BeforeCancelSleepTime : this.AfterCancelSleepTime;

        if (sleep_time > 0)
          Thread.Sleep(sleep_time);
        
        current_state = m_indicator.GetWorkStatus(workGuid);

        if (current_state != null)
          worker.ReportProgress(current_state.Percentage, current_state.Description);
        else
          return;

        if (!cancelled && worker.CancellationPending)
        {
          cancelled = true;
          m_indicator.Cancel(workGuid);
        }
      }
    }
  }

  /// <summary>
  /// Обёртка над локальной задачей, позволяющая выполнять её как удалённую
  /// </summary>
  public class RemoteProgressRepositoryWrapper : IRemoteIndicator
  {
    private readonly RemoteProgressRepository m_repository;
    private readonly IRunBase m_work;

    public RemoteProgressRepositoryWrapper(RemoteProgressRepository repository,
      IRunBase work)
    {
      if (repository == null)
        throw new ArgumentNullException("repository");

      if (work == null)
        throw new ArgumentNullException("work");

      m_work = work;
      m_repository = repository;
    }

    #region IRemoteIndicator Members

    public Guid CreateWork()
    {
      var ret = m_repository.FindRunningWork(wrk => wrk.Equals(m_work));

      if (ret != Guid.Empty)
        return ret;

      return m_repository.BeginRun(m_work);
    }

    public ProgressState GetWorkStatus(Guid workGuid)
    {
      return m_repository.GetCurrentState(workGuid, 1000);
    }

    public void Cancel(Guid workGuid)
    {
      m_repository.Kill(workGuid, 100);
    }

    #endregion
  }
}
