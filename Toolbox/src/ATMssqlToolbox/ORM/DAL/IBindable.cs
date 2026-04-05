using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает операции по привязке данных для единичного объекта
  /// </summary>
  public interface IBindable : ICloneable, IDisposable
  {
    /// <summary>
    /// Загрузка данных из базы по первичному ключу
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool DataBind();

    /// <summary>
    /// Выполняет начальную инициализацию объекта
    /// </summary>
    /// <returns></returns>
    void InitValues();

    /// <summary>
    /// Вставляет запись в базу данных
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool Insert();

    /// <summary>
    /// Обновляет запись в базе данных
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool Update();

    /// <summary>
    /// Удаляет запись из базы данных
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool Delete();

    /// <summary>
    /// Проверяет запись перед вставкой или обновлением
    /// </summary>
    /// <returns>True, если запись может быть вставлена или обновлена</returns>
    bool ValidateWrite(InfoEventArgs messageBuffer);

    /// <summary>
    /// Проверяет запись перед удалением
    /// </summary>
    /// <returns>True, если запись может быть удалена</returns>
    bool ValidateDelete(InfoEventArgs messageBuffer);

    /// <summary>
    /// Инициализирует курсор для прохода по набору записей
    /// </summary>
    /// <param name="query">Запрос, по которому возвращается набор</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool InitReader(QueryObject query);

    /// <summary>
    /// Читает очередную запись из набора и инициализирует поля
    /// </summary>
    /// <returns>True, если не достигнут конец набора записей</returns>
    bool Next();

    /// <summary>
    /// Прерывает проход по набору записей
    /// </summary>
    void Break();

    /// <summary>
    /// Возвращает и устанавливает значение поля, связанного с базой данных
    /// </summary>
    /// <param name="fieldName">Имя поля в базе данных</param>
    /// <returns>Значение поля</returns>
    object this[string fieldName] { get; set; }

    /// <summary>
    /// Признак того, что объект привязан к базе данных
    /// </summary>
    bool _IsDataBound { get; }

    /// <summary>
    /// Признак того, что объект был модифицирован после создания или последней привязки
    /// </summary>
    bool _Modified { get; }

    /// <summary>
    /// Имя поля, используемого в качестве первичного ключа. Если ключ составной, всегда null
    /// </summary>
    string _KeyFieldName { get; }

    /// <summary>
    /// Имя таблицы в базе данных
    /// </summary>
    string _TableName { get; }

    /// <summary>
    /// Статус бизнес-объекта по отношению к кэшу
    /// </summary>
    CacheStatus _CacheStatus { get; set; }
  }
}