using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using log4net;

namespace AT.Toolbox
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
    void ShowBuffer(string summary, InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    void ShowBuffer(InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    bool Confirm(InfoBuffer buffer);

    /// <summary>
    /// Показывает список сообщений
    /// </summary>
    /// <param name="summary">Сообщение обобщающее список сообщений</param>
    /// <param name="buffer">Список сообщений для отображения</param>
    /// <returns>Получено ли подтверждение от пользователя на продолжение операции</returns>
    bool Confirm(string summary, InfoBuffer buffer);

    /// <summary>
    /// Показывает сообщение с возможностью принятия или отмены пользователем
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>True, если сообщение принято. False, если сообщение отклонено</returns>
    bool Confirm(Info message);

    /// <summary>
    /// Показывает одно сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    void ShowMessage(Info message);

    /// <summary>
    /// Помещает сообщение на визуализатор протокола
    /// </summary>
    /// <param name="source">Источник данных</param>
    /// <param name="info">Сообщение</param>
    void Log(string source, Info info);

    /// <summary>
    /// Происходит при добавлении сообщения вызовом метода Log
    /// </summary>
    event EventHandler<InfoSourceEventArgs> LogUpdated;
  }

  /// <summary>
  /// Агрумент события добавления записи в лог
  /// </summary>
  public sealed class InfoSourceEventArgs : EventArgs
  {
    private readonly Info m_info;
    private readonly string m_source;

    internal InfoSourceEventArgs(string source, Info info)
    {
      if (string.IsNullOrEmpty(source))
        source = "AppFramework";

      if (info == null)
        throw new ArgumentNullException("info");

      m_source = source;
      m_info = info;
    }

    /// <summary>
    /// Источник сообщения
    /// </summary>
    public string Source
    {
      get { return m_source; }
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public Info Info
    {
      get { return m_info; }
    }
  }

  public class Notificator : INotificator
  {
    private readonly INotificationView m_view;
    //private readonly ThreadField<bool> m_disable_log = new ThreadField<bool>(false);

    public Notificator(INotificationView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }
    
    #region INotificator Members

    public event EventHandler<InfoSourceEventArgs> LogUpdated;

    public void ShowBuffer(string summary, InfoBuffer buffer)
    {
      this.ShowBufferImpl(summary, buffer, false);
    }

    public void ShowBuffer(InfoBuffer buffer)
    {
      this.ShowBufferImpl(null, buffer, false);
    }

    public bool Confirm(InfoBuffer buffer)
    {
      return this.ShowBufferImpl(null, buffer, true);
    }

    public bool Confirm(string summary, InfoBuffer buffer)
    {
      return this.ShowBufferImpl(summary, buffer, true);
    }

    public void ShowMessage(Info message)
    {
      if (message == null)
        return;

      this.ShowBufferImpl(null, new InfoBuffer { message }, false);
    }

    public bool Confirm(Info message)
    {
      if (message == null)
        return true;

      return this.ShowBufferImpl(null, new InfoBuffer { message }, true);
    }

    public void Log(string source, Info info)
    {
      this.Log(source, info, true);
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      this.LogUpdated = null;
    }

    #endregion

    #region Implementation

    private bool ShowBufferImpl(string summary, InfoBuffer buffer, bool confirm)
    {
      if (buffer == null || buffer.Count == 0)
        return confirm;

      foreach (var info in buffer)
        this.LogMessage(ApplicationInfo.MainAttributes.ProductName, info);

      var wrapper = new SynchronizeOperationWrapper(m_view.Invoker);

      if (buffer.Count == 1 && buffer[0].InnerMessages.Count == 0
        && string.IsNullOrEmpty(summary))
        return wrapper.Invoke(() => m_view.Alert(buffer[0], confirm));
      else
        return wrapper.Invoke(() => m_view.Alert(summary, buffer, confirm));
    }

    private void Log(string source, Info info, bool recursive)
    {
      //if (m_disable_log.Instance)
      //  return;

      //m_disable_log.Instance = true;
      //try
      //{
      //  this.WriteToLogger(LogManager.GetLogger(source), info, recursive);
      //}
      //finally
      //{
      //  m_disable_log.Instance = false;
      //}
      // TODO: взаимодействие с log4net должно быть тут, а не в LogListerPresenter
      this.RaiseLogUpdated(new InfoSourceEventArgs(source, info));
    }

    private void RaiseLogUpdated(InfoSourceEventArgs args)
    {
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

    private void LogMessage(string source, Info info)
    {
      this.Log(source, info, false);

      foreach (var inner in info.InnerMessages)
        this.LogMessage(source, inner);
    }

    private void WriteToLogger(log4net.ILog logger, Info info, bool recursive)
    {
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
  }


  public interface INotificationView : ISynchronizeProvider
  {
    bool Alert(Info info, bool confirm);

    bool Alert(string summary, InfoBuffer buffer, bool confirm);
  }

  internal class NotificationViewStub : SynchronizeProviderStub, INotificationView
  {
    #region INotificationView Members

    public bool Alert(Info info, bool confirm)
    {
      return confirm;
    }

    public bool Alert(string summary, InfoBuffer buffer, bool confirm)
    {
      return confirm;
    }

    #endregion
  }
}
