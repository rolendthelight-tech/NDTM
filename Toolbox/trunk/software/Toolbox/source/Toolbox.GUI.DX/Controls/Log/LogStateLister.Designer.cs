namespace Toolbox.GUI.DX.Controls.Log
{
  partial class LogStateLister
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
      this.colEventDate = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colIcon = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_status_image = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
      this.colMessage = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_message_edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
      this.m_level_flt = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
      this.m_bar_manager = new DevExpress.XtraBars.BarManager(this.components);
      this.m_menu = new DevExpress.XtraBars.Bar();
      this.m_check_errors = new DevExpress.XtraBars.BarCheckItem();
      this.m_check_warnings = new DevExpress.XtraBars.BarCheckItem();
      this.m_check_messages = new DevExpress.XtraBars.BarCheckItem();
      this.m_check_debug = new DevExpress.XtraBars.BarCheckItem();
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
      this.SuspendLayout();
      // 
      // m_grid
      // 
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 24);
      this.m_grid.MainView = this.m_grid_view;
      this.m_grid.Name = "m_grid";
      this.m_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_status_image,
            this.m_message_edit,
            this.m_level_flt});
      this.m_grid.ShowOnlyPredefinedDetails = true;
      this.m_grid.Size = new System.Drawing.Size(535, 240);
      this.m_grid.TabIndex = 1;
      this.m_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_grid_view});
      // 
      // m_grid_view
      // 
      this.m_grid_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colEventDate,
            this.colIcon,
            this.colMessage});
      this.m_grid_view.GridControl = this.m_grid;
      this.m_grid_view.Name = "m_grid_view";
      this.m_grid_view.OptionsCustomization.AllowColumnMoving = false;
      this.m_grid_view.OptionsCustomization.AllowGroup = false;
      this.m_grid_view.OptionsCustomization.AllowSort = false;
      this.m_grid_view.OptionsMenu.EnableColumnMenu = false;
      this.m_grid_view.OptionsMenu.EnableFooterMenu = false;
      this.m_grid_view.OptionsMenu.EnableGroupPanelMenu = false;
      this.m_grid_view.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
      this.m_grid_view.OptionsView.ShowGroupPanel = false;
      this.m_grid_view.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.m_grid_view_CustomDrawCell);
      this.m_grid_view.DoubleClick += new System.EventHandler(this.m_grid_view_DoubleClick);
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
      this.colMessage.VisibleIndex = 2;
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
            this.m_check_errors,
            this.m_check_warnings,
            this.m_check_messages,
            this.m_check_debug});
      this.m_bar_manager.MainMenu = this.m_menu;
      this.m_bar_manager.MaxItemId = 10;
      // 
      // m_menu
      // 
      this.m_menu.BarName = "Main menu";
      this.m_menu.DockCol = 0;
      this.m_menu.DockRow = 0;
      this.m_menu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.m_menu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_check_errors),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_check_warnings),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_check_messages),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_check_debug)});
      this.m_menu.OptionsBar.AllowQuickCustomization = false;
      this.m_menu.OptionsBar.DisableCustomization = true;
      this.m_menu.OptionsBar.DrawDragBorder = false;
      this.m_menu.OptionsBar.MultiLine = true;
      this.m_menu.OptionsBar.UseWholeRow = true;
      this.m_menu.Text = "Main menu";
      // 
      // m_check_errors
      // 
      this.m_check_errors.Caption = "Errors";
      this.m_check_errors.Checked = true;
      this.m_check_errors.Id = 6;
      this.m_check_errors.Name = "m_check_errors";
      this.m_check_errors.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.m_check_errors.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleFilterChanged);
      // 
      // m_check_warnings
      // 
      this.m_check_warnings.Caption = "Warnings";
      this.m_check_warnings.Checked = true;
      this.m_check_warnings.Id = 7;
      this.m_check_warnings.Name = "m_check_warnings";
      this.m_check_warnings.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.m_check_warnings.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleFilterChanged);
      // 
      // m_check_messages
      // 
      this.m_check_messages.Caption = "Messages";
      this.m_check_messages.Id = 8;
      this.m_check_messages.Name = "m_check_messages";
      this.m_check_messages.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.m_check_messages.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleFilterChanged);
      // 
      // m_check_debug
      // 
      this.m_check_debug.Caption = "Debug";
      this.m_check_debug.Id = 9;
      this.m_check_debug.Name = "m_check_debug";
      this.m_check_debug.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.m_check_debug.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleFilterChanged);
      // 
      // barDockControlTop
      // 
      this.barDockControlTop.CausesValidation = false;
      this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
      this.barDockControlTop.Size = new System.Drawing.Size(535, 24);
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
      this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
      this.barDockControlLeft.Size = new System.Drawing.Size(0, 240);
      // 
      // barDockControlRight
      // 
      this.barDockControlRight.CausesValidation = false;
      this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
      this.barDockControlRight.Location = new System.Drawing.Point(535, 24);
      this.barDockControlRight.Size = new System.Drawing.Size(0, 240);
      // 
      // LogStateLister
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_grid);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Name = "LogStateLister";
      this.Size = new System.Drawing.Size(535, 264);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_status_image)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_message_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_level_flt)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraGrid.GridControl m_grid;
    private DevExpress.XtraGrid.Views.Grid.GridView m_grid_view;
    private DevExpress.XtraGrid.Columns.GridColumn colEventDate;
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
    private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit m_level_flt;
    private DevExpress.XtraBars.BarCheckItem m_check_errors;
    private DevExpress.XtraBars.BarCheckItem m_check_warnings;
    private DevExpress.XtraBars.BarCheckItem m_check_messages;
    private DevExpress.XtraBars.BarCheckItem m_check_debug;
  }
}
