namespace AT.Toolbox.MSSQL
{
  partial class BuildMouldForm
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
      this.m_button_ok = new DevExpress.XtraEditors.SimpleButton();
      this.m_button_cancel = new DevExpress.XtraEditors.SimpleButton();
      this.m_old_mould_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_new_version_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_label_old_mould = new DevExpress.XtraEditors.LabelControl();
      this.m_label_new_version = new DevExpress.XtraEditors.LabelControl();
      this.m_new_mould_edit = new DevExpress.XtraEditors.ButtonEdit();
      this.m_label_new_mould = new DevExpress.XtraEditors.LabelControl();
      this.m_old_version_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_label_old_version = new DevExpress.XtraEditors.LabelControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_old_mould_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_new_version_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_new_mould_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_old_version_edit.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_button_ok
      // 
      this.m_button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Location = new System.Drawing.Point(121, 124);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 23);
      this.m_button_ok.TabIndex = 0;
      this.m_button_ok.Text = "OK";
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Location = new System.Drawing.Point(202, 124);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_button_cancel.TabIndex = 1;
      this.m_button_cancel.Text = "Cancel";
      // 
      // m_old_mould_edit
      // 
      this.m_old_mould_edit.Location = new System.Drawing.Point(121, 12);
      this.m_old_mould_edit.Name = "m_old_mould_edit";
      this.m_old_mould_edit.Properties.ReadOnly = true;
      this.m_old_mould_edit.Size = new System.Drawing.Size(156, 20);
      this.m_old_mould_edit.TabIndex = 2;
      // 
      // m_new_version_edit
      // 
      this.m_new_version_edit.Location = new System.Drawing.Point(121, 90);
      this.m_new_version_edit.Name = "m_new_version_edit";
      this.m_new_version_edit.Size = new System.Drawing.Size(100, 20);
      this.m_new_version_edit.TabIndex = 3;
      this.m_new_version_edit.Validating += new System.ComponentModel.CancelEventHandler(this.m_new_version_edit_Validating);
      // 
      // m_label_old_mould
      // 
      this.m_label_old_mould.Location = new System.Drawing.Point(12, 15);
      this.m_label_old_mould.Name = "m_label_old_mould";
      this.m_label_old_mould.Size = new System.Drawing.Size(47, 13);
      this.m_label_old_mould.TabIndex = 4;
      this.m_label_old_mould.Text = "Old mould";
      // 
      // m_label_new_version
      // 
      this.m_label_new_version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_label_new_version.Location = new System.Drawing.Point(12, 93);
      this.m_label_new_version.Name = "m_label_new_version";
      this.m_label_new_version.Size = new System.Drawing.Size(59, 13);
      this.m_label_new_version.TabIndex = 5;
      this.m_label_new_version.Text = "New version";
      // 
      // m_new_mould_edit
      // 
      this.m_new_mould_edit.Location = new System.Drawing.Point(121, 64);
      this.m_new_mould_edit.Name = "m_new_mould_edit";
      this.m_new_mould_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_new_mould_edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_new_mould_edit.Size = new System.Drawing.Size(156, 20);
      this.m_new_mould_edit.TabIndex = 6;
      this.m_new_mould_edit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_path_edit_ButtonClick);
      // 
      // m_label_new_mould
      // 
      this.m_label_new_mould.Location = new System.Drawing.Point(12, 67);
      this.m_label_new_mould.Name = "m_label_new_mould";
      this.m_label_new_mould.Size = new System.Drawing.Size(52, 13);
      this.m_label_new_mould.TabIndex = 7;
      this.m_label_new_mould.Text = "New mould";
      // 
      // m_old_version_edit
      // 
      this.m_old_version_edit.Location = new System.Drawing.Point(121, 38);
      this.m_old_version_edit.Name = "m_old_version_edit";
      this.m_old_version_edit.Properties.ReadOnly = true;
      this.m_old_version_edit.Size = new System.Drawing.Size(100, 20);
      this.m_old_version_edit.TabIndex = 8;
      // 
      // m_label_old_version
      // 
      this.m_label_old_version.Location = new System.Drawing.Point(12, 41);
      this.m_label_old_version.Name = "m_label_old_version";
      this.m_label_old_version.Size = new System.Drawing.Size(54, 13);
      this.m_label_old_version.TabIndex = 9;
      this.m_label_old_version.Text = "Old version";
      // 
      // BuildMouldForm
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(289, 159);
      this.Controls.Add(this.m_label_old_version);
      this.Controls.Add(this.m_old_version_edit);
      this.Controls.Add(this.m_label_new_mould);
      this.Controls.Add(this.m_new_mould_edit);
      this.Controls.Add(this.m_label_new_version);
      this.Controls.Add(this.m_label_old_mould);
      this.Controls.Add(this.m_new_version_edit);
      this.Controls.Add(this.m_old_mould_edit);
      this.Controls.Add(this.m_button_cancel);
      this.Controls.Add(this.m_button_ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "BuildMouldForm";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "BuildMouldForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_old_mould_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_new_version_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_new_mould_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_old_version_edit.Properties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DevExpress.XtraEditors.SimpleButton m_button_ok;
    private DevExpress.XtraEditors.SimpleButton m_button_cancel;
    private DevExpress.XtraEditors.TextEdit m_old_mould_edit;
    private DevExpress.XtraEditors.TextEdit m_new_version_edit;
    private DevExpress.XtraEditors.LabelControl m_label_old_mould;
    private DevExpress.XtraEditors.LabelControl m_label_new_version;
    private DevExpress.XtraEditors.ButtonEdit m_new_mould_edit;
    private DevExpress.XtraEditors.LabelControl m_label_new_mould;
    private DevExpress.XtraEditors.TextEdit m_old_version_edit;
    private DevExpress.XtraEditors.LabelControl m_label_old_version;
  }
}