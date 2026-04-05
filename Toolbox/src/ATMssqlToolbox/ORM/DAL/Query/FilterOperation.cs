using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Flags]
  public enum FilterOperation : byte
  {
    Equals = 1,
    More = 2,
    Less = 4,
    Like = 8,
    NotEquals = 16
  }
}