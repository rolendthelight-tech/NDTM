using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.Extensions;

namespace Toolbox.Configuration
{
  public interface IConfigFileFinder
  {
    string InputConfigFilePath { get; }
    string OutputConfigFilePath { get; }
  }

  public class ConfigFileFinder : IConfigFileFinder
  {
    //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ConfigFileFinder));

		private bool m_io_config_file_pathes_initialized = false;
		private readonly object m_io_config_file_pathes_lock = new object();
		private string m_input_config_file_path;
		private string m_output_config_file_path;
    private readonly string[] m_command_line_args;
    private readonly string m_config_file_name;
    private readonly Version m_current_version;
    private readonly bool _useCurrentVersionOnly;

    /// <summary>
    /// Путь к входному файлу. Если входной файл не существует возвращает <code>null</code>.
    /// </summary>
    public string InputConfigFilePath
    {
      get
      {
	      if (!m_io_config_file_pathes_initialized)
	      {
		      lock (m_io_config_file_pathes_lock)
		      {
			      if (!m_io_config_file_pathes_initialized)
			      {
							Thread.MemoryBarrier();
							Initialize();
							Thread.MemoryBarrier();
							m_io_config_file_pathes_initialized = true;
						}
		      }
	      }

        return m_input_config_file_path;
      }
    }

    public string OutputConfigFilePath
    {
      get
      {
	      if (!m_io_config_file_pathes_initialized)
	      {
		      lock (m_io_config_file_pathes_lock)
		      {
			      if (!m_io_config_file_pathes_initialized)
			      {
							Thread.MemoryBarrier();
							Initialize();
							Thread.MemoryBarrier();
							m_io_config_file_pathes_initialized = true;
						}
		      }
	      }

        return m_output_config_file_path;
      }
    }

    public static string BaseFolder
    {
      get
      {
        var p0 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var p1 = ApplicationInfo.MainAttributes.Company;
        var p2 = ApplicationInfo.MainAttributes.ProductName;
        return Path.Combine(p0, p1 ?? "", p2 ?? "");
      }
    }

		public ConfigFileFinder([PathReference] string configFileName) :
      this(configFileName, false)
    {
    }

		public ConfigFileFinder([CanBeNull] IEnumerable<string> args, [PathReference] string configFileName) :
      this(args, configFileName, null, false)
    {
      var a = Assembly.GetEntryAssembly();
      if (a != null)
      {
        var n = a.GetName();
        m_current_version = n.Version;
      }
    }

		public ConfigFileFinder([PathReference] string configFileName, bool useCurrentVersionOnly) :
      this(null, configFileName, null, useCurrentVersionOnly)
    {
      var a = Assembly.GetEntryAssembly();
      if (a != null)
      {
        var n = a.GetName();
        m_current_version = n.Version;
      }
    }

		public ConfigFileFinder([CanBeNull] IEnumerable<string> args, [PathReference] string configFileName, Version currentVersion, bool useCurrentVersionOnly)
    {
			m_config_file_name = configFileName;
      m_current_version = currentVersion;
      _useCurrentVersionOnly = useCurrentVersionOnly;

      if (string.IsNullOrEmpty(m_config_file_name))
        m_config_file_name = "Settings.xml";

      if (args != null)
        m_command_line_args = args.ToArray();
    }

    /// <summary>
    /// Определение пути к файлу конфигруаций.
    /// </summary>
    private void Initialize()
    {
      // Формируем файл на основе аргументов, не проверяя его наличие!
	    string input_config_file_path;
			string output_config_file_path;

      input_config_file_path = GetConfigFilePathFromCommandLine(m_command_line_args);
	    if (!string.IsNullOrEmpty(input_config_file_path))
	    {
		    output_config_file_path = input_config_file_path;
		    if (!File.Exists(input_config_file_path))
			    input_config_file_path = null;
	    }
	    else
	    {
		    // Формируем стандартный путь к файлу
				string standard_path = Path.Combine(BaseFolder, m_current_version == null ? "" : m_current_version.ToString(), m_config_file_name);
		    output_config_file_path = standard_path;

		    // Ищем файл по стандартным путям для текущей версии
		    if (File.Exists(standard_path))
		    {
			    input_config_file_path = standard_path;
		    }
		    else
		    {
			    bool found = false;

			    // Ищем файл по стандартным путям для любой версии
			    if (!_useCurrentVersionOnly)
			    {
				    input_config_file_path = FindConfigFileInAppDataFolder(m_config_file_name);
				    if (!string.IsNullOrEmpty(input_config_file_path))
				    {
							found = true;
				    }
			    }
			    if (!found)
			    {
				    string module_path;
				    // Ищем файл рядом с приложением (файл только для чтения, созданный при установке дистрибутива)
				    using (var process = System.Diagnostics.Process.GetCurrentProcess())
				    {
					    module_path = process.MainModule.FileName;
				    }
						var module_dir = Path.GetDirectoryName(module_path);
				    if (module_dir != null)
				    {
					    input_config_file_path = Path.Combine(module_dir, m_config_file_name);
					    if (!File.Exists(input_config_file_path))
						    input_config_file_path = null;
				    }
			    }
		    }
	    }

	    m_input_config_file_path = input_config_file_path;
	    m_output_config_file_path = output_config_file_path;
    }

    private string GetConfigFilePathFromCommandLine(IEnumerable<string> args)
    {
      if (args != null)
      {
	      bool found = false;
	      foreach (var arg in args)
	      {
		      if (found)
		      {
			      return arg;
		      }
					else if (Regex.IsMatch(arg, "^(?:--?|/)conf", RegexOptions.IgnoreCase | RegexOptions.Singleline))
		      {
			      found = true;
		      }
	      }
      }

      return null;
    }

		private string FindMostRecentVersion([NotNull] IEnumerable<string> dirs, [NotNull] [PathReference] string config_file_name)
    {
	    if (dirs == null) throw new ArgumentNullException("dirs");
	    if (config_file_name == null) throw new ArgumentNullException("config_file_name");

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

    private string[] GetSortedOnVersions([NotNull] IEnumerable<string> dirs)
    {
	    if (dirs == null) throw new ArgumentNullException("dirs");

			return dirs
        .Select(dir => new { Dir = dir, VersionStr = Path.GetFileName(dir) })
        .Where(dir => Regex.IsMatch(dir.VersionStr, @"\d+(\.\d+){0,3}"))
				.Select(dir =>
					{
						Version ver;
						return new { Success = Version.TryParse(dir.VersionStr, out ver), Version = ver, dir.Dir, };
					})
				.Where(ver => ver.Success)
				.Select(ver => new { ver.Version, ver.Dir, })
				.Where(ver => ver.Version <= m_current_version)
        .OrderByDescending(ver => ver.Version)
        .Select(ver => ver.Dir)
        .ToArray();
    }

		private string FindConfigFileInAppDataFolder([NotNull] [PathReference] string config_file_name)
    {
		  if (config_file_name == null) throw new ArgumentNullException("config_file_name");

			if (!Directory.Exists(BaseFolder))
        Directory.CreateDirectory(BaseFolder);

      var ret = FindMostRecentVersion(Directory.GetDirectories(BaseFolder), config_file_name);

      if (string.IsNullOrEmpty(ret))
      {
        string common_path = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
          ApplicationInfo.MainAttributes.Company ?? "",
          ApplicationInfo.MainAttributes.ProductName);

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
        return ArrayExtensions.Empty<string>();

      return f.GetSortedOnVersions(Directory.GetDirectories(BaseFolder));
    }
  }
}