using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Configuration;
using Toolbox.Log;

namespace Toolbox.GUI.Base
{
  /// <summary>
  /// Интерфейс для страницы свойств
  /// </summary>
  public interface IPropertyPage
  {
    /// <summary>
    /// Группа настроек
    /// </summary>
    string Group { get; }

    /// <summary>
    /// Название страницы свойств
    /// </summary>
    string PageName { get; }

    /// <summary>
    /// Иконка в дереве свойств
    /// </summary>
    Image Image { get; }

    /// <summary>
    /// Контрол — страница свойств
    /// </summary>
    Control Page { get; }

    /// <summary>
    /// Производить валидацию в потоке пользовательского интерфейса или в фоновом
    /// </summary>
    bool UIThreadValidation { get; }

    /// <summary>
    /// Требуется ли перезагрузка программы для применения настроек
    /// </summary>
    bool RestartRequired { get; }

    /// <summary>
    /// Проверка того, что страница является источником сообщения
    /// </summary>
    /// <param name="info">Сообщение</param>
    /// <returns></returns>
    bool IsSourceOfMessage([NotNull] Info info);

    /// <summary>
    /// Проверка корректности настроек
    /// </summary>
    /// <param name="Messages"></param>
    /// <returns></returns>
    bool ValidateSetting([NotNull] InfoBuffer Messages);

    /// <summary>
    /// Применение изменённых настроек
    /// </summary>
    void ApplySettings( );

    /// <summary>
    /// Загрузка текущих настроек
    /// </summary>
    void GetCurrentSettings( );

    event EventHandler Changed;
  }

  /// <summary>
  /// Привязывает конфигурационную секцию к компоненту BidningSource на форме
  /// </summary>
  /// <typeparam name="TSection">Тип привязываемой конфигурационной секции</typeparam>
  public sealed class SettingsBindingSource<TSection>
    where TSection : ConfigurationSection, new()
  {
    private readonly IConfigurator m_configurator;
    private readonly BindingSource m_binding_source;
    private bool m_getting_settings;

    public SettingsBindingSource([NotNull] BindingSource bindingSource)
      : this(AppManager.Configurator, bindingSource) { }

    public SettingsBindingSource([NotNull] IConfigurator configurator, [NotNull] BindingSource bindingSource)
    {
      if (configurator == null)
        throw new ArgumentNullException("configurator");

      if (bindingSource == null)
        throw new ArgumentNullException("bindingSource");

      if (bindingSource.DataSource != null)
      {
        var type = bindingSource.DataSource as Type;

        if (type == null)
          type = bindingSource.DataSource.GetType();

        if (type != typeof(TSection))
          throw new ArgumentOutOfRangeException("bindingSource", type.FullName,
						string.Format("bindingSource.DataSource {0} expected", typeof(TSection).FullName));
      }

      m_configurator = configurator;
      m_binding_source = bindingSource;
      m_binding_source.DataSourceChanged += this.HandleDataSourceChanged;
    }

    /// <summary>
    /// Экземпляр редактируемой конфигурационной секции
    /// </summary>
    public TSection EditingSection
    {
      get { return m_binding_source.DataSource as TSection; }
    }

    /// <summary>
    /// Помещение конфигурационной секции на форму
    /// </summary>
    public void GetCurrentSettings()
    {
      m_getting_settings = true;
      try
      {
        m_binding_source.DataSource = this.CreateSectionCopy();
      }
      finally
      {
        m_getting_settings = false;
      }
    }

    /// <summary>
    /// Сохранение отредактированного экземпляра секции в конфигуратор
    /// </summary>
    public void AssignSettings()
    {
      if (this.EditingSection == null)
				throw new InvalidOperationException("EditingSection = null");

      m_configurator.SaveSection(this.EditingSection);
    }

		/// <summary>
		/// Восстановление настроек по умолчанию.
		/// </summary>
		public void RestoreDefaults()
	  {
			this.EditingSection.RestoreDefaults();
			this.m_binding_source.ResetBindings(false);
	  }

    private void HandleDataSourceChanged(object sender, EventArgs e)
    {
      if (!m_getting_settings)
				throw new InvalidOperationException("¬m_getting_settings");
    }

    private TSection CreateSectionCopy()
    {
      var original = m_configurator.GetSection<TSection>();

      using (var ms = new MemoryStream())
      {
        if (typeof(TSection).IsDefined(typeof(DataContractAttribute), false))
        {
          var ser = new DataContractSerializer(typeof(TSection));

          ser.WriteObject(ms, original);

          ms.Position = 0;

          return (TSection)ser.ReadObject(ms);
        }
        else
        {
          var ser = new XmlSerializer(typeof(TSection));

          ser.Serialize(ms, original);

          ms.Position = 0;

          return (TSection)ser.Deserialize(ms);
        }
      }
    }
  }
}