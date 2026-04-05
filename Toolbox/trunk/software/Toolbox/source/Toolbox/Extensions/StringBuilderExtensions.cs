using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	public static class StringBuilderExtensions
	{
		[NotNull]
		public static StringBuilder TrimEnd([NotNull] this StringBuilder stringBuilder)
		{
			if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");

			var end = stringBuilder.Length - 1;
			while (end >= 0)
			{
				if (!char.IsWhiteSpace(stringBuilder[end]))
				{
					break;
				}
				end--;
			}
			end++;
			stringBuilder.Remove(end, stringBuilder.Length - end);
			return stringBuilder;
		}

		public static void SafeAppendFormat([NotNull] this StringBuilder stringBuilder, [NotNull] string format, [CanBeNull] object arg0)
		{
			if (format == null) throw new ArgumentNullException("format");

			stringBuilder.AppendFormat(format, arg0);
		}

		public static void SafeAppendFormat([NotNull] this StringBuilder stringBuilder, [NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1)
		{
			if (format == null) throw new ArgumentNullException("format");

			stringBuilder.AppendFormat(format, arg0, arg1);
		}

		public static void SafeAppendFormat([NotNull] this StringBuilder stringBuilder, [NotNull] string format, [CanBeNull] object arg0, [CanBeNull] object arg1, [CanBeNull] object arg2)
		{
			if (format == null) throw new ArgumentNullException("format");

			stringBuilder.AppendFormat(format, arg0, arg1, arg2);
		}

		public static void SafeAppendFormat([NotNull] this StringBuilder stringBuilder, [NotNull] string format, [NotNull] params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			stringBuilder.AppendFormat(format, args);
		}

		public static void SafeAppendFormat([NotNull] this StringBuilder stringBuilder, [CanBeNull] IFormatProvider provider, [NotNull] string format, [NotNull] params object[] args)
		{
			if (format == null) throw new ArgumentNullException("format");
			if (args == null) throw new ArgumentNullException("args");

			stringBuilder.AppendFormat(provider, format, args);
		}
	}
}
