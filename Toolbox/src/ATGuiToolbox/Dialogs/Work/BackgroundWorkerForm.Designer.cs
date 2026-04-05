namespace AT.Toolbox.Dialogs
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
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundWorkerForm));
      this.m_group = new DevExpress.XtraEditors.GroupControl();
      this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_info_label = new DevExpress.XtraEditors.LabelControl();
      this.m_picture_box = new System.Windows.Forms.PictureBox();
      this.m_worker = new System.ComponentModel.BackgroundWorker();
      this.groupControlDragger1 = new AT.Toolbox.Dialogs.GroupControlDragger(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_group)).BeginInit();
      this.m_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).BeginInit();
      this.SuspendLayout();
      // 
      // m_group
      // 
      this.m_group.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_group.AppearanceCaption.Options.UseFont = true;
      this.m_group.Controls.Add(this.panelControl1);
      this.m_group.Controls.Add(this.m_cancel_btn);
      this.m_group.Controls.Add(this.m_info_label);
      this.m_group.Controls.Add(this.m_picture_box);
      resources.ApplyResources(this.m_group, "m_group");
      this.m_group.Name = "m_group";
      // 
      // panelControl1
      // 
      this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      resources.ApplyResources(this.panelControl1, "panelControl1");
      this.panelControl1.Name = "panelControl1";
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      resources.ApplyResources(this.m_cancel_btn, "m_cancel_btn");
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Click += new System.EventHandler(this.HandleCancel);
      // 
      // m_info_label
      // 
      resources.ApplyResources(this.m_info_label, "m_info_label");
      this.m_info_label.Name = "m_info_label";
      // 
      // m_picture_box
      // 
      resources.ApplyResources(this.m_picture_box, "m_picture_box");
      this.m_picture_box.MaximumSize = new System.Drawing.Size(50, 50);
      this.m_picture_box.MinimumSize = new System.Drawing.Size(50, 50);
      this.m_picture_box.Name = "m_picture_box";
      this.m_picture_box.TabStop = false;
      // 
      // m_worker
      // 
      this.m_worker.WorkerReportsProgress = true;
      this.m_worker.WorkerSupportsCancellation = true;
      this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RunBackgroundWork);
      this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.HandleWorkComnpleted);
      this.m_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.HandleProgressChange);
      // 
      // groupControlDragger1
      // 
      this.groupControlDragger1.CanMove = true;
      this.groupControlDragger1.CanSize = false;
      this.groupControlDragger1.GroupControlToDragBy = this.m_group;
      this.groupControlDragger1.SizeBounds = 3;
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
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture_box)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.GroupControl m_group;
    private DevExpress.XtraEditors.LabelControl m_info_label;
    private System.Windows.Forms.PictureBox m_picture_box;
    private System.ComponentModel.BackgroundWorker m_worker;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private DevExpress.XtraEditors.PanelControl panelControl1;
    private GroupControlDragger groupControlDragger1;
  }
}