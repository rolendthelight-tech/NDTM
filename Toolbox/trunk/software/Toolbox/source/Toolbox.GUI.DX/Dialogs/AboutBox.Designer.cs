using Toolbox.GUI.DX.Controls;

namespace Toolbox.GUI.DX.Dialogs
{
  public partial class AboutBox
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
    /// Required method for Designer support — do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.m_main_group = new DevExpress.XtraEditors.GroupControl();
      this.m_modules = new DevExpress.XtraEditors.GroupControl();
      this.m_module_grid = new DevExpress.XtraGrid.GridControl();
      this.m_module_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.m_col_company = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_col_name = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_col_version = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_col_ok = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_state_icon = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
      this.m_copyright = new DevExpress.XtraEditors.LabelControl();
      this.m_add_info = new DevExpress.XtraEditors.GroupControl();
      this.m_info_edit = new DevExpress.XtraEditors.MemoEdit();
      this.m_close_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_version = new DevExpress.XtraEditors.LabelControl();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      this.m_dragger = new GroupControlDragger(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_main_group)).BeginInit();
      this.m_main_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_modules)).BeginInit();
      this.m_modules.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_module_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_module_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_state_icon)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_add_info)).BeginInit();
      this.m_add_info.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_info_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      this.SuspendLayout();
      // 
      // m_main_group
      // 
      this.m_main_group.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_main_group.AppearanceCaption.Options.UseFont = true;
      this.m_main_group.Controls.Add(this.m_modules);
      this.m_main_group.Controls.Add(this.m_copyright);
      this.m_main_group.Controls.Add(this.m_add_info);
      this.m_main_group.Controls.Add(this.m_close_btn);
      this.m_main_group.Controls.Add(this.m_version);
      this.m_main_group.Controls.Add(this.m_picture_box);
      this.m_main_group.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_main_group.Location = new System.Drawing.Point(0, 0);
      this.m_main_group.Name = "m_main_group";
      this.m_main_group.Size = new System.Drawing.Size(680, 524);
      this.m_main_group.TabIndex = 1;
      this.m_main_group.Text = "About";
      // 
      // m_modules
      // 
      this.m_modules.Controls.Add(this.m_module_grid);
      this.m_modules.Location = new System.Drawing.Point(126, 265);
      this.m_modules.Name = "m_modules";
      this.m_modules.Size = new System.Drawing.Size(549, 218);
      this.m_modules.TabIndex = 9;
      this.m_modules.Text = "groupControl1";
      // 
      // m_module_grid
      // 
      this.m_module_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_module_grid.Location = new System.Drawing.Point(2, 22);
      this.m_module_grid.MainView = this.m_module_view;
      this.m_module_grid.Name = "m_module_grid";
      this.m_module_grid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_state_icon});
      this.m_module_grid.Size = new System.Drawing.Size(545, 194);
      this.m_module_grid.TabIndex = 0;
      this.m_module_grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_module_view});
      // 
      // m_module_view
      // 
      this.m_module_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.m_col_company,
            this.m_col_name,
            this.m_col_version,
            this.m_col_ok});
      this.m_module_view.GridControl = this.m_module_grid;
      this.m_module_view.Name = "m_module_view";
      this.m_module_view.OptionsCustomization.AllowFilter = false;
      this.m_module_view.OptionsCustomization.AllowGroup = false;
      this.m_module_view.OptionsView.ShowGroupPanel = false;
      // 
      // m_col_company
      // 
      this.m_col_company.Caption = "Company";
      this.m_col_company.FieldName = "Company";
      this.m_col_company.Name = "m_col_company";
      this.m_col_company.OptionsColumn.AllowEdit = false;
      this.m_col_company.OptionsColumn.ReadOnly = true;
      this.m_col_company.OptionsFilter.AllowAutoFilter = false;
      this.m_col_company.OptionsFilter.AllowFilter = false;
      this.m_col_company.Visible = true;
      this.m_col_company.VisibleIndex = 1;
      this.m_col_company.Width = 150;
      // 
      // m_col_name
      // 
      this.m_col_name.Caption = "ProductName";
      this.m_col_name.FieldName = "ProductName";
      this.m_col_name.Name = "m_col_name";
      this.m_col_name.OptionsColumn.AllowEdit = false;
      this.m_col_name.OptionsColumn.ReadOnly = true;
      this.m_col_name.OptionsFilter.AllowAutoFilter = false;
      this.m_col_name.OptionsFilter.AllowFilter = false;
      this.m_col_name.Visible = true;
      this.m_col_name.VisibleIndex = 2;
      this.m_col_name.Width = 150;
      // 
      // m_col_version
      // 
      this.m_col_version.Caption = "Version";
      this.m_col_version.FieldName = "Version";
      this.m_col_version.Name = "m_col_version";
      this.m_col_version.OptionsColumn.AllowEdit = false;
      this.m_col_version.OptionsColumn.ReadOnly = true;
      this.m_col_version.OptionsFilter.AllowAutoFilter = false;
      this.m_col_version.OptionsFilter.AllowFilter = false;
      this.m_col_version.Visible = true;
      this.m_col_version.VisibleIndex = 3;
      this.m_col_version.Width = 162;
      // 
      // m_col_ok
      // 
      this.m_col_ok.Caption = "Pic";
      this.m_col_ok.ColumnEdit = this.m_state_icon;
      this.m_col_ok.FieldName = "SupportPic";
      this.m_col_ok.Name = "m_col_ok";
      this.m_col_ok.OptionsColumn.AllowEdit = false;
      this.m_col_ok.OptionsColumn.ReadOnly = true;
      this.m_col_ok.OptionsFilter.AllowAutoFilter = false;
      this.m_col_ok.OptionsFilter.AllowFilter = false;
      this.m_col_ok.Visible = true;
      this.m_col_ok.VisibleIndex = 0;
      this.m_col_ok.Width = 62;
      // 
      // m_state_icon
      // 
      this.m_state_icon.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
      this.m_state_icon.Name = "m_state_icon";
      this.m_state_icon.ReadOnly = true;
      this.m_state_icon.ShowMenu = false;
      // 
      // m_copyright
      // 
      this.m_copyright.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_copyright.Appearance.Options.UseFont = true;
      this.m_copyright.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_copyright.Location = new System.Drawing.Point(128, 55);
      this.m_copyright.Name = "m_copyright";
      this.m_copyright.Size = new System.Drawing.Size(547, 18);
      this.m_copyright.TabIndex = 7;
      this.m_copyright.Text = "Copyright :";
      // 
      // m_add_info
      // 
      this.m_add_info.Controls.Add(this.m_info_edit);
      this.m_add_info.Location = new System.Drawing.Point(126, 83);
      this.m_add_info.Name = "m_add_info";
      this.m_add_info.Size = new System.Drawing.Size(549, 178);
      this.m_add_info.TabIndex = 6;
      this.m_add_info.Text = "Additional information";
      // 
      // m_info_edit
      // 
      this.m_info_edit.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_info_edit.Location = new System.Drawing.Point(2, 22);
      this.m_info_edit.Name = "m_info_edit";
      this.m_info_edit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.m_info_edit.Properties.ReadOnly = true;
      this.m_info_edit.Size = new System.Drawing.Size(545, 154);
      this.m_info_edit.TabIndex = 5;
      // 
      // m_close_btn
      // 
      this.m_close_btn.Location = new System.Drawing.Point(352, 492);
      this.m_close_btn.Name = "m_close_btn";
      this.m_close_btn.Size = new System.Drawing.Size(97, 23);
      this.m_close_btn.TabIndex = 5;
      this.m_close_btn.Text = "Close";
      this.m_close_btn.Click += new System.EventHandler(this.HandleCloseButton);
      // 
      // m_version
      // 
      this.m_version.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_version.Appearance.Options.UseFont = true;
      this.m_version.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_version.Location = new System.Drawing.Point(128, 31);
      this.m_version.Name = "m_version";
      this.m_version.Size = new System.Drawing.Size(547, 18);
      this.m_version.TabIndex = 1;
      this.m_version.Text = "Version :";
      // 
      // m_picture_box
      // 
      this.m_picture_box.Dock = System.Windows.Forms.DockStyle.Left;
      this.m_picture_box.Location = new System.Drawing.Point(2, 28);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.Size = new System.Drawing.Size(120, 494);
      this.m_picture_box.TabIndex = 0;
      this.m_picture_box.TabStop = false;
      // 
      // m_dragger
      // 
      this.m_dragger.CanMove = true;
      this.m_dragger.CanSize = false;
      this.m_dragger.GroupControlToDragBy = this.m_main_group;
      this.m_dragger.SizeBounds = 3;
      // 
      // AboutBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(680, 524);
      this.Controls.Add(this.m_main_group);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "AboutBox";
      this.Shown += new System.EventHandler(this.HandleShown);
      ((System.ComponentModel.ISupportInitialize)(this.m_main_group)).EndInit();
      this.m_main_group.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_modules)).EndInit();
      this.m_modules.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_module_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_module_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_state_icon)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_add_info)).EndInit();
      this.m_add_info.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_info_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.GroupControl m_main_group;
    private DevExpress.XtraEditors.LabelControl m_version;
    private System.Windows.Forms.PictureBox m_picture_box;
    private DevExpress.XtraEditors.SimpleButton m_close_btn;
    private DevExpress.XtraEditors.GroupControl m_add_info;
    private DevExpress.XtraEditors.MemoEdit m_info_edit;
    private DevExpress.XtraEditors.LabelControl m_copyright;
    private DevExpress.XtraEditors.GroupControl m_modules;
    private DevExpress.XtraGrid.GridControl m_module_grid;
    private DevExpress.XtraGrid.Views.Grid.GridView m_module_view;
    private DevExpress.XtraGrid.Columns.GridColumn m_col_company;
    private DevExpress.XtraGrid.Columns.GridColumn m_col_name;
    private DevExpress.XtraGrid.Columns.GridColumn m_col_version;
    private DevExpress.XtraGrid.Columns.GridColumn m_col_ok;
    private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit m_state_icon;
    private GroupControlDragger m_dragger;

  }
}
