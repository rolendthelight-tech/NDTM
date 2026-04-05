using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using log4net;

namespace Toolbox.Configuration
{
  public interface IConfigurator
  {
    bool LoadSection([NotNull] Type sectionType, [NotNull] InfoBuffer buffer);

	  [NotNull]
	  TSection GetSection<TSection>() where TSection : ConfigurationSection, new();

    void SaveSection<TSection>([NotNull] TSection section) where TSection : ConfigurationSection, new();

    void HandleError([NotNull] Exception error);

    void ApplySettings();

    void SaveSettings();
  }

  [Serializable]
  [DataContract]
  public abstract class ConfigurationSection
  {
	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof(ConfigurationSection));

    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      try
      {
        this.LoadDefaults();
      }
      catch (Exception ex)
      {
        _log.Error("OnDeserializing(): Exception", ex);
      }
    }

	  [OnDeserialized]
		private void OnDeserialized(StreamingContext context)
	  {
			try
			{
				var buffer = new InfoBuffer();

				this.ValidateAndRepair(buffer);

				//TODO: buffer
			}
			catch (Exception ex)
			{
				_log.Error("OnDeserialized(): Exception", ex);
			}
		}

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      this.BeforeSave();
    }

    /// <summary>
    /// Загрузка значений по умолчанию.
    /// </summary>
    protected virtual void LoadDefaults()
    {
      LoadDefaults(this);
    }

    /// <summary>
		/// Загрузка значений по умолчанию.
		/// </summary>
    protected virtual void LoadDefaults([NotNull] object component)
    {
	    if (component == null) throw new ArgumentNullException("component");

			foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(component))
      {
        if (pd.IsReadOnly)
          continue;

        var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

        if (def != null)
          pd.SetValue(component, def.Value);
        else if (pd.PropertyType.IsClass)
        {
          var nestedComponent = pd.GetValue(component);
          if (nestedComponent != null)
            LoadDefaults(nestedComponent);
        }
      }
    }

	  /// <summary>
	  /// Загрузка значения по умолчанию для указанного свойства.
	  /// </summary>
	  protected virtual void LoadDefault([NotNull] string propertyName)
	  {
		  if (propertyName == null) throw new ArgumentNullException("propertyName");

		  foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this))
		  {
			  if (!string.Equals(pd.Name, propertyName) || pd.IsReadOnly)
				  continue;

			  var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

			  if (def != null)
				  pd.SetValue(this, def.Value);
		  }
	  }

	  /// <summary>
		/// Восстановление настроек по умолчанию.
		/// </summary>
		public void RestoreDefaults()
		{
			_log.Info("RestoreDefaults(): Восстановление настроек по умолчанию");
			LoadDefaults();
		}

		/// <summary>
		/// Проверка корректности настроек.
		/// Не изменяет настройки.
		/// </summary>
		/// <param name="buffer">Буфер сообщений.</param>
		/// <returns>Настройки корректны.</returns>
    public virtual bool Validate([NotNull] InfoBuffer buffer)
    {
	    if (buffer == null) throw new ArgumentNullException("buffer");
	    //TODO: имеет смысл в базовом классе писать предупреждение в log4net, не выводя его пользователю
      return true;
    }

		/// <summary>
		/// Исправление настроек до корректного состояния.
		/// Этот метод проверяет те же условия, которые в <see cref="Validate(InfoBuffer)"/> приводят к возврату <code>false</code>.
		/// Этот метод не меняет корректные настройки (например, вызовом <see cref="LoadDefaults()"/>).
		/// </summary>
		/// <param name="buffer">Буфер сообщений.</param>
		/// <returns>Удалось исправить.</returns>
		protected virtual bool Repair([NotNull] InfoBuffer buffer)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");

      return true;
		}

		private bool ValidateAndRepair([NotNull] InfoBuffer buffer)
	  {
			if (buffer == null) throw new ArgumentNullException("buffer");

			bool res;
			try
			{
				res = Validate(buffer);
			}
			catch (Exception ex)
			{
				res = false;
				_log.Error("ValidateAndRepair(): exception", ex);
				buffer.Add("Ошибка при проверке параметров", InfoLevel.Error, details: ex);
			}
			if (!res)
			{
				buffer.Add("Запущено исправление настроек", InfoLevel.Info);
				res = Repair(buffer);
			}

			return res;
	  }

	  public virtual void ApplySettings()
    {
    }

    protected ConfigurationSection()
    {
      this.LoadDefaults();
    }

    protected internal virtual void OnError(Exception error)
    {
    }

    protected virtual void BeforeSave()
    {
    }
  }
}
