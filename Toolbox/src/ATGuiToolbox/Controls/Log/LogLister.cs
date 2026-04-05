using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using AT.Toolbox.Base;
using DevExpress.Data.Filtering;
using AT.Toolbox;
using AT.Toolbox.Properties;
using AT.Toolbox.Dialogs;
using log4net.Appender;

namespace AT.Toolbox.Controls
{
  public partial class LogLister : LocalizableUserControl, ILogListerView
  {
    private LogListerPresenter m_presenter;

    public LogLister()
    {
      InitializeComponent();

      m_presenter = new LogListerPresenter(this);  
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      colEventDate.Caption = Resources.COL_EVENT_TIME;
      colSource.Caption = Resources.COL_SOURCE;
      colMessage.Caption = Resources.COL_MESSAGE;
      colTag.Caption = Resources.COL_DETAILS;
      colLevel.Caption = Resources.LEVEL;

      m_label_level.Caption = Resources.LEVEL;
      m_cmd_clear.Caption = Resources.CLEAR;
      m_cmd_end.Caption = Resources.LIST_TO_END;
      m_cmd_file.Caption = Resources.LOG_SHOW_FILE_TITLE;

      var labels = new List<KeyValuePair<InfoLevel, string>>();

      foreach (InfoLevel value in Enum.GetValues(typeof(InfoLevel)))
      {
        labels.Add(new KeyValuePair<InfoLevel, string>(value, value.GetLabel()));
      }
      m_level_edit.Properties.DataSource = labels;
      m_level_flt.DataSource = labels;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      m_level_bar.EditValue = AT.Toolbox.Controls.LogListerSettings.Preferences.DefaultLevel;
      m_presenter.Load();
    }

    private void tag_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      LogEntry entry = m_grid_view.GetRow(m_grid_view.FocusedRowHandle) as LogEntry;

      if (entry == null)
        return;

      if (entry.InnerMessages.Length > 0)
      {
        InfoListForm frm = new InfoListForm();
        frm.Accept(entry.CreateBuffer());
        frm.ShowDialog(this);
      }
      else
      {
        var tag = entry.Tag;

        if (tag == null || string.IsNullOrEmpty(tag.ToString()))
          return;

        MemoEditForm mf = new MemoEditForm();
        mf.Editable = false;
        mf.Text = entry.Level.GetLabel();
        mf.Lines = Regex.Replace(tag.ToString(), @"(?>\n\r|\r\n?)", "\n", RegexOptions.Compiled | RegexOptions.Singleline).Split('\n');
        mf.Icon = entry.GetLargeIcon();
        mf.ShowDialog(this);
      }

      if (m_entries != null)
        m_entries.ResetBindings();
      //m_entries.Entries.ResetBindings(); // Иначе падает DevExpress
    }

    private BindingList<LogEntry> m_entries;

    private void m_grid_view_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
    {
      LogEntry entry = m_grid_view.GetRow(e.RowHandle) as LogEntry;

      if (entry == null || e.RowHandle == m_grid_view.FocusedRowHandle)
        return;

      e.Appearance.ForeColor = entry.GetColor();
    }

    private void m_level_edit_EditValueChanged(object sender, EventArgs e)
    {
//      m_log.Treshold = (InfoLevel)m_level_bar.EditValue;
      this.UpdateFilter();
    }

    private void UpdateFilter()
    {
      var level = (InfoLevel)m_level_bar.EditValue;
      if (level > InfoLevel.Debug)
      {
        m_grid_view.ActiveFilterCriteria = new BinaryOperator("Level", level, BinaryOperatorType.GreaterOrEqual);
      }
      else
      {
        m_grid_view.ActiveFilterCriteria = null;
      }

      if (AT.Toolbox.Controls.LogListerSettings.Preferences.DefaultLevel != level)
        AT.Toolbox.Controls.LogListerSettings.Preferences.DefaultLevel = level;
    }

    private void m_cmd_clear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (m_entries != null)
        m_entries.Clear();
    }

    private void m_cmd_end_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (m_entries != null)
        m_grid_view.FocusedRowHandle = m_entries.Count - 1;
    }

    private void m_cmd_file_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      try
      {
        var repository = log4net.LogManager.GetRepository();
        var appender = repository.GetAppenders().FirstOrDefault(a => a.Name == "MainAppender") as RollingFileAppender;
        if (appender != null)
        {
          var dir = Path.GetDirectoryName(appender.File);
          Process.Start(dir);
        }
      }
      catch
      {}
    }

    public BindingList<LogEntry> DataSource
    {
      get { return m_entries; }
      set
      {
        m_entries = value;
        m_grid.DataSource = m_entries;
      }
    }

    public InfoLevel Treshold
    {
      get
      {
        if (m_level_bar.EditValue is InfoLevel)
          return (InfoLevel)m_level_bar.EditValue;

        return InfoLevel.Debug;
      }
    }
  }

//  interface ILogStorage
//  {
//    void Add(LogEntry entry);
//    LogEntry[] GetEntries();
//    event EventHandler<EventArgs<LogEntry>> LogUpdated;
//  }
//
//  class LogStorage : ILogStorage
//  {
//    private Queue<LogEntry> m_entries = new Queue<LogEntry>();
//    private int m_max_entries = 100;
//
//    public void Add(LogEntry entry)
//    {
//      lock (m_entries)
//      {
//        while (m_entries.Count >= m_max_entries)
//          m_entries.Dequeue();
//
//        m_entries.Enqueue(entry);
//      }
//
//      if (LogUpdated != null)
//        LogUpdated(this, new EventArgs<LogEntry>(entry));
//    }
//
//    public LogEntry[] GetEntries()
//    {
//      lock (m_entries)
//      {
//        return m_entries.ToArray();
//      }
//    }
//
//    public event EventHandler<EventArgs<LogEntry>> LogUpdated;
//  }

//  class EventArgs<T> : EventArgs
//  {
//    public EventArgs(T data)
//    {
//      Data = data;
//    }
//
//    public T Data { get; private set; }
//  }
}