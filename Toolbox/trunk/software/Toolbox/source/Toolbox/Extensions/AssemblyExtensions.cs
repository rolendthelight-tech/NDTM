using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Extensions
{
  public static class AssemblyExtensions
  {
	  [NotNull] private static readonly ILog _log = log4net.LogManager.GetLogger(typeof(AssemblyExtensions));

		public static bool IsToolboxSearchable([NotNull] this Assembly assembly)
    {
			if (assembly == null) throw new ArgumentNullException("assembly");

			try
			{
				if (assembly.IsDefined(typeof(ToolboxSearchableAttribute), false))
				{
					//var comp = assembly.GetCustomAttributes(typeof (AssemblyCompanyAttribute), false)[0]
					//           as AssemblyCompanyAttribute;

					//var str = (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
					//                                     "RegisteredOrganization", "AudiTech");

					//return comp.Company == "AudiTech" || comp.Company == str;
					return true;
				}
			}
			catch(Exception ex)
			{
				_log.Error("IsToolboxSearchable(): exception: ", ex);
			}

    	return false;
    }

	  [NotNull]
	  public static Type[] GetAvailableTypes([NotNull] this Assembly assembly)
    {
	    if (assembly == null) throw new ArgumentNullException("assembly");

			try
      {
        return assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException rex)
      {
        foreach (var ex in rex.LoaderExceptions)
          _log.Error("GetAvailableTypes(): exception", ex);

        return rex.Types.Where(t => t != null).ToArray();
      }
    }

	  [CanBeNull]
	  private static string GetProductName([NotNull] this Assembly asm, bool throwIfNotFound)
    {
		  if (asm == null) throw new ArgumentNullException("asm");

			object[] objz = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

      if (objz.Length == 0)
      {
        if (throwIfNotFound)
          throw new ApplicationException("??");
        else
          return null;
      }

      var pa = objz[0] as AssemblyProductAttribute ;

      if( null == pa )
        throw new ApplicationException("??");

      return pa.Product;
    }

	  [CanBeNull]
	  public static string ProductName([NotNull] this Assembly assembly)
    {
	    if (assembly == null) throw new ArgumentNullException("assembly");

			return GetProductName(assembly, true);
    }

	  [CanBeNull]
	  public static string ProductVersion([NotNull] this Assembly assembly)
    {
		  if (assembly == null) throw new ArgumentNullException("assembly");

			object[] objz = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);

      if (objz.Length == 0)
        return "0.0.0.0";

      var pa = objz[0] as AssemblyVersionAttribute;

      if (null == pa)
        return "0.0.0.0";

      return pa.Version;
    }

	  [CanBeNull]
	  public static string FileVersion([NotNull] this Assembly assembly)
    {
	    if (assembly == null) throw new ArgumentNullException("assembly");

			object[] objz = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);

      if (objz.Length == 0)
        return "0.0.0.0";

      var pa = objz[0] as AssemblyFileVersionAttribute;

      if (null == pa)
        return "0.0.0.0";

      return pa.Version;
    }
  }

	/// <summary>
	/// Определяется для сборки, в которой можно искать типы для расширения функциональности
	/// </summary>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public class ToolboxSearchableAttribute : Attribute { }
}
