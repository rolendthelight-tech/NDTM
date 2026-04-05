using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;

namespace AT.Toolbox
{
  /// <summary>
  /// Обеспечивает быструю конвертацию стандартных типов и перечислений.
  /// </summary>
  public sealed class ConvertionHelper
  {
    public readonly Type PropertyType;

    private readonly Type m_clear_type;
    private readonly bool m_mandatory;
    private readonly bool m_is_enum;
    private readonly bool m_explicit_possible;
    private readonly Func<string> m_parameter_name;
    private readonly TypeConverter m_converter;

    private static Dictionary<Type, ValueSelector> _clear_type_mapping = new Dictionary<Type, ValueSelector>();

    private static Dictionary<KeyValuePair<Type, Type>, Func<object, object>> _explicit_type_mapping = new Dictionary<KeyValuePair<Type, Type>, Func<object, object>>();

    static ConvertionHelper()
    {
      _clear_type_mapping.Add(typeof(byte?), new ValueSelector<byte>());
      _clear_type_mapping.Add(typeof(sbyte?), new ValueSelector<sbyte>());
      _clear_type_mapping.Add(typeof(short?), new ValueSelector<short>());
      _clear_type_mapping.Add(typeof(ushort?), new ValueSelector<ushort>());
      _clear_type_mapping.Add(typeof(int?), new ValueSelector<int>());
      _clear_type_mapping.Add(typeof(uint?), new ValueSelector<uint>());
      _clear_type_mapping.Add(typeof(long?), new ValueSelector<long>());
      _clear_type_mapping.Add(typeof(ulong?), new ValueSelector<ulong>());
      _clear_type_mapping.Add(typeof(decimal?), new ValueSelector<decimal>());
      _clear_type_mapping.Add(typeof(float?), new ValueSelector<float>());
      _clear_type_mapping.Add(typeof(double?), new ValueSelector<double>());
      _clear_type_mapping.Add(typeof(bool?), new ValueSelector<bool>());
      _clear_type_mapping.Add(typeof(Guid?), new ValueSelector<Guid>());
      _clear_type_mapping.Add(typeof(DateTime?), new ValueSelector<DateTime>());
      _clear_type_mapping.Add(typeof(TimeSpan?), new ValueSelector<TimeSpan>());

      _explicit_type_mapping.Add(new KeyValuePair<Type, Type>(typeof(long), typeof(TimeSpan)), o => new TimeSpan((long)o));
      _explicit_type_mapping.Add(new KeyValuePair<Type, Type>(typeof(TimeSpan), typeof(long)), o => ((TimeSpan)o).Ticks);
      _explicit_type_mapping.Add(new KeyValuePair<Type, Type>(typeof(DateTime), typeof(TimeSpan)), o => ((DateTime)o).TimeOfDay);
      _explicit_type_mapping.Add(new KeyValuePair<Type, Type>(typeof(TimeSpan), typeof(DateTime)), TimeSpan2Date);
      _explicit_type_mapping.Add(new KeyValuePair<Type, Type>(typeof(string), typeof(Guid)), o => new Guid((string)o));
    }

    private static object TimeSpan2Date(object value)
    {
      var span = (TimeSpan)value;
      var date = DateTime.Now.Date;

      return new DateTime(date.Year, date.Month, date.Day)
        .AddHours(span.Hours).AddMinutes(span.Minutes).AddSeconds(span.Seconds);
    }

    private abstract class ValueSelector
    {
      public readonly Type ClearType;

      public ValueSelector()
      {
        this.ClearType = this.GetClearType();
      }

      protected abstract Type GetClearType();

      public abstract object Convert(object value);
    }

    private sealed class ValueSelector<TType> : ValueSelector where TType : struct
    {
      protected override Type GetClearType()
      {
        return typeof(TType);
      }

      public override object Convert(object value)
      {
        return ((TType?)value).Value;
      }
    }

    /// <summary>
    /// Инициализирует новый конвертер
    /// </summary>
    /// <param name="type">Тип, к которому требуется приводить</param>
    /// <param name="parameterNameGetter">Функция, возвращающая имя параметра для ArgumentNullException</param>
    public ConvertionHelper(Type type, Func<string> parameterNameGetter)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      PropertyType = type;
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

    /// <summary>
    /// Тип для конвертации не допускает присвоения null
    /// </summary>
    public bool Mandatory
    {
      get { return m_mandatory; }
    }

    /// <summary>
    /// Конвертирует объект в тип, переданный в конструктор
    /// </summary>
    /// <param name="value">Конвертируемое значение</param>
    /// <returns>Сконвертированное значение нужного типа</returns>
    public object Convert(object value)
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

          Func<object, object> converter;

          if (m_explicit_possible && _explicit_type_mapping.TryGetValue(new KeyValuePair<Type, Type>(type, m_clear_type), out converter))
            return converter(value);

          if (m_is_enum)
          {
            if (type.IsPrimitive)
              return Enum.ToObject(m_clear_type, value);
            else
              return Enum.Parse(m_clear_type, value.ToString());
          }
          else
          {
            IConvertible conv = value as IConvertible;

            if (conv != null)
              return conv.ToType(m_clear_type, null);

            if (m_converter.CanConvertFrom(type))
              return m_converter.ConvertFrom(value);
            else
            {
              var conv2 = TypeDescriptor.GetConverter(type);

              if (conv2.CanConvertTo(m_clear_type))
                return conv2.ConvertTo(value, m_clear_type);
              else
                throw new InvalidCastException();
            }
          }
        }
      }
    }

    public override string ToString()
    {
      return string.Format("Converter for {0}", this.PropertyType.FullName);
    }

    private ArgumentNullException GetNullException()
    {
      string parameter = null;

      if (m_parameter_name != null)
        parameter = m_parameter_name();

      if (string.IsNullOrEmpty(parameter))
        return new ArgumentNullException("value");
      else
        return new ArgumentNullException(parameter);
    }
  }
}
