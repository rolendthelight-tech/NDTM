using System;
using System.Deployment.Application;
using System.Reflection;
using Toolbox.Extensions;

namespace Toolbox.GUI.Deployment.ClickOnce
{
	public class UpdateChecker : IDisposable
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof (UpdateChecker));

		private readonly IFormUpdateCheck m_form;
		private readonly ApplicationDeployment m_app_deploy;
		private bool m_disposed;
		private readonly object m_lock = new object();
		private bool m_subscribed;

		public UpdateChecker(IFormUpdateCheck form)
		{
			this.m_disposed = false;
			this.m_subscribed = false;
			this.m_form = form;
			this.m_app_deploy = ApplicationDeployment.CurrentDeployment;
		}

		private void ThrowIfDisposed()
		{
			if (this.m_disposed)
				throw new ObjectDisposedException("UpdateChecker");
		}

		public bool GetUpdateInfo()
		{
			ThrowIfDisposed();

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
				_log.WarnFormat("GetUpdateInfo(): это приложение не ClickOnce");
				return false;
			}
		}

		public void GetUpdateInfoAsync()
		{
			ThrowIfDisposed();

			_log.Debug("GetUpdateInfoAsync(): подготовка к запуску асинхронной проверки наличия обновлений");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("GetUpdateInfoAsync(): запуск асинхронной проверки наличия обновлений");
					Subscribe();
					this.m_app_deploy.CheckForUpdateAsync();
				}
				catch (Exception ex)
				{
					_log.Error("GetUpdateInfoAsync(): ошибка асинхронной проверки наличия обновлений", ex);
					Unsubscribe();
					this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
					this.m_form.End();
				}
			}
			else
			{
				_log.WarnFormat("GetUpdateInfoAsync(): это приложение не ClickOnce");
				this.m_form.ResultGetInfo = "Это приложение не ClickOnce";
			}
		}

		public void GetUpdateInfoAsyncCancel()
		{
			ThrowIfDisposed();

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
				_log.WarnFormat("GetUpdateInfoAsyncCancel(): это приложение не ClickOnce");
			}
		}

		public Version GetCurrentVersion()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.CurrentVersion;
		}

		public Version GetUpdatedVersion()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.UpdatedVersion;
		}

		public Uri GetActivationUri()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.ActivationUri;
		}

		public string GetDataDirectory()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.DataDirectory;
		}

		public bool GetIsFirstRun()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.IsFirstRun;
		}

		public DateTime GetTimeOfLastUpdateCheck()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.TimeOfLastUpdateCheck;
		}

		public Uri GetUpdateLocation()
		{
			ThrowIfDisposed();

			return this.m_app_deploy.UpdateLocation;
		}

		public void Subscribe()
		{
			lock (this.m_lock)
			{
				if (this.m_subscribed)
					throw new InvalidOperationException("m_subscribed");
				else
				{
					this.m_app_deploy.CheckForUpdateCompleted += this.appDeploy_CheckForUpdateCompleted;
					this.m_app_deploy.CheckForUpdateProgressChanged += this.appDeploy_CheckForUpdateProgressChanged;
					this.m_subscribed = true;
				}
			}
		}

		public void Unsubscribe()
		{
			lock (this.m_lock)
			{
				if (!this.m_subscribed)
					throw new InvalidOperationException("¬m_subscribed");
				else
				{
					this.m_app_deploy.CheckForUpdateCompleted -= this.appDeploy_CheckForUpdateCompleted;
					this.m_app_deploy.CheckForUpdateProgressChanged -= this.appDeploy_CheckForUpdateProgressChanged;
					this.m_subscribed = false;
				}
			}
		}

		#region реакция на события

		private void appDeploy_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
		{
			try
			{
				ThrowIfDisposed();

				this.m_form.Progress = 0;

				this.m_form.ResultGetInfo = e.UpdateAvailable ? string.Format("{0}", e.AvailableVersion) : "обновлений нет";
				this.m_form.UpdateAvailable = e.UpdateAvailable && (e.AvailableVersion > GetUpdatedVersion());

				if (e.Error == null)
					_log.DebugFormat("appDeploy_CheckForUpdateCompleted(): асинхронная проверка наличия обновлений завершена без ошибок (обновление доступно: {0}, его версия: \"{1}\", обновление обязательное: {2}, минимальная обязательная версия обновления: \"{3}\", размер обновления: {4}, отменено: {5}, состояние: \"{6}\")", e.UpdateAvailable, e.AvailableVersion, e.IsUpdateRequired, e.MinimumRequiredVersion, e.UpdateSizeBytes, e.Cancelled, e.UserState);
				else
					_log.ErrorFormat(e.Error, "appDeploy_CheckForUpdateCompleted(): асинхронная проверка наличия обновлений завершена с ошибкой (обновление доступно: {0}, его версия: \"{1}\", обновление обязательное: {2}, минимальная обязательная версия обновления: \"{3}\", размер обновления: {4}, отменено: {5}, ошибка: \"{6}\", состояние: \"{7}\")", e.UpdateAvailable, e.AvailableVersion, e.IsUpdateRequired, e.MinimumRequiredVersion, e.UpdateSizeBytes, e.Cancelled, e.Error, e.UserState);
			}
			catch (ObjectDisposedException ex)
			{
				_log.Warn("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
			}
			catch (InvalidOperationException ex)
			{
				const string not_avail = "Update not available.";
				const string not_avail_ru = "Обновление недоступно.";
				if (ex.Message == not_avail || ex.Message == not_avail_ru)
				{
					_log.Warn("appDeploy_CheckForUpdateCompleted(): асинхронная проверка наличия обновлений не обнаружила новой версии", ex);
					this.m_form.ResultGetInfo = "Нет новой версии";
					this.m_form.UpdateAvailable = false;
				}
				else
				{
					_log.Error("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
					this.m_form.ResultGetInfo = string.Format("Ошибка при получении: {0}", ex);
					this.m_form.UpdateAvailable = false;
				}
			}
			catch (TargetInvocationException ex)
			{
				_log.Error("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
				this.m_form.ResultGetInfo = string.Format("Ошибка при получении: {0}", ex);
				this.m_form.UpdateAvailable = false;
			}
			catch (Exception ex)
			{
				_log.Error("appDeploy_CheckForUpdateCompleted(): ошибка асинхронной проверки наличия обновлений", ex);
				this.m_form.ResultGetInfo = string.Format("Ошибка: {0}", ex);
				this.m_form.UpdateAvailable = false;
			}
			finally
			{
				try
				{
					if(this.m_subscribed)
						Unsubscribe();
				}
				catch (Exception ex)
				{
					_log.Error("appDeploy_CheckForUpdateCompleted(): не удалось отписаться от событий", ex);
				}
				this.m_form.End();
			}
		}

		private void appDeploy_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
		{
			try
			{
				ThrowIfDisposed();

				this.m_form.Progress = e.ProgressPercentage;
			}
			catch (ObjectDisposedException ex)
			{
				_log.Warn("appDeploy_CheckForUpdateProgressChanged(): ошибка асинхронной проверки наличия обновлений", ex);
			}
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

		#region Implementation of IDisposable

		public void Dispose()
		{
			this.m_disposed = true;
			if (this.m_subscribed)
				Unsubscribe();
		}

		#endregion
	}
}
