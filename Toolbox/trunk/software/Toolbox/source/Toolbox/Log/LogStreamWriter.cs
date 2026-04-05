using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Log
{
  /// <summary>
  /// Текстовый поток, пишущий в log4net
  /// </summary>
  public class LogStreamWriter : TextWriter
  {
    public override Encoding Encoding
    {
      get { return Encoding.UTF8; }
    }

    private readonly ILog m_logger;
    private readonly bool m_info;
	  [NotNull] private readonly List<string> m_buffer = new List<string>();

    /// <summary>
    /// Инициализирует новый поток
    /// </summary>
    /// <param name="source">Логгер в log4net, через который работает поток</param>
    /// <param name="info">Тип сообщений. <code>true</code> — Info, <code>false</code> — Debug</param>
    public LogStreamWriter([NotNull] string source, bool info)
    {
	    if (source == null) throw new ArgumentNullException("source");
			if (string.IsNullOrEmpty(source))
				throw new ArgumentException("Empty", "source");

	    m_logger = LogManager.GetLogger(source);
      m_info = info;
    }

    public override void Write([NotNull] string value)
    {
	    if (value == null) throw new ArgumentNullException("value");

	    lock(m_buffer)
        m_buffer.Add(value);

      if (value == this.NewLine)
      {
        this.Flush();
      }
    }

    protected override void Dispose(bool disposing)
    {
      this.Flush();
      base.Dispose(disposing);
    }

    public override void Close()
    {
      this.Flush();
    }

    public override void Flush()
    {
      lock (m_buffer)
      {
        if (m_buffer.Count > 0)
        {
          string description = m_buffer.Count > 1 ? string.Join(this.NewLine, m_buffer) : "";

          if (m_info)
            m_logger.Info(description);
          else
            m_logger.Debug(description);

          m_buffer.Clear();
        }
      }
    }

		[StringFormatMethod("format")]
		public override void WriteLine([NotNull] string format, [NotNull] params object[] arg)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (arg == null) throw new ArgumentNullException("arg");

			this.Write(string.Format(format, arg));
		}

	  public override void Write(bool value)
    {
      this.Write(value.ToString());
    }

    public override void Write([NotNull] char[] buffer)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

			this.Write(new string (buffer));
    }

	  public override void Write(char value)
    {
      this.Write(value.ToString());
    }

    public override void Write([NotNull] char[] buffer, int index, int count)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

			base.Write(new string(buffer.Skip(index).Take(count).ToArray()));
    }

	  public override void Write(decimal value)
    {
      this.Write(value.ToString());
    }

    public override void Write(double value)
    {
      this.Write(value.ToString());
    }

    public override void Write(float value)
    {
      this.Write(value.ToString());
    }

    public override void Write(int value)
    {
      this.Write(value.ToString());
    }

    public override void Write(long value)
    {
      this.Write(value.ToString());
    }

    public override void Write(object value)
    {
      this.Write((value ?? "").ToString());
    }

    public override void Write(string format, object arg0)
    {
      this.Write(string.Format(format, arg0));
    }

    public override void Write(string format, object arg0, object arg1)
    {
      this.Write(string.Format(format, arg0, arg1));
    }

    public override void Write(string format, object arg0, object arg1, object arg2)
    {
      this.Write(string.Format(format, arg0, arg1, arg2));
    }

		[StringFormatMethod("format")]
		public override void Write([NotNull] string format, [NotNull] params object[] arg)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (arg == null) throw new ArgumentNullException("arg");

			this.Write(string.Format(format, arg));
		}

	  public override void Write(uint value)
    {
      this.Write(value.ToString());
    }

    public override void Write(ulong value)
    {
      this.Write(value.ToString());
    }

    public override void WriteLine()
    {
      this.Write(this.NewLine);
    }

    public override void WriteLine(string value)
    {
      this.Write(value + this.NewLine);
    }

    public override void WriteLine(bool value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(char value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine([NotNull] char[] buffer)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

			this.WriteLine(new string(buffer));
    }

	  public override void WriteLine([NotNull] char[] buffer, int index, int count)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");

			this.WriteLine(new string(buffer.Skip(index).Take(count).ToArray()));
    }

	  public override void WriteLine(decimal value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(double value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(float value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(int value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(long value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(object value)
    {
      this.WriteLine((value ?? "").ToString());
    }

    public override void WriteLine(string format, object arg0)
    {
      this.WriteLine(string.Format(format, arg0));
    }

    public override void WriteLine(string format, object arg0, object arg1)
    {
      this.WriteLine(string.Format(format, arg0, arg1));
    }

    public override void WriteLine(string format, object arg0, object arg1, object arg2)
    {
      this.WriteLine(string.Format(format, arg0, arg2));
    }

    public override void WriteLine(uint value)
    {
      this.WriteLine(value.ToString());
    }

    public override void WriteLine(ulong value)
    {
      this.WriteLine(value.ToString());
    }
  }
}
