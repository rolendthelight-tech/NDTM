using System;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Properties;
using Toolbox.GUI.Deployment.ClickOnce;

namespace Toolbox.GUI.DX.Deployment.ClickOnce.UpdateInstallerForm
{
	public partial class UpdateInstallerForm : DialogBase, UpdateChecker.IFormUpdateCheck, UpdateInstaller.IFormUpdateInstaller
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(UpdateInstallerForm));

		protected UpdateChecker m_update_checker;
		protected UpdateInstaller m_update_installer;

		protected Action m_after_update_checked;
		protected Action m_after_update_installed;

		public static BarItem BarItem
		{
			get
			{
				var item = new BarButtonItem() {Caption = Resources.UPDATING_OF_THE_PROGRAM};
				item.ItemClick +=
					(sender, e) =>
						{
							using (var dlg = new UpdateInstallerForm())
							{
								dlg.ShowDialog(sender as IWin32Window);
							}
						};
				return item;
			}
		}

		public UpdateInstallerForm()
		{
			_log.DebugFormat("UpdateInstallerForm(): открытие формы обновления приложения");
			InitializeComponent();
		}

		public override void PerformLocalization(object sender, EventArgs e)
		{
			base.PerformLocalization(sender, e);
			this.m_cur_version_label.Text = Resources.CURRENT_VERSION;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				this.m_update_checker = new UpdateChecker(this);
				this.m_update_installer = new UpdateInstaller(this);
				{
					var ver = this.m_update_checker.GetCurrentVersion();
					this.m_cur_version.Text = ver == null ? "не известно" : ver.ToString();
				}
				{
					var ver = this.m_update_checker.GetUpdatedVersion();
					this.m_updated_version.Text = ver == null ? "не известно" : ver.ToString();
				}
				{
					var uri = this.m_update_checker.GetActivationUri();
					this.m_start_uri.EditValue = uri == null ? new UriBuilder().Uri : uri;
				}
				{
					var first = this.m_update_checker.GetIsFirstRun();
					this.m_is_first_run.Checked = first;
				}
				{
					var time = this.m_update_checker.GetTimeOfLastUpdateCheck();
					this.m_time_of_last_update_check.EditValue = time == null ? new DateTime() : time;
				}
				{
					var uri = new UriBuilder() { Scheme = "file", Path = this.m_update_checker.GetDataDirectory() }.Uri;
					this.m_data_directory.EditValue = uri == null ? new UriBuilder().Uri : uri;
				}
				{
					var uri = this.m_update_checker.GetUpdateLocation();
					this.m_update_location.EditValue = uri == null ? new UriBuilder().Uri : uri;
				}
				this.m_after_update_checked = null;
				this.m_after_update_installed = null;
			}
			catch (System.Deployment.Application.InvalidDeploymentException ex)
			{
				_log.Error("UpdateInstallerForm(): не удалось инициализировать обновление, возможно это приложение не ClickOnce", ex);
				if (this.m_update_checker != null)
				{
					this.m_update_checker.Dispose();
					this.m_update_checker = null;
				}
				if (this.m_update_installer != null)
				{
					this.m_update_installer.Dispose();
					this.m_update_installer = null;
				}
				this.m_install_update_button.Enabled = false;
				this.m_install_update_state.Text = "Не возможно";
				this.m_install_update_button.Click -= new System.EventHandler(this.m_install_update_button_Click);
				this.m_cur_version.Text = Resources.APLLICATION_IS_NOT_CLICKONCE;
				this.m_updated_version.Text = Resources.APLLICATION_IS_NOT_CLICKONCE;
				this.m_available_version.Text = Resources.APLLICATION_IS_NOT_CLICKONCE;
				this.m_start_uri.EditValue = new UriBuilder().Uri;
				this.m_is_first_run.CheckState = CheckState.Indeterminate;
				this.m_time_of_last_update_check.EditValue = new DateTime();
				this.m_data_directory.EditValue = new UriBuilder().Uri;
				this.m_update_location.EditValue = new UriBuilder().Uri;

				for (int i = 0; i < this.m_available_version.Properties.Buttons.Count; ++i)
				{
					switch (this.m_available_version.Properties.Buttons[i].Kind)
					{
						case ButtonPredefines.Redo:
							this.m_available_version.Properties.Buttons[i].Enabled = false;
							break;
					}
				}
			}

			if (this.m_update_checker != null)
				this.m_update_checker.GetUpdateInfoAsync();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if (this.m_update_checker != null)
				this.m_update_checker.GetUpdateInfoAsyncCancel();
			base.OnFormClosed(e);
		}

		#region IFormUpdateCheck Members

		string UpdateChecker.IFormUpdateCheck.ResultGetInfo
		{
			set
			{
				if (InvokeRequired)
				{
					Action<string> d = v => ((UpdateChecker.IFormUpdateCheck)this).ResultGetInfo = v;
					Invoke(d, new object[] {value});
				}
				else
				{
					this.m_available_version.Text = value;
				}
			}
		}

		void UpdateChecker.IFormUpdateCheck.Start()
		{
			if (InvokeRequired)
			{
				Action d = () => ((UpdateChecker.IFormUpdateCheck)this).Start();
				Invoke(d, ArrayExtensions.Empty<object>());
			}
			else
			{
				this.m_available_version.Visible = false;
				this.m_available_version_progress.Visible = true;
			}
		}

		int UpdateChecker.IFormUpdateCheck.Progress
		{
			set
			{
				if (InvokeRequired)
				{
					Action<int> d = v => ((UpdateChecker.IFormUpdateCheck)this).Progress = v;
					Invoke(d, new object[] {value});
				}
				else
				{
					this.m_available_version_progress.Position = value;
				}
			}
		}

		void UpdateChecker.IFormUpdateCheck.End()
		{
			if (InvokeRequired)
			{
				Action d = () => ((UpdateChecker.IFormUpdateCheck)this).End();
				Invoke(d, ArrayExtensions.Empty<object>());
			}
			else
			{
				this.m_available_version_progress.Visible = false;
				this.m_available_version.Visible = true;
				try
				{
					var time = this.m_update_checker.GetTimeOfLastUpdateCheck();
					this.m_time_of_last_update_check.EditValue = time == null ? new DateTime() : time;
				}
				catch (Exception ex)
				{
					_log.Error("UpdateChecker.IFormUpdateCheck.End(): exception:", ex);
				}
				if (this.m_after_update_checked != null)
				{
					this.m_after_update_checked();
					this.m_after_update_checked = null;
				}
			}
		}

		#endregion

		#region IFormUpdateInstaller Members

		string UpdateInstaller.IFormUpdateInstaller.ResultUpdate
		{
			set
			{
				if (InvokeRequired)
				{
					Action<string> d = v => ((UpdateInstaller.IFormUpdateInstaller)this).ResultUpdate = v;
					Invoke(d, new object[] { value });
				}
				else
				{
					this.m_updated_version.Text = value;
				}
			}
		}

		string UpdateInstaller.IFormUpdateInstaller.State
		{
			set
			{
				if (InvokeRequired)
				{
					Action<string> d = v => ((UpdateInstaller.IFormUpdateInstaller) this).State = v;
					Invoke(d, new object[] {value});
				}
				else
				{
					this.m_install_update_state.Text = value;
				}
			}
		}

		void UpdateInstaller.IFormUpdateInstaller.Start()
		{
			if (InvokeRequired)
			{
				Action d = () => ((UpdateInstaller.IFormUpdateInstaller)this).Start();
				Invoke(d, ArrayExtensions.Empty<object>());
			}
			else
			{
				this.m_install_update_progress.Visible = true;
			}
		}

		int UpdateInstaller.IFormUpdateInstaller.Progress
		{
			set
			{
				if (InvokeRequired)
				{
					Action<int> d = v => ((UpdateInstaller.IFormUpdateInstaller) this).Progress = v;
					Invoke(d, new object[] {value});
				}
				else
				{
					this.m_install_update_progress.Position = value;
				}
			}
		}

		void UpdateInstaller.IFormUpdateInstaller.End()
		{
			if (InvokeRequired)
			{
				Action d = () => ((UpdateInstaller.IFormUpdateInstaller)this).End();
				Invoke(d, ArrayExtensions.Empty<object>());
			}
			else
			{
				this.m_install_update_progress.Visible = false;
				if (this.m_after_update_installed != null)
				{
					this.m_after_update_installed();
					this.m_after_update_installed = null;
				}
			}
		}

		#endregion

		private void m_available_version_button_Click(object sender, EventArgs e)
		{
			this.m_update_checker.GetUpdateInfoAsync();
		}

		private void m_install_update_button_Click(object sender, EventArgs e)
		{
			this.m_update_checker.GetUpdateInfoAsyncCancel();
			this.m_update_installer.UpdateAsync();
		}

		#region IFormUpdateInstaller Members

		bool UpdateInstaller.IFormUpdateInstaller.Updated
		{
			set
			{
				if (value)
					this.m_after_update_installed =
						delegate()
							{
								using (var dlg = new MessageBoxEx("Обновление установлено",
								                                  "Обновление установлено. Для запуска новой версии нужно перезапустить программу. Перезапустить сейчас?",
								                                  MessageBoxButtons.YesNo)
										{Text = "Обновление установлено"})
								{
									if (dlg.ShowDialog(this) == DialogResult.Yes)
										AppManager.Instance.Restart();
								}
							};
				else
					this.m_after_update_installed = null;
			}
		}

		#endregion

		#region IFormUpdateCheck Members

		bool UpdateChecker.IFormUpdateCheck.UpdateAvailable
		{
			set
			{
				if (value)
					this.m_after_update_checked =
						delegate()
							{
								using (var dlg = new MessageBoxEx("Доступно обновление", "Доступно обновление. Обновить?", MessageBoxButtons.YesNo)
										{Text = "Доступно обновление"})
								{
									if (dlg.ShowDialog(this) == DialogResult.Yes)
										this.m_update_installer.UpdateAsync();
								}
							};
				else
					this.m_after_update_checked = null;
			}
		}

		#endregion

		private void m_available_version_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			switch (e.Button.Kind)
			{
				case ButtonPredefines.Redo:
					m_available_version_button_Click(sender, e);
					break;
			}
		}
	}
}