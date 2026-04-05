using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace Toolbox.Application
{
  //using Constants;
  //using Files;
  //using Properties;

  public class AssemblyAttributes
  {
    public string Company { get; set; }

    public string Copyright { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// Имя приложения без учёта имени пакета ПО
    /// </summary>
    public string ProductTitle
    {
      get
      {
        var r = new System.Text.RegularExpressions.Regex(@"[^/]*$");
        var m = r.Match(ProductName.Replace('\\', '/'));
        return m.Success ? m.Value : ProductName;
      }
    }

    public string Version { get; set; }

    //public Support Support { get; set; }

    //public Image SupportPic
    //{
    //  get
    //  {
    //    switch( Support )
    //    {
    //      case Support.Full:
    //        return Resources.ICO_FULL_SUPPORT;
    //      case Support.Partial:
    //        return Resources.ICO_PARTIAL_SUPPORT;
    //      case Support.None:
    //        return Resources.ICO_NO_SUPPORT;
    //      case Support.Unknown:
    //        return Resources.ICO_UNKNOWN_SUPPORT;
    //      default:
    //        return Resources.ICO_UNKNOWN_SUPPORT;
    //    }
    //  }
    //}
  }


	public static class ApplicationInfo
	{
		[NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ApplicationInfo));

		#region Private Variables ---------------------------------------------------------------------------------------------------

		[CanBeNull] private static AssemblyAttributes _mainAttributes;

		//static List<AssemblyAttributes> m_other_attributes;

		#endregion

		#region Public Properties ---------------------------------------------------------------------------------------------------

		[CanBeNull]
		public static AssemblyAttributes MainAttributes
		{
			get
			{
				if (null == _mainAttributes)
				{
					try
					{
						_mainAttributes = GetAssemblyAttributes(Assembly.GetEntryAssembly());
					}
					catch (Exception ex)
					{
						_log.Error("MainAttributes.get(): exception", ex);
					}
				}

				return _mainAttributes;
			}
		}

		[NotNull]
		public static List<AssemblyAttributes> OtherModuleAttributes
		{
			get
			{
				var assembly_attributeses = new List<AssemblyAttributes>();

				foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						assembly_attributeses.Add(GetAssemblyAttributes(asm));
					}
					catch (Exception ex)
					{
						_log.Warn("OtherModuleAttributes.get(): failed to gather info about assembly " + asm.FullName, ex);
					}
				}

				return assembly_attributeses;
			}
		}

		#endregion

		#region Private Methods -----------------------------------------------------------------------------------------------------

		[NotNull]
		private static AssemblyAttributes GetAssemblyAttributes([CanBeNull] Assembly target)
		{
			AssemblyAttributes ret_val = new AssemblyAttributes();

			if (target == null)
			{
				ret_val.ProductName = "TestApplication";
				ret_val.Company = String.Empty;
				return ret_val;
			}
			else
			{
				object[] objects = target.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);

				if (objects.Length > 0)
				{
					AssemblyCompanyAttribute att = objects[0] as AssemblyCompanyAttribute;

					if (null != att)
						ret_val.Company = att.Company ?? "";
				}

				objects = target.GetCustomAttributes(typeof(AssemblyProductAttribute), true);

				if (objects.Length > 0)
				{
					AssemblyProductAttribute att = objects[0] as AssemblyProductAttribute;

					if (null != att)
						ret_val.ProductName = att.Product;
				}
				else
				{
					string location;
					try
					{
						location = target.Location;
					}
					catch (NotSupportedException ex)
					{
						_log.Warn("GetAssemblyAttributes(): NotSupportedException", ex);
						location = null;
					}
					ret_val.ProductName = location == null ? null : Path.GetFileName(location);
				}

				ret_val.Version = target.GetName().Version.ToString();

				objects = target.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);

				if (objects.Length > 0)
				{
					AssemblyCopyrightAttribute att = objects[0] as AssemblyCopyrightAttribute;

					if (null != att)
						ret_val.Copyright = att.Copyright;
				}

				objects = target.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);

				if (objects.Length > 0)
				{
					AssemblyDescriptionAttribute att = objects[0] as AssemblyDescriptionAttribute;

					if (null != att)
						ret_val.Description = att.Description;
				}

				return ret_val;
			}
		}

		#endregion
	}
}