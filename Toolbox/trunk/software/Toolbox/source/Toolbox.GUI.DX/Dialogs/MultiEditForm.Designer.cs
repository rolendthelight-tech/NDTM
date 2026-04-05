namespace Toolbox.GUI.DX.Dialogs
{
  partial class MultiEditForm
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
      this.m_vgrid = new DevExpress.XtraVerticalGrid.VGridControl();
      this.m_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cancel_cmd = new System.Windows.Forms.ToolStripMenuItem();
      this.m_apply_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.m_vgrid)).BeginInit();
      this.m_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_vgrid
      // 
      this.m_vgrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_vgrid.ContextMenuStrip = this.m_menu;
      this.m_vgrid.Location = new System.Drawing.Point(13, 12);
      this.m_vgrid.Name = "m_vgrid";
      this.m_vgrid.OptionsView.AutoScaleBands = true;
      this.m_vgrid.OptionsView.FixRowHeaderPanelWidth = false;
      this.m_vgrid.OptionsView.ShowButtons = false;
      this.m_vgrid.OptionsView.ShowRootCategories = false;
      this.m_vgrid.RowHeaderWidth = 150;
      this.m_vgrid.Size = new System.Drawing.Size(574, 321);
      this.m_vgrid.TabIndex = 0;
      this.m_vgrid.RowChanged += new DevExpress.XtraVerticalGrid.Events.RowChangedEventHandler(this.HandleEditChanged);
      // 
      // m_menu
      // 
      this.m_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cancel_cmd});
      this.m_menu.Name = "contextMenuStrip1";
      this.m_menu.Size = new System.Drawing.Size(139, 26);
      // 
      // m_cancel_cmd
      // 
      this.m_cancel_cmd.Name = "m_cancel_cmd";
      this.m_cancel_cmd.Size = new System.Drawing.Size(138, 22);
      this.m_cancel_cmd.Text = "Cancel Edit";
      this.m_cancel_cmd.Click += new System.EventHandler(this.HandleCancelEdit);
      // 
      // m_apply_btn
      // 
      this.m_apply_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_apply_btn.Location = new System.Drawing.Point(500, 343);
      this.m_apply_btn.Name = "m_apply_btn";
      this.m_apply_btn.Size = new System.Drawing.Size(75, 23);
      this.m_apply_btn.TabIndex = 1;
      this.m_apply_btn.Text = "Apply";
      this.m_apply_btn.Click += new System.EventHandler(this.HandleApply);
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ok_btn.Location = new System.Drawing.Point(319, 343);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 2;
      this.m_ok_btn.Text = "OK";
      this.m_ok_btn.Click += new System.EventHandler(this.HandleOK);
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cancel_btn.Location = new System.Drawing.Point(401, 343);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 3;
      this.m_cancel_btn.Text = "Cancel";
      this.m_cancel_btn.Click += new System.EventHandler(this.HandleCancel);
      // 
      // MultiEditForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(599, 374);
      this.Controls.Add(this.m_vgrid);
      this.Controls.Add(this.m_cancel_btn);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_apply_btn);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "MultiEditForm";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "MultiEditForm";
      this.Shown += new System.EventHandler(this.HandleShown);
      ((System.ComponentModel.ISupportInitialize)(this.m_vgrid)).EndInit();
      this.m_menu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraVerticalGrid.VGridControl m_vgrid;
    private DevExpress.XtraEditors.SimpleButton m_apply_btn;
    private System.Windows.Forms.ContextMenuStrip m_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cancel_cmd;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
  }
}