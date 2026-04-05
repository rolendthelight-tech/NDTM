using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using AT.Toolbox.MSSQL.Properties;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AT.Toolbox.Log;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public abstract class ViewRecord<TType> : FdTypeDescriptor, IBindable, ISerializable
    where TType : ViewRecord<TType>, new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ViewRecord<TType>).Name);

    #region Fields

    IRecordBinder record_binder;
    IFieldBinder field_binder;
    bool isDataBound;
    bool pendingBreak;
    string[] currentlyBoundFields;
    Dictionary<string, IBindableObjectList> ref_collections = new Dictionary<string, IBindableObjectList>();

    static BindingRequisites binding_requisites;

    #endregion

    #region Constructors

    protected ViewRecord()
    {
      if (binding_requisites == null)
      {
        binding_requisites = new BindingRequisites();

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
      this.record_binder = RecordManager.GetRecordBinder(this.GetProcedureActions(), this as ICustomRecordBinderProvider);
      this.field_binder = this.GetFieldBinder();
    }

    protected ViewRecord(Dictionary<string, object> keyFields)
      : this()
    {
      foreach (string fieldName in binding_requisites.KeyFieldNames)
      {
        this.field_binder.SetFieldValue(fieldName, keyFields[fieldName]);
      }
      this.DataBind();
    }

    #endregion

    private Dictionary<string, object> GetFieldValues(string[] fieldNames)
    {
      Dictionary<string, object> ret = new Dictionary<string, object>();
      foreach (string fieldName in fieldNames)
      {
        ret.Add(fieldName, this.field_binder.GetFieldValue(fieldName));
      }
      return ret;
    }

    #region IBindable Members

    public bool DataBind()
    {
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
      this.AfterDataBind(result);
      record_binder.CloseReader();
      this.isDataBound = result.Success;
      return isDataBound;
    }

    public bool Insert()
    {
      return false;
    }

    public bool Update()
    {
      return false;
    }

    public bool Delete()
    {
      return false;
    }

    public bool ValidateWrite(InfoEventArgs messageBuffer)
    {
      return false;
    }

    public bool ValidateDelete(InfoEventArgs messageBuffer)
    {
      return false;
    }

    public bool InitReader(QueryObject query)
    {
      if (this._CacheStatus != CacheStatus.Free)
      {
        throw new InvalidOperationException(Resources.BindableObject_CacheCorruption);
      }
      this.currentlyBoundFields = this.record_binder.OpenReader(this._TableName, query);
      this.pendingBreak = false;
      return true;
    }

    public bool Next()
    {
      if (this._CacheStatus != CacheStatus.Free)
      {
        throw new InvalidOperationException(Resources.BindableObject_CacheCorruption);
      }
      if (this.pendingBreak)
      {
        this.pendingBreak = false;
        return false;
      }

      bool ret = record_binder.Next();
      if (ret)
      {
        this.field_binder.ReadDataFields(this.currentlyBoundFields, this.record_binder);
      }
      else
      {
        record_binder.CloseReader();
      }
      this.isDataBound = ret;
      return ret;
    }

    public void Break()
    {
      pendingBreak = true;
      this.record_binder.CloseReader();
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
        if (callerType.Assembly != typeof(ViewRecord<>).Assembly)
        {
          throw new InvalidOperationException(Resources.BindableObject_DirectFieldAccess);
        }

        this.field_binder.SetFieldValue(fieldName, value);
      }
    }

    void IBindable.InitValues() { }

    [Browsable(false)]
    public bool _IsDataBound
    {
      get { return isDataBound; }
    }

    [Browsable(false)]
    public bool _Modified
    {
      get { return false; }
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

    [Browsable(false)]
    public virtual string _TableName
    {
      get { return binding_requisites.TableName; }
    }

    [Browsable(false)]
    public CacheStatus _CacheStatus { get; set; }

    #endregion

    #region ICloneable Members

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    public virtual TType Clone()
    {
      TType ret = this.MemberwiseClone() as TType;
      ret.record_binder = RecordManager.GetRecordBinder(ret.GetProcedureActions(), ret as ICustomRecordBinderProvider);
      ret.field_binder = this.field_binder.Clone(ret);
      ret.ref_collections = new Dictionary<string, IBindableObjectList>();
      ret._CacheStatus = CacheStatus.Free;
      
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

      return ret;
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      this.record_binder.Dispose();
    }

    #endregion

    #region BLL Utils

    protected BindableObjectList<TRef> GetReferenceCollection<TRef>(string name, QueryObject query)
      where TRef : class, IBindable, new()
    {
      if (!this.isDataBound)
      {
        return new BindableObjectList<TRef>();
      }

      BindableObjectList<TRef> ret = null;
      if (!this.ref_collections.ContainsKey(name))
      {
        ret = new BindableObjectList<TRef>();
        ret.IsInLocalCache = true;
        ret.DataBind(query);
        this.ref_collections.Add(name, ret);
      }
      else
      {
        ret = this.ref_collections[name] as BindableObjectList<TRef>;
        if (!RecordCache.ValidateType<TRef>(this) && 
            !RecordManager.CheckActuality<TRef>())
        {
          ret.DataBind(query);
        }
      }
      return ret;
    }

    #endregion

    #region overridable

    protected abstract string[] GetDataBoundFieldNames();

    protected abstract string[] GetKeyFieldNames();

    protected abstract Dictionary<BindingAction, string> GetProcedureActions();

    protected abstract IFieldBinder GetFieldBinder();

    protected TFieldType GetFieldValue<TFieldType>(string fieldName)
    {
      return this.CastFieldValue<TFieldType>(this.field_binder.GetFieldValue(fieldName));
    }

    protected virtual void AfterDataBind(BindingResult result) { }

    private TFieldType CastFieldValue<TFieldType>(object value)
    {
      if (value != null && typeof(TFieldType).IsGenericType
          && typeof(TFieldType).GetGenericArguments()[0].IsEnum)
      {
        value = Enum.ToObject(typeof(TFieldType).GetGenericArguments()[0], value);
      }
      return (TFieldType)value;
    }

    #endregion

    #region Requisites

    private class BindingRequisites
    {
      public string[] DataBoundFieldNames;
      public string[] KeyFieldNames;
      public string TableName;
      public Dictionary<string, string> FieldMapping;
      public Dictionary<BindingAction, string> ProcedureActions;
    }

    #endregion

    #region ISerializable Members

    protected ViewRecord(SerializationInfo info, StreamingContext context)
      : this()
    {
      Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
      
      foreach (FieldInfo field in typeof(TType).GetFields(BindingFlags.Instance
        | BindingFlags.NonPublic | BindingFlags.Public))
      {
        if (field.IsDefined(typeof(PackableFieldAttribute), false)
          && field.FieldType.IsDefined(typeof(SerializableAttribute), true))
        {
          fields.Add("#_" + field.Name, field);
        }
      }
      foreach (SerializationEntry item in info)
      {
        if (item.Name.StartsWith("<<") && item.Value is Package)
        {
          Type itemType = PackHelper.GetTypeByGuid((item.Value as Package).TypeGuid);
          if (itemType == null)
            continue;

          IBindableObjectList list = RecordManager.GetTypedList(itemType, false, null);

          using (MemoryStream ms = new MemoryStream((item.Value as Package).Data))
          {
            BinaryFormatter bf = new BinaryFormatter();
            Array array = (Array)bf.Deserialize(ms);

            if (array == null)
              continue;

            for (int i = 0; i < array.Length; i++)
            {
              list.Add(array.GetValue(i));
            }
            this.ref_collections[item.Name.Substring(2)] = list;
          }
        } 
        else if (item.Name == "$_isDnd")
        {
          this.isDataBound = (bool)item.Value;
        }
        else if (fields.ContainsKey(item.Name))
        {
          fields[item.Name].SetValue(this, item.Value);
        }
        else if (binding_requisites.DataBoundFieldNames.Contains(item.Name))
        {
          this.field_binder.SetFieldValue(item.Name, item.Value);
        }
      }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      foreach (string fieldName in binding_requisites.DataBoundFieldNames)
      {
        info.AddValue(fieldName, field_binder.GetFieldValue(fieldName));
      }
      info.AddValue("$_isDnd", this.isDataBound);

      foreach (FieldInfo field in typeof(TType).GetFields(BindingFlags.Instance
        | BindingFlags.NonPublic | BindingFlags.Public))
      {
        if (field.IsDefined(typeof(PackableFieldAttribute), false)
          && field.FieldType.IsDefined(typeof(SerializableAttribute), true))
        {
          info.AddValue("#_" + field.Name, field.GetValue(this));
        }
      }
      foreach (KeyValuePair<string, IBindableObjectList> kv in this.ref_collections)
      {
        Type itemType = kv.Value.GetType().GetGenericArguments()[0];
        if (!itemType.IsDefined(typeof(SerializableAttribute), false))
          continue;

        IDictionary dic = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(int), itemType));

        // Упорно не хочет сериализовывать массивы через SerializationInfo,
        // приходится через пакет.
        Package pack = new Package();
        pack.TypeGuid = itemType.GUID;
        Array arr = (Array)Activator.CreateInstance(itemType.MakeArrayType(),
          kv.Value.Count);

        for (int i = 0; i < kv.Value.Count; i++)
        {
          arr.SetValue(kv.Value[i], i);
        }
        using (MemoryStream ms = new MemoryStream())
        {
          BinaryFormatter bf = new BinaryFormatter();
          bf.Serialize(ms, arr);
          pack.Data = ms.ToArray();
        }
        info.AddValue("<<" + kv.Key, pack);
      }
    }

    #endregion
  }
}
