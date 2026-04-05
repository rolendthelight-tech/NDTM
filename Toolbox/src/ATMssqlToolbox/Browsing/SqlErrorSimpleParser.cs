using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace AT.Toolbox.MSSQL.Browsing
{
  public class SqlErrorSimpleParser
  {
    private static Regex _percentFinder = new Regex(@"(?<=(^)?)((?:\+|-|(?<!\d))\d+)(?=(?(1)(?:\b)|(?: ?(?:percent\b|%))))",
                                                    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    
    public readonly int Progress;
    public readonly bool IsProgress;
    public readonly string Message;
    public readonly ErrorLevel Level;

    private SqlErrorSimpleParser(int Progress, bool IsProgress, string Message, ErrorLevel Level)
    {
      this.Progress = Progress;
      this.IsProgress = IsProgress;
      this.Message = Message;
      this.Level = Level;
    }

    public static SqlErrorSimpleParser Parse(SqlError error)
    {
      if (error == null)
        throw new ArgumentNullException("error");
      int Progress;
      bool IsProgress;
      string Message;
      ErrorLevel Level;
      Level = error.Class >= 8
                ? (error.Class >= 16
                     ? ErrorLevel.Error
                     : ErrorLevel.Warning)
                : (error.Class >= 4
                     ? ErrorLevel.Info
                     : ErrorLevel.Debug);

      Message = error.Message;

      switch (error.Number)
      {
        case 3211:
          {
            {
              int percent = 0;
              var match = _percentFinder.Match(Message);
              if (IsProgress = (match.Success && int.TryParse(match.Value, out percent)))
              {
                Progress = percent;
              }
              else
              {
                Progress = 0;
              }
            }
            break;
          }
        default:
          {
            IsProgress = false;
            Progress = 0;
            break;
          }
      }

      return new SqlErrorSimpleParser(Progress, IsProgress, Message, Level);
    }

    public enum ErrorLevel : byte
    {
      Debug,
      Info,
      Warning,
      Error,
      Fatal,
      Unknown
    }
  }

}
