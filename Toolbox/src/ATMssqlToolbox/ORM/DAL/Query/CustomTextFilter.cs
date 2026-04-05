using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class CustomTextFilter : FilterBase
  {
    public override bool Enabled
    {
      get
      {
        return !string.IsNullOrEmpty(this.CommandText);
      }
    }

    public string CommandText { get; set; }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      builder.BuildCustomText(this.CommandText);
    }
  }
}
