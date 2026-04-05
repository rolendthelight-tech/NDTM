namespace AT.Toolbox
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
using System.ComponentModel;


  /// <summary>
  /// Тип сборки
  /// </summary>
  public enum AssemblyCollectMode
  {
    /// <summary>
    /// Классы
    /// </summary>
    Type,

    /// <summary>
    /// Класс или базовый класс
    /// </summary>
    TypeOrBaseType,

    /// <summary>
    /// Класс, реализующий интерфейс
    /// </summary>
    Interface
  }

  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ExcludeFromLoading : Attribute { }

  /// <summary>
  /// Сборщик объектов определённого класса или объектов, реализующиъ определённый интерфейс из сборки
  /// Класс инициализируется конструктором по умолчанию ( без параметров )
  /// </summary>
  /// <typeparam name="T">Класс или интерфейс для сбора</typeparam>
  public class AssemblyCollector<T>
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AssemblyCollector<T>).Name);

    private readonly ISynchronizeInvoke m_invoker;

    public AssemblyCollector()
    { }

    public AssemblyCollector(ISynchronizeInvoke invoker)
    {
      m_invoker = invoker;
    }

    #region Protected Variables -------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список созданых классов
    /// </summary>
    protected List<T> m_classes = new List<T>( );

    #endregion


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список собраных и инициализированиых объектов
    /// </summary>
    public List<T> Classes
    {
      get { return m_classes; }
    }

    #endregion


    #region Public Methods ------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Сбор объектов определённого класса
    /// </summary>
    /// <param name="full_assembly_name">Полный путь к сборке</param>
    /// <param name="mode"></param>
    public void Collect( string full_assembly_name, AssemblyCollectMode mode )
    {
      try
      {
        Assembly assembly = Assembly.LoadFrom( full_assembly_name );

        Collect( assembly, mode );
      }
      catch( BadImageFormatException bex)
      {
        Log.Error(string.Format("Collect({0}, {1}): exception", full_assembly_name, mode), bex);
      }
      catch( Exception ex )
      {
        Log.Error(string.Format("Collect({0}, {1}): exception 2", full_assembly_name, mode), ex);

        Log.Error(
          string.Format( @"Error in AssemblyCollector<{0}>::CollectClassesWithType({1})",
                         typeof( T ).Name,
                         full_assembly_name ),
          ex );
      }
    }

    /// <summary>
    /// Сбор объектов определённого класса
    /// </summary>
    /// <param name="assembly">Cборкa</param>
    /// <param name="mode"></param>
    public void Collect( Assembly assembly, AssemblyCollectMode mode )
    {
    	var types = assembly.GetTypes();
			foreach (Type t in types)
      {
        if( t.GetCustomAttributes( typeof( ExcludeFromLoading ), false ).Length > 0 )
          continue;

        if (!typeof(T).IsAssignableFrom(t))
          continue;

        try
        {
          ConstructorInfo ifo = t.GetConstructor(Type.EmptyTypes);

          if (ifo == null)
            continue;

          if (m_invoker != null && m_invoker.InvokeRequired
            && typeof(ISynchronizeInvoke).IsAssignableFrom(t))
          {
            m_invoker.Invoke(new Action(() =>
              {
                T value1 = (T)Activator.CreateInstance(t);
                Classes.Add(value1);
              }), new object[0]);
          }
          else
          {
            T value = (T)Activator.CreateInstance(t);
            Classes.Add(value);
          }
        }
        catch (Exception ex)
        {
          Log.Error(string.Format(@"Collect({0}): exception", assembly.FullName), ex);
        }
      }
    }

    #endregion


    #region Private Methods -----------------------------------------------------------------------------------------------------

    static bool InterfaceComparison(Type t)
    {
      foreach (Type t2 in t.GetInterfaces())
      {
        if (t2.Name == typeof(T).Name)
          return true;
      }

      return false;
    }

    static bool TypeOrBaseComparison(Type t)
    {
      if (t.Name != typeof(T).Name)
      {
        if (null == t.BaseType)
          return false;

        if (t.BaseType.Name != typeof(T).Name)
          return TypeOrBaseComparison(t.BaseType);
      }

      return true;
    }

    static bool TypeComparison(Type t) { return (t.Name == typeof(T).Name); }

    #endregion
  }
}