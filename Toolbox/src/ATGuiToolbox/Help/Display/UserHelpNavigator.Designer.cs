namespace AT.Toolbox.Help
{
  partial class UserHelpNavigator
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserHelpNavigator));
      this.m_tab_control = new DevExpress.XtraTab.XtraTabControl();
      this.m_tab_contents = new DevExpress.XtraTab.XtraTabPage();
      this.treeList1 = new DevExpress.XtraTreeList.TreeList();
      this.colDisplayName1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.helpFileNodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.m_tab_index = new DevExpress.XtraTab.XtraTabPage();
      this.gridControl1 = new DevExpress.XtraGrid.GridControl();
      this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.helpMapper1 = new AT.Toolbox.Help.HelpMapper(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_tab_control)).BeginInit();
      this.m_tab_control.SuspendLayout();
      this.m_tab_contents.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpFileNodeBindingSource)).BeginInit();
      this.m_tab_index.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpMapper1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_tab_control
      // 
      this.m_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_tab_control.HeaderButtons = DevExpress.XtraTab.TabButtons.None;
      this.m_tab_control.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.Never;
      this.m_tab_control.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
      this.m_tab_control.Location = new System.Drawing.Point(0, 0);
      this.m_tab_control.Name = "m_tab_control";
      this.m_tab_control.SelectedTabPage = this.m_tab_contents;
      this.m_tab_control.Size = new System.Drawing.Size(326, 556);
      this.m_tab_control.TabIndex = 0;
      this.m_tab_control.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.m_tab_contents,
            this.m_tab_index});
      this.m_tab_control.Text = "xtraTabControl1";
      // 
      // m_tab_contents
      // 
      this.m_tab_contents.Controls.Add(this.treeList1);
      this.m_tab_contents.Image = global::AT.Toolbox.Properties.Resources.p_16_book;
      this.m_tab_contents.Name = "m_tab_contents";
      this.m_tab_contents.Size = new System.Drawing.Size(317, 523);
      this.m_tab_contents.Text = "Contents";
      // 
      // treeList1
      // 
      this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colDisplayName1});
      this.treeList1.DataSource = this.helpFileNodeBindingSource;
      this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeList1.Location = new System.Drawing.Point(0, 0);
      this.treeList1.Name = "treeList1";
      this.treeList1.OptionsBehavior.AutoFocusNewNode = true;
      this.treeList1.OptionsBehavior.AutoMoveRowFocus = true;
      this.treeList1.OptionsBehavior.Editable = false;
      this.treeList1.OptionsView.ShowColumns = false;
      this.treeList1.OptionsView.ShowIndicator = false;
      this.treeList1.SelectImageList = this.imageList1;
      this.treeList1.Size = new System.Drawing.Size(317, 523);
      this.treeList1.TabIndex = 0;
      this.treeList1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleTreeMouseClick);
      this.treeList1.DoubleClick += new System.EventHandler(this.HandleTreeDoubleClick);
      this.treeList1.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.HandleTreeGetSelectedImage);
      this.treeList1.Click += new System.EventHandler(this.HadleTreeClick);
      // 
      // colDisplayName1
      // 
      this.colDisplayName1.Caption = "DisplayName";
      this.colDisplayName1.FieldName = "DisplayName";
      this.colDisplayName1.Name = "colDisplayName1";
      this.colDisplayName1.OptionsColumn.AllowEdit = false;
      this.colDisplayName1.OptionsColumn.ReadOnly = true;
      this.colDisplayName1.Visible = true;
      this.colDisplayName1.VisibleIndex = 0;
      this.colDisplayName1.Width = 74;
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
      // m_tab_index
      // 
      this.m_tab_index.Controls.Add(this.gridControl1);
      this.m_tab_index.Image = global::AT.Toolbox.Properties.Resources.p_16_help;
      this.m_tab_index.Name = "m_tab_index";
      this.m_tab_index.Size = new System.Drawing.Size(317, 523);
      this.m_tab_index.Text = "Index";
      // 
      // gridControl1
      // 
      this.gridControl1.DataSource = this.helpFileNodeBindingSource;
      this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridControl1.EmbeddedNavigator.Name = "";
      this.gridControl1.Location = new System.Drawing.Point(0, 0);
      this.gridControl1.MainView = this.gridView1;
      this.gridControl1.Name = "gridControl1";
      this.gridControl1.ShowOnlyPredefinedDetails = true;
      this.gridControl1.Size = new System.Drawing.Size(317, 523);
      this.gridControl1.TabIndex = 0;
      this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
      this.gridControl1.DoubleClick += new System.EventHandler(this.HandleGridDoubleClick);
      // 
      // gridView1
      // 
      this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDisplayName});
      this.gridView1.GridControl = this.gridControl1;
      this.gridView1.Name = "gridView1";
      this.gridView1.OptionsBehavior.AllowIncrementalSearch = true;
      this.gridView1.OptionsCustomization.AllowGroup = false;
      this.gridView1.OptionsSelection.UseIndicatorForSelection = false;
      this.gridView1.OptionsView.ShowAutoFilterRow = true;
      this.gridView1.OptionsView.ShowColumnHeaders = false;
      this.gridView1.OptionsView.ShowGroupPanel = false;
      this.gridView1.OptionsView.ShowHorzLines = false;
      this.gridView1.OptionsView.ShowIndicator = false;
      this.gridView1.OptionsView.ShowVertLines = false;
      // 
      // colDisplayName
      // 
      this.colDisplayName.Caption = "DisplayName";
      this.colDisplayName.FieldName = "DisplayName";
      this.colDisplayName.Name = "colDisplayName";
      this.colDisplayName.OptionsColumn.AllowEdit = false;
      this.colDisplayName.OptionsColumn.ReadOnly = true;
      this.colDisplayName.Visible = true;
      this.colDisplayName.VisibleIndex = 0;
      // 
      // helpMapper1
      // 
      this.helpMapper1.HelpMap = new AT.Toolbox.Help.HelpEntry[0];
      this.helpMapper1.MainTopic = null;
      // 
      // UserHelpNavigator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_tab_control);
      this.Name = "UserHelpNavigator";
      this.Size = new System.Drawing.Size(326, 556);
      ((System.ComponentModel.ISupportInitialize)(this.m_tab_control)).EndInit();
      this.m_tab_control.ResumeLayout(false);
      this.m_tab_contents.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpFileNodeBindingSource)).EndInit();
      this.m_tab_index.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpMapper1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraTab.XtraTabControl m_tab_control;
    private DevExpress.XtraTab.XtraTabPage m_tab_contents;
    private DevExpress.XtraTab.XtraTabPage m_tab_index;
    private HelpMapper helpMapper1;
    private DevExpress.XtraTreeList.TreeList treeList1;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colDisplayName1;
    private System.Windows.Forms.BindingSource helpFileNodeBindingSource;
    private DevExpress.XtraGrid.GridControl gridControl1;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    private DevExpress.XtraGrid.Columns.GridColumn colDisplayName;
    private System.Windows.Forms.ImageList imageList1;
  }
}
