namespace AT.Toolbox.Dialogs
{
  partial class InfoListForm
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
        this.m_lister = new AT.Toolbox.Controls.InfoTreeView();
        this.m_picture = new System.Windows.Forms.PictureBox();
        this.m_group_summary = new DevExpress.XtraEditors.GroupControl();
        this.m_close = new DevExpress.XtraEditors.ButtonEdit();
        this.m_label_summary = new DevExpress.XtraEditors.LabelControl();
        this.m_panel_dragger = new AT.Toolbox.Dialogs.GroupControlDragger(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.m_picture)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.m_group_summary)).BeginInit();
        this.m_group_summary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.m_close.Properties)).BeginInit();
        this.SuspendLayout();
        // 
        // m_buttons
        // 
        this.m_buttons.Buttons = System.Windows.Forms.MessageBoxButtons.OK;
        this.m_buttons.Size = new System.Drawing.Size(462, 29);
        // 
        // m_lister
        // 
        this.m_lister.Dock = System.Windows.Forms.DockStyle.Fill;
        this.m_lister.Location = new System.Drawing.Point(0, 76);
        this.m_lister.Name = "m_lister";
        this.m_lister.Size = new System.Drawing.Size(476, 220);
        this.m_lister.TabIndex = 1;
        // 
        // m_picture
        // 
        this.m_picture.Dock = System.Windows.Forms.DockStyle.Left;
        this.m_picture.Location = new System.Drawing.Point(7, 27);
        this.m_picture.Name = "m_picture";
        this.m_picture.Size = new System.Drawing.Size(48, 42);
        this.m_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.m_picture.TabIndex = 0;
        this.m_picture.TabStop = false;
        // 
        // m_group_summary
        // 
        this.m_group_summary.Controls.Add(this.m_close);
        this.m_group_summary.Controls.Add(this.m_label_summary);
        this.m_group_summary.Controls.Add(this.m_picture);
        this.m_group_summary.Dock = System.Windows.Forms.DockStyle.Top;
        this.m_group_summary.Location = new System.Drawing.Point(0, 0);
        this.m_group_summary.Name = "m_group_summary";
        this.m_group_summary.Padding = new System.Windows.Forms.Padding(5);
        this.m_group_summary.Size = new System.Drawing.Size(476, 76);
        this.m_group_summary.TabIndex = 3;
        // 
        // m_close
        // 
        this.m_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.m_close.Location = new System.Drawing.Point(458, 1);
        this.m_close.Name = "m_close";
        this.m_close.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.m_close.Properties.Appearance.Options.UseBackColor = true;
        this.m_close.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.m_close.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
        this.m_close.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
        this.m_close.Size = new System.Drawing.Size(18, 19);
        this.m_close.TabIndex = 2;
        this.m_close.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_close_ButtonClick);
        // 
        // m_label_summary
        // 
        this.m_label_summary.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.m_label_summary.Dock = System.Windows.Forms.DockStyle.Fill;
        this.m_label_summary.Location = new System.Drawing.Point(55, 27);
        this.m_label_summary.Name = "m_label_summary";
        this.m_label_summary.Padding = new System.Windows.Forms.Padding(10);
        this.m_label_summary.Size = new System.Drawing.Size(414, 33);
        this.m_label_summary.TabIndex = 1;
        this.m_label_summary.Text = "l";
        // 
        // m_panel_dragger
        // 
        this.m_panel_dragger.CanMove = true;
        this.m_panel_dragger.CanSize = false;
        this.m_panel_dragger.GroupControlToDragBy = this.m_group_summary;
        this.m_panel_dragger.SizeBounds = 3;
        // 
        // InfoListForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.Buttons = System.Windows.Forms.MessageBoxButtons.OK;
        this.ClientSize = new System.Drawing.Size(476, 339);
        this.ControlBox = false;
        this.Controls.Add(this.m_lister);
        this.Controls.Add(this.m_group_summary);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
        this.MinimumSize = new System.Drawing.Size(212, 42);
        this.Name = "InfoListForm";
        this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
        this.Controls.SetChildIndex(this.m_group_summary, 0);
        this.Controls.SetChildIndex(this.m_lister, 0);
        ((System.ComponentModel.ISupportInitialize)(this.m_picture)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.m_group_summary)).EndInit();
        this.m_group_summary.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.m_close.Properties)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.Controls.InfoTreeView m_lister;
    private System.Windows.Forms.PictureBox m_picture;
    private DevExpress.XtraEditors.GroupControl m_group_summary;
    private DevExpress.XtraEditors.LabelControl m_label_summary;
    private DevExpress.XtraEditors.ButtonEdit m_close;
    private AT.Toolbox.Dialogs.GroupControlDragger m_panel_dragger;
  }
}
