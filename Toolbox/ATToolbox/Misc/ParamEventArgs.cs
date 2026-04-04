namespace AT.Toolbox
{
  using System;


  /// <summary>
  /// Шаблон, позволяюший передавать объект любого класса в качестве параметра для сообщения
  /// </summary>
  /// <typeparam name="T">Класс</typeparam>
  public class ParamEventArgs<T> : EventArgs
  {
    #region Protected Variables -------------------------------------------------------------------------------------------------


    #endregion


    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public ParamEventArgs( ) { Value = default( T ); }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="data">Объект класса для передачи</param>
    public ParamEventArgs( T data ) { Value = data; }


    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Значение
    /// </summary>
    public T Value { get; set; }

    #endregion
  }
}