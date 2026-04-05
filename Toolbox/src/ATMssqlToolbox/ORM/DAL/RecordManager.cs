using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using AT.Toolbox.MSSQL.ORM.DAL.Wcf;
using AT.Toolbox.MSSQL.Properties;
using AT.Toolbox.Log;
//using SecurityManager;
using AT.Toolbox.Extensions;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public static class RecordManager
  {
    #region Fields

    private static readonly object factoryLocker = new object();
    private static IRecordBinderFactory binderFactory;
    private static IWriteLogStrategy currentLogStrategy;
    private static readonly object appLogLocker = new object();
    private static readonly List<DbContext> contexts = new List<DbContext>();
    private static readonly List<IBindingAspect> aspects = new List<IBindingAspect>();

    static RecordManager()
    {
      AutoSaveOnEndEdit = true;
    }

    #endregion

    #region RecordBinder factory members

    internal static string GetTransactionStackId(IRecordBinder binder)
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.GetTransactionStackId(binder);
      }
    }

    /// <summary>
    /// Устанавливает фабрику для привязки к источнику данных определённого типа
    /// </summary>
    /// <param name="factory">Фабрика, которую требуется установить</param>
    public static void SetBinderFactory(IRecordBinderFactory factory)
    {
      lock (factoryLocker)
      {
        if (factory == null)
        {
          throw new ArgumentNullException("factory");
        }
        binderFactory = factory;
      }
    }

    /// <summary>
    /// Проверяет актуальность данных в кэше указанной таблицы
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>True, если данные актуальны</returns>
    public static bool CheckActuality<TType>()
      where TType : class, IBindable, new()
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.CheckActuality<TType>();
      }
    }

    /// <summary>
    /// Возвращает имя пользователя, полученное установленной фабрикой
    /// </summary>
    /// <returns></returns>
    public static string CurrentUserName
    {
      get
      {
        lock (factoryLocker)
        {
          if (binderFactory == null)
          {
            throw new InvalidOperationException(Resources.RecordManager_NoFactory);
          }
          return binderFactory.GetCurrentUserName();
        }
      }
    }

    /// <summary>
    /// Возвращает имя компьютера, полученное установленной фабрикой
    /// </summary>
    public static string ComputerName
    {
      get
      {
        lock (factoryLocker)
        {
          if (binderFactory == null)
          {
            throw new InvalidOperationException(Resources.RecordManager_NoFactory);
          }
          return binderFactory.GetComputerName();
        }
      }
    }

    /// <summary>
    /// Возвращает имя приложения, полученное установленной фабрикой
    /// </summary>
    public static string ApplicationName
    {
      get
      {
        lock (factoryLocker)
        {
          if (binderFactory == null)
          {
            throw new InvalidOperationException(Resources.RecordManager_NoFactory);
          }
          return binderFactory.GetApplicationName();
        }
      }
    }

    /// <summary>
    /// Показывает, установлена ли фабрика
    /// </summary>
    internal static bool IsRecordBinderFactorySet
    {
      get { return binderFactory != null; }
    }

    /// <summary>
    /// Показывает, требует ли установленная фабрика инициализации или готова к работе
    /// </summary>
    public static bool RequireInitialize
    {
      get
      {
        lock (factoryLocker)
        {
          return binderFactory == null || binderFactory.RequireInitialize;
        }
      }
    }

    /// <summary>
    /// Возвращает привязчик, порождаемый установленной фабрикой
    /// </summary>
    /// <param name="procedureActions">Действия, выполняемые через именованные (хранимые) процедуры</param>
    /// <returns>Привязчик к источнику данных установленного типа</returns>
    public static IRecordBinder GetRecordBinder(Dictionary<BindingAction, string> procedureActions)
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.CreateRecordBinder(procedureActions);
      }
    }

    /// <summary>
    /// Возвращает привязчик, порождаемый либо переданным провайдером,
    /// либо, если передано значение null, установленной фабрикой
    /// </summary>
    /// <param name="procedureActions">Действия, выполняемые через именованные (хранимые) процедуры</param>
    /// <param name="provider">Провайдер, порождающий привязчик</param>
    /// <returns>Привязчик к нужному источнику данных</returns>
    public static IRecordBinder GetRecordBinder(Dictionary<BindingAction, string> procedureActions, ICustomRecordBinderProvider provider)
    {
      IRecordBinder ret = null;
      
      if (provider != null)
      {
        ret = provider.GetRecordBinder(procedureActions);
      }

      if (ret == null)
      {
        ret = GetRecordBinder(procedureActions);
      }

      return ret;
    }

    /// <summary>
    /// Возвращает преобразователь объектной модели запроса в запрос
    /// к установленному типу источника данных
    /// </summary>
    /// <returns>Преобразователь запроса</returns>
    public static IQueryBuilder GetQueryBuilder()
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.CreateQueryBuilder();
      }
    }

    /// <summary>
    /// Возвращает коллекцию объектов, удовлетворяющих запросу
    /// </summary>
    public static IEnumerable<IBindable> GetObjectList(string tableName, QueryObject query)
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.GetObjectList(tableName, query);
      }
    }

    /// <summary>
    /// Заполняет переданную таблицу данными, взятыми из указанной таблицы в базе данных
    /// </summary>
    /// <param name="tableName">Имя таблицы в базе данных</param>
    /// <param name="table">Таблица, которую требуется заполнить</param>
    /// <param name="qo">Запрос, по которому данные должны быть отфильтрованы</param>
    public static void FillDataTable(string tableName, DataTable table, QueryObject qo)
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        binderFactory.FillDataTable(tableName, table, qo);
      }
    }

    /// <summary>
    /// Копирует переданную запись на сервер приложения 
    /// и выполняет на сервере указанную операцию
    /// </summary>
    /// <param name="record">Запись, над которой требуется выполнить операцию</param>
    /// <param name="action">Операция, которую требуется выполнить</param>
    /// <returns>Результат выполнения удаённой операции</returns>
    public static BindingResult BindOnServer(IServiceRecord record, BindingAction action)
    {
      lock (factoryLocker)
      {
        if (binderFactory == null)
        {
          throw new InvalidOperationException(Resources.RecordManager_NoFactory);
        }
        return binderFactory.GetResult(record, action);
      }
    }

    #endregion

    #region Cache accessors

    /// <summary>
    /// Возвращает дату последней операции с таблицей,
    /// инициированной текущим экземпляром приложения
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>Дата последней операции изменения данных</returns>
    public static DateTime GetLastInvalidation(string tableName)
    {
      return RecordCache.GetLastInvalidation(tableName);
    }

    /// <summary>
    /// Заменяет все ссылки на один объект в базе ссылками на другой
    /// </summary>
    /// <param name="oldItem">Объект со старым ключом</param>
    /// <param name="newItem">Объект с новым ключом</param>
    /// <returns>True, если замена прошла успешно</returns>
    public static bool Replace(IBindable oldItem, IBindable newItem)
    {
      if (oldItem == null)
        throw new ArgumentNullException("oldItem");

      if (newItem == null)
        throw new ArgumentNullException("newItem");
      
      if (oldItem.GetType() != newItem.GetType())
      {
        throw new ArgumentException();
      }

      MethodInfo caller = null;

      foreach (MethodInfo mi in typeof(RecordManager).GetMethods(BindingFlags.Static | BindingFlags.Public))
      {
        if (mi.Name == "Replace" && mi.IsGenericMethod)
        {
          caller = mi.MakeGenericMethod(oldItem.GetType());
          break;
        }
      }

      return (bool)caller.Invoke(null, new object[] { oldItem, newItem });
    }

    /// <summary>
    /// Заменяет все ссылки на один объект в базе ссылками на другой
    /// </summary>
    /// <typeparam name="TType">Тип объектов</typeparam>
    /// <param name="oldItem">Объект со старым ключом</param>
    /// <param name="newItem">Объект с новым ключом</param>
    /// <returns>True, если замена прошла успешно</returns>
    public static bool Replace<TType>(TType oldItem, TType newItem)
      where TType : class, IBindable, new()
    {
      if (oldItem == null)
        throw new ArgumentNullException("oldItem");

      if (newItem == null)
        throw new ArgumentNullException("newItem");

      if (typeof(ICustomRecordBinderProvider).IsAssignableFrom(typeof(TType)))
        throw new ArgumentException(Resources.CUSTOM_BINDER_TO_REPLACE);

      if (oldItem.GetIdentifier() == newItem.GetIdentifier())
      {
        return false;
      }

      return RunInTransaction(IsolationLevel.RepeatableRead,
        delegate
        {
          ReferenceInfo[] refs = RecordCache.GetReferences<TType>();
          foreach (ReferenceInfo kv in refs)
          {
            using (IBindableObjectList list = GetTypedList(kv.ReferenceType, false, new QueryObject()))
            {
              if (list.KeyFieldName == kv.FieldName)
              {
                IBindable old_ref = RecordCache.GetItem(kv.ReferenceType, oldItem.GetIdentifier());
                IBindable new_ref = RecordCache.GetItem(kv.ReferenceType, newItem.GetIdentifier());
                if (old_ref._IsDataBound && new_ref._IsDataBound)
                {
                  Replace(old_ref, new_ref);
                  continue;
                }
              }

              QueryObject qo = new QueryObject()
              {
                RootFilter = new ExpressionFilter(kv.FieldName,
                  FilterOperation.Equals, oldItem.GetIdentifier())
              };
              list.UpdateRecordSet(qo, new Dictionary<string, object>() 
              {
                { kv.FieldName, newItem.GetIdentifier() }
              });
            }
          }
          return true;
        });
    }

    /// <summary>
    /// Возвращает информацию о ссылках на переданный тип
    /// </summary>
    /// <param name="type">Тип, на который ищутся ссылки</param>
    /// <returns>Массив ссылок</returns>
    public static ReferenceInfo[] GetReferences(Type type)
    {
      return (ReferenceInfo[])typeof(RecordCache).GetMethod("GetReferences", BindingFlags.Public
        | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type)
        .Invoke(null, new object[] { });
    }

    /// <summary>
    /// Возвращает экземпляр бизнес-объекта по имени таблицы
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public static IBindable GetItem(string tableName, object identifier)
    {
      Type itemType = RecordCache.GetTypeByName(tableName);

      return GetItem(itemType, identifier);
    }

    /// <summary>
    /// Возвращает бизнес-объект из кэша по идентификатору
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    /// <param name="identifier">Значение идентификатора</param>
    /// <returns>Если идентификатор пустой или объект не найден в кэше, то новый объект.
    /// Иначе, объект, найденный в кэше</returns>
    public static TType GetItem<TType>(object identifier)
     where TType : class, IBindable, new()
    {
      return RecordCache.GetItem<TType>(identifier);
    }

    /// <summary>
    /// Возвращает бизнес-объект из кэша по идентификатору
    /// </summary>
    /// <param name="type">Тип бизнес-объекта</param>
    /// <param name="identifier">Значение идентификатора</param>
    /// <returns>Если идентификатор пустой или объект не найден в кэше, то новый объект.
    /// Иначе, объект, найденный в кэше</returns>
    public static IBindable GetItem(Type type, object identifier)
    {
      return RecordCache.GetItem(type, identifier);
    }

    /// <summary>
    /// Очищает кэш для заданного типа бизнес-объекта
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    public static void ClearCache<TType>()
      where TType : class, IBindable, new()
    {
      RecordCache.InvalidateType<TType>(null);
      RecordCache.InvalidateByCache<TType>();
    }

    /// <summary>
    /// Однофазно заполняет кэш для заданного типа бизнес-объекта
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    public static void BindSingleType<TType>()
        where TType : class, IBindable, new()
    {
      RecordCache.BindSingleType<TType>();
    }

    /// <summary>
    /// Обновляет перекрестные ссылки по внешним ключам
    /// </summary>
    public static void UpdateReferences()
    {
      RecordCache.UpdateReferences(null);
    }

    /// <summary>
    /// Возвращает новй экземпляр типизированной коллекции, привязанной к источнику данных
    /// </summary>
    /// <param name="itemType">Тип бизнес-объекта</param>
    /// <param name="autoBind">Указывает, надо ли заполнять коллекцию</param>
    /// <param name="query">Запрос, по которому надо заполнить коллекцию</param>
    /// <returns></returns>
    public static IBindableObjectList GetTypedList(Type itemType, bool autoBind, QueryObject query)
    {
      if (!itemType.IsClass || itemType.GetInterface("AT.Toolbox.MSSQL.DAL.RecordBinding.IBindable") == null)
      {
        throw new ArgumentException(Resources.RecordManager_InvalidType, "itemType");
      }
      IBindableObjectList ret = Activator.CreateInstance(typeof(BindableObjectList<>).MakeGenericType(itemType))
          as IBindableObjectList;

      if (autoBind)
      {
        if (query != null)
        {
          ret.DataBind(query);
        }
        else
        {
          ret.DataBind();
        }
      }

      return ret;
    }

    #endregion

    #region Metadata helpers

    /// <summary>
    /// Заполняет переданную коллекцию локализованными метками для перечисления
    /// </summary>
    /// <param name="enumType">Тип перечисления</param>
    /// <param name="destination">Коллекция, которую требуется заполнить</param>
    /// <returns>Размер самой длинной метки в символах</returns>
    public static int FillEnumLabels(Type enumType, IList<ObjectTextStore> destination)
    {
      int maxTextLength = 0;
      foreach (FieldInfo field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        string text = field.Name;
        object[] attributes = field.GetCustomAttributes(typeof(DisplayNameAttribute), true);
        if (attributes.Length > 0)
        {
          text = (attributes[0] as DisplayNameAttribute).DisplayName;
        }
        ObjectTextStore item = new ObjectTextStore();
        item.Value = Enum.Parse(enumType, field.Name);
        item.Text = text;
        if (item.Text != null)
          maxTextLength = Math.Max(item.Text.Length, maxTextLength);
        destination.Add(item);
      }
      return maxTextLength;
    }

    /// <summary>
    /// Возвращает заголовок элемента перечисления
    /// </summary>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Заголовок метки</returns>
    public static string GetLabel(this Enum value)
    {
      if (value == null)
        return null;

      List<ObjectTextStore> labels = new List<ObjectTextStore>();
      FillEnumLabels(value.GetType(), labels);

      foreach (var ot in labels)
      {
        if (value.Equals(ot.Value))
          return ot.Text;
      }

      return string.Empty;
    }

    /// <summary>
    /// Возвращает имя таблицы в базе данных, связанной с бизнес-объектом
    /// </summary>
    /// <typeparam name="TType">Тип бизнес-объекта</typeparam>
    /// <returns>Чаще всего имя таблицы совпадает с именем класса;
    /// различия возникают, когда имя таблицы недопустимо в качестве идентификатора</returns>
    public static string GetTableName<TType>()
        where TType : class, IBindable, new()
    {
      if (typeof(TType).IsDefined(typeof(TableNameAttribute), true))
      {
        return (typeof(TType).GetCustomAttributes(typeof(TableNameAttribute), true)
          [0] as TableNameAttribute).TableName;
      }
      else
      {
        return typeof(TType).Name;
      }
    }

    /// <summary>
    /// Возвращает имя таблицы в базе данных, связанной с бизнес-объектом
    /// </summary>
    /// <param name="objectType">Тип бизнес-объекта</param>
    /// <returns>Чаще всего имя таблицы совпадает с именем класса;
    /// различия возникают, когда имя таблицы недопустимо в качестве идентификатора</returns>
    public static string GetTableName(Type objectType)
    {
      if (objectType == null)
        throw new ArgumentNullException("objectType");

      if (objectType.IsDefined(typeof(TableNameAttribute), true))
      {
        return (objectType.GetCustomAttributes(typeof(TableNameAttribute), true)
          [0] as TableNameAttribute).TableName;
      }
      else
      {
        return objectType.Name;
      }
    }


    // <summary>
    /// Возвращает тип бизнес-объекта, связанного с данной таблицей
    /// </summary>
    /// <param name="tableName">Тимя таблицы</param>
    /// <returns>если среди типов нашелся тип, у которого TableNameAttribute.TableName = tableName,
    /// то возвращает его. Иначе null</returns>
    public static Type GetTypeByTableName(string tableName) 
    {
      return GetItem(tableName, null).GetType();
    }

    /// <summary>
    /// Возвращает имена полей, связанных с базой данных, для переданного типа бизнес-объекта
    /// </summary>
    /// <param name="objectType">Тип бизнес-объекта</param>
    /// <returns>Имена полей так, как они называются в базе данных</returns>
    public static string[] GetFieldNames(Type objectType)
    {
      if (objectType == null)
        throw new ArgumentNullException("objectType");

      if (!typeof(IBindable).IsAssignableFrom(objectType))
        throw new ArgumentException("objectType does not implement IBindable");
      
      Type fieldNamesType = objectType.GetNestedType("FieldNames");
      if (fieldNamesType == null)
        return new string[0];

      List<string> buffer = new List<string>();
      foreach (FieldInfo field in fieldNamesType.GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        buffer.Add((string)field.GetValue(null));
      }
      return buffer.ToArray();
    }

    /// <summary>
    /// Возвращает имя поля по имени свойства
    /// </summary>
    /// <param name="objectType">Тип бизнес-объекта</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <returns>Имя поля, связанного с переданным свойством</returns>
    public static string GetFieldName(Type objectType, string propertyName)
    {
      bool redirected;

      return GetFieldName(objectType, propertyName, out redirected);
    }

    /// <summary>
    /// Возвращает имя поля по имени свойства
    /// </summary>
    /// <param name="objectType">Тип бизнес-объекта</typeparam>
    /// <param name="propertyName">Имя свойства</param>
    /// <param name="redirected">Логическое значение, показывающее, что
    /// переданное свойство не связано с базой данных само, 
    /// а является псевдонимом для другого свойства, связаным с базой данных</param>
    /// <returns>Имя поля, связанного с переданным свойством</returns>
    internal static string GetFieldName(Type objectType, string propertyName, out bool redirected)
    {
      if (objectType == null)
        throw new ArgumentNullException("objectType");

      PropertyDescriptor property = TypeDescriptor.GetProperties(objectType)[propertyName];
      redirected = false;

      if (property == null)
        return null;

      PropertyRedirectAttribute redirect = property.Attributes[typeof(PropertyRedirectAttribute)]
       as PropertyRedirectAttribute;

      if (redirect != null)
      {
        redirected = true;
        return redirect.FieldName;
      }

      Type fieldNamesType = objectType.GetNestedType("FieldNames");
      if (fieldNamesType != null)
      {
        try
        {
          return fieldNamesType.GetField(propertyName, BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
        }
        catch (Exception ex)
        {
          return null;
        }
      }
      return propertyName;
    }

    /// <summary>
    /// Возвращает имя свойства, связанного с переданным полем в БД
    /// </summary>
    /// <param name="objectType">Тип бизнес-объекта</typeparam>
    /// <param name="fieldName">Имя поля в базе данных</param>
    /// <returns>Имя свойства</returns>
    public static string GetPropertyName(Type objectType, string fieldName)
    {
      if (objectType == null)
        throw new ArgumentNullException("objectType");

      Type fieldNamesType = objectType.GetNestedType("FieldNames");
      if (fieldNamesType != null)
      {
        try
        {
          foreach (FieldInfo field in fieldNamesType.GetFields(BindingFlags.Public | BindingFlags.Static))
          {
            if ((string)field.GetValue(null) == fieldName)
            {
              return field.Name;
            }
          }
        }
        catch (Exception ex)
        {
          return "";
        }
      }
      return fieldName;
    }

    #endregion

    #region Strategy members (Write log / Update query)

    public static bool AutoSaveOnEndEdit { get; set; }

    //public static ISecurityControl SecurityControl { get; set; }

    /// <summary>
    /// Дополнительно обрабатывает запрос с использованием
    /// установленной стратегии. Если стратегия не установлена, запрос не обрабатывается
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="source">Запрос, который требуется обработать</param>
    /// <returns>Обработанный запрос</returns>
    public static QueryObject UpdateQuery(string tableName, QueryObject source)
    {
      lock (factoryLocker)
      {
        //if (SecurityControl == null)
          return source;

        //else return SecurityControl.UpdateQuery(tableName, source);
      }
    }

    /// <summary>
    /// Устанавливает стратегию протоколирования
    /// </summary>
    public static void SetWriteLogStrategy(IWriteLogStrategy writeLogStrategy)
    {
      lock (appLogLocker)
      {
        if (writeLogStrategy == null)
        {
          throw new ArgumentNullException("writeLogStrategy");
        }
        currentLogStrategy = writeLogStrategy;
      }
    }

    /// <summary>
    /// Протоколирует сообщение с использованием установленной стратегии
    /// </summary>
    /// <param name="info">Сообщение</param>
    public static void WriteLog(Info info)
    {
      WriteLog(info.Message, info.InfoLevel);
      if (!string.IsNullOrEmpty(info.Description))
      {
        currentLogStrategy.WriteLog(info.Description, info.InfoLevel);
      }
    }

    /// <summary>
    /// Протоколирует сообщение с использованием установленной стратегии
    /// </summary>
    /// <param name="eventDescription">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public static void WriteLog(string eventDescription, InfoLevel state)
    {
      lock (appLogLocker)
      {
        if (currentLogStrategy == null)
        {
          currentLogStrategy = new ATWriteLogStrategy();
        }
        currentLogStrategy.WriteLog(eventDescription, state);
      }
    }

    /// <summary>
    /// Протоколирует ошибку с использованием установленной стратегии
    /// </summary>
    /// <param name="ex">Ошибка</param>
    public static void ReportError(Exception ex)
    {
      lock (appLogLocker)
      {
        if (currentLogStrategy == null)
        {
          currentLogStrategy = new ATWriteLogStrategy();
        }
        currentLogStrategy.ReportError(ex);
      }
    }

    #endregion

    #region Aspect management

    public static List<IBindingAspect> Aspects
    {
      get { return aspects; }
    }

    public static void AutoLoadAspects()
    {
      aspects.Clear();
      HashSet<string> typeNames = new HashSet<string>();
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        try
        {
          if (!asm.IsAtToolbox())
            continue;

          foreach (Type currentType in asm.GetTypes())
          {
            if (!currentType.IsAbstract
              && typeof(IBindingAspect).IsAssignableFrom(currentType)
              && currentType.GetConstructor(Type.EmptyTypes) != null
              && !typeNames.Contains(currentType.FullName))
            {
              aspects.Add((IBindingAspect)Activator.CreateInstance(currentType));
              typeNames.Add(currentType.FullName);
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    #endregion

    #region Context handlers

    /// <summary>
    /// Контексты для работы с компонентной моделью
    /// </summary>
    internal static List<DbContext> Contexts
    {
      get { return contexts; }
    }

    /// <summary>
    /// Посылает всем контекстам сообщение об ошибке
    /// </summary>
    /// <param name="ex">Ошибка</param>
    public static void SendErrorMessage(Exception ex)
    {
      bool handled = false;
      foreach (DbContext context in Contexts)
      {
        handled = context.SendErrorMessage(ex) || handled;
      }
      if (!handled)
      {
        throw new Exception(Properties.Resources.ContextException, ex);
      }
    }

    /// <summary>
    /// Посылает всем констекстам набор сообщений
    /// </summary>
    /// <param name="messageBuffer">Набор сообщений</param>
    public static void SendMessageList(InfoEventArgs messageBuffer)
    {
      foreach (DbContext context in Contexts)
      {
        context.SendMessageList(messageBuffer);
      }
    }

    public static void SendMessage(Info info)
    {
      var args = new InfoEventArgs();
      args.Messages.Add(info);
      SendMessageList(args);
    }

    #endregion

    #region Transaction handlers

    /// <summary>
    /// Выполняет операцию в транзакции
    /// </summary>
    /// <param name="bdr">Привязчик к источнику данных</param>
    /// <param name="iso">Уровень изоляции для транзакции</param>
    /// <param name="condition">Операция, выполняемая в транзакции</param>
    /// <returns>Успех операции</returns>
    public static bool RunInTransaction(IRecordBinder bdr, IsolationLevel iso, Func<bool> condition)
    {
      bdr.BeginTransaction(iso);
      try
      {
        bool ret = condition();
        if (ret)
        {
          bdr.CommitTransaction();
        }
        else
        {
          bdr.RollbackTransaction();
        }
        return ret;
      }
      catch (Exception ex)
      {
        bdr.RollbackTransaction();
        throw;
      }
    }

    /// <summary>
    /// Выполняет операцию в транзакции с использованием привязчика, 
    /// порождённого установленной фабрикой
    /// </summary>
    /// <param name="iso">Уровень изоляции для транзакции</param>
    /// <param name="condition">Операция, выполняемая в транзакции</param>
    /// <returns>Успех операции</returns>
    public static bool RunInTransaction(IsolationLevel iso, Func<bool> condition)
    {
      using (IRecordBinder bdr = GetRecordBinder(new Dictionary<BindingAction, string>()))
      {
        return RunInTransaction(bdr, iso, condition);
      }
    }

    #endregion

    #region IBindable extensions

    /// <summary>
    /// Возвращает идентификатор переданного бизнес-объекта
    /// </summary>
    /// <param name="item">Бизнес-объект</param>
    /// <returns>Если передано пустое значение, или бизнес-объект имеет составной первичный ключ,
    /// то всегда null. Иначе, значение идентификатора</returns>
    public static object GetIdentifier(this IBindable item)
    {
      if (item == null)
        return null;

      if (string.IsNullOrEmpty(item._KeyFieldName))
        return null;

      return item[item._KeyFieldName];
    }

    /// <summary>
    /// Ищет объект по переданным реквизитам, если не найден, создает такой объект в базе
    /// </summary>
    /// <param name="item">Объект</param>
    /// <param name="fieldValues"></param>
    /// <returns>True, если объект найден или создан</returns>
    public static bool FindOrCreateCommon(this IBindable item, Dictionary<string, object> fieldValues)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      if (item._IsDataBound)
        throw new InvalidOperationException("Object to find or create must be unbound");

      string[] fieldNames = GetFieldNames(item.GetType());

      foreach (KeyValuePair<string, object> kv in fieldValues)
      {
        if (!Array.Exists<string>(fieldNames, p => p == kv.Key))
          throw new ArgumentException(string.Format("Field {0} not exists in business object", kv.Key));
      }
      
      QueryObject qo = QueryObject.CreateFromFieldValues(fieldValues);

      item.InitReader(qo);

      if (item.Next())
      {
        item.Break();
        return true;
      }
      else
      {
        foreach (KeyValuePair<string, object> kv in fieldValues)
        {
          item[kv.Key] = kv.Value;
        }
        InfoEventArgs args = new InfoEventArgs();

        if (item.ValidateWrite(args))
        {
          return item.Insert();
        }
        else
        {
          SendMessageList(args);
          return false;
        }
      }
    }

    public static IBindable CreateObjectByFieldValues (string tableName, Dictionary<string, object> dict, InfoEventArgs errors)
    {
      Type objType = RecordManager.GetTypeByTableName(tableName);
      
      if (objType == null)
        return null;

      IBindable obj = Activator.CreateInstance(objType) as IBindable;
      PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(obj);

      foreach (string fieldName in dict.Keys)
      {
        try
        {
          obj[fieldName] = dict[fieldName];
        }
        catch (Exception ex)
        {
          errors.Messages.Add(new Info("Полю " + fieldName + 
                                        " не удалось присвоить значение " + dict[fieldName] + 
                                          " типа " + dict[fieldName].GetType(), InfoLevel.Error));
                                      
        }
      }

      //obj.ValidateWrite(errors);
      return obj;
    }

    #endregion
  }

  public class ATWriteLogStrategy : IWriteLogStrategy
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ATWriteLogStrategy).Name);

    #region IWriteLogStrategy Members

    public void WriteLog(string message, InfoLevel state)
    {
      switch (state)
      {
        case InfoLevel.Error:
          Log.Error(message);
          break;
        case InfoLevel.Warning:
          Log.Warn(message);
          break;
        case InfoLevel.Info:
          Log.Info(message);
          break;
      }
    }

    public void ReportError(Exception ex)
    {
      Log.Error(ex.Message, ex);
    }

    #endregion
  }
}