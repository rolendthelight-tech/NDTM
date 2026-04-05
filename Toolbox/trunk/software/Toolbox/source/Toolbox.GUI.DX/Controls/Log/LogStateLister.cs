using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using JetBrains.Annotations;
using log4net;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using Toolbox.GUI.Controls.Log;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Dialogs.EditForms;
using Toolbox.GUI.DX.Properties;

namespace Toolbox.GUI.DX.Controls.Log
{
  /// <summary>
  /// Элемент управления для отображения состояния активной дочерней формы
  /// </summary>
  public partial class LogStateLister : LocalizableUserControl
  {
    private readonly ILog _log = LogManager.GetLogger(typeof(LogStateLister));

    private BindingList<LogEntry> m_entries;
    private IInfoStateSource m_current_source;

    public LogStateLister()
    {
      InitializeComponent();
    }

    public event EventHandler<InfoEventArgs> RowDoubleClick;

    BindingList<LogEntry> DataSource
    {
      get { return m_entries; }
      set
      {
        m_entries = value;
        m_grid.DataSource = m_entries;
        UpdateLabels();
      }
    }

    [DefaultValue(true)]
    public bool ShowButtons
    {
      get { return m_menu.Visible; }
      set { m_menu.Visible = value; }
    }

    [DefaultValue(true)]
    public bool ShowErrors
    {
      get { return m_check_errors.Checked; }
      set { m_check_errors.Checked = value; }
    }

    [DefaultValue(true)]
    public bool ShowWarnings
    {
      get { return m_check_warnings.Checked; }
      set { m_check_warnings.Checked = value; }
    }

    [DefaultValue(false)]
    public bool ShowMessages
    {
      get { return m_check_messages.Checked; }
      set { m_check_messages.Checked = value; }
    }

    [DefaultValue(false)]
    public bool ShowDebug
    {
      get { return m_check_debug.Checked; }
      set { m_check_debug.Checked = value; }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      var frm = this.FindForm();

      if (frm.IsMdiContainer)
      {
        frm.MdiChildActivate += new EventHandler(HandleMdiChildActivate);
      }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      colEventDate.Caption = Resources.COL_EVENT_TIME;
      colMessage.Caption = Resources.COL_MESSAGE;

      var labels = (from InfoLevel value in Enum.GetValues(typeof(InfoLevel))
                    select new KeyValuePair<InfoLevel, string>(value, value.GetLabel())).ToList();

      m_level_flt.DataSource = labels;

      UpdateLabels();

      var etalon = new LogEntry();

      etalon.Level = InfoLevel.Debug;
      m_check_debug.Glyph = etalon.Icon;

      etalon.Level = InfoLevel.Info;
      m_check_messages.Glyph = etalon.Icon;

      etalon.Level = InfoLevel.Warning;
      m_check_warnings.Glyph = etalon.Icon;

      etalon.Level = InfoLevel.Error;
      m_check_errors.Glyph = etalon.Icon;
    }

    private void UpdateLabels()
    {
      var list = m_entries ?? new BindingList<LogEntry>();

      m_check_debug.Caption = string.Format("{0} ({1})",Resources.DEBUG_COUNT, list.Count(m => m.Level == InfoLevel.Debug));
      m_check_messages.Caption = string.Format("{0} ({1})", Resources.MESSAGES_COUNT, list.Count(m => m.Level == InfoLevel.Info));
      m_check_warnings.Caption = string.Format("{0} ({1})", Resources.WARNINGS_COUNT, list.Count(m => m.Level == InfoLevel.Warning));
      m_check_errors.Caption = string.Format("{0} ({1})", Resources.ERRORS_COUNT, list.Count(m => m.Level == InfoLevel.Error || m.Level == InfoLevel.FatalError));
    }

    private void HandleMdiChildActivate(object sender, EventArgs e)
    {
      if (m_current_source != null)
        m_current_source.StateChanged -= this.HandleStateChanged;

      var findForm = this.FindForm();
      if (findForm != null)
        m_current_source = findForm.ActiveMdiChild as IInfoStateSource;

      if (m_current_source != null)
      {
        ((Action)this.UpdateDataSource).BeginInvoke(null, null);
        m_current_source.StateChanged += this.HandleStateChanged;
      }
      else
        this.DataSource = null;
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
      ((Action)this.UpdateDataSource).BeginInvoke(null, null);
    }

    private void UpdateDataSource()
    {
      try
      {
	      var ds = new BindingList<LogEntry>(
		      m_current_source
			      .GetState()
			      .Select(
				      info => new LogEntry
					      {
						      EventDate = DateTime.Now,
						      InnerMessages = info.InnerMessages.ToArray(),
						      Level = info.Level,
						      Message = info.Message,
						      Tag = info.Details,
						      Link = info.Link
					      })
			      .ToList());

        this.UIThread(() => this.DataSource = ds);
      }
      catch(Exception ex)
      {
        _log.Error("UpdateDataSource(): exception", ex);
      }
    }

    private void m_grid_view_CustomDrawCell([NotNull] object sender, [NotNull] DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			LogEntry entry = m_grid_view.GetRow(e.RowHandle) as LogEntry;

      if (entry == null || e.RowHandle == m_grid_view.FocusedRowHandle)
        return;

      e.Appearance.ForeColor = entry.GetColor();
    }

    private void m_message_edit_ButtonClick([NotNull] object sender, [NotNull] DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

    private void HandleFilterChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      var set = new HashSet<InfoLevel>();

      if (m_check_errors.Checked)
      {
        set.Add(InfoLevel.Error);
        set.Add(InfoLevel.FatalError);
      }
      if (m_check_warnings.Checked)
        set.Add(InfoLevel.Warning);

      if (m_check_messages.Checked)
        set.Add(InfoLevel.Info);

      if (m_check_debug.Checked)
        set.Add(InfoLevel.Debug);

      m_grid_view.ActiveFilterCriteria = new InOperator("Level", set.ToArray());
    }
  }

  public interface IInfoStateSource
  {
    event EventHandler StateChanged;

    InfoBuffer GetState();
  }
}
