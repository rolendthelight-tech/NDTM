using System;
using System.ComponentModel;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using JetBrains.Annotations;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Properties;

namespace Toolbox.GUI.DX.Extensions
{
  /// <summary>
  /// Расширение для множественного редактирования записей в гриде через MultiEditForm
  /// </summary>
  [DefaultEvent("ExceptFieldsFilling")]
  public class GridControlMultiEditFeature : GridControlFeature
  {
    #region Fields

    ContextMenuStrip m_context_menu;
	  readonly IContainer components;
    ToolStripMenuItem m_cmd_multi_edit;
    GridControl m_grid;

    #endregion

    #region Constructors

    public GridControlMultiEditFeature()
      : base()
    {
      components = new Container();
    }

    public GridControlMultiEditFeature(IContainer container)
      : base(container)
    {
      components = container;
    }

    #endregion

    #region Properties

    public string[] ExceptFields { get; set; }

    #endregion

    #region Methods

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_cmd_multi_edit.Text = Resources.MultiEdit;
      base.PerformLocalization(sender, e);
    }

    public override void ApplyFeature([NotNull] GridControl grid)
    {
	    if (grid == null) throw new ArgumentNullException("grid");

			this.m_grid = grid;
      if (grid.ContextMenuStrip == null)
      {
        grid.ContextMenuStrip = new ContextMenuStrip(this.components);
      }
      m_context_menu = grid.ContextMenuStrip;
      m_cmd_multi_edit = new ToolStripMenuItem();

      this.m_context_menu.SuspendLayout();
      this.m_context_menu.Items.AddRange(new ToolStripItem[] {
            this.m_cmd_multi_edit});
      this.m_context_menu.Name = "m_context_menu";
      this.m_context_menu.Size = new System.Drawing.Size(126, 26);
      this.m_context_menu.Opening += new CancelEventHandler(this.m_context_menu_Opening);

      this.m_cmd_multi_edit.Name = "m_cmd_multi_edit";
      this.m_cmd_multi_edit.Size = new System.Drawing.Size(125, 22);
      this.m_cmd_multi_edit.Text = "MultiEdit";
      this.m_cmd_multi_edit.Click += m_cmd_multi_edit_Click;

      this.m_context_menu.ResumeLayout(false);
    }

    void m_cmd_multi_edit_Click(object sender, EventArgs e)
    {
	    using (MultiEditForm frm = new MultiEditForm())
	    {
		    if (this.ExceptFields != null)
		    {
			    frm.ExceptFields.AddRange(this.ExceptFields);
		    }

		    frm.Grid = this.m_grid;
		    frm.ShowDialog(this.m_grid);
	    }

	    GridView view = this.m_grid.DefaultView as GridView;
      if (view != null)
      {
        int old_row_handle = view.FocusedRowHandle;
        foreach (int i in view.GetSelectedRows())
        {
          view.FocusedRowHandle = i;
        }
        view.FocusedRowHandle = -1;
        view.FocusedRowHandle = old_row_handle;
      }
    }

    private void m_context_menu_Opening([NotNull] object sender, [NotNull] CancelEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			if (this.m_grid == null || this.m_grid.FocusedView != this.m_grid.DefaultView)
      {
        e.Cancel = true;
      }
    }

	  #endregion
  }
}
