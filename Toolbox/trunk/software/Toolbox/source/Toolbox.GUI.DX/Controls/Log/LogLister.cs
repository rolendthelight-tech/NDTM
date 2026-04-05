using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using Toolbox.Extensions;
using Toolbox.GUI.Controls.Log;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Dialogs.EditForms;
using Toolbox.GUI.DX.Properties;
using Toolbox.Application.Services;
using log4net.Appender;

namespace Toolbox.GUI.DX.Controls.Log
{
  public partial class LogLister : LocalizableUserControl, ILogListerView
  {
    private readonly LogListerPresenter m_presenter;

    public LogLister()
    {
      InitializeComponent();

      m_presenter = new LogListerPresenter(this);  
    }

    public event EventHandler<InfoEventArgs> RowDoubleClick;

    public override void PerformLocalization(object sender, EventArgs e)
    {
      colEventDate.Caption = Resources.COL_EVENT_TIME;
      colSource.Caption = Resources.COL_SOURCE;
      colMessage.Caption = Resources.COL_MESSAGE;

      m_label_level.Caption = Resources.LEVEL;
      m_cmd_clear.Caption = Resources.CLEAR;
      m_cmd_end.Caption = Resources.LIST_TO_END;
      m_cmd_file.Caption = Resources.LOG_SHOW_FILE_TITLE;

    	var labels = (from InfoLevel value in Enum.GetValues(typeof (InfoLevel))
    	              select new KeyValuePair<InfoLevel, string>(value, value.GetLabel())).ToList();

    	m_level_edit.DataSource = labels;
      m_level_flt.DataSource = labels;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      m_level_bar.EditValue = LogListerSettings.Preferences.DefaultLevel;
      m_presenter.Load();
    }

    private void m_message_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      LogEntry entry = m_grid_view.GetRow(m_grid_view.FocusedRowHandle) as LogEntry;

      if (entry == null)
        return;

      if (entry.InnerMessages.Length > 0)
      {
				using (var frm = new InfoListForm())
				{
					frm.Accept(entry.CreateBuffer());
					frm.ShowDialog(this);
				}
      }
      else
      {
      	var message =
      		(entry.Message == null)
      			? (entry.Tag == null)
      			  	? null
      			  	: entry.Tag.ToString()
      			: (entry.Tag == null)
      			  	? entry.Message
      			  	: string.Format("{0}{2}{1}", entry.Message, entry.Tag, Environment.NewLine);

				if (message == null)
        	return;

	      using (var mf = new MemoEditForm())
	      {
		      mf.Editable = false;
		      mf.Text = entry.Level.GetLabel();
		      mf.Value = message;
		      mf.Icon = entry.GetLargeIcon();

		      mf.ShowDialog(this);
	      }
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

      if (LogListerSettings.Preferences.DefaultLevel != level)
        LogListerSettings.Preferences.DefaultLevel = level;
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
          using(Process.Start(dir)){}
        }
      }
      catch
      {}
    }

    private void m_grid_view_DoubleClick(object sender, EventArgs e)
    {
      var info = m_grid_view.CalcHitInfo(m_grid.PointToClient(MousePosition));

      if (!info.InRow || info.InFilterPanel)
        return;

      var entry = m_grid_view.GetFocusedRow() as LogEntry;

      if (entry == null)
        return;

      if (this.RowDoubleClick != null)
        this.RowDoubleClick(this, new InfoEventArgs(new Info(link: entry.Link, message: entry.Message, state: entry.Level, details: entry.Tag)));
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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