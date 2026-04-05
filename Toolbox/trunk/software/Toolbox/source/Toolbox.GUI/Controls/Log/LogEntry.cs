using System;
using System.ComponentModel;
using System.Drawing;
using Toolbox.GUI.Properties;
using Toolbox.Application.Services;

namespace Toolbox.GUI.Controls.Log
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

      if (infoCounter >= LogListerSettings.Preferences.RecentLogCount)
      {
        var exc = new OverflowException(Resources.BUFFER_OVER);
        LogEntry ent = new LogEntry
	        {
		        Level = m_level = InfoLevel.Error,
		        Message = exc.Message,
		        ID = infoCounter,
		        Tag = exc
	        };
	      m_entries.Add(ent);
        throw exc;
      }

      LogEntry entry = new LogEntry
	      {
		      Level = info.Level,
		      Message = info.Message,
		      Tag = info.Details,
		      ID = infoCounter,
		      ParentID = parentId
	      };
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

		public global::System.Collections.IList GetList()
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
					case InfoLevel.FatalError:
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
          return Toolbox_GUI_Resources.p_48_debug;

        case InfoLevel.Info:
					return Toolbox_GUI_Resources.p_48_info;

        case InfoLevel.Warning:
					return Toolbox_GUI_Resources.p_48_warning;

        case InfoLevel.Error:
				case InfoLevel.FatalError:
					return Toolbox_GUI_Resources.p_48_error;

        default:
          return null;
      }
    }

    public Color GetColor()
    {
      switch (this.Level)
      {
        case InfoLevel.Debug:
          return LogListerSettings.Preferences.DebugColor;

        case InfoLevel.Info:
          return LogListerSettings.Preferences.InfoColor;

        case InfoLevel.Warning:
          return LogListerSettings.Preferences.WarningColor;

        case InfoLevel.Error:
				case InfoLevel.FatalError:
					return LogListerSettings.Preferences.ErrorColor;
      }
      return Color.White;
    }

    public InfoBuffer CreateBuffer()
    {
      InfoBuffer buf = new InfoBuffer();
      Info root = buf.Add(message: this.Message, state: this.Level, details: this.Tag);

      foreach (var info in this.InnerMessages)
        root.InnerMessages.Add(info);

      return buf;
    }

		[Obsolete("Это поле существует для отображения в таблицах DevExpress.", true)]
  	public string DetailedMessage
  	{
  		get
  		{
  			return
  				(this.Message == null)
  					? (this.Tag == null)
  					  	? null
  					  	: this.Tag.ToString()
  					: (this.Tag == null)
  					  	? this.Message
  					  	: string.Format("{0} {1}", this.Message, this.Tag);
  		}
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
