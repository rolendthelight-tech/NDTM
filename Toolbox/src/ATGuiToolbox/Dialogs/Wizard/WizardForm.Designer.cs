namespace AT.Toolbox.Dialogs
{
  partial class WizardForm
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
      this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
      this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
      this.m_page_pane = new DevExpress.XtraEditors.PanelControl();
      this.m_back_button = new DevExpress.XtraEditors.SimpleButton();
      this.m_fwd_button = new DevExpress.XtraEditors.SimpleButton();
      this.m_finish_btn = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
      this.m_binding = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_page_pane)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding)).BeginInit();
      this.SuspendLayout();
      // 
      // panelControl1
      // 
      this.panelControl1.Dock = System.Windows.Forms.DockStyle.Left;
      this.panelControl1.Location = new System.Drawing.Point(0, 0);
      this.panelControl1.Name = "panelControl1";
      this.panelControl1.Size = new System.Drawing.Size(122, 505);
      this.panelControl1.TabIndex = 0;
      // 
      // labelControl2
      // 
      this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.labelControl2.Appearance.Options.UseFont = true;
      this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl2.Dock = System.Windows.Forms.DockStyle.Top;
      this.labelControl2.Location = new System.Drawing.Point(122, 0);
      this.labelControl2.Name = "labelControl2";
      this.labelControl2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
      this.labelControl2.Size = new System.Drawing.Size(638, 29);
      this.labelControl2.TabIndex = 1;
      this.labelControl2.Text = "Title";
      // 
      // m_page_pane
      // 
      this.m_page_pane.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_page_pane.Location = new System.Drawing.Point(122, 29);
      this.m_page_pane.Name = "m_page_pane";
      this.m_page_pane.Size = new System.Drawing.Size(638, 476);
      this.m_page_pane.TabIndex = 2;
      // 
      // m_back_button
      // 
      this.m_back_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_back_button.Location = new System.Drawing.Point(379, 511);
      this.m_back_button.Name = "m_back_button";
      this.m_back_button.Size = new System.Drawing.Size(75, 23);
      this.m_back_button.TabIndex = 3;
      this.m_back_button.Text = "Назад";
      this.m_back_button.Click += new System.EventHandler(this.m_back_button_Click);
      // 
      // m_fwd_button
      // 
      this.m_fwd_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_fwd_button.Location = new System.Drawing.Point(460, 511);
      this.m_fwd_button.Name = "m_fwd_button";
      this.m_fwd_button.Size = new System.Drawing.Size(75, 23);
      this.m_fwd_button.TabIndex = 4;
      this.m_fwd_button.Text = "Вперёд";
      this.m_fwd_button.Click += new System.EventHandler(this.HandleForwards);
      // 
      // m_finish_btn
      // 
      this.m_finish_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_finish_btn.Location = new System.Drawing.Point(571, 511);
      this.m_finish_btn.Name = "m_finish_btn";
      this.m_finish_btn.Size = new System.Drawing.Size(75, 23);
      this.m_finish_btn.TabIndex = 5;
      this.m_finish_btn.Text = "Завершить";
      this.m_finish_btn.Click += new System.EventHandler(this.m_finish_btn_Click);
      // 
      // simpleButton4
      // 
      this.simpleButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.simpleButton4.Location = new System.Drawing.Point(652, 511);
      this.simpleButton4.Name = "simpleButton4";
      this.simpleButton4.Size = new System.Drawing.Size(75, 23);
      this.simpleButton4.TabIndex = 6;
      this.simpleButton4.Text = "Отмена";
      this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
      // 
      // WizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(760, 540);
      this.Controls.Add(this.simpleButton4);
      this.Controls.Add(this.m_finish_btn);
      this.Controls.Add(this.m_fwd_button);
      this.Controls.Add(this.m_back_button);
      this.Controls.Add(this.m_page_pane);
      this.Controls.Add(this.labelControl2);
      this.Controls.Add(this.panelControl1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "WizardForm";
      this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 35);
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "WizardForm";
      this.Shown += new System.EventHandler(this.WizardForm_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_page_pane)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.LabelControl labelControl2;
    private DevExpress.XtraEditors.PanelControl m_page_pane;
    protected System.Windows.Forms.BindingSource m_binding;
    protected DevExpress.XtraEditors.SimpleButton m_back_button;
    protected DevExpress.XtraEditors.SimpleButton m_fwd_button;
    protected DevExpress.XtraEditors.SimpleButton m_finish_btn;
    protected DevExpress.XtraEditors.SimpleButton simpleButton4;
    protected DevExpress.XtraEditors.PanelControl panelControl1;
  }
}