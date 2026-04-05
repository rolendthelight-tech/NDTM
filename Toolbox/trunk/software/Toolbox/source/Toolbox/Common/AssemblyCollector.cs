using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Common
{
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
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(AssemblyCollector<T>));

    private readonly ISynchronizeInvoke m_invoker;

    public AssemblyCollector()
    { }

    public AssemblyCollector(ISynchronizeInvoke invoker)
    {
	    m_invoker = invoker;
    }

	  #region Protected Variables -------------------------------------------------------------------------------------------------

    /// <summary>
    /// Список созданных классов
    /// </summary>
    protected readonly List<T> m_classes = new List<T>( );

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
    public void Collect([NotNull] string full_assembly_name, AssemblyCollectMode mode )
    {
	    if (full_assembly_name == null) throw new ArgumentNullException("full_assembly_name");
	    try
      {
        Assembly assembly = Assembly.LoadFrom( full_assembly_name );

        Collect( assembly, mode );
      }
      catch( BadImageFormatException bex)
      {
				_log.ErrorFormat(bex, "Collect({0}, {1}): exception", full_assembly_name, mode);
      }
      catch( Exception ex )
      {
				_log.ErrorFormat(ex, "Collect({0}, {1}): exception 2", full_assembly_name, mode);

	      _log.ErrorFormat(
		      ex,
		      @"Error in AssemblyCollector<{0}>::CollectClassesWithType({1})",
		      typeof (T).Name,
		      full_assembly_name);
      }
    }

	  /// <summary>
    /// Сбор объектов определённого класса
    /// </summary>
    /// <param name="assembly">Сборка</param>
    /// <param name="mode"></param>
    public void Collect([NotNull] Assembly assembly, AssemblyCollectMode mode )
    {
	    if (assembly == null) throw new ArgumentNullException("assembly");

	    var types = assembly.GetAvailableTypes();
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
	          Type t1 = t;
	          m_invoker.Invoke((Action)(() =>
              {
                T value1 = (T)Activator.CreateInstance(t1);
                Classes.Add(value1);
              }), ArrayExtensions.Empty<object>());
          }
          else
          {
            T value = (T)Activator.CreateInstance(t);
            Classes.Add(value);
          }
        }
        catch (Exception ex)
        {
					_log.ErrorFormat(ex, @"Collect({0}): exception", assembly.FullName);
        }
      }
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    static bool InterfaceComparison([NotNull] Type t)
    {
	    if (t == null) throw new ArgumentNullException("t");

			foreach (Type t2 in t.GetInterfaces())
      {
        if (t2.Name == typeof(T).Name)
          return true;
      }

      return false;
    }

    static bool TypeOrBaseComparison([NotNull] Type t)
    {
	    if (t == null) throw new ArgumentNullException("t");

			if (t.Name != typeof(T).Name)
      {
        if (null == t.BaseType)
          return false;

        if (t.BaseType.Name != typeof(T).Name)
          return TypeOrBaseComparison(t.BaseType);
      }

      return true;
    }

    static bool TypeComparison([NotNull] Type t)
    {
	    if (t == null) throw new ArgumentNullException("t");

			return (t.Name == typeof(T).Name);
    }

	  #endregion
  }
}