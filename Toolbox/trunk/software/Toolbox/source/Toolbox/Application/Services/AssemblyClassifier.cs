using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reflection;
using JetBrains.Annotations;
using Toolbox.Common.Files;
using Toolbox.Extensions;
using log4net;
using System.IO;
using System.Xml;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Классификатор сборок, загруженных в домен
  /// </summary>
  public interface IAssemblyClassifier : IDisposable
  {
    /// <summary>
    /// Список загруженных в домен управляемых сборок
    /// </summary>
    [NotNull]
    IList<string> LoadedAssemblies { get; }

    /// <summary>
    /// Список путей, по которым находятся неуправляемые сборки
    /// </summary>
    [NotNull]
    IList<string> UnmanagedAssemblies { get; }

    /// <summary>
    /// Список сборок, обрабатываемых тулбоксом
    /// </summary>
    [NotNull]
    IList<Assembly> ToolboxAssemblies { get; }

    /// <summary>
    /// Список плагинов, загруженных помимо статически прописанных зависимостей
    /// </summary>
    [NotNull]
    IList<Assembly> Plugins { get; }

    /// <summary>
    /// Ссылка на сборщик зависимостей
    /// </summary>
    [NotNull]
    IAssemblyTreeBuilder TreeBuilder { get; }

    /// <summary>
    /// Загрузка плагинов
    /// </summary>
    void FindAndLoadPlugins();

    /// <summary>
    /// Загрузка плагинов
    /// </summary>
    /// <param name="pluginFilenameRegex">Регулярное выражения для поиска файлов описывающих сборки плагинов</param>
    void FindAndLoadPlugins([CanBeNull] string pluginFilenameRegex);

    /// <summary>
    /// Загрузка плагинов
    /// </summary>
    /// <param name="searchPath">Путь поиска сборок плагинов</param>
    /// <param name="pluginFilenameRegex">Регулярное выражения для поиска файлов описывающих сборки плагинов</param>
		void FindAndLoadPlugins([NotNull] [PathReference] string searchPath, [CanBeNull] string pluginFilenameRegex);
  }

  public class AssemblyClassifier : IAssemblyClassifier
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyClassifier));
	  [NotNull] private static readonly Type _asm_type = typeof(AssemblyClassifier).Assembly.GetType();

	  [NotNull] private readonly List<string> m_loaded_assemblies;
	  [NotNull] private readonly ReadOnlyCollection<string> m_loaded_assemblies_out;
	  [NotNull] private readonly List<Assembly> m_toolbox_assemblies ;
	  [NotNull] private readonly ReadOnlyCollection<Assembly> m_toolbox_assemblies_out;
	  [NotNull] private readonly List<string> m_unmanaged_asms;
	  [NotNull] private readonly ReadOnlyCollection<string> m_unmanaged_asms_out;

	  [NotNull] private readonly List<Assembly> m_plugins;
	  [NotNull] private readonly ReadOnlyCollection<Assembly> m_plugins_out;

	  [NotNull] protected readonly IList<PluginInfo> _PluginsInfo;
	  [NotNull] private readonly ReadOnlyCollection<PluginInfo> m_pluginsInfo_out;

	  [NotNull] private readonly IAssemblyTreeBuilder m_tree_builder;
	  [NotNull] protected readonly object _Lock = new object();

    public AssemblyClassifier([NotNull] IAssemblyTreeBuilder treeBuilder)
    {
      if (treeBuilder == null)
        throw new ArgumentNullException("treeBuilder");

      m_tree_builder = treeBuilder;
      m_loaded_assemblies = new List<string>();
      m_toolbox_assemblies = new List<Assembly>();
      m_unmanaged_asms = new List<string>();

      m_plugins = new List<Assembly>();
      _PluginsInfo = new List<PluginInfo>();
      m_loaded_assemblies_out = new ReadOnlyCollection<string>(m_loaded_assemblies);
      m_toolbox_assemblies_out = new ReadOnlyCollection<Assembly>(m_toolbox_assemblies);
      m_unmanaged_asms_out = new ReadOnlyCollection<string>(m_unmanaged_asms);

      m_plugins_out = new ReadOnlyCollection<Assembly>(m_plugins);
      m_pluginsInfo_out = new ReadOnlyCollection<PluginInfo>(_PluginsInfo);

      var asms = new List<Assembly>();
      foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (a.GlobalAssemblyCache) // Плагин не может быть установлены в GAC
          continue;
        if (a.GetType() != _asm_type || string.IsNullOrEmpty(a.Location)) // Плагин должен быть отдельным файлом
          continue;

        //TODO Изменить проверку, определять что полное имя не задано, и использовать краткое
        //if (a.ManifestModule.FullyQualifiedName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
        asms.Add(a);
      }

      m_loaded_assemblies.AddRange(asms.Select(asm => asm.ManifestModule.FullyQualifiedName));
      m_toolbox_assemblies.AddRange(asms.Where(asm => asm.IsToolboxSearchable()));

      AppDomain.CurrentDomain.AssemblyLoad += this.AddAssembly;
    }

	  [NotNull]
	  public IList<string> LoadedAssemblies
    {
      get { return m_loaded_assemblies_out; }
    }

	  [NotNull]
	  public IList<string> UnmanagedAssemblies
    {
      get { return m_unmanaged_asms_out; }
    }

	  [NotNull]
	  public IList<Assembly> ToolboxAssemblies
    {
      get { return m_toolbox_assemblies_out; }
    }

	  [NotNull]
	  public IList<Assembly> Plugins
    {
      get { return m_plugins_out; }
    }

	  [NotNull]
	  public IList<PluginInfo> PluginsInfo
    {
      get { return m_pluginsInfo_out; }
    }

	  [NotNull]
	  public IAssemblyTreeBuilder TreeBuilder
    {
      get { return m_tree_builder; }
    }

    private void AddAssembly([NotNull] object sender, [NotNull] AssemblyLoadEventArgs e)
    {
	    if (sender == null) throw new ArgumentNullException("sender");
	    if (e == null) throw new ArgumentNullException("e");

			if (e.LoadedAssembly.GetType() != _asm_type || string.IsNullOrEmpty(e.LoadedAssembly.Location))
        return;

      lock (_Lock)
      {
        var name = e.LoadedAssembly.ManifestModule.FullyQualifiedName;

        if (!m_loaded_assemblies.Contains(name))
          m_loaded_assemblies.Add(name);

        if (e.LoadedAssembly.IsToolboxSearchable() && !m_toolbox_assemblies.Contains(e.LoadedAssembly))
          m_toolbox_assemblies.Add(e.LoadedAssembly);
      }
    }

    public void FindAndLoadPlugins()
    {
      FindAndLoadPlugins(@"\.plugin$");
    }

	  [CanBeNull]
	  private string GetStartupPath()
    {
      using (var p = System.Diagnostics.Process.GetCurrentProcess())
      {
        var ret = Path.GetDirectoryName(p.MainModule.FileName);

        return ret;
      }
    }

    public void FindAndLoadPlugins([CanBeNull] string pluginFilenameRegex)
    {
      var searchPath = this.GetStartupPath();
      FindAndLoadPlugins(searchPath, pluginFilenameRegex);
    }

		public void FindAndLoadPlugins([NotNull] [PathReference] string searchPath, [CanBeNull] string pluginFilenameRegex)
    {
	    if (searchPath == null) throw new ArgumentNullException("searchPath");

			var paths = FSUtils.FindFiles(searchPath, pluginFilenameRegex, true);
      if (paths.Count == 0)
        _log.WarnFormat("FindAndLoadPlugins(): Plugin files not found for regex \"{0}\", search path \"{1}\"", pluginFilenameRegex, searchPath);

      foreach (string path in paths)
      {
        PluginInfo pluginInfo = GetPluginInfo(path);

        if (pluginInfo == null)
        {
          continue;
        }

        if (!File.Exists(pluginInfo.AssemblyFile))
        {
          _log.WarnFormat("FindAndLoadPlugins(): file '{0}' not found", pluginInfo.AssemblyFile);
          continue;
        }

        try
        {
          if (m_unmanaged_asms.Contains(pluginInfo.AssemblyFile))
          {
            continue;
          }

          Assembly assembly;

          if (!m_loaded_assemblies.Contains(pluginInfo.AssemblyFile))
          {
            assembly = Assembly.LoadFrom(pluginInfo.AssemblyFile);
          }
          else
          {
            var name2_search = Path.GetFileName(pluginInfo.AssemblyFile);

            assembly = AppDomain.CurrentDomain.GetAssemblies().Single(
              asm =>
              asm.ManifestModule.ScopeName == name2_search);
          }

          pluginInfo.Assembly = assembly;

          lock (_Lock)
          {
            if (!m_plugins.Contains(assembly))
            {
              m_plugins.Add(assembly);
            }

            if (!PluginsInfo.Contains(pluginInfo))
            {
              _PluginsInfo.Add(pluginInfo);
            }

            _log.DebugFormat(Properties.Resources.PLUGIN_LOADED, pluginInfo);
          }
        }
        catch (FileLoadException fex)
        {
          _log.Error("FindAndLoadPlugins(): FileLoadException", fex);
          m_unmanaged_asms.Add(pluginInfo.AssemblyFile);
        }
        catch (Exception ex)
        {
          _log.Error("FindAndLoadPlugins(): exception", ex);
          m_unmanaged_asms.Add(pluginInfo.AssemblyFile);
        }
      }

      _log.DebugFormat("FindAndLoadPlugins(): Added plugins = {0}", m_plugins.Count);
    }

	  [CanBeNull]
	  protected virtual PluginInfo GetPluginInfo([NotNull] [PathReference] string path)
    {
	    if (path == null) throw new ArgumentNullException("path");

      PluginInfo info = new PluginInfo();

      var x_doc = new XmlDocument();
      x_doc.Load(path);

      var node = x_doc.SelectSingleNode("/plugin/@assembly");

      if (node != null)
      {
        info.AssemblyFile = node.InnerText;

        if (!Path.IsPathRooted(info.AssemblyFile))
        {
          info.AssemblyFile = Path.Combine(Path.GetDirectoryName(path), info.AssemblyFile);
        }
      }
      else
      {
        return null;
      }

      node = x_doc.SelectSingleNode("/plugin/@name");

      info.Name = node != null
                    ? node.InnerText
                    : Path.GetFileNameWithoutExtension(info.AssemblyFile);

      return info;
    }

	  public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyLoad -= this.AddAssembly;
    }
  }
}
