using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Toolbox.Application.Services;
using Toolbox.Log;

namespace Toolbox.GUI.Controls.Log
{
  public class LogListerPresenter 
  {
    private readonly ILogListerView m_view;
    private readonly MessageLogAppender.IDestination m_previous_destination;
    private readonly BindingList<LogEntry> m_entries;

    public LogListerPresenter(ILogListerView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      MaxMessages = 1000;
      m_view = view;

      m_previous_destination = MessageLogAppender.Destination;

      m_view.Disposed += OnViewDisposed;

      m_entries = new BindingList<LogEntry>();
      m_view.DataSource = m_entries;
    }

    public void Load()
    {
      AppManager.Notificator.LogUpdated += this.HandleLogUpdated;

      foreach (var e in AppManager.Notificator.GetLastLoggedEvents())
        this.Append(e.Source, e.Info);
    }

    void HandleLogUpdated(object sender, InfoSourceEventArgs e)
    {
      this.Append(e.Source, e.Info);
    }

    void OnViewDisposed(object sender, EventArgs e)
    {
      AppManager.Notificator.LogUpdated -= this.HandleLogUpdated;
    }

    private HashSet<Info> m_pending_infos = new HashSet<Info>();

    public int MaxMessages { get; set; }

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
                          EventDate = DateTime.Now,
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
  }
}