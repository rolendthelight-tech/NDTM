using DevExpress.Utils;

namespace AT.Toolbox.MSSQL
{
  partial class DatabaseBrowserControl
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
      DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
      DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip5 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem5 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem5 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip6 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem6 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem6 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip7 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem7 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem7 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip8 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem8 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem8 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip9 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem9 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem9 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip10 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem10 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem10 = new DevExpress.Utils.ToolTipItem();
      DevExpress.Utils.SuperToolTip superToolTip11 = new DevExpress.Utils.SuperToolTip();
      DevExpress.Utils.ToolTipTitleItem toolTipTitleItem11 = new DevExpress.Utils.ToolTipTitleItem();
      DevExpress.Utils.ToolTipItem toolTipItem11 = new DevExpress.Utils.ToolTipItem();
      this.m_database_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.m_db_name_col = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_db_ok_col = new DevExpress.XtraGrid.Columns.GridColumn();
      this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
      this.m_db_used_col = new DevExpress.XtraGrid.Columns.GridColumn();
      this.UsageEditor = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
      this.m_db_version_column = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_version_edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
      this.m_server_grid = new DevExpress.XtraGrid.GridControl();
      this.serverEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_server_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_bar_manager = new DevExpress.XtraBars.BarManager(this.components);
      this.bar1 = new DevExpress.XtraBars.Bar();
      this.m_add_server_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_remove_server_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_refresh_server_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_refresh_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_add_db_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_remove_db_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_reset_db_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_mould_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_import_data_cmd = new DevExpress.XtraBars.BarSubItem();
      this.m_backup_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_restore_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_script = new DevExpress.XtraBars.BarButtonItem();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
      this.repositoryItemPopupContainerEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit();
      ((System.ComponentModel.ISupportInitialize)(this.m_database_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.UsageEditor)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_version_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.serverEntryBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_database_view
      // 
      this.m_database_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.m_db_name_col,
            this.m_db_ok_col,
            this.m_db_used_col,
            this.m_db_version_column});
      this.m_database_view.GridControl = this.m_server_grid;
      this.m_database_view.Name = "m_database_view";
      this.m_database_view.OptionsBehavior.AutoExpandAllGroups = true;
      this.m_database_view.OptionsBehavior.CacheValuesOnRowUpdating = DevExpress.Data.CacheRowValuesMode.Disabled;
      this.m_database_view.OptionsCustomization.AllowGroup = false;
      this.m_database_view.OptionsFilter.UseNewCustomFilterDialog = true;
      this.m_database_view.OptionsView.ShowGroupPanel = false;
      this.m_database_view.OptionsView.ShowIndicator = false;
      this.m_database_view.GotFocus += new System.EventHandler(this.HandleTableListLostFocus);
      // 
      // m_db_name_col
      // 
      this.m_db_name_col.Caption = "Name";
      this.m_db_name_col.FieldName = "Name";
      this.m_db_name_col.Name = "m_db_name_col";
      this.m_db_name_col.OptionsColumn.AllowEdit = false;
      this.m_db_name_col.OptionsColumn.AllowFocus = false;
      this.m_db_name_col.OptionsColumn.ReadOnly = true;
      this.m_db_name_col.Visible = true;
      this.m_db_name_col.VisibleIndex = 0;
      this.m_db_name_col.Width = 279;
      // 
      // m_db_ok_col
      // 
      this.m_db_ok_col.Caption = "Status";
      this.m_db_ok_col.ColumnEdit = this.repositoryItemPictureEdit1;
      this.m_db_ok_col.FieldName = "SupportedPic";
      this.m_db_ok_col.Name = "m_db_ok_col";
      this.m_db_ok_col.OptionsColumn.AllowEdit = false;
      this.m_db_ok_col.OptionsColumn.AllowFocus = false;
      this.m_db_ok_col.OptionsColumn.AllowMove = false;
      this.m_db_ok_col.OptionsColumn.AllowSize = false;
      this.m_db_ok_col.OptionsColumn.FixedWidth = true;
      this.m_db_ok_col.OptionsColumn.ReadOnly = true;
      this.m_db_ok_col.Visible = true;
      this.m_db_ok_col.VisibleIndex = 2;
      this.m_db_ok_col.Width = 57;
      // 
      // repositoryItemPictureEdit1
      // 
      this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
      // 
      // m_db_used_col
      // 
      this.m_db_used_col.Caption = "Used";
      this.m_db_used_col.ColumnEdit = this.UsageEditor;
      this.m_db_used_col.FieldName = "Used";
      this.m_db_used_col.Name = "m_db_used_col";
      this.m_db_used_col.OptionsColumn.AllowMove = false;
      this.m_db_used_col.OptionsColumn.AllowSize = false;
      this.m_db_used_col.OptionsColumn.FixedWidth = true;
      this.m_db_used_col.Visible = true;
      this.m_db_used_col.VisibleIndex = 1;
      this.m_db_used_col.Width = 101;
      // 
      // UsageEditor
      // 
      this.UsageEditor.AutoHeight = false;
      this.UsageEditor.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
      this.UsageEditor.Name = "UsageEditor";
      this.UsageEditor.CheckedChanged += new System.EventHandler(this.HandleUsageCheckedChanged);
      this.UsageEditor.Validating += new System.ComponentModel.CancelEventHandler(this.HandleUsageValidating);
      // 
      // m_db_version_column
      // 
      this.m_db_version_column.Caption = "Version";
      this.m_db_version_column.ColumnEdit = this.m_version_edit;
      this.m_db_version_column.FieldName = "Version";
      this.m_db_version_column.Name = "m_db_version_column";
      this.m_db_version_column.Visible = true;
      this.m_db_version_column.VisibleIndex = 3;
      this.m_db_version_column.Width = 211;
      // 
      // m_version_edit
      // 
      this.m_version_edit.AutoHeight = false;
      this.m_version_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_version_edit.Name = "m_version_edit";
      this.m_version_edit.ReadOnly = true;
      this.m_version_edit.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.HandleExtendedInfo);
      // 
      // m_server_grid
      // 
      this.m_server_grid.DataSource = this.serverEntryBindingSource;
      this.m_server_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      gridLevelNode1.LevelTemplate = this.m_database_view;
      gridLevelNode1.RelationName = "Bases";
      this.m_server_grid.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
      this.m_server_grid.Location = new System.Drawing.Point(0, 42);
      this.m_server_grid.MainView = this.m_server_view;
      this.m_server_grid.Name = "m_server_grid";
      this.m_server_grid.Padding = new System.Windows.Forms.Padding(3);
      this.m_server_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.UsageEditor,
            this.m_version_edit,
            this.repositoryItemPictureEdit1});
      this.m_server_grid.ShowOnlyPredefinedDetails = true;
      this.m_server_grid.Size = new System.Drawing.Size(652, 396);
      this.m_server_grid.TabIndex = 4;
      this.m_server_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_server_view,
            this.m_database_view});
      this.m_server_grid.Load += new System.EventHandler(this.HandleLoad);
      // 
      // serverEntryBindingSource
      // 
      this.serverEntryBindingSource.DataSource = typeof(AT.Toolbox.MSSQL.ServerEntry);
      // 
      // m_server_view
      // 
      this.m_server_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName});
      this.m_server_view.GridControl = this.m_server_grid;
      this.m_server_view.Name = "m_server_view";
      this.m_server_view.OptionsCustomization.AllowGroup = false;
      this.m_server_view.OptionsDetail.ShowDetailTabs = false;
      this.m_server_view.OptionsDetail.SmartDetailExpandButtonMode = DevExpress.XtraGrid.Views.Grid.DetailExpandButtonMode.CheckAllDetails;
      this.m_server_view.OptionsHint.ShowColumnHeaderHints = false;
      this.m_server_view.OptionsView.ShowColumnHeaders = false;
      this.m_server_view.OptionsView.ShowGroupPanel = false;
      this.m_server_view.OptionsView.ShowHorzLines = false;
      this.m_server_view.OptionsView.ShowIndicator = false;
      this.m_server_view.OptionsView.ShowVertLines = false;
      this.m_server_view.GotFocus += new System.EventHandler(this.HandleServerListGotFocus);
      // 
      // colName
      // 
      this.colName.Caption = "Name";
      this.colName.FieldName = "Name";
      this.colName.Name = "colName";
      this.colName.OptionsColumn.AllowEdit = false;
      this.colName.OptionsColumn.ReadOnly = true;
      this.colName.Visible = true;
      this.colName.VisibleIndex = 0;
      // 
      // m_bar_manager
      // 
      this.m_bar_manager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
      this.m_bar_manager.CloseButtonAffectAllTabs = false;
      this.m_bar_manager.DockControls.Add(this.barDockControlTop);
      this.m_bar_manager.DockControls.Add(this.barDockControlBottom);
      this.m_bar_manager.DockControls.Add(this.barDockControlLeft);
      this.m_bar_manager.DockControls.Add(this.barDockControlRight);
      this.m_bar_manager.Form = this;
      this.m_bar_manager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_add_server_cmd,
            this.m_remove_server_cmd,
            this.m_refresh_server_cmd,
            this.m_refresh_cmd,
            this.m_add_db_cmd,
            this.m_remove_db_cmd,
            this.m_reset_db_cmd,
            this.m_mould_cmd,
            this.m_backup_cmd,
            this.m_restore_cmd,
            this.barButtonItem1,
            this.m_import_data_cmd,
            this.m_cmd_script});
      this.m_bar_manager.MaxItemId = 22;
      this.m_bar_manager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPopupContainerEdit1});
      // 
      // bar1
      // 
      this.bar1.BarName = "Tools";
      this.bar1.DockCol = 0;
      this.bar1.DockRow = 0;
      this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_add_server_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_remove_server_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_refresh_server_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_refresh_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_add_db_cmd, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_remove_db_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_reset_db_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_mould_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_import_data_cmd, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_backup_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_restore_cmd),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_script)});
      this.bar1.OptionsBar.AllowQuickCustomization = false;
      this.bar1.OptionsBar.DisableClose = true;
      this.bar1.OptionsBar.DisableCustomization = true;
      this.bar1.OptionsBar.DrawDragBorder = false;
      this.bar1.OptionsBar.UseWholeRow = true;
      this.bar1.Text = "Tools";
      // 
      // m_add_server_cmd
      // 
      this.m_add_server_cmd.Caption = "Add Server";
      this.m_add_server_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_server_add;
      this.m_add_server_cmd.Id = 0;
      this.m_add_server_cmd.Name = "m_add_server_cmd";
      toolTipTitleItem1.Text = "Add Server";
      toolTipItem1.LeftIndent = 6;
      toolTipItem1.Text = "Adds MS SQL server to the list";
      superToolTip1.Items.Add(toolTipTitleItem1);
      superToolTip1.Items.Add(toolTipItem1);
      this.m_add_server_cmd.SuperTip = superToolTip1;
      this.m_add_server_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleAddServer);
      // 
      // m_remove_server_cmd
      // 
      this.m_remove_server_cmd.Caption = "Remove Server";
      this.m_remove_server_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_server_remove;
      this.m_remove_server_cmd.Id = 1;
      this.m_remove_server_cmd.Name = "m_remove_server_cmd";
      toolTipTitleItem2.Text = "Remove server";
      toolTipItem2.LeftIndent = 6;
      toolTipItem2.Text = "Removes server form the list";
      superToolTip2.Items.Add(toolTipTitleItem2);
      superToolTip2.Items.Add(toolTipItem2);
      this.m_remove_server_cmd.SuperTip = superToolTip2;
      this.m_remove_server_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleRemoveServer);
      // 
      // m_refresh_server_cmd
      // 
      this.m_refresh_server_cmd.Caption = "Refresh Server Info";
      this.m_refresh_server_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_server_refresh;
      this.m_refresh_server_cmd.Id = 2;
      this.m_refresh_server_cmd.Name = "m_refresh_server_cmd";
      toolTipTitleItem3.Text = "Refresh server";
      toolTipItem3.LeftIndent = 6;
      toolTipItem3.Text = "Refresh selected server";
      superToolTip3.Items.Add(toolTipTitleItem3);
      superToolTip3.Items.Add(toolTipItem3);
      this.m_refresh_server_cmd.SuperTip = superToolTip3;
      this.m_refresh_server_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleRefreshServer);
      // 
      // m_refresh_cmd
      // 
      this.m_refresh_cmd.Caption = "Refresh All";
      this.m_refresh_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_refresh;
      this.m_refresh_cmd.Id = 3;
      this.m_refresh_cmd.Name = "m_refresh_cmd";
      toolTipTitleItem4.Text = "Refresh list";
      toolTipItem4.LeftIndent = 6;
      toolTipItem4.Text = "Refreshes database list for all servers";
      superToolTip4.Items.Add(toolTipTitleItem4);
      superToolTip4.Items.Add(toolTipItem4);
      this.m_refresh_cmd.SuperTip = superToolTip4;
      this.m_refresh_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleRefreshAll);
      // 
      // m_add_db_cmd
      // 
      this.m_add_db_cmd.Caption = "Add Database";
      this.m_add_db_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_add;
      this.m_add_db_cmd.Id = 4;
      this.m_add_db_cmd.Name = "m_add_db_cmd";
      toolTipTitleItem5.Text = "Add database";
      toolTipItem5.LeftIndent = 6;
      toolTipItem5.Text = "Add database to the server";
      superToolTip5.Items.Add(toolTipTitleItem5);
      superToolTip5.Items.Add(toolTipItem5);
      this.m_add_db_cmd.SuperTip = superToolTip5;
      this.m_add_db_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleAddDatabase);
      // 
      // m_remove_db_cmd
      // 
      this.m_remove_db_cmd.Caption = "Remove Database";
      this.m_remove_db_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_delete;
      this.m_remove_db_cmd.Id = 5;
      this.m_remove_db_cmd.Name = "m_remove_db_cmd";
      toolTipTitleItem6.Text = "Remove database";
      toolTipItem6.LeftIndent = 6;
      toolTipItem6.Text = "Remove selected database";
      superToolTip6.Items.Add(toolTipTitleItem6);
      superToolTip6.Items.Add(toolTipItem6);
      this.m_remove_db_cmd.SuperTip = superToolTip6;
      this.m_remove_db_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleRemoveDatabase);
      // 
      // m_reset_db_cmd
      // 
      this.m_reset_db_cmd.Caption = "Update structure";
      this.m_reset_db_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_refresh;
      this.m_reset_db_cmd.Id = 6;
      this.m_reset_db_cmd.Name = "m_reset_db_cmd";
      toolTipTitleItem7.Text = "Reset database";
      toolTipItem7.LeftIndent = 6;
      toolTipItem7.Text = "Reset selected database";
      superToolTip7.Items.Add(toolTipTitleItem7);
      superToolTip7.Items.Add(toolTipItem7);
      this.m_reset_db_cmd.SuperTip = superToolTip7;
      this.m_reset_db_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleResetDatabase);
      // 
      // m_mould_cmd
      // 
      this.m_mould_cmd.Caption = "Test";
      this.m_mould_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_script;
      this.m_mould_cmd.Id = 9;
      this.m_mould_cmd.Name = "m_mould_cmd";
      toolTipTitleItem8.Text = "Test";
      toolTipItem8.LeftIndent = 6;
      toolTipItem8.Text = "Test database";
      superToolTip8.Items.Add(toolTipTitleItem8);
      superToolTip8.Items.Add(toolTipItem8);
      this.m_mould_cmd.SuperTip = superToolTip8;
      this.m_mould_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleDataBaseMould);
      // 
      // m_import_data_cmd
      // 
      this.m_import_data_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_into;
      this.m_import_data_cmd.Id = 19;
      this.m_import_data_cmd.Name = "m_import_data_cmd";
      this.m_import_data_cmd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      toolTipTitleItem9.Text = "Import data";
      toolTipItem9.LeftIndent = 6;
      toolTipItem9.Text = "Select method for importing data";
      superToolTip9.Items.Add(toolTipTitleItem9);
      superToolTip9.Items.Add(toolTipItem9);
      this.m_import_data_cmd.SuperTip = superToolTip9;
      // 
      // m_backup_cmd
      // 
      this.m_backup_cmd.Caption = "Backup";
      this.m_backup_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_save;
      this.m_backup_cmd.Id = 10;
      this.m_backup_cmd.Name = "m_backup_cmd";
      toolTipTitleItem10.Text = "Backup";
      toolTipItem10.LeftIndent = 6;
      toolTipItem10.Text = "Backup database";
      superToolTip10.Items.Add(toolTipTitleItem10);
      superToolTip10.Items.Add(toolTipItem10);
      this.m_backup_cmd.SuperTip = superToolTip10;
      this.m_backup_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_backup_cmd_ItemClick);
      // 
      // m_restore_cmd
      // 
      this.m_restore_cmd.Caption = "Restore";
      this.m_restore_cmd.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_restore;
      this.m_restore_cmd.Id = 11;
      this.m_restore_cmd.Name = "m_restore_cmd";
      toolTipTitleItem11.Text = "Restore";
      toolTipItem11.LeftIndent = 6;
      toolTipItem11.Text = "Restore database";
      superToolTip11.Items.Add(toolTipTitleItem11);
      superToolTip11.Items.Add(toolTipItem11);
      this.m_restore_cmd.SuperTip = superToolTip11;
      this.m_restore_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_restore_cmd_ItemClick);
      // 
      // m_cmd_script
      // 
      this.m_cmd_script.Caption = " ";
      this.m_cmd_script.Glyph = global::AT.Toolbox.MSSQL.Properties.Resources.p_32_database_run;
      this.m_cmd_script.Hint = "SQL";
      this.m_cmd_script.Id = 21;
      this.m_cmd_script.Name = "m_cmd_script";
      this.m_cmd_script.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_script_ItemClick);
      // 
      // barButtonItem1
      // 
      this.barButtonItem1.Caption = "barButtonItem1";
      this.barButtonItem1.Id = 17;
      this.barButtonItem1.Name = "barButtonItem1";
      // 
      // repositoryItemPopupContainerEdit1
      // 
      this.repositoryItemPopupContainerEdit1.AutoHeight = false;
      this.repositoryItemPopupContainerEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.repositoryItemPopupContainerEdit1.Name = "repositoryItemPopupContainerEdit1";
      // 
      // DatabaseBrowserControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_server_grid);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Name = "DatabaseBrowserControl";
      this.Size = new System.Drawing.Size(652, 438);
      this.Load += new System.EventHandler(this.HandleLoad);
      ((System.ComponentModel.ISupportInitialize)(this.m_database_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.UsageEditor)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_version_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.serverEntryBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_server_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraBars.BarManager m_bar_manager;
    private DevExpress.XtraBars.Bar bar1;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraBars.BarButtonItem m_add_server_cmd;
    private DevExpress.XtraBars.BarButtonItem m_remove_server_cmd;
    private DevExpress.XtraBars.BarButtonItem m_refresh_server_cmd;
    private DevExpress.XtraBars.BarButtonItem m_refresh_cmd;
    private DevExpress.XtraBars.BarButtonItem m_add_db_cmd;
    private DevExpress.XtraBars.BarButtonItem m_remove_db_cmd;
    private DevExpress.XtraGrid.GridControl m_server_grid;
    private System.Windows.Forms.BindingSource serverEntryBindingSource;
    private DevExpress.XtraGrid.Views.Grid.GridView m_database_view;
    private DevExpress.XtraGrid.Views.Grid.GridView m_server_view;
    private DevExpress.XtraGrid.Columns.GridColumn colName;
    private DevExpress.XtraGrid.Columns.GridColumn m_db_name_col;
    private DevExpress.XtraGrid.Columns.GridColumn m_db_ok_col;
    private DevExpress.XtraGrid.Columns.GridColumn m_db_used_col;
    private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit UsageEditor;


    private DevExpress.Utils.SuperToolTip m_add_server_supertip = new SuperToolTip();
    private DevExpress.Utils.ToolTipTitleItem m_add_server_tip_title = new ToolTipTitleItem() ;
    private DevExpress.Utils.ToolTipItem m_add_server_tip = new ToolTipItem();
    
    private DevExpress.Utils.SuperToolTip m_refresh_server_supertip = new SuperToolTip();
    private DevExpress.Utils.ToolTipTitleItem m_refresh_server_tip_title = new ToolTipTitleItem();
    private DevExpress.Utils.ToolTipItem m_refresh_server_tip = new ToolTipItem();

    private DevExpress.Utils.SuperToolTip m_remove_server_supertip = new SuperToolTip();
    private DevExpress.Utils.ToolTipTitleItem m_remove_server_tip_title = new ToolTipTitleItem();
    private DevExpress.Utils.ToolTipItem m_remove_server_tip = new ToolTipItem();
    private DevExpress.XtraBars.BarButtonItem m_reset_db_cmd;
    private DevExpress.XtraBars.BarButtonItem m_mould_cmd;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit m_version_edit;
    private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
    private DevExpress.XtraBars.BarButtonItem m_backup_cmd;
    private DevExpress.XtraBars.BarButtonItem m_restore_cmd;
    private DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit repositoryItemPopupContainerEdit1;
    private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    private DevExpress.XtraBars.BarSubItem m_import_data_cmd;
    private DevExpress.XtraGrid.Columns.GridColumn m_db_version_column;
    private DevExpress.XtraBars.BarButtonItem m_cmd_script;
  }
}
