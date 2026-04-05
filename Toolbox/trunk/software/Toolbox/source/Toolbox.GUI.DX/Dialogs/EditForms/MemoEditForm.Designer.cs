namespace Toolbox.GUI.DX.Dialogs.EditForms
{
  partial class MemoEditForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
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
      this.m_text_edit = new DevExpress.XtraEditors.MemoEdit();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.m_text_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      this.SuspendLayout();
      // 
      // m_text_edit
      // 
      this.m_text_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_text_edit.Location = new System.Drawing.Point(56, 4);
      this.m_text_edit.Name = "m_text_edit";
      this.m_text_edit.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.m_text_edit.Size = new System.Drawing.Size(561, 432);
      this.m_text_edit.TabIndex = 1;
      // 
      // m_picture_box
      // 
      this.m_picture_box.Location = new System.Drawing.Point(4, 4);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.Size = new System.Drawing.Size(50, 50);
      this.m_picture_box.TabIndex = 0;
      this.m_picture_box.TabStop = false;
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(533, 442);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 3;
      this.m_ok_btn.Text = "OK";
      // 
      // MemoEditForm
      // 
      this.AcceptButton = this.m_ok_btn;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(620, 469);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_text_edit);
      this.Controls.Add(this.m_picture_box);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "MemoEditForm";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.Text = "MemoEditForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_text_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox m_picture_box;
    private DevExpress.XtraEditors.MemoEdit m_text_edit;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
  }
}