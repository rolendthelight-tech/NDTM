using Toolbox.GUI.DX.Controls;

namespace Toolbox.GUI.DX.Dialogs.Work
{
  partial class BackgroundWorkerForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundWorkerForm));
			this.m_group = new DevExpress.XtraEditors.GroupControl();
			this.m_panel_state = new DevExpress.XtraEditors.PanelControl();
			this.m_progress_bar = new DevExpress.XtraEditors.ProgressBarControl();
			this.m_marquee_bar = new DevExpress.XtraEditors.MarqueeProgressBarControl();
			this.m_label_state = new DevExpress.XtraEditors.LabelControl();
			this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
			this.m_picture_box = new System.Windows.Forms.PictureBox();
			this.m_worker = new System.ComponentModel.BackgroundWorker();
			this.m_dragger = new Toolbox.GUI.DX.Controls.GroupControlDragger(this.components);
			((System.ComponentModel.ISupportInitialize)(this.m_group)).BeginInit();
			this.m_group.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_panel_state)).BeginInit();
			this.m_panel_state.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_marquee_bar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
			this.SuspendLayout();
			// 
			// m_group
			// 
			this.m_group.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("m_group.AppearanceCaption.Font")));
			this.m_group.AppearanceCaption.Options.UseFont = true;
			this.m_group.Controls.Add(this.m_panel_state);
			this.m_group.Controls.Add(this.m_cancel_btn);
			this.m_group.Controls.Add(this.m_picture_box);
			resources.ApplyResources(this.m_group, "m_group");
			this.m_group.Name = "m_group";
			// 
			// m_panel_state
			// 
			resources.ApplyResources(this.m_panel_state, "m_panel_state");
			this.m_panel_state.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.m_panel_state.Controls.Add(this.m_progress_bar);
			this.m_panel_state.Controls.Add(this.m_marquee_bar);
			this.m_panel_state.Controls.Add(this.m_label_state);
			this.m_panel_state.Name = "m_panel_state";
			// 
			// m_progress_bar
			// 
			resources.ApplyResources(this.m_progress_bar, "m_progress_bar");
			this.m_progress_bar.Name = "m_progress_bar";
			this.m_progress_bar.Properties.Maximum = 536870912;
			this.m_progress_bar.Properties.Minimum = -536870912;
			this.m_progress_bar.Properties.Step = 67108864;
			// 
			// m_marquee_bar
			// 
			resources.ApplyResources(this.m_marquee_bar, "m_marquee_bar");
			this.m_marquee_bar.Name = "m_marquee_bar";
			this.m_marquee_bar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
			// 
			// m_label_state
			// 
			resources.ApplyResources(this.m_label_state, "m_label_state");
			this.m_label_state.Name = "m_label_state";
			// 
			// m_cancel_btn
			// 
			this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.m_cancel_btn, "m_cancel_btn");
			this.m_cancel_btn.Name = "m_cancel_btn";
			this.m_cancel_btn.Click += new System.EventHandler(this.HandleCancel);
			// 
			// m_picture_box
			// 
			resources.ApplyResources(this.m_picture_box, "m_picture_box");
			this.m_picture_box.Name = "m_picture_box";
			this.m_picture_box.TabStop = false;
			// 
			// m_worker
			// 
			this.m_worker.WorkerReportsProgress = true;
			this.m_worker.WorkerSupportsCancellation = true;
			this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RunBackgroundWork);
			this.m_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.HandleProgressChange);
			this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.HandleWorkComnpleted);
			// 
			// m_dragger
			// 
			this.m_dragger.CanMove = true;
			this.m_dragger.CanSize = false;
			this.m_dragger.GroupControlToDragBy = this.m_group;
			this.m_dragger.SizeBounds = 3;
			// 
			// BackgroundWorkerForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_group);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "BackgroundWorkerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Shown += new System.EventHandler(this.HandleFormShown);
			((System.ComponentModel.ISupportInitialize)(this.m_group)).EndInit();
			this.m_group.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_panel_state)).EndInit();
			this.m_panel_state.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_progress_bar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_marquee_bar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
			this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.GroupControl m_group;

    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private System.Windows.Forms.PictureBox m_picture_box;
    private System.ComponentModel.BackgroundWorker m_worker;
    private DevExpress.XtraEditors.LabelControl m_label_state;
    private GroupControlDragger m_dragger;
		private DevExpress.XtraEditors.MarqueeProgressBarControl m_marquee_bar;
		private DevExpress.XtraEditors.ProgressBarControl m_progress_bar;
		private DevExpress.XtraEditors.PanelControl m_panel_state;
	}
}