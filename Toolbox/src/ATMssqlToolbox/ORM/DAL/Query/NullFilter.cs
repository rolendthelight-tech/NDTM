using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class NullFilter : FilterBase
  {
    public string FieldName { get; set; }
    
    public override void DoBuildFilter(IQueryBuilder builder)
    {
      if (!string.IsNullOrEmpty(this.FieldName))
      {
        builder.BuildIsNull(this);
      }
    }
  }
}
