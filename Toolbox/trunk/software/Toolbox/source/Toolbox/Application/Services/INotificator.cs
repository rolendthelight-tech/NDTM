using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Log;
using Toolbox.Threading;
using log4net;
using System.Threading;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Оповещение пользователя
  /// </summary>
  public interface INotificator : IDisposable
  {
    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="summary">Сообщение обобщающее список сообщений</param>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    void ShowBuffer([CanBeNull] string summary, [NotNull] InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    void ShowBuffer([NotNull] InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    bool Confirm([NotNull] InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="summary">Сообщение обобщающее список сообщений</param>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    bool Confirm([CanBeNull] string summary, [NotNull] InfoBuffer buffer);

    /// <summary>
    /// Показывает сообщение с возможностью принятия или отмены пользователем
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns><code>true</code>, если сообщение принято. <code>false</code>, если сообщение отклонено</returns>
    bool Confirm([NotNull] Info message);

    /// <summary>
    /// Показывает одно сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    void ShowMessage([NotNull] Info message);

    /// <summary>
    /// Помещает сообщение на визуализатор протокола
    /// </summary>
    /// <param name="source">Источник данных</param>
    /// <param name="info">Сообщение</param>
    void Log([NotNull] string source, [NotNull] Info info);

    /// <summary>
    /// Получение списка последних сообщений, записанных в лог
    /// </summary>
    /// <returns>Список последних сообщений. Количество сообщений
    /// получается через INotificatorView.LoggingQueueSize</returns>
    [NotNull]
    IList<InfoSourceEventArgs> GetLastLoggedEvents();

    /// <summary>
    /// Происходит при добавлении сообщения вызовом метода Log
    /// </summary>
    event EventHandler<InfoSourceEventArgs> LogUpdated;
  }

  /// <summary>
  /// Аргумент события добавления записи в лог
  /// </summary>
  public sealed class InfoSourceEventArgs : EventArgs
  {
	  [NotNull] private readonly Info m_info;
	  private readonly DateTime m_time_stamp;
	  [NotNull] private readonly string m_source;
	  [NotNull] private readonly string m_thread_name;

		internal InfoSourceEventArgs([CanBeNull] string source, [NotNull] Info info, [CanBeNull] string threadName, DateTime timeStamp)
    {
      if (string.IsNullOrEmpty(source))
        source = "AppFramework";

      if (info == null)
        throw new ArgumentNullException("info");

      m_source = source;
      m_info = info;
			m_time_stamp = timeStamp;
			m_thread_name =
				threadName
				?? (string.IsNullOrEmpty(Thread.CurrentThread.Name)
					    ? Thread.CurrentThread.ManagedThreadId.ToString()
					    : Thread.CurrentThread.Name);
    }

    /// <summary>
    /// Источник сообщения
    /// </summary>
    [NotNull]
    public string Source
    {
      get { return m_source; }
    }

    /// <summary>
    /// Имя потока, в котором произошло событие
    /// </summary>
    [NotNull]
    public string ThreadName
    {
      get { return m_thread_name; }
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    [NotNull]
    public Info Info
    {
      get { return m_info; }
    }

		/// <summary>
		/// Время возникновения сообщения
		/// </summary>
		public DateTime TimeStamp
	  {
		  get { return m_time_stamp; }
	  }
  }

  public class Notificator : INotificator, MessageLogAppender.IDestination
  {
	  [NotNull] private readonly INotificationView m_view;
	  [NotNull] private readonly ThreadField<bool> m_disable_log = new ThreadField<bool>(false);
	  [NotNull] private readonly Queue<InfoSourceEventArgs> m_log_queue = new Queue<InfoSourceEventArgs>();

    public Notificator([NotNull] INotificationView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    #region INotificator Members

    public event EventHandler<InfoSourceEventArgs> LogUpdated;

    public void ShowBuffer(string summary, InfoBuffer buffer)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

			this.ShowBufferImpl(summary, buffer, false);
    }

	  public void ShowBuffer(InfoBuffer buffer)
	  {
		  if (buffer == null) throw new ArgumentNullException("buffer");

			this.ShowBufferImpl(null, buffer, false);
	  }

	  public bool Confirm(InfoBuffer buffer)
	  {
		  if (buffer == null) throw new ArgumentNullException("buffer");

			return this.ShowBufferImpl(null, buffer, true);
	  }

	  public bool Confirm(string summary, InfoBuffer buffer)
	  {
		  if (buffer == null) throw new ArgumentNullException("buffer");

			return this.ShowBufferImpl(summary, buffer, true);
	  }

	  public void ShowMessage(Info message)
    {
	    if (message == null) throw new ArgumentNullException("message");

	    this.ShowBufferImpl(null, new InfoBuffer { message }, false);
    }

	  public bool Confirm(Info message)
	  {
		  if (message == null) throw new ArgumentNullException("message");

		  return this.ShowBufferImpl(null, new InfoBuffer { message }, true);
	  }

	  public void Log(string source, Info info)
	  {
		  if (source == null) throw new ArgumentNullException("source");
		  if (info == null) throw new ArgumentNullException("info");

			this.Log(source, info, true, DateTime.Now/*TODO*/);
	  }

	  [NotNull]
	  public IList<InfoSourceEventArgs> GetLastLoggedEvents()
    {
      lock (m_log_queue)
      {
        return m_log_queue.ToList();
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      this.LogUpdated = null;
    }

    #endregion

    #region Implementation

		[Obsolete("Замените на версию с timeStamp")]
		private bool ShowBufferImpl([CanBeNull] string summary, [NotNull] InfoBuffer buffer, bool confirm)
		{
			return ShowBufferImpl(summary, buffer, confirm, DateTime.Now);
		}

		private bool ShowBufferImpl([CanBeNull] string summary, [NotNull] InfoBuffer buffer, bool confirm, DateTime timeStamp)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

	    if (buffer.Count == 0)
        return confirm;

      foreach (var info in buffer)
				this.LogMessage(ApplicationInfo.MainAttributes.ProductName, info, timeStamp);

      var wrapper = new SynchronizeOperationWrapper(m_view.Invoker);

      if (buffer.Count == 1 && buffer[0].InnerMessages.Count == 0
        && string.IsNullOrEmpty(summary))
        return wrapper.Invoke(() => m_view.Alert(buffer[0], confirm));
      else
        return wrapper.Invoke(() => m_view.Alert(summary, buffer, confirm));
    }

		private void Log([NotNull] string source, [NotNull] Info info, bool recursive, DateTime timeStamp)
    {
	    if (source == null) throw new ArgumentNullException("source");
	    if (info == null) throw new ArgumentNullException("info");

			if (m_disable_log.Instance)
        return;

      m_disable_log.Instance = true;
      try
      {
        this.WriteToLogger(LogManager.GetLogger(source), info, recursive);
      }
      finally
      {
        m_disable_log.Instance = false;
      }

			this.RaiseLogUpdated(new InfoSourceEventArgs(source, info, null, timeStamp));
    }

    private void RaiseLogUpdated([NotNull] InfoSourceEventArgs args)
    {
	    if (args == null) throw new ArgumentNullException("args");

			var queue_size = m_view.LoggingQueueSize;

      if (queue_size > 0)
      {
        lock (m_log_queue)
        {
          while (m_log_queue.Count >= queue_size)
            m_log_queue.Dequeue();

          m_log_queue.Enqueue(args);
        }
      }

      var handlers = this.LogUpdated;

      if (handlers == null)
        return;

      foreach (EventHandler<InfoSourceEventArgs> handler in handlers.GetInvocationList())
      {
        var invoker = handler.Target as ISynchronizeInvoke ?? m_view.Invoker;

        if (invoker.InvokeRequired)
          invoker.BeginInvoke(handler, new object[] { this, args });
        else
          handler(this, args);
      }
    }

		private void LogMessage([NotNull] string source, [NotNull] Info info, DateTime timeStamp)
    {
	    if (source == null) throw new ArgumentNullException("source");
	    if (info == null) throw new ArgumentNullException("info");

			this.Log(source, info, false, timeStamp);

      foreach (var inner in info.InnerMessages)
				this.LogMessage(source, inner, timeStamp);
    }

    private void WriteToLogger([NotNull] log4net.ILog logger, [NotNull] Info info, bool recursive)
    {
	    if (logger == null) throw new ArgumentNullException("logger");
	    if (info == null) throw new ArgumentNullException("info");

	    if (recursive)
      {
        foreach (var inner in info.InnerMessages)
          this.WriteToLogger(logger, inner, recursive);
      }

      switch (info.Level)
      {
        case InfoLevel.Debug:
          logger.Debug(info.Message, info.Details as Exception);
          break;
        case InfoLevel.Error:
          logger.Error(info.Message, info.Details as Exception);
          break;
        case InfoLevel.Info:
          logger.Info(info.Message, info.Details as Exception);
          break;
        case InfoLevel.Warning:
          logger.Warn(info.Message, info.Details as Exception);
          break;
        case InfoLevel.FatalError:
          logger.Fatal(info.Message, info.Details as Exception);
          break;
      }
    }

    private string GetApplicationName()
    {
      if (Assembly.GetEntryAssembly() == null)
        return "AppFramework";

      return ApplicationInfo.MainAttributes.ProductName;
    }

    #endregion

    #region IDestination Members

		void MessageLogAppender.IDestination.Append([NotNull] string source, [NotNull] Info message, [NotNull] string threadName, DateTime timeStamp)
    {
	    if (source == null) throw new ArgumentNullException("source");
	    if (message == null) throw new ArgumentNullException("message");
	    if (threadName == null) throw new ArgumentNullException("threadName");

			this.RaiseLogUpdated(new InfoSourceEventArgs(source, message, threadName, timeStamp));
    }

	  bool MessageLogAppender.IDestination.Skip
    {
      get { return m_disable_log.Instance; }
    }

    #endregion
  }


  public interface INotificationView : ISynchronizeProvider
  {
    bool Alert([NotNull] Info info, bool confirm);

    bool Alert([CanBeNull] string summary, [NotNull] InfoBuffer buffer, bool confirm);

    int LoggingQueueSize { get; }
  }

  internal class NotificationViewStub : SynchronizeProviderStub, INotificationView
  {
    #region INotificationView Members

    public bool Alert(Info info, bool confirm)
    {
	    if (info == null) throw new ArgumentNullException("info");

			return confirm;
    }

	  public bool Alert(string summary, InfoBuffer buffer, bool confirm)
    {
		  if (buffer == null) throw new ArgumentNullException("buffer");

	    return confirm;
    }

	  public int LoggingQueueSize
    {
      get { return 0; }
    }

    #endregion
  }
}
