using System;
using System.Collections.Generic;
using System.Text;
using AT.Toolbox.MSSQL.UI;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [DisplayNameRes("BindingAction", typeof(BindingAction))]
  public enum BindingAction : byte
  {
    [DisplayNameRes("Get", typeof(BindingAction))]
    Get,
    [DisplayNameRes("Insert", typeof(BindingAction))]
    Insert,
    [DisplayNameRes("Update", typeof(BindingAction))]
    Update,
    [DisplayNameRes("Delete", typeof(BindingAction))]
    Delete,
    [DisplayNameRes("List", typeof(BindingAction))]
    List,
    [DisplayNameRes("Execute", typeof(BindingAction))]
    Execute
  }
}