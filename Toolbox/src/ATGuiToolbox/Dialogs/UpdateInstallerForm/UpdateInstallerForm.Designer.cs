namespace AT.Toolbox.Dialogs.UpdateInstallerForm
{
  partial class UpdateInstallerForm
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
			this.m_version_info = new System.Windows.Forms.GroupBox();
			this.m_available_version = new DevExpress.XtraEditors.ButtonEdit();
			this.m_available_version_label = new DevExpress.XtraEditors.LabelControl();
			this.m_updated_version = new DevExpress.XtraEditors.ButtonEdit();
			this.m_updated_version_label = new DevExpress.XtraEditors.LabelControl();
			this.m_cur_version_label = new DevExpress.XtraEditors.LabelControl();
			this.m_cur_version = new DevExpress.XtraEditors.ButtonEdit();
			this.m_available_version_progress = new DevExpress.XtraEditors.ProgressBarControl();
			this.m_install_update_button = new DevExpress.XtraEditors.SimpleButton();
			this.m_install_update_progress = new DevExpress.XtraEditors.ProgressBarControl();
			this.m_install_update_state = new DevExpress.XtraEditors.ButtonEdit();
			this.m_install_update_label = new DevExpress.XtraEditors.LabelControl();
			this.m_update_info = new System.Windows.Forms.GroupBox();
			this.m_version_info.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_updated_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_cur_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version_progress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_progress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_state.Properties)).BeginInit();
			this.m_update_info.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_buttons
			// 
			this.m_buttons.Buttons = System.Windows.Forms.MessageBoxButtons.OK;
			this.m_buttons.Size = new System.Drawing.Size(430, 29);
			// 
			// m_version_info
			// 
			this.m_version_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_version_info.Controls.Add(this.m_available_version);
			this.m_version_info.Controls.Add(this.m_available_version_label);
			this.m_version_info.Controls.Add(this.m_updated_version);
			this.m_version_info.Controls.Add(this.m_updated_version_label);
			this.m_version_info.Controls.Add(this.m_cur_version_label);
			this.m_version_info.Controls.Add(this.m_cur_version);
			this.m_version_info.Controls.Add(this.m_available_version_progress);
			this.m_version_info.Location = new System.Drawing.Point(7, 12);
			this.m_version_info.Name = "m_version_info";
			this.m_version_info.Size = new System.Drawing.Size(430, 95);
			this.m_version_info.TabIndex = 1;
			this.m_version_info.TabStop = false;
			this.m_version_info.Text = "Версия";
			// 
			// m_available_version
			// 
			this.m_available_version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_available_version.EditValue = "";
			this.m_available_version.Location = new System.Drawing.Point(128, 69);
			this.m_available_version.Name = "m_available_version";
			this.m_available_version.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Redo)});
			this.m_available_version.Properties.ReadOnly = true;
			this.m_available_version.Size = new System.Drawing.Size(296, 20);
			this.m_available_version.TabIndex = 9;
			this.m_available_version.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.m_available_version_ButtonClick);
			// 
			// m_available_version_label
			// 
			this.m_available_version_label.Location = new System.Drawing.Point(6, 72);
			this.m_available_version_label.Name = "m_available_version_label";
			this.m_available_version_label.Size = new System.Drawing.Size(93, 13);
			this.m_available_version_label.TabIndex = 8;
			this.m_available_version_label.Text = "Доступная версия";
			// 
			// m_updated_version
			// 
			this.m_updated_version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_updated_version.EditValue = "";
			this.m_updated_version.Location = new System.Drawing.Point(128, 43);
			this.m_updated_version.Name = "m_updated_version";
			this.m_updated_version.Properties.ReadOnly = true;
			this.m_updated_version.Size = new System.Drawing.Size(296, 20);
			this.m_updated_version.TabIndex = 7;
			// 
			// m_updated_version_label
			// 
			this.m_updated_version_label.Location = new System.Drawing.Point(6, 46);
			this.m_updated_version_label.Name = "m_updated_version_label";
			this.m_updated_version_label.Size = new System.Drawing.Size(116, 13);
			this.m_updated_version_label.TabIndex = 6;
			this.m_updated_version_label.Text = "Установленная версия";
			// 
			// m_cur_version_label
			// 
			this.m_cur_version_label.Location = new System.Drawing.Point(6, 20);
			this.m_cur_version_label.Name = "m_cur_version_label";
			this.m_cur_version_label.Size = new System.Drawing.Size(83, 13);
			this.m_cur_version_label.TabIndex = 5;
			this.m_cur_version_label.Text = "Текущая версия";
			// 
			// m_cur_version
			// 
			this.m_cur_version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_cur_version.EditValue = "";
			this.m_cur_version.Location = new System.Drawing.Point(128, 17);
			this.m_cur_version.Name = "m_cur_version";
			this.m_cur_version.Properties.ReadOnly = true;
			this.m_cur_version.Size = new System.Drawing.Size(296, 20);
			this.m_cur_version.TabIndex = 4;
			// 
			// m_available_version_progress
			// 
			this.m_available_version_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_available_version_progress.Location = new System.Drawing.Point(128, 69);
			this.m_available_version_progress.Name = "m_available_version_progress";
			this.m_available_version_progress.Size = new System.Drawing.Size(296, 19);
			this.m_available_version_progress.TabIndex = 0;
			this.m_available_version_progress.Visible = false;
			// 
			// m_install_update_button
			// 
			this.m_install_update_button.Location = new System.Drawing.Point(6, 20);
			this.m_install_update_button.Name = "m_install_update_button";
			this.m_install_update_button.Size = new System.Drawing.Size(117, 46);
			this.m_install_update_button.TabIndex = 11;
			this.m_install_update_button.Text = "Обновить программу";
			this.m_install_update_button.Click += new System.EventHandler(this.m_install_update_button_Click);
			// 
			// m_install_update_progress
			// 
			this.m_install_update_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_install_update_progress.Location = new System.Drawing.Point(129, 47);
			this.m_install_update_progress.Name = "m_install_update_progress";
			this.m_install_update_progress.Size = new System.Drawing.Size(295, 19);
			this.m_install_update_progress.TabIndex = 11;
			this.m_install_update_progress.Visible = false;
			// 
			// m_install_update_state
			// 
			this.m_install_update_state.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_install_update_state.EditValue = "Не запущено";
			this.m_install_update_state.Location = new System.Drawing.Point(253, 20);
			this.m_install_update_state.Name = "m_install_update_state";
			this.m_install_update_state.Properties.ReadOnly = true;
			this.m_install_update_state.Size = new System.Drawing.Size(172, 20);
			this.m_install_update_state.TabIndex = 12;
			// 
			// m_install_update_label
			// 
			this.m_install_update_label.Location = new System.Drawing.Point(129, 23);
			this.m_install_update_label.Name = "m_install_update_label";
			this.m_install_update_label.Size = new System.Drawing.Size(118, 13);
			this.m_install_update_label.TabIndex = 11;
			this.m_install_update_label.Text = "Получение обновлений";
			// 
			// m_update_info
			// 
			this.m_update_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_update_info.Controls.Add(this.m_install_update_button);
			this.m_update_info.Controls.Add(this.m_install_update_progress);
			this.m_update_info.Controls.Add(this.m_install_update_label);
			this.m_update_info.Controls.Add(this.m_install_update_state);
			this.m_update_info.Location = new System.Drawing.Point(7, 113);
			this.m_update_info.Name = "m_update_info";
			this.m_update_info.Size = new System.Drawing.Size(430, 76);
			this.m_update_info.TabIndex = 13;
			this.m_update_info.TabStop = false;
			this.m_update_info.Text = "Обновление";
			// 
			// UpdateInstallerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Buttons = System.Windows.Forms.MessageBoxButtons.OK;
			this.ClientSize = new System.Drawing.Size(444, 237);
			this.Controls.Add(this.m_update_info);
			this.Controls.Add(this.m_version_info);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Name = "UpdateInstallerForm";
			this.Text = "Установка обновлений ClickOnce";
			this.Controls.SetChildIndex(this.m_version_info, 0);
			this.Controls.SetChildIndex(this.m_update_info, 0);
			this.m_version_info.ResumeLayout(false);
			this.m_version_info.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_updated_version.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_cur_version.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version_progress.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_progress.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_state.Properties)).EndInit();
			this.m_update_info.ResumeLayout(false);
			this.m_update_info.PerformLayout();
			this.ResumeLayout(false);

    }

    #endregion

		private System.Windows.Forms.GroupBox m_version_info;
		private DevExpress.XtraEditors.ProgressBarControl m_available_version_progress;
		private DevExpress.XtraEditors.ButtonEdit m_cur_version;
		private DevExpress.XtraEditors.ButtonEdit m_available_version;
		private DevExpress.XtraEditors.LabelControl m_available_version_label;
		private DevExpress.XtraEditors.ButtonEdit m_updated_version;
		private DevExpress.XtraEditors.LabelControl m_updated_version_label;
		private DevExpress.XtraEditors.LabelControl m_cur_version_label;
		private DevExpress.XtraEditors.SimpleButton m_install_update_button;
		private DevExpress.XtraEditors.ProgressBarControl m_install_update_progress;
		private DevExpress.XtraEditors.ButtonEdit m_install_update_state;
		private DevExpress.XtraEditors.LabelControl m_install_update_label;
		private System.Windows.Forms.GroupBox m_update_info;

	}
}