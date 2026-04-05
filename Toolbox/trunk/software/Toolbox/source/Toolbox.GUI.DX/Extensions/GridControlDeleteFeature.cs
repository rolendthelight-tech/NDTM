using System;
using System.Linq;
using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;
using Toolbox.GUI.DX.Properties;

namespace Toolbox.GUI.DX.Extensions
{
  /// <summary>
  /// Расширение для добавления возможности удаления записи
  /// </summary>
  public class GridControlDeleteFeature : GridControlFeature
  {
    #region Fields

    GridControl m_grid;

    #endregion

    #region Constructors

    public GridControlDeleteFeature() : base() { }

    public GridControlDeleteFeature(IContainer container) : base(container) { }

    #endregion

    #region Events

		public class DeletePerformedEventArgs : EventArgs
		{ }

		public delegate void DeletePerformedEventHandler(object sender, DeletePerformedEventArgs e);

		/// <summary>
		/// Возникает после удаления строки
		/// </summary>
		public event DeletePerformedEventHandler DeletePerformed;

		/// <summary>
		/// Возникает для строки, непосредственно над которой была удалена строка
		/// </summary>
		public event RowObjectEventHandler RowAboveDeleted;

		/// <summary>
		/// Возникает для строки, непосредственно под которой была удалена строка
		/// </summary>
		public event RowObjectEventHandler RowBelowDeleted;

    #endregion

    #region Properties

    // public GridControlDeleteTool DeleteTool { get; set; }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool CheckControl { get; set; }

    #endregion

    #region Methods

    public override void ApplyFeature(GridControl grid)
    {
      m_grid = grid;
      foreach (BaseView view in grid.ViewCollection)
      {
        GridView grid_view = view as GridView;
        if (grid_view != null)
        {
          grid_view.KeyUp += new KeyEventHandler(m_grid_vew_KeyUp);
        }
      }
    }

    private void m_grid_vew_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete && (e.Control || !this.CheckControl))
      {
        GridView view = sender as GridView;
        if (view != null)
        {
          if (MessageBox.Show(Resources.DELETE_CONFIRM
              , Resources.EXCEPTION_MESSAGE
              , MessageBoxButtons.OKCancel
              , MessageBoxIcon.Question
              , MessageBoxDefaultButton.Button2) != DialogResult.OK)
          {
            return;
          }
					var row_handleds_deleted = view.GetSelectedRows();
					var rows_prev_event_args = row_handleds_deleted.Where(h => h > 0                 && !row_handleds_deleted.Any(h2 => h2 == h - 1)).Select(h => new RowObjectEventArgs(h - 1 - row_handleds_deleted.Count(h2 => h2 < h - 1), view.GetRow(h - 1))).ToArray(); // TODO: Проверить реализацию
					var rows_next_event_args = row_handleds_deleted.Where(h => h < view.RowCount - 1 && !row_handleds_deleted.Any(h2 => h2 == h + 1)).Select(h => new RowObjectEventArgs(h + 1 - row_handleds_deleted.Count(h2 => h2 < h + 1), view.GetRow(h + 1))).ToArray(); // TODO: Проверить реализацию
					view.DeleteSelectedRows();

					if (this.DeletePerformed != null)
						this.DeletePerformed(this, new DeletePerformedEventArgs());

					if (this.RowBelowDeleted != null)
						foreach (var re in rows_prev_event_args)
						{
							this.RowBelowDeleted(this, re);
						}

        	if (this.RowAboveDeleted != null)
						foreach (var re in rows_next_event_args)
						{
							this.RowAboveDeleted(this, re);
						}
        }
      }
    }

    #endregion
  }

  /*
  [Flags]
  public enum GridControlDeleteTool
  {
    None = 0,
    KeyBoard = 1,
    ContextMenu = 2
  }
  */
}
