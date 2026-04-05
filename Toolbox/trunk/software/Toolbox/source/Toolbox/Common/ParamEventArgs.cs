using System;

namespace Toolbox.Common
{
	/// <summary>
  /// Шаблон, позволяюший передавать объект любого класса в качестве параметра для сообщения
  /// </summary>
  /// <typeparam name="T">Класс</typeparam>
  public class ParamEventArgs<T> : EventArgs
  {
		private readonly T m_value;

		#region Protected Variables -------------------------------------------------------------------------------------------------

    #endregion

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public ParamEventArgs( ) :this( default( T )){}

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="data">Объект класса для передачи</param>
    public ParamEventArgs( T data ) { m_value = data; }

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Значение
    /// </summary>
    public T Value
    {
	    get { return m_value; }
    }

		#endregion
  }
}