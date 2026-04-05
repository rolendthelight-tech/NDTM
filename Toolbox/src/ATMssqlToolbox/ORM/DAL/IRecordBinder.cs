using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает привязку к внешнему источнику данных
  /// </summary>
  public interface IRecordBinder : IDisposable
  {
    /// <summary>
    /// Вычисляет значение поля, полученного из источника данных
    /// </summary>
    /// <typeparam name="TFieldType">Тип поля</typeparam>
    /// <param name="fieldNumber">Порядковый номер поля</param>
    /// <returns>Значение поля</returns>
    object GetFieldValue(int fieldNumber);

    /// <summary>
    /// Открывает доступ к источнику данных на чтение одной записи по ключу
    /// </summary>
    /// <param name="tableName">Имя таблицы, из которой приготовляются читать данные</param>
    /// <param name="keyFieldValues">Значения ключевых полей</param>
    /// <returns>Последовательность имен полей для чтения</returns>
    string[] OpenReader(string tableName, Dictionary<string, object> keyFieldValues);

    /// <summary>
    /// Открывает доступ к источнику данных на чтение множества записей по запросу
    /// </summary>
    /// <param name="tableName">Имя таблицы, из которой приготовляются читать данные</param>
    /// <param name="query">Запрос</param>
    /// <returns>Последовательность имен полей для чтения</returns>
    string[] OpenReader(string tableName, QueryObject query);

    /// <summary>
    /// Перемещается на очередную позицию в таблице
    /// </summary>
    /// <returns>True, если не достигнут конец таблицы</returns>
    bool Next();

    /// <summary>
    /// Закрывает доступ к источнику данных и освобождает ресурс
    /// </summary>
    void CloseReader();

    /// <summary>
    /// Вставляет новую запись в источник данных
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="fieldValues">Значения полей</param>
    /// <param name="outputFieldNames">Имена полей, значения которых инициализируются при вставке</param>
    /// <returns>True, если запись вставлена</returns>
    bool Insert(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames);

    /// <summary>
    /// Обновляет существующую запись в источнике данных
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="fieldValues">Значения полей</param>
    /// <param name="keyFieldValues">Значения ключевых полей оригинальной записи</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool Update(string tableName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues);

    /// <summary>
    /// Удаляет запись из источника данных
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="keyFieldValues">Значения ключевых полей</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool Delete(string tableName, Dictionary<string, object> keyFieldValues);

    /// <summary>
    /// Обновляет множество записей за один запрос
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="query">Запрос</param>
    /// <param name="parameters">Значения ключевых полей</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool UpdateRecordset(string tableName, QueryObject query, Dictionary<string, object> parameters);

    /// <summary>
    /// Удаляет множество записей за один запрос
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="query">Запрос</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool DeleteRecordSet(string tableName, QueryObject query);

    //string Identifier { get; }

    void BeginTransaction(IsolationLevel iso);
    void CommitTransaction();
    void RollbackTransaction();
  }
}