using System.Runtime.Serialization;
using JetBrains.Annotations;
using Toolbox.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Toolbox.Extensions;

namespace Toolbox.Network
{
	/// <summary>
	/// Сервер для обмена сообщениями по TCP/IP в режиме Client/Server.
	/// Обмен происходит в синхронном режиме, но в отдельном потоке
	/// </summary>
	/// <typeparam name="TCommandType">тип ответа от клиента</typeparam>
	/// <typeparam name="TEventType">тип ответа</typeparam>
	public class SyncTCPServer<TCommandType, TEventType> : IDisposable
    where TCommandType : new() where TEventType : new()
  {
		[NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SyncTCPServer<TCommandType, TEventType>));

    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список клиентов
    /// </summary>
    readonly BindingList<SyncTCPMessenger<TCommandType, TEventType>> m_clients =
      new BindingList<SyncTCPMessenger<TCommandType, TEventType>>( );

    /// <summary>
    /// Событие — остановка
    /// </summary>
    [NotNull] readonly ManualResetEvent m_stop_event = new ManualResetEvent( false );

    /// <summary>
    /// Событие — сброс клиента
    /// </summary>
    [NotNull] readonly ManualResetEvent m_drop_event = new ManualResetEvent( false );

    /// <summary>
    /// Слушатель для приёма входящих подключений
    /// </summary>
    TcpListener m_listener;

    /// <summary>
    /// Поток сетевого обмена
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
    public event EventHandler<ParamEventArgs<TCommandType>> ClientHasCommand;

    public event EventHandler<ParamEventArgs<TEventType>> ClientHasEvent;

    /// <summary>
    /// Сообщение о подключении клиента
    /// </summary>
    public event EventHandler<ParamEventArgs<SyncTCPMessenger<TCommandType, TEventType>>> ClientConnected;

    /// <summary>
    /// Сообщение об отключении клиента
    /// </summary>
    public event EventHandler<ParamEventArgs<SyncTCPMessenger<TCommandType, TEventType>>> ClientDisconnected;

    #endregion

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список клиентов
    /// </summary>
    public BindingList<SyncTCPMessenger<TCommandType, TEventType>> Clients
    {
      get { return m_clients; }
    }

    /// <summary>
    /// Времена последних обращений от клиента
    /// </summary>
    Dictionary<SyncTCPMessenger<TCommandType, TEventType>, DateTime> m_timeouts =
      new Dictionary<SyncTCPMessenger<TCommandType, TEventType>, DateTime>( );

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
        m_accept_thread = new Thread( NetworkAcceptThreadProc )
          {
            Name = "SyncTCPServer.NetworkAcceptThreadProc",
            CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
          };
        m_accept_thread.Start( );

        m_thread = new Thread( NetworkThreadProc )
            {
              Name = "SyncTCPServer.NetworkThreadProc",
              CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
            };
        m_thread.Start( );
      }
      catch( Exception ex )
      {
        _log.Error("Start(): exception", ex);
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
        _log.Error("Stop(): exception", ex);
      }
      finally
      {
        if( null != m_thread && m_thread.IsAlive )
          m_thread.Abort( );

        m_clients.Clear( );
      }
    }

    /// <summary>
		/// Посылка запроса клиенту
    /// </summary>
		/// <param name="client">Клиент. Если <code>null</code> — сообщение посылается всем</param>
    /// <param name="dataType">запрос</param>
    public void Send( SyncTCPMessenger<TCommandType, TEventType> client, TCommandType dataType )
    {
      if( null != client )
        client.Send( dataType );
      else
      {
        foreach( SyncTCPMessenger<TCommandType, TEventType> m_client in m_clients )
        {
          if( m_client.Connected )
            m_client.Send( dataType );
        }
      }
    }

    public void Send( SyncTCPMessenger<TCommandType, TEventType> client, TEventType dataType )
    {
      if( null != client )
        client.Send( dataType );
      else
      {
        foreach( SyncTCPMessenger<TCommandType, TEventType> m_client in m_clients )
          m_client.Send( dataType );
      }
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    void NetworkAcceptThreadProc( )
    {
      TCommandType ping = new TCommandType( );
      bool restarting = false;

      while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
      {
        try
        {
          _log.Info("NetworkAcceptThreadProc(): starting to accept connections");

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
              SyncTCPMessenger<TCommandType, TEventType> runner = new SyncTCPMessenger<TCommandType, TEventType>( m_formatter );

              runner.Client = client;
              runner.Connect( );
              runner.OnDisconnected += OnClientDisconnected;

              lock( Clients )
              {
                m_clients.Add( runner );
              }

              if( ClientConnected != null )
                ClientConnected( this, new ParamEventArgs<SyncTCPMessenger<TCommandType, TEventType>>( runner ) );
            }
          }
        }
        catch( Exception ex )
        {
          _log.Error("NetworkAcceptThreadProc(): exception", ex);
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
      TCommandType ping = new TCommandType( );
      bool restarting = false;

      while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
      {
        try
        {
          if( restarting && null != Restarted )
            Restarted( this, EventArgs.Empty );

          while( !m_stop_event.WaitOne( MainLoopSleepTime, false ) )
          {
            foreach( SyncTCPMessenger<TCommandType, TEventType> m_client in m_clients )
            {
              // Проверяем данные у клиентов
              if( !m_client.DataPending )
                continue;

              TCommandType message_cmd;
              TEventType message_event;

              if( !m_client.Recieve( out message_cmd, out message_event ) )
                continue;

              m_timeouts[m_client] = DateTime.Now;

              if( null != message_cmd )
              {
                if( message_cmd.Equals( ping ) )
                  continue;

                if( null != ClientHasCommand )
                  ClientHasCommand( m_client, new ParamEventArgs<TCommandType>( message_cmd ) );
              }

              if( null != message_event )
              {
                if( null != ClientHasEvent )
                  ClientHasEvent( m_client, new ParamEventArgs<TEventType>( message_event ) );
              }
            }

            if( !TrackClientActivity )
              continue;

            DateTime dt = DateTime.Now;

            foreach( KeyValuePair<SyncTCPMessenger<TCommandType, TEventType>, DateTime> pair in m_timeouts )
            {
              //Проверка жизнеспособности
              if( dt - pair.Value > WarningTimeSpan )
                _log.WarnFormat("NetworkThreadProc(): client didn't respond for {0} seconds", WarningTimeSpan.TotalSecondsDecimal());
              else if( ( dt - pair.Value > DisconnectTimeSpan ) && DisconnectInactive )
                pair.Key.Disconnect( );
            }
          }
        }
        catch( Exception ex )
        {
          _log.Error("NetworkThreadProc(): exception", ex);
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
        SyncTCPMessenger<TCommandType, TEventType> client = (SyncTCPMessenger<TCommandType, TEventType>) sender;

        m_clients.Remove( client );

        if( m_clients.Count == 0 )
          m_drop_event.Set( );

        if( ClientDisconnected != null )
          ClientDisconnected( this, new ParamEventArgs<SyncTCPMessenger<TCommandType, TEventType>>( client ) );
      }
      catch( Exception ex )
      {
        _log.Error("OnClientDisconnected(): exception", ex);
      }
    }

    #endregion

    public void Dispose( ) { m_stop_event.Close( ); }
  }
}