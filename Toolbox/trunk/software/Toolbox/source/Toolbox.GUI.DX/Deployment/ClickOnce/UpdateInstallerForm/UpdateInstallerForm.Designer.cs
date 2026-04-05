namespace Toolbox.GUI.DX.Deployment.ClickOnce.UpdateInstallerForm
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
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }

			{
				var checker = this.m_update_checker;
				if (checker != null)
					checker.Dispose();
			}
			{
				var installer = this.m_update_installer;
				if (installer != null)
					installer.Dispose();
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
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
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
			this.m_additional_info = new System.Windows.Forms.GroupBox();
			this.m_update_location = new DevExpress.XtraEditors.HyperLinkEdit();
			this.m_data_directory = new DevExpress.XtraEditors.HyperLinkEdit();
			this.m_start_uri = new DevExpress.XtraEditors.HyperLinkEdit();
			this.m_is_first_run = new DevExpress.XtraEditors.CheckEdit();
			this.m_time_of_last_update_check_label = new DevExpress.XtraEditors.LabelControl();
			this.m_update_location_label = new DevExpress.XtraEditors.LabelControl();
			this.m_time_of_last_update_check = new DevExpress.XtraEditors.ButtonEdit();
			this.m_data_directory_label = new DevExpress.XtraEditors.LabelControl();
			this.m_is_first_run_label = new DevExpress.XtraEditors.LabelControl();
			this.m_start_uri_label = new DevExpress.XtraEditors.LabelControl();
			this.m_version_info.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_updated_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_cur_version.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_available_version_progress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_progress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_install_update_state.Properties)).BeginInit();
			this.m_update_info.SuspendLayout();
			this.m_additional_info.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_update_location.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_data_directory.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_start_uri.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_is_first_run.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_time_of_last_update_check.Properties)).BeginInit();
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
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Redo, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "Обновить информацию", null, null, true)});
			this.m_available_version.Properties.ReadOnly = true;
			this.m_available_version.Size = new System.Drawing.Size(296, 20);
			this.m_available_version.TabIndex = 9;
			this.m_available_version.ToolTip = "Доступная версия на сервере";
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
			this.m_updated_version.ToolTip = "Версия установленной программы (она будет запущена в следующий раз)";
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
			this.m_cur_version.ToolTip = "Версия этой запущеной копии программы";
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
			this.m_install_update_button.ToolTip = "Установить новую версию программы, если доступна";
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
			this.m_install_update_state.ToolTip = "Статус установки новой версии";
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
			// m_additional_info
			// 
			this.m_additional_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_additional_info.Controls.Add(this.m_update_location);
			this.m_additional_info.Controls.Add(this.m_data_directory);
			this.m_additional_info.Controls.Add(this.m_start_uri);
			this.m_additional_info.Controls.Add(this.m_is_first_run);
			this.m_additional_info.Controls.Add(this.m_time_of_last_update_check_label);
			this.m_additional_info.Controls.Add(this.m_update_location_label);
			this.m_additional_info.Controls.Add(this.m_time_of_last_update_check);
			this.m_additional_info.Controls.Add(this.m_data_directory_label);
			this.m_additional_info.Controls.Add(this.m_is_first_run_label);
			this.m_additional_info.Controls.Add(this.m_start_uri_label);
			this.m_additional_info.Location = new System.Drawing.Point(7, 274);
			this.m_additional_info.Name = "m_additional_info";
			this.m_additional_info.Size = new System.Drawing.Size(430, 147);
			this.m_additional_info.TabIndex = 1;
			this.m_additional_info.TabStop = false;
			this.m_additional_info.Text = "Дополнительная информация";
			// 
			// m_update_location
			// 
			this.m_update_location.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_update_location.Location = new System.Drawing.Point(128, 121);
			this.m_update_location.Name = "m_update_location";
			this.m_update_location.Properties.ReadOnly = true;
			this.m_update_location.Size = new System.Drawing.Size(296, 20);
			this.m_update_location.TabIndex = 10;
			this.m_update_location.ToolTip = "Uri местоположения обновлений этой программы";
			// 
			// m_data_directory
			// 
			this.m_data_directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_data_directory.Location = new System.Drawing.Point(128, 95);
			this.m_data_directory.Name = "m_data_directory";
			this.m_data_directory.Properties.ReadOnly = true;
			this.m_data_directory.Size = new System.Drawing.Size(296, 20);
			this.m_data_directory.TabIndex = 10;
			this.m_data_directory.ToolTip = "Папка данных программы";
			// 
			// m_start_uri
			// 
			this.m_start_uri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_start_uri.Location = new System.Drawing.Point(128, 17);
			this.m_start_uri.Name = "m_start_uri";
			this.m_start_uri.Properties.ReadOnly = true;
			this.m_start_uri.Size = new System.Drawing.Size(296, 20);
			this.m_start_uri.TabIndex = 10;
			this.m_start_uri.ToolTip = "Uri запуска этой программы";
			// 
			// m_is_first_run
			// 
			this.m_is_first_run.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_is_first_run.Location = new System.Drawing.Point(127, 43);
			this.m_is_first_run.Name = "m_is_first_run";
			this.m_is_first_run.Properties.Caption = "";
			this.m_is_first_run.Properties.ReadOnly = true;
			this.m_is_first_run.Size = new System.Drawing.Size(297, 19);
			this.m_is_first_run.TabIndex = 9;
			this.m_is_first_run.ToolTip = "Это первый запуск программы";
			// 
			// m_time_of_last_update_check_label
			// 
			this.m_time_of_last_update_check_label.Location = new System.Drawing.Point(6, 72);
			this.m_time_of_last_update_check_label.Name = "m_time_of_last_update_check_label";
			this.m_time_of_last_update_check_label.Size = new System.Drawing.Size(169, 13);
			this.m_time_of_last_update_check_label.TabIndex = 8;
			this.m_time_of_last_update_check_label.Text = "Последняя проверка обновлений";
			// 
			// m_update_location_label
			// 
			this.m_update_location_label.Location = new System.Drawing.Point(6, 124);
			this.m_update_location_label.Name = "m_update_location_label";
			this.m_update_location_label.Size = new System.Drawing.Size(111, 13);
			this.m_update_location_label.TabIndex = 5;
			this.m_update_location_label.Text = "Источник обновлений";
			// 
			// m_time_of_last_update_check
			// 
			this.m_time_of_last_update_check.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_time_of_last_update_check.EditValue = "";
			this.m_time_of_last_update_check.Location = new System.Drawing.Point(181, 69);
			this.m_time_of_last_update_check.Name = "m_time_of_last_update_check";
			this.m_time_of_last_update_check.Properties.ReadOnly = true;
			this.m_time_of_last_update_check.Size = new System.Drawing.Size(243, 20);
			this.m_time_of_last_update_check.TabIndex = 7;
			this.m_time_of_last_update_check.ToolTip = "Время последней проверки обновлений";
			// 
			// m_data_directory_label
			// 
			this.m_data_directory_label.Location = new System.Drawing.Point(6, 98);
			this.m_data_directory_label.Name = "m_data_directory_label";
			this.m_data_directory_label.Size = new System.Drawing.Size(73, 13);
			this.m_data_directory_label.TabIndex = 5;
			this.m_data_directory_label.Text = "Папка данных";
			// 
			// m_is_first_run_label
			// 
			this.m_is_first_run_label.Location = new System.Drawing.Point(6, 46);
			this.m_is_first_run_label.Name = "m_is_first_run_label";
			this.m_is_first_run_label.Size = new System.Drawing.Size(76, 13);
			this.m_is_first_run_label.TabIndex = 6;
			this.m_is_first_run_label.Text = "Первый запуск";
			// 
			// m_start_uri_label
			// 
			this.m_start_uri_label.Location = new System.Drawing.Point(6, 20);
			this.m_start_uri_label.Name = "m_start_uri_label";
			this.m_start_uri_label.Size = new System.Drawing.Size(56, 13);
			this.m_start_uri_label.TabIndex = 5;
			this.m_start_uri_label.Text = "Uri запуска";
			// 
			// UpdateInstallerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Buttons = System.Windows.Forms.MessageBoxButtons.OK;
			this.ClientSize = new System.Drawing.Size(444, 237);
			this.Controls.Add(this.m_update_info);
			this.Controls.Add(this.m_version_info);
			this.Controls.Add(this.m_additional_info);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Name = "UpdateInstallerForm";
			this.Text = "Установка обновлений ClickOnce";
			this.Controls.SetChildIndex(this.m_additional_info, 0);
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
			this.m_additional_info.ResumeLayout(false);
			this.m_additional_info.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_update_location.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_data_directory.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_start_uri.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_is_first_run.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_time_of_last_update_check.Properties)).EndInit();
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
		private System.Windows.Forms.GroupBox m_additional_info;
		private DevExpress.XtraEditors.LabelControl m_time_of_last_update_check_label;
		private DevExpress.XtraEditors.ButtonEdit m_time_of_last_update_check;
		private DevExpress.XtraEditors.LabelControl m_is_first_run_label;
		private DevExpress.XtraEditors.LabelControl m_start_uri_label;
		private DevExpress.XtraEditors.CheckEdit m_is_first_run;
		private DevExpress.XtraEditors.HyperLinkEdit m_start_uri;
		private DevExpress.XtraEditors.HyperLinkEdit m_update_location;
		private DevExpress.XtraEditors.HyperLinkEdit m_data_directory;
		private DevExpress.XtraEditors.LabelControl m_update_location_label;
		private DevExpress.XtraEditors.LabelControl m_data_directory_label;

	}
}