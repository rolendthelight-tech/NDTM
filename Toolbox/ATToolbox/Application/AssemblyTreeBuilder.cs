using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using AT.Toolbox.Files;
using System.IO;
using System.Collections.ObjectModel;

namespace AT.Toolbox
{
  /// <summary>
  /// Сборщик всех сборок, требуемых в дереве зависимостей приложения
  /// </summary>
  public interface IAssemblyTreeBuilder
  {
    /// <summary>
    /// Построение списка всех зависимостей корневой сборки
    /// </summary>
    void Collect();

    /// <summary>
    /// Префиксы имён сборок, которые должны быть исключены при поиске зависимостей
    /// </summary>
    HashSet<string> ExcludePrefixes { get; }
  }

  public class AssemblyTreeBuilder : IAssemblyTreeBuilder
  {
    private static readonly ILog _log = LogManager.GetLogger("AssemblyTreeBuilder");
    
    private readonly HashSet<string> m_exclude_prefixes = new HashSet<string>();
    private readonly HashSet<Assembly> m_assemblies = new HashSet<Assembly>();

    /// <summary>
    /// Инициализирует новый экземпляр класса AssemblyTreeBuilder
    /// </summary>
    /// <param name="root">Сборка, являющаяся корневым элементом при анализе зависимостей</param>
    public AssemblyTreeBuilder(Assembly root)
    {
      if (root == null)
        throw new ArgumentNullException("root");

      m_assemblies.Add(root);
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса AssemblyTreeBuilder
    /// </summary>
    public AssemblyTreeBuilder()
      : this(Assembly.GetEntryAssembly() ?? typeof(AppManager).Assembly) { }

    public HashSet<string> ExcludePrefixes
    {
      get { return m_exclude_prefixes; }
    }

    public void Collect()
    {
      lock (m_assemblies)
      {
        if (m_assemblies.Count > 1)
          return;

        Dictionary<string, Assembly> asms = new Dictionary<string, Assembly>();
        var ass = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in ass)
        {
          if (ExcludeRequired(asm.FullName))
            continue;

          asms.Add(asm.GetName().FullName, asm);
        }

        CollectChildren(m_assemblies.Single(), new HashSet<string>(), asms);

        foreach (var kv in asms)
          m_assemblies.Add(kv.Value);
      }
    }

    private void CollectChildren(Assembly root, HashSet<string> foundNames, 
      Dictionary<string, Assembly> loaded_assemblies)
    {
      Dictionary<string, AssemblyName> new_names = new Dictionary<string, AssemblyName>();

      foreach (var name in root.GetReferencedAssemblies())
      {
        if (ExcludeRequired(name.FullName))
          continue;
        
        if (foundNames.Add(name.FullName))
          new_names.Add(name.FullName, name);
      }

      foreach (var kv in new_names)
      {
        Assembly asm;

        try
        {
          if (!loaded_assemblies.TryGetValue(kv.Key, out asm))
          {
            asm = AppDomain.CurrentDomain.Load(kv.Value);
            loaded_assemblies.Add(kv.Key, asm);
          }

          CollectChildren(asm, foundNames, loaded_assemblies);
        }
        catch (Exception ex)
        {
          _log.Error(string.Format("Failed to load assembly {0}", kv.Key), ex);
        }
      }
    }

    private bool ExcludeRequired(string name)
    {
      if (string.IsNullOrEmpty(name))
        return true;

      if (name.Contains(".resources"))
        return true;
      
      bool exclude = false;

      foreach (var pfx in m_exclude_prefixes)
      {
        if (name.StartsWith(pfx))
        {
          exclude = true;
          break;
        }
      }
      return exclude;
    }
  }
}
