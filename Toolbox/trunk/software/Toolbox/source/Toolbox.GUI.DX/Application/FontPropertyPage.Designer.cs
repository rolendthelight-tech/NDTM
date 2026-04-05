namespace Toolbox.GUI.DX.Application
{
  partial class FontPropertyPage
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support — do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_font_grid = new DevExpress.XtraGrid.GridControl();
      this.m_font_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_font_id_edit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
      this.colFont = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_font_edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
      this.m_binding_source = new System.Windows.Forms.BindingSource();
      this.fontsBindingSource = new System.Windows.Forms.BindingSource();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_id_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.fontsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // m_font_grid
      // 
      this.m_font_grid.DataSource = this.fontsBindingSource;
      this.m_font_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_font_grid.Location = new System.Drawing.Point(0, 0);
      this.m_font_grid.MainView = this.m_font_view;
      this.m_font_grid.Name = "m_font_grid";
      this.m_font_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_font_id_edit,
            this.m_font_edit});
      this.m_font_grid.Size = new System.Drawing.Size(506, 423);
      this.m_font_grid.TabIndex = 3;
      this.m_font_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_font_view});
      // 
      // m_font_view
      // 
      this.m_font_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colFont});
      this.m_font_view.GridControl = this.m_font_grid;
      this.m_font_view.Name = "m_font_view";
      this.m_font_view.OptionsView.ShowGroupPanel = false;
      this.m_font_view.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.m_font_view_CustomDrawCell);
      this.m_font_view.CalcRowHeight +=new DevExpress.XtraGrid.Views.Grid.RowHeightEventHandler(m_font_view_CalcRowHeight);
      // 
      // colID
      // 
      this.colID.Caption = "Компонент";
      this.colID.ColumnEdit = this.m_font_id_edit;
      this.colID.FieldName = "ID";
      this.colID.Name = "colID";
      this.colID.OptionsColumn.AllowEdit = false;
      this.colID.Visible = true;
      this.colID.VisibleIndex = 0;
      // 
      // m_font_id_edit
      // 
      this.m_font_id_edit.AutoHeight = false;
      this.m_font_id_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_font_id_edit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name1")});
      this.m_font_id_edit.DisplayMember = "Value";
      this.m_font_id_edit.Name = "m_font_id_edit";
      this.m_font_id_edit.ShowHeader = false;
      this.m_font_id_edit.ValueMember = "Key";
      // 
      // colFont
      // 
      this.colFont.Caption = "Шрифт";
      this.colFont.ColumnEdit = this.m_font_edit;
      this.colFont.FieldName = "Font";
      this.colFont.Name = "colFont";
      this.colFont.Visible = true;
      this.colFont.VisibleIndex = 1;
      // 
      // m_font_edit
      // 
      this.m_font_edit.AutoHeight = false;
      this.m_font_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_font_edit.Name = "m_font_edit";
      this.m_font_edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_font_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_font_edit_ButtonClick);
      this.m_font_edit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.m_font_edit_CustomDisplayText);
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Toolbox.GUI.Base.FontSettings);
      // 
      // fontsBindingSource
      // 
      this.fontsBindingSource.DataMember = "Fonts";
      this.fontsBindingSource.DataSource = this.m_binding_source;
      // 
      // FontPropertyPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_font_grid);
      this.Name = "FontPropertyPage";
      ((System.ComponentModel.ISupportInitialize)(this.m_font_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_id_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.fontsBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraGrid.GridControl m_font_grid;
    private DevExpress.XtraGrid.Views.Grid.GridView m_font_view;
    private DevExpress.XtraGrid.Columns.GridColumn colID;
    private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit m_font_id_edit;
    private DevExpress.XtraGrid.Columns.GridColumn colFont;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit m_font_edit;
    private System.Windows.Forms.BindingSource fontsBindingSource;
    private System.Windows.Forms.BindingSource m_binding_source;
  }
}
