using System;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;
using Toolbox.Extensions;
using log4net;

namespace Toolbox.Common
{
	/// <summary>
	/// Локализованная метка для свойства, поля, элемента перечисления или типа
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
	public class DisplayNameFromResourceAttribute : DisplayNameAttribute
	{
		[NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(DisplayNameFromResourceAttribute));
		[NotNull] private static readonly object[] _empty_array = ArrayExtensions.Empty<object>();
		private readonly Type m_resource_type;
		private readonly string m_resource_name;
		private ResourceManager m_resource_manager;

		/// <summary>
		/// Инициализирует новую метку
		/// </summary>
		/// <param name="resourceType">Тип ресурса</param>
		/// <param name="resourceName">Имя ресурса</param>
		public DisplayNameFromResourceAttribute([NotNull] Type resourceType, [Localizable(false)] [NotNull] string resourceName)
			: base()
		{
			if (resourceType == null) throw new ArgumentNullException("resourceType");
			if (resourceName == null) throw new ArgumentNullException("resourceName");

			this.m_resource_type = resourceType;
			this.m_resource_name = resourceName;
		}

		[CanBeNull]
		private ResourceManager CachedByTypeResourceManager
		{
			get
			{
				try
				{
					var pi = this.m_resource_type.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.NonPublic/* Некоторые internal */ | BindingFlags.Static);
					if (pi == null)
					{
						_log.WarnFormat("CachedByTypeResourceManager.get: ResourceManager не найден у типа {0}", this.m_resource_type);
						return null;
					}
					else
					{
						ResourceManager resource_manager = pi.GetValue(null, _empty_array) as ResourceManager;
						if (resource_manager == null)
							_log.WarnFormat("CachedByTypeResourceManager.get: ResourceManager не получен у типа {0}", this.m_resource_type);
						return resource_manager;
					}
				}
				catch (Exception ex)
				{
					_log.Error("CachedByTypeResourceManager.get: exception", ex);
					return null;
				}
			}
		}

		[NotNull]
		private ResourceManager ResourceManager
		{
			get
			{
				return LazyInitializer.EnsureInitialized(ref this.m_resource_manager, () => this.CachedByTypeResourceManager ?? new ResourceManager(this.m_resource_type)).ThrowIfNull("m_resource_manager");
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
		/// Если ресурс найден, возвращает локализованный ресурс, иначе — бросает исключение <see cref="ResourceNotFound"/>
		/// </summary>
		[NotNull]
		public override string DisplayName
		{
			get
			{
				try
				{
					var resource_manager = this.ResourceManager.ThrowIfNull("ResourceManager");
					string resource = resource_manager.GetString(this.m_resource_name).ThrowIfNull("resource");
					return resource;
				}
				catch (Exception ex)
				{
					throw new ResourceNotFound(this.m_resource_type, this.m_resource_name, ex);
				}
			}
		}

		/// <summary>
		/// Сравнивает атрибут с переданным объектом
		/// </summary>
		/// <param name="obj">Сравниваемый объект</param>
		/// <returns><code>true</code>, если переданный объект является таким же</returns>
		public override bool Equals(object obj)
		{
			DisplayNameFromResourceAttribute res = obj as DisplayNameFromResourceAttribute;
			if (res == this)
				return true;

			if (res != null)
			{
				return (res.m_resource_type == this.m_resource_type) && (res.m_resource_name == this.m_resource_name);
			}
			return false;
		}

		/// <summary>
		/// Возвращает числовой идентификатор
		/// </summary>
		/// <returns>Числовой идентификатор из имени типа и ресурса</returns>
		public override int GetHashCode()
		{
			return GetHashCodeHelper.CombineHashCodes<object>(2020880453, 1474111649, this.m_resource_type, this.m_resource_name);
		}

		[Serializable]
		public class ResourceNotFound : Exception
		{
			public ResourceNotFound([NotNull] Type resourceType, [NotNull] string resourceName)
				: base(GetMessage(resourceType, resourceName))
			{
				if (resourceType == null) throw new ArgumentNullException("resourceType");
				if (resourceName == null) throw new ArgumentNullException("resourceName");
			}

			public ResourceNotFound([NotNull] Type resourceType, [NotNull] string resourceName, Exception innerException)
				: base(GetMessage(resourceType, resourceName), innerException)
			{
				if (resourceType == null) throw new ArgumentNullException("resourceType");
				if (resourceName == null) throw new ArgumentNullException("resourceName");
			}

			protected ResourceNotFound([NotNull] SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}

			[NotNull]
			private static string GetMessage([NotNull] Type resourceType, [NotNull] string resourceName)
			{
				if (resourceType == null) throw new ArgumentNullException("resourceType");
				if (resourceName == null) throw new ArgumentNullException("resourceName");

				return string.Format("Ресурс {0}.{1} не найден", resourceType, resourceName).ThrowIfNull("message");
			}
		}
	}
}