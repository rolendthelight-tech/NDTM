using System;
using System.Windows.Forms;
using AT.Toolbox.Network.ClickOnce;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;

namespace AT.Toolbox.Dialogs.UpdateInstallerForm
{
	public partial class UpdateInstallerForm : AT.Toolbox.Dialogs.DialogBase, UpdateChecker.IFormUpdateCheck, UpdateInstaller.IFormUpdateInstaller
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(UpdateInstallerForm));

		protected UpdateChecker m_update_checker;
		protected UpdateInstaller m_update_installer;

		public static BarItem BarItem
		{
			get
			{
				var item = new BarButtonItem() {Caption = "Обновление программы"};
				item.ItemClick += (sender, e) => new UpdateInstallerForm().ShowDialog(sender as IWin32Window);
				return item;
			}
		}

		public UpdateInstallerForm()
		{
			_log.Debug("UpdateInstallerForm(): открытие формы обновления приложения");
			InitializeComponent();
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
			}
			catch (System.Deployment.Application.InvalidDeploymentException ex)
			{
				_log.Error("UpdateInstallerForm(): не удалось инициализировать обновление, возможно это приложение не ClickOnce", ex);
				this.m_update_checker = null;
				this.m_update_installer = null;
				this.m_install_update_button.Enabled = false;
				this.m_install_update_button.Click -= new System.EventHandler(this.m_install_update_button_Click);
				this.m_cur_version.Text = this.m_updated_version.Text = this.m_available_version.Text = "Это приложение не ClickOnce";

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

		protected override void OnClosed(EventArgs e)
		{
			if (this.m_update_checker != null)
				this.m_update_checker.GetUpdateInfoAsyncCancel();
			base.OnClosed(e);
		}

		#region IFormUpdateCheck Members

		string UpdateChecker.IFormUpdateCheck.ResultGetInfo
		{
			set { this.m_available_version.Text = value; }
		}

		void UpdateChecker.IFormUpdateCheck.Start()
		{
			this.m_available_version.Visible = false;
			this.m_available_version_progress.Visible = true;
		}

		int UpdateChecker.IFormUpdateCheck.Progress
		{
			set { this.m_available_version_progress.Position = value; }
		}

		void UpdateChecker.IFormUpdateCheck.End()
		{
			this.m_available_version_progress.Visible = false;
			this.m_available_version.Visible = true;
		}

		#endregion

		#region IFormUpdateInstaller Members

		string UpdateInstaller.IFormUpdateInstaller.ResultUpdate
		{
			set { this.m_updated_version.Text = value; }
		}

		string UpdateInstaller.IFormUpdateInstaller.State
		{
			set { this.m_install_update_state.Text = value; }
		}

		void UpdateInstaller.IFormUpdateInstaller.Start()
		{
			this.m_install_update_progress.Visible = true;
		}

		int UpdateInstaller.IFormUpdateInstaller.Progress
		{
			set { this.m_install_update_progress.Position = value; }
		}

		void UpdateInstaller.IFormUpdateInstaller.End()
		{
			this.m_install_update_progress.Visible = false;
		}

		#endregion

		private void m_available_version_button_Click(object sender, EventArgs e)
		{
			this.m_update_checker.GetUpdateInfoAsync();
		}

		private void m_install_update_button_Click(object sender, EventArgs e)
		{
			this.m_update_installer.UpdateAsync();
		}

		#region IFormUpdateInstaller Members


		bool UpdateInstaller.IFormUpdateInstaller.Updated
		{
			set
			{
				if (value)
					if (
						new MessageBoxEx("Обновление установлено", "Обновление установлено. Для запуска новой версии нужно перезапустить программу. Перезапустить сейчас?", MessageBoxButtons.YesNo)
						{
							Text = "Обновление установлено"
						}.ShowDialog(this) == DialogResult.Yes)
						Application.Restart();
			}
		}

		#endregion

		#region IFormUpdateCheck Members


		bool UpdateChecker.IFormUpdateCheck.UpdateAvailable
		{
			set
			{
				if (value)
					if (
						new MessageBoxEx("Доступно обновление", "Доступно обновление. Обновить?", MessageBoxButtons.YesNo)
							{
								Text = "Доступно обновление"
							}.ShowDialog(this) == DialogResult.Yes)
						this.m_update_installer.UpdateAsync();
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