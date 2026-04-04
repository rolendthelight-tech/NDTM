using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using AT.Toolbox.Properties;

namespace AT.Toolbox
{
  public static class SystemExtensions
  {
    #region Fields

    private static readonly Graphics _graphics = Graphics.FromHwnd(IntPtr.Zero);
    private static readonly Dictionary<Type, List<EnumLabel>> _enum_labels =
      new Dictionary<Type, List<EnumLabel>>();

    #endregion

    #region Reflection helpers

    /// <summary>
    /// Возвращает экземпляр атрибута у элемента
    /// </summary>
    /// <typeparam name="A">Тип атрибута</typeparam>
    /// <param name="item">Элемент</param>
    /// <param name="inherit">Указывает, нужно ли искать атрибут в предках</param>
    /// <returns>Если имеется единственный экземпляр, то его. Иначе, null</returns>
    public static A GetCustomAttribute<A>(this MemberInfo item, bool inherit)
      where A : Attribute
    {
      if (item.IsDefined(typeof(A), inherit))
      {
        return item.GetCustomAttributes(typeof(A), inherit)[0] as A;
      }

      return null;
    }

    /// <summary>
    /// Возвращает экземпляр атрибута у сборки, содержащей указанный тип
    /// </summary>
    /// <typeparam name="A">Тип атрибута</typeparam>
    /// <param name="type">тип</param>
    /// <returns>Если имеется единственный экземпляр, то его. Если экземпляров нет, то null. Иначе бросает исключение</returns>
    public static A GetCustomAssemblyAttribute<A>(this Type type)
      where A : Attribute
    {
      try
      {
        return (A) (type.Assembly.GetCustomAttributes(type, false).SingleOrDefault());
      }
      catch(Exception ex)
      {
        throw new Exception(string.Format("У сборки {1}, содержащей тип {0}, слишком много атрибутов {2}", type, type.Assembly, typeof(A)), ex);
      }
    }

    /// <summary>
    /// Проверяет тип на допустимось присвоения null
    /// </summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns>True, если тип допускает присвоение null</returns>
    public static bool IsNullable(this Type type)
    {
      return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Возвращает не-nullable тип
    /// </summary>
    /// <param name="type">Произвольный тип</param>
    /// <returns>Тип, приведённый к не-nullable виду</returns>
    public static Type GetClearType(this Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        return type.GetGenericArguments()[0];

      return type;
    }

    /// <summary>
    /// Проверяет, является ли тип числовым
    /// </summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns>true, если тип относится к числовым типам</returns>
    public static bool IsNumeric(this Type type)
    {
      if (!type.IsPrimitive)
        return type == typeof(decimal);
      else
        return type != typeof(bool);
    }

    /// <summary>
    /// Возвращает недействительную (nullable) версию типа
    /// </summary>
    /// <param name="type">Произвольный тип</param>
    /// <returns>Тип, приведённый к недействительному виду</returns>
    public static Type GetNullableType(this Type type)
    {
      if (type == null)
        return null;

      if (type == typeof(void))
        return null; // Не существует There is no Nullable version of void

      return IsNullable(type) ? type : typeof(Nullable<>).MakeGenericType(type);
    }

    public static string GetDisplayName(this Type type)
    {
      string displayName = null;

      if (type.IsDefined(typeof(DisplayNameAttribute), true))
        displayName = type.GetCustomAttribute<DisplayNameAttribute>(true).DisplayName;

      return string.IsNullOrEmpty(displayName) ? type.Name : displayName;
    }

    #endregion

    #region Enum Helpers

    /// <summary>
    /// Получает метку у перечисления
    /// </summary>
    /// <param name="value">Объект перечислимого типа</param>
    /// <returns>Метка</returns>
    public static string GetLabel(this Enum value)
    {
      if (value == null)
        return null;

      lock (_enum_labels)
      {
        foreach (var item in GetLabels(value.GetType()))
        {
          if (item.Value.Equals(value))
            return item.Label;
        }

        return value.ToString();
      }
    }

    /// <summary>
    /// Оичщает локализованные метки у перечислений
    /// </summary>
    public static void ClearEnumLabels()
    {
      lock (_enum_labels)
        _enum_labels.Clear();
    }

    public static T ParseEnumLabel<T>(string label)
      where T : struct 
    {
      object result = ParseEnumLabel(typeof (T), label);

      if (result == null)
      {
        return (T)Enum.Parse(typeof(T), label);
      }

      return (T)result;
    }

    internal static object ParseEnumLabel(Type enumType, string label)
    {
      if (string.IsNullOrEmpty(label))
        return null;

      foreach (var enumLabel in GetLabels(enumType))
      {
        if (label.Equals(enumLabel.Label))
          return enumLabel.Value;
      }

      return null;
    }

    private class EnumLabel
    {
      public object Value { get; set; }

      public string Label { get; set; }
    }

    private static List<EnumLabel> GetLabels(Type enumType)
    {
      List<EnumLabel> list;
      if (!_enum_labels.TryGetValue(enumType, out list))
      {
        list = new List<EnumLabel>();
        foreach (var fi in enumType.GetFields(BindingFlags.Public
          | BindingFlags.NonPublic | BindingFlags.Static))
        {
          EnumLabel item = new EnumLabel();

          if (fi.IsDefined(typeof(DisplayNameAttribute), true))
          {
            item.Label = (fi.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
              as DisplayNameAttribute).DisplayName;
          }

          if (string.IsNullOrEmpty(item.Label))
            item.Label = fi.Name;

          item.Value = fi.GetValue(null);
          list.Add(item);
        }

        _enum_labels.Add(enumType, list);
      }
      return list;
    }

    #endregion

    #region Call helpers

    /// <summary>
    /// Выполняет вызов обработчика события с синхронизацией
    /// </summary>
    /// <typeparam name="TArgs">Тип события</typeparam>
    /// <param name="eventHandler">Обрабатываемый делегат</param>
    /// <param name="sender">Компонент, инициировавший событие</param>
    /// <param name="args">Событие</param>
    public static void InvokeSynchronized<TArgs>(this Delegate eventHandler, object sender, TArgs args)
      where TArgs : EventArgs
    {
      if (eventHandler == null)
        return;

      foreach (var dlgt in eventHandler.GetInvocationList())
      {
        var invoker = dlgt.Target as ISynchronizeInvoke;

        if (invoker != null && invoker.InvokeRequired)
        {
          invoker.Invoke(dlgt, new object[] { sender, args });
        }
        else
        {
          var handler = dlgt as EventHandler<TArgs>;

          if (handler != null)
            handler(sender, args);
          else
          {
            try
            {
              dlgt.DynamicInvoke(sender, args);
            }
            catch (Exception ex)
            {
              throw new ArgumentException(string.Format(Resources.INVALID_EVENT_HANDLER,
                eventHandler.GetType(), typeof(TArgs)), ex);
            }
          }
        }
      }
    }

    /// <summary>
    /// Выполнение действия в потоке пользовательского интерфейса
    /// </summary>
    /// <param name="control">Элемент для синхронизации</param>
    /// <param name="code">Выполняемый код</param>
    static public void UIThread(this ISynchronizeInvoke control, Action code)
    {
      if (control.InvokeRequired)
      {
        control.BeginInvoke(code, new object[0]);
        return;
      }
      code.Invoke();
    }

    static public void UIThreadInvoke(this ISynchronizeInvoke control, Action code)
    {
      if (control.InvokeRequired)
      {
        control.Invoke(code, new object[0]);
        return;
      }
      code.Invoke();
    }

    #endregion

    #region PathHelpers

    public static string GetRelativePath(string rootPath, string fullPath)
    {
      string[] root_lines = Path.GetFullPath(rootPath).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      string[] full_lines = Path.GetFullPath(fullPath).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

      int number2up = root_lines.Length - full_lines.Length;

      if (number2up < 0)
        number2up = 0;

      for (int i = 0; i < root_lines.Length && i < full_lines.Length; i++)
      {
        string full_part = GetSubPath(full_lines, i);
        string root_part = GetSubPath(root_lines, i);

        if (Path.GetFullPath(full_part) != Path.GetFullPath(root_part))
        {
          number2up = root_lines.Length - i;
          break;
        }
      }

      List<string> ret = new List<string>();

      for (int i = 0; i < number2up; i++)
        ret.Add("..");

      for (int i = root_lines.Length - number2up; i < full_lines.Length; i++)
        ret.Add(full_lines[i]);

      return string.Join(Path.DirectorySeparatorChar.ToString(), ret.ToArray());
    }

    public static string GetAbsolutePath(string rootPath, string fullPath)
    {
      if (!Directory.Exists(rootPath) && File.Exists(rootPath))
        rootPath = Path.GetFullPath(Path.GetDirectoryName(rootPath));
      else
        rootPath = Path.GetFullPath(rootPath);

      if (!Directory.Exists(rootPath) && File.Exists(rootPath))
        rootPath = Path.GetDirectoryName(rootPath);

      string[] root_lines = rootPath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      string[] full_lines = fullPath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

      int number2up = 0;

      foreach (var line in full_lines)
      {
        if (line != "..")
          break;

        number2up++;
      }

      List<string> ret = new List<string>();

      for (int i = 0; i < root_lines.Length - number2up; i++)
        ret.Add(root_lines[i]);

      for (int i = number2up; i < full_lines.Length; i++)
        ret.Add(full_lines[i]);

      return string.Join(Path.DirectorySeparatorChar.ToString(), ret.ToArray());
    }

    private static string GetSubPath(string[] full_lines, int i)
    {
      string[] full_bit = new string[i + 1];

      for (int j = 0; j < full_bit.Length; j++)
        full_bit[j] = full_lines[j];

      string full_part = string.Join(Path.DirectorySeparatorChar.ToString(), full_bit);
      return full_part;
    }

    [DllImport("shlwapi.dll")]
    private static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

    public static string TruncatePath(string path, Font font, int width)
    {
      if (string.IsNullOrEmpty(path) || width < 1)
        return string.Empty;

      if (font == null)
      {
        StringBuilder sb = new StringBuilder();
        PathCompactPathEx(sb, path, width, 0);
        return sb.ToString();
      }
      
      string tmp = path;
      int length = tmp.Length;

      while (_graphics.MeasureString(tmp, font).Width > width)
      {
        length--;
        StringBuilder sb = new StringBuilder();
        PathCompactPathEx(sb, path, length, 0);
        tmp = sb.ToString();
      }

      return tmp;
    }

    #endregion

    #region Misc

    /// <summary>
    /// Строго типизированный доступ к сервису через провайдер
    /// </summary>
    /// <typeparam name="TService">Тип сервиса</typeparam>
    /// <param name="provider">Провайдер сервиса</param>
    /// <returns>Экземпляр сервиса</returns>
    public static TService GetService<TService>(this IServiceProvider provider)
    {
      return (TService)provider.GetService(typeof(TService));
    }

    /// <summary>
    /// Подстановка значений управляющих символов
    /// </summary>
    /// <param name="text">Текст с обозначениями управляющих символов</param>
    /// <returns>Текст с управляющими символами</returns>
    public static string ReplaceEscapes(string text)
    {
      var escapes = new Dictionary<string, string>();
      escapes.Add("(^\\\\n)|[^\\\\](\\\\n)", "\n");
      escapes.Add("(^\\\\r)|[^\\\\](\\\\r)", "\r");
      escapes.Add("(^\\\\t)|[^\\\\](\\\\t)", "\t");
      escapes.Add("(^\\\\s)|[^\\\\](\\\\s)", " ");

      foreach (var pair in escapes)
      {
        text = Regex.Replace(text, pair.Key,
                        delegate(Match m)
                        {
                          string v = m.Groups[0].Value;
                          return (v.Length > 2 ? v.Substring(0, v.Length - 2) : string.Empty) + pair.Value;
                        });
      }

      return text;
    }

    /// <summary>
    /// Конвертация форматирующей строки с набором полей
    /// </summary>
    /// <param name="format">Форматирующая строка, содержащая в фигурных скобках ссылки на имена полей</param>
    /// <param name="fieldNames">Массив имён полей</param>
    /// <param name="indexes">Список, заполняемый индексами имён полей</param>
    /// <returns>Форматирующая строка, в которой имена полей заменены индексами в заполненном списке</returns>
    public static string ConvertFormatString(string format, string[] fieldNames, IList<int> indexes)
    {
      indexes.Clear();
      
      for (int i = 0; i < fieldNames.Length; i++)
      {
        string pattern1 = "{" + fieldNames[i] + "}";
        string pattern2 = "{" + fieldNames[i] + ":";
        bool hit1 = format.Contains(pattern1);
        bool hit2 = format.Contains(pattern2);

        if (hit1 || hit2)
        {
          var idx = indexes.Count;

          if (hit1)
            format = format.Replace(pattern1, "{" + idx + "}");

          if (hit2)
            format = format.Replace(pattern2, "{" + idx + ":");

          indexes.Add(i);
        }
      }

      return format;
    }

    public static int GetCategoryIndex(string category, IList<string> order)
    {
      var ret = order.IndexOf(category);

      if (ret < 0)
        return int.MaxValue;

      return ret;
    }

    public static bool AreEqual(object obj1, object  obj2, bool emptyStringIsNull)
    {
      if (emptyStringIsNull)
      {
        if (obj1 == null)
          obj1 = string.Empty;
        if (obj2 == null)
          obj2 = string.Empty;
      }

      if (ReferenceEquals(obj1, obj2))
        return true;

      if (obj1 == null)
        return obj2 == null;
      return obj1.Equals(obj2);
    }

    public static Type GetParameterType(this ParameterInfo pi)
    {
      try
      {
        var t = Type.GetType(string.IsNullOrEmpty(pi.AssemblyName)
                              ? pi.TypeName
                              : string.Format("{0}, {1}",
                                              pi.TypeName,
                                              pi.AssemblyName));

        return !pi.IsNullable ? t : t.GetNullableType();
      }
      catch 
      {
        throw new ArgumentException(Resources.WRONG_PARAMETER_TYPE_DESCRIPTION, pi.Id);
      }
    }

    #endregion
  }
}
