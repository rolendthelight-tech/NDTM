namespace AT.Toolbox
{
  using System;


  /// <summary>
  /// Интерфейс для форм и объектов, реализующих смену языка на лету
  /// Локализация реализуется аналогично DevExpress -- через установку значений напрямую из Satellite Assemblies
  /// </summary>
  public interface ISwitchableLanguage : IDisposable
  {
    /// <summary>
    /// Инициализация. Подписка на события, 
    /// </summary>
    void InitLocalization( );

    /// <summary>
    /// Выполнение локализации -- подгрузка значений из ресурсов
    /// </summary>
    /// <param name="sender">Нагрузки не несёт</param>
    /// <param name="e">Нагрузки не несёт</param>
    void PerformLocalization( object sender, EventArgs e );

    /// <summary>
    /// Сообщение о том, что объект разрушается и сообщения о смене языка посылать больше не нужно
    /// </summary>
    event EventHandler Terminating;
  }
}