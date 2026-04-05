using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AT.Toolbox;
using log4net;

namespace AT.Toolbox.Controls
{
  public class LogListerPresenter :  MessageLogAppender.IDestination
  {
    private readonly ILogListerView m_view;
    private readonly MessageLogAppender.IDestination m_previous_destination;
    private readonly BindingList<LogEntry> m_entries;
    private readonly HashSet<int> m_skip_threads = new HashSet<int>();

    public LogListerPresenter(ILogListerView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      
      MaxMessages = 1000;
      m_view = view;

      m_previous_destination = MessageLogAppender.Destination;

      m_view.Disposed += OnViewDisposed;
      MessageLogAppender.Destination = this;

      m_entries = new BindingList<LogEntry>();
      m_view.DataSource = m_entries;
    }

    public void Load()
    {
      AppManager.Notificator.LogUpdated += this.HandleLogUpdated;
    }

    void HandleLogUpdated(object sender, InfoSourceEventArgs e)
    {
      this.Log(e.Source, e.Info);
    }

    void OnViewDisposed(object sender, EventArgs e)
    {
      MessageLogAppender.Destination = m_previous_destination;
      AppManager.Notificator.LogUpdated -= this.HandleLogUpdated;
    }

    private HashSet<Info> m_pending_infos = new HashSet<Info>();

    public int MaxMessages { get; set; }

    public void Log(string source, Info info)
    {
      var thread_id = Thread.CurrentThread.ManagedThreadId;

      lock (m_skip_threads)
        m_skip_threads.Add(thread_id);

      try
      {
        this.LogMessage(LogManager.GetLogger(source), info);
      }
      finally
      {
        lock (m_skip_threads)
          m_skip_threads.Remove(thread_id);
      }

      this.Append(source, info);
    }

    bool MessageLogAppender.IDestination.Skip
    {
      get { return m_skip_threads.Contains(Thread.CurrentThread.ManagedThreadId); }
    }

    public void Append(string source, Info info)
    {
      // Здесь нужно только поместить данные в панель на пользовательском интерфейсе
      if (m_view.InvokeRequired)
      {
        m_view.BeginInvoke(new Action<string, Info>(this.Append),
          new object[] { source, info });

        return;
      }

      if (info.Level < m_view.Treshold)
        return;

      var log_entry = new LogEntry
                        {
                          EventDate = DateTime.UtcNow,
                          InnerMessages = info.InnerMessages.ToArray(),
                          Level = info.Level,
                          Message = info.Message,
                          Source = source,
                          Tag = info.Details,
                          Link = info.Link
                        };

      while (m_entries.Count >= MaxMessages)
        m_entries.RemoveAt(0);

      m_entries.Add(log_entry);

      if (m_view.DataSource == null)
        m_view.DataSource = m_entries;
    }

    private void LogMessage(log4net.ILog logger, Info info)
    {
      foreach (var inner in info.InnerMessages)
        this.LogMessage(logger, inner);

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
      }
    }
  }
}