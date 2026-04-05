using System;
using System.Deployment.Application;
using System.Reflection;

namespace AT.Toolbox.Network.ClickOnce
{
	public class UpdateChecker
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof (UpdateChecker));

		private readonly IFormUpdateCheck m_form;
		private readonly ApplicationDeployment m_app_deploy;

		public UpdateChecker(IFormUpdateCheck form)
		{
			this.m_form = form;
			this.m_app_deploy = ApplicationDeployment.CurrentDeployment;
		}

		public bool GetUpdateInfo()
		{
			_log.Debug("GetUpdateInfo(): подготовка к запуску проверки наличия обновлений");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("GetUpdateInfo(): запуск проверки наличия обновлений");
					var uci = this.m_app_deploy.CheckForDetailedUpdate();
					this.m_form.ResultGetInfo = uci.UpdateAvailable ? string.Format("{0}", uci.AvailableVersion) : "обновлений нет";
					_log.DebugFormat("GetUpdateInfo(): проверка наличия обновлений завершена (обновление доступно: {0}, его версия: \"{1}\", обновление обязательное: {2}, минимальная обязательная версия обновления: \"{3}\", размер обновления: {4})", uci.UpdateAvailable, uci.AvailableVersion, uci.IsUpdateRequired, uci.MinimumRequiredVersion, uci.UpdateSizeBytes);
					return uci.UpdateAvailable;
				}
				catch (Exception ex)
				{
					_log.Error("GetUpdateInfo(): ошибка проверки наличия обновлений", ex);
					this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
					return false;
				}
				finally
				{
					this.m_form.End();
				}
			}
			else
			{
				_log.Warn("GetUpdateInfo(): это приложение не ClickOnce");
				return false;
			}
		}

		public void GetUpdateInfoAsync()
		{
			_log.Debug("GetUpdateInfoAsync(): подготовка к запуску асинхронной проверки наличия обновлений");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("GetUpdateInfoAsync(): запуск асинхронной проверки наличия обновлений");
					this.m_app_deploy.CheckForUpdateCompleted += this.appDeploy_CheckForUpdateCompleted;
					this.m_app_deploy.CheckForUpdateProgressChanged += this.appDeploy_CheckForUpdateProgressChanged;
					this.m_app_deploy.CheckForUpdateAsync();
				}
				catch (Exception ex)
				{
					_log.Error("GetUpdateInfoAsync(): ошибка асинхронной проверки наличия обновлений", ex);
					this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
				}
			}
			else
			{
				_log.Warn("GetUpdateInfoAsync(): это приложение не ClickOnce");
				this.m_form.ResultGetInfo = "Это приложение не ClickOnce";
			}
		}

		public void GetUpdateInfoAsyncCancel()
		{
			_log.Debug("GetUpdateInfoAsyncCancel(): подготовка к прерыванию асинхронной проверки наличия обновлений");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				try
				{
					_log.Debug("GetUpdateInfoAsyncCancel(): прерывание асинхронной проверки наличия обновлений");
					this.m_app_deploy.CheckForUpdateAsyncCancel();
				}
				catch (Exception ex)
				{
					_log.Error("GetUpdateInfoAsyncCancel(): ошибка прерывания асинхронной проверки наличия обновлений", ex);
					this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
				}
			}
			else
			{
				_log.Warn("GetUpdateInfoAsyncCancel(): это приложение не ClickOnce");
			}
		}

		public Version GetCurrentVersion()
		{
			return this.m_app_deploy.CurrentVersion;
		}

		public Version GetUpdatedVersion()
		{
			return this.m_app_deploy.UpdatedVersion;
		}

		#region реакция на события

		private void appDeploy_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
		{
			try
			{
				this.m_form.Progress = 0;

				this.m_form.ResultGetInfo = e.UpdateAvailable ? string.Format("{0}", e.AvailableVersion) : "обновлений нет";

				_log.Debug(string.Format("appDeploy_CheckForUpdateCompleted(): асинхронная проверка наличия обновлений завершена (обновление доступно: {0}, его версия: \"{1}\", обновление обязательное: {2}, минимальная обязательная версия обновления: \"{3}\", размер обновления: {4}, отменено: {5}, ошибка: \"{6}\", состояние: \"{7}\")", e.UpdateAvailable, e.AvailableVersion, e.IsUpdateRequired, e.MinimumRequiredVersion, e.UpdateSizeBytes, e.Cancelled, e.Error, e.UserState), e.Error);
			}
			catch(TargetInvocationException ex)
			{
				_log.Error("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
				this.m_form.ResultGetInfo = string.Format("Ошибка при получении: {0}", ex);
			}
			catch (Exception ex)
			{
				_log.Error("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
				this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
			}
			finally
			{
				try
				{
					ApplicationDeployment ad = (ApplicationDeployment) sender;
					ad.CheckForUpdateCompleted -= this.appDeploy_CheckForUpdateCompleted;
					ad.CheckForUpdateProgressChanged -= this.appDeploy_CheckForUpdateProgressChanged;
				}
				catch (Exception ex)
				{
					_log.Error("appDeploy_CheckForUpdateCompleted(): не удалось отписаться от событий", ex);
				}
				finally
				{
					this.m_form.UpdateAvailable = e.UpdateAvailable && (e.AvailableVersion > GetUpdatedVersion());
				}
				this.m_form.End();
			}
		}

		private void appDeploy_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
		{
			this.m_form.Progress = e.ProgressPercentage;
		}

		#endregion

		public interface IFormUpdateCheck
		{
			string ResultGetInfo { set; }
			bool UpdateAvailable { set; }

			void Start();
			int Progress { set; }
			void End();
		}
	}
}
