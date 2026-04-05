using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using AT.Toolbox.MSSQL.Properties;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Глобальный кэш бизнес-объектов
  /// </summary>
  internal static class RecordCache
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(RecordCache).Name);

    private static readonly Dictionary<Type, BindableClassState> m_states = new Dictionary<Type, BindableClassState>();
    private static readonly Dictionary<string, Type> m_table_names = new Dictionary<string, Type>();
    private static readonly List<Type> m_context_types = new List<Type>();

    static RecordCache()
    {
      MaxCacheSize = 1000;
    }

    public static int MaxCacheSize { get; set; }

    /// <summary>
    /// Регистрирует тип
    /// </summary>
    /// <param name="list"></param>
    internal static void RegisterType(IBindableObjectList list)
    {
      Type listType = list.GetType();
      if (listType.IsGenericType)
      {
        listType = listType.GetGenericArguments()[0];
        RegisterType(listType, list);
      }
    }

    private static bool RegisterType(Type type, IBindableObjectList list)
    {
      if (!typeof(IBindable).IsAssignableFrom(type))
      {
        throw new ArgumentException("Type does not implement IBindable");
      }
      lock (m_states)
      {
        if (m_states.ContainsKey(type) || m_table_names.ContainsKey(list.TableName.ToUpper()))
        {
          return false;
        }
        else
        {
          if (list == null)
          {
            list = RecordManager.GetTypedList(type, false, null);
          }
          m_states.Add(type, new BindableClassState() 
          { 
            List = list,
            LastInvalidation = DateTime.Now
          });
          m_table_names.Add(list.TableName.ToUpper(), type);
          return true;
        }
      }
    }

    /// <summary>
    /// Перестраивает перекрестные ссылки внешних ключей
    /// </summary>
    /// <param name="contextType">Тип контекста, инициировавшего перестройку</param>
    internal static void UpdateReferences(Type contextType)
    {
      lock (m_context_types)
      {
        if (!m_context_types.Contains(contextType))
        {
          UpdateReferences();

          if (contextType != null)
          {
            m_context_types.Add(contextType);
          }
        }
      }
    }

    private static void UpdateReferences()
    {
      lock (m_states)
      {
        foreach (KeyValuePair<Type, BindableClassState> kv in m_states)
        {
          kv.Value.References.Clear();
        }

        foreach (KeyValuePair<Type, BindableClassState> kv in m_states)
        {
          foreach (PropertyInfo pi in kv.Key.GetProperties(BindingFlags.Instance | BindingFlags.Public))
          {
            if (!pi.IsDefined(typeof(ReferenceAttribute), true))
              continue;

            ReferenceAttribute refer = pi.GetCustomAttributes(typeof(ReferenceAttribute), true)[0]
              as ReferenceAttribute;

            BindableClassState state;
            if (m_states.TryGetValue(refer.ReferenceType, out state))
            {
              ReferenceTrigger trigger = new ReferenceTrigger()
              {
                CurrentType = refer.ReferenceType,
                ReferenceType = kv.Key,
                DeleteAction = refer.DeleteAction,
                UpdateAction = refer.UpdateAction,
                PrimaryField = GetFieldName(kv.Key, pi.Name)
              };
              state.References.Add(trigger);
            }
          }
        }
      }
    }

    private static string GetFieldName(Type type, string propertyName)
    {
      Type fieldNamesType = type.GetNestedType("FieldNames");
      if (fieldNamesType != null)
      {
        try
        {
          return fieldNamesType.GetField(propertyName, 
            BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
        }
        catch (Exception ex)
        {
          Log.Error(string.Format("GetFieldName({0}, {1}): exception", type, propertyName), ex);
          return null;
        }
      }
      return propertyName;
    }

    internal static Type GetTypeByName(string tableName)
    {
      Type ret;
      m_table_names.TryGetValue(tableName.ToUpper(), out ret);
      return ret;
    }

    /// <summary>
    /// Возвращает дату последней операции с таблицей,
    /// инициированной текущим экземпляром приложения
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>Дата последней операции изменения данных</returns>
    internal static DateTime GetLastInvalidation(string tableName)
    {
      Type type;

      if (m_table_names.TryGetValue(tableName.ToUpper(), out type))
      {
        BindableClassState state;
        if (m_states.TryGetValue(type, out state))
        {
          return state.LastInvalidation;
        }
      }
      return DateTime.MinValue;
    }

    /// <summary>
    /// Помечает тип как неактуальный для привязываемого объекта
    /// </summary>
    /// <typeparam name="TType">Тип, для которого нужно очистить кэш</typeparam>
    internal static void InvalidateType<TType>(TType item)
      where TType : class, IBindable, new()
    {
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        lock (state)
        {
          state.Invokers.Clear();
          state.LastInvalidation = DateTime.Now;
          object identifier = item.GetIdentifier();
          if (identifier != null)
          {
            state.Cache.Remove(identifier);
          }
          else
          {
            state.Cache.Clear();
          }
          foreach (ReferenceTrigger trigger in state.References)
          {
            BindableClassState state2;

            if (m_states.TryGetValue(trigger.ReferenceType, out state2))
            {
              state2.Invokers.Clear();
              state2.Cache.Clear();
              state2.LastInvalidation = DateTime.Now;
              if (item != null)
              {
                state2.InvalidatedByCache = (item._CacheStatus == CacheStatus.LocalCache) && state2.InvalidatedByCache;
              }

              if (trigger.ReferenceType.IsDefined(typeof(ViewRedirectAttribute), true))
              {
                Type redirectType =
                  (trigger.ReferenceType.GetCustomAttributes(typeof(ViewRedirectAttribute), true)[0]
                  as ViewRedirectAttribute).RedirectType;

                BindableClassState stateRedirect;
                if (m_states.TryGetValue(redirectType, out stateRedirect))
                {
                  stateRedirect.Cache.Clear();
                  stateRedirect.LastInvalidation = DateTime.Now;
                  if (item != null)
                  {
                    stateRedirect.InvalidatedByCache
                      = (item._CacheStatus == CacheStatus.LocalCache) && stateRedirect.InvalidatedByCache;
                  }
                }
              }
            }
          }
          if (typeof(TType).IsDefined(typeof(ViewRedirectAttribute), true))
          {
            Type redirectType =
              (typeof(TType).GetCustomAttributes(typeof(ViewRedirectAttribute), true)[0]
              as ViewRedirectAttribute).RedirectType;

            if (redirectType != typeof(TType))
            {
              BindableClassState stateRedirect;
              if (m_states.TryGetValue(redirectType, out stateRedirect))
              {
                stateRedirect.Invokers.Clear();
                stateRedirect.LastInvalidation = DateTime.Now;
                if (identifier != null)
                {
                  stateRedirect.Cache.Remove(identifier);
                }
                else
                {
                  stateRedirect.Cache.Clear();
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Проверяет актуальность типа для привязываемого объекта
    /// </summary>
    /// <typeparam name="TTarget">Проверяемый тип</typeparam>
    /// <param name="invoker">Проверяемый объект</param>
    /// <returns>True, если для объекта тип актуален</returns>
    internal static bool ValidateType<TTarget>(IBindable invoker)
      where TTarget : class, IBindable, new()
    {
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TTarget), out state))
      {
        lock (state)
        {
          if (state.Invokers.Count > MaxCacheSize)
          {
            state.Invokers.Clear();
          }
          if (state.Invokers.Contains(invoker))
          {
            return true;
          }
          else
          {
            state.Invokers.Add(invoker);
            return false;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Помечает тип как неактуальный для контекста
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal static void InvalidateByCache<TType>()
      where TType : class, IBindable, new()
    {
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        lock (state)
        {
          state.InvalidatedByCache = true;
        }
      }
    }

    /// <summary>
    /// Проверяет допустимость удаления записи
    /// </summary>
    /// <param name="prim">Запись, которую планируется удалить</param>
    /// <param name="messageBuffer">Место, куда надо писать предупреждения</param>
    /// <returns>True, если удаление возможно; иначе false</returns>
    public static bool ValidateDelete(IBindable prim, InfoEventArgs messageBuffer)
    {
      if (prim == null)
        throw new ArgumentNullException("prim");

      BindableClassState state;
      if (m_states.TryGetValue(prim.GetType(), out state))
      {
        bool ret = prim._IsDataBound;

        foreach (var trigger in state.References)
        {
          if (trigger.DeleteAction == Rule.None)
          {
            ret = trigger.CheckReference(prim, messageBuffer) && ret;
          }
          else if (trigger.DeleteAction == Rule.Cascade)
          {
            foreach (IBindable sec in trigger.GetReferences(prim, messageBuffer))
            {
              if (!ValidateDelete(sec, messageBuffer))
              {
                ret = false;
                break;
              }
            }
          }
        }
        return ret;
      }
      return true;
    }

    /// <summary>
    /// Проверяет наличие ссылок в базе на переданный объект
    /// </summary>
    /// <param name="prim">Объект, который может содержать внешние ключи</param>
    /// <returns>True, если объекты не найдены</returns>
    public static bool CheckReferences(IBindable prim)
    {
      if (prim == null)
        throw new ArgumentNullException("prim");

      BindableClassState state;
      InfoEventArgs messageBuffer = new InfoEventArgs();
      if (m_states.TryGetValue(prim.GetType(), out state))
      {
        bool ret = prim._IsDataBound;

        foreach (var trigger in state.References)
        {
          ret = trigger.CheckReference(prim, messageBuffer) && ret;
        }
        return ret;
      }
      return true;
    }

    /// <summary>
    /// Выполняет автоматическую привязку, если тип списка неактуален для контекста
    /// </summary>
    /// <typeparam name="TType">Тип элемента списка</typeparam>
    /// <param name="source">Проверяемый список</param>
    /// <returns>Тот же список, но актуальный</returns>
    internal static BindableObjectList<TType> GetValidList<TType>()
      where TType : class, IBindable, new()
    {
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        bool invalid = false;
        lock (state)
        {
          invalid = state.InvalidatedByCache;
          state.InvalidatedByCache = false;
          state.Cache.Clear();
        }
        if (invalid)
        {
          state.List.DataBind();
        }
        return (BindableObjectList<TType>)state.List;
      }
      else
      {
        return new BindableObjectList<TType>();
      }
    }

    /// <summary>
    /// Загружает кэш заданного типа бизнес-объекта однофазно
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public static void BindSingleType<TType>()
      where TType : class, IBindable, new()
    {
      BindableClassState state;

      if (m_states.TryGetValue(typeof(TType), out state))
      {
        using (TType item = new TType())
        {
          if (string.IsNullOrEmpty(item._KeyFieldName))
            return;

          item.InitReader(new QueryObject());

          while (item.Next())
          {
            state.Cache[item.GetIdentifier()] = (IBindable)item.Clone();
          }
        }
      }
    }

    /// <summary>
    /// Ищет в кэше объект по ключу, если не найден, берёт из базы
    /// </summary>
    /// <param name="itemType">Тип бизнес-объекта</param>
    /// <param name="identifier">Значение первичного ключа</param>
    /// <returns>Сохраненный при предыдущем обращении объект или новый</returns>
    public static IBindable GetItem(Type itemType, object identifier)
    {
      if (itemType == null)
        throw new ArgumentNullException("itemType");

      if (identifier == null)
        return (IBindable)Activator.CreateInstance(itemType);

      BindableClassState state;
      if (m_states.TryGetValue(itemType, out state))
      {
        IBindable ret;

        if (!state.Cache.TryGetValue(identifier, out ret))
        {
          ret = (IBindable)Activator.CreateInstance(itemType);
          ret[ret._KeyFieldName] = identifier;
          ret.DataBind();
          if (ret._IsDataBound)
          {
            ret._CacheStatus = CacheStatus.GlobalCache;
            state.Cache.Add(identifier, ret);
          }
        }

        return ret;
      }
      IBindable item = (IBindable)Activator.CreateInstance(itemType);
      item[item._KeyFieldName] = identifier;
      item.DataBind();
      return item;
    }

    /// <summary>
    /// Ищет в кэше объект по ключу, если не найден, берёт из базы
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    /// <param name="identifier">Значение первичного ключа</param>
    /// <returns>Сохраненный при предыдущем обращении объект или новый</returns>
    public static TType GetItem<TType>(object identifier)
      where TType : class, IBindable, new()
    {
      if (identifier == null)
        return new TType();

      lock (m_states)
      {
        BindableClassState state;
        if (m_states.TryGetValue(typeof (TType), out state))
        {
          IBindable ret;

          if (!state.Cache.TryGetValue(identifier, out ret))
          {
            ret = new TType();
            ret[ret._KeyFieldName] = identifier;
            if (ret._TableName == "TableDescriptionView")
            {
            }
            ret.DataBind();
            if (ret._IsDataBound)
            {
              ret._CacheStatus = CacheStatus.GlobalCache;
              state.Cache.Add(identifier, ret);
            }
          }
          return (TType) ret;
        }
      }

      TType item = new TType();
      item[item._KeyFieldName] = identifier;
      item.DataBind();
      return item;
    }

    /// <summary>
    /// Принудительно кладёт объект в кэш
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    /// <param name="item">Объект</param>
    /// <param name="clone">Признак, класть в кэш сам объект, илиего копию</param>
    /// <returns>True, если объект был положен в кэш</returns>
    public static bool SetItem<TType>(TType item, bool clone)
      where TType : class, IBindable, new()
    {
      if (item == null || item.GetIdentifier() == null && !item._IsDataBound)
        return false;
      
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        IBindable ret = clone ? (IBindable)item.Clone() : item;
        ret._CacheStatus = CacheStatus.GlobalCache;
        state.Cache[item.GetIdentifier()] = ret;
      }
      return false;
    }

    /// <summary>
    /// Проверяет, имеется ли объект с указанным идентификатором в кэше
    /// </summary>
    /// <typeparam name="TType">Тип объекта</typeparam>
    /// <param name="identifier">Идентификатор</param>
    /// <returns>True, если объект есть в кэше</returns>
    public static bool Contains<TType>(object identifier)
      where TType : class, IBindable, new()
    {
      if (identifier == null)
        return false;

      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        bool ret = state.Cache.ContainsKey(identifier);
        return ret;
      }
      return false;
    }

    /// <summary>
    /// Возвращает список внешних ключей, ссылающихся на переданный тип
    /// </summary>
    /// <typeparam name="TType">Тип бизнес объекта</typeparam>
    /// <returns>Набор внешних ключей</returns>
    internal static ReferenceInfo[] GetReferences<TType>()
      where TType : class, IBindable, new()
    {
      BindableClassState state;
      if (m_states.TryGetValue(typeof(TType), out state))
      {
        return state.References.Select(p => new ReferenceInfo()
        {
          FieldName = p.PrimaryField,
          ReferenceType = p.ReferenceType
        }).ToArray();
      }
      return new ReferenceInfo[0];
    }

    #region Nested classes

    private class ReferenceTrigger
    {
      public Rule DeleteAction { get; set; }

      public Rule UpdateAction { get; set; }

      public Type CurrentType { get; set; }

      public Type ReferenceType { get; set; }

      public string PrimaryField { get; set; }

      public List<IBindable> GetReferences(IBindable prim, InfoEventArgs messageBuffer)
      {
         if (prim == null)
          throw new ArgumentNullException("prim");

        if (!this.CurrentType.IsAssignableFrom(prim.GetType()))
          throw new ArgumentException(
            string.Format(Resources.ReflectionBuilder_TypeMismatch, this.CurrentType, prim.GetType()));

        using (IBindable val = (IBindable)Activator.CreateInstance(this.ReferenceType))
        {
          List<IBindable> ret = new List<IBindable>();
          QueryObject query = new QueryObject();
          try
          {
            query.AddFilterExpression(this.PrimaryField, prim[prim._KeyFieldName]);
            val.InitReader(query);
            while (val.Next())
            {
              ret.Add((IBindable)val.Clone());
            }
          }
          catch (Exception ex)
          {
            Log.Error("GetReferences(): exception", ex);
            messageBuffer.Messages.Add(new Info(ex));
          }
          return ret;
        }
      }

      public bool CheckReference(IBindable prim, InfoEventArgs messageBuffer)
      {
        if (prim == null)
          throw new ArgumentNullException("prim");

        if (!this.CurrentType.IsAssignableFrom(prim.GetType()))
          throw new ArgumentException(
            string.Format(Resources.ReflectionBuilder_TypeMismatch, this.CurrentType, prim.GetType()));

        using (IBindable val = (IBindable)Activator.CreateInstance(this.ReferenceType))
        {
          QueryObject query = new QueryObject();
          try
          {
            query.AddFilterExpression(this.PrimaryField, prim[prim._KeyFieldName]);
            if (val.InitReader(query) && val.Next())
            {
              val.Break();
              string primName = this.CurrentType.Name;
              string valName = this.ReferenceType.Name;
              if (this.CurrentType.IsDefined(typeof(DisplayNameAttribute), true))
              {
                primName = (this.CurrentType.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
                  as DisplayNameAttribute).DisplayName;
              }
              if (this.ReferenceType.IsDefined(typeof(DisplayNameAttribute), true))
              {
                valName = (this.ReferenceType.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
                  as DisplayNameAttribute).DisplayName;
              }
              messageBuffer.Messages.Add(new Info(string.Format(Resources.BindableObject_RestrictedTrigger, primName, valName),
                InfoLevel.Warning));
              return false;
            }
          }
          catch (Exception ex)
          {
            Log.Error("CheckReference(): exception", ex);
            messageBuffer.Messages.Add(new Info(ex));
            return false;
          }
          return true;
        }
      }

      public override string ToString()
      {
        return "current: " + this.CurrentType + ", reference: " + this.ReferenceType
          + ", delete action: " + this.DeleteAction + ", update action: " + this.UpdateAction;
      }
    }

    /// <summary>
    /// Информация об актуальности кэша временных коллекций
    /// </summary>
    private class BindableClassState
    {
      List<IBindable> m_invokers = new List<IBindable>();
      Dictionary<object, IBindable> m_cache = new Dictionary<object, IBindable>();
      List<ReferenceTrigger> m_references = new List<ReferenceTrigger>();

      public IBindableObjectList List { get; set; }

      public bool InvalidatedByCache { get; set; }

      public DateTime LastInvalidation { get; set; }

      public Dictionary<object, IBindable> Cache
      {
        get { return m_cache; }
      }

      public List<IBindable> Invokers
      {
        get { return m_invokers; }
      }

      public List<ReferenceTrigger> References
      {
        get { return m_references; }
      }
    }

    #endregion
  }

  /// <summary>
  /// Статус бизнес-объекта по отношению к кэшу
  /// </summary>
  public enum CacheStatus
  {
    /// <summary>
    /// Объект не в кэше
    /// </summary>
    Free,
    /// <summary>
    /// Объект в глобальном кэше
    /// </summary>
    GlobalCache,
    /// <summary>
    /// Объект в локальном кэше другого бизнес-объекта.
    /// </summary>
    LocalCache
  }

  /// <summary>
  /// Информация о внешнем ключе
  /// </summary>
  public class ReferenceInfo
  {
    /// <summary>
    /// Тип бизнес-объекта, на котором определён внешний ключ
    /// </summary>
    public Type ReferenceType { get; set; }

    /// <summary>
    /// Имя поля, на котором определён внешний ключ
    /// </summary>
    public string FieldName { get; set; }
  }
}
