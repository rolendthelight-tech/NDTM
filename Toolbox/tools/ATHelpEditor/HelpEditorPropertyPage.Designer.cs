namespace ATHelpEditor
{
  partial class HelpEditorPropertyPage
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
      this.m_edit_template_folder = new DevExpress.XtraEditors.ButtonEdit();
      this.m_label_template_folder = new DevExpress.XtraEditors.LabelControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_edit_template_folder.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_edit_template_folder
      // 
      this.m_edit_template_folder.Location = new System.Drawing.Point(53, 36);
      this.m_edit_template_folder.Name = "m_edit_template_folder";
      this.m_edit_template_folder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_edit_template_folder.Size = new System.Drawing.Size(403, 20);
      this.m_edit_template_folder.TabIndex = 0;
      this.m_edit_template_folder.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_edit_template_folder_ButtonPressed);
      // 
      // m_label_template_folder
      // 
      this.m_label_template_folder.Location = new System.Drawing.Point(53, 17);
      this.m_label_template_folder.Name = "m_label_template_folder";
      this.m_label_template_folder.Size = new System.Drawing.Size(63, 13);
      this.m_label_template_folder.TabIndex = 1;
      this.m_label_template_folder.Text = "labelControl1";
      // 
      // HelpEditorPropertyPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_label_template_folder);
      this.Controls.Add(this.m_edit_template_folder);
      this.Name = "HelpEditorPropertyPage";
      this.Size = new System.Drawing.Size(563, 233);
      ((System.ComponentModel.ISupportInitialize)(this.m_edit_template_folder.Properties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DevExpress.XtraEditors.ButtonEdit m_edit_template_folder;
    private DevExpress.XtraEditors.LabelControl m_label_template_folder;
  }
}
