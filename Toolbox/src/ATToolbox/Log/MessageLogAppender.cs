using System.Collections.Generic;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using System;

namespace AT.Toolbox
{
  public class MessageLogAppender : IAppender
  {
    public interface IDestination
    {
      void Append(string source, Info message);

      bool Skip { get; }
    }

    private static int _instance_count;

    public MessageLogAppender()
    {
      Interlocked.Increment(ref _instance_count);
    }

    public void Close() 
    {
      Interlocked.Decrement(ref _instance_count);
    }

    public static bool Enabled
    {
      get { return _instance_count > 0; }
    }

    public static IDestination Destination { get; set; }

    public void DoAppend(LoggingEvent logging_event)
    {
      var dest = Destination;

      if (dest == null || dest.Skip)
        return;

      dest.Append(logging_event.LoggerName,
        new Info(logging_event.MessageObject.ToString(),
          this.ParseLevel(logging_event.Level.Name))
        {
          Details = logging_event.ExceptionObject
        });
    }

    private InfoLevel ParseLevel(string level)
    {
      switch (level)
      {
        case "FATAL":
        case "ERROR":
          return InfoLevel.Error;

        case "WARN":
          return InfoLevel.Warning;

        case "INFO":
          return InfoLevel.Info;

        default:
          return InfoLevel.Debug;
      }
    }

    public string Name { get; set; }
  }
}