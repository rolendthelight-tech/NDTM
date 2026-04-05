using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Extensions
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

    public event EventHandler DeletePerformed;

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
          view.DeleteSelectedRows();

          if (this.DeletePerformed != null)
            this.DeletePerformed(this, EventArgs.Empty);
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
