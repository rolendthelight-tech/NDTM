using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Application
{
  /// <summary>
  /// Аргументы командной строки
  /// </summary>
  public class CommandLineArgs
  {
    private readonly string[] _args;
  	private readonly ILookup<string, string> _keys;

  	/// <summary>
    /// Оператор индексации для получения аргументов командной строки
    /// </summary>
    /// <param name="index">индекс аргумента</param>
    /// <returns>аргумент командной строки</returns>
    public string this[int index]
    {
      get
      {
        return _args.Length > index && index >= 0 ? _args[index] : null;
      }
    }

    /// <summary>
    /// Количество аргументов
    /// </summary>
    public int Count { get { return _args.Length; } }

    /// <summary>
    /// Ключи командной строки
    /// </summary>
		public ILookup<string, string> Keys { get { return _keys; } }

    /// <summary>
    /// Оператор индексации для получения ключей командной строки
    /// </summary>
    /// <param name="keyName">название ключа</param>
    /// <returns>значение ключа, если ключей несколько, то значения будут объединены через перевод строки</returns>
    public string this[[NotNull] string keyName]
    {
      get
      {
	      if (keyName == null) throw new ArgumentNullException("keyName");

				var v = Keys[keyName];
        if (!v.Any())
          return null;
        return string.Join(Environment.NewLine, v);
      }
    }

    public bool ContainsKey([NotNull] string regexPattern)
    {
	    if (regexPattern == null) throw new ArgumentNullException("regexPattern");

			return ContainsKey(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }

	  public bool ContainsKey([NotNull] string regexPattern, RegexOptions options)
    {
	    if (regexPattern == null) throw new ArgumentNullException("regexPattern");

			return Keys.Any(a => Regex.IsMatch(a.Key, regexPattern, options));
    }

	  public override string ToString()
    {
			if (_args == null || _args.Length == 0)
        return string.Empty;

      var args = new string[_args.Length];

      for (int i = 0; i < args.Length; i++)
      {
        var arg = _args[i];

        if (arg.Contains(' ') || arg.Contains('\t'))
          args[i] = string.Format("\"{0}\"", arg);
        else
          args[i] = arg;
      }

      return string.Join(" ", args);
    }

		public CommandLineArgs(string[] args)
		{
			if (args == null)
				args = ArrayExtensions.Empty<string>();

			_args = args;

			_keys = args
        .Select(arg => Regex.Match(arg, @"[/-](?<key>[\w]+)([=](?<value>[^=]+)){0,1}"))
				.Where(m => m.Success)
				.Select(m => new
				{
					Key = m.Groups["key"].Value,
					Value = m.Groups["value"].Value,
				})
				.ToLookup(pair => pair.Key, pair => pair.Value);
		}

		public CommandLineArgs(ILookup<string, string> keys)
		{
			if (keys == null)
				keys = Enumerable.Empty<string>().ToLookup(str => str);

			_args = null;

			_keys = keys; // TODO: Копировать
		}
	}
}
