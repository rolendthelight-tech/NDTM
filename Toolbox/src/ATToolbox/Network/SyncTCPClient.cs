using System.Runtime.Serialization;


namespace AT.Toolbox.Network
{
  using System;
  using System.Net.Sockets;
  using System.Threading;


  /// <summary>
  /// Клиент для обмена сообщениями по TCP/IP в режиме Client/Server.
  /// Обмен происходит в синхронном режиме, но в отдельном потоке
  /// </summary>
  /// <typeparam name="CommandType">тип команд</typeparam>
  /// /// <typeparam name="EventType">тип сообщений</typeparam>
  public class SyncTCPClient<CommandType, EventType> : IDisposable
    where CommandType : new() where EventType : new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SyncTCPClient<CommandType, EventType>).Name);


    #region Private Variables ---------------------------------------------------------------------------------------------------

    SyncTCPMessenger<CommandType, EventType> m_server_conn;

    /// <summary>
    /// Сообщение об остановке
    /// </summary>
    readonly ManualResetEvent m_stop_event = new ManualResetEvent( false );

    /// <summary>
    /// Поток, в котором происходит обмен
    /// </summary>
    Thread m_thread;

    /// <summary>
    /// Задержка в основном цикле
    /// </summary>
    int m_main_loop_sleep_time = 100;

    IFormatter m_formatter;

    #endregion


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Задержка в основном цикле
    /// </summary>
    public int MainLoopSleepTime
    {
      get { return m_main_loop_sleep_time; }
      set { m_main_loop_sleep_time = value; }
    }

    /// <summary>
    /// Адрес сервера
    /// </summary>
    public string Adress { get; set; }

    /// <summary>
    /// Порт для подключения
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Пытаться ли восстановить подключение при ошибке
    /// </summary>
    public bool KeepAlive { get; set; }

    /// <summary>
    /// Запущен ли сетевой обмен
    /// </summary>
    public bool Running { get; protected set; }

    #endregion


    #region Public Events -------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Сообщение о том, что с сервера пришли данные
    /// </summary>
    public event EventHandler<ParamEventArgs<CommandType>> ServerHasCommand;

    /// <summary>
    /// Сообщение о том, что с сервера пришли данные
    /// </summary>
    public event EventHandler<ParamEventArgs<EventType>> ServerHasEvent;

    #endregion


    public SyncTCPClient( IFormatter Formatter )
    {
      m_formatter = Formatter;
    }

    #region Public Methods ------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Запуск цикла сетевого обмена
    /// </summary>
    public void Start( )
    {
      try
      {
        m_thread = new Thread( ThreadProcess );
        m_thread.Name = "SyncTCPClient.ThreadProcess()";
        m_stop_event.Reset( );
        m_thread.Start( );
      }
      catch( Exception ex )
      {
        Log.Error("Start(): exception", ex );
      }
    }

    /// <summary>
    /// Остановка сетевого обмена
    /// </summary>
    public void Stop( )
    {
      try
      {
        if( !m_thread.IsAlive )
          return;

        m_stop_event.Set( );

        if( null != m_thread )
          m_thread.Join( 2000 );
      }
      catch( Exception ex )
      {
        Log.Error("Stop(): exception", ex);
      }
      finally
      {
        if( null != m_thread && m_thread.IsAlive )
        {
          Log.Warn("Stop(): aborting network thread");
          m_thread.Abort( );
        }
      }
    }

    /// <summary>
    /// Пинг сервера
    /// </summary>
    /// <returns>true если пинг прошёл</returns>
    public bool PingServer( ) { return m_server_conn.Ping( ); }

    /// <summary>
    /// Отправка запроса на сервер
    /// </summary>
    /// <param name="dataType">запрос</param>
    public void Send( CommandType dataType )
    {
      if( Running )
        m_server_conn.Send( dataType );
    }

    public void Send( EventType dataType )
    {
      if( Running )
        m_server_conn.Send( dataType );
    }

    #endregion


    #region Private Methods -----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Цикл сетевого обмена
    /// </summary>
    void ThreadProcess( )
    {
      CommandType ping = new CommandType( ); // для сравнения

      while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
      {
        try
        {
          m_server_conn = new SyncTCPMessenger<CommandType, EventType>(m_formatter);
          m_server_conn.Client = new TcpClient( Adress, Port );

          int read_timeouts = 0;

          m_server_conn.Connect( );

          while( m_server_conn.Connected ) // Пока установлено соединение
          {
            Running = true;

            if( !m_server_conn.DataPending )
            {
              if( m_stop_event.WaitOne( 0, false ) )
                break;

              Thread.Sleep( MainLoopSleepTime );
              read_timeouts++;

              if( read_timeouts > 500 ) // Проверяем, жив ли сервер
              {
                if( !m_server_conn.Ping( ) )
                  throw new NetworkException( "Server is down" );
              }

              continue;
            }

            CommandType message_cmd;
            EventType message_event;

            if( !m_server_conn.Recieve( out message_cmd, out message_event ) )
              continue;

            if( null != message_cmd )
            {
              if( message_cmd.Equals( ping ) )
                continue;

              if( null != ServerHasCommand )
                ServerHasCommand( this, new ParamEventArgs<CommandType>( message_cmd ) );
            }

            if( null != message_event )
            {
              if( null != ServerHasEvent )
                ServerHasEvent( this, new ParamEventArgs<EventType>( message_event ) );
            }
          }
        }
        catch( Exception ex )
        {
          Log.Error("ThreadProcess(): exception", ex);
        }

        m_server_conn.Disconnect( );

        if( !KeepAlive )
          break;
      }

      Running = false;
    }

    #endregion


    public void Dispose( ) { m_stop_event.Close( ); }
  }


  public class NetworkException : Exception
  {
    public NetworkException( string s )
      : base( s ) { }
  }
}