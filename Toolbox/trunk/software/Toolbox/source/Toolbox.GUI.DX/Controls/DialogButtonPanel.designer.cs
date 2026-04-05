namespace Toolbox.GUI.DX.Controls
{
  partial class DialogButtonPanel
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support — do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_layout_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_no_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_yes_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ignore_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_retry_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_abort_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_layout_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_layout_panel
      // 
      this.m_layout_panel.AutoSize = true;
      this.m_layout_panel.Controls.Add(this.m_cancel_btn);
      this.m_layout_panel.Controls.Add(this.m_ok_btn);
      this.m_layout_panel.Controls.Add(this.m_no_btn);
      this.m_layout_panel.Controls.Add(this.m_yes_btn);
      this.m_layout_panel.Controls.Add(this.m_ignore_btn);
      this.m_layout_panel.Controls.Add(this.m_retry_btn);
      this.m_layout_panel.Controls.Add(this.m_abort_btn);
      this.m_layout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_layout_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_layout_panel.Location = new System.Drawing.Point(0, 0);
      this.m_layout_panel.Name = "m_layout_panel";
      this.m_layout_panel.Size = new System.Drawing.Size(567, 30);
      this.m_layout_panel.TabIndex = 1;
      this.m_layout_panel.WrapContents = false;
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(489, 3);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 0;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(408, 3);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 1;
      this.m_ok_btn.Text = "OK";
      // 
      // m_no_btn
      // 
      this.m_no_btn.DialogResult = System.Windows.Forms.DialogResult.No;
      this.m_no_btn.Location = new System.Drawing.Point(327, 3);
      this.m_no_btn.Name = "m_no_btn";
      this.m_no_btn.Size = new System.Drawing.Size(75, 23);
      this.m_no_btn.TabIndex = 2;
      this.m_no_btn.Text = "No";
      // 
      // m_yes_btn
      // 
      this.m_yes_btn.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.m_yes_btn.Location = new System.Drawing.Point(246, 3);
      this.m_yes_btn.Name = "m_yes_btn";
      this.m_yes_btn.Size = new System.Drawing.Size(75, 23);
      this.m_yes_btn.TabIndex = 3;
      this.m_yes_btn.Text = "Yes";
      // 
      // m_ignore_btn
      // 
      this.m_ignore_btn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
      this.m_ignore_btn.Location = new System.Drawing.Point(165, 3);
      this.m_ignore_btn.Name = "m_ignore_btn";
      this.m_ignore_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ignore_btn.TabIndex = 4;
      this.m_ignore_btn.Text = "Ignore";
      // 
      // m_retry_btn
      // 
      this.m_retry_btn.DialogResult = System.Windows.Forms.DialogResult.Retry;
      this.m_retry_btn.Location = new System.Drawing.Point(84, 3);
      this.m_retry_btn.Name = "m_retry_btn";
      this.m_retry_btn.Size = new System.Drawing.Size(75, 23);
      this.m_retry_btn.TabIndex = 5;
      this.m_retry_btn.Text = "Retry";
      // 
      // m_abort_btn
      // 
      this.m_abort_btn.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.m_abort_btn.Location = new System.Drawing.Point(3, 3);
      this.m_abort_btn.Name = "m_abort_btn";
      this.m_abort_btn.Size = new System.Drawing.Size(75, 23);
      this.m_abort_btn.TabIndex = 6;
      this.m_abort_btn.Text = "Abort";
      // 
      // DialogButtonPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.m_layout_panel);
      this.Name = "DialogButtonPanel";
      this.Size = new System.Drawing.Size(567, 30);
      this.m_layout_panel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_layout_panel;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_no_btn;
    private DevExpress.XtraEditors.SimpleButton m_yes_btn;
    private DevExpress.XtraEditors.SimpleButton m_ignore_btn;
    private DevExpress.XtraEditors.SimpleButton m_retry_btn;
    private DevExpress.XtraEditors.SimpleButton m_abort_btn;
  }
}
