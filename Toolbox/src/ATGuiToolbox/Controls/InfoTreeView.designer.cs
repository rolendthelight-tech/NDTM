namespace AT.Toolbox.Controls
{
  partial class InfoTreeView
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoTreeView));
      this.m_tree = new DevExpress.XtraTreeList.TreeList();
      this.colMessage = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.m_details_edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
      this.m_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_entries = new AT.Toolbox.Controls.LogEntrySet();
      this.m_picture_edit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
      this.m_images = new System.Windows.Forms.ImageList(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_tree)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_details_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_edit)).BeginInit();
      this.SuspendLayout();
      // 
      // m_tree
      // 
      this.m_tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colMessage});
      this.m_tree.DataSource = this.m_source;
      this.m_tree.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_tree.ImageIndexFieldName = "Level";
      this.m_tree.Location = new System.Drawing.Point(0, 0);
      this.m_tree.Name = "m_tree";
      this.m_tree.OptionsView.ShowColumns = false;
      this.m_tree.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_picture_edit,
            this.m_details_edit});
      this.m_tree.SelectImageList = this.m_images;
      this.m_tree.Size = new System.Drawing.Size(367, 301);
      this.m_tree.TabIndex = 2;
      // 
      // colMessage
      // 
      this.colMessage.Caption = "Message";
      this.colMessage.ColumnEdit = this.m_details_edit;
      this.colMessage.FieldName = "Message";
      this.colMessage.Name = "colMessage";
      this.colMessage.Visible = true;
      this.colMessage.VisibleIndex = 0;
      this.colMessage.Width = 336;
      // 
      // m_details_edit
      // 
      this.m_details_edit.AutoHeight = false;
      this.m_details_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_details_edit.Name = "m_details_edit";
      this.m_details_edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_details_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_details_edit_ButtonClick);
      // 
      // m_source
      // 
      this.m_source.DataSource = this.m_entries;
      // 
      // m_picture_edit
      // 
      this.m_picture_edit.Name = "m_picture_edit";
      // 
      // m_images
      // 
      this.m_images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_images.ImageStream")));
      this.m_images.TransparentColor = System.Drawing.Color.Turquoise;
      this.m_images.Images.SetKeyName(0, "Debug");
      this.m_images.Images.SetKeyName(1, "Info");
      this.m_images.Images.SetKeyName(2, "Warning");
      this.m_images.Images.SetKeyName(3, "Error");
      // 
      // InfoTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_tree);
      this.Name = "InfoTreeView";
      this.Size = new System.Drawing.Size(367, 301);
      ((System.ComponentModel.ISupportInitialize)(this.m_tree)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_details_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_edit)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraTreeList.TreeList m_tree;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colMessage;
    private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit m_details_edit;
    private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit m_picture_edit;
    private LogEntrySet m_entries;
    private System.Windows.Forms.BindingSource m_source;
    private System.Windows.Forms.ImageList m_images;
  }
}
