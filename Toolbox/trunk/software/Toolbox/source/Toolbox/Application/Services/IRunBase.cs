using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Toolbox.Common;
using Toolbox.Extensions;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Предоставляет возможность выполнения задачи
  /// как в пользовательском интерфейсе, так и через службу процессов
  /// </summary>
  public interface IRunBase
  {
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
  /// Предоставляет возможность отмены задачи через пользовательский интерфейс
  /// </summary>
  public interface ICancelableRunBase : IRunBase
  {
    /// <summary>
    /// Может ли задача быть отменена в текущий момент
    /// </summary>
    bool CanCancel { get; }

    /// <summary>
    /// Возникает, если изменилась возможность задачи быть отменённой
    /// </summary>
    event EventHandler CanCancelChanged;

    /// <summary>
    /// Возникает, когда задача запрашивает, была ли она прервана
    /// </summary>
    event CancelEventHandler CancellationCheck;
  }

  /// <summary>
  /// Оповещает контекст о создаваемых потоках
  /// </summary>
  public interface IMultyThreadRunBase : IRunBase
  {
		[Obsolete("Оставляет мусор в потоках")]
    event EventHandler<ParamEventArgs<Thread>> ThreadCreated;
  }

  /// <summary>
  /// Задача поддерживает оповещение о проценте выполнения
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
	[BaseTypeRequired(typeof(IRunBase))]
	[BaseTypeRequired(typeof(IReportProgress))]
  public sealed class PercentNotificationAttribute : Attribute { }

  /// <summary>
  /// Контекст выполнения задачи
  /// </summary>
  public interface IRunningContext
  {
    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    /// <param name="runBase">Выполняемая задача</param>
    /// <param name="parameters">Параметры запуска задачи</param>
		/// <returns><code>true</code>, если выполнение успешно, иначе — <code>false</code></returns>
    bool Run([NotNull] IRunBase runBase, [NotNull] LaunchParameters parameters);
  }

	/// <summary>
	/// Оповещает о прогрессе операции
	/// </summary>
	public interface IReportProgress
	{
		/// <summary>
		/// Оповещает о прогрессе операции
		/// </summary>
		/// <param name="state">Сообщение для оповещения</param>
		void ReportProgress(string state);

		/// <summary>
		/// Оповещает о прогрессе операции
		/// </summary>
		/// <param name="percentage">Процент выполнения</param>
		void ReportProgress(int percentage);

		/// <summary>
		/// Оповещает о прогрессе операции
		/// </summary>
		/// <param name="percentage">Процент выполнения</param>
		/// <param name="state">Сообщение для оповещения</param>
		void ReportProgress(int percentage, string state);
	}

	/// <summary>
  /// Набор свойств задачи, которые могут перекрыть настройки запуска и свойств задачи
  /// </summary>
  public class TaskVisualizationOverride
  {
    /// <summary>
    /// Поддерживается ли оповещение о проценте выполнения
    /// </summary>
    public bool? PercentNotification { get; set; }

    /// <summary>
    /// Вес задачи при выполнении нескольких задач как одной
    /// </summary>
    public float? Weight { get; set; }

    /// <summary>
    /// Заголовок диалога с индикатором прогресса
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Значок на диалоге с индикатором прогресса
    /// </summary>
    public Bitmap Icon { get; set; }
  }

  /// <summary>
  /// Базовая реализация универсальной задачи
  /// </summary>
  
  [Serializable]
	public abstract class RunBase : IRunBase, IReportProgress
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(RunBase));

    private volatile int m_progress;
    private string m_state;
	  [NotNull] private readonly object m_lock = new object();
    /// <summary>
    /// Порог для оповещения об изменении состояния.
    /// </summary>
    protected int m_progress_threshold;

		protected RunBase()
		{
		}

	  #region Events

    public event ProgressChangedEventHandler ProgressChanged;

    #endregion

	  private static void Check(int percentage)
		{
		  try
		  {
			  if (percentage < 0 || percentage > 100)
				  throw new ArgumentOutOfRangeException("percentage", percentage, "percentage < 0 ∨ percentage > 100");
		  }
		  catch (ArgumentOutOfRangeException ex)
		  {
			  _log.Error("Check(): ArgumentOutOfRangeException", ex);
		  }
		}

    /// <summary>
    /// Оповещает о прогрессе операции
    /// </summary>
		/// <param name="percentage">Процент выполнения</param>
    /// <param name="state">Сообщение для оповещения</param>
    /// <returns>true - при изменения прогресса</returns>
    protected bool ReportProgress(int percentage, string state)
    {
	    Check(percentage);

	    lock (m_lock)
      {
        if (Math.Abs(percentage - m_progress) < m_progress_threshold && percentage != 100)
          return false;

        m_progress = percentage;
        m_state = state;

        if (ProgressChanged != null)
          ProgressChanged(this, new ProgressChangedEventArgs(percentage, state));

        return true;
      }
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
		/// <param name="percentage">Процент выполнения</param>
    protected void ReportProgress(int percentage)
    {
			Check(percentage);

			if (m_progress != percentage)
        this.ReportProgress(percentage, null);
    }

		#region IRunBase methods

		public abstract void Run();

		#endregion

		#region IReportProgress methods

		void IReportProgress.ReportProgress(int percentage, string state)
		{
			this.ReportProgress(percentage, state);
		}

		void IReportProgress.ReportProgress(string state)
		{
			this.ReportProgress(state);
		}

		void IReportProgress.ReportProgress(int percentage)
		{
			this.ReportProgress(percentage);
		}

		#endregion

    /// <summary>
    /// Создание задачи без оповещения параметров на основе внешнего метода
    /// </summary>
    /// <param name="action">Метод, которому делегируется реализация Run</param>
    /// <returns>Задача-обёртка над методом</returns>
    [NotNull]
    public static RunBase Create([NotNull] Action action)
    {
	    if (action == null) throw new ArgumentNullException("action");

	    return new ParametricRunBase(action);
    }

	  private sealed class ParametricRunBase : RunBase
    {
      private readonly Action m_action;

			public ParametricRunBase([NotNull] Action action)
				: base()
      {
        if (action == null)
          throw new ArgumentNullException("action");

        m_action = action;
      }

      public override void Run()
      {
        m_action();
      }
    }
	}

	/// <summary>
  /// Базовая реализация универсальной задачи с поддержкой отмены
  /// </summary>
  [Serializable]
  public abstract class CancelableRunBase : RunBase, ICancelableRunBase
  {
		private bool m_can_cancel = true;

		protected CancelableRunBase()
			: base()
		{
		}

		public event EventHandler CanCancelChanged;

    public event CancelEventHandler CancellationCheck;

    /// <summary>
    /// Проверяет, была ли задача отменена пользователем
    /// </summary>
    protected bool CancellationPending
    {
      get
      {
        if (this.AutoAllowCancelOnCheck)
          this.CanCancel = true;

				if (this.CancellationCheck == null)
					return false;

				var ea = new CancelEventArgs(false);
				this.CancellationCheck(this, ea);
				return ea.Cancel;
      }
    }

    /// <summary>
    /// Указывает возможность отмены задачи
    /// </summary>
    public bool CanCancel
    {
      get { return m_can_cancel; }
      protected set
      {
        if (m_can_cancel == value)
          return;

        m_can_cancel = value;
        this.CanCancelChanged.InvokeSynchronized(this, EventArgs.Empty);
      }
    }

    public virtual void OnCancel()
    {
    }

    /// <summary>
    /// Указывает, разрешать ли отмену автоматически,
    /// когда задача проверяет, была ли она отменена
    /// </summary>
    protected virtual bool AutoAllowCancelOnCheck
    {
      get { return false; }
    }
  }

  public class RunningContextStub : IRunningContext
  {
    #region IRunningContext Members

    public bool Run([NotNull] IRunBase runBase, LaunchParameters parameters)
    {
	    if (runBase == null) throw new ArgumentNullException("runBase");

			runBase.Run();
      return true;
    }

    #endregion
  }

	/// <summary>
	/// Оболочка BackgroundWorker когда требуется IReportProgress
	/// </summary>
	public class BackgroundWorkerWrapperReportProgress : IReportProgress
	{
		private int m_percentage;
		private readonly BackgroundWorker m_background_worker;

		public BackgroundWorkerWrapperReportProgress([NotNull] BackgroundWorker backgroundWorker)
		{
			if (backgroundWorker == null) throw new ArgumentNullException("backgroundWorker");

			if (!backgroundWorker.WorkerReportsProgress)
				throw new ArgumentException("¬backgroundWorker.WorkerReportsProgress", "backgroundWorker");

			this.m_background_worker = backgroundWorker;
		}

		#region Implementation of IReportProgress

		void IReportProgress.ReportProgress(string state)
		{
			((IReportProgress)(this)).ReportProgress(m_percentage, state);
		}

		void IReportProgress.ReportProgress(int percentage, string state)
		{
			if (percentage < 0 || percentage > 100)
				throw new ArgumentOutOfRangeException("percentage", percentage, "percentage < 0 ∨ percentage > 100");

			this.m_percentage = percentage;
			this.m_background_worker.ReportProgress(percentage, state);
		}

		void IReportProgress.ReportProgress(int percentage)
		{
			((IReportProgress)(this)).ReportProgress(percentage, null);
		}

		#endregion
	}

	/// <summary>
	/// Пустышка, ничего не делающая при оповещении о прогрессе операции
	/// </summary>
	public class ReportProgressStub : IReportProgress
	{
		#region Implementation of IReportProgress

		public void ReportProgress(string state)
		{
		}

		public void ReportProgress(int percentage)
		{
		}

		public void ReportProgress(int percentage, string state)
		{
		}

		#endregion
	}
}
