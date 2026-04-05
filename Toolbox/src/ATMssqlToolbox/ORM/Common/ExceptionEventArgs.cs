using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL
{
  public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);

  public class ExceptionEventArgs : EventArgs
  {
    Exception exception;

    public ExceptionEventArgs(Exception exception)
    {
      this.exception = exception;
    }

    public ExceptionEventArgs(string message)
    {
      this.exception = new Exception(message);
    }

    public Exception Exception
    {
      get { return exception; }
    }
  }
}
