using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Toolbox.Extensions;

namespace Toolbox.Application.Services
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
    [NotNull]
    HashSet<string> ExcludePrefixes { get; }
  }

  public class AssemblyTreeBuilder : IAssemblyTreeBuilder
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyTreeBuilder));

	  [NotNull] private readonly HashSet<string> m_exclude_prefixes = new HashSet<string>();
	  [NotNull] private readonly HashSet<Assembly> m_assemblies = new HashSet<Assembly>();

    /// <summary>
    /// Инициализирует новый экземпляр класса AssemblyTreeBuilder
    /// </summary>
    /// <param name="root">Сборка, являющаяся корневым элементом при анализе зависимостей</param>
    public AssemblyTreeBuilder([NotNull] Assembly root)
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

	  [NotNull]
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

        var asms = new Dictionary<string, Assembly>();
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

    private void CollectChildren([NotNull] Assembly root, [NotNull] HashSet<string> foundNames, [NotNull] Dictionary<string, Assembly> loadedAssemblies)
    {
	    if (root == null) throw new ArgumentNullException("root");
	    if (foundNames == null) throw new ArgumentNullException("foundNames");
	    if (loadedAssemblies == null) throw new ArgumentNullException("loadedAssemblies");

	    var new_names = new Dictionary<string, AssemblyName>();

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
          if (!loadedAssemblies.TryGetValue(kv.Key, out asm))
          {
            asm = AppDomain.CurrentDomain.Load(kv.Value);
            loadedAssemblies.Add(kv.Key, asm);
          }

          CollectChildren(asm, foundNames, loadedAssemblies);
        }
        catch (Exception ex)
        {
					_log.ErrorFormat(ex, "Failed to load assembly {0}", kv.Key);
        }
      }
    }

    private bool ExcludeRequired([NotNull] string name)
    {
	    if (name == null) throw new ArgumentNullException("name");

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
