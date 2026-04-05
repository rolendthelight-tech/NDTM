using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	public static class StringExtensions
	{
		[Pure]
		public static bool ContainsAnyOf([NotNull] this string str, [NotNull] string rw)
		{
			if (str == null) throw new ArgumentNullException("str");
			if (rw == null) throw new ArgumentNullException("rw");

			foreach (char c in rw)
			{
				if (str.Contains(c))
					return true;
			}

			return false;
		}

		[NotNull]
		[Pure]
		public static StringToken[] SimpleTokenize([NotNull] this string str)
		{
			if (str == null) throw new ArgumentNullException("str");

			var current_token = new StringToken();
			var ret_val = new List<StringToken>();

			foreach (char c in str)
			{
				if (current_token.Category != char.GetUnicodeCategory(c))
				{
					if (!string.IsNullOrEmpty(current_token.Value))
						ret_val.Add(current_token);

					current_token = new StringToken
					{
						Category = char.GetUnicodeCategory(c),
						Value = new string(c, 1)
					};
				}
				else
					current_token.Value += c.ToString();
			}

			if (!string.IsNullOrEmpty(current_token.Value))
				ret_val.Add(current_token);

			return ret_val.ToArray();
		}

		/// <summary> Закавычивание строки </summary>
		[Pure]
		public static string Quoted(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return "\"\"";

			if (!s.StartsWith("\""))
				s = "\"" + s;

			if (!s.EndsWith("\""))
				s += "\"";

			return s;
		}

		/// <summary> Заключение текста в кавычки, если он есть, иначе — возвращает "null" без кавычек. </summary>
		/// <param name="text">Исходный текст.</param>
		/// <returns>Текст в кавычках или "null" без кавычек.</returns>
		[NotNull]
		[Pure]
		public static string QuoteOrNull([CanBeNull] this string text)
		{
			if (text == null)
				return "null";
			else
				return string.Format("\"{0}\"", text);
		}

		[Pure]
		public static string Capitalize(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
				return s;

			var b = new StringBuilder(s.Length);
			b.Append(s.Substring(0, 1).ToUpper());
			b.Append(s.Substring(1).ToLower());

			return b.ToString();
		}

		public interface IStringAppender
		{
			void Append([CanBeNull] string value);
			void Append([CanBeNull] string value, int startIndex, int count);
			void Append([CanBeNull] object value);
			void AppendLine();
			void AppendLine([CanBeNull] string value);
		}

		public interface IStringAppenderNotProvided : IStringAppender
		{
			void AppendFormat([NotNull] string format, [CanBeNull] object arg0);
			void AppendFormat([NotNull] string format, [NotNull] params object[] args);
			void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1);
			void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2);
		}

		public interface IStringAppenderProvided : IStringAppender
		{
			void Append([NotNull] IFormatProvider provider, [CanBeNull] object value);
			void AppendFormat([NotNull] IFormatProvider provider, [NotNull] string format, [NotNull] params object[] args);
		}

		public interface IStringAppenderFixedProvided : IStringAppender
		{
			void Append([CanBeNull] object value);
			void AppendFormat([NotNull] string format, [NotNull] params object[] args);
		}

		public class StringAppender : IStringAppender
		{
			[NotNull] private readonly StringBuilder m_string_builder;

			[Pure]
			public StringAppender([NotNull] StringBuilder stringBuilder)
			{
				if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");

				this.m_string_builder = stringBuilder;
			}

			public virtual void Append([CanBeNull] string value)
			{
				this.m_string_builder.Append(value);
			}

			public virtual void Append([CanBeNull] string value, int startIndex, int count)
			{
				this.m_string_builder.Append(value, startIndex, count);
			}

			public virtual void Append([CanBeNull] object value)
			{
				this.m_string_builder.Append(value);
			}

			public virtual void AppendLine()
			{
				this.m_string_builder.AppendLine();
			}

			public virtual void AppendLine([CanBeNull] string value)
			{
				this.m_string_builder.AppendLine(value);
			}

			[NotNull]
			[Pure]
			public static string Use([NotNull] Action<IStringAppender> action)
			{
				if (action == null) throw new ArgumentNullException("action");

				var string_builder = new StringBuilder();
				var string_appender = new StringExtensions.StringAppender(string_builder);
				action(string_appender);
				var result = string_builder.ToString();
				string_builder.Clear();
				return result;
			}
		}

		public class StringAppenderNotProvided : StringAppender, IStringAppenderNotProvided
		{
			[NotNull] private readonly StringBuilder m_string_builder;

			[Pure]
			public StringAppenderNotProvided([NotNull] StringBuilder stringBuilder)
				: base(stringBuilder)
			{
				if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");

				this.m_string_builder = stringBuilder;
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_string_builder.AppendFormat(format, arg0);
			}

			public virtual void AppendFormat([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_string_builder.AppendFormat(format, args);
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_string_builder.AppendFormat(format, arg0, arg1);
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_string_builder.AppendFormat(format, arg0, arg1, arg2);
			}

			[NotNull]
			[Pure]
			public static string Use([NotNull] Action<IStringAppenderNotProvided> action)
			{
				if (action == null) throw new ArgumentNullException("action");

				var string_builder = new StringBuilder();
				var string_appender = new StringExtensions.StringAppenderNotProvided(string_builder);
				action(string_appender);
				var result = string_builder.ToString();
				string_builder.Clear();
				return result;
			}
		}

		public class StringAppenderProvided : StringAppender, IStringAppenderProvided
		{
			[NotNull]
			private readonly StringBuilder m_string_builder;

			[Pure]
			public StringAppenderProvided([NotNull] StringBuilder stringBuilder)
				: base(stringBuilder)
			{
				if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");

				this.m_string_builder = stringBuilder;
			}

			public virtual void Append([NotNull] IFormatProvider provider, [CanBeNull] object value)
			{
				if (provider == null) throw new ArgumentNullException("provider");

				if (value != null)
				{
					this.AppendFormat(provider, "{0}", value);
				}
			}

			public virtual void AppendFormat([NotNull] IFormatProvider provider, [NotNull] string format, [NotNull] params object[] args)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_string_builder.AppendFormat(provider, format, args);
			}

			[NotNull]
			[Pure]
			public static string Use([NotNull] Action<IStringAppenderProvided> action)
			{
				if (action == null) throw new ArgumentNullException("action");

				var string_builder = new StringBuilder();
				var string_appender = new StringExtensions.StringAppenderProvided(string_builder);
				action(string_appender);
				var result = string_builder.ToString();
				string_builder.Clear();
				return result;
			}
		}

		public class StringAppenderFixedProvided : StringAppender, IStringAppenderFixedProvided
		{
			[NotNull] private readonly StringBuilder m_string_builder;
			[NotNull] private readonly IFormatProvider m_provider;

			[Pure]
			public StringAppenderFixedProvided([NotNull] StringBuilder stringBuilder, [NotNull] IFormatProvider provider)
				: base(stringBuilder)
			{
				if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");
				if (provider == null) throw new ArgumentNullException("provider");

				this.m_string_builder = stringBuilder;
				this.m_provider = provider;
			}

			public override void Append([CanBeNull] object value)
			{
				if (value != null)
				{
					this.AppendFormat("{0}", value);
				}
			}

			public virtual void AppendFormat([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_string_builder.AppendFormat(this.m_provider, format, args);
			}

			[NotNull]
			[Pure]
			public static string Use([NotNull] IFormatProvider provider, [NotNull] Action<IStringAppenderFixedProvided> action)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (action == null) throw new ArgumentNullException("action");

				var string_builder = new StringBuilder();
				var string_appender = new StringExtensions.StringAppenderFixedProvided(string_builder, provider);
				action(string_appender);
				var result = string_builder.ToString();
				string_builder.Clear();
				return result;
			}
		}

		public class TextAppender : IStringAppender
		{
			[NotNull] private readonly TextWriter m_text_writer;

			[Pure]
			public TextAppender([NotNull] TextWriter textWriter)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");

				this.m_text_writer = textWriter;
			}

			public virtual void Append([CanBeNull] string value)
			{
				this.m_text_writer.Write(value);
			}

			public virtual void Append([CanBeNull] string value, int startIndex, int count)
			{
				if (value != null)
					this.m_text_writer.Write(value.ToCharArray(startIndex, count));
			}

			public virtual void Append([CanBeNull] object value)
			{
				this.m_text_writer.Write(value);
			}

			public virtual void AppendLine()
			{
				this.m_text_writer.WriteLine();
			}

			public virtual void AppendLine([CanBeNull] string value)
			{
				this.m_text_writer.WriteLine(value);
			}

			[NotNull]
			public static void Use([NotNull] TextWriter textWriter, [NotNull] Action<IStringAppender> action)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");
				if (action == null) throw new ArgumentNullException("action");

				var string_appender = new StringExtensions.TextAppender(textWriter);
				action(string_appender);
			}
		}

		public class TextAppenderNotProvided : TextAppender, IStringAppenderNotProvided
		{
			[NotNull] private readonly TextWriter m_text_writer;

			[Pure]
			public TextAppenderNotProvided([NotNull] TextWriter textWriter)
				: base(textWriter)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");

				this.m_text_writer = textWriter;
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_text_writer.Write(format, arg0);
			}

			public virtual void AppendFormat([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_text_writer.Write(format, args);
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_text_writer.Write(format, arg0, arg1);
			}

			public virtual void AppendFormat([NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2)
			{
				if (format == null) throw new ArgumentNullException("format");

				this.m_text_writer.Write(format, arg0, arg1, arg2);
			}

			[NotNull]
			public static void Use([NotNull] TextWriter textWriter, [NotNull] Action<IStringAppenderNotProvided> action)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");
				if (action == null) throw new ArgumentNullException("action");

				var string_appender = new StringExtensions.TextAppenderNotProvided(textWriter);
				action(string_appender);
			}
		}

		public class TextAppenderProvided : TextAppender, IStringAppenderProvided
		{
			[NotNull] private readonly TextWriter m_text_writer;

			[Pure]
			public TextAppenderProvided([NotNull] TextWriter textWriter)
				: base(textWriter)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");

				this.m_text_writer = textWriter;
			}

			public virtual void Append([NotNull] IFormatProvider provider, [CanBeNull] object value)
			{
				if (provider == null) throw new ArgumentNullException("provider");

				if (value != null)
				{
					this.AppendFormat(provider, "{0}", value); // TODO
				}
			}

			public virtual void AppendFormat([NotNull] IFormatProvider provider, [NotNull] string format, [NotNull] params object[] args)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_text_writer.Write(string.Format(provider, format, args)); // TODO
			}

			[NotNull]
			public static void Use([NotNull] TextWriter textWriter, [NotNull] Action<IStringAppenderProvided> action)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");
				if (action == null) throw new ArgumentNullException("action");

				var string_appender = new StringExtensions.TextAppenderProvided(textWriter);
				action(string_appender);
			}
		}

		public class TextAppenderFixedProvided : TextAppender, IStringAppenderFixedProvided
		{
			[NotNull] private readonly TextWriter m_text_writer;
			[NotNull] private readonly IFormatProvider m_provider;

			[Pure]
			public TextAppenderFixedProvided([NotNull] TextWriter textWriter, [NotNull] IFormatProvider provider)
				: base(textWriter)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");
				if (provider == null) throw new ArgumentNullException("provider");

				this.m_text_writer = textWriter;
				this.m_provider = provider;
			}

			public override void Append([CanBeNull] object value)
			{
				if (value != null)
				{
					this.AppendFormat("{0}", value); // TODO
				}
			}

			public virtual void AppendFormat([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_text_writer.Write(string.Format(this.m_provider, format, args)); // TODO
			}

			[NotNull]
			public static void Use([NotNull] TextWriter textWriter, [NotNull] IFormatProvider provider, [NotNull] Action<IStringAppenderFixedProvided> action)
			{
				if (textWriter == null) throw new ArgumentNullException("textWriter");
				if (provider == null) throw new ArgumentNullException("provider");
				if (action == null) throw new ArgumentNullException("action");

				var string_appender = new StringExtensions.TextAppenderFixedProvided(textWriter, provider);
				action(string_appender);
			}
		}

		public interface IAppendable
		{
			void AppendTo([NotNull] IStringAppender appender);
		}

		public interface IAppendableNotProvided
		{
			void AppendFormatTo([NotNull] IStringAppenderNotProvided appender);
		}

		public interface IAppendableProvided : IFormattable
		{
			[Pure]
			[NotNull]
			[Obsolete("Следует использовать метод ToString(IFormatProvider)", true)]
			string ToString();

			[Pure]
			[NotNull]
			string ToString([NotNull] IFormatProvider formatProvider);

			void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender);
		}

		public class TryCatchProvided : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly IAppendableProvided m_catch_string;
			[NotNull] private readonly IAppendableProvided m_try_string;

			[Pure]
			public TryCatchProvided([NotNull] IAppendableProvided catchString, [NotNull] IAppendableProvided tryString)
			{
				if (catchString == null) throw new ArgumentNullException("catchString");
				if (tryString == null) throw new ArgumentNullException("tryString");

				this.m_catch_string = catchString;
				this.m_try_string = tryString;
			}

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				try
				{
					this.m_try_string.AppendFormatTo(provider, appender);
				}
				catch (Exception)
				{
					this.m_catch_string.AppendFormatTo(provider, appender);
				}
			}

			#endregion

			#region Члены IFormattable

			[Pure]
			[NotNull]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion

			#region Overrides of Format

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#endregion
		}

		public class TryCatchNotProvided : IAppendableNotProvided
		{
			[NotNull] private readonly IAppendableNotProvided m_catch_string;
			[NotNull] private readonly IAppendableNotProvided m_try_string;

			[Pure]
			public TryCatchNotProvided([NotNull] IAppendableNotProvided catchString, [NotNull] IAppendableNotProvided tryString)
			{
				if (catchString == null) throw new ArgumentNullException("catchString");
				if (tryString == null) throw new ArgumentNullException("tryString");

				this.m_catch_string = catchString;
				this.m_try_string = tryString;
			}

			#region Overrides of Format

			[Pure]
			public override string ToString()
			{
				return StringAppenderNotProvided.Use(appender => AppendFormatTo(appender));
			}

			#endregion

			#region Члены IAppendableNotProvided

			public virtual void AppendFormatTo([NotNull] IStringAppenderNotProvided appender)
			{
				if (appender == null) throw new ArgumentNullException("appender");

				try
				{
					this.m_try_string.AppendFormatTo(appender);
				}
				catch (Exception)
				{
					this.m_catch_string.AppendFormatTo(appender);
				}
			}

			#endregion
		}

		public class StringPart : IAppendable
		{
			[NotNull] private readonly string m_string;

			[Pure]
			public StringPart([NotNull] string @string)
			{
				if (@string == null) throw new ArgumentNullException("string");

				this.m_string = @string;
			}

			#region Члены IAppendable

			[Pure]
			public override string ToString()
			{
				return this.m_string;
			}

			public virtual void AppendTo([NotNull] IStringAppender appender)
			{
				if (appender == null) throw new ArgumentNullException("appender");

				appender.Append(this.m_string);
			}

			#endregion
		}

		public class StringPartProvided : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly string m_string;

			[Pure]
			public StringPartProvided([NotNull] string @string)
			{
				if (@string == null) throw new ArgumentNullException("string");

				this.m_string = @string;
			}

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				appender.AppendFormat(provider, "{0}", this.m_string);
			}

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#endregion

			#region Члены IFormattable

			[Pure]
			[NotNull]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion
		}

		public class StringPartNotProvided : IAppendableNotProvided
		{
			[NotNull] private readonly string m_string;

			[Pure]
			public StringPartNotProvided([NotNull] string @string)
			{
				if (@string == null) throw new ArgumentNullException("string");

				this.m_string = @string;
			}

			#region Члены IAppendableNotProvided

			[Pure]
			public override string ToString()
			{
				return this.m_string;
			}

			public virtual void AppendFormatTo([NotNull] IStringAppenderNotProvided appender)
			{
				if (appender == null) throw new ArgumentNullException("appender");

				appender.Append(this.m_string);
			}

			#endregion
		}

		public class FormatProvided : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly string m_format;
			[NotNull] private readonly object[] m_args;

			[Pure]
			public FormatProvided([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_format = format;
				this.m_args = args;
			}

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				appender.AppendFormat(provider, this.m_format, this.m_args);
			}

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#endregion

			#region Члены IFormattable

			[Pure]
			[NotNull]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion
		}

		public class FormatNotProvided : IAppendableNotProvided
		{
			[NotNull] private readonly string m_format;
			[NotNull] private readonly object[] m_args;

			[Pure]
			public FormatNotProvided([NotNull] string format, [NotNull] params object[] args)
			{
				if (format == null) throw new ArgumentNullException("format");
				if (args == null) throw new ArgumentNullException("args");

				this.m_format = format;
				this.m_args = args;
			}

			#region Члены IAppendableNotProvided

			[Pure]
			public override string ToString()
			{
				return StringAppenderNotProvided.Use(appender => AppendFormatTo(appender));
			}

			public virtual void AppendFormatTo([NotNull] IStringAppenderNotProvided appender)
			{
				if (appender == null) throw new ArgumentNullException("appender");

				appender.AppendFormat(this.m_format, this.m_args);
			}

			#endregion
		}

		public class ConcatProvided<T> : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly IEnumerable<T> m_values;

			[Pure]
			public ConcatProvided([NotNull] IEnumerable<T> values)
			{
				if (values == null) throw new ArgumentNullException("values");

				this.m_values = values;
			}

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#region Члены IFormattable

			[Pure]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				foreach (var value in this.m_values)
				{
					appender.Append(provider, value);
				}
			}

			#endregion
		}

		public class JoinProvided<T> : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly string m_separator;
			[NotNull] private readonly IEnumerable<T> m_values;

			[Pure]
			public JoinProvided([NotNull] string separator, [NotNull] IEnumerable<T> values)
			{
				if (separator == null) throw new ArgumentNullException("separator");
				if (values == null) throw new ArgumentNullException("values");

				this.m_separator = separator;
				this.m_values = values;
			}

			[Pure]
			public JoinProvided([NotNull] string separator, [NotNull] params T[] values)
				: this(separator, (IEnumerable<T>)values)
			{
			}

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#region Члены IFormattable

			[Pure]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				using (var enumerator = this.m_values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						{
							var value = enumerator.Current;
							appender.Append(provider, value);
						}
						while (enumerator.MoveNext())
						{
							var value = enumerator.Current;
							appender.Append(provider, this.m_separator);
							appender.Append(provider, value);
						}
					}
				}
			}

			#endregion
		}

		public class ArrayFormatProvided<T> : IFormattable, IAppendableProvided
		{
			[NotNull] private readonly IAppendableProvided m_values;

			[Pure]
			public ArrayFormatProvided([NotNull] params T[] values)
			{
				if (values == null) throw new ArgumentNullException("values");

				this.m_values = new JoinProvided<T>(", ", values);
			}

			[Pure]
			[Obsolete("Не указан IFormatProvider", true)]
			public override string ToString()
			{
				throw new InvalidOperationException("Не указан IFormatProvider");
			}

			#region Члены IFormattable

			[Pure]
			public virtual string ToString([CanBeNull] string format, [NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return StringAppenderProvided.Use(appender => AppendFormatTo(formatProvider, appender));
			}

			#endregion

			#region Члены IAppendableProvided

			[Pure]
			public virtual string ToString([NotNull] IFormatProvider formatProvider)
			{
				if (formatProvider == null) throw new ArgumentNullException("formatProvider");

				return ToString(null, formatProvider);
			}

			public virtual void AppendFormatTo([NotNull] IFormatProvider provider, [NotNull] IStringAppenderProvided appender)
			{
				if (provider == null) throw new ArgumentNullException("provider");
				if (appender == null) throw new ArgumentNullException("appender");

				appender.Append(provider, "{");
				this.m_values.AppendFormatTo(provider, appender);
				appender.Append(provider, "}");
			}

			#endregion
		}
	}

	public class StringToken
  {
    public StringToken()
    {
      Category = default( UnicodeCategory );
      Value = "";
    }

    public string Value { get; set; }
    public UnicodeCategory Category { get; set; }
  }
}
