using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using AT.Toolbox.MSSQL.Properties;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Filtering;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  public class BindableObjectList<TType> : IBindableObjectList, IList<TType>, ICancelAddNew, IRaiseItemChangedEvents, IListServer
      where TType : class, IBindable, new()
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BindableObjectList<TType>).Name);

    #region Fields

    readonly List<IDObjectStore> items = new List<IDObjectStore>();
    readonly MaxAccessLevel maxAccessLevel = MaxAccessLevel.Delete;
    readonly PropertyDescriptorCollection itemProperties = TypeDescriptor.GetProperties(typeof(TType));
    readonly List<IBindableObjectList> children = new List<IBindableObjectList>();
    readonly List<string> sorting = new List<string>();
    readonly object locker = new object();   // Все операции по привязке данных должны быть синхронизированы

    private string m_key_field_name;
    public string KeyFieldName
    {
      get
      {
        if (string.IsNullOrEmpty(m_table_name))
          GetTableAndKeyFieldName();

        return m_key_field_name;
      }
    }

    private string m_table_name;
    public string TableName
    {
      get
      {
        if (string.IsNullOrEmpty(m_table_name))
          GetTableAndKeyFieldName();

        return m_table_name;
      }
    }

    void GetTableAndKeyFieldName()
    {
      using (TType keyFinder = new TType())
      {
        m_key_field_name = keyFinder._KeyFieldName;
        m_table_name = keyFinder._TableName;
      }
    }

    PropertyDescriptor sortProperty;
    QueryObject currentQuery = new QueryObject();
    IBindableObjectList parentList = null;
    private CriteriaOperator filter_criteria;
    int addNewPos;
    int currentObjectIdx = -1;
    volatile bool bindingInProcess = false;

    // Поля, используемые для поддержки ServerMode
    bool m_firstFilter = true;
    ListSortDescriptionCollection m_sortInfo;
    Dictionary<ListSourceGroupInfo, ListSourceGroupInfo> m_group_tree = new Dictionary<ListSourceGroupInfo, ListSourceGroupInfo>();
    Dictionary<object, object> totalSummary = new Dictionary<object, object>();

    #endregion

    #region Constructors

    public BindableObjectList()
    {
      if (typeof(TType).IsDefined(typeof(MaxAccessLevelAttribute), true))
      {
        maxAccessLevel = (typeof(TType).GetCustomAttributes(typeof(MaxAccessLevelAttribute), true)[0]
          as MaxAccessLevelAttribute).MaxAccessLevel;
      }
      this.Append = true;
    }

    #endregion

    #region Events

    public event AddingNewEventHandler AddingNew;
    public event ListChangedEventHandler ListChanged;

    #endregion

    #region Access properties

    public int Count
    {
      get { return this.items.Count; }
    }

    public bool AllowEdit
    {
      get { return maxAccessLevel >= MaxAccessLevel.Update; }
    }

    public bool AllowNew
    {
      get { return maxAccessLevel >= MaxAccessLevel.Insert; }
    }

    public bool AllowRemove
    {
      get { return maxAccessLevel >= MaxAccessLevel.Delete; }
    }

    public bool IsReadOnly
    {
      get { return this.maxAccessLevel < MaxAccessLevel.Update; }
    }

    bool IList.IsFixedSize
    {
      get { return this.maxAccessLevel <= MaxAccessLevel.Update; }
    }

    bool IBindingList.SupportsChangeNotification
    {
      get { return true; }
    }

    bool IBindingList.SupportsSearching
    {
      get { return false; }
    }

    bool IBindingList.SupportsSorting
    {
      get { return true; }
    }

    bool IRaiseItemChangedEvents.RaisesItemChangedEvents
    {
      get { return true; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)items).SyncRoot; }
    }

    #endregion

    #region Index methods

    void IBindingList.AddIndex(PropertyDescriptor property)
    {
    }

    void IBindingList.RemoveIndex(PropertyDescriptor property)
    {
    }

    #endregion

    #region AddNew methods

    object IBindingList.AddNew()
    {
      return this.AddNew();
    }

    private TType FireAddingNew()
    {
      AddingNewEventArgs e = new AddingNewEventArgs(null);
      if (this.AddingNew != null)
      {
        this.AddingNew(this, e);
      }
      return e.NewObject as TType;
    }

    public TType AddNew()
    {
      lock (locker)
      {
        TType ret = this.FireAddingNew();
        if (ret == null)
        {
          ret = new TType();
        }
        this.addNewPos = this.Append ? this.items.Count : 0;
        this.currentObjectIdx = this.addNewPos;
        this.InsertItem(this.addNewPos, ret);
        return ret;
      }
    }

    public void CancelNew(int itemIndex)
    {
      lock (locker)
      {
        if ((this.addNewPos >= 0) && (this.addNewPos == itemIndex))
        {
          this.RemoveItem(this.addNewPos);
          this.addNewPos = -1;
        }
      }
    }

    public void EndNew(int itemIndex)
    {
      lock (locker)
      {
        if ((this.addNewPos >= 0) && (this.addNewPos == itemIndex))
        {
          this.addNewPos = -1;
        }
      }
    }

    #endregion

    #region Sort members

    void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
    {
      if (property != null)
      {
        if (!sorting.Contains(property.Name))
        {
          sorting.Add(property.Name);
          direction = ListSortDirection.Ascending;
        }
        else
        {
          sorting.Remove(property.Name);
          direction = ListSortDirection.Descending;
        }

        this.ApplySort(new ListSortDescriptionCollection(new ListSortDescription[] { new ListSortDescription(property, direction) })
          , 0, new List<ListSourceSummaryItem>(), new List<ListSourceSummaryItem>());
      }
      sortProperty = property;
    }

    void IBindingList.RemoveSort()
    {
      sortProperty = null;
      sorting.Clear();
    }

    bool IBindingList.IsSorted
    {
      get { return sortProperty != null; }
    }

    ListSortDirection IBindingList.SortDirection
    {
      get
      {
        if (currentQuery.Ordering.Count == 1)
        {
          IEnumerator<KeyValuePair<string, ListSortDirection>> en = currentQuery.Ordering.GetEnumerator();
          en.MoveNext();
          return en.Current.Value;
        }
        return ListSortDirection.Ascending;
      }
    }

    PropertyDescriptor IBindingList.SortProperty
    {
      get { return sortProperty; }
    }

    #endregion

    #region IBindingList Members

    public int Find(PropertyDescriptor property, object key)
    {
      return -1;
    }

    private void OnListChanged(ListChangedEventArgs e)
    {
      if (this.ListChanged != null)
      {
        this.ListChanged(this, e);
      }
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      TType item;
      try
      {
        item = (TType)sender;
      }
      catch (Exception ex)
      {
        Log.Error("OnItemPropertyChanged(): exception", ex);
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        return;
      }
      INotifyPropertyChanged pc = item as INotifyPropertyChanged;
      if (pc != null)
      {
        int itemId = this.currentObjectIdx;
        if (itemId >= 0 && this.items[itemId].Value != item)
        {
          itemId = this.IndexOf(item);
        }
        if (itemId >= 0)
        {
          this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged,
            itemId, this.itemProperties.Find(e.PropertyName, true)));
        }
      }
    }

    #endregion

    #region IList Members

    int IList.Add(object value)
    {
      this.Add(value as TType);
      return this.items.Count - 1;
    }

    public void Add(TType item)
    {
      lock (locker)
      {
        this.InsertItem(this.items.Count, item);
      }
    }

    private void InsertItem(int index, TType item)
    {
      if (item != null)
      {
        this.PrepareItemForInsert(item);
        this.items.Insert(index, new IDObjectStore(item.GetIdentifier(), item));
      }
      else
      {
        this.items.Insert(index, new IDObjectStore(null, null));
      }
      this.CurrentObjectIdx = index;
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
    }

    public void Clear()
    {
      lock (locker)
      {
        this.ClearItems();
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
      }
    }

    private void ClearItems()
    {
      for (int i = 0; i < this.items.Count; i++)
      {
        this.PrepareItemForRemove(this.items[i].Value, false);
      }
      this.items.Clear();
      this.currentObjectIdx = -1;
    }

    bool IList.Contains(object value)
    {
      lock (locker)
      {
        return this.IndexOf(value as TType) >= 0;
      }
    }

    public bool Contains(TType item)
    {
      lock (locker)
      {
        return this.IndexOf(item) >= 0;
      }
    }

    int IList.IndexOf(object value)
    {
      lock (locker)
      {
        return this.IndexOf(value as TType);
      }
    }

    public int IndexOf(TType item)
    {
      if (item == null || this.items.Count == 0)
        return -1;

      if (this.currentObjectIdx >= 0 && this.items[this.currentObjectIdx].Value == item)
        return this.currentObjectIdx;

      if (addNewPos >= 0 && this.items[this.addNewPos].Value == item)
        return this.addNewPos;

      for (int i = 0; i < this.items.Count; i++)
      {
        if (this.items[i].Value == item)
          return i;
      }
      return -1;
    }

    void IList.Insert(int index, object value)
    {
      lock (locker)
      {
        this.InsertItem(index, value as TType);
      }
    }

    public void Insert(int index, TType item)
    {
      lock (locker)
      {
        this.InsertItem(index, item);
      }
    }

    private bool RemoveItem(int index)
    {
      if (index >= 0 && index < this.items.Count && this.maxAccessLevel >= MaxAccessLevel.Delete)
      {
        bool ret = this.PrepareItemForRemove(this[index], true);
        if (ret)
        {
          this.items.RemoveAt(index);
          this.currentObjectIdx = -1;
          this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }
        return ret;
      }
      return false;
    }

    public bool Remove(TType item)
    {
      lock (locker)
      {
        return this.RemoveItem(this.IndexOf(item));
      }
    }

    void IList.Remove(object value)
    {
      lock (locker)
      {
        this.RemoveItem(this.IndexOf(value as TType));
      }
    }

    public void RemoveAt(int index)
    {
      lock (locker)
      {
        this.RemoveItem(index);
      }
    }

    object IList.this[int index]
    {
      get { return this[index]; }
      set { this[index] = value as TType; }
    }

    public TType this[int index]
    {
      get
      {
        const int pageSize = 20;
        IDObjectStore store = this.items[index];
        if (store.Value == null && store.ID != null)
        {
          int start = index >= pageSize ? (index - pageSize) : 0;
          int end = (index + pageSize) < this.items.Count ? index + pageSize : this.items.Count;
          Dictionary<object, int> idxs = new Dictionary<object, int>();
          bool pageRequired = false;

          for (int i = start; i < end; i++)
          {
            IDObjectStore cur = this.items[i];

            if (cur.Value == null && cur.ID != null && !RecordCache.Contains<TType>(cur.ID))
            {
              idxs[cur.ID] = i;

              if (i == index)
                pageRequired = true;
            }
          }

          if (pageRequired)
          {
            QueryObject qo = new QueryObject();
            InFilter flt = new InFilter() { FieldName = this.KeyFieldName };
            qo.RootFilter = flt;

            foreach (object key in idxs.Keys)
            {
              flt.Values.Add(key);
            }

            using (TType prototype = new TType())
            {
              prototype.InitReader(qo);

              while (prototype.Next())
              {
                TType clone = (TType)prototype.Clone();
                RecordCache.SetItem<TType>(clone, false);
                int idx;

                if (idxs.TryGetValue(prototype.GetIdentifier(), out idx))
                {
                  IDObjectStore cur = this.items[idx];
                  cur.Value = clone;
                  this.PrepareItemForInsert(clone);
                  this.items[idx] = cur;
                }
              }
            }
          }

          store.Value = RecordCache.GetItem<TType>(store.ID);
          this.PrepareItemForInsert(store.Value);
          this.items[index] = store;
        }
        return store.Value;
      }
      set
      {
        if (this.items[index].Value == value)
          return;

        IDObjectStore store = this.items[index];
        if (store.Value != null)
        {
          this.PrepareItemForRemove(store.Value, false);
        }
        if (value != null)
        {
          this.PrepareItemForInsert(value);
          this.items[index] = new IDObjectStore(value.GetIdentifier(), value);
        }
        else
        {
          this.items[index] = new IDObjectStore(null, null);
        }
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
      }
    }

    #endregion

    #region Collection Members

    public void CopyTo(TType[] array, int index)
    {
      this.items.ConvertAll<TType>((p) => p.Value).CopyTo(array, index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
      {
        throw new ArgumentNullException("array");
      }
      if (array.Rank > 1)
      {
        throw new ArgumentException();
      }
      TType[] localArray = array as TType[];
      if (localArray != null)
      {
        this.CopyTo(localArray, index);
      }
    }

    public IEnumerator<TType> GetEnumerator()
    {
      for (int i = 0; i < this.items.Count; i++)
      {
        yield return this[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < this.items.Count; i++)
      {
        yield return this[i];
      }
    }

    #endregion

    #region IBindableObjectList Members

    IEnumerable IBindableObjectList.GetIdsFilter()
    {
      if (string.IsNullOrEmpty(this.KeyFieldName))
        throw new InvalidOperationException("Table " + this.TableName + " does not have single identifier field");

      return this.items.Select(i => i.ID);
    }

    public FilterBase StaticFilter { get; set; }
    
    public bool Append { get; set; }

    public List<IBindableObjectList> Children
    {
      get { return children; }
    }

    public IBindableObjectList ParentList
    {
      get { return parentList; }
      set
      {
        if (value == this.parentList)
        {
          return;
        }
        else
        {
          lock (this.locker) // Когда происходит действие по обработке данных, менять родителя нельзя
          {
            if (this.parentList == value)
            {
              return;
            }
            if (this.parentList != null)
            {
              this.parentList.Children.Remove(this);
            }
            if (value != null)
            {
              value.Children.Add(this);
            }
            parentList = value;
            this.RelationField = null;
          }
        }
      }
    }

    public int CurrentObjectIdx
    {
      get { return currentObjectIdx; }
      set
      {
        if (value != currentObjectIdx)
        {
          int valueToSet = (value >= 0 && value < this.Count) ? value : -1;
          if (currentObjectIdx >= 0 && this[currentObjectIdx]._Modified)
          {
            this.Write();
          }

          currentObjectIdx = valueToSet;

          if (!bindingInProcess)
          {
            if (currentObjectIdx >= 0)
            {
              foreach (IBindableObjectList list in this.children)
              {
                list.DataBind();
              }
            }
            else
            {
              foreach (IBindableObjectList list in this.children)
              {
                list.Clear();
              }
            }
          }
        }
      }
    }

    IBindable IBindableObjectList.CurrentObject
    {
      get { return this.CurrentObject; }
    }

    public TType CurrentObject
    {
      get { return currentObjectIdx >= 0 ? this[currentObjectIdx] : null; }
    }

    internal bool IsInLocalCache { get; set; }

    public string RelationField { get; set; }

    public event ExceptionEventHandler ErrorMessage;

    public bool DataBind()
    {
      if (this.parentList == null || this.parentList.CurrentObject == null)
      {
        return this.DataBind(this.currentQuery);
      }
      else
      {
        return this.BindFromParent(this.parentList.CurrentObject.GetIdentifier());
      }
    }

    public bool DataBind(QueryObject query)
    {
      if (maxAccessLevel < MaxAccessLevel.Read)
      {
        return false;
      }
      if (query == null)
      {
        throw new ArgumentNullException("query");
      }
      this.currentQuery = query;

      if (!string.IsNullOrEmpty(this.KeyFieldName)
        && query.Aggregation.Count == 0)
      {
        query.Aggregation.Add(this.KeyFieldName, AggregateOperation.None);
        try
        {
          return this.LoadIdentifiers(query);
        }
        finally
        {
          query.Aggregation.Clear();
        }
      }
      else
      {
        using (TType prototype = new TType())
        {
          lock (this.locker)
          {
            if (!this.PerformUnsafeOperation(delegate { return prototype.InitReader(query); }))
            {
              return false;
            }

            return this.PerformBinding(prototype);
          }
        }
      }
    }

    private bool LoadIdentifiers(QueryObject query)
    {
      if (!RecordManager.CheckActuality<TType>())
      {
        RecordCache.InvalidateType<TType>(null);
      }

      TType prototype = new TType();
      IRecordBinder bdr
          = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>(),
          prototype as ICustomRecordBinderProvider);
      bindingInProcess = true;
      try
      {
        this.ClearItems();
        bdr.OpenReader(prototype._TableName, query);
        while (bdr.Next())
        {
          this.items.Add(new IDObjectStore(bdr.GetFieldValue(0), null));
        }

        if (this.Count > 0)
        {
          // необходимо сменить позицию, чтобы заполнить дочерние источники данных
          this.currentObjectIdx = -1;
          this.CurrentObjectIdx = 0;
          return true;
        }
        else
        {
          this.CurrentObjectIdx = -1;
          return false;
        }
      }
      finally
      {
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        bindingInProcess = false;
        prototype.Dispose();
        bdr.Dispose();
      }
    }

    private bool PerformBinding(TType prototype)
    {
      if (!RecordManager.CheckActuality<TType>())
      {
        RecordCache.InvalidateType<TType>(null);
      }
      bindingInProcess = true;
      try
      {
        this.ClearItems();

        while (prototype.Next())
        {
          TType item = (TType)prototype.Clone();
          this.PrepareItemForInsert(item);
          this.items.Add(new IDObjectStore(prototype.GetIdentifier(), item));
        }
      }
      finally
      {
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        bindingInProcess = false;
      }

      if (this.Count > 0)
      {
        // необходимо сменить позицию, чтобы заполнить дочерние источники данных
        this.currentObjectIdx = -1;
        this.CurrentObjectIdx = 0;
        return true;
      }
      else
      {
        this.CurrentObjectIdx = -1;
        return false;
      }
    }

    public bool Write()
    {
      if (this.currentObjectIdx >= 0)
      {
        return this.PerformUnsafeOperation(delegate
        {
          TType item = this[currentObjectIdx];
          if (item._IsDataBound)
          {
            item.Update();
          }
          else
          {
            item.Insert();
          }
          return !item._Modified;
        });
      }
      return false;
    }

    /// <summary>
    /// Метод предназначен для множественного обновления записей в базе данных за один запрос
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <param name="parameters">Значения обновляемых плоей</param>
    /// <returns></returns>
    public bool UpdateRecordSet(QueryObject query, Dictionary<string, object> parameters)
    {
      using (IRecordBinder binder = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>()))
      {
        return this.PerformUnsafeOperation(delegate
        {
          if (binder.UpdateRecordset(RecordManager.GetTableName<TType>(), query, parameters))
          {
            RecordCache.InvalidateType<TType>(null);
            return true;
          }
          return false;
        });
      }
    }

    /// <summary>
    /// Метод предназначен для множественного удаления записей из базы данных за один запрос
    /// </summary>
    /// <param name="query">Запрос. Внимание: не все возможности запроса могут поддерживаться!</param>
    /// <returns></returns>
    public bool DeleteRecordSet(QueryObject query)
    {
      using (IRecordBinder binder = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>()))
      {
        return this.PerformUnsafeOperation(delegate
        {
          if (binder.DeleteRecordSet(RecordManager.GetTableName<TType>(), query))
          {
            RecordCache.InvalidateType<TType>(null);
            return true;
          }
          return false;
        });
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      for (int i = 0; i < this.items.Count; i++)
      {
        this.PrepareItemForRemove(this.items[i].Value, false);
      }
      this.items.Clear();
    }

    #endregion

    #region Binding implementation

    private void PrepareItemForInsert(TType item)
    {
      INotifyPropertyChanged pc = item as INotifyPropertyChanged;
      if (pc != null)
      {
        pc.PropertyChanged += this.OnItemPropertyChanged;
      }
      item._CacheStatus = this.IsInLocalCache ? CacheStatus.LocalCache : item._CacheStatus;

      if (this.parentList != null && !bindingInProcess)
      {
        this.InitRelationField();
        object parm = null;
        IBindable parentObject = this.parentList.CurrentObject;
        if (parentObject != null)
        {
          parm = parentObject.GetIdentifier();
        }
        item[this.RelationField] = parm;
      }
    }

    private bool PrepareItemForRemove(TType item, bool deleting)
    {
      INotifyPropertyChanged pc = item as INotifyPropertyChanged;
      if (pc != null)
      {
        pc.PropertyChanged -= this.OnItemPropertyChanged;
      }
      if (item != null)
      {
        if (deleting)
        {
          bool canRemove = true;
          InfoEventArgs messageBuffer = new InfoEventArgs();

          if (item._IsDataBound
            && (canRemove = this.PerformUnsafeOperation(delegate
            {
              return item.ValidateDelete(messageBuffer);
            })))
          {
            this.PerformUnsafeOperation(item.Delete);
          }
          else if (messageBuffer.Messages.Count > 0)
          {
            if (RecordManager.Contexts.Count > 0)
            {
              RecordManager.SendMessageList(messageBuffer);
            }
            else if (this.ErrorMessage != null)
            {
              this.ErrorMessage(item,
                new ExceptionEventArgs(string.Join(Environment.NewLine,
                  messageBuffer.Messages.ConvertAll<string>(delegate(Info info)
                  {
                    return info.Message;
                  }).ToArray())));
            }
          }

          if (canRemove)
          {
            foreach (IBindableObjectList list in this.children)
            {
              list.Clear();
            }
            item.Dispose();
          }
          return canRemove;
        }
      }
      return true;
    }

    private bool PerformUnsafeOperation(Predicate<List<Info>> operation, List<Info> messageBuffer)
    {
      try
      {
        return operation(messageBuffer);
      }
      catch (Exception ex)
      {
        Log.Error("PerformUnsafeOperation(operation, messageBuffer): exception", ex);

        if (RecordManager.Contexts.Count > 0)
        {
          RecordManager.SendErrorMessage(ex);
        }
        else if (this.ErrorMessage != null)
        {
          this.ErrorMessage(this, new ExceptionEventArgs(ex));
        }
        else
        {
          throw;
        }
        return false;
      }
    }

    private bool PerformUnsafeOperation(Func<bool> operation)
    {
      try
      {
        return operation();
      }
      catch (Exception ex)
      {
        Log.Error("PerformUnsafeOperation(operation): exception", ex);

        if (RecordManager.Contexts.Count > 0)
        {
          RecordManager.SendErrorMessage(ex);
        }
        else if (this.ErrorMessage != null)
        {
          this.ErrorMessage(this, new ExceptionEventArgs(ex));
        }
        else
        {
          throw;
        }
        return false;
      }
    }

    private bool BindFromParent(object parentID)
    {
      if (this.parentList == null)
      {
        throw new InvalidOperationException(Resources.BindableObjectList_NullParent);
      }

      this.InitRelationField();

      this.currentQuery.AddFilterExpression(this.RelationField, parentID);

      return this.DataBind(this.currentQuery);
    }

    private void InitRelationField()
    {
      if (string.IsNullOrEmpty(this.RelationField))
      {
        foreach (PropertyInfo currentProperty in typeof(TType).GetProperties())
        {
          if (currentProperty.IsDefined(typeof(ReferenceAttribute), true))
          {
            ReferenceAttribute reference
              = currentProperty.GetCustomAttributes(typeof(ReferenceAttribute), true)[0]
              as ReferenceAttribute;
            this.RelationField = RecordManager.GetFieldName(typeof(TType), currentProperty.Name);
            break;
          }
        }
      }
    }

    #endregion

    #region Nested types

    private struct IDObjectStore
    {
      public object ID;
      public TType Value;

      public IDObjectStore(object id, TType value)
      {
        this.ID = id;
        this.Value = value;
      }
    }

    #endregion

    #region IListServer Members

    /// <summary>
    /// Производит сортировку в серверном режиме XtraGrid
    /// </summary>
    /// <param name="sortInfo">Набор полей для сортировки. 
    /// Сначала идёт сортировка для получения групп, потом сортировка внутри групп</param>
    /// <param name="groupCount">Глубина вложенности групп. Этот параметр используется для
    /// того, чтобы сортировка по дополнительным полям не затёрла группировку</param>
    /// <param name="summaryInfo">Поля, используемые для суммирования про группам</param>
    /// <param name="totalSummaryInfo">Поля, используемые для суммирования по всей таблице</param>
    public void ApplySort(ListSortDescriptionCollection sortInfo, int groupCount, List<ListSourceSummaryItem> summaryInfo, List<ListSourceSummaryItem> totalSummaryInfo)
    {
      this.m_sortInfo = sortInfo;

      if (sortInfo != null)
      {
        this.currentQuery.Ordering.Clear();
        this.currentQuery.Aggregation.Clear();
        int counter = 0;
        foreach (ListSortDescription lsd in sortInfo)
        {
          try
          {
            this.currentQuery.Ordering.Add(RecordManager.GetFieldName(typeof(TType), lsd.PropertyDescriptor.Name), lsd.SortDirection);
          }
          catch (Exception ex)
          {
            Log.Error("ApplySort(): exception", ex);
            RecordManager.ReportError(ex);
          }
          counter++;
        }
        int oldObjectIdx = currentObjectIdx;
        this.DataBind();
        this.CurrentObjectIdx = oldObjectIdx;
      }

      if (totalSummaryInfo != null && totalSummaryInfo.Count > 0)
      {
        using (TType item = new TType())
        {
          totalSummary.Clear();
          QueryObject qo = this.currentQuery != null ? this.currentQuery.Clone() : new QueryObject();

          qo.Aggregation.Clear();
          qo.Ordering.Clear();
          foreach (ListSourceSummaryItem summaryItem in totalSummaryInfo)
          {
            AggregateOperation operation = CastAggregation(summaryItem);

            if (operation == AggregateOperation.None)
              continue;

						qo.Aggregation.Add(RecordManager.GetFieldName(typeof(TType), summaryItem.Info.Name), operation);
          }

          try
          {
            using (IRecordBinder bdr = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>(),
              item as ICustomRecordBinderProvider))
            {
              string[] fieldNames = bdr.OpenReader(item._TableName, qo);

              if (bdr.Next())
              {
                foreach (ListSourceSummaryItem summaryItem in totalSummaryInfo)
                {
									string fieldName = RecordManager.GetFieldName(typeof(TType), summaryItem.Info.Name);

                  for (int i = 0; i < fieldNames.Length; i++)
                  {
                    if (fieldNames[i] == fieldName)
                    {
                      totalSummary[summaryItem.Key] = bdr.GetFieldValue(i);
                      break;
                    }
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            Log.Error("ApplySort(): exception 2", ex);
          }
        }
      }
    }

    private static AggregateOperation CastAggregation(ListSourceSummaryItem summaryItem)
    {
      AggregateOperation operation = AggregateOperation.None;

      switch (summaryItem.SummaryType)
      {
        case SummaryItemType.Count:
          operation = AggregateOperation.Count;
          break;

        case SummaryItemType.Average:
          operation = AggregateOperation.Avg;
          break;

        case SummaryItemType.Max:
          operation = AggregateOperation.Max;
          break;

        case SummaryItemType.Min:
          operation = AggregateOperation.Min;
          break;

        case SummaryItemType.Sum:
          operation = AggregateOperation.Sum;
          break;
      }
      return operation;
    }

    public int FindIncremental( PropertyDescriptor column, string value, int startIndex, bool searchUp ) { throw new NotImplementedException( ); }

    /// <summary>
    /// Фильтр на XtraGrid, применяемый к коллекции
    /// </summary>
    public CriteriaOperator FilterCriteria
    {
      get { return filter_criteria; }
      set
      {
        if (bindingInProcess)
          return;

        filter_criteria = value;
        if (!ReferenceEquals(value, null))
        {
          m_firstFilter = false;
          FilterConverter<TType> converter = new FilterConverter<TType>();
          filter_criteria.Accept(converter);
          QueryObject query = converter.GetQuery();
          if (this.StaticFilter != null)
          {
            query.AddAnyFilter(this.StaticFilter, true);
          }
          try
          {
            query.AddFilterExpression(this.RelationField, this.parentList.CurrentObject.GetIdentifier());
          }
          catch (Exception ex)
          {
            Log.Error("FilterCriteria.set(): exception", ex);
          }
          this.DataBind(query);
        }
        else if (!m_firstFilter) // Запрет на множество перезагрузок первого фильтра
        {
          QueryObject query = new QueryObject();
          query.RootFilter = this.StaticFilter;
          try
          {
            query.AddFilterExpression(this.RelationField, this.parentList.CurrentObject.GetIdentifier());
          }
          catch (Exception ex)
          {
            Log.Error("FilterCriteria.set(): exception 2", ex);
          }
          this.DataBind(query);
        }
      }
    }

    /// <summary>
    /// Инкрементальный поиск
    /// </summary>
    /// <param name="column">Колонка, по которой производится поиск</param>
    /// <param name="value">Значение, введенное пользователем</param>
    /// <returns>Значение ключевого поля</returns>
    public object FindKeyByBeginWith(PropertyDescriptor column, string value)
    {
      string fieldName = RecordManager.GetFieldName(typeof(TType), column.Name);

      QueryObject query = new QueryObject();
      if (value != null)
      {
        query.AddFilterExpression(fieldName, FilterOperation.Like, value + "%");
      }
      else
      {
        query.AddFilterExpression(fieldName, FilterOperation.Equals, value);
      }

      using (TType ret = new TType())
      {
        ret.InitReader(query);
        if (ret.Next())
        {
          ret.Break();
        }
        return ret.GetIdentifier();
      }
    }

    /// <summary>
    /// Поиск ключа по значению
    /// </summary>
    /// <param name="column">Колонка, по которой производится поиск</param>
    /// <param name="value">Переданное значение</param>
    /// <returns>Значение ключевого поля</returns>
    public object FindKeyByValue(PropertyDescriptor column, object value)
    {
      string fieldName = RecordManager.GetFieldName(typeof(TType), column.Name);

      if (fieldName == this.KeyFieldName)
        return value;

      QueryObject qo = new QueryObject();
      qo.AddFilterExpression(fieldName, value);

      using (TType ret = new TType())
      {
        ret.InitReader(qo);
        try
        {
          if (ret.Next())
          {
            ret.Break();
          }
        }
        catch (Exception ex)
        {
          Log.Error("FindKeyByValue(): exception", ex);
          return null;
        }
        return ret.GetIdentifier();
      }
    }

    /// <summary>
    /// Собирает информацию о группах
    /// </summary>
    /// <param name="parentGroup">Родительская группа</param>
    /// <returns>Набор дочерних групп для выбранной родительской</returns>
    public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup)
    {
      List<ListSourceGroupInfo> ret = new List<ListSourceGroupInfo>();
      if (m_sortInfo == null)
        return ret;

      int level = parentGroup == null ? 0 : parentGroup.Level + 1;
      if (level >= m_sortInfo.Count)
        return ret;
      PropertyDescriptor pd = m_sortInfo[level].PropertyDescriptor;
      string fieldName = RecordManager.GetFieldName(typeof(TType), pd.Name);

      // Исследуем все вышестоящие группы; если хотя бы одна пустая,
      // дочернюю группу создавать не нужно - возвращаем управление,
      // чтобы зря не дёргать базу; заодно строим список прямых предков
      ListSourceGroupInfo checkGroup = parentGroup;
      List<ListSourceGroupInfo> parentGroups = new List<ListSourceGroupInfo>();
      while (checkGroup != null)
      {
        if (checkGroup.ChildDataRowCount == 0)
        {
          return new List<ListSourceGroupInfo>();
        }
        parentGroups.Add(checkGroup);
        checkGroup = m_group_tree[checkGroup];
      }

      QueryObject query = this.currentQuery != null ? this.currentQuery.Clone() : new QueryObject();
      foreach (ListSourceGroupInfo pg in parentGroups)
      {
        // при построении дочерней группы надо отфильтровать по всем прямым предкам группы
        query.AddFilterExpression(RecordManager.GetFieldName(typeof(TType), m_sortInfo[pg.Level].PropertyDescriptor.Name), pg.GroupValue);
      }
      // в первое поле пишем группирующее значение, во второе - количество строк в группе
      query.Aggregation.Clear();
      query.Aggregation.Add(fieldName, AggregateOperation.Group);
      query.Aggregation.Add("*", AggregateOperation.Count);
      query.Ordering.Clear();
      try
      {
        // Если для интересующего нас поля задана сортировка, следуем ей
        query.Ordering.Add(fieldName, this.currentQuery.Ordering[fieldName]);
      }
      catch (Exception ex)
      {
        Log.Error("GetGroupInfo(): exception", ex);
        // Иначе, сортируем список групп по возрастанию.
        query.Ordering.Add(fieldName, ListSortDirection.Ascending);
      }
      using (TType item = new TType())
      {
        Type pt = pd.PropertyType;
        if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
          pt = pt.GetGenericArguments()[0];

        using (IRecordBinder binder = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>(),
          item as ICustomRecordBinderProvider))
        {
          binder.OpenReader(item._TableName, query);
          while (binder.Next())
          {
            object group_value = binder.GetFieldValue(0);
            if (pt.IsEnum && group_value != null)
              group_value = Enum.ToObject(pt, group_value);

            ListSourceGroupInfo group = new ListSourceGroupInfo();
            group.Level = level;
            group.GroupValue = group_value;
            group.ChildDataRowCount = (int)binder.GetFieldValue(1);
            m_group_tree.Add(group, parentGroup);
            ret.Add(group);
          }
        }
      }

      return ret;
    }

    /// <summary>
    /// Получение номера строки в отфильтрованной таблице по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns>Номер строки</returns>
    public int GetRowIndexByKey(object key)
    {
      if (key != null)
      {
        for (int i = 0; i < this.items.Count; i++)
        {
          IDObjectStore st = this.items[i];
          if (st.ID != null && st.ID.Equals(key))
          {
            if (this.items[i].Value == null)
              this.items[i] = new IDObjectStore(key, RecordCache.GetItem<TType>(key));
            
            return i;
          }
        }
      }
      return -1;
    }

    /// <summary>
    /// Получение ключа по номеру строки отфильтрованной таблице
    /// </summary>
    /// <param name="index">Номер строки</param>
    /// <returns>Ключ</returns>
    public object GetRowKey(int index)
    {
      return this.items[index].ID;
    }

    // TODO: реализовать
    public Dictionary<object, object> GetTotalSummary()
    {
      return totalSummary;
    }

    /// <summary>
    /// Получение списка значений полей для автофильтра
    /// </summary>
    /// <param name="descriptor">Свойство, для которого получаем список значений</param>
    /// <param name="maxCount">Максимальное количество, поддерживаемое</param>
    /// <param name="includeFilteredOut">Указывает, надо ли использовать всю таблицу, или только отфильтрованную часть</param>
    /// <param name="roundDataTime"></param>
    /// <returns>Массив уникальных значений</returns>
    public object[] GetUniqueColumnValues(PropertyDescriptor descriptor, int maxCount, bool includeFilteredOut, bool roundDataTime)
    {
      List<object> ret = new List<object>();
      string fieldName = RecordManager.GetFieldName(typeof(TType), descriptor.Name);

      QueryObject query = new QueryObject();
      query.Aggregation.Add(fieldName, AggregateOperation.Group);
      query.Ordering.Add(fieldName, ListSortDirection.Ascending);
      if (!includeFilteredOut)
      {
        query.RootFilter = this.currentQuery.RootFilter;
      }

      using (TType item = new TType())
      {
        if (item.InitReader(query))
        {
          int counter = 0;
          while (item.Next())
          {
            if (maxCount > 0 && counter > maxCount)
            {
              item.Break();
            }
            ret.Add(descriptor.GetValue(item));
          }
        }
      }

      return ret.ToArray();
    }

    #endregion

		#region IListServer Members

		void IListServer.Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo)
		{
			throw new NotImplementedException();
		}

		event EventHandler<ServerModeExceptionThrownEventArgs> IListServer.ExceptionThrown
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		int IListServer.FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop)
		{
			throw new NotImplementedException();
		}

		IList IListServer.GetAllFilteredAndSortedRows()
		{
			throw new NotImplementedException();
		}

		List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup)
		{
			throw new NotImplementedException();
		}

		int IListServer.GetRowIndexByKey(object key)
		{
			throw new NotImplementedException();
		}

		object IListServer.GetRowKey(int index)
		{
			throw new NotImplementedException();
		}

		Dictionary<object, object> IListServer.GetTotalSummary()
		{
			throw new NotImplementedException();
		}

		object[] IListServer.GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut)
		{
			throw new NotImplementedException();
		}

		event EventHandler<ServerModeInconsistencyDetectedEventArgs> IListServer.InconsistencyDetected
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		int IListServer.LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp)
		{
			throw new NotImplementedException();
		}

		void IListServer.Refresh()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IList Members


		void IList.Clear()
		{
			throw new NotImplementedException();
		}

		bool IList.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		void IList.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICollection Members


		int ICollection.Count
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
