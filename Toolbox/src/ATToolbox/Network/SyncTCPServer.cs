using System.Runtime.Serialization;


namespace AT.Toolbox.Network
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Net;
  using System.Net.Sockets;
  using System.Threading;


  /// Сервер для обмена сообщениями по TCP/IP в режиме Client/Server.
  /// Обмен происходит в синхронном режиме, но в отдельном потоке
  /// </summary>
  /// <typeparam name="CommandType">тип ответа от клиента</typeparam>
  public class SyncTCPServer<CommandType, EventType> : IDisposable
    where CommandType : new() where EventType : new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SyncTCPServer<CommandType, EventType>).Name);


    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список клиентов
    /// </summary>
    readonly BindingList<SyncTCPMessenger<CommandType, EventType>> m_clients =
      new BindingList<SyncTCPMessenger<CommandType, EventType>>( );

    /// <summary>
    /// Событие -- остановка
    /// </summary>
    readonly ManualResetEvent m_stop_event = new ManualResetEvent( false );

    /// <summary>
    /// Событие -- сброс клиента
    /// </summary>
    readonly ManualResetEvent m_drop_event = new ManualResetEvent( false );

    /// <summary>
    /// Слушатель для приёма входящих подключений
    /// </summary>
    TcpListener m_listener;

    /// <summary>
    /// Поток сетевго обмена
    /// </summary>
    Thread m_thread;

    Thread m_accept_thread;

    /// <summary>
    /// Задержка в цикле сетевого обмена
    /// </summary>
    int m_main_loop_sleep_time = 100;

    #endregion


    #region Public Events -------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Сообщение о перезапуске обмена
    /// </summary>
    public event EventHandler Restarted;

    /// <summary>
    /// Сообщение об ответе от клиента
    /// </summary>
    public event EventHandler<ParamEventArgs<CommandType>> ClientHasCommand;

    public event EventHandler<ParamEventArgs<EventType>> ClientHasEvent;

    /// <summary>
    /// Сообщение о подключении клиента
    /// </summary>
    public event EventHandler<ParamEventArgs<SyncTCPMessenger<CommandType, EventType>>> ClientConnected;

    /// <summary>
    /// Сообщение об отключении клиента
    /// </summary>
    public event EventHandler<ParamEventArgs<SyncTCPMessenger<CommandType, EventType>>> ClientDisconnected;

    #endregion


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список клиентов
    /// </summary>
    public BindingList<SyncTCPMessenger<CommandType, EventType>> Clients
    {
      get { return m_clients; }
    }

    /// <summary>
    /// Времена последних обращений от клиента
    /// </summary>
    Dictionary<SyncTCPMessenger<CommandType, EventType>, DateTime> m_timeouts =
      new Dictionary<SyncTCPMessenger<CommandType, EventType>, DateTime>( );

    IFormatter m_formatter;

    /// <summary>
    /// Промежуток времени до оповещении о неактивности клиента
    /// </summary>
    public TimeSpan WarningTimeSpan { get; set; }

    /// <summary>
    /// Промежуток времени до отключения неактивного клиента
    /// </summary>
    public TimeSpan DisconnectTimeSpan { get; set; }

    /// <summary>
    /// Отслеживать активность клиентов
    /// </summary>
    public bool TrackClientActivity { get; set; }

    /// <summary>
    /// Отсоединять неактивных клиентов
    /// </summary>
    public bool DisconnectInactive { get; set; }

    /// <summary>
    /// Адрес, используемый для приёма соединений. Для нескольких сетей
    /// </summary>
    public IPAddress IpAdress { get; set; }

    /// <summary>
    /// Порт для приёма соединений
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Перезапускать ли сетевой обмен при ошибке
    /// </summary>
    public bool KeepAlive { get; set; }

    /// <summary>
    /// Задержка в цикле сетевого обмена
    /// </summary>
    public int MainLoopSleepTime
    {
      get { return m_main_loop_sleep_time; }
      set { m_main_loop_sleep_time = value; }
    }

    #endregion


    public SyncTCPServer(IFormatter Formatter)
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
        m_accept_thread = new Thread( NetworkAcceptThreadProc );
        m_accept_thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
        m_accept_thread.Start( );

        m_thread = new Thread( NetworkThreadProc );
        m_thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
        m_thread.Start( );
      }
      catch( Exception ex )
      {
        Log.Error("Start(): exception", ex);
      }
    }

    /// <summary>
    /// Остановка цикла сетевого обмена
    /// </summary>
    public void Stop( )
    {
      try
      {
        m_stop_event.Set( );

        if( null != m_accept_thread )
          m_accept_thread.Join( 2000 );

        if( null != m_thread )
          m_thread.Join( 2000 );

        while( m_clients.Count > 0 )
          m_clients[0].Disconnect( );
      }
      catch( Exception ex )
      {
        Log.Error("Stop(): exception", ex);
      }
      finally
      {
        if( null != m_thread && m_thread.IsAlive )
          m_thread.Abort( );

        m_clients.Clear( );
      }
    }

    /// <summary>
    /// посылка запроса клиенту
    /// </summary>
    /// <param name="client">Клент. Если null -- сообщение посылается всем</param>
    /// <param name="dataType">запрос</param>
    public void Send( SyncTCPMessenger<CommandType, EventType> client, CommandType dataType )
    {
      if( null != client )
        client.Send( dataType );
      else
      {
        foreach( SyncTCPMessenger<CommandType, EventType> m_client in m_clients )
        {
          if( m_client.Connected )
            m_client.Send( dataType );
        }
      }
    }

    public void Send( SyncTCPMessenger<CommandType, EventType> client, EventType dataType )
    {
      if( null != client )
        client.Send( dataType );
      else
      {
        foreach( SyncTCPMessenger<CommandType, EventType> m_client in m_clients )
          m_client.Send( dataType );
      }
    }

    #endregion


    #region Private Methods -----------------------------------------------------------------------------------------------------

    void NetworkAcceptThreadProc( )
    {
      CommandType ping = new CommandType( );
      bool restarting = false;

      while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
      {
        try
        {
          Log.Info("NetworkAcceptThreadProc(): starting to accept connections");

          m_listener = new TcpListener( IpAdress, Port );
          m_listener.Start( );

          if( restarting && null != Restarted )
            Restarted( this, EventArgs.Empty );

          while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
          {
            if( null == m_listener )
              break;

            if( m_listener.Pending( ) )
            {
              //Входящий клиент
              TcpClient client = m_listener.AcceptTcpClient( );
              SyncTCPMessenger<CommandType, EventType> runner = new SyncTCPMessenger<CommandType, EventType>( m_formatter );

              runner.Client = client;
              runner.Connect( );
              runner.OnDisconnected += OnClientDisconnected;

              lock( Clients )
              {
                m_clients.Add( runner );
              }

              if( ClientConnected != null )
                ClientConnected( this, new ParamEventArgs<SyncTCPMessenger<CommandType, EventType>>( runner ) );
            }
          }
        }
        catch( Exception ex )
        {
          Log.Error("NetworkAcceptThreadProc(): exception", ex);
        }

        if( null != m_listener )
          m_listener.Stop( );

        m_listener = null;

        if( !KeepAlive )
          break;

        restarting = true;
      }
    }

    /// <summary>
    /// Цикл сетевого обмена
    /// </summary>
    void NetworkThreadProc( )
    {
      CommandType ping = new CommandType( );
      bool restarting = false;

      while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
      {
        try
        {
          if( restarting && null != Restarted )
            Restarted( this, EventArgs.Empty );

          while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
          {
            foreach( SyncTCPMessenger<CommandType, EventType> m_client in m_clients )
            {
              // Проверяем данные у клиентов
              if( !m_client.DataPending )
                continue;

              CommandType message_cmd;
              EventType message_event;

              if( !m_client.Recieve( out message_cmd, out message_event ) )
                continue;

              m_timeouts[m_client] = DateTime.Now;

              if( null != message_cmd )
              {
                if( message_cmd.Equals( ping ) )
                  continue;

                if( null != ClientHasCommand )
                  ClientHasCommand( m_client, new ParamEventArgs<CommandType>( message_cmd ) );
              }

              if( null != message_event )
              {
                if( null != ClientHasEvent )
                  ClientHasEvent( m_client, new ParamEventArgs<EventType>( message_event ) );
              }
            }

            if( !TrackClientActivity )
              continue;

            DateTime dt = DateTime.Now;

            foreach( KeyValuePair<SyncTCPMessenger<CommandType, EventType>, DateTime> pair in m_timeouts )
            {
              //Проверка жизнеспособности
              if( dt - pair.Value > WarningTimeSpan )
                Log.WarnFormat("NetworkThreadProc(): client didn't respond for {0} seconds", WarningTimeSpan.Seconds);
              else if( ( dt - pair.Value > DisconnectTimeSpan ) && DisconnectInactive )
                pair.Key.Disconnect( );
            }
          }
        }
        catch( Exception ex )
        {
          Log.Error("NetworkThreadProc(): exception", ex);
        }

        if( null != m_listener )
          m_listener.Stop( );

        m_listener = null;

        if( !KeepAlive )
          break;

        restarting = true;
      }
    }

    /// <summary>
    /// Обработка сообщения об отсоединиышемся клиенте
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnClientDisconnected( object sender, EventArgs e )
    {
      try
      {
        SyncTCPMessenger<CommandType, EventType> client = (SyncTCPMessenger<CommandType, EventType>) sender;

        m_clients.Remove( client );

        if( m_clients.Count == 0 )
          m_drop_event.Set( );

        if( ClientDisconnected != null )
          ClientDisconnected( this, new ParamEventArgs<SyncTCPMessenger<CommandType, EventType>>( client ) );
      }
      catch( Exception ex )
      {
        Log.Error("OnClientDisconnected(): exception", ex);
      }
    }

    #endregion


    public void Dispose( ) { m_stop_event.Close( ); }
  }
}