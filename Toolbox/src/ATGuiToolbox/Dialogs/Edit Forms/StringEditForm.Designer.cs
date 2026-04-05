namespace AT.Toolbox.Dialogs.Edit_Forms
{
  partial class StringEditForm
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
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_text_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.m_text_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      this.SuspendLayout();
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(296, 29);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 7;
      this.m_ok_btn.Text = "OK";
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(376, 29);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 6;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_text_edit
      // 
      this.m_text_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_text_edit.Location = new System.Drawing.Point(58, 4);
      this.m_text_edit.Name = "m_text_edit";
      this.m_text_edit.Size = new System.Drawing.Size(419, 20);
      this.m_text_edit.TabIndex = 8;
      // 
      // m_picture_box
      // 
      this.m_picture_box.Location = new System.Drawing.Point(2, 2);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.Size = new System.Drawing.Size(50, 50);
      this.m_picture_box.TabIndex = 4;
      this.m_picture_box.TabStop = false;
      // 
      // StringEditForm
      // 
      this.AcceptButton = this.m_ok_btn;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_cancel_btn;
      this.ClientSize = new System.Drawing.Size(481, 57);
      this.Controls.Add(this.m_text_edit);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_cancel_btn);
      this.Controls.Add(this.m_picture_box);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "StringEditForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "StringEditForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_text_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private System.Windows.Forms.PictureBox m_picture_box;
    private DevExpress.XtraEditors.TextEdit m_text_edit;
  }
}