using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AT.Toolbox
{
  public interface IConfigFileFinder
  {
    string InputConfigFilePath { get; }
    string OutputConfigFilePath { get; }
  }

  public class ConfigFileFinder : IConfigFileFinder
  {
    //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger("ConfigFileFinder");

    private string m_input_config_file_path;
    private string m_output_config_file_path;
    private IList<string> m_command_line_args;
    private string m_config_file_name;
    private Version m_current_version;
    private bool _useCurrentVersionOnly;

    /// <summary>
    /// ???? ? ???????? ?????. ???? ??????? ???? ?? ?????????? ?????????? null.
    /// </summary>
    public string InputConfigFilePath
    {
      get
      {
        if (m_input_config_file_path == null)
          Initialize();

        return m_input_config_file_path;
      }
    }

    public string OutputConfigFilePath
    {
      get
      {
        if (m_output_config_file_path == null)
          Initialize();

        return m_output_config_file_path;
      }
    }

    public static string BaseFolder
    {
      get
      {
        var p0 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(p0))
          p0 = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        if (string.IsNullOrEmpty(p0))
        {
          var profile = Environment.GetEnvironmentVariable("USERPROFILE");
          if (!string.IsNullOrEmpty(profile))
            p0 = Path.Combine(profile, "AppData", "Local");
        }
        if (string.IsNullOrEmpty(p0))
          p0 = AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrEmpty(p0))
          p0 = Path.GetTempPath();
        var main = ApplicationInfo.MainAttributes;
        var p1 = main != null ? (main.Company ?? string.Empty) : string.Empty;
        var p2 = main != null ? (main.ProductName ?? string.Empty) : string.Empty;
        return Path.Combine(p0, Path.Combine(p1, p2));
      }
    }

    /// <summary>????????????? ?? ??????? ???????? (?????? ?????????????? ??? BasePath).</summary>
    public static string BasePath
    {
      get { return BaseFolder; }
    }

    public ConfigFileFinder(string config_file_name) :
      this(config_file_name, false)
    {
    }

    public ConfigFileFinder(IEnumerable<string> args, string config_file_name) :
      this(args, config_file_name, null, false)
    {
      var a = Assembly.GetEntryAssembly();
      if (a != null)
      {
        var n = a.GetName();
        m_current_version = n.Version;
      }
    }

    public ConfigFileFinder(string config_file_name, bool useCurrentVersionOnly) :
      this(null, config_file_name, null, useCurrentVersionOnly)
    {
      var a = Assembly.GetEntryAssembly();
      if (a != null)
      {
        var n = a.GetName();
        m_current_version = n.Version;
      }
    }

    public ConfigFileFinder(IEnumerable<string> args, string config_file_name, Version current_version, bool useCurrentVersionOnly)
    {
      m_config_file_name = config_file_name;
      m_current_version = current_version;
      _useCurrentVersionOnly = useCurrentVersionOnly;

      if (string.IsNullOrEmpty(m_config_file_name))
        m_config_file_name = "Settings.xml";

      if (args != null)
        m_command_line_args = new List<string>(args);
    }

    /// <summary>
    /// ??????????? ???? ? ????? ????????????.
    /// </summary>
    private void Initialize()
    {
      // ????????? ???? ?? ?????? ??????????, ?? ???????? ??? ???????!
      m_input_config_file_path = GetConfigFilePathFromCommandLine(m_command_line_args);
      if (!string.IsNullOrEmpty(m_input_config_file_path))
      {
        m_output_config_file_path = m_input_config_file_path;
        if (!File.Exists(m_input_config_file_path))
          m_input_config_file_path = null;
        return;
      }

      // ????????? ??????????? ???? ? ?????
      string standard_path = Path.Combine(BaseFolder, string.Format(@"{0}\{1}", m_current_version, m_config_file_name));
      m_output_config_file_path = standard_path;

      // ???? ???? ?? ??????????? ????? ??? ??????? ??????
      if (File.Exists(standard_path))
      {
        m_input_config_file_path = standard_path;
        return;
      }

      // ???? ???? ?? ??????????? ????? ??? ????? ??????
      if (!_useCurrentVersionOnly)
      {
        m_input_config_file_path = FindConfigFileInAppDataFolder(m_config_file_name);
        if (!string.IsNullOrEmpty(m_input_config_file_path))
        {
          return;
        }
      }

      // ???? ???? ????? ? ??????????? (???? ?????? ??? ??????, ????????? ??? ????????? ????????????)
      string moduleDir = null;
      try
      {
        var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
        if (mainModule != null && !string.IsNullOrEmpty(mainModule.FileName))
          moduleDir = Path.GetDirectoryName(mainModule.FileName);
      }
      catch
      {
        // ?????????? ?????? ? ????????? ?????????? (?????????? ??????, ????? ??????)
      }
      if (string.IsNullOrEmpty(moduleDir))
        moduleDir = AppDomain.CurrentDomain.BaseDirectory;
      if (string.IsNullOrEmpty(moduleDir))
        moduleDir = Directory.GetCurrentDirectory();
      m_input_config_file_path = Path.Combine(moduleDir, m_config_file_name);
      if (!File.Exists(m_input_config_file_path))
        m_input_config_file_path = null;
    }

    private string GetConfigFilePathFromCommandLine(IList<string> args)
    {
      if (args != null)
      {
        for (int i = 0; i < args.Count; i++)
        {
          string key = args[i];

          if (Regex.IsMatch(key, "[-/]conf", RegexOptions.IgnoreCase))
          {
            if (i + 1 < args.Count)
              return args[i + 1];
          }
        }
      }

      return null;
    }

    private string FindMostRecentVersion(IEnumerable<string> dirs, string config_file_name)
    {
      string[] dirs2 = GetSortedOnVersions(dirs);

      foreach (var dir in dirs2)
      {
        string file_path = Path.Combine(dir, config_file_name);
        if (File.Exists(file_path))
        {
          return file_path;
        }
      }

      return null;
    }

    private string[] GetSortedOnVersions(IEnumerable<string> dirs)
    {
      var withVer = dirs
        .Select(dir => new { Dir = dir, VersionStr = Path.GetFileName(dir) })
        .Where(dir => Regex.IsMatch(dir.VersionStr, @"\d+(\.\d+){0,3}"))
        .Select(dir => new { Version = new Version(dir.VersionStr), dir.Dir })
        .ToList();
      if (m_current_version == null)
      {
        return withVer
          .OrderByDescending(ver => ver.Version)
          .Select(ver => ver.Dir)
          .ToArray();
      }
      return withVer
        .Where(ver => ver.Version <= m_current_version)
        .OrderByDescending(ver => ver.Version)
        .Select(ver => ver.Dir)
        .ToArray();
    }

    private string FindConfigFileInAppDataFolder(string config_file_name)
    {
      if (!Directory.Exists(BaseFolder))
        Directory.CreateDirectory(BaseFolder);

      var ret = FindMostRecentVersion(Directory.GetDirectories(BaseFolder), config_file_name);

      if (string.IsNullOrEmpty(ret))
      {
        var ma = ApplicationInfo.MainAttributes;
        string common_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                          (ma != null ? (ma.Company ?? string.Empty) : string.Empty) + "\\" + (ma != null ? (ma.ProductName ?? string.Empty) : string.Empty));

        if (!Directory.Exists(common_path))
          return null;

        return FindMostRecentVersion(Directory.GetDirectories(common_path), config_file_name);
      }

      return ret;
    }

    public static string[] FindConfigFoldersInAppDataFolder()
    {
      var f = new ConfigFileFinder("");

      if (!Directory.Exists(BaseFolder))
        return new string [] {};

      return f.GetSortedOnVersions(Directory.GetDirectories(BaseFolder));
    }
  }
}