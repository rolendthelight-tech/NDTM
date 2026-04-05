using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class BindableItemList<TType> : BindingList<TType>
    where TType : FdTypeDescriptor
  {
  }
}
