using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace AT.Toolbox
{
  using Constants;
  using Files;
  using Properties;

  public class AssemblyAttributes
  {
    public string Company { get; set; }

    public string Copyright { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// ˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜
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

    public Support Support { get; set; }

    public Image SupportPic
    {
      get
      {
        switch( Support )
        {
          case Support.Full:
            return Resources.ICO_FULL_SUPPORT;
          case Support.Partial:
            return Resources.ICO_PARTIAL_SUPPORT;
          case Support.None:
            return Resources.ICO_NO_SUPPORT;
          case Support.Unknown:
            return Resources.ICO_UNKNOWN_SUPPORT;
          default:
            return Resources.ICO_UNKNOWN_SUPPORT;
        }
      }
    }
  }


  public static class ApplicationInfo
  {
    static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ApplicationInfo).Name);

    #region Private Variables ---------------------------------------------------------------------------------------------------

    static AssemblyAttributes m_main_attributes;

    //static List<AssemblyAttributes> m_other_attributes;

    #endregion


    #region Public Properties ---------------------------------------------------------------------------------------------------

    public static AssemblyAttributes MainAttributes
    {
      get
      {
        if( null == m_main_attributes )
        {
          try
          {
            m_main_attributes = GetAssemblyAttributes(Assembly.GetEntryAssembly());
          }
          catch (Exception ex)
          {
            Log.Error("Init(): exception", ex);
          }
          if (m_main_attributes == null)
            m_main_attributes = GetAssemblyAttributes(null);
        }

        return m_main_attributes;
      }
    }

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
            Log.Warn("Init(): failed to gather info about assembly " + asm.FullName, ex);
          }
        }

        return assembly_attributeses;
      }
    }

    #endregion


    #region Private Methods -----------------------------------------------------------------------------------------------------

    static AssemblyAttributes GetAssemblyAttributes( Assembly Target )
    {
      AssemblyAttributes ret_val = new AssemblyAttributes( );

      if (Target == null)
      {
        ret_val.Company = string.Empty;
        ret_val.ProductName = "TestApplication";
        return ret_val;
      }

      object[] objects = Target.GetCustomAttributes( typeof( AssemblyCompanyAttribute ), true );

      if( objects.Length > 0 )
      {
        AssemblyCompanyAttribute att = objects[0] as AssemblyCompanyAttribute;

        if( null != att )
          ret_val.Company = att.Company;
      }

      objects = Target.GetCustomAttributes( typeof( AssemblyProductAttribute ), true );

      if( objects.Length > 0 )
      {
        AssemblyProductAttribute att = objects[0] as AssemblyProductAttribute;

        if (null != att)
          ret_val.ProductName = att.Product;
      }
      else
      {
        ret_val.ProductName = Path.GetFileName(Target.Location);
      }

      ret_val.Version = Target.GetName( ).Version.ToString( );

      objects = Target.GetCustomAttributes( typeof( AssemblyCopyrightAttribute ), true );

      if( objects.Length > 0 )
      {
        AssemblyCopyrightAttribute att = objects[0] as AssemblyCopyrightAttribute;

        if( null != att )
          ret_val.Copyright = att.Copyright;
      }

      objects = Target.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);

      if (objects.Length > 0)
      {
        AssemblyDescriptionAttribute att = objects[0] as AssemblyDescriptionAttribute;

        if (null != att)
          ret_val.Description = att.Description;
      }

      return ret_val;
    }

    #endregion
  }
}