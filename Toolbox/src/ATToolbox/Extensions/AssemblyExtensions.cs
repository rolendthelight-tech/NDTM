using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using log4net;

namespace AT.Toolbox.Extensions
{
  public static class AssemblyExtensions
  {
		private static readonly ILog _log = log4net.LogManager.GetLogger(typeof(AssemblyExtensions));
		
		public static bool IsAtToolbox(this Assembly assembly)
    {
			try
			{
				if (assembly.IsDefined(typeof (AssemblyCompanyAttribute), false))
				{
					var comp = assembly.GetCustomAttributes(typeof (AssemblyCompanyAttribute), false)[0]
					           as AssemblyCompanyAttribute;

					var str = (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
					                                     "RegisteredOrganization", "AudiTech");

					return comp.Company == "AudiTech" || comp.Company == str;
				}
			}
			catch(Exception ex)
			{
				_log.Error("IsAtToolbox(): exception: ", ex);
			}

    	return false;
    }

    private static string GetProductName(this Assembly asm, bool throwIfNotFound)
    {
      object[] objz = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

      if (objz.Length == 0)
      {
        if (throwIfNotFound)
          throw new ApplicationException("??");
        else
          return null;
      }

      AssemblyProductAttribute pa = objz[0] as AssemblyProductAttribute ;

      if( null == pa )
        throw new ApplicationException("??");

      return pa.Product;
    }

    public static string ProductName(this Assembly assembly)
    {
      return GetProductName(assembly, true);
    }

    public static string ProductVersion(this Assembly assembly)
    {
      object[] objz = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);

      if (objz.Length == 0)
        return "0.0.0.0";

      AssemblyVersionAttribute pa = objz[0] as AssemblyVersionAttribute;

      if (null == pa)
        return "0.0.0.0";

      return pa.Version;
    }

    public static string FileVersion(this Assembly assembly)
    {
      object[] objz = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);

      if (objz.Length == 0)
        return "0.0.0.0";

      AssemblyFileVersionAttribute pa = objz[0] as AssemblyFileVersionAttribute;

      if (null == pa)
        return "0.0.0.0";

      return pa.Version;
    }
  }
}
