namespace Toolbox.GUI.DX.Dialogs
{
  partial class ChoiceForm
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
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(68, 62);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 7;
      this.m_ok_btn.Text = "OK";
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(148, 62);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 6;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_picture_box
      // 
      this.m_picture_box.Location = new System.Drawing.Point(12, 6);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.Size = new System.Drawing.Size(50, 50);
      this.m_picture_box.TabIndex = 4;
      this.m_picture_box.TabStop = false;
      // 
      // panelControl1
      // 
      this.panelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.panelControl1.Location = new System.Drawing.Point(69, 6);
      this.panelControl1.Name = "panelControl1";
      this.panelControl1.Size = new System.Drawing.Size(172, 50);
      this.panelControl1.TabIndex = 8;
      // 
      // ChoiceForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(253, 91);
      this.Controls.Add(this.panelControl1);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_cancel_btn);
      this.Controls.Add(this.m_picture_box);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.MinimumSize = new System.Drawing.Size(259, 113);
      this.Name = "ChoiceForm";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ChoiceForm";
      this.Load += new System.EventHandler(this.HandleLoad);
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private System.Windows.Forms.PictureBox m_picture_box;
    private DevExpress.XtraEditors.PanelControl panelControl1;

  }
}