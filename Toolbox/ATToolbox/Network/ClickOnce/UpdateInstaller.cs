using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Reflection;

namespace AT.Toolbox.Network.ClickOnce
{
	public class UpdateInstaller
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(UpdateInstaller));
		
		private readonly IFormUpdateInstaller m_form;
		private readonly ApplicationDeployment m_app_deploy;

		public UpdateInstaller(IFormUpdateInstaller form)
		{
			this.m_form = form;
			this.m_app_deploy = ApplicationDeployment.CurrentDeployment;
		}

		public bool Update()
		{
			_log.Debug("Update(): подготовка к запуску обновления");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("Update(): запуск обновления");
					var suc = this.m_app_deploy.Update();
					this.m_form.ResultUpdate = suc ? string.Format("{0}", this.m_app_deploy.UpdatedVersion) : "обновлений нет";
					_log.DebugFormat("Update(): обновление завершено (обновлено: {0}, обновлённая версия: \"{1}\")", suc, this.m_app_deploy.UpdatedVersion);
					return suc;
				}
				catch (Exception ex)
				{
					_log.Error("Update(): ошибка обновления", ex);
					this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
					return false;
				}
				finally
				{
					this.m_form.End();
				}
			}
			else
			{
				_log.Warn("Update(): это приложение не ClickOnce");
				return false;
			}
		}

		public void UpdateAsync()
		{
			_log.Debug("UpdateAsync(): подготовка к запуску асинхронного обновления");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("UpdateAsync(): запуск асинхронного обновления");
					this.m_app_deploy.UpdateCompleted += this.appDeploy_UpdateCompleted;
					this.m_app_deploy.UpdateProgressChanged += this.appDeploy_UpdateProgressChanged;
					this.m_app_deploy.UpdateAsync();
				}
				catch (Exception ex)
				{
					_log.Error("UpdateAsync(): ошибка асинхронного обновления", ex);
					this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
				}
			}
			else
			{
				_log.Warn("UpdateAsync(): это приложение не ClickOnce");
				this.m_form.ResultUpdate = "Это приложение не ClickOnce";
			}
		}

		public void UpdateAsyncCancel()
		{
			_log.Debug("UpdateAsyncCancel(): подготовка к прерыванию асинхронного обновления");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				try
				{
					_log.Debug("UpdateAsyncCancel(): прерывание асинхронного обновления");
					this.m_app_deploy.UpdateAsyncCancel();
				}
				catch (Exception ex)
				{
					_log.Error("UpdateAsyncCancel(): ошибка прерывания асинхронного обновления", ex);
					this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
				}
			}
			else
			{
				_log.Warn("UpdateAsyncCancel(): это приложение не ClickOnce");
			}
		}

		#region реакция на события

		private void appDeploy_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
		{
			try
			{
				this.m_form.Progress = 0;

				this.m_form.ResultUpdate =
					e.Error == null
						? e.Cancelled
								? "Отменено"
								: string.Format("{0}", this.m_app_deploy.UpdatedVersion)
						: string.Format("Ошибка: {0}", e.Error);
				this.m_form.State = "Готово";

				_log.Debug(string.Format("appDeploy_UpdateCompleted(): асинхронное обновление завершено (отменено: {0}, ошибка: \"{1}\", состояние: \"{2}\")", e.Cancelled, e.Error, e.UserState), e.Error);
			}
			catch (TargetInvocationException ex)
			{
				_log.Error("appDeploy_UpdateCompleted(): ошибка асинхронного обновления", ex);
				this.m_form.ResultUpdate = string.Format("Ошибка при получении: {0}", ex);
			}
			catch (Exception ex)
			{
				_log.Error("appDeploy_UpdateCompleted(): ошибка асинхронного обновления", ex);
				this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
			}
			finally
			{
				try
				{
					ApplicationDeployment ad = (ApplicationDeployment) sender;
					ad.UpdateCompleted -= this.appDeploy_UpdateCompleted;
					ad.UpdateProgressChanged -= this.appDeploy_UpdateProgressChanged;
				}
				catch (Exception ex)
				{
					_log.Error("appDeploy_UpdateCompleted(): не удалось отписаться от событий", ex);
				}
				finally
				{
					this.m_form.Updated = e.Error == null && !e.Cancelled && this.m_app_deploy.CurrentVersion != this.m_app_deploy.UpdatedVersion;
				}
				this.m_form.End();
			}
		}

		private void appDeploy_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
		{
			this.m_form.Progress = e.ProgressPercentage;
			switch (e.State)
			{
				case DeploymentProgressState.DownloadingDeploymentInformation:
					this.m_form.State = "1/3 (проверка наличия обновлений)";
					break;
				case
					DeploymentProgressState.DownloadingApplicationInformation:
					this.m_form.State = "2/3 (загрузка информации о приложении)";
					break;
				case DeploymentProgressState.DownloadingApplicationFiles:
					this.m_form.State = "3/3 (загрузка приложения)";
					break;
				default:
					this.m_form.State = "?/3 (неизвестная стадия)";
					break;
			}
		}

		#endregion

		public interface IFormUpdateInstaller
		{
			string ResultUpdate { set; }
			bool Updated { set; }

			string State { set; }

			void Start();
			int Progress { set; }
			void End();
		}
	}
}
