using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public interface IBindingAspect
  {
    bool BuildingQuery(IBindableEntity owner, QueryObject query);
    bool ValidateInsert(IBindableEntity owner, InfoEventArgs messageBuffer);
    bool ValidateUpdate(IBindableEntity owner, InfoEventArgs messageBuffer);
    bool ValidateDelete(IBindableEntity owner, InfoEventArgs messageBuffer);
    void AfterDataBound(IBindableEntity owner, ref bool success);
    void AfterInsert(IBindableEntity owner, ref bool success);
    void AfterUpdate(IBindableEntity owner, ref bool success);
    void AfterDelete(IBindableEntity owner, ref bool success);
    void BeginEdit(IBindableEntity owner);
    void CancelEdit(IBindableEntity owner);
  }

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
  public sealed class AspectWeavingAttribute : Attribute
  {
    public AspectWeavingAttribute(Type conditionType)
    {
      if (conditionType == null)
      {
        throw new ArgumentNullException("conditionType");
      }
      this.ConditionType = conditionType;
    }

    public Type ConditionType { get; private set; }
  }

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
  public sealed class AspectExclusionAttribute : Attribute
  {
    public AspectExclusionAttribute(Type restrictedType)
    {
      this.RestrictedType = restrictedType ?? typeof(object);
    }

    public AspectExclusionAttribute()
      : this(null) { }

    public Type RestrictedType { get; private set; }
  }
}
