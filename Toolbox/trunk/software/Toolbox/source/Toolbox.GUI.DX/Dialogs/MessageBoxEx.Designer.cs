using Toolbox.GUI.DX.Controls;

namespace Toolbox.GUI.DX.Dialogs
{
  partial class MessageBoxEx
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
      this.components = new System.ComponentModel.Container();
      this.m_group = new DevExpress.XtraEditors.GroupControl();
      this.m_message = new DevExpress.XtraEditors.LabelControl();
      this.m_picture = new System.Windows.Forms.PictureBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_no_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_yes_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ignore_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_retry_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_abort_btn = new DevExpress.XtraEditors.SimpleButton();
      this.groupControlDragger1 = new GroupControlDragger(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_group)).BeginInit();
      this.m_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).BeginInit();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_group
      // 
      this.m_group.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.m_group.AppearanceCaption.Options.UseFont = true;
      this.m_group.Controls.Add(this.m_message);
      this.m_group.Controls.Add(this.m_picture);
      this.m_group.Controls.Add(this.flowLayoutPanel1);
      this.m_group.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_group.Location = new System.Drawing.Point(0, 0);
      this.m_group.Name = "m_group";
      this.m_group.Size = new System.Drawing.Size(574, 109);
      this.m_group.TabIndex = 0;
      this.m_group.Text = "groupControl1";
      // 
      // m_message
      // 
      this.m_message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_message.Appearance.Options.UseTextOptions = true;
      this.m_message.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
      this.m_message.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
      this.m_message.AutoEllipsis = true;
      this.m_message.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_message.Cursor = System.Windows.Forms.Cursors.Default;
      this.m_message.Location = new System.Drawing.Point(62, 24);
      this.m_message.Name = "m_message";
      this.m_message.Size = new System.Drawing.Size(508, 51);
      this.m_message.TabIndex = 2;
      // 
      // m_picture
      // 
      this.m_picture.Location = new System.Drawing.Point(6, 24);
      this.m_picture.Name = "m_picture";
      this.m_picture.Size = new System.Drawing.Size(50, 50);
      this.m_picture.TabIndex = 1;
      this.m_picture.TabStop = false;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.m_cancel_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_ok_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_no_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_yes_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_ignore_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_retry_btn);
      this.flowLayoutPanel1.Controls.Add(this.m_abort_btn);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 78);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(570, 29);
      this.flowLayoutPanel1.TabIndex = 0;
      this.flowLayoutPanel1.WrapContents = false;
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(492, 3);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 0;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(411, 3);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 1;
      this.m_ok_btn.Text = "OK";
      // 
      // m_no_btn
      // 
      this.m_no_btn.DialogResult = System.Windows.Forms.DialogResult.No;
      this.m_no_btn.Location = new System.Drawing.Point(330, 3);
      this.m_no_btn.Name = "m_no_btn";
      this.m_no_btn.Size = new System.Drawing.Size(75, 23);
      this.m_no_btn.TabIndex = 2;
      this.m_no_btn.Text = "No";
      // 
      // m_yes_btn
      // 
      this.m_yes_btn.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.m_yes_btn.Location = new System.Drawing.Point(249, 3);
      this.m_yes_btn.Name = "m_yes_btn";
      this.m_yes_btn.Size = new System.Drawing.Size(75, 23);
      this.m_yes_btn.TabIndex = 3;
      this.m_yes_btn.Text = "Yes";
      // 
      // m_ignore_btn
      // 
      this.m_ignore_btn.DialogResult = System.Windows.Forms.DialogResult.Ignore;
      this.m_ignore_btn.Location = new System.Drawing.Point(168, 3);
      this.m_ignore_btn.Name = "m_ignore_btn";
      this.m_ignore_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ignore_btn.TabIndex = 4;
      this.m_ignore_btn.Text = "Ignore";
      // 
      // m_retry_btn
      // 
      this.m_retry_btn.DialogResult = System.Windows.Forms.DialogResult.Retry;
      this.m_retry_btn.Location = new System.Drawing.Point(87, 3);
      this.m_retry_btn.Name = "m_retry_btn";
      this.m_retry_btn.Size = new System.Drawing.Size(75, 23);
      this.m_retry_btn.TabIndex = 5;
      this.m_retry_btn.Text = "Retry";
      // 
      // m_abort_btn
      // 
      this.m_abort_btn.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.m_abort_btn.Location = new System.Drawing.Point(6, 3);
      this.m_abort_btn.Name = "m_abort_btn";
      this.m_abort_btn.Size = new System.Drawing.Size(75, 23);
      this.m_abort_btn.TabIndex = 6;
      this.m_abort_btn.Text = "Abort";
      // 
      // groupControlDragger1
      // 
      this.groupControlDragger1.CanMove = true;
      this.groupControlDragger1.CanSize = false;
      this.groupControlDragger1.GroupControlToDragBy = this.m_group;
      this.groupControlDragger1.SizeBounds = 3;
      // 
      // MessageBoxEx
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(574, 109);
      this.Controls.Add(this.m_group);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MinimumSize = new System.Drawing.Size(574, 109);
      this.Name = "MessageBoxEx";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "MessageBoxEx";
      this.TopMost = false;
      this.Shown += new System.EventHandler(this.HandleShown);
      ((System.ComponentModel.ISupportInitialize)(this.m_group)).EndInit();
      this.m_group.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).EndInit();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.GroupControl m_group;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_no_btn;
    private DevExpress.XtraEditors.SimpleButton m_yes_btn;
    private DevExpress.XtraEditors.SimpleButton m_ignore_btn;
    private DevExpress.XtraEditors.SimpleButton m_retry_btn;
    private DevExpress.XtraEditors.SimpleButton m_abort_btn;
    private System.Windows.Forms.PictureBox m_picture;
    private DevExpress.XtraEditors.LabelControl m_message;
    private GroupControlDragger groupControlDragger1;
  }
}