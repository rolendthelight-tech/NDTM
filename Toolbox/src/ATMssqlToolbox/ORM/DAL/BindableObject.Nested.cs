using System;
using System.Collections.Generic;
using AT.Toolbox.MSSQL.Properties;
using System.Reflection;
using System.Security;
using System.ComponentModel;
using System.Data;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  partial class BindableObject<TType>
  {
    private static class BindableAspectManager
    {
      public static void InitAspects(BindingRequisites binding_requisites)
      {
        foreach (IBindingAspect aspect in RecordManager.Aspects)
        {
          Type currentType = aspect.GetType();
          bool forbidden = false;
          foreach (AspectWeavingAttribute weaving in currentType.GetCustomAttributes(typeof(AspectWeavingAttribute), true))
          {
            forbidden = true;
            if (weaving.ConditionType.IsAssignableFrom(typeof(TType)))
            {
              forbidden = false;
              break;
            }
          }
          foreach (AspectExclusionAttribute restriction in typeof(TType).GetCustomAttributes(typeof(AspectExclusionAttribute), true))
          {
            if (restriction.RestrictedType.IsAssignableFrom(currentType))
            {
              forbidden = true;
              break;
            }
          }
          foreach (Type weavedType in binding_requisites.Aspects.Keys)
          {
            if (currentType.IsAssignableFrom(weavedType))
            {
              forbidden = true;
            }
            if (weavedType.IsAssignableFrom(currentType))
            {
              throw new InvalidProgramException(string.Format(Resources.BindableObject_ExtendedAspect, weavedType));
            }
          }
          if (!forbidden)
          {
            binding_requisites.Aspects[currentType] = aspect;
          }
        }
      }

      public static void AfterDataBound(BindableObject<TType> owner, BindingResult result)
      {
        bool success = result.Success;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.AfterDataBound(entry, ref success);
            }
          }
          finally
          {
            owner.aspectWorking = false;
            result.Success = success;
          }
        }
      }

      public static void BuildingQuery(BindableObject<TType> owner, QueryObject query)
      {
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              if (!aspect.BuildingQuery(entry, query))
              {
                IQueryBuilder builder = RecordManager.GetQueryBuilder();
                builder.MainTable = owner._TableName;
                query.BuildQuery(builder);
                throw new SecurityException(string.Format(Resources.BindableObject_NoAccess, aspect, builder.GetResult()));
              }
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
      }

      public static bool ValidateInsert(BindableObject<TType> owner, InfoEventArgs messageBuffer)
      {
        bool ret = true;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              ret = aspect.ValidateInsert(entry, messageBuffer) && ret;
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
        return ret;
      }

      public static bool ValidateUpdate(BindableObject<TType> owner, InfoEventArgs messageBuffer)
      {
        bool ret = true;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              ret = aspect.ValidateUpdate(entry, messageBuffer) && ret;
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
        return ret;
      }

      public static bool ValidateDelete(BindableObject<TType> owner, InfoEventArgs messageBuffer)
      {
        bool ret = true;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              ret = aspect.ValidateDelete(entry, messageBuffer) && ret;
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
        return ret;
      }

      public static void AfterInsert(BindableObject<TType> owner, BindingResult result)
      {
        bool success = result.Success;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.AfterInsert(entry, ref success);
            }
          }
          finally
          {
            owner.aspectWorking = false;
            result.Success = success;
          }
        }
      }

      public static void AfterUpdate(BindableObject<TType> owner, BindingResult result)
      {
        bool success = result.Success;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.AfterUpdate(entry, ref success);
            }
          }
          finally
          {
            owner.aspectWorking = false;
            result.Success = success;
          }
        }
      }

      public static void AfterDelete(BindableObject<TType> owner, BindingResult result)
      {
        bool success = result.Success;
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.AfterDelete(entry, ref success);
            }
          }
          finally
          {
            owner.aspectWorking = false;
            result.Success = success;
          }
        }
      }

      public static void BeginEdit(BindableObject<TType> owner)
      {
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.BeginEdit(entry);
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
      }

      public static void CancelEdit(BindableObject<TType> owner)
      {
        IBindableEntity entry = owner as IBindableEntity;
        if (entry != null)
        {
          owner.aspectWorking = true;
          try
          {
            foreach (IBindingAspect aspect in binding_requisites.Aspects.Values)
            {
              aspect.CancelEdit(entry);
            }
          }
          finally
          {
            owner.aspectWorking = false;
          }
        }
      }
    }

    private struct ReferenceStore
    {
      public Type referenceType;
      public IBindable reference;

      public ReferenceStore(Type referenceType, IBindable reference)
      {
        this.referenceType = referenceType;
        this.reference = reference;
      }
    }

    private static readonly object st_locker = new object();

    private class BindingRequisites
    {
      public string IdentityField;
      public string TableName;
      public string[] DataBoundFieldNames;
      public string[] KeyFieldNames;
      public Dictionary<string, string> FieldMapping;
      public Dictionary<Type, IBindingAspect> Aspects;
      public Dictionary<BindingAction, string> ProcedureActions;
    }

    [Flags]
    private enum BindableState : byte
    {
      Clear = 0,
      IsDataBound = 1,
      Modified = 2,
      PendingBreak = 4,
      Uncommitted = 8,
      AspectWorking = 16,
      DisableBLL = 32
    }
  }

  internal interface IEditableReference
  {
    IBindable Value { get; set; }
  }

  [Serializable]
  public class EditableReference<TType> : IEditableReference
    where TType: class, IBindable, new()
  {
    #region IBindableReference Members

    public TType Value { get; set; }

    IBindable IEditableReference.Value
    {
      get { return this.Value; }
      set { this.Value = (TType)value; }
    }

    #endregion
  }

  /// <summary>
  /// Указывает имя таблицы для
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class TableNameAttribute : Attribute
  {
    public TableNameAttribute(string tableName)
    {
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentNullException("tableName");

      this.TableName = tableName;
    }
    
    public string TableName { get; private set; }
  }

  /// <summary>
  /// Указывает класс, используемый для отображения списка объектов текущего типа
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public sealed class ViewRedirectAttribute : Attribute
  {
    public ViewRedirectAttribute(Type redirectType)
    {
      if (redirectType == null)
        throw new ArgumentNullException("redirectType");
      
      if (!typeof(IBindable).IsAssignableFrom(redirectType))
        throw new ArgumentException();
      
      this.RedirectType = redirectType;
    }
    
    public Type RedirectType { get; private set; }
  }

  /// <summary>
  /// Показывает, что объект является вложенной в некий другой объект сущностью
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public sealed class NestedEntity : Attribute
  {
    public NestedEntity()
    {      
    }
  }

  /// <summary>
  /// Указывает базовое свойство, связанное с базой данных, для свойства, вычисляемого на клиенте
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class PropertyRedirectAttribute : Attribute
  {
    public PropertyRedirectAttribute(string fieldName)
    {
      this.FieldName = fieldName;
    }

    public string FieldName { get; private set; }
  }

  /// <summary>
  /// Запрещает редактирование таблицы через типовую форму
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class FixedReferenceAttribute : Attribute { }

  /// <summary>
  /// Указывает контроллер, используемый для работы
  /// с бизнес-объектами через графический интерфейс
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public sealed class UIControllerAttribute : Attribute
  {
    public UIControllerAttribute(string controllerName)
    {
      this.ControllerName = controllerName;
    }

    public string ControllerName { get; private set; }
  }

  /// <summary>
  /// Указывает ключ для вхождения в FieldDescription, позволяющий управлять отображением свойств
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class FieldDescriptionAttribute : Attribute
  {
    public FieldDescriptionAttribute(string table, string column)
    {
      this.Table = table;
      this.Column = column;
    }
    
    public string Table { get; private set; }

    public string Column { get; private set; }
  }

  /// <summary>
  /// Указывает тип бизнес-объекта первичного ключа для свойства, являющегося внешним ключом
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class ReferenceAttribute : Attribute
  {
    public ReferenceAttribute(Type referenceType)
      : this(referenceType, Rule.None) { }

    public ReferenceAttribute(Type referenceType, Rule changeAction)
      : this(referenceType, changeAction, changeAction) { }

    public ReferenceAttribute(Type referenceType, Rule deleteAction, Rule updateAction)
    {
      this.ReferenceType = referenceType;
      this.DeleteAction = deleteAction;
      this.UpdateAction = updateAction;
    }

    public Type ReferenceType { get; private set; }

    public Rule UpdateAction { get; private set; }

    public Rule DeleteAction { get; private set; }
  }
}