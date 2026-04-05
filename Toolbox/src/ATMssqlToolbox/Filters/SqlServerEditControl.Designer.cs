namespace AT.Toolbox.MSSQL
{
  partial class SqlServerEditControl
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
      this.m_server_edit = new DevExpress.XtraEditors.ComboBoxEdit();
      this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
      this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
      this.m_pass_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_username_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_auth_edit = new DevExpress.XtraEditors.ComboBoxEdit();
      this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
      this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
      this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
      this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
      this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
      this.m_networklibrary_edit = new DevExpress.XtraEditors.LookUpEdit();
      this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
      this.objectTextStoreBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_server_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_pass_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_username_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_auth_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_networklibrary_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectTextStoreBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // m_server_edit
      // 
      this.m_server_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_server_edit.Location = new System.Drawing.Point(109, 24);
      this.m_server_edit.Name = "m_server_edit";
      this.m_server_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_server_edit.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.comboBoxEdit1_Properties_ButtonPressed);
      this.m_server_edit.Size = new System.Drawing.Size(195, 20);
      this.m_server_edit.TabIndex = 10;
      this.m_server_edit.EditValueChanged += new System.EventHandler(this.m_server_edit_EditValueChanged);
      this.m_server_edit.Validated += new System.EventHandler(this.load_connection_types);
      // 
      // checkEdit1
      // 
      this.checkEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.checkEdit1.Location = new System.Drawing.Point(107, 50);
      this.checkEdit1.Name = "checkEdit1";
      this.checkEdit1.Properties.Caption = "checkEdit1";
      this.checkEdit1.Size = new System.Drawing.Size(197, 19);
      this.checkEdit1.TabIndex = 4;
      this.checkEdit1.CheckedChanged += new System.EventHandler(this.checkEdit1_CheckedChanged);
      // 
      // labelControl1
      // 
      this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl1.Location = new System.Drawing.Point(14, 24);
      this.labelControl1.Name = "labelControl1";
      this.labelControl1.Size = new System.Drawing.Size(89, 20);
      this.labelControl1.TabIndex = 2;
      this.labelControl1.Text = "labelControl1";
      // 
      // m_pass_edit
      // 
      this.m_pass_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_pass_edit.Location = new System.Drawing.Point(109, 155);
      this.m_pass_edit.Name = "m_pass_edit";
      this.m_pass_edit.Properties.PasswordChar = '*';
      this.m_pass_edit.Size = new System.Drawing.Size(194, 20);
      this.m_pass_edit.TabIndex = 9;
      this.m_pass_edit.Validated += new System.EventHandler(this.m_pass_edit_Validated);
      // 
      // m_username_edit
      // 
      this.m_username_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_username_edit.Location = new System.Drawing.Point(109, 129);
      this.m_username_edit.Name = "m_username_edit";
      this.m_username_edit.Size = new System.Drawing.Size(195, 20);
      this.m_username_edit.TabIndex = 8;
      this.m_username_edit.Validated += new System.EventHandler(this.m_username_edit_Validated);
      // 
      // m_auth_edit
      // 
      this.m_auth_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_auth_edit.Location = new System.Drawing.Point(109, 101);
      this.m_auth_edit.Name = "m_auth_edit";
      this.m_auth_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_auth_edit.Properties.PopupBorderStyle = DevExpress.XtraEditors.Controls.PopupBorderStyles.Style3D;
      this.m_auth_edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_auth_edit.Size = new System.Drawing.Size(195, 20);
      this.m_auth_edit.TabIndex = 7;
      this.m_auth_edit.EditValueChanged += new System.EventHandler(this.m_auth_edit_EditValueChanged);
      this.m_auth_edit.SelectedIndexChanged += new System.EventHandler(this.m_auth_edit_SelectedIndexChanged);
      // 
      // labelControl5
      // 
      this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl5.Location = new System.Drawing.Point(14, 155);
      this.labelControl5.Name = "labelControl5";
      this.labelControl5.Size = new System.Drawing.Size(89, 20);
      this.labelControl5.TabIndex = 6;
      this.labelControl5.Text = "labelControl5";
      // 
      // labelControl3
      // 
      this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl3.Location = new System.Drawing.Point(14, 101);
      this.labelControl3.Name = "labelControl3";
      this.labelControl3.Size = new System.Drawing.Size(89, 20);
      this.labelControl3.TabIndex = 5;
      this.labelControl3.Text = "labelControl3";
      // 
      // labelControl4
      // 
      this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl4.Location = new System.Drawing.Point(14, 129);
      this.labelControl4.Name = "labelControl4";
      this.labelControl4.Size = new System.Drawing.Size(89, 20);
      this.labelControl4.TabIndex = 4;
      this.labelControl4.Text = "labelControl4";
      // 
      // labelControl6
      // 
      this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.labelControl6.Appearance.Options.UseFont = true;
      this.labelControl6.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl6.Location = new System.Drawing.Point(3, 75);
      this.labelControl6.Name = "labelControl6";
      this.labelControl6.Size = new System.Drawing.Size(164, 20);
      this.labelControl6.TabIndex = 11;
      this.labelControl6.Text = "labelControl6";
      // 
      // labelControl7
      // 
      this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.labelControl7.Appearance.Options.UseFont = true;
      this.labelControl7.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl7.Location = new System.Drawing.Point(3, 1);
      this.labelControl7.Name = "labelControl7";
      this.labelControl7.Size = new System.Drawing.Size(164, 20);
      this.labelControl7.TabIndex = 12;
      this.labelControl7.Text = "labelControl7";
      // 
      // m_networklibrary_edit
      // 
      this.m_networklibrary_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_networklibrary_edit.Location = new System.Drawing.Point(109, 181);
      this.m_networklibrary_edit.Name = "m_networklibrary_edit";
      this.m_networklibrary_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
      this.m_networklibrary_edit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", "Тип подключения")});
      this.m_networklibrary_edit.Properties.DataSource = this.objectTextStoreBindingSource;
      this.m_networklibrary_edit.Properties.DisplayMember = "Text";
      this.m_networklibrary_edit.Properties.NullText = "<по умолчанию>";
      this.m_networklibrary_edit.Properties.ValueMember = "Value";
      this.m_networklibrary_edit.Size = new System.Drawing.Size(194, 20);
      this.m_networklibrary_edit.TabIndex = 13;
      this.m_networklibrary_edit.EditValueChanged += new System.EventHandler(this.m_networklibrary_edit_EditValueChanged);
      // 
      // labelControl2
      // 
      this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl2.Location = new System.Drawing.Point(14, 181);
      this.labelControl2.Name = "labelControl2";
      this.labelControl2.Size = new System.Drawing.Size(97, 20);
      this.labelControl2.TabIndex = 14;
      this.labelControl2.Text = "Тип подключения";
      // 
      // objectTextStoreBindingSource
      // 
      this.objectTextStoreBindingSource.DataSource = typeof(AT.Toolbox.MSSQL.ObjectTextStore);
      // 
      // SqlServerEditControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_networklibrary_edit);
      this.Controls.Add(this.labelControl2);
      this.Controls.Add(this.labelControl7);
      this.Controls.Add(this.labelControl6);
      this.Controls.Add(this.m_pass_edit);
      this.Controls.Add(this.m_server_edit);
      this.Controls.Add(this.m_username_edit);
      this.Controls.Add(this.checkEdit1);
      this.Controls.Add(this.labelControl1);
      this.Controls.Add(this.m_auth_edit);
      this.Controls.Add(this.labelControl5);
      this.Controls.Add(this.labelControl4);
      this.Controls.Add(this.labelControl3);
      this.MinimumSize = new System.Drawing.Size(300, 180);
      this.Name = "SqlServerEditControl";
      this.Size = new System.Drawing.Size(306, 208);
      this.Load += new System.EventHandler(this.load_connection_types);
      ((System.ComponentModel.ISupportInitialize)(this.m_server_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_pass_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_username_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_auth_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_networklibrary_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectTextStoreBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.CheckEdit checkEdit1;
    private DevExpress.XtraEditors.LabelControl labelControl1;
    private DevExpress.XtraEditors.TextEdit m_pass_edit;
    private DevExpress.XtraEditors.TextEdit m_username_edit;
    private DevExpress.XtraEditors.ComboBoxEdit m_auth_edit;
    private DevExpress.XtraEditors.LabelControl labelControl5;
    private DevExpress.XtraEditors.LabelControl labelControl3;
    private DevExpress.XtraEditors.LabelControl labelControl4;
    private DevExpress.XtraEditors.ComboBoxEdit m_server_edit;
    private DevExpress.XtraEditors.LabelControl labelControl6;
    private DevExpress.XtraEditors.LabelControl labelControl7;
    private DevExpress.XtraEditors.LookUpEdit m_networklibrary_edit;
    private DevExpress.XtraEditors.LabelControl labelControl2;
    private System.Windows.Forms.BindingSource objectTextStoreBindingSource;

  }
}
