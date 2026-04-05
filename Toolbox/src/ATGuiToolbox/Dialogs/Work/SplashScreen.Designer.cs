namespace AT.Toolbox.Dialogs
{
  partial class SplashScreen
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
      this.m_worker = new System.ComponentModel.BackgroundWorker();
      this.m_panel = new DevExpress.XtraEditors.PanelControl();
      this.m_version_info = new DevExpress.XtraEditors.LabelControl();
      this.m_operation_info = new DevExpress.XtraEditors.LabelControl();
      this.m_progress_bar = new DevExpress.XtraEditors.ProgressBarControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_panel)).BeginInit();
      this.m_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_worker
      // 
      this.m_worker.WorkerReportsProgress = true;
      this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoBackgroundWork);
      this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.HandleWorkCompleted);
      this.m_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.HandleProgressChanged);
      // 
      // m_panel
      // 
      this.m_panel.Appearance.BackColor = System.Drawing.Color.Transparent;
      this.m_panel.Appearance.Options.UseBackColor = true;
      this.m_panel.AutoSize = true;
      this.m_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.m_panel.Controls.Add(this.m_version_info);
      this.m_panel.Controls.Add(this.m_operation_info);
      this.m_panel.Controls.Add(this.m_progress_bar);
      this.m_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_panel.Location = new System.Drawing.Point(0, 169);
      this.m_panel.Name = "m_panel";
      this.m_panel.Size = new System.Drawing.Size(320, 71);
      this.m_panel.TabIndex = 1;
      // 
      // m_version_info
      // 
      this.m_version_info.Appearance.Options.UseTextOptions = true;
      this.m_version_info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
      this.m_version_info.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_version_info.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_version_info.Location = new System.Drawing.Point(2, 2);
      this.m_version_info.Name = "m_version_info";
      this.m_version_info.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
      this.m_version_info.Size = new System.Drawing.Size(316, 25);
      this.m_version_info.TabIndex = 2;
      this.m_version_info.Text = "m_version_info";
      // 
      // m_operation_info
      // 
      this.m_operation_info.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_operation_info.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_operation_info.Location = new System.Drawing.Point(2, 27);
      this.m_operation_info.Name = "m_operation_info";
      this.m_operation_info.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
      this.m_operation_info.Size = new System.Drawing.Size(316, 24);
      this.m_operation_info.TabIndex = 1;
      this.m_operation_info.Text = "m_operation_info";
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_progress_bar.Location = new System.Drawing.Point(2, 51);
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Properties.UseParentBackground = true;
      this.m_progress_bar.Size = new System.Drawing.Size(316, 18);
      this.m_progress_bar.TabIndex = 0;
      // 
      // SplashScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(320, 240);
      this.Controls.Add(this.m_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "SplashScreen";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "SplashScreen";
      this.Shown += new System.EventHandler(this.HandleFormShown);
      ((System.ComponentModel.ISupportInitialize)(this.m_panel)).EndInit();
      this.m_panel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.ComponentModel.BackgroundWorker m_worker;
    private DevExpress.XtraEditors.PanelControl m_panel;
    private DevExpress.XtraEditors.LabelControl m_version_info;
    private DevExpress.XtraEditors.LabelControl m_operation_info;
    private DevExpress.XtraEditors.ProgressBarControl m_progress_bar;
  }
}