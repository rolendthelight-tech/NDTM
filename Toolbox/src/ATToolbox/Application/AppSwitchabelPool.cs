namespace AT.Toolbox
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Threading;


  /// <summary>
  /// Пул объектов, поддерживающих смену языка на лету
  /// </summary>
  public static class AppSwitchablePool
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

    static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AppSwitchablePool).Name);
    static CultureInfo m_current_culture = new CultureInfo(ApplicationSettings.Instance.DefaultLocale ?? "EN");

    #endregion


    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список объектов
    /// </summary>
    static readonly List<ISwitchableLanguage> m_switchables = new List<ISwitchableLanguage>( );

    /// <summary>
    /// Пул managed-потоков
    /// </summary>
    static readonly Dictionary<string, Thread> m_threads = new Dictionary<string, Thread>();

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

    public static CultureInfo CurrentLocale
    {
      get
      {
        return m_current_culture;
      }
    }

    /// <summary>
    /// Потоки
    /// </summary>
    public static Dictionary<string, Thread> Threads
    {
      get { return m_threads; }
    }

    /// <summary>
    /// Регистрация managed-потока
    /// </summary>
    /// <param name="t">Поток</param>
    public static void RegisterThread(Thread t) { RegisterThread(t.ManagedThreadId.ToString(), t); }

    /// <summary>
    /// Разрегистрация managed-потока
    /// </summary>
    /// <param name="t">Поток</param>
    public static void UnRegisterThread(Thread t) { UnRegisterThread(t.ManagedThreadId.ToString()); }

    /// <summary>
    /// Разрегистрация managed-потока
    /// </summary>
    /// <param name="s"> ID Потокa</param>
    static void UnRegisterThread(string s)
    {
      if (!m_threads.ContainsKey(s))
        m_threads.Remove(s);
    }

    /// <summary>
    /// Регистрация managed-потока
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="t">Поток</param>
    public static void RegisterThread(string id, Thread t)
    {
      if (!m_threads.ContainsKey(id))
        m_threads.Add(id, t);

      t.CurrentCulture = Thread.CurrentThread.CurrentCulture;
      t.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
    }
    
    /// <summary>
    /// Регистрация объекта
    /// </summary>
    /// <param name="l">Объект для регистрации</param>
    public static void RegisterSwitchable( ISwitchableLanguage l )
    {
      m_switchables.Add( l );

      l.Terminating += RemoveSwitchable;
      LanguageChange += l.PerformLocalization;
    }

    /// <summary>
    /// Смена Locale
    /// </summary>
    /// <param name="LocaleName">Имя Новой Locale</param>
    public static void SwitchLocale( string LocaleName ) { SwitchLocale( new CultureInfo( LocaleName ) ); }

    /// <summary>
    /// Смена Locale
    /// </summary>
    /// <param name="i">Новая Locale</param>
    public static void SwitchLocale( CultureInfo i )
    {
      List<string> obslete_threads = new List<string>( );

      foreach( KeyValuePair<string, Thread> pair in Threads )
      {
        try
        {
          pair.Value.CurrentUICulture = i;
        }
        catch (Exception ex)
        {
          Log.Error("SwitchLocale(): exception", ex);
          obslete_threads.Add( pair.Key );
        }
      }

      foreach( string s in obslete_threads )
        Threads.Remove( s );

      LanguageChange( null, EventArgs.Empty );
      m_current_culture = i;
    }

    #endregion


    #region Private Methods -----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Удаление объекта при его уничтожении
    /// </summary>
    /// <param name="sender">Уничтожаемый объект</param>
    /// <param name="e">Нагрузки не несёт</param>
    static void RemoveSwitchable( object sender, EventArgs e )
    {
      ISwitchableLanguage s = sender as ISwitchableLanguage;

      if (s == null)
        return;

      if( m_switchables.Contains( s ) )
        m_switchables.Remove( s );

      LanguageChange -= s.PerformLocalization;
    }

    #endregion
  }
}