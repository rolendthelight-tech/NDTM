using System.Runtime.Serialization;


namespace AT.Toolbox.Network
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;
  using System.Net.Mail;
  using System.Net.Security;
  using System.Net.Sockets;
  using System.Text;

  //using Log;


  public class PopSmtpMessenger<CommandType, EventType> : Messenger<CommandType, EventType>, IDisposable
    where CommandType : new() where EventType : new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(PopSmtpMessenger<CommandType, EventType>).Name);


    #region Settings ------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Настройки
    /// </summary>
    public class Settings
    {
      public string SmtpServer;

      public int SmtpPort;

      public bool SmtpUseSSL;

      public string PopServer;

      public int PopPort;

      public bool PopUseSSL;

      public string Username;

      public string Password;

      public string OutboxRecipient;

      public string SmtpUsername;

      public string SmtpPassword;

      public string SmtpFrom;

      public string RemoteAdress;

      public Settings( ) { }

      public event EventHandler PreferencesChanged;

      protected void FireChanged( )
      {
        if( null != PreferencesChanged )
          PreferencesChanged( this, EventArgs.Empty );
      }
    }


    /// <summary>
    /// Настройки 
    /// </summary>
    readonly Settings m_settings = new Settings( );

    public PopSmtpMessenger( IFormatter Formatter )
      : base( Formatter ) {}

    /// <summary>
    /// Настройки 
    /// </summary>
    public Settings Preferences
    {
      get { return m_settings; }
    }

    #endregion


    TcpClient m_pop_connection;

    Stream m_pop_stream;

    StreamReader m_pop_stream_reader;

    bool m_pop_connected;

    public override void Connect( )
    {
      try
      {
        // Присоединяемся к серверу
        m_pop_connection = new TcpClient( Preferences.PopServer, Preferences.PopPort );

        // Создаём поток ( SSL или обычный )
        if( Preferences.PopUseSSL )
        {
          m_pop_stream = new SslStream( m_pop_connection.GetStream( ), false );
          ( (SslStream) m_pop_stream ).AuthenticateAsClient( Preferences.PopServer );
        }
        else
          m_pop_stream = m_pop_connection.GetStream( );

        // POP работает только через ASCII -- перекодируем потом
        m_pop_stream_reader = new StreamReader( m_pop_stream, Encoding.ASCII );

        // Отправляем логин и пароль
        string responce;

        if( !ExecutePopCommand( "USER " + Preferences.Username, out responce ) )
          throw new MailException( "Wrong User" );

        if( !ExecutePopCommand( "PASS " + Preferences.Password, out responce ) )
          throw new MailException( "Wrong Passwod" );

        m_pop_connected = true;

        if (this.OnConnected != null)
        {
          this.OnConnected(this, EventArgs.Empty);
        }
      }
      catch( Exception ex )
      {
        Log.Error("Connect(): exception", ex );
      }
    }

    public override void Disconnect( )
    {
      try
      {
        // Отправляем команду выход
        string responce;

        ExecutePopCommand( "QUIT", out responce );

        m_pop_stream_reader.Close( );
        m_pop_stream.Close( );
        m_pop_connection.Close( );

        if (this.OnDisconnected != null)
        {
          this.OnDisconnected(this, EventArgs.Empty);
        }
      }
      catch( Exception ex )
      {
        Log.Error("Disconnect(): exception", ex );
      }
      finally
      {
        m_pop_connected = false;
      }
    }

    public override bool Connected
    {
      get { return m_pop_connected; }
    }

    public override bool DataPending
    {
      get { return true; }
    }

    public override bool Recieve( out CommandType cmd, out EventType evt )
    {
      cmd = default( CommandType );
      evt = default( EventType );

      List<int> ids;
      GetEmailIds( out ids );

      Log.Info("Receive(): email count : " + ids.Count );

      foreach( int i in ids )
      {
        string mail;
        GetRawEmail( i, out mail );
        //DeleteEmail(i);

        EmailMessage msg;

        if( ParseEmail( mail, out msg ) )
          PostProcessMessage( msg, out cmd, out evt );
      }

      return true;
    }

    protected virtual void PostProcessMessage( EmailMessage msg, out CommandType cmd, out EventType evt )
    {
      cmd = default( CommandType );
      evt = default( EventType );

      string MailPath = Path.Combine( ApplicationSettings.Instance.TempPath, "Mail" );

      if( !Directory.Exists( MailPath ) )
        Directory.CreateDirectory( MailPath );

      foreach( KeyValuePair<string, string> pair in msg.Contents )
      {
        string val = pair.Value;

        foreach( string s in msg.Boundaries )
        {
          if( pair.Value.Contains( s + "--\r\n." ) )
            val = val.Replace( s + "--\r\n.", "" );

          if( pair.Value.Contains( s ) )
            val = val.Replace( s, "" );
        }

        if( pair.Key == "HTML" || pair.Key == "PLAIN" )
          SaveMessage( val, Path.Combine( MailPath, pair.Key ) );
        else
          SaveBinary( Path.Combine( MailPath, pair.Key ), val );
      }
    }

    void SaveBinary( string key, string val )
    {
      val = val.Trim( new char[] {'\r', '\n'} );
      val = val.Replace( "\r\n", "" );
      byte[] data = Convert.FromBase64String( val );

      FileStream fs = null;

      try
      {
        fs = new FileStream( key, FileMode.Create, FileAccess.Write, FileShare.None );

        using( BinaryWriter writer = new BinaryWriter( fs ) )
          writer.Write( data );

        fs.Close( );
      }
      catch( Exception ex )
      {
        Log.Error("SaveBinary(): exception", ex );
      }
    }

    void SaveMessage( string val, string key )
    {
      FileStream fs = null;

      try
      {
        string ext;

        if( key == "PLAIN" )
          ext = ".txt";
        else
          ext = ".html";

        fs = new FileStream( "messge" + ext, FileMode.Create, FileAccess.Write, FileShare.None );

        using( StreamWriter writer = new StreamWriter( fs ) )
          writer.Write( val );

        fs.Close( );
      }
      catch( Exception ex )
      {
        Log.Error("SaveMessage(): exception", ex );
      }
    }

    public override bool Send( CommandType dataType )
    {
      MailMessage mail = new MailMessage( );
      SmtpClient SmtpServer = new SmtpClient( );
      SmtpServer.Credentials = new NetworkCredential( Preferences.SmtpUsername, Preferences.SmtpPassword );
      SmtpServer.Port = Preferences.SmtpPort;
      SmtpServer.Host = Preferences.SmtpServer;
      SmtpServer.EnableSsl = Preferences.SmtpUseSSL;

      try
      {
        mail.From = new MailAddress( Preferences.SmtpFrom, "", Encoding.UTF8 );
        mail.To.Add( Preferences.RemoteAdress );

        string att = SaveCommand( dataType );
        mail.Attachments.Add( new Attachment( att ) );

        SmtpServer.Send( mail );
      }
      catch( Exception ex )
      {
        Log.Error("Send(): exception", ex );
      }

      return true;
    }

    public override bool Send( EventType dataType ) { return true; }

    public override event EventHandler OnDisconnected;

    public override event EventHandler OnConnected;

    const string CRLF = "\r\n";

    bool ExecutePopCommand( string command, out string response )
    {
      try
      {
        //Отсылка команды
        byte[] commandBytes = Encoding.ASCII.GetBytes( ( command + CRLF ).ToCharArray( ) );

        m_pop_stream.Write( commandBytes, 0, commandBytes.Length );
        m_pop_stream.Flush( );

        //Получение ответа
        response = m_pop_stream_reader.ReadLine( );
        return ( response.Length > 0 && response[0] == '+' );
      }
      catch( Exception ex )
      {
        Log.Error( String.Format( "ExecutePopCommand({0}): exception", command ), ex );
      }

      response = null;
      return false;
    }

    bool GetEmailIds( out List<int> EmailIds )
    {
      EmailIds = new List<int>( );

      try
      {
        if( !m_pop_connected )
          return false;

        //Получаем список писем
        string responce;
        if( !ExecutePopCommand( "LIST", out responce ) )
          throw new MailException( "No Email Listing" );

        //И парсим его
        while( ReadLinesFromServer( out responce, null ) )
        {
          int EmailId;

          if( !responce.StartsWith( "+" ) )
          {
            if( int.TryParse( responce.Split( ' ' )[0], out EmailId ) )
              EmailIds.Add( EmailId );
            else
              throw new MailException( "Wrong Email Id format" );
          }
        }
      }
      catch( Exception ex )
      {
        Log.Error( "GetEmailIds(): exception", ex );
      }

      return false;
    }

    bool ReadLinesFromServer( out string Responce, StringBuilder raw_builder )
    {
      try
      {
        Responce = m_pop_stream_reader.ReadLine( );

        if( null == Responce )
          throw new MailException( "Server did not respond" );

        if( null != raw_builder )
          raw_builder.Append( Responce + CRLF );

        //check for byte stuffing, i.e. if a line starts with a '.', another '.' is added, unless
        //it is the last line
        if( Responce.Length > 0 && Responce[0] == '.' )
        {
          if( Responce == "." )
            return false; //closing line found

          //remove the first '.'
          Responce = Responce.TrimStart( '.' );
        }
        return true;
      }
      catch( Exception ex )
      {
        Log.Error( "ReadLinesFromServer(): exception", ex );
        Responce = null;
        return false;
      }
    }

    bool SendRetrCommand( int MessageNo )
    {
      if( !m_pop_connected )
        return false;

      string responce;

      if( !ExecutePopCommand( "RETR " + MessageNo, out responce ) )
      {
        Log.Warn( "SendRetrCommand(): RETR failed for" + MessageNo );
        return false;
      }
      return true;
    }

    bool GetRawEmail( int MessageNo, out string EmailText )
    {
      EmailText = null;

      if( !m_pop_connected )
        return false;

      //send 'RETR int' command to server
      if( !SendRetrCommand( MessageNo ) )
        return false;

      //get the lines
      string response;
      int LineCounter = 0;

      //empty StringBuilder
      StringBuilder email_string = new StringBuilder( );

      while( ReadLinesFromServer( out response, email_string ) )
        LineCounter += 1;

      EmailText = email_string.ToString( );

      Log.Debug("GetRawEmail(): mail recieved : " + LineCounter + " lines " );

      return true;
    }

    bool DeleteEmail( int MessageNo )
    {
      if( !m_pop_connected )
        return false;

      string responce;

      if( !ExecutePopCommand( "DELE " + MessageNo, out responce ) )
      {
        Log.Warn( "DeleteEmail(): DELE failed for " + MessageNo );
        return false;
      }

      return true;
    }

    bool ParseEmail( string Message, out EmailMessage msg )
    {
      msg = new EmailMessage( );

      try
      {
        string message_lower = Message.ToLower( );

        msg.From = GetField( "from: ", message_lower, Message );
        msg.To = GetField( "to: ", message_lower, Message );
        msg.Cc = GetField( "cc: ", message_lower, Message );
        msg.Subj = GetField( "subject: ", message_lower, Message );
        msg.Date = GetField( "date: ", message_lower, Message );
        msg.MimeVer = GetField( "mime-version: ", message_lower, Message );

        msg.Boundaries = GetBoundaries( message_lower, Message );
        msg.Contents = GetContents( message_lower, Message );

        return true;
      }
      catch( Exception ex )
      {
        Log.Error( "ParseEmail(): exception", ex );
      }

      return false;
    }

    static string GetField( string Value, string LowerEmail, string OriginalEmail )
    {
      int index_mime = LowerEmail.IndexOf( Value, 0 );

      if( index_mime == -1 )
        return null;

      index_mime += Value.Length;
      int index_term = OriginalEmail.IndexOf( "\r\n", index_mime );

      return OriginalEmail.Substring( index_mime, index_term - index_mime ).Trim( );
    }

    static List<string> GetBoundaries( string LowerEmail, string OriginalEmail )
    {
      int length = "boundary=".Length;
      int index_mime = LowerEmail.IndexOf( "boundary=", 0 );
      int index_term = 0;

      List<string> boundaries = new List<string>( );

      for( ; index_mime != -1; index_mime = LowerEmail.IndexOf( "boundary=", index_term + 1 ) )
      {
        index_mime += length;
        index_term = LowerEmail.IndexOf( "\r\n", index_mime );

        string temp = "";

        temp = OriginalEmail.Substring( index_mime, index_term - index_mime );
        temp = temp.TrimStart( new char[] {'"'} );
        temp = temp.TrimEnd( new char[] {'"'} );

        boundaries.Add( "--" + temp );
      }

      return boundaries;
    }

    static Dictionary<string, string> GetContents( string LowerEmail, string OriginalEmail )
    {
      int length = "\r\ncontent-type:".Length;
      int index_mime = LowerEmail.IndexOf( "\r\ncontent-type:", 0 );
      int index_term = 0;

      Dictionary<string, string> contents = new Dictionary<string, string>( );

      for( ; index_mime != -1; index_mime = LowerEmail.IndexOf( "\r\ncontent-type:", index_term + 1 ) )
      {
        index_mime += length;
        index_term = LowerEmail.IndexOf( "\r\n\r\n", index_mime );

        string temp = OriginalEmail.Substring( index_mime, index_term - index_mime );
        string last_key = "";

        if( temp.Contains( "text/html" ) )
        {
          contents.Add( "HTML", "" );
          last_key = "HTML";
        }
        else if( temp.Contains( "base64" ) )
        {
          int index1 = temp.ToLower( ).IndexOf( "name=", 0 );
          int index2 = temp.IndexOf( "\r\n", index1 + 5 );
          string filename = temp.Substring( index1 + 5, index2 - ( index1 + 5 ) );
          contents.Add( filename, "" );
          last_key = filename;
        }
        else if( temp.Contains( "text/plain" ) )
        {
          contents.Add( "PLAIN", "" );
          last_key = "PLAIN";
        }

        if( !String.IsNullOrEmpty( last_key ) )
        {
          index_term += 4;
          index_mime = LowerEmail.IndexOf( "\r\ncontent-type:", index_term );

          if( index_mime != -1 )
          {
            temp = OriginalEmail.Substring( index_term, index_mime - index_term );
            contents[last_key] = temp;
          }
          else
          {
            temp = OriginalEmail.Substring( index_term );
            contents[last_key] = temp;
          }
        }
      }

      return contents;
    }

    public void Dispose( )
    {
      m_pop_connection.Close( );

      m_pop_stream.Close( );
      m_pop_stream.Dispose( );

      m_pop_stream_reader.Close( );
      m_pop_stream_reader.Dispose( );
    }

    public virtual string SaveCommand( CommandType dataType ) { return dataType.ToString( ); }
  }


  [Serializable]
  public class MailException : Exception
  {
    public MailException( string s )
      : base( s ) { }
  }


  public class EmailMessage
  {
    public string From;

    public string To;

    public string Cc;

    public string Subj;

    public string Date;

    public string MimeVer;

    public List<string> Boundaries = new List<string>( );

    public Dictionary<string, string> Contents = new Dictionary<string, string>( );
  }
}