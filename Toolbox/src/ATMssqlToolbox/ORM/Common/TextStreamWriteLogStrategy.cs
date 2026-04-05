using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AT.Toolbox.MSSQL
{
  public class TextStreamWriteLogStrategy : IWriteLogStrategy
  {
    public TextStreamWriteLogStrategy()
    {
      StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "app.log", false);
      writer.AutoFlush = true;
      writer.Close();
    }
    
    #region IWriteLogStrategy Members

    public void WriteLog(string eventDescription, InfoLevel state)
    {
      StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\app.log", true);
      writer.AutoFlush = true;
      writer.WriteLine(state.ToString().ToUpper() + ": " 
        + eventDescription + ": " + DateTime.Now.ToLongTimeString() 
        + "." + DateTime.Now.Millisecond.ToString());
      writer.Close();
    }

    public void ReportError(Exception ex)
    {
      StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\app.log", true);
      writer.AutoFlush = true;
      writer.WriteLine("ERROR: " + ex.Message + ": " + DateTime.Now.ToLongTimeString()
        + "." + DateTime.Now.Millisecond.ToString());
      writer.WriteLine(ex.StackTrace);
      writer.WriteLine();
      writer.Close();
    }

    #endregion
  }
}
