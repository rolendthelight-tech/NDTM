using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
  public static class ReflectionExtensions
  {
    #region Reflection helpers

    /// <summary>
    /// Возвращает экземпляр атрибута у элемента
    /// </summary>
    /// <typeparam name="TAttribute">Тип атрибута</typeparam>
    /// <param name="item">Элемент</param>
    /// <param name="inherit">Указывает, нужно ли искать атрибут в предках</param>
    /// <returns>Если имеется единственный экземпляр, то его. Иначе, <code>null</code></returns>
    [CanBeNull]
		[Pure]
		public static TAttribute GetCustomAttribute<TAttribute>([NotNull] this MemberInfo item, bool inherit)
      where TAttribute : Attribute
    {
	    if (item == null) throw new ArgumentNullException("item");

	    if (item.IsDefined(typeof(TAttribute), inherit))
      {
        return item.GetCustomAttributes(typeof(TAttribute), inherit)[0] as TAttribute;
      }

      return null;
    }

    /// <summary>
    /// Возвращает экземпляр атрибута у сборки, содержащей указанный тип
    /// </summary>
    /// <typeparam name="TAttribute">Тип атрибута</typeparam>
    /// <param name="type">тип</param>
    /// <returns>Если имеется единственный экземпляр, то его. Если экземпляров нет, то <code>null</code>. Иначе бросает исключение</returns>
    [CanBeNull]
		[Pure]
		public static TAttribute GetCustomAssemblyAttribute<TAttribute>([NotNull] this Type type)
      where TAttribute : Attribute
    {
	    if (type == null) throw new ArgumentNullException("type");

	    var attrs = type.Assembly.GetCustomAttributes(typeof (TAttribute), false);

	    try
      {
				return (TAttribute)(attrs.SingleOrDefault());
      }
			catch (InvalidOperationException ex)
      {
				throw new InvalidOperationException(string.Format("У сборки {1}, содержащей тип {0}, слишком много атрибутов типа {2}", type, type.Assembly, typeof(TAttribute)), ex);
      }
    }

	  /// <summary>
    /// Проверяет тип на допустимость присвоения <code>null</code>
    /// </summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns><code>true</code>, если тип допускает присвоение <code>null</code></returns>
		[Pure]
		public static bool IsNullable([NotNull] this Type type)
	  {
		  if (type == null) throw new ArgumentNullException("type");

		  return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
	  }

	  /// <summary>
    /// Возвращает не-nullable тип
    /// </summary>
    /// <param name="type">Произвольный тип</param>
    /// <returns>Тип, приведённый к не-nullable виду</returns>
	  [NotNull]
		[Pure]
		public static Type GetClearType([NotNull] this Type type)
    {
		  if (type == null) throw new ArgumentNullException("type");

			//if (type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			//	return type.GetGenericArguments()[0];
			//return type;

			return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    /// Проверяет, является ли тип числовым
    /// </summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns><code>true</code>, если тип относится к числовым типам</returns>
		[Pure]
		public static bool IsNumeric([NotNull] this Type type)
    {
	    if (type == null) throw new ArgumentNullException("type");

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
	  [NotNull]
		[Pure]
		public static Type GetNullableType([NotNull] this Type type)
    {
		  if (type == null) throw new ArgumentNullException("type");

		  if (type == typeof(void))
				throw new ArgumentException("There is no Nullable version of void", "type"); // Не существует There is no Nullable version of void

      return IsNullable(type) ? type : typeof(Nullable<>).MakeGenericType(type);
    }

	  [NotNull]
		[Pure]
		public static string GetDisplayName([NotNull] this MemberInfo member)
    {
      if (member == null) throw new ArgumentNullException("member");

	    string display_name = null;

      if (member.IsDefined(typeof(DisplayNameAttribute), true))
        display_name = member.GetCustomAttribute<DisplayNameAttribute>(true).DisplayName;

	    return display_name ?? member.Name;
    }

	  [NotNull]
		[Pure]
		public static string GetCategory([NotNull] this MemberInfo member)
    {
	    if (member == null) throw new ArgumentNullException("member");

	    string display_name = null;

      if (member.IsDefined(typeof(CategoryAttribute), true))
        display_name = member.GetCustomAttribute<CategoryAttribute>(true).Category;

      return display_name ?? "Misc";
    }

    #endregion
  }
}
