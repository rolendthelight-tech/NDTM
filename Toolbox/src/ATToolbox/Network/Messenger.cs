using System.Runtime.Serialization;


namespace AT.Toolbox.Network
{
  using System;


  public abstract class Messenger<TCommandType, TEventType>
    where TCommandType : new() where TEventType : new()
  {
    protected IFormatter m_formatter;

    public Messenger( IFormatter Formatter )
    {
      m_formatter = Formatter;
    }
 
    public abstract void Connect( );

    public abstract void Disconnect( );

    public abstract bool Connected { get; }

    public abstract bool DataPending { get; }

    public abstract bool Recieve( out TCommandType command, out TEventType evt );

    public abstract bool Send( TCommandType data );

    public abstract bool Send( TEventType data );

    public abstract event EventHandler OnDisconnected;

    public abstract event EventHandler OnConnected;
  }
}