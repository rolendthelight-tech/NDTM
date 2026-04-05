using System;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using log4net.Appender;
using log4net.Core;

namespace Toolbox.Log
{
  public class MessageLogAppender : IAppender
  {
    public interface IDestination
    {
      void Append([NotNull] string source, [NotNull] Info message, [NotNull] string threadName, DateTime timeStamp);

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

    public static IDestination Destination
    {
      get { return AppManager.Notificator as IDestination; }
    }

    public void DoAppend([NotNull] LoggingEvent loggingEvent)
    {
	    if (loggingEvent == null) throw new ArgumentNullException("loggingEvent");

			var dest = Destination;

      if (dest == null || dest.Skip)
        return;

      dest.Append(loggingEvent.LoggerName,
        new Info(loggingEvent.RenderedMessage,
          this.ParseLevel(loggingEvent.Level.Name),
					details: loggingEvent.ExceptionObject
				), loggingEvent.ThreadName, loggingEvent.TimeStamp);
    }

    private InfoLevel ParseLevel(string level)
    {
      switch (level)
      {
        case "FATAL":
					return InfoLevel.FatalError;

				case "ERROR":
          return InfoLevel.Error;

        case "WARN":
          return InfoLevel.Warning;

        case "INFO":
          return InfoLevel.Info;

				case "DEBUG":
					return InfoLevel.Debug;

				default:
					return InfoLevel.Error;
			}
    }

    public string Name { get; set; }
  }
}