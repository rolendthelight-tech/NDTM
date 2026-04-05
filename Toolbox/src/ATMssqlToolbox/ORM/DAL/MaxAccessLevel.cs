using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public enum MaxAccessLevel
  {
    NoAccess = 0,
    Read = 1,
    Update = 2,
    Insert = 3,
    Delete = 4
  }

  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public sealed class MaxAccessLevelAttribute : Attribute
  {
    public MaxAccessLevelAttribute(MaxAccessLevel maxAccessLevel)
    {
      this.MaxAccessLevel = maxAccessLevel;
    }

    public MaxAccessLevel MaxAccessLevel { get; private set; }
  }
}
