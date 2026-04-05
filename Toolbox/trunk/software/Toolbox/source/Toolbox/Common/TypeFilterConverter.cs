using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Common
{
  /// <summary>
  /// Базовый класс конвертера для выбора типа, удовлетворяющего некоторому условию.
  /// </summary>
  public class TypeFilterConverter : TypeConverter
  {
	  [NotNull] private static Dictionary<string, Type> _type_names = new Dictionary<string, Type>();

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;

      return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      string s = value as string;
      if (s != null)
      {
        if (string.IsNullOrWhiteSpace(s))
          return null;

        Type ret;

        if (_type_names.TryGetValue(s, out ret))
          return ret;
      }

      return null;
    }

    protected virtual bool CheckType([NotNull] Type type)
    {
	    if (type == null) throw new ArgumentNullException("type");

			return false;
    }

	  [NotNull]
	  private Type[] GetTypes([NotNull] Assembly asm)
    {
	    if (asm == null) throw new ArgumentNullException("asm");

			try
      {
        return asm.GetAvailableTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        return ex.Types.Where(t => t != null).ToArray();
      }
      catch
      {
        return Type.EmptyTypes;
      }
    }

	  [NotNull]
	  public override TypeConverter.StandardValuesCollection GetStandardValues([NotNull] ITypeDescriptorContext context)
    {
		  if (context == null) throw new ArgumentNullException("context");

			var values = from asm in AppDomain.CurrentDomain.GetAssemblies()
                   where asm.IsToolboxSearchable()
                   from type in this.GetTypes(asm)
                   where this.CheckType(type)
                   select type;

      foreach (var type in values)
        _type_names[type.ToString()] = type;

      return new StandardValuesCollection(_type_names.Values.OrderBy(v => v.FullName).ToArray());
    }
  }
}
