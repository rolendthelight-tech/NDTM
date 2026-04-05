using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application.Load;
using Toolbox.Common;
using Toolbox.Extensions;

namespace Toolbox.GUI.Controls
{
  /// <summary>
  /// Заместитель элемента управления для отложенной инициализации
  /// </summary>
  public partial class ControlPlaceholder : UserControl, IApplicationLoader
  {
    private string m_replacing_type;
    private IApplicationLoader m_inner_loader;
    private readonly object m_lock = new object();
    IOperationWrapper m_op_wrapper;
    private bool m_loading;

    public ControlPlaceholder()
    {
      m_op_wrapper = new SynchronizeOperationWrapper(this);

      InitializeComponent();
    }

    [Category("Contract")]
    public event EventHandler<LoadingCompletedEventArgs> LoadingCompleted;

    [Category("Contract")]
    [TypeConverter(typeof(ControlTypeTypeConverter))]
    public string ReplacingType
    {
      get { return m_replacing_type; }
      set
      {
        lock (m_lock)
        {
          m_replacing_type = value;
          m_inner_loader = null;
        }
      }
    }

    [Category("Contract")]
    [DefaultValue(null)]
    public string ControlDescription { get; set; }

	  [CanBeNull]
	  public TControl GetControl<TControl>()
      where TControl : Control
    {
      if (this.Controls.Count != 1)
        return null;

      return (TControl)this.Controls[0];
    }

	  [CanBeNull]
	  private IApplicationLoader GetCurrentLoader()
    {
      lock (m_lock)
      {
        if (m_inner_loader == null && !string.IsNullOrEmpty(m_replacing_type))
        {
          var control_type = Type.GetType(m_replacing_type, false);

          if (control_type != null)
          {
            var loader_type = typeof(ControlPlaceholderApplicationLoader<,>)
              .MakeGenericType(
              control_type.GetCustomAttribute<ContractPlaceholderAttribute>(false).ContractType,
              control_type);

            m_inner_loader = (IApplicationLoader)Activator.CreateInstance(loader_type);
          }
        }

        return m_inner_loader;
      }
    }

	  [CanBeNull]
	  private TResult GetLoaderProperty<TResult>([NotNull] Func<IApplicationLoader, TResult> func)
      where TResult : class
    {
	    if (func == null) throw new ArgumentNullException("func");

			var loader = this.GetCurrentLoader();

      if (loader != null)
        return func(loader);
      else
        return null;
    }

    private void HandleControlsChanged(object sender, ControlEventArgs e)
    {
      if (!m_loading)
				throw new InvalidOperationException("¬m_loading");
    }

    #region IApplicationLoader Members

    bool IComponentLoader.Load([NotNull] LoadingContext context)
    {
	    if (context == null) throw new ArgumentNullException("context");

			if (!string.IsNullOrEmpty(this.ControlDescription))
        context.Worker.ReportProgress(0, this.ControlDescription);

      var loader = this.GetCurrentLoader();

      if (loader == null)
        return false;

      if (loader.Load(context))
      {
        var control = m_op_wrapper.Invoke(GetGetService(context, loader));

        if (control == null)
          return false;

        m_op_wrapper.Invoke(() =>
        {
          m_loading = true;

          try
          {
            var diposee = this.Controls.Cast<Control>().ToArray();

            this.Controls.Clear();

            foreach (var ctrl in diposee)
              ctrl.Dispose();

            control.Dock = DockStyle.Fill;
            this.Controls.Add(control);
          }
          finally
          {
            m_loading = false;
          }
        });

        var ctrl_loader = control as IComponentLoader;

        if (ctrl_loader != null && !ctrl_loader.Load(context))
          return false;

        m_op_wrapper.Invoke(() =>
        {
          if (this.LoadingCompleted != null)
            this.LoadingCompleted(this, new LoadingCompletedEventArgs(context));
        });

        return true;
      }
      else
        return false;
    }

	  [NotNull]
	  private static Func<Control> GetGetService([NotNull] LoadingContext context, [NotNull] IApplicationLoader loader)
	  {
		  if (context == null) throw new ArgumentNullException("context");
		  if (loader == null) throw new ArgumentNullException("loader");

			return () => context.Container.GetService(loader.Key) as Control;
	  }

	  Type IDependencyItem<Type>.Key
    {
      get { return this.GetLoaderProperty(l => l.Key); }
    }

    IList<Type> IDependencyItem<Type>.MandatoryDependencies
    {
      get { return this.GetLoaderProperty(l => l.MandatoryDependencies); }
    }

    IList<Type> IDependencyItem<Type>.OptionalDependencies
    {
      get { return this.GetLoaderProperty(l => l.OptionalDependencies); }
    }

    #endregion
  }

  internal class ControlTypeTypeConverter : TypeConverter
  {
    public override bool  GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(Type))
        return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string))
        return true;

      return base.CanConvertTo(context, destinationType);
    }

		public override object ConvertFrom(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
    {
      if (value is Type)
        return GetTypeString(value);

      return base.ConvertFrom(context, culture, value);
    }

		public override object ConvertTo(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (value is Type && destinationType == typeof(string))
        return GetTypeString(value);

      return base.ConvertTo(context, culture, value, destinationType);
    }

    private static string GetTypeString(object value)
    {
      var t = (Type)value;

      return string.Format("{0}, {1}", t.FullName, t.Assembly.GetName().Name);
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      var ret = new List<string>();
      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!asm.IsToolboxSearchable())
          continue;

        try
        {
          foreach (var type in asm.GetAvailableTypes())
          {
            if (!typeof(Control).IsAssignableFrom(type))
              continue;

            var contract = type.GetCustomAttribute<ContractPlaceholderAttribute>(false);

            if (contract == null || !contract.ContractType.IsAssignableFrom(type))
              continue;

            ret.Add(GetTypeString(type));
          }
        }
        catch { }
      }

      return new StandardValuesCollection(ret);
    }
  }

  internal class ControlPlaceholderApplicationLoader<TContract, TService>
    : ApplicationLoader<TContract, TService>
    where TService : TContract
    where TContract : class
  {
		protected override bool FilterProperty([NotNull] global::System.Reflection.PropertyInfo property)
		{
			if (property == null) throw new ArgumentNullException("property");

			return property.IsDefined(typeof(ContractDependencyAttribute), false);
		}
  }

  /// <summary>
  /// Задаёт для элемента управления контракт 
	/// для отложенной инициализации с помощью <see cref="ControlPlaceholder"/>
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class ContractPlaceholderAttribute : Attribute
  {
	  [NotNull] private readonly Type m_contract_type;

    /// <summary>
    /// Задаёт для элемента управления контракт 
		/// для отложенной инициализации с помощью <see cref="ControlPlaceholder"/>
    /// </summary>
    /// <param name="contractType">Тип контракта, реализуемый элементом управления</param>
    public ContractPlaceholderAttribute([NotNull] Type contractType)
    {
      if (contractType == null)
        throw new ArgumentNullException("contractType");

      if (!contractType.IsInterface)
        throw new ArgumentException("Contract type must be an interface type", "contractType");

      m_contract_type = contractType;
    }

    /// <summary>
    /// Тип контракта, реализуемый элементом управления
    /// </summary>
    [NotNull]
    public Type ContractType
    {
      get { return m_contract_type; }
    }
  }

  /// <summary>
  /// Помечает свойство как зависимость при отложенной инициализации
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public sealed class ContractDependencyAttribute : Attribute { }

  public class LoadingCompletedEventArgs : EventArgs
  {
	  [NotNull] private readonly LoadingContext m_context;

    public LoadingCompletedEventArgs([NotNull] LoadingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");

      m_context = context;
    }

	  [NotNull]
	  public LoadingContext Context
    {
      get { return m_context; }
    }
  }
}
