using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using AT.Toolbox.MSSQL.Properties;
using System.Resources;
using AT.Toolbox.MSSQL.DAL.RecordBinding;
using AT.Toolbox.Misc;

namespace AT.Toolbox.MSSQL.UI
{
  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
  public class DisplayNameResAttribute : DisplayNameAttribute
  {
    public DisplayNameResAttribute(string displayName, Type targetType)
      : base(displayName)
    {
      if (targetType == null)
        throw new ArgumentNullException("targetType");

      this.TargetType = targetType;
    }

    public Type TargetType { get; private set; }

    public override string DisplayName
    {
      get
      {
        try
        {
          string[] resourceNames = this.TargetType.Assembly.GetManifestResourceNames();
          foreach (string resourceRoot in resourceNames)
          {
            string base_name = resourceRoot.Replace(".resources", "");
            if (base_name.EndsWith("." + this.TargetType.Name) || base_name == this.TargetType.Name)
            {
              return new ResourceManager(base_name, this.TargetType.Assembly).GetString(base.DisplayNameValue);
            }
          }
          foreach (string resourceRoot in resourceNames)
          {
            string base_name = resourceRoot.Replace(".resources", "");
            string resource = new ResourceManager(base_name, this.TargetType.Assembly).GetString(base.DisplayNameValue);
            if (resource != null)
            {
              return resource;
            }
          }
        }
        catch { }
        return base.DisplayNameValue;
      }
    }

    public override bool IsDefaultAttribute()
    {
      return false;
    }

    public override bool Equals(object obj)
    {
      DisplayNameResAttribute res = obj as DisplayNameResAttribute;
      if (res == this)
        return true;

      if (res != null)
      {
        return (res.DisplayNameValue == this.DisplayNameValue) && (res.TargetType == this.TargetType);
      }
      return false;
    }

    public override int GetHashCode()
    {
      return GetHashCodeHelper.CombineHashCodes<object>(typeof(string).GetHashCode(), typeof(string).GetHashCode(),
                                                        this.DisplayNameValue, this.TargetType);
    }
  }
}
