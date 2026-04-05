using System;
using System.Threading;

namespace Toolbox.Threading
{
  public class ParametrizedTimer<T> 
  {
    protected T m_param;

    Timer m_timer;

    public T Parameter
    {
      get {return m_param;}
      set { m_param = value; }
    }

    public void Start()
    {
      m_timer = new Timer(TimerCallback, null, Span, /*Timeout.InfiniteTimeSpan — в .NET 4.5*/new TimeSpan(0, 0, 0, 0, -1));
    }

    public TimeSpan Span{get; set;}

    void TimerCallback( object State ) 
    {
			try
			{
				if (null != Ticked)
					Ticked(this, EventArgs.Empty);
			}
			finally
			{
				m_timer.Dispose();
				m_timer = null;
			}
    }

    public event EventHandler Ticked;
  }
}
