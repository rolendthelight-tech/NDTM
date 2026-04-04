using System.Runtime.Serialization;



namespace AT.Toolbox.Network
{
  using System;
  using System.Net.Sockets;
  using System.Runtime.Serialization.Formatters.Binary;


  /// <summary>
  /// Класс для синхронного обмена сообщениями при помощи TCP Sockets
  /// </summary>
  /// <typeparam name="CommandType">тип ответа</typeparam>
  /// <typeparam name="EventType">тип ответа</typeparam>
  public class SyncTCPMessenger<CommandType, EventType> : Messenger<CommandType, EventType>
    where CommandType : new() where EventType : new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SyncTCPMessenger<CommandType, EventType>).Name);


    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Клиент
    /// </summary>
    TcpClient m_client;

    /// <summary>
    /// Поток для обмена
    /// </summary>
    NetworkStream m_stream;

    public SyncTCPMessenger( IFormatter Formatter )
      : base( Formatter ) {}

    public string Name
    {
      get
      {
        string s = Client.Client.RemoteEndPoint.ToString( );
        int a = s.IndexOf( ':' );

        return -1 != a ? s.Substring( 0, a ) : s;
      }
    }

    #endregion


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Клиент
    /// </summary>
    public TcpClient Client
    {
      get { return m_client; }
      set { m_client = value; }
    }

    /// <summary>
    /// Установлено ли соединение
    /// </summary>
    public override bool Connected
    {
      get
      {
        lock( m_client )
        {
          return m_client.Connected;
        }
      }
    }

    /// <summary>
    /// Есть ли данные в канале
    /// при ошибке соединение разрывается
    /// </summary>
    public override bool DataPending
    {
      get
      {
        try
        {
          lock( m_client )
          {
            return m_stream.DataAvailable;
          }
        }
        catch( Exception ex )
        {
          Log.Error("DataPending.get(): exception", ex );
          Disconnect( );
          return false;
        }
      }
    }

    #endregion


    #region Public Methods ------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Соединяется с удалённой точкой
    /// </summary>
    public override void Connect( )
    {
      try
      {
        m_formatter = new BinaryFormatter( );
        m_stream = m_client.GetStream( );
        m_stream.ReadTimeout = 10000;
        m_stream.WriteTimeout = 10000;

        if( null != OnConnected )
          OnConnected( this, EventArgs.Empty );

        Log.Info("Connect(): connected to : " + m_client.Client.RemoteEndPoint);
      }
      catch( Exception ex )
      {
        Log.Error("Connect(): exception", ex);
      }
    }

    /// <summary>
    /// Разрывает соединенение
    /// </summary>
    public override void Disconnect( )
    {
      try
      {
        if( null == m_client )
          return;

        lock( m_client )
        {
          m_stream.Close( );
          m_client.Close( );

          m_formatter = null;
          m_stream = null;
          m_client = null;
        }

        Log.Info("Disconnect(): disconnected");
      }
      catch( Exception ex )
      {
        Log.Error("Disconnect(): exception", ex);
      }
      finally
      {
        if( null != OnDisconnected )
          OnDisconnected( this, EventArgs.Empty );
      }
    }

    /// <summary>
    /// Получает данные от удалённой точки
    /// при ошибке соединение разрывается
    /// </summary>
    /// <returns>true если данные приняты успешно</returns>
    public override bool Recieve( out CommandType command, out EventType evt )
    {
      command = default( CommandType );
      evt = default( EventType );

      try
      {
        object o = null;

        lock( m_client )
        {
          o = m_formatter.Deserialize( m_stream );
        }

        if( o is CommandType )
          command = (CommandType) o;
        else if( o is EventType )
          evt = (EventType) o;
      }
      catch( Exception ex )
      {
        Log.Error("Receive(): exception", ex);
        Disconnect( );
        return false;
      }

      return true;
    }

    /// <summary>
    /// посылает запрос на удалённую точку 
    /// при ошибке соединение разрывается
    /// </summary>
    /// <param name="data">данные запроса</param>
    /// <returns>true если данные переданы успешно</returns>
    public override bool Send( CommandType data )
    {
      try
      {
        if( null == m_stream )
          return false;

        lock( m_client )
        {
          m_formatter.Serialize( m_stream, data );
          m_stream.Flush( );
        }
      }
      catch( Exception ex )
      {
        Log.Error("Send(CommandType): exception", ex);
        Disconnect( );
        return false;
      }

      return true;
    }

    public override bool Send( EventType data )
    {
      try
      {
        lock( m_client )
        {
          m_formatter.Serialize( m_stream, data );
          m_stream.Flush( );
        }
      }
      catch( Exception ex )
      {
        Log.Error("Send(EventType): exception", ex);
        Disconnect( );
        return false;
      }

      return true;
    }

    /// <summary>
    /// Пингует удалённую точку
    /// при ошибке соединение разрывается
    /// </summary>
    /// <returns>true если ping прошёл корректно</returns>
    public bool Ping( ) { return Send( new CommandType( ) ); }

    #endregion


    public override event EventHandler OnDisconnected;

    public override event EventHandler OnConnected;
  }
}