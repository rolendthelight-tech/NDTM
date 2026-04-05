namespace TestInfoSchema
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_details_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.m_grid_control = new DevExpress.XtraGrid.GridControl();
      this.m_server_entry_set = new AT.Toolbox.MSSQL.ServerEntrySet();
      this.m_grid_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colConnectionString = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_set2 = new AT.Toolbox.MSSQL.ServerEntrySet();
      this.gridControl1 = new DevExpress.XtraGrid.GridControl();
      this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colConnectionString1 = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colName1 = new DevExpress.XtraGrid.Columns.GridColumn();
      ((System.ComponentModel.ISupportInitialize)(this.m_details_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_control)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_entry_set)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_set2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_details_view
      // 
      this.m_details_view.GridControl = this.m_grid_control;
      this.m_details_view.Name = "m_details_view";
      this.m_details_view.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
      // 
      // m_grid_control
      // 
      this.m_grid_control.DataSource = this.m_server_entry_set;
      this.m_grid_control.EmbeddedNavigator.Name = "";
      this.m_grid_control.Location = new System.Drawing.Point(12, 12);
      this.m_grid_control.MainView = this.m_grid_view;
      this.m_grid_control.Name = "m_grid_control";
      this.m_grid_control.Size = new System.Drawing.Size(400, 200);
      this.m_grid_control.TabIndex = 0;
      this.m_grid_control.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_grid_view,
            this.m_details_view});
      // 
      // m_server_entry_set
      // 
      this.m_server_entry_set.ConfigurationKey = "Default";
      // 
      // m_grid_view
      // 
      this.m_grid_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colConnectionString,
            this.colName});
      this.m_grid_view.GridControl = this.m_grid_control;
      this.m_grid_view.Name = "m_grid_view";
      this.m_grid_view.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
      // 
      // colConnectionString
      // 
      this.colConnectionString.Caption = "ConnectionString";
      this.colConnectionString.FieldName = "ConnectionString";
      this.colConnectionString.Name = "colConnectionString";
      this.colConnectionString.Visible = true;
      this.colConnectionString.VisibleIndex = 0;
      // 
      // colName
      // 
      this.colName.Caption = "Name";
      this.colName.FieldName = "Name";
      this.colName.Name = "colName";
      this.colName.Visible = true;
      this.colName.VisibleIndex = 1;
      // 
      // m_set2
      // 
      this.m_set2.ConfigurationKey = "new";
      // 
      // gridControl1
      // 
      this.gridControl1.DataSource = this.m_set2;
      this.gridControl1.EmbeddedNavigator.Name = "";
      this.gridControl1.Location = new System.Drawing.Point(12, 218);
      this.gridControl1.MainView = this.gridView1;
      this.gridControl1.Name = "gridControl1";
      this.gridControl1.Size = new System.Drawing.Size(400, 234);
      this.gridControl1.TabIndex = 1;
      this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
      // 
      // gridView1
      // 
      this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colConnectionString1,
            this.colName1});
      this.gridView1.GridControl = this.gridControl1;
      this.gridView1.Name = "gridView1";
      // 
      // colConnectionString1
      // 
      this.colConnectionString1.Caption = "ConnectionString";
      this.colConnectionString1.FieldName = "ConnectionString";
      this.colConnectionString1.Name = "colConnectionString1";
      this.colConnectionString1.Visible = true;
      this.colConnectionString1.VisibleIndex = 0;
      // 
      // colName1
      // 
      this.colName1.Caption = "Name";
      this.colName1.FieldName = "Name";
      this.colName1.Name = "colName1";
      this.colName1.Visible = true;
      this.colName1.VisibleIndex = 1;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(462, 492);
      this.Controls.Add(this.gridControl1);
      this.Controls.Add(this.m_grid_control);
      this.Name = "MainForm";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.m_details_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_control)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_entry_set)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_set2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.MSSQL.ServerEntrySet m_server_entry_set;
    private DevExpress.XtraGrid.GridControl m_grid_control;
    private DevExpress.XtraGrid.Views.Grid.GridView m_grid_view;
    private DevExpress.XtraGrid.Views.Grid.GridView m_details_view;
    private DevExpress.XtraGrid.Columns.GridColumn colConnectionString;
    private DevExpress.XtraGrid.Columns.GridColumn colName;
    private AT.Toolbox.MSSQL.ServerEntrySet m_set2;
    private DevExpress.XtraGrid.GridControl gridControl1;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    private DevExpress.XtraGrid.Columns.GridColumn colConnectionString1;
    private DevExpress.XtraGrid.Columns.GridColumn colName1;
  }
}

