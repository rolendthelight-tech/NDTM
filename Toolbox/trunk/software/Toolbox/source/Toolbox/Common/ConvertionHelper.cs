using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using JetBrains.Annotations;
using Toolbox.Extensions;
using Toolbox.Properties;

namespace Toolbox.Common
{
  /// <summary>
  /// Обеспечивает быструю конвертацию стандартных типов и перечислений.
  /// </summary>
  public sealed class ConvertionHelper
  {
		[NotNull] private readonly Type m_property_type;
	  [NotNull] private readonly Type m_clear_type;
    private readonly bool m_mandatory;
    private readonly bool m_is_enum;
    private readonly bool m_explicit_possible;
	  [CanBeNull] private readonly Func<string> m_parameter_name;
	  [NotNull] private readonly TypeConverter m_converter;

	  private static readonly Dictionary<Type, ValueSelector> _clear_type_mapping = new Dictionary<Type, ValueSelector>()
		  {
			  {typeof (byte?), new ValueSelector<byte>()},
			  {typeof (sbyte?), new ValueSelector<sbyte>()},
			  {typeof (short?), new ValueSelector<short>()},
			  {typeof (ushort?), new ValueSelector<ushort>()},
			  {typeof (int?), new ValueSelector<int>()},
			  {typeof (uint?), new ValueSelector<uint>()},
			  {typeof (long?), new ValueSelector<long>()},
			  {typeof (ulong?), new ValueSelector<ulong>()},
			  {typeof (decimal?), new ValueSelector<decimal>()},
			  {typeof (float?), new ValueSelector<float>()},
			  {typeof (double?), new ValueSelector<double>()},
			  {typeof (bool?), new ValueSelector<bool>()},
			  {typeof (Guid?), new ValueSelector<Guid>()},
			  {typeof (DateTime?), new ValueSelector<DateTime>()},
			  {typeof (DateTimeOffset?), new ValueSelector<DateTimeOffset>()},
			  {typeof (TimeSpan?), new ValueSelector<TimeSpan>()},
		  };

	  private static readonly Dictionary<KeyValuePair<Type, Type>, Func<object, object>> _explicit_type_mapping = new Dictionary
		  <KeyValuePair<Type, Type>, Func<object, object>>()
		  {
			  {new KeyValuePair<Type, Type>(typeof (long), typeof (TimeSpan)), o => new TimeSpan((long) o)},
			  {new KeyValuePair<Type, Type>(typeof (TimeSpan), typeof (long)), o => (long) ((TimeSpan) o).Ticks},
			  {new KeyValuePair<Type, Type>(typeof (DateTime), typeof (TimeSpan)), o => (TimeSpan) ((DateTime) o).TimeOfDay},
			  {new KeyValuePair<Type, Type>(typeof (TimeSpan), typeof (DateTime)), TimeSpan2DateTime},
			  {new KeyValuePair<Type, Type>(typeof (string), typeof (Guid)), o => new Guid((string) o)},
			  {new KeyValuePair<Type, Type>(typeof (string), typeof (Uri)), o => new Uri((string) o)}/*TODO: заменить на редактор Uri*/,
			  {new KeyValuePair<Type, Type>(typeof (TimeSpan), typeof (string)), o => ((TimeSpan) o).ToString("c")}/*TODO: заменить на передачу TimeSpan как есть*/,
			  {new KeyValuePair<Type, Type>(typeof (string), typeof (TimeSpan)), o => TimeSpan.ParseExact((string) o, "c", null)}/*TODO: заменить на передачу TimeSpan как есть*/,
			  {new KeyValuePair<Type, Type>(typeof (decimal), typeof (TimeSpan)), TimeSpanFromSeconds},
			  {new KeyValuePair<Type, Type>(typeof (TimeSpan), typeof (decimal)), TimeSpanToSeconds},
			  {new KeyValuePair<Type, Type>(typeof (double), typeof (TimeSpan)), TimeSpanFromSecondsFloat},
			  {new KeyValuePair<Type, Type>(typeof (TimeSpan), typeof (double)), TimeSpanToSecondsFloat},
		  };

	  static ConvertionHelper()
    {

		}

	  [NotNull]
	  private static object TimeSpan2DateTime([NotNull] object value)
    {
		  if (value == null) throw new ArgumentNullException("value");

		  var span = (TimeSpan)value;
      var date = DateTime.Now.Date;

			return date.Add(span);
    }

		[NotNull]
		private static object TimeSpanToSeconds([NotNull] object o)
		{
			if (o == null) throw new ArgumentNullException("o");

			return ((TimeSpan)o).TotalSecondsDecimal();
		}

		[NotNull]
		private static object TimeSpanToSecondsFloat([NotNull] object o)
		{
			if (o == null) throw new ArgumentNullException("o");

			return ((TimeSpan)o).TotalSeconds;
		}

	  [NotNull]
	  private static object TimeSpanFromSeconds([NotNull] object o)
	  {
		  if (o == null) throw new ArgumentNullException("o");

		  return TimeSpanExtensions.FromSeconds((decimal) o);
	  }

		[NotNull]
		private static object TimeSpanFromSecondsFloat([NotNull] object o)
		{
			if (o == null) throw new ArgumentNullException("o");

			return TimeSpanExtensions.FromSeconds((double)o);
		}

	  private abstract class ValueSelector
    {
		  [NotNull] public readonly Type ClearType;

      protected ValueSelector()
      {
        this.ClearType = this.GetClearType();
      }

		  [NotNull]
		  protected abstract Type GetClearType();

		  [NotNull]
		  public abstract object Convert([NotNull] object value);
    }

    private sealed class ValueSelector<TType> : ValueSelector where TType : struct
    {
      protected override Type GetClearType()
      {
        return typeof(TType);
      }

	    [NotNull]
	    public override object Convert([NotNull] object value)
      {
	      if (value == null) throw new ArgumentNullException("value");

	      return ((TType?)value).Value;
      }
    }

    /// <summary>
    /// Инициализирует новый конвертер
    /// </summary>
    /// <param name="type">Тип, к которому требуется приводить</param>
    /// <param name="parameterNameGetter">Функция, возвращающая имя параметра для ArgumentNullException</param>
    public ConvertionHelper([NotNull] Type type, [CanBeNull] Func<string> parameterNameGetter)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      m_property_type = type;
      m_mandatory = !type.IsNullable();
      m_clear_type = type.GetClearType();
      m_is_enum = m_clear_type.IsEnum;
      m_parameter_name = parameterNameGetter;
      m_converter = TypeDescriptor.GetConverter(m_clear_type);

      foreach (var kv in _explicit_type_mapping.Keys)
      {
        if (kv.Value == m_clear_type)
        {
          m_explicit_possible = true;
          break;
        }
      }
    }

		[NotNull]
		public Type PropertyType
		{
			get { return m_property_type; }
		}

    /// <summary>
    /// Тип для конвертации не допускает присвоения <code>null</code>
    /// </summary>
    public bool Mandatory
    {
      get { return m_mandatory; }
    }

		/// <summary>
		/// Конвертирует объект в тип, переданный в конструктор, зависимо от языка.
		/// </summary>
		/// <param name="value">Конвертируемое значение.</param>
		/// <returns>Сконвертированное значение нужного типа.</returns>
		[CanBeNull]
		[ContractAnnotation("value:null => null; value:notnull => notnull")]
    [Obsolete("Используйте ConvertVariant или ConvertInvariant")]
		public object Convert([CanBeNull] object value)
		{
      return ConvertVariant(value);
		}

    /// <summary>
    /// Конвертирует объект в тип, переданный в конструктор, зависимо от языка.
    /// </summary>
    /// <param name="value">Конвертируемое значение.</param>
    /// <returns>Сконвертированное значение нужного типа.</returns>
    [CanBeNull]
    [ContractAnnotation("value:null => null; value:notnull => notnull")]
    public object ConvertVariant([CanBeNull] object value)
    {
      return Convert(value, false);
    }

		/// <summary>
		/// Конвертирует объект в тип, переданный в конструктор, независимо от языка.
		/// </summary>
		/// <param name="value">Конвертируемое значение.</param>
		/// <returns>Сконвертированное значение нужного типа.</returns>
		[CanBeNull]
		[ContractAnnotation("value:null => null; value:notnull => notnull")]
		public object ConvertInvariant([CanBeNull] object value)
		{
			return Convert(value, true);
		}

	  /// <summary>
	  /// Конвертирует объект в тип, переданный в конструктор.
	  /// </summary>
	  /// <param name="value">Конвертируемое значение.</param>
	  /// <param name="invariantCulture">Использовать региональные параметры, не зависящие от языка.</param>
	  /// <returns>Сконвертированное значение нужного типа.</returns>
	  [CanBeNull]
		[ContractAnnotation("value:null => null; value:notnull => notnull")]
		private object Convert([CanBeNull] object value, bool invariantCulture)
		{
      if (value == null)
      {
        if (m_mandatory)
          throw this.GetNullException();
        else
          return null;
      }
      else
      {
        var type = value.GetType();

        if (type == PropertyType || type == m_clear_type
          || m_clear_type.IsAssignableFrom(type))
          return value;
        else
				{
					{
						ValueSelector selector;

						if (_clear_type_mapping.TryGetValue(type, out selector))
						{
							value = selector.Convert(value);
							type = selector.ClearType;
						}
					}

					{
						Func<object, object> converter;

						if (m_explicit_possible &&
						    _explicit_type_mapping.TryGetValue(new KeyValuePair<Type, Type>(type, m_clear_type), out converter))
							return converter(value);
					}

					if (m_is_enum)
					{
						if (type.IsPrimitive)
							return Enum.ToObject(m_clear_type, value);
						else
							return Enum.Parse(m_clear_type, value.ToString());
					}
					else
					{
						{
							IConvertible conv = value as IConvertible;

							if (conv != null)
								return FormatExceptionDetalizer((v, ci) => v.ToType(m_clear_type, ci), conv, invariantCulture ? CultureInfo.InvariantCulture : null);
						}

						if (m_converter.CanConvertFrom(type))
							return FormatExceptionDetalizer((v, ci) => m_converter.ConvertFrom(null, ci, v), value, invariantCulture ? CultureInfo.InvariantCulture : null); // TODO: Проверить реализацию
						else
						{
							var conv = TypeDescriptor.GetConverter(type);

							if (conv.CanConvertTo(m_clear_type))
								return FormatExceptionDetalizer((v, ci) => conv.ConvertTo(null, ci, v, m_clear_type), value, invariantCulture ? CultureInfo.InvariantCulture : null);
							else
								throw new InvalidCastException();
						}
					}
				}
      }
    }

		private TTarget FormatExceptionDetalizer<TTarget, TSource>([NotNull] Func<TSource, CultureInfo, TTarget> function, [CanBeNull] TSource value, [CanBeNull] CultureInfo cultureInfo)
		{
			if (function == null) throw new ArgumentNullException("function");

			try
			{
				return function(value, cultureInfo);
			}
			catch (FormatException ex)
			{
				throw new DetalizedFormatException(ex.Message, GetCultureForException(cultureInfo), value, ex);
			}
		}

	  [NotNull]
		private static string GetCultureForException([CanBeNull] CultureInfo cultureInfo)
	  {
			return cultureInfo == null ? "null" : cultureInfo.Name;
	  }

	  [Serializable]
	  public class DetalizedFormatException : FormatException
	  {
		  [CanBeNull] private readonly string m_culture;
		  [CanBeNull] private readonly object m_value;

		  protected DetalizedFormatException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		  {
			  if (info == null) throw new ArgumentNullException("info");

				this.m_culture = info.GetString("Culture");
				this.m_value = info.GetValue("Value", typeof(object));
			}

			public DetalizedFormatException([CanBeNull] string message, [CanBeNull] string culture, [CanBeNull] object value)
			  : base(message)
		  {
				this.m_culture = culture;
			  this.m_value = value;
		  }

			public DetalizedFormatException([CanBeNull] string message, [CanBeNull] string culture, [CanBeNull] object value, [CanBeNull] Exception innerException)
			  : base(message, innerException)
		  {
				this.m_culture = culture;
				this.m_value = value;
			}

		  [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		  public override void GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
		  {
			  if (info == null) throw new ArgumentNullException("info");

			  base.GetObjectData(info, context);

				info.AddValue("Culture", this.m_culture, typeof(string));
			  info.AddValue("Value", this.m_value, typeof (object));
		  }

		  public override string Message
		  {
			  get
			  {
				  string message = base.Message;
				  return string.Format(
					  "{0}{1}{2}", message, Environment.NewLine,
					  string.Format(Resources.DetalizedFormatException_Message_Format, FormatObject(this.Culture), FormatObject(this.Value)));
			  }
		  }

		  [CanBeNull]
		  public virtual string Culture
		  {
			  get { return this.m_culture; }
		  }

		  [CanBeNull]
		  public virtual object Value
		  {
			  get { return this.m_value; }
		  }

		  [NotNull]
		  private static string FormatObject([CanBeNull] object value)
		  {
			  if (value == null)
				  return "null";
			  else
				  return string.Format("\"{0}\"", value);
		  }
	  }

	  [NotNull]
	  public override string ToString()
    {
      return string.Format(Resources.ConvertionHelper_ToString_Format, this.PropertyType.FullName);
    }

	  [NotNull]
	  private ArgumentNullException GetNullException()
    {
      string parameter = null;

	    if (m_parameter_name != null)
	    {
		    try
		    {
			    parameter = m_parameter_name();
		    }
		    catch (Exception ex)
		    {
					throw new InvalidCastException(Resources.ConvertionHelper_GetNullException_parameterNameGetterThrowsException, ex);
		    }
	    }

			if (string.IsNullOrEmpty(parameter))
				return new ArgumentNullException("value");
			else
				return new ArgumentNullException(parameter);
    }
  }
}
