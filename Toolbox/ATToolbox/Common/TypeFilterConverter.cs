using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using AT.Toolbox;
using AT.Toolbox.Extensions;
using System.Reflection;

namespace AT.Toolbox
{
  /// <summary>
  /// Базовый класс конвертера для выбра типа, удовлетворяющего некоторому условию.
  /// </summary>
  public class TypeFilterConverter : TypeConverter
  {
    private static Dictionary<string, Type> _type_names = new Dictionary<string, Type>();

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
        if (s.Trim() == "")
          return null;

        Type ret;

        if (_type_names.TryGetValue(s, out ret))
          return ret;
      }

      return null;
    }

    protected virtual bool CheckType(Type type)
    {
      return false;
    }

    private Type[] GetTypes(Assembly asm)
    {
      try
      {
        return asm.GetTypes();
      }
      catch
      {
        return new Type[0];
      }
    }

    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      var values = from asm in AppDomain.CurrentDomain.GetAssemblies()
                   where asm.IsAtToolbox()
                   from type in this.GetTypes(asm)
                   where this.CheckType(type)
                   select type;

      foreach (var type in values)
        _type_names[type.ToString()] = type;

      return new StandardValuesCollection(_type_names.Values.OrderBy(v => v.FullName).ToArray());
    }
  }
}
