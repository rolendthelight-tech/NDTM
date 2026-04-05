namespace ATDatabaseManager
{
  partial class MouldEditForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MouldEditForm));
      this.m_split_container = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_split_fd = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_bindable_tree = new ATDatabaseManager.BindableTree();
      this.m_command_panel = new DevExpress.XtraEditors.PanelControl();
      this.m_btn_test_fd = new DevExpress.XtraEditors.SimpleButton();
      this.m_label_title = new DevExpress.XtraEditors.LabelControl();
      this.m_label_save_path = new DevExpress.XtraEditors.LabelControl();
      this.m_label_generate_path = new DevExpress.XtraEditors.LabelControl();
      this.m_save_path_edit = new DevExpress.XtraEditors.ButtonEdit();
      this.m_btn_save = new DevExpress.XtraEditors.SimpleButton();
      this.m_btn_generate = new DevExpress.XtraEditors.SimpleButton();
      this.m_generate_path_edit = new DevExpress.XtraEditors.ButtonEdit();
      this.m_check_replace = new DevExpress.XtraEditors.CheckEdit();
      this.m_group_fd = new DevExpress.XtraEditors.GroupControl();
      this.m_grid_fd = new DevExpress.XtraGrid.GridControl();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_grid_fd_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colTableName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_table_edit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
      this.colColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_column_edit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
      this.colCaption = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colCategory = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colEditableOnCreate = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colVisible = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colReadOnly = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_property_grid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
      this.m_grid_extender = new AT.Toolbox.Extensions.GridControlExtender(this.components);
      this.m_delete_feature = new AT.Toolbox.Extensions.GridControlDeleteFeature(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).BeginInit();
      this.m_split_container.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_split_fd)).BeginInit();
      this.m_split_fd.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_command_panel)).BeginInit();
      this.m_command_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_save_path_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_generate_path_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_replace.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_group_fd)).BeginInit();
      this.m_group_fd.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_fd)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_fd_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_column_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_property_grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_extender)).BeginInit();
      this.SuspendLayout();
      // 
      // m_split_container
      // 
      this.m_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_split_container.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
      this.m_split_container.Location = new System.Drawing.Point(0, 0);
      this.m_split_container.Name = "m_split_container";
      this.m_split_container.Panel1.Controls.Add(this.m_split_fd);
      this.m_split_container.Panel1.Text = "Panel1";
      this.m_split_container.Panel2.Controls.Add(this.m_property_grid);
      this.m_split_container.Panel2.Text = "Panel2";
      this.m_split_container.Size = new System.Drawing.Size(850, 509);
      this.m_split_container.SplitterPosition = 211;
      this.m_split_container.TabIndex = 0;
      this.m_split_container.Text = "splitContainerControl1";
      // 
      // m_split_fd
      // 
      this.m_split_fd.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_split_fd.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
      this.m_split_fd.Horizontal = false;
      this.m_split_fd.Location = new System.Drawing.Point(0, 0);
      this.m_split_fd.Name = "m_split_fd";
      this.m_split_fd.Panel1.Controls.Add(this.m_bindable_tree);
      this.m_split_fd.Panel1.Controls.Add(this.m_command_panel);
      this.m_split_fd.Panel1.MinSize = 240;
      this.m_split_fd.Panel1.Text = "Panel1";
      this.m_split_fd.Panel2.Controls.Add(this.m_group_fd);
      this.m_split_fd.Panel2.Text = "Panel2";
      this.m_split_fd.Size = new System.Drawing.Size(633, 509);
      this.m_split_fd.SplitterPosition = 259;
      this.m_split_fd.TabIndex = 5;
      this.m_split_fd.Text = "splitContainerControl1";
      // 
      // m_bindable_tree
      // 
      this.m_bindable_tree.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_bindable_tree.HideSelection = false;
      this.m_bindable_tree.Location = new System.Drawing.Point(208, 0);
      this.m_bindable_tree.Name = "m_bindable_tree";
      this.m_bindable_tree.Size = new System.Drawing.Size(425, 244);
      this.m_bindable_tree.TabIndex = 2;
      this.m_bindable_tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.m_bindable_tree_AfterSelect);
      // 
      // m_command_panel
      // 
      this.m_command_panel.Controls.Add(this.m_btn_test_fd);
      this.m_command_panel.Controls.Add(this.m_label_title);
      this.m_command_panel.Controls.Add(this.m_label_save_path);
      this.m_command_panel.Controls.Add(this.m_label_generate_path);
      this.m_command_panel.Controls.Add(this.m_save_path_edit);
      this.m_command_panel.Controls.Add(this.m_btn_save);
      this.m_command_panel.Controls.Add(this.m_btn_generate);
      this.m_command_panel.Controls.Add(this.m_generate_path_edit);
      this.m_command_panel.Controls.Add(this.m_check_replace);
      this.m_command_panel.Dock = System.Windows.Forms.DockStyle.Left;
      this.m_command_panel.Location = new System.Drawing.Point(0, 0);
      this.m_command_panel.Name = "m_command_panel";
      this.m_command_panel.Size = new System.Drawing.Size(208, 244);
      this.m_command_panel.TabIndex = 1;
      // 
      // m_btn_test_fd
      // 
      this.m_btn_test_fd.Location = new System.Drawing.Point(12, 172);
      this.m_btn_test_fd.Name = "m_btn_test_fd";
      this.m_btn_test_fd.Size = new System.Drawing.Size(184, 23);
      this.m_btn_test_fd.TabIndex = 3;
      this.m_btn_test_fd.Text = "Test field description";
      this.m_btn_test_fd.Click += new System.EventHandler(this.m_btn_test_fd_Click);
      // 
      // m_label_title
      // 
      this.m_label_title.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_label_title.Appearance.Options.UseFont = true;
      this.m_label_title.Location = new System.Drawing.Point(12, 24);
      this.m_label_title.Name = "m_label_title";
      this.m_label_title.Size = new System.Drawing.Size(110, 16);
      this.m_label_title.TabIndex = 3;
      this.m_label_title.Text = "Available actions";
      // 
      // m_label_save_path
      // 
      this.m_label_save_path.Location = new System.Drawing.Point(12, 128);
      this.m_label_save_path.Name = "m_label_save_path";
      this.m_label_save_path.Size = new System.Drawing.Size(49, 13);
      this.m_label_save_path.TabIndex = 3;
      this.m_label_save_path.Text = "Save path";
      // 
      // m_label_generate_path
      // 
      this.m_label_generate_path.Location = new System.Drawing.Point(12, 83);
      this.m_label_generate_path.Name = "m_label_generate_path";
      this.m_label_generate_path.Size = new System.Drawing.Size(70, 13);
      this.m_label_generate_path.TabIndex = 4;
      this.m_label_generate_path.Text = "Generate path";
      // 
      // m_save_path_edit
      // 
      this.m_save_path_edit.Location = new System.Drawing.Point(12, 147);
      this.m_save_path_edit.Name = "m_save_path_edit";
      this.m_save_path_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_save_path_edit.Size = new System.Drawing.Size(184, 20);
      this.m_save_path_edit.TabIndex = 3;
      this.m_save_path_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_save_path_edit_ButtonClick);
      // 
      // m_btn_save
      // 
      this.m_btn_save.Location = new System.Drawing.Point(12, 201);
      this.m_btn_save.Name = "m_btn_save";
      this.m_btn_save.Size = new System.Drawing.Size(85, 23);
      this.m_btn_save.TabIndex = 3;
      this.m_btn_save.Text = "Save";
      this.m_btn_save.Click += new System.EventHandler(this.m_btn_save_Click);
      // 
      // m_btn_generate
      // 
      this.m_btn_generate.Location = new System.Drawing.Point(103, 201);
      this.m_btn_generate.Name = "m_btn_generate";
      this.m_btn_generate.Size = new System.Drawing.Size(93, 23);
      this.m_btn_generate.TabIndex = 3;
      this.m_btn_generate.Text = "Generate";
      this.m_btn_generate.Click += new System.EventHandler(this.m_btn_generate_Click);
      // 
      // m_generate_path_edit
      // 
      this.m_generate_path_edit.Location = new System.Drawing.Point(12, 102);
      this.m_generate_path_edit.Name = "m_generate_path_edit";
      this.m_generate_path_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_generate_path_edit.Size = new System.Drawing.Size(184, 20);
      this.m_generate_path_edit.TabIndex = 3;
      this.m_generate_path_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_generate_path_edit_ButtonClick);
      // 
      // m_check_replace
      // 
      this.m_check_replace.Location = new System.Drawing.Point(10, 58);
      this.m_check_replace.Name = "m_check_replace";
      this.m_check_replace.Properties.Caption = "Replace customizable code";
      this.m_check_replace.Size = new System.Drawing.Size(186, 19);
      this.m_check_replace.TabIndex = 1;
      // 
      // m_group_fd
      // 
      this.m_group_fd.Controls.Add(this.m_grid_fd);
      this.m_group_fd.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_group_fd.Location = new System.Drawing.Point(0, 0);
      this.m_group_fd.Name = "m_group_fd";
      this.m_group_fd.Size = new System.Drawing.Size(633, 259);
      this.m_group_fd.TabIndex = 4;
      this.m_group_fd.Text = "Field description";
      // 
      // m_grid_fd
      // 
      this.m_grid_fd.DataSource = this.m_binding_source;
      this.m_grid_fd.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid_fd.Location = new System.Drawing.Point(2, 22);
      this.m_grid_fd.MainView = this.m_grid_fd_view;
      this.m_grid_fd.Name = "m_grid_fd";
      this.m_grid_fd.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_column_edit,
            this.m_table_edit});
      this.m_grid_fd.Size = new System.Drawing.Size(629, 235);
      this.m_grid_fd.TabIndex = 0;
      this.m_grid_fd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_grid_fd_view});
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataMember = "Fields";
      this.m_binding_source.DataSource = typeof(AT.Toolbox.MSSQL.DbMould.FieldDescriptionContainer);
      // 
      // m_grid_fd_view
      // 
      this.m_grid_fd_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTableName,
            this.colColumnName,
            this.colCaption,
            this.colCategory,
            this.colEditableOnCreate,
            this.colVisible,
            this.colReadOnly});
      this.m_grid_fd_view.GridControl = this.m_grid_fd;
      this.m_grid_fd_view.Name = "m_grid_fd_view";
      this.m_grid_fd_view.OptionsView.ColumnAutoWidth = false;
      this.m_grid_fd_view.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
      this.m_grid_fd_view.OptionsView.ShowAutoFilterRow = true;
      this.m_grid_fd_view.OptionsView.ShowGroupPanel = false;
      // 
      // colTableName
      // 
      this.colTableName.Caption = "TableName";
      this.colTableName.ColumnEdit = this.m_table_edit;
      this.colTableName.FieldName = "TableName";
      this.colTableName.Name = "colTableName";
      this.colTableName.Visible = true;
      this.colTableName.VisibleIndex = 0;
      // 
      // m_table_edit
      // 
      this.m_table_edit.AutoHeight = false;
      this.m_table_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_table_edit.Name = "m_table_edit";
      // 
      // colColumnName
      // 
      this.colColumnName.Caption = "ColumnName";
      this.colColumnName.ColumnEdit = this.m_column_edit;
      this.colColumnName.FieldName = "ColumnName";
      this.colColumnName.Name = "colColumnName";
      this.colColumnName.Visible = true;
      this.colColumnName.VisibleIndex = 1;
      // 
      // m_column_edit
      // 
      this.m_column_edit.AutoHeight = false;
      this.m_column_edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_column_edit.Name = "m_column_edit";
      // 
      // colCaption
      // 
      this.colCaption.Caption = "Caption";
      this.colCaption.FieldName = "Caption";
      this.colCaption.Name = "colCaption";
      this.colCaption.Visible = true;
      this.colCaption.VisibleIndex = 2;
      // 
      // colCategory
      // 
      this.colCategory.Caption = "Category";
      this.colCategory.FieldName = "Category";
      this.colCategory.Name = "colCategory";
      this.colCategory.Visible = true;
      this.colCategory.VisibleIndex = 3;
      // 
      // colEditableOnCreate
      // 
      this.colEditableOnCreate.Caption = "Editable on create";
      this.colEditableOnCreate.FieldName = "EditableOnCreate";
      this.colEditableOnCreate.Name = "colEditableOnCreate";
      this.colEditableOnCreate.Visible = true;
      this.colEditableOnCreate.VisibleIndex = 6;
      // 
      // colVisible
      // 
      this.colVisible.Caption = "Visible";
      this.colVisible.FieldName = "Visible";
      this.colVisible.Name = "colVisible";
      this.colVisible.Visible = true;
      this.colVisible.VisibleIndex = 4;
      // 
      // colReadOnly
      // 
      this.colReadOnly.Caption = "ReadOnly";
      this.colReadOnly.FieldName = "ReadOnly";
      this.colReadOnly.Name = "colReadOnly";
      this.colReadOnly.Visible = true;
      this.colReadOnly.VisibleIndex = 5;
      // 
      // m_property_grid
      // 
      this.m_property_grid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.m_property_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_property_grid.Location = new System.Drawing.Point(0, 0);
      this.m_property_grid.Name = "m_property_grid";
      this.m_property_grid.Size = new System.Drawing.Size(211, 509);
      this.m_property_grid.TabIndex = 0;
      this.m_property_grid.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(this.m_property_grid_FocusedRowChanged);
      // 
      // m_grid_extender
      // 
      this.m_grid_extender.Features.Add(this.m_delete_feature);
      this.m_grid_extender.Grid = this.m_grid_fd;
      // 
      // MouldEditForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(850, 509);
      this.Controls.Add(this.m_split_container);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Name = "MouldEditForm";
      this.Text = "MouldEditForm";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MouldEditForm_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).EndInit();
      this.m_split_container.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_split_fd)).EndInit();
      this.m_split_fd.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_command_panel)).EndInit();
      this.m_command_panel.ResumeLayout(false);
      this.m_command_panel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_save_path_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_generate_path_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_replace.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_group_fd)).EndInit();
      this.m_group_fd.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_fd)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_fd_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_column_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_property_grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_extender)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SplitContainerControl m_split_container;
    private BindableTree m_bindable_tree;
    private DevExpress.XtraEditors.PanelControl m_command_panel;
    private DevExpress.XtraVerticalGrid.PropertyGridControl m_property_grid;
    private DevExpress.XtraEditors.SimpleButton m_btn_save;
    private DevExpress.XtraEditors.SimpleButton m_btn_generate;
    private DevExpress.XtraEditors.ButtonEdit m_generate_path_edit;
    private DevExpress.XtraEditors.CheckEdit m_check_replace;
    private DevExpress.XtraEditors.ButtonEdit m_save_path_edit;
    private DevExpress.XtraEditors.LabelControl m_label_save_path;
    private DevExpress.XtraEditors.LabelControl m_label_generate_path;
    private DevExpress.XtraEditors.LabelControl m_label_title;
    private DevExpress.XtraEditors.SplitContainerControl m_split_fd;
    private DevExpress.XtraEditors.GroupControl m_group_fd;
    private DevExpress.XtraGrid.GridControl m_grid_fd;
    private DevExpress.XtraGrid.Views.Grid.GridView m_grid_fd_view;
    private System.Windows.Forms.BindingSource m_binding_source;
    private DevExpress.XtraGrid.Columns.GridColumn colTableName;
    private DevExpress.XtraGrid.Columns.GridColumn colColumnName;
    private DevExpress.XtraGrid.Columns.GridColumn colCaption;
    private DevExpress.XtraGrid.Columns.GridColumn colEditableOnCreate;
    private DevExpress.XtraGrid.Columns.GridColumn colVisible;
    private DevExpress.XtraGrid.Columns.GridColumn colReadOnly;
    private DevExpress.XtraEditors.Repository.RepositoryItemComboBox m_table_edit;
    private DevExpress.XtraEditors.Repository.RepositoryItemComboBox m_column_edit;
    private AT.Toolbox.Extensions.GridControlExtender m_grid_extender;
    private AT.Toolbox.Extensions.GridControlDeleteFeature m_delete_feature;
    private DevExpress.XtraEditors.SimpleButton m_btn_test_fd;
    private DevExpress.XtraGrid.Columns.GridColumn colCategory;
  }
}