using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Application
{
	/// <summary>
  /// Пул объектов, поддерживающих смену языка на лету
  /// </summary>
  public static class AppSwitchablePool
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

		[NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(AppSwitchablePool));
		[NotNull] private static CultureInfo _currentCulture = new CultureInfo(ApplicationSettings.Instance.DefaultLocale ?? "RU");

    #endregion

    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список объектов
    /// </summary>
    [NotNull] private static readonly List<ISwitchableLanguage> _switchables = new List<ISwitchableLanguage>();

    /// <summary>
    /// Пул managed-потоков
    /// </summary>
		[NotNull] private static readonly Dictionary<string, CultureInfoThreadData> _threads = new Dictionary<string, CultureInfoThreadData>();

    #endregion

    #region Public Events -------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Сообщение о том, что нужно сменить локаль
    /// </summary>
    public static event EventHandler LanguageChange;

    #endregion

    #region Constructors/Dispose ------------------------------------------------------------------------------------------------

    static AppSwitchablePool( ) { LanguageChange += delegate { }; }

    #endregion

    #region Public Methods ------------------------------------------------------------------------------------------------------

		[NotNull]
		public static CultureInfo CurrentLocale
    {
      get
      {
        return _currentCulture;
      }
    }

    /// <summary>
    /// Потоки
    /// </summary>
    [NotNull]
		public static Dictionary<string, CultureInfoThreadData> Threads
    {
      get { return _threads; }
    }

    /// <summary>
    /// Регистрация managed-потока
    /// </summary>
    /// <param name="t">Поток</param>
    public static void RegisterThread([NotNull] Thread t)
    {
	    if (t == null) throw new ArgumentNullException("t");

	    RegisterThread(t.ManagedThreadId.ToString(), t);
    }

		/// <summary>
    /// Разрегистрация managed-потока
    /// </summary>
    /// <param name="t">Поток</param>
    public static void UnRegisterThread([NotNull] Thread t)
		{
			if (t == null) throw new ArgumentNullException("t");

			UnRegisterThread(t.ManagedThreadId.ToString());
		}

		/// <summary>
    /// Разрегистрация managed-потока
    /// </summary>
		/// <param name="s">Идентификатор потока</param>
    static void UnRegisterThread([NotNull] string s)
    {
			if (s == null) throw new ArgumentNullException("s");

			CultureInfoThreadData data;
			if (_threads.TryGetValue(s, out data))
			{
				data.Revert();
	      _threads.Remove(s);
      }
    }

    /// <summary>
    /// Регистрация managed-потока
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="t">Поток</param>
    public static void RegisterThread([NotNull] string id, [NotNull] Thread t)
    {
	    if (id == null) throw new ArgumentNullException("id");
	    if (t == null) throw new ArgumentNullException("t");

	    if (!_threads.ContainsKey(id))
				_threads.Add(id, new CultureInfoThreadData(t));

			t.CurrentCulture = _currentCulture;
			t.CurrentUICulture = _currentCulture;
    }

    /// <summary>
    /// Регистрация объекта
    /// </summary>
    /// <param name="l">Объект для регистрации</param>
    public static void RegisterSwitchable([NotNull] ISwitchableLanguage l )
    {
	    if (l == null) throw new ArgumentNullException("l");

	    _switchables.Add( l );

      l.Terminating += RemoveSwitchable;
      LanguageChange += l.PerformLocalization;
    }

    /// <summary>
    /// Смена Locale
    /// </summary>
    /// <param name="localeName">Имя Новой Locale</param>
    public static void SwitchLocale([NotNull] string localeName )
    {
	    if (localeName == null) throw new ArgumentNullException("localeName");

	    SwitchLocale( new CultureInfo( localeName ) );
    }

		/// <summary>
    /// Смена Locale
    /// </summary>
    /// <param name="i">Новая Locale</param>
    public static void SwitchLocale([NotNull] CultureInfo i )
    {
			if (i == null) throw new ArgumentNullException("i");

			var obsolete_threads = new List<string>( );

			foreach (KeyValuePair<string, CultureInfoThreadData> pair in _threads)
      {
        try
        {
          pair.Value.Thread.CurrentUICulture = i;
        }
        catch (Exception ex)
        {
          _log.Error("SwitchLocale(): exception", ex);
          obsolete_threads.Add( pair.Key );
        }
      }

      foreach( string s in obsolete_threads )
        Threads.Remove( s );

			EnumExtensions.ClearEnumLabels();

      LanguageChange( null, EventArgs.Empty );
      _currentCulture = i;
    }

		/// <summary>
		/// Регистрация текущего managed-потока (для разрегистрации вызывается Dispose в том же потоке)
		/// </summary>
		[NotNull]
		public static IDisposable GetRegistrationCurrentThread()
		{
			return new RegistrationThread();
		}

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Удаление объекта при его уничтожении
    /// </summary>
    /// <param name="sender">Уничтожаемый объект</param>
    /// <param name="e">Нагрузки не несёт</param>
    static void RemoveSwitchable([NotNull] object sender, EventArgs e )
    {
	    if (sender == null) throw new ArgumentNullException("sender");

	    ISwitchableLanguage s = sender as ISwitchableLanguage;

      if (s == null)
        return;

      if( _switchables.Contains( s ) )
        _switchables.Remove( s );

      LanguageChange -= s.PerformLocalization;
    }

    #endregion

		#region Public Classes --------------------------------------------------------------------------------------------

		public class CultureInfoThreadData
		{
			[NotNull] private readonly Thread m_thread;
			[NotNull] private readonly CultureInfo m_culture;
			[NotNull] private readonly CultureInfo m_ui_culture;

			public CultureInfoThreadData([NotNull] Thread thread)
			{
				if (thread == null) throw new ArgumentNullException("thread");

				m_thread = thread;
				m_culture = m_thread.CurrentCulture;
				m_ui_culture = m_thread.CurrentUICulture;
			}

			[NotNull]
			public Thread Thread
			{
				get { return m_thread; }
			}

			[NotNull]
			public CultureInfo Culture
			{
				get { return m_culture; }
			}

			[NotNull]
			public CultureInfo UICulture
			{
				get { return m_ui_culture; }
			}

			public void Revert()
			{
				m_thread.CurrentCulture = m_culture;
				m_thread.CurrentUICulture = m_ui_culture;
			}
		}

		#endregion

		#region Private Classes -------------------------------------------------------------------------------------------

		private class RegistrationThread : IDisposable
		{
			private readonly int m_create_tid = Thread.CurrentThread.ManagedThreadId;
			private bool m_disposed;

			public RegistrationThread()
			{
				AppSwitchablePool.RegisterThread(Thread.CurrentThread);
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				if (m_create_tid != Thread.CurrentThread.ManagedThreadId)
					throw new InvalidOperationException("Dispose() должен вызываться в том же потоке, что и конструктор");

				if (!m_disposed)
					AppSwitchablePool.UnRegisterThread(Thread.CurrentThread);

				m_disposed = true;
			}

			#endregion
		}

		#endregion

	}

	public interface IThreadDataReference : IDisposable
	{
		/// <summary>
		/// Временно меняет языковые настройки текущего потока (до вызова <see cref="ThreadDataReference.Dispose"/>).
		/// Результат обязательно нужно брать в блок <code>using</code> вида <code>using(var reg = threadDataReference.GetRegistration())
		/// {
		///		reg.Open();
		///		…
		/// }</code>
		/// </summary>
		/// <returns></returns>
		[NotNull]
		ThreadDataReference.IRegistrationThread GetRegistration();

		/// <summary>
		/// Дополняет указанный метод запомненными настройками.
		/// </summary>
		/// <param name="action">Метод.</param>
		/// <returns>Дополненный метод.</returns>
		[NotNull]
		Action WithSettings([NotNull] Action action);

		/// <summary>
		/// Вызывает указанный метод с запомненными настройками.
		/// </summary>
		/// <param name="action">Метод.</param>
		void RunWithSettings([NotNull] Action action);

		/// <summary>
		/// Дополняет указанный метод запомненными настройками.
		/// </summary>
		/// <typeparam name="TResult">Тип результата.</typeparam>
		/// <param name="function">Метод.</param>
		/// <returns>Дополненный метод.</returns>
		Func<TResult> WithSettings<TResult>([NotNull] Func<TResult> function);

		/// <summary>
		/// Вызывает указанный метод с запомненными настройками.
		/// </summary>
		/// <typeparam name="TResult">Тип результата.</typeparam>
		/// <param name="function">Метод.</param>
		/// <returns>Результат.</returns>
		TResult RunWithSettings<TResult>([NotNull] Func<TResult> function);
	}

	/// <summary>
	/// Запоминает языковые настройки потока для использования в других потоках
	/// </summary>
	public static class ThreadDataReference
	{
		[NotNull]
		public static IThreadDataReference CreateCultureInfoThreadDataReference()
		{
			return new CultureInfoThreadDataReference();
		}

		public interface IRegistrationThread : IDisposable
		{
			void Open();
		}

		private class CultureInfoThreadDataReference : IThreadDataReference
		{
			[NotNull]
			private readonly AppSwitchablePool.CultureInfoThreadData m_data;
			private volatile bool m_disposed;

			public CultureInfoThreadDataReference()
			{
				this.m_data = new AppSwitchablePool.CultureInfoThreadData(Thread.CurrentThread);
				Thread.MemoryBarrier();
			}

			[NotNull]
			public IRegistrationThread GetRegistration()
			{
				ThrowIfDisposed();

				return new CultureInfoRegistrationThread(this.m_data);
			}

			[NotNull]
			public Action WithSettings([NotNull] Action action)
			{
				if (action == null) throw new ArgumentNullException("action");
				ThrowIfDisposed();

				return () => RunWithSettings(action);
			}

			public void RunWithSettings([NotNull] Action action)
			{
				if (action == null) throw new ArgumentNullException("action");
				ThrowIfDisposed();

				using (var reg = GetRegistration())
				{
					reg.Open();
					action();
				}
			}

			public Func<TResult> WithSettings<TResult>([NotNull] Func<TResult> function)
			{
				if (function == null) throw new ArgumentNullException("function");
				ThrowIfDisposed();

				return () => RunWithSettings(function);
			}

			public TResult RunWithSettings<TResult>([NotNull] Func<TResult> function)
			{
				if (function == null) throw new ArgumentNullException("function");
				ThrowIfDisposed();

				using (var reg = GetRegistration())
				{
					reg.Open();
					return function();
				}
			}

			#region Implementation of IDisposable

			public void Dispose()
			{
				if (Thread.CurrentThread.ManagedThreadId != this.m_data.Thread.ManagedThreadId)
					throw new InvalidOperationException("Dispose() должен вызываться в том же потоке, что и конструктор");

				Thread.MemoryBarrier();
				this.m_disposed = true;
				Thread.MemoryBarrier();
			}

			private void ThrowIfDisposed()
			{
				Thread.MemoryBarrier();
				if (this.m_disposed)
					throw new ObjectDisposedException(this.GetType().Name);
			}

			#endregion

			private class CultureInfoRegistrationThread : IRegistrationThread
			{
				[NotNull] private readonly AppSwitchablePool.CultureInfoThreadData m_new_data;
				[NotNull] private readonly AppSwitchablePool.CultureInfoThreadData m_data;
				private bool m_disposed;

				public CultureInfoRegistrationThread([NotNull] AppSwitchablePool.CultureInfoThreadData threadData)
				{
					if (threadData == null) throw new ArgumentNullException("threadData");

					this.m_new_data = threadData;
					this.m_data = new AppSwitchablePool.CultureInfoThreadData(Thread.CurrentThread);
				}

				public void Open()
				{
					if (Thread.CurrentThread.ManagedThreadId != this.m_data.Thread.ManagedThreadId)
						throw new InvalidOperationException("Open() должен вызываться в том же потоке, что и конструктор");

					ThrowIfDisposed();

					this.m_data.Thread.CurrentCulture = this.m_new_data.Culture;
					this.m_data.Thread.CurrentUICulture = this.m_new_data.UICulture;
				}

				#region Implementation of IDisposable

				public void Dispose()
				{
					if (Thread.CurrentThread.ManagedThreadId != this.m_data.Thread.ManagedThreadId)
						throw new InvalidOperationException("Dispose() должен вызываться в том же потоке, что и конструктор");

					this.m_disposed = true;
					this.m_data.Revert();
				}

				private void ThrowIfDisposed()
				{
					if (this.m_disposed)
						throw new ObjectDisposedException(this.GetType().Name);
				}

				#endregion
			}
		}
	}
}