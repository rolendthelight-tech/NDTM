namespace AT.Toolbox.Controls
{
  partial class TaskLister
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.m_task_grid = new DevExpress.XtraGrid.GridControl();
      this.taskInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_task_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_status_edit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
      this.colKill = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_button_kill = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
      ((System.ComponentModel.ISupportInitialize)(this.m_task_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.taskInfoBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_task_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_status_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_button_kill)).BeginInit();
      this.SuspendLayout();
      // 
      // m_task_grid
      // 
      this.m_task_grid.DataSource = this.taskInfoBindingSource;
      this.m_task_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_task_grid.Location = new System.Drawing.Point(0, 0);
      this.m_task_grid.MainView = this.m_task_view;
      this.m_task_grid.Name = "m_task_grid";
      this.m_task_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_status_edit,
            this.m_button_kill});
      this.m_task_grid.Size = new System.Drawing.Size(517, 300);
      this.m_task_grid.TabIndex = 0;
      this.m_task_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_task_view});
      // 
      // taskInfoBindingSource
      // 
      this.taskInfoBindingSource.DataSource = typeof(AT.Toolbox.TaskInfo);
      // 
      // m_task_view
      // 
      this.m_task_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName,
            this.colStatus,
            this.colKill});
      this.m_task_view.GridControl = this.m_task_grid;
      this.m_task_view.Name = "m_task_view";
      this.m_task_view.OptionsMenu.EnableColumnMenu = false;
      this.m_task_view.OptionsView.ShowGroupPanel = false;
      this.m_task_view.DoubleClick += new System.EventHandler(this.m_task_view_DoubleClick);
      // 
      // colName
      // 
      this.colName.FieldName = "Name";
      this.colName.Name = "colName";
      this.colName.OptionsColumn.AllowEdit = false;
      this.colName.OptionsColumn.ReadOnly = true;
      this.colName.Visible = true;
      this.colName.VisibleIndex = 0;
      // 
      // colStatus
      // 
      this.colStatus.ColumnEdit = this.m_status_edit;
      this.colStatus.FieldName = "Status";
      this.colStatus.Name = "colStatus";
      this.colStatus.OptionsColumn.AllowEdit = false;
      this.colStatus.OptionsColumn.ReadOnly = true;
      this.colStatus.Visible = true;
      this.colStatus.VisibleIndex = 1;
      this.colStatus.Width = 236;
      // 
      // m_status_edit
      // 
      this.m_status_edit.AutoHeight = false;
      this.m_status_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_status_edit.DisplayMember = "Value";
      this.m_status_edit.Name = "m_status_edit";
      this.m_status_edit.ValueMember = "Key";
      // 
      // colKill
      // 
      this.colKill.ColumnEdit = this.m_button_kill;
      this.colKill.FieldName = "colKill";
      this.colKill.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
      this.colKill.Name = "colKill";
      this.colKill.UnboundType = DevExpress.Data.UnboundColumnType.String;
      this.colKill.Visible = true;
      this.colKill.VisibleIndex = 2;
      this.colKill.Width = 22;
      // 
      // m_button_kill
      // 
      this.m_button_kill.AutoHeight = false;
      this.m_button_kill.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
      this.m_button_kill.Name = "m_button_kill";
      this.m_button_kill.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
      this.m_button_kill.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_button_kill_ButtonClick);
      // 
      // TaskLister
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_task_grid);
      this.Name = "TaskLister";
      this.Size = new System.Drawing.Size(517, 300);
      ((System.ComponentModel.ISupportInitialize)(this.m_task_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.taskInfoBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_task_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_status_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_button_kill)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraGrid.GridControl m_task_grid;
    private DevExpress.XtraGrid.Views.Grid.GridView m_task_view;
    private System.Windows.Forms.BindingSource taskInfoBindingSource;
    private DevExpress.XtraGrid.Columns.GridColumn colName;
    private DevExpress.XtraGrid.Columns.GridColumn colStatus;
    private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit m_status_edit;
    private DevExpress.XtraGrid.Columns.GridColumn colKill;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit m_button_kill;
  }
}
