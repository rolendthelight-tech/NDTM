using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using AT.Toolbox.MSSQL.Properties;
using System.Xml.Serialization;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using AT.Toolbox.Log;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Базовый класс, реализующий IBindable, от которого наследуются все сохраняемые объекты бизнес-логики
  /// </summary>
  /// <typeparam name="TType">Тип текущего класса бизнес-логики, унаследованного от BindableObject</typeparam>
  [Serializable]
  public abstract partial class BindableObject<TType> : FdTypeDescriptor, IBindable, INotifyPropertyChanged, INotifyPropertyChanging, IEditableObject, IServiceRecord, IChangeTracking, ISerializable
      where TType : BindableObject<TType>, new()
  {
    #region Fields

    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BindableObject<TType>).Name);

    IRecordBinder record_binder;                      // Привязчик для базы
    IFieldBinder field_binder;                        // Привязчик для полей
    static BindingRequisites binding_requisites;      // Реквизиты привязки
    string[] currentlyBoundFields;                    // Текущий набор связанных с базой полей

    TType orig;                     // Неизменённая запись из базы данных (оригинал)
    CacheStatus cacheStatus = CacheStatus.Free;         // Признак, что объект находится в кэше
    object locker = new object();   // Объект блокировки операций привязки данных

    volatile bool isDataBound = false;    // Признак того, что данные загружены из базы
    volatile bool modified = false;       // Признак, что данные были изменены
    volatile bool pendingBreak = false;   // Признак, что проход курсора был прерван
    volatile bool uncommitted = false;    // Флаг для запрета множественного оповещения
    volatile bool aspectWorking = false;  // Флаг для запрета рекурсивных вызовов через аспекты
    volatile bool disableBLL = false;     // Флаг для отключения переопределённой бизнес-логики
    volatile bool initializing = false;   // Флаг, отключающий смену статуса при редактировании свойств

    BindingResult m_server_result;

    // Объекты, на которые ссылаются отношения текущего объекта
    Dictionary<string, ReferenceStore> referencedObjects;

    // Коллекции объектов, ссылающихся на текущий через отношения
    Dictionary<string, IBindableObjectList> referenceCollections = new Dictionary<string, IBindableObjectList>();

    Dictionary<string, IEditableCollection> editableCollections = new Dictionary<string, IEditableCollection>();

    Dictionary<string, IEditableReference> editableObjects = new Dictionary<string, IEditableReference>();

    #endregion

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;
    public event PropertyChangingEventHandler PropertyChanging;

    #endregion

    #region Constructors

    /// <summary>
    /// Инициализирует новый сохраняемый объект
    /// </summary>
    protected BindableObject()
    {
      lock (st_locker)
      {
        if (binding_requisites == null)
        {
          binding_requisites = new BindingRequisites();

          binding_requisites.Aspects = new Dictionary<Type, IBindingAspect>();
          if (this is IBindableEntity)
          {
            BindableAspectManager.InitAspects(binding_requisites);
          }

          binding_requisites.IdentityField = this.GetIdentityField();

          binding_requisites.DataBoundFieldNames = this.GetDataBoundFieldNames();
          if (binding_requisites.DataBoundFieldNames == null)
          {
            throw new ArgumentNullException("dataBoundFieldNames");
          }

          binding_requisites.KeyFieldNames = this.GetKeyFieldNames();
          if (binding_requisites.KeyFieldNames == null)
          {
            throw new ArgumentNullException("keyFieldNames");
          }

          binding_requisites.FieldMapping = new Dictionary<string, string>();
          foreach (string fieldName in binding_requisites.DataBoundFieldNames)
          {
            binding_requisites.FieldMapping.Add(fieldName, RecordManager.GetPropertyName(typeof(TType), fieldName));
          }

          if (typeof(TType).IsDefined(typeof(TableNameAttribute), true))
          {
            binding_requisites.TableName = (typeof(TType).GetCustomAttributes(typeof(TableNameAttribute), true)
              [0] as TableNameAttribute).TableName;
          }
          else
          {
            binding_requisites.TableName = typeof(TType).Name;
          }

          binding_requisites.ProcedureActions = this.GetProcedureActions() ?? new Dictionary<BindingAction, string>();
        }
      }

      this.field_binder = this.GetFieldBinder();
      if (this.field_binder == null)
      {
        throw new ArgumentNullException("fieldBinder");
      }

      this.record_binder = this.GetRecordBinder();
      if (this.record_binder == null)
      {
        throw new ArgumentNullException("recordBinder");
      }

      this.referencedObjects = this.GetReferences();
    }

    /// <summary>
    /// Инициализирует новый объект с привязкой по первичному ключу
    /// </summary>
    /// <param name="keyFields">Значения полей первичного ключа</param>
    protected BindableObject(Dictionary<string, object> keyFields)
      : this()
    {
      foreach (string fieldName in binding_requisites.KeyFieldNames)
      {
        this.field_binder.SetFieldValue(fieldName, keyFields[fieldName]);
      }
      this.DataBind();
    }

    ~BindableObject()
    {
      this.Dispose(false);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Возвращает неизмененную копию текущей записи из базы данных
    /// </summary>
    [Browsable(false)]
    public TType _Orig
    {
      get
      {
        this.InitOrig();
        return this.orig;
      }
    }

    /// <summary>
    /// Признак того, что объект привязан к базе данных
    /// </summary>
    [Browsable(false)]
    public bool _IsDataBound
    {
      get { return isDataBound; }
    }

    /// <summary>
    /// Признак, что объект был модифицирован после создания или последней привязки
    /// </summary>
    [Browsable(false)]
    public bool _Modified
    {
      get { return modified; }
    }

    /// <summary>
    /// Имя таблицы в базе данных
    /// </summary>
    [Browsable(false)]
    public virtual string _TableName
    {
      get
      {
        return binding_requisites.TableName;
      }
    }
    string IBindable._KeyFieldName
    {
      get
      {
        if (binding_requisites.KeyFieldNames.Length == 1)
        {
          return binding_requisites.KeyFieldNames[0];
        }
        return null;
      }
    }

    CacheStatus IBindable._CacheStatus
    {
      get { return this.cacheStatus; }
      set { this.cacheStatus = value; }
    }

    object IBindable.this[string fieldName]
    {
      get
      {
        return this.field_binder.GetFieldValue(fieldName);
      }
      set
      {
        Type callerType = new StackTrace(0).GetFrame(1).GetMethod().DeclaringType;
        if (callerType.Assembly != typeof(BindableObject<>).Assembly)
        {
          string prop = binding_requisites.FieldMapping[fieldName];

          PropertyInfo pi = typeof(TType).GetProperty(prop);

          bool ok = false;

          if (value != null)
          {
            if (pi.PropertyType.IsAssignableFrom(value.GetType()))
            {
              this.SetFieldValue<object>(fieldName, value);
              ok = true;
            }
          }
          else if (!pi.PropertyType.IsValueType ||
            (pi.PropertyType.IsGenericType
            && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
          {
            this.SetFieldValue<object>(fieldName, value);
            ok = true;
          }

          if (!ok)
            throw new InvalidOperationException(Resources.BindableObject_DirectFieldAccess);

        }
        else
          this.field_binder.SetFieldValue(fieldName, value);
      }
    }

    #endregion

    #region IBindable methods

    void IBindable.InitValues() 
    {
      this.initializing = true;
      this.InitValues();
      this.initializing = false;
    }
    /// <summary>
    /// Привязывает текущий объект к базе данных по ключу
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    public bool DataBind()
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      lock (this.locker)
      {
        this.BeforeDataBind();

        BindingResult result = new BindingResult(BindingAction.Get);

        this.currentlyBoundFields = record_binder.OpenReader(this._TableName,
          this.GetFieldValues(binding_requisites.KeyFieldNames));
        result.Success = record_binder.Next();
        if (result.Success)
        {
          if (!this.field_binder.ReadDataFields(this.currentlyBoundFields, this.record_binder))
          {
            for (int i = 0; i < this.currentlyBoundFields.Length; i++)
            {
              result.Fields[this.currentlyBoundFields[i]] = this.record_binder.GetFieldValue(i);
            }
          }
        }
        record_binder.CloseReader();

        this.AfterDataBind(result);
        BindableAspectManager.AfterDataBound(this as TType, result);

        this.ResetReferences();
        this.isDataBound = result.Success;
        this.modified = result.Success ? false : this.modified;
        return result.Success;
      }
    }

    /// <summary>
    /// Вставляет текущую запись в базу данных
    /// </summary>
    /// <returns></returns>
    public bool Insert()
    {
      BindingResult result = null;
      lock (this.locker)
      {
        result = RecordManager.BindOnServer(this, BindingAction.Insert);

        if (result == null)
          return false;

        if (result.Success)
        {
          this.AfterWrite(result);
        }
      }
      if (result.Messages.Count > 0)
      {
        this.NotifyContext(new InfoEventArgs(result.Messages));
      }
      return result.Success;
    }

    /// <summary>
    /// Обновляет текущую запись в базе данных
    /// </summary>
    /// <returns></returns>
    public bool Update()
    {
      BindingResult result = null;
      lock (this.locker)
      {
        result = RecordManager.BindOnServer(this, BindingAction.Update);

        if (result == null)
          return false;

        if (result.Success)
        {
          this.AfterWrite(result);
        }
        else
        {
          this.CancelEdit();
          this.ResetReferences();
        }
      }
      if (result.Messages.Count > 0)
      {
        this.NotifyContext(new InfoEventArgs(result.Messages));
      }
      return result.Success;
    }

    /// <summary>
    /// Удаляет текущую запись из базы данных
    /// </summary>
    /// <returns></returns>
    public bool Delete()
    {
      BindingResult result = null;
      lock (this.locker)
      {
        result = RecordManager.BindOnServer(this, BindingAction.Delete);

        if (result == null)
          return false;

        if (result.Success)
        {
          if (this.orig != null)
          {
            this.orig.Dispose();
            this.orig = null;
          }

          this.isDataBound = false;
          this.InvalidateContext();
        }
      }
      if (result.Messages.Count > 0)
      {
        this.NotifyContext(new InfoEventArgs(result.Messages));
      }
      return result.Success;
    }

    /// <summary>
    /// Инициализирует курсор для прохода по набору записей
    /// </summary>
    /// <param name="query">Запрос, по которому возвращается набор</param>
    /// <returns>True, если операция прошла успешно</returns>
    public bool InitReader(QueryObject query)
    {
      if (this.cacheStatus != CacheStatus.Free)
      {
        throw new InvalidOperationException(Resources.BindableObject_CacheCorruption);
      }
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      lock (this.locker)
      {
        BindableAspectManager.BuildingQuery(this, query);
        this.currentlyBoundFields = this.record_binder.OpenReader(this._TableName, query);
        this.modified = false;
        this.pendingBreak = false;
        return true;
      }
    }

    /// <summary>
    /// Читает очередную запись из набора и инициализирует поля
    /// </summary>
    /// <returns>True, если не достигнут конец набора записей</returns>
    public bool Next()
    {
      if (this.cacheStatus != CacheStatus.Free)
      {
        throw new InvalidOperationException(Resources.BindableObject_CacheCorruption);
      }
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      lock (this.locker)
      {
        if (this.pendingBreak)
        {
          this.pendingBreak = false;
          return false;
        }

        bool ret = record_binder.Next();
        if (ret)
        {
          this.field_binder.ReadDataFields(this.currentlyBoundFields, this.record_binder);
          this.AfterDataBind(new BindingResult(BindingAction.List) { Success = true });
        }
        else
        {
          record_binder.CloseReader();
        }
        this.ResetReferences();
        this.isDataBound = ret;
        this.modified = ret ? false : this.modified;
        return ret;
      }
    }

    /// <summary>
    /// Прерывает проход по набору записей
    /// </summary>
    public void Break()
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      lock (this.locker)
      {
        pendingBreak = true;
        this.record_binder.CloseReader();
      }
    }

    bool IBindable.ValidateWrite(InfoEventArgs messageBuffer)
    {
      bool ret = this.CheckRelations(messageBuffer);
      foreach (string currentField in binding_requisites.DataBoundFieldNames)
      {
        ret = this.ValidateField(currentField,
          this.field_binder.GetFieldValue(currentField), messageBuffer) && ret;
      }
      foreach (IEditableCollection coll in this.editableCollections.Values)
      {
        for (int i = 0; i < coll.Count; i++)
        {
          IBindable item = coll[i];
          if (item._Modified || coll.IsAdded(i))
          {
            ret = item.ValidateWrite(messageBuffer) && ret;
          }
        }
        foreach (IBindable item in coll.ItemsToDelete)
        {
          if (item._IsDataBound)
            ret = item.ValidateDelete(messageBuffer) && ret;
        }
      }
      foreach (IEditableReference refer in this.editableObjects.Values)
      {
        if (refer.Value != null)
        {
          ret = refer.Value.ValidateWrite(messageBuffer) && ret;
        }
      }
      ret = this.ValidateWrite(messageBuffer) && ret;
      return ret;
    }

    bool IBindable.ValidateDelete(InfoEventArgs messageBuffer)
    {
      bool ret = RecordCache.ValidateDelete(this, messageBuffer);
      return this.ValidateDelete(messageBuffer) && ret;
    }

    #endregion

    #region System interface methods

    public virtual TType Clone()
    {
      TType ret = (TType)this.MemberwiseClone();
      ret.orig = this.record_binder == null ? ret : null;
      ret.record_binder = ret.GetRecordBinder();
      ret.field_binder = this.field_binder.Clone(ret);
      ret.aspectWorking = false;
      ret.cacheStatus = CacheStatus.Free;

      foreach (FieldInfo fi in typeof(TType).GetFields())
      {
        if (fi.IsDefined(typeof(PackableFieldAttribute), true))
        {
          try
          {
            fi.SetValue(ret, null);
          }
          catch (Exception ex)
          {
            Log.Error("Clone(): exception", ex);
          }
        }
      }

      ret.referencedObjects = new Dictionary<string,ReferenceStore>(this.referencedObjects);
      ret.referenceCollections = new Dictionary<string, IBindableObjectList>();
      ret.editableObjects = new Dictionary<string, IEditableReference>(this.editableObjects);
      ret.editableCollections = new Dictionary<string, IEditableCollection>(this.editableCollections);
      return ret;
    }

    public void Dispose()
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        lock (this.locker)
        {
          this.PropertyChanging = null;
          this.PropertyChanged = null;

          if (this.record_binder != null)
          {
            this.record_binder.Dispose();
          }
        }
      }
    }

    public override string ToString()
    {
      if (typeof(TType).IsDefined(typeof(DisplayNameAttribute), true))
      {
        DisplayNameAttribute dna = typeof(TType).GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
          as DisplayNameAttribute;

        return dna.DisplayName;
      }
      else
      {
        return this._TableName;
      }
    }

    void IEditableObject.BeginEdit() { }

    public void CancelEdit()
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      if (this.orig == null && !this.modified)
      {
        return;
      }

      bool problem = false;
      if (orig != null)
      {
        foreach (string fieldName in binding_requisites.DataBoundFieldNames)
        {
          try
          {
            this.field_binder.SetFieldValue(fieldName, this.orig.GetFieldValue<object>(fieldName));
          }
          catch (Exception ex)
          {
            Log.Error("CancelEdit(): exception", ex);
            problem = true;
          }
        }
      }
      this.modified = problem;
      if (!problem)
      {
        BindableAspectManager.CancelEdit(this);
      }
    }

    void IEditableObject.EndEdit()
    {
      if (this.modified && RecordManager.AutoSaveOnEndEdit)
      {
        this.EndEditCore();
      }
    }

    bool IChangeTracking.IsChanged
    {
      get { return modified; }
    }

    void IChangeTracking.AcceptChanges()
    {
      this.EndEditCore();
    }

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    #endregion

    #region BLL util methods

    /// <summary>
    /// Привязывает текущий объект к базе данных по набору значений полей
    /// </summary>
    /// <param name="parameters">Значения полей</param>
    /// <returns>True, если операция прошла успешно</returns>
    protected bool DoDataBind(Dictionary<string, object> parameters)
    {
      if (this.cacheStatus != CacheStatus.Free)
      {
        throw new InvalidOperationException(Resources.BindableObject_CacheCorruption);
      }
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      lock (this.locker)
      {
        try
        {
          QueryObject query = QueryObject.CreateFromFieldValues(parameters);
          BindableAspectManager.BuildingQuery(this, query);
          BindingResult result = new BindingResult(BindingAction.Get);

          this.currentlyBoundFields = record_binder.OpenReader(this._TableName, query);
          result.Success = record_binder.Next();
          if (result.Success)
          {
            this.field_binder.ReadDataFields(this.currentlyBoundFields, this.record_binder);
          }
          record_binder.CloseReader();

          BindableAspectManager.AfterDataBound(this, result);
          this.ResetReferences();
          this.isDataBound = result.Success;
          this.modified = result.Success ? false : this.modified;

          return result.Success;
        }
        catch (Exception ex)
        {
          Log.Error("DoDataBind(): exception", ex);
          return this.CheckFailed(ex, true);
        }
      }
    }

    public bool DoInsert()
    {
      this.disableBLL = true;
      try
      {
        return this.Insert();
      }
      finally
      {
        this.disableBLL = false;
      }
    }

    public bool DoUpdate()
    {
      this.disableBLL = true;
      try
      {
        return this.Update();
      }
      finally
      {
        this.disableBLL = false;
      }
    }

    public bool DoDelete()
    {
      this.disableBLL = true;
      try
      {
        return this.Delete();
      }
      finally
      {
        this.disableBLL = false;
      }
    }

    /// <summary>
    /// Возвращает ссылку на объект, с которым данный объект связан внешним ключом
    /// </summary>
    /// <typeparam name="TRef">Тип связанного объека</typeparam>
    /// <param name="propertyName">Имя свойства текущего класса, используемого в качестве внешнего ключа</param>
    /// <param name="allowNull">Параметр, определяющий, можно ли возвращать null</param>
    /// <returns>Если объект найден в базе, ссылку на этот объект. Иначе, если allowNull == true, пустое значение. 
    /// Иначе, новый объект нужного типа.</returns>
    protected TRef GetReference<TRef>(string fieldName, bool allowNull)
      where TRef : class, IBindable, new()
    {
      if (!referencedObjects.ContainsKey(fieldName))
      {
        throw new ArgumentException(string.Format(Resources.BindableObject_FK_NotFound, fieldName));
      }
      ReferenceStore reference = referencedObjects[fieldName];

      if (reference.reference == null
          || (RecordCache.ValidateType<TRef>(this)))
      {
        // Вызываем конструктор с параметром, который осуществляет привязку по ключу
        object fieldValue = this.field_binder.GetFieldValue(fieldName);
        if (fieldValue != null)
        {
          reference.reference = Activator.CreateInstance(reference.referenceType, fieldValue) as TRef;
          reference.reference._CacheStatus = CacheStatus.LocalCache;
        }

        if (reference.reference != null)
        {
          if (!reference.reference._IsDataBound)
          {
            reference.reference = null;
          }
        }
        referencedObjects[fieldName] = reference;
      }

      return allowNull ? reference.reference as TRef :
        (reference.reference as TRef ?? new TRef());
    }

    /// <summary>
    /// Возвращает коллекцию объектов, отфильтрованных по внешнему ключу
    /// </summary>
    /// <typeparam name="TRef">Тип связанного объека</typeparam>
    /// <param name="propertyName">Имя свойства связанного класса, которое является внешним ключом</param>
    /// <returns></returns>
    protected BindableObjectList<TRef> GetReferenceCollection<TRef>(string fieldName)
      where TRef : class, IBindable, new()
    {
      if (!this.isDataBound)
      {
        return new BindableObjectList<TRef>();
      }
      BindableObjectList<TRef> ret = null;
      string fullPropertyName = typeof(TRef).Name + "." + fieldName;
      if (!referenceCollections.ContainsKey(fullPropertyName))
      {
        ret = new BindableObjectList<TRef>();
        BindableObjectList<TType> parent = new BindableObjectList<TType>();
        parent.Add(this as TType);
        ret.ParentList = parent;
        ret.RelationField = fieldName;
        ret.IsInLocalCache = true;
        ret.DataBind();
        referenceCollections.Add(fullPropertyName, ret);
      }
      else
      {
        ret = referenceCollections[fullPropertyName] as BindableObjectList<TRef>;
        if (RecordCache.ValidateType<TRef>(this))
        {
          ret.DataBind();
        }
      }
      return ret;
    }

    /// <summary>
    /// Очищает все объекты, связанные с текущим
    /// </summary>
    protected void ClearCachedReferences()
    {
      this.referenceCollections.Clear();
      Dictionary<string, ReferenceStore> cloneDic = new Dictionary<string, ReferenceStore>();
      foreach (KeyValuePair<string, ReferenceStore> kv in this.referencedObjects)
      {
        cloneDic.Add(kv.Key, new ReferenceStore(kv.Value.referenceType, null));
        if (kv.Value.reference != null)
        {
          kv.Value.reference.Dispose();
        }
      }
      this.referencedObjects = cloneDic;
    }

    /// <summary>
    /// Помечает свойство как измененное
    /// </summary>
    /// <param name="propertyName">Имя свойства</param>
    protected void NotifyChanged(string fieldName)
    {
      try
      {
        this.NotifyChanged(fieldName, this.field_binder.GetFieldValue(fieldName));
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("NotifyChanged({0}): exception", fieldName), ex);
        this.NotifyChanged(fieldName, null);
      }
    }

    /// <summary>
    /// Проверяет корректность заполнения внешних ключей
    /// </summary>
    /// <returns>True, если каждый внешний ключ либо пустой, либо ссылается на существующую запись</returns>
    protected bool CheckRelations(InfoEventArgs messageBuffer)
    {
      bool ret = true;
      try
      {
        foreach (string fieldName in this.referencedObjects.Keys)
        {
          if (!this.isDataBound && fieldName == ((IBindable)this)._KeyFieldName)
            continue;

          PropertyDescriptor currentProperty = TypeDescriptor.GetProperties(this)[binding_requisites.FieldMapping[fieldName]];

          if (currentProperty == null)
            continue;

          object foreignKeyValue = this.field_binder.GetFieldValue(fieldName);
          if (foreignKeyValue == null)
            continue;

          Type referenceType
            = (currentProperty.Attributes[typeof(ReferenceAttribute)] as ReferenceAttribute).ReferenceType;

          IBindable finder = Activator.CreateInstance(referenceType, foreignKeyValue) as IBindable;

          if (!finder._IsDataBound)
          {
            messageBuffer.Messages.Add(new Info(string.Format(Resources.BindableObject_IncorrectForeignKey,
              string.IsNullOrEmpty(currentProperty.DisplayName) ? currentProperty.Name : currentProperty.DisplayName), InfoLevel.Warning));
            ret = false;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("CheckRelations(): exception", ex);
        return this.CheckFailed(ex, false);
      }
      return ret;
    }

    /// <summary>
    /// Читает значение поля из базы данных
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldNumber">Порядковый номер поля в текущем спике привязанных полей</param>
    /// <returns>Значение нужного типа.</returns>
    protected TFieldType ReadFieldValue<TFieldType>(int fieldNumber)
    {
      return this.CastFieldValue<TFieldType>(this.record_binder.GetFieldValue(fieldNumber));
    }

    /// <summary>
    /// Позволяет узнать значение поля, связанного с базой данных
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldName">Имя поля</param>
    /// <returns>Значение поля</returns>
    protected TFieldType GetFieldValue<TFieldType>(string fieldName)
    {
      return this.CastFieldValue<TFieldType>(this.field_binder.GetFieldValue(fieldName));
    }

    /// <summary>
    /// Устанавливает значение переданного поля, связанного с базой данных
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldName">Имя поля</param>
    /// <param name="field">Ссылка на изменяемое поле</param>
    /// <param name="newValue">Новое значение поля</param>
    protected void SetFieldValue<TFieldType>(string fieldName, ref TFieldType field, TFieldType newValue)
    {
      if (this.DoValidateField(fieldName, field, newValue))
      {
        field = this.ParseEditValue(newValue, fieldName);
        this.NotifyChanged(fieldName, newValue);
      }
    }

    /// <summary>
    /// Устанавливает значение поля, связанного с базой данных
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldName">Имя поля</param>
    /// <param name="newValue">Новое значение поля</param>
    protected void SetFieldValue<TFieldType>(string fieldName, TFieldType newValue)
    {
      if (this.DoValidateField(fieldName, this.field_binder.GetFieldValue(fieldName), newValue))
      {
        this.field_binder.SetFieldValue(fieldName, this.ParseEditValue(newValue, fieldName));
        this.NotifyChanged(fieldName, newValue);
      }
    }

    /// <summary>
    /// Оповещает контекст
    /// </summary>
    /// <param name="messageBuffer">Набор сообщений</param>
    protected void NotifyContext(InfoEventArgs messageBuffer)
    {
      this.NotifyContext(messageBuffer, false);
    }

    /// <summary>
    /// Реализация метода FindOrCreate по умолчанию. Реквизитами считаются
    /// все поля, кроме системного идентификатора и ObjectGuid, если есть. 
    /// Чтобы изменить набор реквизитов, необходимо переопределить GetRequisites()
    /// </summary>
    /// <returns>Идентификатор объекта, найденного по реквизитам</returns>
    protected object DoFindOrCreate()
    {
      if (binding_requisites.KeyFieldNames.Length != 1)
      {
        throw new InvalidOperationException("DoFindOrCreate can be only used for single-field key tables");
      }

      if (this.isDataBound || this.modified)
      {
        // Если объект не существует в базе данных, то необходимо либо привязаться по
        // реквизитам, либо создать новый объект с нужными реквизитами
        
        QueryObject searchQuery = new QueryObject();
        searchQuery.Aggregation.Add(binding_requisites.KeyFieldNames[0], AggregateOperation.None);
        GroupFilter group = new GroupFilter(true);
        searchQuery.RootFilter = group;

        var reqs = this.GetRequisites();
        foreach (string fieldName in reqs)
        {
          if (binding_requisites.KeyFieldNames[0] == fieldName
            || binding_requisites.FieldMapping[fieldName] == "ObjectGuid")
            continue;

          object value = this.field_binder.GetFieldValue(fieldName);

          if (value != null)
          {
            group.Add(new ExpressionFilter(fieldName, FilterOperation.Equals, value));
          }
          else
          {
            group.Add(new NullFilter() { FieldName = fieldName });
          }
        }

        this.record_binder.OpenReader(this._TableName, searchQuery);
        if (this.record_binder.Next())
        {
          object ret = this.record_binder.GetFieldValue(0);
          this.record_binder.CloseReader();
          
          if (this.isDataBound && this.modified)
          {
            IBindable updater = this._Orig;
            updater[updater._KeyFieldName] = ret;
            this.Update();
          }
          return ret;
        }
        else
        {
          this.record_binder.CloseReader();

          // Если объект с текущим идентификатором уже используется,
          // то необходимо создать новый объект.
          // Иначе, можно просто обновить текущий объект
          bool check = RecordCache.CheckReferences(this);
          using (TType clone = this.Clone())
          {
            bool ret = false;
            if (check && this._IsDataBound)
            {
              ret = clone.Update();
            }
            else
            {
              ret = clone.Insert();
            }

            if (ret)
            {
              return clone.GetIdentifier();
            }
          }
        }
      }
      return null;
    }

    public EditableReference<TRef> GetEditableReference<TRef>(string relationField, bool createIfNotExists)
        where TRef : class, IBindable, new()
    {
      if (!this.referencedObjects.ContainsKey(relationField))
      {
        throw new ArgumentException();
      }
      lock (this.editableObjects)
      {
        IEditableReference ret;
        if (!this.editableObjects.TryGetValue(relationField, out ret))
        {
          ret = new EditableReference<TRef>();
          object identifier = this.field_binder.GetFieldValue(relationField);
          if (identifier != null)
          {
            ret.Value = RecordManager.GetItem<TRef>(identifier);
          }
          else if (createIfNotExists)
          {
            ret.Value = new TRef();
          }
          this.editableObjects.Add(relationField, ret);
        }
        if (ret.Value != null)
        {
          ret.Value._CacheStatus = CacheStatus.LocalCache;
        }
        return (EditableReference<TRef>)ret;
      }
    }

    public EditableCollection<TRef> GetEditableCollection<TRef>(string relationField)
      where TRef : class, IBindable, new()
    {
      if (binding_requisites.KeyFieldNames.Length != 1)
      {
        throw new InvalidOperationException();
      }

      lock (this.editableCollections)
      {
        IEditableCollection ret;
        if (!this.editableCollections.TryGetValue(relationField, out ret))
        {
          ret = new EditableCollection<TRef>();
          ret.Binding = true;
          using (TRef prototype = new TRef())
          {
            QueryObject query = new QueryObject();
            query.AddFilterExpression(relationField, this.GetIdentifier());
            prototype.InitReader(query);
            while (prototype.Next())
            {
              TRef clone = (TRef)prototype.Clone();
              clone._CacheStatus = CacheStatus.LocalCache;
              ret.Add(clone);
            }
          }
          ret.Binding = false;

          this.editableCollections.Add(relationField, ret);
        }

        return (EditableCollection<TRef>)ret;
      }
    }

    #endregion

    #region Private methods

    private TFieldType CastFieldValue<TFieldType>(object value)
    {
      if (value != null && typeof(TFieldType).IsGenericType
          && typeof(TFieldType).GetGenericArguments()[0].IsEnum)
      {
        value = Enum.ToObject(typeof(TFieldType).GetGenericArguments()[0], value);
      }
      return (TFieldType)value;
    }

    private Dictionary<string, object> GetFieldValues(string[] fieldNames)
    {
      Dictionary<string, object> ret = new Dictionary<string, object>();
      foreach (string fieldName in fieldNames)
      {
        ret.Add(fieldName, this.field_binder.GetFieldValue(fieldName));
      }
      return ret;
    }

    private void NotifyContext(InfoEventArgs messageBuffer, bool checkCommit)
    {
      if (messageBuffer.Messages.Count > 0 &&
        (!checkCommit || this.uncommitted))
      {
        RecordManager.SendMessageList(messageBuffer);

        if (checkCommit)
          this.uncommitted = false;
      }
    }

    private void EndEditCore()
    {
      InfoEventArgs messageBuffer = new InfoEventArgs();
      if (((IBindable)this).ValidateWrite(messageBuffer))
      {
        try
        {
          this.isDataBound = this.isDataBound ? this.Update() : this.Insert();
        }
        catch (Exception ex)
        {
          Log.Error("EndEditCore(): exception", ex);
          this.CheckFailed(ex, true);
        }
      }
      else
      {
        this.NotifyContext(messageBuffer, true);
      }
    }

    private bool CheckFailed(Exception ex, bool checkCommit)
    {
      if (RecordManager.Contexts.Count > 0)
      {
        if (this.uncommitted || !checkCommit)
        {
          RecordManager.SendErrorMessage(ex);

          if (checkCommit)
            this.uncommitted = false;
        }
        return false;
      }
      else
      {
        if (checkCommit)
          this.uncommitted = false;

        throw ex;
      }
    }

    private Dictionary<string, ReferenceStore> GetReferences()
    {
      Dictionary<string, ReferenceStore> ret = new Dictionary<string, ReferenceStore>();
      foreach (PropertyInfo currentProperty in typeof(TType).GetProperties())
      {
        if (currentProperty.IsDefined(typeof(ReferenceAttribute), false))
        {
          ReferenceAttribute currentReference
              = currentProperty.GetCustomAttributes(typeof(ReferenceAttribute), false)[0]
              as ReferenceAttribute;

          ret.Add(RecordManager.GetFieldName(typeof(TType), currentProperty.Name),
            new ReferenceStore(currentReference.ReferenceType, null));
        }
      }
      return ret;
    }

    private void ResetReferences()
    {
      Dictionary<string, ReferenceStore> cloneDic = new Dictionary<string, ReferenceStore>();
      foreach (KeyValuePair<string, ReferenceStore> kv in this.referencedObjects)
      {
        cloneDic.Add(kv.Key, new ReferenceStore(kv.Value.referenceType, null));
      }
      this.referencedObjects = cloneDic;
    }

    private bool DoValidateField(string fieldName, object oldValue, object newValue)
    {
      if (this.record_binder == null)
      {
        throw new InvalidOperationException(Resources.BindableObject_OrigEdit);
      }
      string propertyName = null;

      InfoEventArgs messageBuffer = new InfoEventArgs();
      if (this.ValidateField(fieldName, newValue, messageBuffer))
      {
        if (!binding_requisites.FieldMapping.TryGetValue(fieldName, out propertyName))
        {
          throw new ArgumentException(string.Format(Resources.BindableObject_PropertyNotFound, fieldName));
        }
        if (isDataBound)
        {
          this.InitOrig();
        }
        if (this.PropertyChanging != null)
        {
          this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
        if (!this.modified)
        {
          BindableAspectManager.BeginEdit(this);
        }
        return true;
      }
      else if (messageBuffer.Messages.Count > 0)
      {
        this.NotifyContext(messageBuffer, false);
      }
      return false;
    }

    private void NotifyChanged(string fieldName, object value)
    {
      this.modified = true;
      this.uncommitted = true;
      string propertyName;

      if (initializing)
        return;

      if (m_server_result != null)
      {
        m_server_result.Fields[fieldName] = value;
      }

      if (!binding_requisites.FieldMapping.TryGetValue(fieldName, out propertyName))
      {
        propertyName = "";
      }

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }

      if (referencedObjects.ContainsKey(fieldName))
      {
        ReferenceStore reference = referencedObjects[fieldName];
        reference.reference = null;
        referencedObjects[fieldName] = reference;
      }

      this.AfterPropertyChanged(propertyName);
    }

    private void InitOrig()
    {
      if (initializing)
        return;

      if (this.orig == null)
      {
        if (this.record_binder == null)
        {
          this.orig = this as TType;
        }
        else
        {
          this.orig = this.MemberwiseClone() as TType;
          this.orig.orig = this.orig;
          this.orig.field_binder = this.field_binder.Clone(this.orig);
          this.orig.record_binder = null;
          this.orig.modified = false;
        }
      }
    }

    private void InvalidateContext()
    {
      RecordCache.InvalidateType<TType>(this as TType);

      if (this.cacheStatus == CacheStatus.LocalCache)
      {
        RecordCache.InvalidateByCache<TType>();
      }
    }

    private void ServerInsert(BindingResult result)
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }

      InfoEventArgs messageBuffer = new InfoEventArgs();
      result.BindMessages(messageBuffer);
      
      this.BeforeServerWrite();

      IBindableEntity entry = this as IBindableEntity;
      Guid oldGuid = Guid.Empty;

 
     if (entry != null)
      {
        if (entry.ObjectGuid == Guid.Empty)
        {
          entry.ObjectGuid = Guid.NewGuid();
        }
        else
        {
          oldGuid = entry.ObjectGuid;
        }
        if (!BindableAspectManager.ValidateInsert(this, messageBuffer))
        {
          result.Success = false;
          return;
        }
      }

     if (!this.disableBLL)
     {
       this.BeforeInsert();
     }

      Dictionary<string, object> parameters
        = this.GetFieldValues(binding_requisites.DataBoundFieldNames);

      string[] outputFieldNames = string.IsNullOrEmpty(binding_requisites.IdentityField) ?
        new string[0] : new string[] { binding_requisites.IdentityField };

      result.Success = record_binder.Insert(this._TableName, parameters, outputFieldNames);

      if (result.Success)
      {
        foreach (string fieldName in outputFieldNames)
        {
          this.field_binder.SetFieldValue(fieldName, parameters[fieldName]);
        }
      }
      else if (entry != null)
      {
        entry.ObjectGuid = oldGuid;
      }
      if (!this.disableBLL)
      {
        this.AfterInsert(result);
      }
      BindableAspectManager.AfterInsert(this, result);
      this.AfterServerWrite(result);

      if (result.Success)
      {
        foreach (string fieldName in binding_requisites.KeyFieldNames)
        {
          result.Fields[fieldName] = this.field_binder.GetFieldValue(fieldName);
        }
      }
    }

    private void ServerUpdate(BindingResult result)
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      InfoEventArgs messageBuffer = new InfoEventArgs();
      result.BindMessages(messageBuffer);

      this.BeforeServerWrite();
      if (!BindableAspectManager.ValidateUpdate(this, messageBuffer))
      {
        result.Success = false;
        return;
      }
      if (!this.disableBLL)
      {
        this.BeforeUpdate();
      }

      if (this._IsDataBound)
      {
        Dictionary<string, object> nonIdentityFieldValues = new Dictionary<string, object>();
        foreach (string currentFieldName in binding_requisites.DataBoundFieldNames)
        {
          if (currentFieldName != binding_requisites.IdentityField)
          {
            nonIdentityFieldValues.Add(currentFieldName,
              this.field_binder.GetFieldValue(currentFieldName));
          }
        }

        result.Success = record_binder.Update(this._TableName
          , nonIdentityFieldValues
          , this._Orig.GetFieldValues(binding_requisites.KeyFieldNames));
      }
      if (!this.disableBLL)
      {
        this.AfterUpdate(result);
      }
      BindableAspectManager.AfterUpdate(this, result);
      this.AfterServerWrite(result);

      if (result.Success)
      {
        foreach (string fieldName in binding_requisites.KeyFieldNames)
        {
          result.Fields[fieldName] = this.field_binder.GetFieldValue(fieldName);
        }
      }
    }

    private void ServerDelete(BindingResult result)
    {
      if (this.aspectWorking)
      {
        throw new InvalidOperationException(Resources.BindableObject_RecursiveAspectCall);
      }
      InfoEventArgs messageBuffer = new InfoEventArgs();
      result.BindMessages(messageBuffer);

      if (!BindableAspectManager.ValidateDelete(this, messageBuffer))
      {
        result.Success = false;
        return;
      }
      if (!this.disableBLL)
      {
        this.BeforeDelete();
      }
      if (this._IsDataBound)
      {
        result.Success = record_binder.Delete(this._TableName,
          this.GetFieldValues(binding_requisites.KeyFieldNames));
      }
      if (result.Success)
      {
        if (!string.IsNullOrEmpty(binding_requisites.IdentityField))
        {
          this.field_binder.SetFieldValue(binding_requisites.IdentityField, 0);
        }
      }
      if (!this.disableBLL)
      {
        this.AfterDelete(result);
      }
      BindableAspectManager.AfterDelete(this, result);
    }

    private void BeforeServerWrite()
    {
      foreach (KeyValuePair<string, IEditableReference> kv in this.editableObjects)
      {
        if (kv.Value.Value != null)
        {
          if (kv.Value.Value._Modified ||
            (this.field_binder.GetFieldValue(kv.Key) != kv.Value.Value.GetIdentifier()
            && kv.Value.Value._IsDataBound))
          {
            ISupportFindOrCreate foc = kv.Value.Value as ISupportFindOrCreate;
            if (foc != null)
            {
              object value = foc.FindOrCreate();
              this.field_binder.SetFieldValue(kv.Key, value);
            }
            else
            {
              if (kv.Value.Value._IsDataBound)
              {
                kv.Value.Value.Update();
              }
              else
              {
                kv.Value.Value.Insert();
              }
              this.field_binder.SetFieldValue(kv.Key, kv.Value.Value.GetIdentifier());
            }

            if (m_server_result != null)
            {
              m_server_result.Fields[kv.Key] = this.field_binder.GetFieldValue(kv.Key);
            }
          }
        }
        else
        {
          this.field_binder.SetFieldValue(kv.Key, null);
        }
      }
    }

    private void AfterServerWrite(BindingResult result)
    {
      if (!result.Success)
        return;

      foreach (KeyValuePair<string,IEditableCollection> coll in this.editableCollections)
      {
        foreach (IBindable item in coll.Value.ItemsToDelete)
        {
          if (item._IsDataBound)
            item.Delete();
        }
        for (int i = 0; i < coll.Value.Count; i++)
        {
          IBindable item = coll.Value[i];
          item[coll.Key] = RecordManager.GetIdentifier(this);
          if (item._IsDataBound)
          {
            result.Success = item.Update() && result.Success;
          }
          else
          {
            result.Success = item.Insert() && result.Success;
          }
        }
      }
    }

    private void AfterWrite(BindingResult result)
    {
      if (this.orig != null)
      {
        this.orig.Dispose();
        this.orig = null;
      }

      this.InvalidateContext();

      foreach (KeyValuePair<string, object> kv in result.Fields)
      {
        this.field_binder.SetFieldValue(kv.Key, kv.Value);
      }

      if (result.RebindRequired)
      {
        this.DataBind();
      }
      this.isDataBound = true;
      this.modified = false;
      this.uncommitted = false;
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(""));
      }
    }

    private IRecordBinder GetRecordBinder()
    {
      return RecordManager.GetRecordBinder(binding_requisites.ProcedureActions, 
        this as ICustomRecordBinderProvider);
    }

    #endregion

    #region Protected virtual methods

    protected abstract string[] GetDataBoundFieldNames();

    protected abstract string[] GetKeyFieldNames();

    protected abstract string GetIdentityField();

    protected abstract Dictionary<BindingAction, string> GetProcedureActions();

    protected virtual string[] GetRequisites()
    {
      return binding_requisites.DataBoundFieldNames;
    }

    protected virtual IFieldBinder GetFieldBinder()
    {
      return new ReflectionFieldBinder<TType>(this as TType, false);
    }

    protected virtual bool ValidateField(string fieldName, object newValue, InfoEventArgs messageBuffer)
    {
      return true;
    }

    protected virtual TFieldType ParseEditValue<TFieldType>(TFieldType value, string fieldName)
    {
      return value;
    }

    protected virtual bool ValidateWrite(InfoEventArgs messageBuffer)
    {
      return true;
    }

    protected virtual bool ValidateDelete(InfoEventArgs messageBuffer)
    {
      return true;
    }

    protected virtual void InitValues() { }

    protected virtual void AfterPropertyChanged(string propertyName) { }

    protected virtual void BeforeDataBind() { }

    protected virtual void AfterDataBind(BindingResult result) { }

    protected virtual void BeforeInsert() { }

    protected virtual void AfterInsert(BindingResult result) { }

    protected virtual void BeforeUpdate() { }

    protected virtual void AfterUpdate(BindingResult result) { }

    protected virtual void BeforeDelete() { }

    protected virtual void AfterDelete(BindingResult result) { }

    #endregion

    #region IServiceRecord Members

    void IServiceRecord.UpdateData(BindingResult result)
    {
      if (result == null)
        return;
      
      m_server_result = result;
      try
      {
        switch (result.BindingAction)
        {
          case BindingAction.Insert:
            this.ServerInsert(result);
            return;
          case BindingAction.Update:
            this.ServerUpdate(result);
            return;
          case BindingAction.Delete:
            this.ServerDelete(result);
            return;
          default: throw new NotImplementedException();
        }
      }
      finally
      {
        m_server_result = null;
        if (result.Success)
        {
          this.InvalidateContext();
        }
      }
    }

    #endregion

    #region IPackable Members

    private Dictionary<string, object> PackToDictionary()
    {
      Dictionary<string, object> pack = this.GetFieldValues(binding_requisites.DataBoundFieldNames);

      BindableState state = BindableState.Clear;

      if (this.isDataBound)
        state = state | BindableState.IsDataBound;

      if (this.modified)
        state = state | BindableState.Modified;

      if (this.pendingBreak)
        state = state | BindableState.PendingBreak;

      if (this.uncommitted)
        state = state | BindableState.Uncommitted;

      if (this.aspectWorking)
        state = state | BindableState.AspectWorking;

      if (this.disableBLL)
        state = state | BindableState.DisableBLL;

      pack["$_state"] = (byte)state;

      foreach (FieldInfo field in typeof(TType).GetFields(BindingFlags.Instance
        | BindingFlags.NonPublic | BindingFlags.Public))
      {
        if (field.IsDefined(typeof(PackableFieldAttribute), false))
        {
          pack["#_" + field.Name] = field.GetValue(this);
        }
      }

      if (this.orig != null && this._IsDataBound)
      {
        foreach (string fieldName in binding_requisites.KeyFieldNames)
        {
          pack["@orig_" + fieldName] = this.orig.field_binder.GetFieldValue(fieldName);
        }
      }

      pack["<<"] = this.editableCollections;
      pack[">>"] = this.editableObjects;

      return pack;
    }

    private void UnpackDictionary(Dictionary<string, object> unpacked)
    {
      BindableState state = (BindableState)unpacked["$_state"];

      this.isDataBound = (state & BindableState.IsDataBound) != BindableState.Clear;
      this.modified = (state & BindableState.Modified) != BindableState.Clear;
      this.pendingBreak = (state & BindableState.PendingBreak) != BindableState.Clear;
      this.uncommitted = (state & BindableState.Uncommitted) != BindableState.Clear;
      this.aspectWorking = (state & BindableState.AspectWorking) != BindableState.Clear;
      this.disableBLL = (state & BindableState.DisableBLL) != BindableState.Clear;

      foreach (string fieldName in binding_requisites.DataBoundFieldNames)
      {
        this.field_binder.SetFieldValue(fieldName, unpacked[fieldName]);
      }
      foreach (FieldInfo field in typeof(TType).GetFields(BindingFlags.Instance
        | BindingFlags.NonPublic | BindingFlags.Public))
      {
        if (field.IsDefined(typeof(PackableFieldAttribute), false))
        {
          field.SetValue(this, unpacked["#_" + field.Name]);
        }
      }
      if (this.isDataBound)
      {
        Dictionary<string, object> origFieldValues = new Dictionary<string, object>();
        foreach (string fieldName in binding_requisites.KeyFieldNames)
        {
          if (unpacked.ContainsKey("@orig_" + fieldName))
          {
            origFieldValues[fieldName] = unpacked["@orig_" + fieldName];
          }
        }
        if (origFieldValues.Count == binding_requisites.KeyFieldNames.Length)
        {
          this.orig = new TType();
          foreach (KeyValuePair<string, object> kv in origFieldValues)
          {
            this.orig.field_binder.SetFieldValue(kv.Key, kv.Value);
          }
        }
      }

      this.editableObjects = (Dictionary<string, IEditableReference>)unpacked[">>"];
      this.editableCollections = (Dictionary<string, IEditableCollection>)unpacked["<<"];
    }

    byte[] IPackable.Pack()
    {
      return PackHelper.PackFieldValues(this.PackToDictionary());
    }

    bool IPackable.Unpack(byte[] data)
    {
      try
      {
        Dictionary<string, object> unpacked = PackHelper.UnpackFieldValues(data);
        this.UnpackDictionary(unpacked);
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("IPackable.Unpack(): exception", ex);
        return false;
      }
    }

    #endregion

    #region ISerializable Members

    protected BindableObject(SerializationInfo info, StreamingContext context)
      : this()
    {
      Dictionary<string, object> values = new Dictionary<string, object>();
      foreach (SerializationEntry entry in info)
      {
        values[entry.Name] = entry.Value;
      }
      this.UnpackDictionary(values);
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      foreach (KeyValuePair<string, object> kv in this.PackToDictionary())
      {
        info.AddValue(kv.Key, kv.Value);
      }
    }

    #endregion
  }
}