namespace AT.Toolbox.MSSQL
{
  partial class CSVImportForm
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
      this.m_label_table = new DevExpress.XtraEditors.LabelControl();
      this.m_csv_mapper = new AT.Toolbox.MSSQL.CSVMapper(this.components);
      this.m_table_edit = new DevExpress.XtraEditors.ComboBoxEdit();
      this.m_grid_control = new DevExpress.XtraGrid.GridControl();
      this.m_grid_view = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colColumnIndex = new DevExpress.XtraGrid.Columns.GridColumn();
      this.colColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
      this.m_column_edit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
      this.m_file_edit = new DevExpress.XtraEditors.ButtonEdit();
      this.m_encoding_edit = new DevExpress.XtraEditors.ComboBoxEdit();
      this.m_separator_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_button_ok = new DevExpress.XtraEditors.SimpleButton();
      this.m_button_cancel = new DevExpress.XtraEditors.SimpleButton();
      this.m_label_file = new DevExpress.XtraEditors.LabelControl();
      this.m_label_encoding = new DevExpress.XtraEditors.LabelControl();
      this.m_label_separator = new DevExpress.XtraEditors.LabelControl();
      this.m_grid_extender = new AT.Toolbox.Extensions.GridControlExtender(this.components);
      this.m_delete_feature = new AT.Toolbox.Extensions.GridControlDeleteFeature(this.components);
      this.m_cmd_retrieve_default = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_control)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_column_edit)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_file_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_encoding_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_separator_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_extender)).BeginInit();
      this.SuspendLayout();
      // 
      // m_label_table
      // 
      this.m_label_table.Location = new System.Drawing.Point(12, 12);
      this.m_label_table.Name = "m_label_table";
      this.m_label_table.Size = new System.Drawing.Size(26, 13);
      this.m_label_table.TabIndex = 0;
      this.m_label_table.Text = "Table";
      // 
      // m_table_edit
      // 
      this.m_table_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_table_edit.Location = new System.Drawing.Point(153, 9);
      this.m_table_edit.Name = "m_table_edit";
      this.m_table_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_table_edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_table_edit.Size = new System.Drawing.Size(164, 20);
      this.m_table_edit.TabIndex = 1;
      this.m_table_edit.SelectedIndexChanged += new System.EventHandler(this.m_table_edit_SelectedIndexChanged);
      // 
      // m_grid_control
      // 
      this.m_grid_control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_grid_control.DataMember = "Items";
      this.m_grid_control.DataSource = this.m_csv_mapper;
      this.m_grid_control.EmbeddedNavigator.Name = "Zox";
      this.m_grid_control.Location = new System.Drawing.Point(12, 64);
      this.m_grid_control.MainView = this.m_grid_view;
      this.m_grid_control.Name = "m_grid_control";
      this.m_grid_control.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.m_column_edit});
      this.m_grid_control.Size = new System.Drawing.Size(305, 256);
      this.m_grid_control.TabIndex = 2;
      this.m_grid_control.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.m_grid_view});
      // 
      // m_grid_view
      // 
      this.m_grid_view.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colColumnIndex,
            this.colColumnName});
      this.m_grid_view.GridControl = this.m_grid_control;
      this.m_grid_view.Name = "m_grid_view";
      this.m_grid_view.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
      this.m_grid_view.OptionsView.ShowGroupPanel = false;
      // 
      // colColumnIndex
      // 
      this.colColumnIndex.Caption = "ColumnIndex";
      this.colColumnIndex.FieldName = "ColumnIndex";
      this.colColumnIndex.Name = "colColumnIndex";
      this.colColumnIndex.Visible = true;
      this.colColumnIndex.VisibleIndex = 0;
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
      this.m_column_edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      // 
      // m_file_edit
      // 
      this.m_file_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_file_edit.Location = new System.Drawing.Point(159, 326);
      this.m_file_edit.Name = "m_file_edit";
      this.m_file_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_file_edit.Size = new System.Drawing.Size(158, 20);
      this.m_file_edit.TabIndex = 3;
      this.m_file_edit.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_file_edit_ButtonPressed);
      // 
      // m_encoding_edit
      // 
      this.m_encoding_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_encoding_edit.Location = new System.Drawing.Point(159, 352);
      this.m_encoding_edit.Name = "m_encoding_edit";
      this.m_encoding_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_encoding_edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_encoding_edit.Size = new System.Drawing.Size(158, 20);
      this.m_encoding_edit.TabIndex = 4;
      // 
      // m_separator_edit
      // 
      this.m_separator_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_separator_edit.Location = new System.Drawing.Point(159, 378);
      this.m_separator_edit.Name = "m_separator_edit";
      this.m_separator_edit.Size = new System.Drawing.Size(158, 20);
      this.m_separator_edit.TabIndex = 5;
      this.m_separator_edit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_separator_edit_KeyUp);
      // 
      // m_button_ok
      // 
      this.m_button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Location = new System.Drawing.Point(153, 404);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 23);
      this.m_button_ok.TabIndex = 6;
      this.m_button_ok.Text = "OK";
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Location = new System.Drawing.Point(242, 404);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_button_cancel.TabIndex = 7;
      this.m_button_cancel.Text = "Cancel";
      // 
      // m_label_file
      // 
      this.m_label_file.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_label_file.Location = new System.Drawing.Point(12, 329);
      this.m_label_file.Name = "m_label_file";
      this.m_label_file.Size = new System.Drawing.Size(45, 13);
      this.m_label_file.TabIndex = 8;
      this.m_label_file.Text = "File name";
      // 
      // m_label_encoding
      // 
      this.m_label_encoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_label_encoding.Location = new System.Drawing.Point(12, 355);
      this.m_label_encoding.Name = "m_label_encoding";
      this.m_label_encoding.Size = new System.Drawing.Size(43, 13);
      this.m_label_encoding.TabIndex = 9;
      this.m_label_encoding.Text = "Encoding";
      // 
      // m_label_separator
      // 
      this.m_label_separator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.m_label_separator.Location = new System.Drawing.Point(12, 381);
      this.m_label_separator.Name = "m_label_separator";
      this.m_label_separator.Size = new System.Drawing.Size(48, 13);
      this.m_label_separator.TabIndex = 10;
      this.m_label_separator.Text = "Separator";
      // 
      // m_grid_extender
      // 
      this.m_grid_extender.Features.Add(this.m_delete_feature);
      this.m_grid_extender.Grid = this.m_grid_control;
      // 
      // m_cmd_retrieve_default
      // 
      this.m_cmd_retrieve_default.Location = new System.Drawing.Point(159, 35);
      this.m_cmd_retrieve_default.Name = "m_cmd_retrieve_default";
      this.m_cmd_retrieve_default.Size = new System.Drawing.Size(158, 23);
      this.m_cmd_retrieve_default.TabIndex = 11;
      this.m_cmd_retrieve_default.Text = "Retrieve default";
      this.m_cmd_retrieve_default.Click += new System.EventHandler(this.m_cmd_retrieve_default_Click);
      // 
      // CSVImportForm
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(329, 439);
      this.Controls.Add(this.m_cmd_retrieve_default);
      this.Controls.Add(this.m_label_separator);
      this.Controls.Add(this.m_label_encoding);
      this.Controls.Add(this.m_label_file);
      this.Controls.Add(this.m_button_cancel);
      this.Controls.Add(this.m_button_ok);
      this.Controls.Add(this.m_separator_edit);
      this.Controls.Add(this.m_encoding_edit);
      this.Controls.Add(this.m_file_edit);
      this.Controls.Add(this.m_grid_control);
      this.Controls.Add(this.m_table_edit);
      this.Controls.Add(this.m_label_table);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "CSVImportForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "CSV Import Form";
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_control)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_view)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_column_edit)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_file_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_encoding_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_separator_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid_extender)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DevExpress.XtraEditors.LabelControl m_label_table;
    private CSVMapper m_csv_mapper;
    private DevExpress.XtraEditors.ComboBoxEdit m_table_edit;
    private DevExpress.XtraGrid.GridControl m_grid_control;
    private DevExpress.XtraGrid.Views.Grid.GridView m_grid_view;
    private DevExpress.XtraEditors.ButtonEdit m_file_edit;
    private DevExpress.XtraGrid.Columns.GridColumn colColumnIndex;
    private DevExpress.XtraGrid.Columns.GridColumn colColumnName;
    private DevExpress.XtraEditors.ComboBoxEdit m_encoding_edit;
    private DevExpress.XtraEditors.TextEdit m_separator_edit;
    private DevExpress.XtraEditors.SimpleButton m_button_ok;
    private DevExpress.XtraEditors.SimpleButton m_button_cancel;
    private DevExpress.XtraEditors.LabelControl m_label_file;
    private DevExpress.XtraEditors.LabelControl m_label_encoding;
    private DevExpress.XtraEditors.LabelControl m_label_separator;
    private DevExpress.XtraEditors.Repository.RepositoryItemComboBox m_column_edit;
    private AT.Toolbox.Extensions.GridControlExtender m_grid_extender;
    private AT.Toolbox.Extensions.GridControlDeleteFeature m_delete_feature;
    private DevExpress.XtraEditors.SimpleButton m_cmd_retrieve_default;
  }
}