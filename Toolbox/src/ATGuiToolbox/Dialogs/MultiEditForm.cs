using System;
using System.Collections.Generic;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid.Rows;
using AT.Toolbox.Base;

namespace AT.Toolbox.Dialogs
{

  public partial class MultiEditForm : LocalizableForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MultiEditForm).Name);

    protected bool m_loading = true;
    protected List<string> m_edited = new List<string>();
    protected GridView m_view = null;
    protected List<string> m_except_fields = new List<string>();

    public GridControl Grid { get; set; }
    public List<string> ExceptFields { get { return m_except_fields; } }


    public MultiEditForm()
    {
      InitializeComponent();
    }

    private void HandleShown(object sender, EventArgs e)
    {
      if (null == Grid)
        throw new ApplicationException("Grid not set");

      m_view = Grid.FocusedView as GridView;

      if (null == m_view)
        throw new ApplicationException("No focused View");

      try
      {
        foreach (GridColumn col in m_view.Columns)
        {
          if (col.ReadOnly)
            continue;

          if (m_except_fields.Contains(col.FieldName))
            continue;

          EditorRow r = new EditorRow(col.FieldName);
          r.Properties.Caption = col.Caption;
          r.Properties.FieldName = col.FieldName;

          if (null != col.ColumnEdit)
            r.Properties.RowEdit = col.ColumnEdit;


          m_vgrid.Rows.Add(r);
        }

      }
      catch (Exception ex)
      {
        Log.Error("HandleShown(): exception", ex);
      }

      m_vgrid.AddNewRecord();
      m_edited.Clear();
      m_loading = false;
    }

    private void HandleApply(object sender, EventArgs e)
    {
      if (null == m_view)
        throw new ApplicationException("No focused View");

      foreach (EditorRow r in m_vgrid.Rows)
      {
        if (!m_edited.Contains(r.Properties.FieldName))
          continue;

        if (null == r.Properties.Value)
          continue;

        foreach (int i in m_view.GetSelectedRows())
        {
          if (i < 0)
            continue;

          m_view.SetRowCellValue(i, r.Properties.FieldName, r.Properties.Value);
        }

        r.Properties.Caption = r.Properties.Caption.Replace("*", "");
      }

      m_edited.Clear();
    }

    private void HandleEditChanged(object sender, DevExpress.XtraVerticalGrid.Events.RowChangedEventArgs e)
    {
      if (m_loading)
        return;

      EditorRow r = e.Row as EditorRow;

      if (null == r)
        return;

      if (m_edited.Contains(r.Properties.FieldName))
        return;

      m_edited.Add(r.Properties.FieldName);
      r.Properties.Caption += "*";
    }

    private void HandleCancelEdit(object sender, EventArgs e)
    {
      EditorRow r = m_vgrid.FocusedRow as EditorRow;

      if (r == null) 
        return;

      if (!m_edited.Contains(r.Properties.FieldName))
        return;

      r.Properties.Value = null;
      r.Properties.Caption = r.Properties.Caption.Replace("*", "");
      m_edited.Remove(r.Properties.FieldName);
    }

    private void HandleOK(object sender, EventArgs e)
    {
      HandleApply(sender, e);
      Close();
    }

    private void HandleCancel(object sender, EventArgs e)
    {
      Close();
    }
  }
}
