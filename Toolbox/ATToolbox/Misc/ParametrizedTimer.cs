using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace AT.Toolbox.Misc
{
  public class ParametrizedTimer<T> 
  {
    protected T m_param;

    System.Threading.Timer m_timer;
    
    public T Parameter
    {
      get {return m_param;}
      set { m_param = value; }
    }

    public void Start()
    {
      m_timer = new Timer( TimerCallback, null, (int)Span.TotalMilliseconds, -1 );
    }

    public TimeSpan Span{get; set;}

    void TimerCallback( object State ) 
    { 
      if( null != Ticked )
        Ticked( this, EventArgs.Empty );

      m_timer.Dispose();
      m_timer = null;
    }

    public event EventHandler Ticked;
  }
}
