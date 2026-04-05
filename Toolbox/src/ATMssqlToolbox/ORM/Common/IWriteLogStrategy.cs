using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL
{
  public interface IWriteLogStrategy
  {
    void WriteLog(string message, InfoLevel state);
    void ReportError(Exception ex);
  }
}
