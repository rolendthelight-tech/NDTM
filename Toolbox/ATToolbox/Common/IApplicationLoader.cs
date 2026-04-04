using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;
using log4net;

namespace AT.Toolbox
{
  /// <summary>
  /// Загрузчик компонента приложения с зависимостями
  /// </summary>
  public interface IApplicationLoader : IComponentLoader, IDependencyItem<Type>
  {
  }

  /// <summary>
  /// Загрузчик компонента приложения
  /// </summary>
  public interface IComponentLoader
  {
    /// <summary>
    /// Загрузка компонента приложения
    /// </summary>
    /// <param name="context">Контекст загрузки</param>
    /// <returns>true, если компонент загружен. False, если не загружен</returns>
    bool Load(LoadingContext context);
  }

  /// <summary>
  /// Контекст загрузки компонентов приложения
  /// </summary>
  public class LoadingContext : IRunningContext
  {
    private static readonly ILog _log = LogManager.GetLogger("LoadingContext");

    private readonly BackgroundWorker m_worker;
    private readonly ISynchronizeInvoke m_invoker;
    private readonly DependencyContainer m_container;
    private readonly InfoBuffer m_buffer = new InfoBuffer();

    public LoadingContext(DependencyContainer container, BackgroundWorker worker, ISynchronizeInvoke invoker)
    {
      if (container == null)
        throw new ArgumentNullException("container");
      
      if (worker == null)
        throw new ArgumentNullException("worker");

      if (invoker == null)
        throw new ArgumentNullException("invoker");

      m_worker = worker;
      m_invoker = invoker;
      m_container = container;
    }

    public LoadingContext(BackgroundWorker worker, ISynchronizeInvoke invoker)
      : this(new DependencyContainer(), worker, invoker) { }

    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    public string[] CommandArgs { get; set; }

    /// <summary>
    /// Индикатор прогресса загрузки приложения
    /// </summary>
    public BackgroundWorker Worker
    {
      get { return m_worker; }
    }

    /// <summary>
    /// Объект синхронизации
    /// </summary>
    public ISynchronizeInvoke Invoker
    {
      get { return m_invoker; }
    }

    /// <summary>
    /// Контейнер загружаемых компонентов
    /// </summary>
    public DependencyContainer Container
    {
      get { return m_container; }
    }

    /// <summary>
    /// Буфер для сбора сообщений загрузки
    /// </summary>
    public InfoBuffer Buffer
    {
      get { return m_buffer; }
    }

    #region IRunningContext Members

    public bool Run(IRunBase runBase, LaunchParameters parameters)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      try
      {
        runBase.ProgressChanged += this.HandleProgressChanged;
        runBase.CancellationCheck += this.HandleCancellationCheck;
        runBase.Run();
        
        return true;
      }
      catch (Exception ex)
      {
        _log.Error("Run(): exception", ex);
        return false;
      }
      finally
      {
        runBase.ProgressChanged -= this.HandleProgressChanged;
        runBase.CancellationCheck -= this.HandleCancellationCheck;
      }
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      m_worker.ReportProgress(e.ProgressPercentage, e.UserState);
    }

    private void HandleCancellationCheck(object sender, CancelEventArgs e)
    {
      e.Cancel = m_worker.CancellationPending;
    }

    #endregion
  }

  /// <summary>
  /// Заместитель реального загрузчика для случаев, когда компоненты уже инициализированы
  /// </summary>
  public class EmptyApplicationLoader : IApplicationLoader
  {
    private readonly Type m_type;
    private readonly List<Type> m_mandatory_deps = new List<Type>();
    private readonly List<Type> m_optional_deps = new List<Type>();

    public EmptyApplicationLoader(Type keyType)
    {
      if (keyType == null)
        throw new ArgumentNullException("keyType");

      m_type = keyType;
    }

    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
      return true;
    }

    #endregion

    #region IDependencyItem<Type> Members

    public Type Key
    {
      get { return m_type; }
    }

    public IList<Type> MandatoryDependencies
    {
      get { return m_mandatory_deps; }
    }

    public IList<Type> OptionalDependencies
    {
      get { return m_optional_deps; }
    }

    #endregion
  }

  /// <summary>
  /// Загрузчик компонента по типу
  /// </summary>
  /// <typeparam name="TContract">Контракт, реализуемый типом компонента</typeparam>
  /// <typeparam name="TService">Реальный тип компонента</typeparam>
  public class ApplicationLoader<TContract, TService> : IApplicationLoader
    where TService : TContract
    where TContract : class
  {
    private static readonly FactoryMethod _factory_method;
    private static readonly Type[] _constructor_types;
    private static readonly List<KeyValuePair<PropertyInfo, Action<object, object>>> _properties 
      = new List<KeyValuePair<PropertyInfo, Action<object, object>>>();
    private static readonly bool _synchronization_required;

    private readonly ReadOnlyCollection<Type> m_mandatory_dependencies;
    private readonly ReadOnlyCollection<Type> m_optional_dependencies;

    static ApplicationLoader()
    {
      if (typeof(TService).IsAbstract)
        throw new InvalidProgramException();

      _synchronization_required = typeof(ISynchronizeInvoke).IsAssignableFrom(typeof(TService));

      var constructor = (from ctor in typeof(TService).GetConstructors()
                         let item = new 
                         {
                           Method = ctor,
                           Params = ctor.GetParameters()
                         }
                         where item.Params.Length == 0 ||
                         item.Params.All(p => !IsScalar(p.ParameterType))
                         orderby item.Params.Length descending
                         select item).First();

      _factory_method = DynamicMethodFactory.CreateFactoryMethod(constructor.Method);
      _constructor_types = new Type[constructor.Params.Length];

      for (int i = 0; i < _constructor_types.Length; i++)
        _constructor_types[i] = constructor.Params[i].ParameterType;

      foreach (var pi in typeof(TService).GetProperties())
      {
        if (IsScalar(pi.PropertyType))
          continue;
        
        var set = pi.GetSetMethod(false);

        if (set == null)
          continue;

        _properties.Add(new KeyValuePair<PropertyInfo, Action<object, object>>(pi, pi.CreateSetter()));
      }
    }

    private static bool IsScalar(Type type)
    {
      return type.IsValueType || type == typeof(string);
    }

    public ApplicationLoader()
    {
      m_mandatory_dependencies = new ReadOnlyCollection<Type>(
        new HashSet<Type>(_constructor_types).ToList());

      m_optional_dependencies = new ReadOnlyCollection<Type>(
        new HashSet<Type>(_properties
          .Where(kv => this.FilterProperty(kv.Key))
          .Select(kv => kv.Key.PropertyType)).ToList());
    }

    protected virtual bool FilterProperty(PropertyInfo property)
    {
      return true;
    }
    
    #region IApplicationLoader Members

    public bool Load(LoadingContext context)
    {
      var ctor_params = new object[_constructor_types.Length];
      var lookup = new Dictionary<Type, object>();

      for (int i = 0; i < ctor_params.Length; i++)
      {
        object value;

        if (!lookup.TryGetValue(_constructor_types[i], out value))
        {
          value = context.Container.GetService(_constructor_types[i]);
          lookup[_constructor_types[i]] = value;
        }

        ctor_params[i] = value;

        if (ctor_params[i] == null)
          return false;
      }

      object item = null;

      if (_synchronization_required && context.Invoker.InvokeRequired)
        item = context.Invoker.Invoke(_factory_method, new object[] { ctor_params });
      else
        item = _factory_method(ctor_params);

      if (item == null)
        return false;

      foreach (var pi in _properties)
      {
        if (!this.FilterProperty(pi.Key))
          continue;
        
        object value;

        if (!lookup.TryGetValue(pi.Key.PropertyType, out value))
        {
          value = context.Container.GetService(pi.Key.PropertyType);
          lookup[pi.Key.PropertyType] = value;
        }

        if (value != null)
        {
          if (_synchronization_required && context.Invoker.InvokeRequired)
            context.Invoker.Invoke(pi.Value, new object[] { item, value });
          else
            pi.Value(item, value);
        }
      }

      context.Container.SetService(typeof(TContract), item);
      return true;
    }

    #endregion

    #region IDependencyItem<Type> Members

    Type IDependencyItem<Type>.Key
    {
      get { return typeof(TContract); }
    }

    IList<Type> IDependencyItem<Type>.MandatoryDependencies
    {
      get { return m_mandatory_dependencies; }
    }

    IList<Type> IDependencyItem<Type>.OptionalDependencies
    {
      get { return m_optional_dependencies; }
    }

    #endregion
  }
}
