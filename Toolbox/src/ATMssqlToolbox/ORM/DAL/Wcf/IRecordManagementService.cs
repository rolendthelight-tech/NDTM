using System;
using System.Collections.Generic;
using System.ServiceModel;
using AT.Toolbox.MSSQL.DAL.RecordBinding;
using System.Data;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Wcf
{
  /// <summary>
  /// Контракт для универсального сервиса по работе с бизнес-объектами
  /// </summary>
  [ServiceContract(Name = "RecordManagement")]
  public interface IRecordManagementService
  {
    /// <summary>
    /// Регистрирует клиента по его реквизитам
    /// </summary>
    /// <param name="userName">Имя пользователя</param>
    /// <param name="computerName">Имя компьютера</param>
    /// <param name="applicationName">Имя клиентского приложения</param>
    [OperationContract]
    void SendClientRequisites(string userName, string computerName, string applicationName);

    /// <summary>
    /// Проверяет актуальность таблицы
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>True, если таблица актуальна</returns>
    [OperationContract]
    bool CheckActuality(string tableName, DateTime lastChange);

    /// <summary>
    /// Операция выполнения бизнес-логики на сервере
    /// </summary>
    /// <param name="package">Упакованный объект, над которым надо выполнить операцию</param>
    /// <param name="action">Действие, которое надо выполнить</param>
    /// <returns>Результат выполнения операции</returns>
    [OperationContract]
    BindingResult GetResult(Package package, BindingAction action);

    /// <summary>
    /// Возвращает упакованную в бинарный массив матрицу данных,
    /// которую вернула переданная команда
    /// </summary>
    /// <param name="commandType">Тип команды</param>
    /// <param name="commandText">Текст команды</param>
    /// <param name="parameters">Именованные параметры команды</param>
    /// <returns>Продольная матрица, упакованная в байтовый массив</returns>
    [OperationContract]
    byte[] ExecuteReaderCommand(CommandType cmd, string commandText, Dictionary<string, object> parameters);

    /// <summary>
    /// Возвращает упакованную в бинарный массив матрицу данных,
    /// которую вернул запрос к указанной таблице
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="packedQuery">Упакованный в бинарный массив запрос</param>
    /// <returns>Упкаованный массив</returns>
    [OperationContract]
    byte[] InitReader(string tableName, byte[] packedQuery);

    /// <summary>
    /// Возвращает массив бизнес-объектов, который вернул переданный запрос
    /// </summary>
    /// <param name="tableName">Имя таблицы, по которому определяется тип бизнес-объекта</param>
    /// <param name="packedQuery">Запрос, упакованный в байтовый массив</param>
    /// <returns>Упакованный в байтовый массив массив бизнес-объектов</returns>
    [OperationContract]
    byte[] GetReadyObjects(string tableName, byte[] packedQuery);

    /// <summary>
    /// Вставляет данные в таблицу
    /// </summary>
    /// <param name="tableName">Таблица, в которую надо вставить данные</param>
    /// <param name="fieldValues">Значения полей</param>
    /// <param name="outputFieldNames">Имена полей, которые идут на выход</param>
    /// <returns>Результат выполнения операции с заполненными выходными полями</returns>
    [OperationContract]
    BindingResult Insert(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames);

    /// <summary>
    /// Обновляет данные в таблице
    /// </summary>
    /// <param name="tableName">Таблица, в которой нужно обновить данные</param>
    /// <param name="fieldValues">Новые значения полей</param>
    /// <param name="keyFieldValues">Исходные значения ключевых полей</param>
    /// <returns>Результат выполнения операции</returns>
    [OperationContract]
    BindingResult Update(string tableName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues);

    /// <summary>
    /// Удаляет запись из таблицы
    /// </summary>
    /// <param name="tableName">Имя</param>
    /// <param name="keyFieldValues">Значения ключевых полей</param>
    /// <returns>Результат выполнения операции</returns>
    [OperationContract]
    BindingResult Delete(string tableName, Dictionary<string, object> keyFieldValues);

    /// <summary>
    /// Обновляет набор записей, удовлетворяющих запросу
    /// </summary>
    /// <param name="tableName">Имя таблицы, в которой надо обновить записи</param>
    /// <param name="packedQuery">Упакованный в байтовый массив запрос</param>
    /// <param name="parameters">Значения полей, которые нужно обновить</param>
    /// <returns>Результат выполнения операции</returns>
    [OperationContract]
    BindingResult UpdateRecordset(string tableName, byte[] packedQuery, Dictionary<string, object> parameters);

    /// <summary>
    /// Удаляет набор записей, удовлетворяющих запросу
    /// </summary>
    /// <param name="tableName">Имя таблицы, из которой нужно удалить записи</param>
    /// <param name="packedQuery">Упакованный в байтовый массив запрос</param>
    /// <returns>Результат выполнения операции</returns>
    [OperationContract]
    BindingResult DeleteRecordSet(string tableName, byte[] packedQuery);
  }
}