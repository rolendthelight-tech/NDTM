using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Toolbox.Common;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{

	public static class PathExtensions
	{
		private const string _UriSchemeUnsafeFile = "unsafe-file";

		#region Fields

		[NotNull] private static readonly Graphics _graphics = Graphics.FromHwnd(IntPtr.Zero).ThrowIfNull("_graphics");

		[NotNull] private static readonly Regex _path_regexp = new Regex(
			string.Format(
				@"\A(?:(?<relative>(?!{1})(?:(?!{1})(?<disk>{3}){2})?(?<path>(?<name>{0}+?)(?:(?<separator>{1})(?<name>{0}*?))*?))|(?<absolute>(?!{1}{4}2{5})(?:(?!{1})(?<disk>{3}){2})?(?:(?<separator>{1})(?<name>{0}*?))*?)|(?<UNC>{1}{4}2{5}(?<host>{0}+?)(?<path>(?:(?<separator>{1})(?<name>{0}*?))*?)))\z",
				string.Format("(?:(?!{0}).)", string.Join("|", Path.GetInvalidFileNameChars().Concat(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, }).Distinct().Select(ch => Regex.Escape(ch.ToString())))),
				string.Format("(?:{0})",string.Join("|", new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, }.Distinct().Select(ch => Regex.Escape(ch.ToString())))),
				Regex.Escape(Path.VolumeSeparatorChar.ToString()),
				"(?i:[a-z])",
				"{",
				"}"),
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

		[NotNull] private static readonly Regex _uri_builder_escape_compensation_regexp = new Regex(
			"%",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

		[NotNull]
		public static string UriSchemeUnsafeFile
		{
			get { return _UriSchemeUnsafeFile; }
		}

		#endregion

		#region PathHelpers

		/// <summary>
		/// Получает относительный путь второго каталога относительно первого независимо от текущей директории приложения
		/// </summary>
		/// <param name="rootPath">Каталог, относительно которого найти путь</param>
		/// <param name="fullPath">Целевой каталог</param>
		/// <returns>Относительный путь</returns>
		[NotNull]
		[Pure]
		public static string GetRelativePath([NotNull][PathReference] string rootPath, [NotNull][PathReference] string fullPath)
		{
			if (rootPath == null) throw new ArgumentNullException("rootPath");
			if (fullPath == null) throw new ArgumentNullException("fullPath");
			// Этот метод не может использовать Path.GetFullPath, Directory.Exists, File.Exists так как не зависит от текущей директории приложения, наличия каких-либо файлов

			List<string> root_lines = rootPath.Split(new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}).ToList();
			List<string> full_lines = fullPath.Split(new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}).ToList();

			if (root_lines.Count == 0 || (root_lines.Count == 1 && root_lines[0] == "") || full_lines.Count == 0 || (full_lines.Count == 1 && full_lines[0] == ""))
			{
				return fullPath;
			}
			if (full_lines.Count == 1 && full_lines[0].Contains(Path.VolumeSeparatorChar))
			{
				return fullPath;
			}
			if (root_lines.Count == 1 && root_lines[0].Contains(Path.VolumeSeparatorChar))
			{
				if (root_lines[0] != full_lines[0])
					return fullPath;
				else
				{
					full_lines[0] = "";
					string.Join(Path.DirectorySeparatorChar.ToString(), full_lines);
				}
			}

			int root_min_level = root_lines.Count >= 2 && root_lines[0] == "" && root_lines[1] == "" ? 2 : 1; // Определяется путь UNC
			int full_min_level = full_lines.Count >= 2 && full_lines[0] == "" && full_lines[1] == "" ? 2 : 1; // Определяется путь UNC

			if (root_min_level != full_min_level)
				return fullPath;
			if (root_min_level == 2)
			{
				if (root_lines.Count < 3 || full_lines.Count < 3 || root_lines[2] != full_lines[2])
						return fullPath;
				if (root_lines.Count < 4 && full_lines.Count < 4 || root_lines[3] != full_lines[3])
					return fullPath;
			}
			else
			{
				if (root_lines.Count < 1 || full_lines.Count < 1 || root_lines[0] != full_lines[0])
					return fullPath;
			}

			{
				bool root_reduced = true;

				for (int i = root_lines.Count - 1, n_up = 0; i >= root_min_level; --i)
				{
					var name = root_lines[i];
					if (name == "" || name == ".")
						root_lines.RemoveAt(i);
					else if (name == "..")
					{
						n_up++;
						root_reduced = false;
					}
					else if (n_up > 0)
					{
						root_lines.RemoveRange(i, n_up*2);
						n_up = 0;
						root_reduced = true;
					}
				}

				if (!root_reduced)
					return fullPath;
			}

			bool full_is_dir = full_lines[full_lines.Count - 1] == "";
			{
				bool full_reduced = true;

				for (int i = full_lines.Count - 1, n_up = 0; i >= full_min_level; --i)
				{
					var name = full_lines[i];
					if (name == "" || name == ".")
						full_lines.RemoveAt(i);
					else if (name == "..")
					{
						n_up++;
						full_reduced = false;
					}
					else if (n_up > 0)
					{
						full_lines.RemoveRange(i, n_up*2);
						n_up = 0;
						full_reduced = true;
					}
				}

				if (!full_reduced)
					return fullPath;
			}

			int number2up = root_lines.Count - full_lines.Count;

			if (number2up < 0)
				number2up = 0;

			var max = Math.Min(root_lines.Count, full_lines.Count);

			for (int i = 0; i < max; i++)
			{
				if (root_lines[i] != full_lines[i])
				{
					number2up = root_lines.Count - i;
					break;
				}
			}

			var ret = new List<string>(Math.Max(full_lines.Count - root_lines.Count + number2up * 2, 0));

			ret.AddRange(Enumerable.Repeat("..", number2up));

			for (int i = root_lines.Count - number2up; i < full_lines.Count; i++)
				ret.Add(full_lines[i]);

			if (full_is_dir)
				ret.Add("");

			return string.Join(Path.DirectorySeparatorChar.ToString(), ret);
		}

		/// <summary>
		/// Получает относительный путь второго каталога относительно первого независимо от текущей директории приложения. 
		/// Использует <see cref="T:System.Uri"/>, в отличие от функции <see cref="GetRelativePath"/> правильно работает, если файл находится в каталоге полного пути.
		/// </summary>
		/// <param name="rootPath">Каталог, относительно которого найти путь</param>
		/// <param name="fullPath">Целевой каталог</param>
		/// <returns>Относительный путь</returns>
		[CanBeNull]
		[Pure]
		public static String GetRelativePath2([NotNull][PathReference] String rootPath, [CanBeNull][PathReference] String fullPath)
		{
			if (rootPath == null) throw new ArgumentNullException("rootPath");
			if (String.IsNullOrEmpty(rootPath)) throw new ArgumentException("Empty", "rootPath");
			if (String.IsNullOrEmpty(fullPath)) return null;

			var fromUri = new Uri(rootPath);
			var toUri = new Uri(fullPath);

			if (!fromUri.AbsoluteUri.EndsWith("/"))
				fromUri = new Uri(rootPath.Replace("\\", "/") + "/");

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			relativePath = relativePath.Replace(@"file:///", "");
			return relativePath.Replace('/', Path.DirectorySeparatorChar);
		}

		/// <summary>
		/// Получает абсолютный путь второго каталога относительно первого независимо от текущей директории приложения
		/// </summary>
		/// <param name="rootPath">Каталог, относительно которого считать целевой путь</param>
		/// <param name="fullPath">Целевой каталог</param>
		/// <returns>Абсолютный путь</returns>
		[NotNull]
		[Pure]
		public static string GetAbsolutePath([NotNull][PathReference] string rootPath, [NotNull][PathReference] string fullPath)
		{
			if (rootPath == null) throw new ArgumentNullException("rootPath");
			if (fullPath == null) throw new ArgumentNullException("fullPath");
			// Этот метод не может использовать Path.GetFullPath, Directory.Exists, File.Exists так как не зависит от текущей директории приложения, наличия каких-либо файлов

			List<string> ret = new List<string>();
			ret.AddRange(Path.Combine(rootPath, fullPath).Split(new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}));

			int min_level = ret.Count >= 2 && ret[0] == "" && ret[1] == "" ? 2 : 1; // Определяется путь UNC

			for (int i = min_level; i < ret.Count && i >= min_level; ++i)
			{
				if (ret[i] == ".")
					ret.RemoveAt(i--);
				else if (ret[i] == ".." && i - 1 >= min_level && ret[i - 1] != "." && ret[i - 1] != "..") // В этой строке нет избыточных сравнений
				{
					ret.RemoveRange(i - 1, 2);
					i -= 2;
				}
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), ret);
		}

		[NotNull]
		private static string UriBuilderEscapeCompensate([NotNull] string srt)
		{
			if (srt == null) throw new ArgumentNullException("srt");

			return _uri_builder_escape_compensation_regexp.Replace(srt, UriBuilderEscapeCompensateOnEvaluator);
		}

		[NotNull]
		private static string UriBuilderEscapeCompensateOnEvaluator([NotNull] Match match)
		{
			if (match == null) throw new ArgumentNullException("match");

			if (!match.Success)
				throw new RegExpException("Ошибка в регулярном выражении");
			else
			{
				switch (match.Value)
				{
					case "%":
						return Uri.EscapeDataString(match.Value);
					default:
						throw new RegExpException("Несогласованное регулярное выражение");
				}
			}
		}

		/// <summary>
		/// Преобразует путь в <see cref="T:System.Uri"/>.
		/// </summary>
		/// <param name="path">Путь.</param>
		/// <param name="uriKind">Допустимый тип пути.</param>
		/// <returns><see cref="T:System.Uri"/>.</returns>
		// Не реализованы имена хостов с русскими буквами
		[NotNull]
		[Pure]
		public static Uri PathToUri([NotNull][PathReference] string path, UriKind uriKind = UriKind.RelativeOrAbsolute)
		{
			if (path == null) throw new ArgumentNullException("path");

			var match = _path_regexp.Match(path);

			if (!match.Success)
				throw new FormatException("Неизвестный формат пути");
			else
			{
				var groups = match.Groups;
				if (groups["relative"].Success)
					if (groups["disk"].Success)
						throw new ArgumentException("Относительный формат пути с указанием диска не преобразуем в Uri", "path");
					else if ((uriKind != UriKind.RelativeOrAbsolute && uriKind != UriKind.Relative))
						throw new FormatException("Запрещённый формат пути");
					else
					{
						var uri = new Uri(UriBuilderEscapeCompensate(path), UriKind.Relative);
						return uri.ThrowIfMatch("Неправильный Uri", "uri", uri2 => uri2.UserEscaped);
					}
				else if (groups["absolute"].Success)
				{
					if ((uriKind != UriKind.RelativeOrAbsolute && uriKind != UriKind.Absolute))
						throw new FormatException("Запрещённый формат пути");
					else
					{
						var ub = new UriBuilder()
						{
							Scheme = Uri.UriSchemeFile,
							Host = string.Empty,
							Path = UriBuilderEscapeCompensate(path),
						};
						var uri = ub.Uri.ThrowIfNull("Uri");
						/*
						var uri = new Uri(path, UriKind.Absolute);
						*/
						return uri.ThrowIfMatch("Неправильный Uri", "uri", uri2 => uri2.Scheme != Uri.UriSchemeFile || uri2.UserEscaped);
					}
				}
				else if (groups["UNC"].Success)
				{
					if ((uriKind != UriKind.RelativeOrAbsolute && uriKind != UriKind.Absolute))
						throw new FormatException("Запрещённый формат пути");
					else
					{
						var host = groups["host"];
						if (!host.Success)
							throw new RegExpException("Ошибка в регулярном выражении");
						else
						{
							var path2 = groups["path"];
							if (!path2.Success)
								throw new RegExpException("Ошибка в регулярном выражении");
							else
							{
								var ub = new UriBuilder()
								{
									Scheme = Uri.UriSchemeFile,
									Host = host.Value,
									Path = UriBuilderEscapeCompensate(path2.Value),
								};
								Uri uri;
								try
								{
									uri = ub.Uri.ThrowIfNull("Uri");
								}
								catch (UriFormatException)
								{
									ub.Scheme = UriSchemeUnsafeFile;
									uri = ub.Uri.ThrowIfNull("Uri");
								}
								/*
								var uri = new Uri(path, UriKind.Absolute);
								*/
								return uri.ThrowIfMatch("Неправильный Uri", "uri", uri2 => (uri2.Scheme != Uri.UriSchemeFile && uri2.Scheme != UriSchemeUnsafeFile) || uri2.UserEscaped);
							}
						}
					}
				}
				else
					throw new RegExpException("Ошибка в регулярном выражении");
			}
		}

		/// <summary>
		/// Преобразует абсолютный путь в <see cref="T:System.Uri"/>.
		/// </summary>
		/// <param name="path">Путь.</param>
		/// <returns><see cref="T:System.Uri"/>.</returns>
		[NotNull]
		[Pure]
		public static Uri AbsolutePathToUri([NotNull][PathReference] string path)
		{
			if (path == null) throw new ArgumentNullException("path");

			return PathToUri(path, UriKind.Absolute);
		}

		/// <summary>
		/// Преобразует относительный путь в <see cref="T:System.Uri"/>.
		/// </summary>
		/// <param name="path">Путь.</param>
		/// <returns><see cref="T:System.Uri"/>.</returns>
		[NotNull]
		[Pure]
		public static Uri RelativePathToUri([NotNull] [PathReference] string path)
		{
			if (path == null) throw new ArgumentNullException("path");

			return PathToUri(path, UriKind.Relative);
		}

		/// <summary>
		/// Преобразует <see cref="T:System.Uri"/> в путь к папке или файлу.
		/// </summary>
		/// <param name="path"><see cref="T:System.Uri"/>.</param>
		/// <returns>Путь.</returns>
		[NotNull]
		[Pure]
		public static string UriToPath([NotNull] Uri path)
		{
			if (path == null) throw new ArgumentNullException("path");

			string str;
			if (!path.IsAbsoluteUri)
			{
				str = path.GetComponents(UriComponents.SerializationInfoString, UriFormat.Unescaped);
			}
			else
			{
				if (path.IsFile)
				{
					str = path.LocalPath;
				}
				else if (path.Scheme == PathExtensions.UriSchemeUnsafeFile)
				{
					str = path.GetComponents(UriComponents.Fragment | UriComponents.Query | UriComponents.Path | UriComponents.Port | UriComponents.Host | UriComponents.UserInfo | UriComponents.KeepDelimiter, UriFormat.Unescaped).ThrowIfNull("result_filename");
					if (Path.DirectorySeparatorChar != '/')
						str = str.Replace('/', Path.DirectorySeparatorChar);
					str = string.Format("{1}{1}{0}", str, Path.DirectorySeparatorChar);
				}
				else
					throw new ArgumentException("Не файл", "path");
			}
			str.ThrowIfNull("str");
			return str;
		}

		[NotNull]
		[Pure]
		private static string GetSubPath([NotNull] string[] fullLines, int i)
		{
			if (fullLines == null) throw new ArgumentNullException("fullLines");

			var full_bit = fullLines.Concat(new string[] {string.Empty,});

			string full_part = string.Join(Path.DirectorySeparatorChar.ToString(), full_bit);
			return full_part;
		}

		[Obsolete("Эта функция валит приложение при неправильном использовании. Используйте PathCompactPathEx2", false)]
		[DllImport("shlwapi.dll")]
		private static extern bool PathCompactPathEx([NotNull, Out] StringBuilder pszOut, [NotNull][PathReference] string szPath, int cchMax, int dwFlags);

		[CanBeNull]
		private static string PathCompactPathExLight([NotNull][PathReference] string path, int length)
		{
			if (path == null) throw new ArgumentNullException("path");

			if (length < 0)
				throw new ArgumentOutOfRangeException("length", length, "< 0");

			if (path.Length < length)
				return path;

			var sb = new StringBuilder(length, length);
#pragma warning disable 612,618
			PathCompactPathEx(sb, path, length, 0);
#pragma warning restore 612,618
			return sb.ToString();
		}

		[NotNull]
		[Pure]
		private static string PathCompactPathEx2([NotNull][PathReference] string path, int length)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length <= length) // Не нужно сокращать
				return path;

			const string ellipsis = "…";
			//const string ellipsis = "...";

			if (ellipsis.Length > length)
				throw new ArgumentOutOfRangeException("length", length, "< " + ellipsis.Length);

			int file_name_pos = Math.Max(path.LastIndexOf(Path.DirectorySeparatorChar), path.LastIndexOf(Path.AltDirectorySeparatorChar));

			{
				int old_separator_pos = path.Length - 1;

				while (old_separator_pos == file_name_pos && file_name_pos > 0) // Все последние разделители относим к имени
				{
					old_separator_pos = file_name_pos - 1;
					file_name_pos = Math.Max(path.LastIndexOf(Path.DirectorySeparatorChar, old_separator_pos),
						path.LastIndexOf(Path.AltDirectorySeparatorChar, old_separator_pos));
				}
			}

			if (file_name_pos < 0) // Если разделителей нет, весь путь считаем именем файла
				file_name_pos = 0;

			int file_name_len = path.Length - file_name_pos;

			if (file_name_len > length - ellipsis.Length) // Даже имя файла слишком длинное
			{
				if (length < ellipsis.Length*2) // Не влезает даже 2 многоточия
					return ellipsis;
				else
					return new StringBuilder(length, length)
						.Append(ellipsis)
						.Append(path.Substring(file_name_pos, length - ellipsis.Length*2))
						.Append(ellipsis)
						.ToString();
			}
			else
				return new StringBuilder(length, length)
					.Append(path.Substring(0, length - file_name_len - ellipsis.Length))
					.Append(ellipsis)
					.Append(path.Substring(file_name_pos))
					.ToString();
		}

		[NotNull]
		[Pure]
		public static string TruncatePath([CanBeNull][PathReference] string path, [CanBeNull] Font font, int width)
		{
			if (string.IsNullOrEmpty(path) || width < 1)
				return string.Empty;

			if (font == null)
			{
				return PathCompactPathEx2(path, width);
			}

			string tmp = path;
			int length = tmp.Length;

			while (_graphics.MeasureString(tmp, font).Width > width)
			{
				length--;
				tmp = PathCompactPathEx2(path, length);
			}

			return tmp;
		}

		#endregion
	}
}
