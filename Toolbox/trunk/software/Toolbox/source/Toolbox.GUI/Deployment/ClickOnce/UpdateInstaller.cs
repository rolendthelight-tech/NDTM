using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Reflection;
using Toolbox.Extensions;

namespace Toolbox.GUI.Deployment.ClickOnce
{
	public class UpdateInstaller : IDisposable
	{
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(UpdateInstaller));

		private readonly IFormUpdateInstaller m_form;
		private readonly ApplicationDeployment m_app_deploy;
		private bool m_disposed;
		private readonly object m_lock = new object();
		private bool m_subscribed;

		public UpdateInstaller(IFormUpdateInstaller form)
		{
			this.m_disposed = false;
			this.m_subscribed = false;
			this.m_form = form;
			this.m_app_deploy = ApplicationDeployment.CurrentDeployment;
		}

		private void ThrowIfDisposed()
		{
			if (this.m_disposed)
				throw new ObjectDisposedException("UpdateInstaller");
		}

		public bool Update()
		{
			ThrowIfDisposed();

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
				_log.WarnFormat("Update(): это приложение не ClickOnce");
				return false;
			}
		}

		public void UpdateAsync()
		{
			ThrowIfDisposed();

			_log.Debug("UpdateAsync(): подготовка к запуску асинхронного обновления");
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				this.m_form.Start();
				try
				{
					_log.Debug("UpdateAsync(): запуск асинхронного обновления");
					Subscribe();
					this.m_app_deploy.UpdateAsync();
				}
				catch (Exception ex)
				{
					_log.Error("UpdateAsync(): ошибка асинхронного обновления", ex);
					Unsubscribe();
					this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
					this.m_form.End();
				}
			}
			else
			{
				_log.WarnFormat("UpdateAsync(): это приложение не ClickOnce");
				this.m_form.ResultUpdate = "Это приложение не ClickOnce";
			}
		}

		public void UpdateAsyncCancel()
		{
			ThrowIfDisposed();

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
				_log.WarnFormat("UpdateAsyncCancel(): это приложение не ClickOnce");
			}
		}

		private void Subscribe()
		{
			lock (this.m_lock)
			{
				if (this.m_subscribed)
					throw new InvalidOperationException("m_subscribed");
				else
				{
					this.m_app_deploy.UpdateCompleted += this.appDeploy_UpdateCompleted;
					this.m_app_deploy.UpdateProgressChanged += this.appDeploy_UpdateProgressChanged;
					this.m_subscribed = true;
				}
			}
		}

		private void Unsubscribe()
		{
			lock (this.m_lock)
			{
				if (!this.m_subscribed)
					throw new InvalidOperationException("¬m_subscribed");
				else
				{
					this.m_app_deploy.UpdateCompleted -= this.appDeploy_UpdateCompleted;
					this.m_app_deploy.UpdateProgressChanged -= this.appDeploy_UpdateProgressChanged;
					this.m_subscribed = false;
				}
			}
		}

		#region реакция на события

		private void appDeploy_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
		{
			try
			{
				ThrowIfDisposed();

				this.m_form.Progress = 0;

				this.m_form.ResultUpdate =
					e.Error == null
						? e.Cancelled
								? "Отменено"
								: string.Format("{0}", this.m_app_deploy.UpdatedVersion)
						: string.Format("Ошибка: {0}", e.Error);
				this.m_form.State = "Готово";
				this.m_form.Updated = e.Error == null && !e.Cancelled && this.m_app_deploy.CurrentVersion != this.m_app_deploy.UpdatedVersion;

				if (e.Error == null)
					_log.DebugFormat("appDeploy_UpdateCompleted(): асинхронное обновление завершено без ошибок (отменено: {0}, состояние: \"{1}\")", e.Cancelled, e.UserState);
				else
					_log.ErrorFormat(e.Error, "appDeploy_UpdateCompleted(): асинхронное обновление завершено с ошибкой (отменено: {0}, ошибка: \"{1}\", состояние: \"{2}\")", e.Cancelled, e.Error, e.UserState);
			}
			catch (ObjectDisposedException ex)
			{
				_log.Warn("appDeploy_UpdateCompleted(): ошибка асинхронного обновления", ex);
			}
			catch (TargetInvocationException ex)
			{
				_log.Error("appDeploy_UpdateCompleted(): ошибка асинхронного обновления", ex);
				this.m_form.ResultUpdate = string.Format("Ошибка при получении: {0}", ex);
				this.m_form.Updated = false;
			}
			catch (Exception ex)
			{
				_log.Error("appDeploy_UpdateCompleted(): ошибка асинхронного обновления", ex);
				this.m_form.ResultUpdate = string.Format("Ошибка: {0}", ex);
				this.m_form.Updated = false;
			}
			finally
			{
				try
				{
					if (this.m_subscribed)
						Unsubscribe();
				}
				catch (Exception ex)
				{
					_log.Error("appDeploy_UpdateCompleted(): не удалось отписаться от событий", ex);
				}
				this.m_form.End();
			}
		}

		private void appDeploy_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
		{
			try
			{
				ThrowIfDisposed();

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
						if (e.ProgressPercentage != 100)
							this.m_form.State = "3/3 (загрузка приложения)";
						else
							this.m_form.State = "²²∕₇/3 (заключительные тормоза)";
						break;
					default:
						this.m_form.State = "?/3 (неизвестная стадия)";
						break;
				}
			}
			catch (ObjectDisposedException ex)
			{
				_log.Warn("appDeploy_UpdateProgressChanged(): ошибка асинхронного обновления", ex);
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
