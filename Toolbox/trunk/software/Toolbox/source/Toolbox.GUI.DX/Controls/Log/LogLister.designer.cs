namespace Toolbox.GUI.DX.Controls.Log
{
  partial class LogLister
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
			this.components = new System.ComponentModel.Container();
			this.m_grid = new DevExpress.XtraGrid.GridControl();
			this.m_grid_view = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colSource = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colEventDate = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colIcon = new DevExpress.XtraGrid.Columns.GridColumn();
			this.m_status_image = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
			this.colMessage = new DevExpress.XtraGrid.Columns.GridColumn();
			this.m_message_edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.m_level_flt = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
			this.m_bar_manager = new DevExpress.XtraBars.BarManager(this.components);
			this.m_menu = new DevExpress.XtraBars.Bar();
			this.m_label_level = new DevExpress.XtraBars.BarStaticItem();
			this.m_level_bar = new DevExpress.XtraBars.BarEditItem();
			this.m_level_edit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
			this.m_cmd_end = new DevExpress.XtraBars.BarButtonItem();
			this.m_cmd_clear = new DevExpress.XtraBars.BarButtonItem();
			this.m_cmd_file = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_status_image)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_message_edit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_level_flt)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_level_edit)).BeginInit();
			this.SuspendLayout();
			// 
			// m_grid
			// 
			this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_grid.Location = new System.Drawing.Point(0, 22);
			this.m_grid.MainView = this.m_grid_view;
			this.m_grid.Name = "m_grid";
			this.m_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_status_image,
            this.m_message_edit,
            this.m_level_flt});
			this.m_grid.ShowOnlyPredefinedDetails = true;
			this.m_grid.Size = new System.Drawing.Size(535, 242);
			this.m_grid.TabIndex = 1;
			this.m_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_grid_view});
			// 
			// m_grid_view
			// 
			this.m_grid_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSource,
            this.colEventDate,
            this.colIcon,
            this.colMessage});
			this.m_grid_view.GridControl = this.m_grid;
			this.m_grid_view.Name = "m_grid_view";
			this.m_grid_view.OptionsView.ShowGroupPanel = false;
			this.m_grid_view.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.m_grid_view_CustomDrawCell);
			this.m_grid_view.DoubleClick += new System.EventHandler(m_grid_view_DoubleClick);
			// 
			// colSource
			// 
			this.colSource.Caption = "Source";
			this.colSource.FieldName = "Source";
			this.colSource.Name = "colSource";
			this.colSource.OptionsColumn.AllowEdit = false;
			this.colSource.Visible = true;
			this.colSource.VisibleIndex = 2;
            this.colSource.Width = 90;
			// 
			// colEventDate
			// 
			this.colEventDate.Caption = "Event time";
			this.colEventDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.colEventDate.FieldName = "EventDate";
			this.colEventDate.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colEventDate.Name = "colEventDate";
			this.colEventDate.OptionsColumn.AllowEdit = false;
			this.colEventDate.OptionsColumn.AllowSize = false;
			this.colEventDate.Visible = true;
			this.colEventDate.VisibleIndex = 1;
			this.colEventDate.Width = 109;
			// 
			// colIcon
			// 
			this.colIcon.Caption = "Level";
			this.colIcon.ColumnEdit = this.m_status_image;
			this.colIcon.FieldName = "Icon";
			this.colIcon.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
			this.colIcon.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colIcon.Name = "colIcon";
			this.colIcon.OptionsColumn.AllowEdit = false;
			this.colIcon.OptionsColumn.AllowSize = false;
			this.colIcon.OptionsColumn.ShowCaption = false;
			this.colIcon.OptionsFilter.AllowAutoFilter = false;
			this.colIcon.OptionsFilter.AllowFilter = false;
			this.colIcon.Visible = true;
			this.colIcon.VisibleIndex = 0;
			this.colIcon.Width = 31;
			// 
			// m_status_image
			// 
			this.m_status_image.Name = "m_status_image";
			this.m_status_image.PictureStoreMode = DevExpress.XtraEditors.Controls.PictureStoreMode.Image;
			this.m_status_image.ReadOnly = true;
			// 
			// colMessage
			// 
			this.colMessage.Caption = "Message";
			this.colMessage.ColumnEdit = this.m_message_edit;
			this.colMessage.FieldName = "DetailedMessage";
			this.colMessage.Name = "colMessage";
			this.colMessage.OptionsColumn.ReadOnly = true;
			this.colMessage.Visible = true;
			this.colMessage.VisibleIndex = 3;
            this.colMessage.Width = 285;
			// 
			// m_message_edit
			// 
			this.m_message_edit.AutoHeight = false;
			this.m_message_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
			this.m_message_edit.Name = "m_message_edit";
			this.m_message_edit.ReadOnly = true;
			this.m_message_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_message_edit_ButtonClick);
			// 
			// m_level_flt
			// 
			this.m_level_flt.AutoHeight = false;
			this.m_level_flt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.m_level_flt.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value")});
			this.m_level_flt.DisplayMember = "Value";
			this.m_level_flt.Name = "m_level_flt";
			this.m_level_flt.ShowHeader = false;
			this.m_level_flt.ValueMember = "Key";
			// 
			// m_bar_manager
			// 
			this.m_bar_manager.AllowCustomization = false;
			this.m_bar_manager.AllowQuickCustomization = false;
			this.m_bar_manager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.m_menu});
			this.m_bar_manager.DockControls.Add(this.barDockControlTop);
			this.m_bar_manager.DockControls.Add(this.barDockControlBottom);
			this.m_bar_manager.DockControls.Add(this.barDockControlLeft);
			this.m_bar_manager.DockControls.Add(this.barDockControlRight);
			this.m_bar_manager.Form = this;
			this.m_bar_manager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_level_bar,
            this.m_label_level,
            this.m_cmd_clear,
            this.m_cmd_end,
            this.m_cmd_file});
			this.m_bar_manager.MainMenu = this.m_menu;
			this.m_bar_manager.MaxItemId = 6;
			this.m_bar_manager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_level_edit});
			// 
			// m_menu
			// 
			this.m_menu.BarName = "Main menu";
			this.m_menu.DockCol = 0;
			this.m_menu.DockRow = 0;
			this.m_menu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.m_menu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_label_level),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.m_level_bar, "", false, true, true, 130),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_end, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_clear, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_file, true)});
			this.m_menu.OptionsBar.AllowQuickCustomization = false;
			this.m_menu.OptionsBar.DisableCustomization = true;
			this.m_menu.OptionsBar.DrawDragBorder = false;
			this.m_menu.OptionsBar.MultiLine = true;
			this.m_menu.OptionsBar.UseWholeRow = true;
			this.m_menu.Text = "Main menu";
			// 
			// m_label_level
			// 
			this.m_label_level.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.m_label_level.Caption = "Level";
			this.m_label_level.Id = 2;
			this.m_label_level.Name = "m_label_level";
			this.m_label_level.TextAlignment = System.Drawing.StringAlignment.Near;
			// 
			// m_level_bar
			// 
			this.m_level_bar.Edit = this.m_level_edit;
			this.m_level_bar.Id = 1;
			this.m_level_bar.Name = "m_level_bar";
			this.m_level_bar.EditValueChanged += new System.EventHandler(this.m_level_edit_EditValueChanged);
			// 
			// m_level_edit
			// 
			this.m_level_edit.AutoHeight = false;
			this.m_level_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.m_level_edit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", " ")});
			this.m_level_edit.DisplayMember = "Value";
			this.m_level_edit.Name = "m_level_edit";
			this.m_level_edit.ShowHeader = false;
			this.m_level_edit.ValueMember = "Key";
			// 
			// m_cmd_end
			// 
			this.m_cmd_end.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			this.m_cmd_end.Caption = "List to end";
			this.m_cmd_end.Id = 4;
			this.m_cmd_end.Name = "m_cmd_end";
			this.m_cmd_end.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.m_cmd_end.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_end_ItemClick);
			// 
			// m_cmd_clear
			// 
			this.m_cmd_clear.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			this.m_cmd_clear.Caption = "Clear";
			this.m_cmd_clear.Id = 3;
			this.m_cmd_clear.Name = "m_cmd_clear";
			this.m_cmd_clear.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.m_cmd_clear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_clear_ItemClick);
			// 
			// m_cmd_file
			// 
			this.m_cmd_file.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			this.m_cmd_file.Caption = "Show log file";
			this.m_cmd_file.Id = 5;
			this.m_cmd_file.Name = "m_cmd_file";
			this.m_cmd_file.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.m_cmd_file.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_file_ItemClick);
			// 
			// barDockControlTop
			// 
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(535, 22);
			// 
			// barDockControlBottom
			// 
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 264);
			this.barDockControlBottom.Size = new System.Drawing.Size(535, 0);
			// 
			// barDockControlLeft
			// 
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 242);
			// 
			// barDockControlRight
			// 
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(535, 22);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 242);
			// 
			// LogLister
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_grid);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "LogLister";
			this.Size = new System.Drawing.Size(535, 264);
			((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_status_image)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_message_edit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_level_flt)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_level_edit)).EndInit();
			this.ResumeLayout(false);

    }

    #endregion

    //private LogEntrySet m_entries;
    private DevExpress.XtraGrid.GridControl m_grid;
    private DevExpress.XtraGrid.Views.Grid.GridView m_grid_view;
    private DevExpress.XtraGrid.Columns.GridColumn colEventDate;
    private DevExpress.XtraGrid.Columns.GridColumn colSource;
    private DevExpress.XtraGrid.Columns.GridColumn colIcon;
		private DevExpress.XtraGrid.Columns.GridColumn colMessage;
    private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit m_status_image;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit m_message_edit;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarManager m_bar_manager;
    private DevExpress.XtraBars.Bar m_menu;
    private DevExpress.XtraBars.BarEditItem m_level_bar;
    private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit m_level_edit;
    private DevExpress.XtraBars.BarStaticItem m_label_level;
    private DevExpress.XtraBars.BarButtonItem m_cmd_clear;
    private DevExpress.XtraBars.BarButtonItem m_cmd_end;
		private DevExpress.XtraBars.BarButtonItem m_cmd_file;
    private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit m_level_flt;

  }
}