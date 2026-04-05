using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using AT.Toolbox.Log;

namespace AT.Toolbox.DB
{
  /// <summary>
  /// Фильтр
  /// </summary>
  public class Filter
  {
    public Filter()
    {
      FilterID = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Идентификатор
    /// </summary>
    public string FilterID { get; set; }

    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public string ParentID { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string FilterName { get; set; }

    /// <summary>
    /// Является ли группой
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// Название источника данных для  фильтрации
    /// </summary>
    public string DataSourceName { get; set; }

    /// <summary>
    /// Выражение для фильтрации
    /// </summary>
    public string FilterExpression { get; set; }

    /// <summary>
    /// Выражение для SQL Where-Clause
    /// </summary>
    public string SQLSelect
    {
      get
      {
        string filter = FilterExpression.Replace("[", "");
        filter = filter.Replace("]", "");

        return filter;
      }
    }
  }

  /// <summary>
  /// Абстрактное хранилище фильтров
  /// </summary>
  public abstract class FilterStorage
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FilterStorage).Name);

    /// <summary>
    /// Список фильтров
    /// </summary>
    protected BindingList<Filter> m_filters = new BindingList<Filter>();

    /// <summary>
    /// Строка для подключения или путь к файлу
    /// </summary>
    protected string m_connection_string;

    /// <summary>
    /// Список фильтров
    /// </summary>
    public BindingList<Filter> Filters
    {
      get { return m_filters; }
    }

    /// <summary>
    /// Загрузка фильтров
    /// </summary>
    /// <param name="ConnectionString">Строка для подключения</param>
    public virtual void Load(string ConnectionString)
    {
      m_connection_string = ConnectionString;
    }

    /// <summary>
    /// Сохранение фильтров
    /// </summary>
    public abstract void Save();

    /// <summary>
    /// Повторная загрузка фильтров
    /// </summary>
    public void ReLoad()
    {
      Load(m_connection_string);
    }
  }


  public static class FilterStorages
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FilterStorages).Name);

    private static string m_prev_connection;
    private static FilterStorage m_instance;

    public static FilterStorage Instance(string ConnectionString)
    {
      if (ConnectionString != m_prev_connection)
      {
        if (null != m_instance)
          m_instance.Save();

        m_prev_connection = ConnectionString;

        try
        {
          if (!File.Exists(ConnectionString))
            File.Create(ConnectionString);

          m_instance = new FileFilterStorage();
          m_instance.Load(ConnectionString);
        }
        catch (Exception ex)
        {
          Log.Error("Instance.get(): exception", ex);
          //ex.ToString();
        }
      }

      return m_instance;
    }

    public static FilterStorage CurrentInstance
    {
      get { return m_instance; }
    }
  }

  internal class FileFilterStorage : FilterStorage
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FileFilterStorage).Name);

    public override void Load(string ConnectionString)
    {
      base.Load(ConnectionString);

      try
      {
        XmlSerializer s = new XmlSerializer(typeof (BindingList<Filter>));
        FileStream stream = new FileStream(m_connection_string, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        m_filters = s.Deserialize(stream) as BindingList<Filter>;
      }
      catch (Exception ex)
      {
        Log.Error("Load(): exception", ex);
      }
    }

    public override void Save()
    {
      if (string.IsNullOrEmpty(m_connection_string))
        return;

      XmlSerializer s = new XmlSerializer(typeof (BindingList<Filter>));

      FileStream stream = new FileStream(m_connection_string, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

      s.Serialize(stream, m_filters);

      stream.Close();
    }
  }
}