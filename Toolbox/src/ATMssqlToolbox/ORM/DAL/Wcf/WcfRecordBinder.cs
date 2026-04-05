using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Wcf
{
  /// <summary>
  /// Привязчик для работы с базой через сервис
  /// </summary>
  public class WcfRecordBinder : IRecordBinder
  {
    #region Fields

    private IRecordManagementService m_record_management_service;
    PackedMatrix m_current_buffer;
    int m_current_position;

    #endregion

    #region Constructors

    public WcfRecordBinder(IRecordManagementService record_management_service)
    {
      Debug.Assert(record_management_service != null);
      m_record_management_service = record_management_service;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Хранимые процедуры, назначенные стандартным действиям
    /// </summary>
    public Dictionary<BindingAction, string> BindingActions { get; set; }

    #endregion

    #region IRecordBinder Members

    /// <summary>
    /// Возвращаем поле из продольной матрицы
    /// </summary>
    /// <param name="fieldNumber">Номер поля</param>
    /// <returns>Значение поля</returns>
    public object GetFieldValue(int fieldNumber)
    {
      if (m_current_buffer == null)
        throw new InvalidOperationException();

      object ret = m_current_buffer.Data[fieldNumber].GetValue(m_current_position);
      if (ret == Convert.DBNull)
        ret = null;

      return ret;
    }

    /// <summary>
    /// В трёхзвенке работаем отсоединённо: закачали всё в продольную матрицу,
    /// потом имитируем DataReader
    /// </summary>
    /// <param name="tableName">Тия таблицы</param>
    /// <param name="keyFieldValues">Значения ключевых полей</param>
    /// <returns>Имена полей</returns>
    public string[] OpenReader(string tableName, Dictionary<string, object> keyFieldValues)
    {
      if (this.BindingActions.ContainsKey(BindingAction.Get))
      {
        this.CloseReader();

        using (MemoryStream ms = new MemoryStream(m_record_management_service.ExecuteReaderCommand(CommandType.StoredProcedure,
          this.BindingActions[BindingAction.Get], keyFieldValues)))
        {
          m_current_buffer = (PackedMatrix)(new BinaryFormatter().Deserialize(ms));

          return m_current_buffer.Headers;
        }
      }
      else
      {
        QueryObject qo = QueryObject.CreateFromFieldValues(keyFieldValues);
        return this.OpenReader(tableName, qo);
      }
    }

    /// <summary>
    /// В трёхзвенке работаем отсоединённо: закачали всё в продольную матрицу,
    /// потом имитируем DataReader
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="query">Запрос</param>
    /// <returns>Имена полей</returns>
    public string[] OpenReader(string tableName, QueryObject query)
    {
      this.CloseReader();

      using (MemoryStream ms2 = new MemoryStream(m_record_management_service.InitReader(tableName, this.PackQuery(query))))
      {
        m_current_buffer = (PackedMatrix)(new BinaryFormatter().Deserialize(ms2));
        return m_current_buffer.Headers;
      }
    }

    /// <summary>
    /// Упаковывает запрос в бинарник
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <returns>Упакованный в байтовый массив запрос</returns>
    private byte[] PackQuery(QueryObject query)
    {
      byte[] packedQuery = null;
      using (MemoryStream ms = new MemoryStream())
      {
        new BinaryFormatter().Serialize(ms, query);
        packedQuery = ms.ToArray();
      }
      return packedQuery;
    }

    /// <summary>
    /// Переводит позицию на следующий индекс в продольной матрице
    /// </summary>
    /// <returns>True, если матрица ещё не кончилась</returns>
    public bool Next()
    {
      if (m_current_buffer == null)
        throw new InvalidOperationException();

      m_current_position++;

      return m_current_position < m_current_buffer.GetLength();
    }

    /// <summary>
    /// Здесь достаточно просто обнулить матрицу и позицию
    /// </summary>
    public void CloseReader()
    {
      m_current_position = -1;
      m_current_buffer = null;
    }


    /// <summary>
    /// Вставляет данные через сервер приложения
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <param name="fieldValues">Значения полей</param>
    /// <param name="outputFieldNames">Имена выходных полей</param>
    /// <returns>True, если запись вставилась</returns>
    public bool Insert(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      BindingResult res = m_record_management_service.Insert(tableName, fieldValues, outputFieldNames);

      if (res.Messages.Count != 0)
      {
        InfoEventArgs args = new InfoEventArgs();

        foreach (Info info in res.Messages)
          args.Messages.Add(info);

        RecordManager.SendMessageList(args);
      }
      return res.Success;
    }

    public bool Update(string tableName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues)
    {
      BindingResult res = m_record_management_service.Update(tableName, fieldValues, keyFieldValues);

      if (res.Messages.Count != 0)
      {
        InfoEventArgs args = new InfoEventArgs();

        foreach (Info info in res.Messages)
          args.Messages.Add(info);

        RecordManager.SendMessageList(args);
      }
      return res.Success;
    }

    public bool Delete(string tableName, Dictionary<string, object> keyFieldValues)
    {
      BindingResult res = m_record_management_service.Delete(tableName, keyFieldValues);

      if (res.Messages.Count != 0)
      {
        InfoEventArgs args = new InfoEventArgs();

        foreach (Info info in res.Messages)
          args.Messages.Add(info);

        RecordManager.SendMessageList(args);
      }
      return res.Success;
    }

    public bool UpdateRecordset(string tableName, QueryObject query, Dictionary<string, object> parameters)
    {
      BindingResult res = m_record_management_service.UpdateRecordset(tableName, this.PackQuery(query), parameters);

      if (res.Messages.Count != 0)
      {
        InfoEventArgs args = new InfoEventArgs();

        foreach (Info info in res.Messages)
          args.Messages.Add(info);

        RecordManager.SendMessageList(args);
      }
      return res.Success;
    }

    public bool DeleteRecordSet(string tableName, QueryObject query)
    {
      BindingResult res = m_record_management_service.DeleteRecordSet(tableName, this.PackQuery(query));

      if (res.Messages.Count != 0)
      {
        InfoEventArgs args = new InfoEventArgs();

        foreach (Info info in res.Messages)
          args.Messages.Add(info);

        RecordManager.SendMessageList(args);
      }
      return res.Success;
    }

    #endregion

    #region Transaction management

    /// <summary>
    /// Если бизнес-логика хочет что-то делать в транзакции, не мешаем
    /// </summary>
    /// <param name="iso"></param>
    void IRecordBinder.BeginTransaction(System.Data.IsolationLevel iso)
    {
    }

    /// <summary>
    /// Закрывать транзакцию тоже не мешаем
    /// </summary>
    void IRecordBinder.CommitTransaction()
    {
    }

    /// <summary>
    /// А вот если бизнес-логика хочет отменить транзакцию - 
    /// ну извините, в трёхзвенке на клиенте такое не прокатит!
    /// </summary>
    void IRecordBinder.RollbackTransaction()
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
    }

    #endregion
  }
}
