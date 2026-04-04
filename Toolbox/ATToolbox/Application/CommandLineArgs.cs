using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AT.Toolbox
{

  /// <summary>
  /// Аргументы коммандной строки
  /// </summary>
  public class CommandLineArgs
  {
    private string[] _args;

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
    public Dictionary<string, List<string>> Keys { get; private set; }

    /// <summary>
    /// Оператор индексации для получения ключей командной строки
    /// </summary>
    /// <param name="keyName">название ключа</param>
    /// <returns>значение ключа, если ключей несколько, то значения будут объединены через перевод строки</returns>
    public string this[string keyName]
    {
      get
      {
        var v = Keys.Where(a => a.Key == keyName).Select(a => a.Value).SelectMany(a => a).ToArray();
        if (v.Length == 0)
          return null;
        return string.Join(Environment.NewLine, v);
      }
    }

    public bool ContainsKey(string regexPattern)
    {
      return ContainsKey(regexPattern, RegexOptions.IgnoreCase);
    }

    public bool ContainsKey(string regexPattern, RegexOptions options)
    {
      return Keys.Any(a => Regex.IsMatch(a.Key, regexPattern, options));
    }

    public override string ToString()
    {
      if (_args.Length == 0)
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
        args = new string[0];
      
      _args = args;

      Keys = new Dictionary<string, List<string>>();
      foreach (var arg in args)
      {
        Match m = Regex.Match(arg.Trim(), @"[/-](?<key>[\w]+)([=](?<value>[^=]+)){0,1}");
        if (m.Success)
        {
          List<string> values;

          if (!Keys.TryGetValue(m.Groups["key"].Value, out values))
          {
            values = new List<string>();
            Keys.Add(m.Groups["key"].Value, values);
          }

          values.Add(m.Groups["value"].Value);
        }
      }
    }
  }
}
