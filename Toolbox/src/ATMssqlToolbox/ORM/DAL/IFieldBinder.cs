using System;
using System.Collections.Generic;
using System.Text;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает функциональность привязчика для полей
  /// </summary>
  public interface IFieldBinder
  {
    /// <summary>
    /// Вычисляет значение поля
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldName">Имя поля</param>
    /// <returns>Значение поля</returns>
    object GetFieldValue(string fieldName);

    /// <summary>
    /// Устанавливает значение поля
    /// </summary>
    /// <param name="fieldName">Имя поля</param>
    /// <param name="newValue">Значение поля</param>
    void SetFieldValue(string fieldName, object newValue);

    /// <summary>
    /// Создает новый привязчик для другого объекта того же типа, что и текущий
    /// </summary>
    /// <param name="newObject">Новый объект для привязки</param>
    /// <returns>Новый привязчик</returns>
    IFieldBinder Clone(IBindable newObject);

    /// <summary>
    /// Осуществляет, если необходимо, согласование работы с привязчиком к базе данных
    /// </summary>
    /// <param name="recordBinder">Привязчик к источнику данных</param>
    bool ReadDataFields(string[] fieldNames, IRecordBinder recordBinder);
  }
}