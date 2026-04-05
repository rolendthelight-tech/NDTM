namespace AT.Toolbox.Dialogs
{
  partial class MinimizableTaskForm
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
      this.m_main_group = new DevExpress.XtraEditors.GroupControl();
      this.m_label_state = new DevExpress.XtraEditors.LabelControl();
      this.m_progress_bar = new DevExpress.XtraEditors.ProgressBarControl();
      this.m_marquee = new DevExpress.XtraEditors.MarqueeProgressBarControl();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      this.m_cancel_button = new DevExpress.XtraEditors.SimpleButton();
      this.m_minimize_button = new DevExpress.XtraEditors.ButtonEdit();
      this.m_worker = new System.ComponentModel.BackgroundWorker();
      this.m_dragger = new AT.Toolbox.Dialogs.GroupControlDragger(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_main_group)).BeginInit();
      this.m_main_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_marquee.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_minimize_button.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_main_group
      // 
      this.m_main_group.Controls.Add(this.m_label_state);
      this.m_main_group.Controls.Add(this.m_progress_bar);
      this.m_main_group.Controls.Add(this.m_marquee);
      this.m_main_group.Controls.Add(this.m_picture_box);
      this.m_main_group.Controls.Add(this.m_cancel_button);
      this.m_main_group.Controls.Add(this.m_minimize_button);
      this.m_main_group.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_main_group.Location = new System.Drawing.Point(0, 0);
      this.m_main_group.Name = "m_main_group";
      this.m_main_group.Size = new System.Drawing.Size(465, 103);
      this.m_main_group.TabIndex = 0;
      // 
      // m_label_state
      // 
      this.m_label_state.Location = new System.Drawing.Point(68, 33);
      this.m_label_state.Name = "m_label_state";
      this.m_label_state.Size = new System.Drawing.Size(0, 13);
      this.m_label_state.TabIndex = 5;
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Location = new System.Drawing.Point(68, 52);
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Size = new System.Drawing.Size(389, 18);
      this.m_progress_bar.TabIndex = 4;
      // 
      // m_marquee
      // 
      this.m_marquee.EditValue = 0;
      this.m_marquee.Location = new System.Drawing.Point(68, 52);
      this.m_marquee.Name = "m_marquee";
      this.m_marquee.Size = new System.Drawing.Size(389, 18);
      this.m_marquee.TabIndex = 3;
      // 
      // m_picture_box
      // 
      this.m_picture_box.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_picture_box.Location = new System.Drawing.Point(12, 25);
      this.m_picture_box.MaximumSize = new System.Drawing.Size(50, 50);
      this.m_picture_box.MinimumSize = new System.Drawing.Size(50, 50);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.Size = new System.Drawing.Size(50, 50);
      this.m_picture_box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.m_picture_box.TabIndex = 2;
      this.m_picture_box.TabStop = false;
      // 
      // m_cancel_button
      // 
      this.m_cancel_button.Location = new System.Drawing.Point(194, 76);
      this.m_cancel_button.Name = "m_cancel_button";
      this.m_cancel_button.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_button.TabIndex = 1;
      this.m_cancel_button.Text = "Cancel";
      this.m_cancel_button.Click += new System.EventHandler(this.m_cancel_button_Click);
      // 
      // m_minimize_button
      // 
      this.m_minimize_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_minimize_button.Location = new System.Drawing.Point(443, 1);
      this.m_minimize_button.Name = "m_minimize_button";
      this.m_minimize_button.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
      this.m_minimize_button.Properties.Appearance.Options.UseBackColor = true;
      this.m_minimize_button.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.m_minimize_button.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Minus)});
      this.m_minimize_button.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
      this.m_minimize_button.Size = new System.Drawing.Size(22, 17);
      this.m_minimize_button.TabIndex = 0;
      this.m_minimize_button.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_minimize_button_ButtonClick);
      // 
      // m_worker
      // 
      this.m_worker.WorkerReportsProgress = true;
      this.m_worker.WorkerSupportsCancellation = true;
      this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.m_worker_DoWork);
      this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.m_worker_RunWorkerCompleted);
      // 
      // m_dragger
      // 
      this.m_dragger.CanMove = true;
      this.m_dragger.CanSize = false;
      this.m_dragger.GroupControlToDragBy = this.m_main_group;
      this.m_dragger.SizeBounds = 3;
      // 
      // MinimizableTaskForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(465, 103);
      this.ControlBox = false;
      this.Controls.Add(this.m_main_group);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "MinimizableTaskForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "";
      ((System.ComponentModel.ISupportInitialize)(this.m_main_group)).EndInit();
      this.m_main_group.ResumeLayout(false);
      this.m_main_group.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_marquee.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_minimize_button.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.GroupControl m_main_group;
    private DevExpress.XtraEditors.ButtonEdit m_minimize_button;
    private DevExpress.XtraEditors.SimpleButton m_cancel_button;
    private System.Windows.Forms.PictureBox m_picture_box;
    private DevExpress.XtraEditors.MarqueeProgressBarControl m_marquee;
    private DevExpress.XtraEditors.ProgressBarControl m_progress_bar;
    private System.ComponentModel.BackgroundWorker m_worker;
    private DevExpress.XtraEditors.LabelControl m_label_state;
    private GroupControlDragger m_dragger;
  }
}