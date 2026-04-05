using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class InFilter : FilterBase
  {
    private readonly ArrayList m_values = new ArrayList();
    
    public string FieldName { get; set; }

    public ArrayList Values
    {
      get { return m_values; }
    }

    public override void DoBuildFilter(IQueryBuilder builder)
    {
      builder.BuildIn(this);
    }

    public override bool Enabled
    {
      get
      {
        return !string.IsNullOrEmpty(this.FieldName) && m_values.Count > 0;
      }
    }
  }
}
