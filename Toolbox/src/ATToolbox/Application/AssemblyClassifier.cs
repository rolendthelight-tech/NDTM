using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using AT.Toolbox.Extensions;
using log4net;
using AT.Toolbox.Files;
using System.IO;

namespace AT.Toolbox
{
  /// <summary>
  /// Классификатор сборок, загруженных в домен
  /// </summary>
  public interface IAssemblyClassifier : IDisposable
  {
    /// <summary>
    /// Список загруженных в домен управляемых сборок
    /// </summary>
    IList<string> LoadedAssemblies { get; }

    /// <summary>
    /// Список путей, по которым находятся неуправляемые сборки
    /// </summary>
    IList<string> UnmanagedAssemblies { get; }

    /// <summary>
    /// Список сборок, обрабатываемых тулбоксом
    /// </summary>
    IList<Assembly> ToolboxAssemblies { get; }

    /// <summary>
    /// Список плагинов, загруженных помимо статически прописанных зависимостей
    /// </summary>
    IList<Assembly> Plugins { get; }

    /// <summary>
    /// Ссылка на сборщик зависимостей
    /// </summary>
    IAssemblyTreeBuilder TreeBuilder { get; }

    /// <summary>
    /// Загрузка плагинов
    /// </summary>
    void FindAndLoadPlugins();

    /// <summary>
    /// Загрузка плагинов
    /// </summary>
    /// <param name="pluginFilenameRegex">Регулярное выражения для поиска файлов описывающих сборки плагинов</param>
    void FindAndLoadPlugins(string pluginFilenameRegex);
  }

  public class AssemblyClassifier : IAssemblyClassifier
  {
    private static readonly ILog _log = LogManager.GetLogger("AssemblyClassifier");
    private static readonly Type _asm_type = typeof(AssemblyClassifier).Assembly.GetType();
  
    private readonly List<string> m_loaded_assemblies;
    private readonly ReadOnlyCollection<string> m_loaded_assemblies_out;
    private readonly List<Assembly> m_toolbox_assemblies ;
    private readonly ReadOnlyCollection<Assembly> m_toolbox_assemblies_out;
    private readonly List<string> m_unmanaged_asms;
    private readonly ReadOnlyCollection<string> m_unmanaged_asms_out;
    
    private readonly List<Assembly> m_plugins;
    private readonly ReadOnlyCollection<Assembly> m_plugins_out;

    protected readonly IList<PluginInfo> _PluginsInfo;
    private readonly ReadOnlyCollection<PluginInfo> m_pluginsInfo_out;

    private readonly IAssemblyTreeBuilder m_tree_builder;
    protected readonly object _Lock = new object();

    public AssemblyClassifier(IAssemblyTreeBuilder treeBuilder)
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
        //TODO Изменить проверку, определять что полное имя не задано, и использовать краткое
        //if (a.ManifestModule.FullyQualifiedName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
          asms.Add(a);

      m_loaded_assemblies.AddRange(asms.Select(asm => asm.ManifestModule.FullyQualifiedName));
      m_toolbox_assemblies.AddRange(asms.Where(asm => asm.IsAtToolbox()));

      AppDomain.CurrentDomain.AssemblyLoad += this.AddAssembly;
    }

    public IList<string> LoadedAssemblies
    {
      get { return m_loaded_assemblies_out; }
    }

    public IList<string> UnmanagedAssemblies
    {
      get { return m_unmanaged_asms_out; }
    }

    public IList<Assembly> ToolboxAssemblies
    {
      get { return m_toolbox_assemblies_out; }
    }

    public IList<Assembly> Plugins
    {
      get { return m_plugins_out; }
    }

    public IList<PluginInfo> PluginsInfo
    {
      get { return m_pluginsInfo_out; }
    }

    public IAssemblyTreeBuilder TreeBuilder
    {
      get { return m_tree_builder; }
    }

    private void AddAssembly(object sender, AssemblyLoadEventArgs e)
    {
      if (e.LoadedAssembly.GetType() != _asm_type || string.IsNullOrEmpty(e.LoadedAssembly.Location))
        return;

      lock (_Lock)
      {
        var name = e.LoadedAssembly.ManifestModule.FullyQualifiedName;

        if (!m_loaded_assemblies.Contains(name))
          m_loaded_assemblies.Add(name);

        if (e.LoadedAssembly.IsAtToolbox() && !m_toolbox_assemblies.Contains(e.LoadedAssembly))
          m_toolbox_assemblies.Add(e.LoadedAssembly);
      }
    }

    public void FindAndLoadPlugins()
    {
      FindAndLoadPlugins(@".*\.plugin$");
    }

    private string GetStartupPath()
    {
      using (var p = System.Diagnostics.Process.GetCurrentProcess())
      {
        var ret = Path.GetDirectoryName(p.MainModule.FileName);

        return ret;
      }
    }

    public void FindAndLoadPlugins(string pluginFilenameRegex)
    {

      foreach (string path in FSUtils.FindFiles(this.GetStartupPath(), pluginFilenameRegex, true))
      {
        PluginInfo pluginInfo = GetPluginInfo(path);
        
        if (pluginInfo == null)
        {
          continue;
        }

        if (!File.Exists(pluginInfo.AssemblyFile))
        {
          _log.DebugFormat("FindAndLoadPlugins(): file '{0}' not found", pluginInfo.AssemblyFile);
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
          }
        }
        catch (FileLoadException fex)
        {
          _log.Debug("FindAndLoadPlugins(): FileLoadException", fex);
          m_unmanaged_asms.Add(pluginInfo.AssemblyFile);
        }
        catch (Exception ex)
        {
          _log.Error("FindAndLoadPlugins(): exception", ex);
          m_unmanaged_asms.Add(pluginInfo.AssemblyFile);
        }
      }
    }

    protected virtual PluginInfo GetPluginInfo(string path)
    {
      return new PluginManifestReader().ReadManifest(path);
    }

    public void Dispose()
    {
      AppDomain.CurrentDomain.AssemblyLoad -= this.AddAssembly;
    }
  }
}
