using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using log4net.ObjectRenderer;
using log4net.Util;
using Toolbox.Application.Services;
using log4net;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	public static class Log4netExtensions
	{
		[NotNull] private static readonly CultureInfo _log4_net_default_culture_info = CultureInfo.InvariantCulture;

		#region Message

		public static void Message([NotNull] this ILog log, [NotNull] InfoLevel infoLevel, [NotNull] string message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			switch (infoLevel)
			{
				case InfoLevel.Debug: log.Debug(message); break;
				case InfoLevel.Info: log.Info(message); break;
				case InfoLevel.Warning: log.Warn(message); break;
				case InfoLevel.Error: log.Error(message); break;
				case InfoLevel.FatalError: log.Fatal(message); break;
				default: throw new NotImplementedException(infoLevel.ToString());
			}
		}

		public static void Message([NotNull] this ILog log, [NotNull] InfoLevel infoLevel, [NotNull] string message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			switch (infoLevel)
			{
				case InfoLevel.Debug: log.Debug(message, exception); break;
				case InfoLevel.Info: log.Info(message, exception); break;
				case InfoLevel.Warning: log.Warn(message, exception); break;
				case InfoLevel.Error: log.Error(message, exception); break;
				case InfoLevel.FatalError: log.Fatal(message, exception); break;
				default: throw new NotImplementedException(infoLevel.ToString());
			}
		}

		#endregion Message

		#region MessageFormat

		[StringFormatMethod("format")]
		public static void MessageFormat([NotNull] this ILog log, [NotNull] InfoLevel infoLevel, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			switch (infoLevel)
			{
				case InfoLevel.Debug: log.DebugFormat(format, args); break;
				case InfoLevel.Info: log.InfoFormat(format, args); break;
				case InfoLevel.Warning: log.WarnFormat(format, args); break;
				case InfoLevel.Error: log.ErrorFormat(format, args); break;
				case InfoLevel.FatalError: log.FatalFormat(format, args); break;
				default: throw new NotImplementedException(infoLevel.ToString());
			}
		}

		[StringFormatMethod("format")]
		public static void MessageFormat([NotNull] this ILog log, [NotNull] InfoLevel infoLevel, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			switch (infoLevel)
			{
				case InfoLevel.Debug: log.Debug(message, exception); break;
				case InfoLevel.Info: log.Info(message, exception); break;
				case InfoLevel.Warning: log.Warn(message, exception); break;
				case InfoLevel.Error: log.Error(message, exception); break;
				case InfoLevel.FatalError: log.Fatal(message, exception); break;
				default: throw new NotImplementedException(infoLevel.ToString());
			}
		}

		#endregion MessageFormat

		#region LevelFormat

		[StringFormatMethod("format")]
		public static void DebugFormat([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			log.Debug(message, exception);
		}

		[StringFormatMethod("format")]
		public static void InfoFormat([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			log.Info(message, exception);
		}

		[StringFormatMethod("format")]
		public static void WarnFormat([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			log.Warn(message, exception);
		}

		[StringFormatMethod("format")]
		public static void ErrorFormat([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			log.Error(message, exception);
		}

		[StringFormatMethod("format")]
		public static void FatalFormat([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (exception == null) throw new ArgumentNullException("exception");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			var message = new SystemStringFormat(_log4_net_default_culture_info, format, args);

			log.Fatal(message, exception);
		}

		#endregion LevelFormat

		#region LevelEscapeMultiline

		public static void DebugEscapeMultiline([NotNull] this ILog log, [NotNull] object message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			log.Debug(EscapedMultilineMessage.CreateEscapedMultilineMessage(message));
		}

		public static void InfoEscapeMultiline([NotNull] this ILog log, [NotNull] object message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			log.Info(EscapedMultilineMessage.CreateEscapedMultilineMessage(message));
		}

		public static void WarnEscapeMultiline([NotNull] this ILog log, [NotNull] object message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			log.Warn(EscapedMultilineMessage.CreateEscapedMultilineMessage(message));
		}

		public static void ErrorEscapeMultiline([NotNull] this ILog log, [NotNull] object message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			log.Error(EscapedMultilineMessage.CreateEscapedMultilineMessage(message));
		}

		public static void FatalEscapeMultiline([NotNull] this ILog log, [NotNull] object message)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");

			log.Fatal(EscapedMultilineMessage.CreateEscapedMultilineMessage(message));
		}

		#endregion LevelEscapeMultiline

		#region LevelEscapeMultiline

		public static void DebugEscapeMultiline([NotNull] this ILog log, [NotNull] object message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			log.Debug(EscapedMultilineMessage.CreateEscapedMultilineMessage(message), exception);
		}

		public static void InfoEscapeMultiline([NotNull] this ILog log, [NotNull] object message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			log.Info(EscapedMultilineMessage.CreateEscapedMultilineMessage(message), exception);
		}

		public static void WarnEscapeMultiline([NotNull] this ILog log, [NotNull] object message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			log.Warn(EscapedMultilineMessage.CreateEscapedMultilineMessage(message), exception);
		}

		public static void ErrorEscapeMultiline([NotNull] this ILog log, [NotNull] object message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			log.Error(EscapedMultilineMessage.CreateEscapedMultilineMessage(message), exception);
		}

		public static void FatalEscapeMultiline([NotNull] this ILog log, [NotNull] object message, [NotNull] Exception exception)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (message == null) throw new ArgumentNullException("message");
			if (exception == null) throw new ArgumentNullException("exception");

			log.Fatal(EscapedMultilineMessage.CreateEscapedMultilineMessage(message), exception);
		}

		#endregion LevelEscapeMultiline

		#region LevelFormatEscapeMultiline

		[StringFormatMethod("format")]
		public static void DebugFormatEscapeMultiline([NotNull] this ILog log, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Debug(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args));
		}

		[StringFormatMethod("format")]
		public static void InfoFormatEscapeMultiline([NotNull] this ILog log, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Info(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args));
		}

		[StringFormatMethod("format")]
		public static void WarnFormatEscapeMultiline([NotNull] this ILog log, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Warn(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args));
		}

		[StringFormatMethod("format")]
		public static void ErrorFormatEscapeMultiline([NotNull] this ILog log, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Error(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args));
		}

		[StringFormatMethod("format")]
		public static void FatalFormatEscapeMultiline([NotNull] this ILog log, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Fatal(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args));
		}

		#endregion LevelFormatEscapeMultiline

		#region LevelFormatEscapeMultiline

		[StringFormatMethod("format")]
		public static void DebugFormatEscapeMultiline([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Debug(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args), exception);
		}

		[StringFormatMethod("format")]
		public static void InfoFormatEscapeMultiline([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Info(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args), exception);
		}

		[StringFormatMethod("format")]
		public static void WarnFormatEscapeMultiline([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Warn(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args), exception);
		}

		[StringFormatMethod("format")]
		public static void ErrorFormatEscapeMultiline([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Error(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args), exception);
		}

		[StringFormatMethod("format")]
		public static void FatalFormatEscapeMultiline([NotNull] this ILog log, [NotNull] Exception exception, [NotNull] string format, [NotNull] params object[] args)
		{
			if (log == null) throw new ArgumentNullException("log");
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			log.Fatal(EscapedMultilineMessage.CreateEscapedMultilineMessageOld(format, args), exception);
		}

		private class EscapedMultilineMessage
		{
			private const string _start_line_escape = ">";
			[NotNull] public static readonly Regex _start_line_regexp = new Regex(@"^(?!\A)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

			[NotNull] private readonly object m_message;

			[Pure]
			public EscapedMultilineMessage([NotNull] object message)
			{
				if (message == null) throw new ArgumentNullException("message");

				this.m_message = message;
			}

			[NotNull]
			public static object CreateEscapedMultilineMessage([NotNull] object message)
			{
				if (message == null) throw new ArgumentNullException("message");

				var formattable = message as IFormattable;
				if (formattable != null)
					return new EscapedMultilineFormattableMessage(formattable);
				else
					return new EscapedMultilineMessage(message);
			}

			[NotNull]
			public static object CreateEscapedMultilineMessageOld([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				return new EscapedMultilineMessage(new SystemStringFormat(_log4_net_default_culture_info, format, args));
			}

			[NotNull]
			public static object CreateEscapedMultilineMessage([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				return new EscapedMultilineFormattableMessage(new StringExtensions.FormatProvided(format, args));
			}

			#region Overrides of Object

			public override string ToString()
			{
				var str = this.m_message.ToString().ThrowIfNull("str");
				var str2 = _start_line_regexp.Replace(str, _start_line_escape).ThrowIfNull("str2");
				return str2;
			}

			#endregion
		}

		private class EscapedMultilineFormattableMessage : IFormattable
		{
			private const string _start_line_escape = ">";
			[NotNull] private static readonly Regex _start_line_regexp = EscapedMultilineMessage._start_line_regexp;

			[NotNull] private readonly IFormattable m_message;

			[Pure]
			public EscapedMultilineFormattableMessage([NotNull] IFormattable message)
			{
				if (message == null) throw new ArgumentNullException("message");

				this.m_message = message;
			}

			#region Overrides of Object

			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			[NotNull]
			public string ToString([CanBeNull] string format, IFormatProvider formatProvider)
			{
				var str = this.m_message.ToString(format, formatProvider).ThrowIfNull("str");
				var str2 = _start_line_regexp.Replace(str, _start_line_escape).ThrowIfNull("str2");
				return str2;
			}

			#endregion
		}

		#endregion LevelFormatEscapeMultiline

		public class FormatProvidedMessage
		{
			[NotNull] private readonly IFormatProvider m_provider;
			[NotNull] private readonly StringExtensions.IAppendableProvided m_message;

			[Pure]
			public FormatProvidedMessage([NotNull] IFormatProvider provider, [NotNull] StringExtensions.IAppendableProvided message)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (message == null) throw new ArgumentNullException("message");

				this.m_provider = provider;
				this.m_message = message;
			}

			#region Overrides of Object

			[NotNull]
			public override string ToString()
			{
				return StringExtensions.StringAppenderProvided.Use(AppendTo);
			}

			#endregion

			private void AppendTo([NotNull] StringExtensions.IStringAppenderProvided stringAppender)
			{
				if (stringAppender == null) throw new ArgumentNullException("stringAppender");

				this.m_message.AppendFormatTo(this.m_provider, stringAppender);
			}

			[NotNull]
			[Pure]
			public static FormatProvidedMessage FormatInvariantMessage([NotNull] StringExtensions.IAppendableProvided message)
			{
				if (message == null) throw new ArgumentNullException("message");

				return FormatVariantMessage(_log4_net_default_culture_info, message);
			}

			[NotNull]
			[Pure]
			public static FormatProvidedMessage FormatVariantMessage([NotNull] IFormatProvider provider, [NotNull] StringExtensions.IAppendableProvided message)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (message == null) throw new ArgumentNullException("message");

				return new FormatProvidedMessage(provider, message);
			}
		}

		/// <summary>
		/// Рендерер <see cref="T:System.IFormattable"/> для log4net.
		/// </summary>
		[UsedImplicitly]
		public class FormattableRenderer : IObjectRenderer
		{
			public FormattableRenderer()
			{
			}

			#region Члены IObjectRenderer

			public void RenderObject([NotNull] RendererMap rendererMap, [CanBeNull] object obj, [NotNull] System.IO.TextWriter writer)
			{
				if (rendererMap == null) throw new ArgumentNullException("rendererMap");
				if (writer == null) throw new ArgumentNullException("writer");

				if (obj == null)
				{
					writer.Write(SystemInfo.NullText);
				}
				else
				{
					var appendable = obj as StringExtensions.IAppendable;
					if (appendable != null)
					{
						StringExtensions.TextAppender.Use(writer, appendable.AppendTo);
					}
					else
					{
						var appendable_provided = obj as StringExtensions.IAppendableProvided;
						if (appendable_provided != null)
						{
							StringExtensions.TextAppenderProvided.Use(writer, (stringAppender) => appendable_provided.AppendFormatTo(writer.FormatProvider, stringAppender));
						}
						else
						{
							var appendable_not_provided = obj as StringExtensions.IAppendableNotProvided;
							if (appendable_not_provided != null)
							{
								StringExtensions.TextAppenderNotProvided.Use(writer, appendable_not_provided.AppendFormatTo);
							}
							else
							{
								var formattable = ((IFormattable)obj).ThrowIfNull("formattable");
								writer.Write(formattable);
							}
						}
					}
				}
			}

			#endregion
		}
	}
}
