using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using AT.Toolbox;
using AT.Toolbox.Controls;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Controls
{
  public class LogEntrySet : Component, IListSource
  {
    private readonly BindingList<LogEntry> m_entries = new BindingList<LogEntry>();
    private InfoLevel m_level = InfoLevel.Debug;

    public BindingList<LogEntry> Entries
    {
      get { return m_entries; }
    }

    //public void Accept(LogEventArgs args)
    //{
    //  LogEntry entry = new LogEntry();

    //  entry.Level = args.Info.Level;
    //  entry.Source = args.Source;
    //  entry.Message = args.Info.Message;
    //  entry.Tag = args.Info.Details;
    //  entry.EventDate = DateTime.Now;
    //  entry.InnerMessages = args.Info.InnerMessages.ToArray();

    //  if (m_entries.Count > 0
    //    && m_entries.Count == LogLister.Preferences.RecentLogCount)
    //    m_entries.RemoveAt(0);

    //  m_entries.Add(entry);
    //}

    public InfoLevel Accept(InfoBuffer buffer)
    {
      m_level = InfoLevel.Debug;

      int infoCounter = 0;
      try
      {
        foreach (var info in buffer)
        {
          this.CreateEntry(info, null, ref infoCounter);
        }
      }
      catch { }

      return m_level;
    }

    private void CreateEntry(Info info, int? parentId, ref int infoCounter)
    {
      infoCounter++;

      if (infoCounter >= AT.Toolbox.Controls.LogListerSettings.Preferences.RecentLogCount)
      {
        var exc = new OverflowException(Resources.BUFFER_OVER);
        LogEntry ent = new LogEntry();
        ent.Level = m_level = InfoLevel.Error;
        ent.Message = exc.Message;
        ent.ID = infoCounter;
        ent.Tag = exc;
        m_entries.Add(ent);
        throw exc;
      }

      LogEntry entry = new LogEntry();
      entry.Level = info.Level;
      entry.Message = info.Message;
      entry.Tag = info.Details;
      entry.ID = infoCounter;
      entry.ParentID = parentId;
      m_entries.Add(entry);

      if (m_level < info.Level)
        m_level = info.Level;

      foreach (var innerItem in info.InnerMessages)
      {
        this.CreateEntry(innerItem, entry.ID, ref infoCounter);
      }
    }

    #region IListSource Members

    public bool ContainsListCollection
    {
      get { return false; }
    }

    public System.Collections.IList GetList()
    {
      return m_entries;
    }

    #endregion
  }

  /// <summary>
  /// Класс для отображения записи протокола
  /// </summary>
  public class LogEntry
  {
    public DateTime EventDate { get; set; }
    
    public string Source { get; set; }

    public Image Icon
    {
      get
      {
        switch (this.Level)
        {
          case InfoLevel.Debug:
            return Resources.p_16_debug;

          case InfoLevel.Info:
            return Resources.p_16_info;

          case InfoLevel.Warning:
            return Resources.p_16_warning;

          case InfoLevel.Error:
            return Resources.p_16_error;
        }
        return null;

      }
    }

    public Image GetLargeIcon()
    {
      switch (this.Level)
      {
        case InfoLevel.Debug:
          return Resources.p_48_debug;

        case InfoLevel.Info:
          return Resources.p_48_info;

        case InfoLevel.Warning:
          return Resources.p_48_warning;

        case InfoLevel.Error:
          return Resources.p_48_error;

        default:
          return null;
      }
    }

    public Color GetColor()
    {
      switch (this.Level)
      {
        case InfoLevel.Debug:
          return AT.Toolbox.Controls.LogListerSettings.Preferences.DebugColor;

        case InfoLevel.Info:
          return AT.Toolbox.Controls.LogListerSettings.Preferences.InfoColor;

        case InfoLevel.Warning:
          return AT.Toolbox.Controls.LogListerSettings.Preferences.WarningColor;

        case InfoLevel.Error:
          return AT.Toolbox.Controls.LogListerSettings.Preferences.ErrorColor;
      }
      return Color.White;
    }

    public InfoBuffer CreateBuffer()
    {
      InfoBuffer buf = new InfoBuffer();
      Info root = buf.Add(this.Message, this.Level);
      root.Details = this.Tag;

      foreach (var info in this.InnerMessages)
        root.InnerMessages.Add(info);

      return buf;
    }

    public InfoLevel Level { get; set; }

    public string Message { get; set; }

    public object Tag { get; set; }

    public Info[] InnerMessages { get; set; }

    public string Link { get; set; }

    public int ID { get; set; }

    public int? ParentID { get; set; }
  }
}
