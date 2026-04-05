using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Xml.Serialization;
using AT.Toolbox.MSSQL.DAL.RecordBinding.Sql;
using AT.Toolbox.MSSQL.ORM.DAL;
using AT.Toolbox.MSSQL.Properties;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Базовый класс, не реализующий IListSource
  /// </summary>
  public class DbContextBase : IDbContext, IComponent, ISupportInitialize
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DbContextBase).Name);

    #region Fields

    private bool isDataBound;
    private bool disposed;
    private volatile bool initializing;
    private Dictionary<Type, IBindableObjectList> m_lists = new Dictionary<Type, IBindableObjectList>();

    #endregion

    #region Constructors

    static DbContextBase()
    {
      if (!RecordManager.IsRecordBinderFactorySet)
      {
        RecordManager.SetBinderFactory(new SqlRecordBinderFactory());
      }
    }

    internal DbContextBase()
    {
      this.MaxCacheSize = 1000;

      foreach (IBindableObjectList list in this.GetObjectLists())
      {
        this.RegisterType(list);
      }
    }

    #endregion

    #region Events

    public event EventHandler Disposed;

    /// <summary>
    /// Происходит во время ошибки при привязке данных
    /// </summary>
    public event ExceptionEventHandler ErrorMessage;

    public event InfoEventHandler InfoMessage;

    public event EventHandler InitBeforeTest;

    #endregion

    #region Properties

    [Browsable(false)]
    [XmlIgnore]
    public ISite Site { get; set; }

    /// <summary>
    /// Показывает, привязаны ли данные
    /// </summary>
    [Browsable(false)]
    public bool IsDataBound
    {
      get { return isDataBound; }
    }

    public IBindableObjectList this[Type itemType]
    {
      get
      {
        return m_lists[itemType];
      }
    }

    /// <summary>
    /// Если установлено в true, отключает автоматическую привязку данных для всех таблиц
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Disables automatic data binding for all lists on loading")]
    public bool ExactBinding { get; set; }

    /// <summary>
    /// Задает максимальный размер кэша для временных коллекций
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(1000)]
    [Description("Sets maximum cache size for temporary collections")]
    public int MaxCacheSize { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected bool DesignMode
    {
      get
      {
        return this.Site != null && this.Site.DesignMode;
      }
    }

    #endregion

    #region Binding methods

    /// <summary>
    /// Привязывает все коллекции к таблицам базы данных
    /// </summary>
    public void DataBind()
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }
      if (this.TestConnection(null))
      {
        foreach (IBindableObjectList state in this.m_lists.Values)
        {
          state.DataBind();
        }
        isDataBound = true;
      }
    }

    /// <summary>
    /// Проверка подключения по всем таблицам, объявленным в контексте
    /// </summary>
    /// <returns>True, если все таблицы подключились удачно</returns>
    public bool TestConnection(BackgroundWorker worker)
    {
      worker.WorkerReportsProgress = true;
      try
      {
        if (worker != null)
        {
          worker.ReportProgress(0, Resources.SQL_CHECKING_SERVER);
        }

        if (this.InitBeforeTest != null)
          this.InitBeforeTest(this, EventArgs.Empty);

        using (IRecordBinder bdr = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>()))
        {
          foreach (Type tableType in m_lists.Keys)
          {
            using (IBindable item = Activator.CreateInstance(tableType) as IBindable)
            {
              if (string.IsNullOrEmpty(item._KeyFieldName))
                continue;

              QueryObject query = new QueryObject();
              query.RootFilter = new NullFilter() { FieldName = item._KeyFieldName };
              bdr.OpenReader(item._TableName, query);
              if (worker != null)
              {
                worker.ReportProgress(0, item._TableName);
              }
              bdr.Next();
              bdr.CloseReader();
            }
          }
          RecordManager.WriteLog(Resources.CONTEXT_CONNECTION_OK, InfoLevel.Info);
          return true;
        }
      }
      catch(SecurityException ex)
      {
        Log.Error("TestConnection(): security exception", ex);
        SendErrorMessage(ex);
        return false;
      }
      catch (Exception ex)
      {
        Log.Error("TestConnection(): exception", ex);
        this.SendErrorMessage(new Exception(Resources.CONTEXT_CONNECTION_FAILED, ex));
        return false;
      }
    }

    /// <summary>
    /// В производном классе должен возвращать массив коллекций, привязываемых к базе данных
    /// </summary>
    /// <returns></returns>
    protected virtual IBindableObjectList[] GetObjectLists()
    {
      return new IBindableObjectList[0];
    }

    #endregion

    #region Validation methods

    internal void RegisterType(IBindableObjectList list)
    {
      Type listType = list.GetType();
      if (listType.IsGenericType)
      {
        listType = listType.GetGenericArguments()[0];
        m_lists[listType] = list;
        RecordCache.RegisterType(list);
      }
    }

    /// <summary>
    /// Оповещает владельца об ошибке. Если владелец не подписан на событие ErrorMessage, бросает исключение
    /// </summary>
    /// <param name="exception">Произошедшая ошибка</param>
    internal bool SendErrorMessage(Exception exception)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }
      RecordManager.ReportError(exception);
      if (this.ErrorMessage != null)
      {
        this.ErrorMessage(this, new ExceptionEventArgs(exception));
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Оповещает владельца
    /// </summary>
    /// <param name="messageBuffer">Набор сообщений</param>
    internal void SendMessageList(InfoEventArgs messageBuffer)
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }
      if (this.InfoMessage != null)
      {
        this.InfoMessage(this, messageBuffer);
      }
    }

    /// <summary>
    /// Выполняет автоматическую привязку, если тип списка неактуален для контекста
    /// </summary>
    /// <typeparam name="TType">Тип элемента списка</typeparam>
    /// <param name="source">Проверяемый список</param>
    /// <returns>Тот же список, но актуальный</returns>
    protected BindableObjectList<TType> GetValidList<TType>()
      where TType : class, IBindable, new()
    {
      if (disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }

      if (this.initializing || this.ExactBinding)
      {
        if (m_lists.ContainsKey(typeof(TType)))
        {
          return (BindableObjectList<TType>)m_lists[typeof(TType)];
        }
        else
          return null;
      }
      return RecordCache.GetValidList<TType>();
    }


    #endregion

    #region ISupportInitialize Members

    protected virtual void OnBeginInit() { }

    protected virtual void BeforeEndInit() { }

    protected virtual void AfterEndInit() { }

    void ISupportInitialize.BeginInit()
    {
      this.initializing = true;
      this.OnBeginInit();
    }

    void ISupportInitialize.EndInit()
    {
      if (!this.initializing)
      {
        return;
      }
      this.BeforeEndInit();
      if (this.Site == null || !this.Site.DesignMode)
      {
        if (!this.isDataBound && !this.ExactBinding && this.TestConnection(null))
        {
          this.DataBind();
        }
      }
      this.initializing = false;
      this.AfterEndInit();
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~DbContextBase()
    {
      this.Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        lock (this)
        {
          if (this.Site != null && this.Site.Container != null)
          {
            this.Site.Container.Remove(this);
          }
          foreach (IBindableObjectList state in this.m_lists.Values)
          {
            state.Dispose();
          }
          disposed = true;
          if (this.Disposed != null)
          {
            this.Disposed(this, EventArgs.Empty);
          }
        }
      }
    }

    #endregion
  }
}
