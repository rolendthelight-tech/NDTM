using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.ComponentModel;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Common
{
	/// <summary>
	/// Локализуемая метка для свойства, поля, элемента перечисления или типа
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
	[Obsolete("Этот класс использует ресурс или его имя. Используйте DisplayNameAttribute или DisplayName2Attribute, если ресурса нет, и DisplayNameFromResourceAttribute — если есть")]
	public class DisplayNameResAttribute : DisplayName2Attribute
  {
	  private static readonly ILog _log = LogManager.GetLogger(typeof (DisplayNameResAttribute));

    /// <summary>
    /// Инициализирует новую метку
    /// </summary>
    /// <param name="resourceName">Имя ресурса</param>
    /// <param name="targetType">Тип, к которому применён атрибут, находящийся в той же сборке, что и ресурс</param>
    public DisplayNameResAttribute([NotNull] string resourceName, [NotNull] Type targetType)
      : base(resourceName)
    {
	    if (resourceName == null) throw new ArgumentNullException("resourceName");
	    if (targetType == null) throw new ArgumentNullException("targetType");

      this.TargetType = targetType;
    }

    /// <summary>
    /// Тип, к которому применён атрибут
    /// </summary>
    public Type TargetType { get; private set; }

    /// <summary>
    /// Если ресурс найден, возвращает локализованный ресурс. Иначе, возвращает имя ресурса
    /// </summary>
    public override string DisplayName
    {
      get
      {
	      try
	      {
		      string[] resource_names = this.TargetType.Assembly.GetManifestResourceNames();
		      foreach (string resource_root in resource_names)
		      {
			      string base_name = resource_root.Replace(".resources", "");
			      if (base_name.EndsWith("." + this.TargetType.Name) || base_name == this.TargetType.Name)
			      {
				      return new ResourceManager(base_name, this.TargetType.Assembly).GetString(base.DisplayNameValue);
			      }
		      }
		      foreach (string resource_root in resource_names)
		      {
			      string base_name = resource_root.Replace(".resources", "");
			      string resource = new ResourceManager(base_name, this.TargetType.Assembly).GetString(base.DisplayNameValue);
			      if (resource != null)
			      {
				      return resource;
			      }
		      }
	      }
	      catch (Exception ex)
	      {
		      _log.Error("DisplayName.get(): exception", ex);
	      }
				_log.WarnFormat("DisplayName.get(): ресурс с полем \"{0}\" не найден", base.DisplayNameValue);
				return base.DisplayNameValue;
      }
    }

    /// <summary>
    /// Этот атрибут не может быть атрибутом по умолчанию
    /// </summary>
    /// <returns><code>false</code></returns>
    public override bool IsDefaultAttribute()
    {
      return false;
    }

    /// <summary>
    /// Сравнивает атрибут с переданным объектом
    /// </summary>
    /// <param name="obj">Сравниваемый объект</param>
    /// <returns><code>true</code>, если переданный объект является таким же</returns>
    public override bool Equals(object obj)
    {
      DisplayNameResAttribute res = obj as DisplayNameResAttribute;
      if (res == this)
        return true;

      if (res != null)
      {
        return (res.DisplayNameValue == this.DisplayNameValue) && (res.TargetType == this.TargetType);
      }
      return false;
    }

    /// <summary>
    /// Возвращает числовой идентификатор
    /// </summary>
    /// <returns>Числовой идентификатор из имени ресурса и типа, к которому применён атрибут</returns>
    public override int GetHashCode()
    {
	    return GetHashCodeHelper.CombineHashCodes<object>(830781503, 1568581429, this.DisplayNameValue, this.TargetType);
    }
  }
}
