using AT.Toolbox.Help;

namespace ATHelpEditor
{
  partial class DevHelpNavigator
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DevHelpNavigator));
      AT.Toolbox.Help.HelpEntry helpEntry1 = new AT.Toolbox.Help.HelpEntry();
      AT.Toolbox.Help.HelpEntry helpEntry2 = new AT.Toolbox.Help.HelpEntry();
      AT.Toolbox.Help.HelpEntry helpEntry3 = new AT.Toolbox.Help.HelpEntry();
      AT.Toolbox.Help.HelpEntry helpEntry4 = new AT.Toolbox.Help.HelpEntry();
      this.m_cmd_new = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_delete = new DevExpress.XtraBars.BarButtonItem();
      this.colDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.colID = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
      this.bar1 = new DevExpress.XtraBars.Bar();
      this.m_cmd_import = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_import_prj = new DevExpress.XtraBars.BarButtonItem();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
      this.helpFileNodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.treeList1 = new DevExpress.XtraTreeList.TreeList();
      this.helpMapper1 = new AT.Toolbox.Help.HelpMapper(this.components);
      this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpFileNodeBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpMapper1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_cmd_new
      // 
      this.m_cmd_new.Caption = "New Help Topic";
      this.m_cmd_new.Glyph = global::ATHelpEditor.Properties.Resources._24_book_blue_add;
      this.m_cmd_new.Id = 0;
      this.m_cmd_new.Name = "m_cmd_new";
      this.m_cmd_new.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnNewTopic);
      // 
      // m_cmd_delete
      // 
      this.m_cmd_delete.Caption = "Delete";
      this.m_cmd_delete.Glyph = global::ATHelpEditor.Properties.Resources._24_book_blue_delete;
      this.m_cmd_delete.Id = 1;
      this.m_cmd_delete.Name = "m_cmd_delete";
      this.m_cmd_delete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnDeleteTopic);
      // 
      // colDisplayName
      // 
      this.colDisplayName.Caption = "DisplayName";
      this.colDisplayName.FieldName = "DisplayName";
      this.colDisplayName.Name = "colDisplayName";
      this.colDisplayName.Visible = true;
      this.colDisplayName.VisibleIndex = 0;
      this.colDisplayName.Width = 98;
      // 
      // colID
      // 
      this.colID.Caption = "ID";
      this.colID.FieldName = "ID";
      this.colID.Name = "colID";
      this.colID.OptionsColumn.AllowEdit = false;
      this.colID.OptionsColumn.ReadOnly = true;
      this.colID.Visible = true;
      this.colID.VisibleIndex = 1;
      // 
      // barManager1
      // 
      this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
      this.barManager1.DockControls.Add(this.barDockControlTop);
      this.barManager1.DockControls.Add(this.barDockControlBottom);
      this.barManager1.DockControls.Add(this.barDockControlLeft);
      this.barManager1.DockControls.Add(this.barDockControlRight);
      this.barManager1.Form = this;
      this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_cmd_new,
            this.m_cmd_delete,
            this.m_cmd_import,
            this.barButtonItem4,
            this.m_cmd_import_prj});
      this.barManager1.MaxItemId = 5;
      // 
      // bar1
      // 
      this.bar1.BarName = "Tools";
      this.bar1.DockCol = 0;
      this.bar1.DockRow = 0;
      this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_new),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_delete),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_import),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_import_prj)});
      this.bar1.Text = "Tools";
      // 
      // m_cmd_import
      // 
      this.m_cmd_import.Caption = "Import";
      this.m_cmd_import.Id = 2;
      this.m_cmd_import.Name = "m_cmd_import";
      this.m_cmd_import.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Import);
      // 
      // m_cmd_import_prj
      // 
      this.m_cmd_import_prj.Caption = "Import Project";
      this.m_cmd_import_prj.Id = 4;
      this.m_cmd_import_prj.Name = "m_cmd_import_prj";
      this.m_cmd_import_prj.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ImportProject);
      // 
      // barDockControlTop
      // 
      this.toolTipController1.SetSuperTip(this.barDockControlTop, null);
      // 
      // barDockControlBottom
      // 
      this.toolTipController1.SetSuperTip(this.barDockControlBottom, null);
      // 
      // barDockControlLeft
      // 
      this.toolTipController1.SetSuperTip(this.barDockControlLeft, null);
      // 
      // barDockControlRight
      // 
      this.toolTipController1.SetSuperTip(this.barDockControlRight, null);
      // 
      // barButtonItem4
      // 
      this.barButtonItem4.Caption = "Import Project";
      this.barButtonItem4.Id = 3;
      this.barButtonItem4.Name = "barButtonItem4";
      // 
      // helpFileNodeBindingSource
      // 
      this.helpFileNodeBindingSource.DataSource = typeof(AT.Toolbox.Help.HelpFileNode);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "16_book.png");
      this.imageList1.Images.SetKeyName(1, "16_book_open.png");
      this.imageList1.Images.SetKeyName(2, "16_help.png");
      this.imageList1.Images.SetKeyName(3, "16_pin_blue.png");
      // 
      // treeList1
      // 
      this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colDisplayName,
            this.colID});
      this.treeList1.DataSource = this.helpFileNodeBindingSource;
      this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeList1.Location = new System.Drawing.Point(0, 34);
      this.treeList1.Name = "treeList1";
      this.treeList1.OptionsBehavior.AllowExpandOnDblClick = false;
      this.treeList1.OptionsBehavior.AllowIncrementalSearch = true;
      this.treeList1.OptionsBehavior.DragNodes = true;
      this.treeList1.SelectImageList = this.imageList1;
      this.treeList1.Size = new System.Drawing.Size(315, 480);
      this.treeList1.TabIndex = 4;
      this.treeList1.DoubleClick += new System.EventHandler(this.treeList1_DoubleClick);
      this.treeList1.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeList1_GetSelectImage);
      // 
      // helpMapper1
      // 
      helpEntry1.HelpLink = "NewHelpTopic";
      helpEntry1.Target = this.m_cmd_new;
      helpEntry2.HelpLink = "DeleteTopic";
      helpEntry2.Target = this.m_cmd_delete;
      helpEntry3.HelpLink = "DisplayName";
      helpEntry3.Target = this.colDisplayName;
      helpEntry4.HelpLink = "TopicID";
      helpEntry4.Target = this.colID;
      this.helpMapper1.HelpMap = new AT.Toolbox.Help.HelpEntry[] {
        helpEntry1,
        helpEntry2,
        helpEntry3,
        helpEntry4};
      this.helpMapper1.MainTopic = "DevHelpBrowser";
      // 
      // toolTipController1
      // 
      this.toolTipController1.Appearance.Options.UseTextOptions = true;
      this.toolTipController1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
      this.toolTipController1.AutoPopDelay = 2000;
      this.toolTipController1.IconSize = DevExpress.Utils.ToolTipIconSize.Large;
      this.toolTipController1.ShowBeak = true;
      this.toolTipController1.ToolTipLocation = DevExpress.Utils.ToolTipLocation.RightBottom;
      this.toolTipController1.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
      // 
      // DevHelpNavigator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.treeList1);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Name = "DevHelpNavigator";
      this.Size = new System.Drawing.Size(315, 514);
      this.toolTipController1.SetSuperTip(this, null);
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpFileNodeBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpMapper1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraBars.BarManager barManager1;
    private DevExpress.XtraBars.Bar bar1;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private System.Windows.Forms.BindingSource helpFileNodeBindingSource;
    private DevExpress.XtraBars.BarButtonItem m_cmd_new;
    private DevExpress.XtraBars.BarButtonItem m_cmd_delete;
    private System.Windows.Forms.ImageList imageList1;
    private DevExpress.XtraTreeList.TreeList treeList1;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colDisplayName;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colID;
    private HelpMapper helpMapper1;
    private DevExpress.Utils.ToolTipController toolTipController1;
    private DevExpress.XtraBars.BarButtonItem m_cmd_import;
    private DevExpress.XtraBars.BarButtonItem barButtonItem4;
    private DevExpress.XtraBars.BarButtonItem m_cmd_import_prj;
  }
}
