using System;
using System.Collections.Generic;
using AT.Toolbox.MSSQL.DAL.RecordBinding.Sql;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AT.Toolbox.MSSQL.ORM.DAL.Wcf;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Wcf
{
  /// <summary>
  /// Клиентская фабрика для привязки в трёхзвенке
  /// </summary>
  public class WcfRecordBinderFactory : IRecordBinderFactory
  {
    //string m_file_load_service_address;
    
    private IRecordManagementService m_record_management_service;
    private IRecordManagementService RecordManagementService
    {
      get
      {
        //if (m_record_management_service == null)
        //  m_record_management_service = CreateRecordManagementService();
        //else
        //{
        //  var channel = (ICommunicationObject) m_record_management_service;
        //  if (channel.State == CommunicationState.Faulted)
        //    m_record_management_service = CreateRecordManagementService();
        //}

        return m_record_management_service;
      }
    }

//    private IRecordManagementService CreateRecordManagementService()
//    {
//      var bng = new NetTcpBinding();
//      bng.MaxReceivedMessageSize = int.MaxValue;
//      bng.ReaderQuotas.MaxArrayLength = int.MaxValue;
//
//      var factory = new ChannelFactory<IRecordManagementService>(bng);
//      Debug.Assert(!string.IsNullOrEmpty(m_host_address));
//      m_record_management_service = factory.CreateChannel(new EndpointAddress(m_host_address));
//      m_record_management_service.SendClientRequisites(GetCurrentUserName(), GetComputerName(), GetApplicationName());
//      return m_record_management_service;
//    }

    readonly static Dictionary<string, KeyValuePair<DateTime, bool>> m_recent_checks
      = new Dictionary<string, KeyValuePair<DateTime, bool>>();

    public WcfRecordBinderFactory(string host_name, int base_port)
    {
      //string record_management_service_address = string.Format("net.tcp://{0}:{1}/RecordManagement", host_name, base_port);
      //m_file_load_service_address = string.Format("net.tcp://{0}:{1}/FileLoadService", host_name, base_port + 3);

      m_record_management_service = new RecordManagementServiceProxy(host_name, base_port);
      m_record_management_service.SendClientRequisites(GetCurrentUserName(), GetComputerName(), GetApplicationName());
    }

    #region IRecordBinderFactory Members

    public string GetTransactionStackId(IRecordBinder binder)
    {
      return "currentHost";
    }

    public IRecordBinder CreateRecordBinder(Dictionary<BindingAction, string> procedureActions)
    {
      WcfRecordBinder ret = new WcfRecordBinder(RecordManagementService);
      ret.BindingActions = procedureActions ?? new Dictionary<BindingAction, string>();
      return ret;
    }

    public BindingResult GetResult(IServiceRecord record, BindingAction action)
    {
      Package pack = new Package(record);

      //NetTcpBinding bng = new NetTcpBinding();
      //bng.MaxReceivedMessageSize = int.MaxValue;

      BindingResult res = RecordManagementService.GetResult(pack, action);
      return res;
    }


    /// <summary>
    /// Преобразователь объектной модели запроса пока один
    /// </summary>
    /// <returns></returns>
    public IQueryBuilder CreateQueryBuilder()
    {
      return new SqlQueryBuilder();
    }

    /// <summary>
    /// Лезем за информацией об актуальности на сервер, 
    /// но не чаще, чем раз в 2 секунды для каждой таблицы
    /// </summary>
    /// <param name="tableName">Имя таблицы</param>
    /// <returns>True, если данные актуальны</returns>
    public bool CheckActuality<TType>()
      where TType : class, IBindable, new()
    {
      string tableName = RecordManager.GetTableName<TType>();
      
      KeyValuePair<DateTime, bool> entry;

      if (m_recent_checks.ContainsKey(tableName))
      {
        entry = m_recent_checks[tableName];

        if (DateTime.Now < entry.Key.AddSeconds(2))
        {
          return entry.Value;
        }
      }

      entry = new KeyValuePair<DateTime, bool>(DateTime.Now,
        RecordManagementService.CheckActuality(tableName, RecordManager.GetLastInvalidation(tableName)));

      m_recent_checks[tableName] = entry;

      return entry.Value;
    }

    /// <summary>
    /// Поскольку на клиенте, возвращаем текущего пользователя Windows
    /// </summary>
    /// <returns></returns>
    public string GetCurrentUserName()
    {
      return string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName).ToLower();
    }

    public string GetComputerName()
    {
      return Environment.MachineName;
    }

    public string GetApplicationName()
    {
      return ApplicationInfo.MainAttributes.ProductName;
    }

    /// <summary>
    /// Обращается к серверу приложения и получает массив бизнес-объектов, удовлетворяющих запросу
    /// </summary>
    /// <param name="tableName">Имя таблицы, по которому ищется тип</param>
    /// <param name="query">Запрос</param>
    /// <returns></returns>
    public IEnumerable<IBindable> GetObjectList(string tableName, QueryObject query)
    {
      using (MemoryStream ms = new MemoryStream(RecordManagementService.GetReadyObjects(tableName, PackQuery(query))))
      {
        return (IEnumerable<IBindable>)(new BinaryFormatter().Deserialize(ms));
      }
    }

    /// <summary>
    /// Обращается к серверу приложения, запрашивает у него продольную матрицу,
    /// удовлетворяющую запросу, потом переводит её в DataTable и заполняет
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="table"></param>
    /// <param name="query"></param>
    public void FillDataTable(string tableName, DataTable table, QueryObject query)
    {
      table.Rows.Clear();
      table.Columns.Clear();

      using (MemoryStream ms2 = new MemoryStream(RecordManagementService.InitReader(tableName, this.PackQuery(query))))
      {
        // Получили с сервера продольную матрицу
        PackedMatrix buffer = (PackedMatrix)(new BinaryFormatter().Deserialize(ms2));

        // Сначала формируем колонки
        for (int i = 0; i < buffer.Headers.Length; i++)
        {
          Type dataType = buffer.Data[i].GetType().GetElementType();
          bool nullable = false;

          if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
          {
            nullable = true;
            dataType = dataType.GetGenericArguments()[0];
          }

          DataColumn col = new DataColumn(buffer.Headers[i], dataType);
          col.AllowDBNull = nullable || dataType == typeof(string);
          table.Columns.Add(col);
        }

        // Проходим по всем строкам матрицы
        for (int i = 0; i < buffer.GetLength(); i++)
        {
          // Формируем временный список - образ будущей строки
          object[] cache = new object[buffer.Headers.Length];

          for (int j = 0; j < buffer.Headers.Length; j++)
          {
            cache[j] = buffer.Data[j].GetValue(i);
          }
          // Формируем строку на основе временного списка
          table.Rows.Add(cache);
        }
      }
    }

    /// <summary>
    /// Упаковывает запрос в бинарный формат
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <returns>Запрос, упакованный в бинарный формат</returns>
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

    public bool RequireInitialize
    {
      get { return false; } // !string.IsNullOrEmpty(WcfRecordBinder.HostAddress); }
    }

    #endregion
  }
}
