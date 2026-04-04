using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Reflection;
using AT.Toolbox.Properties;
using AT.Toolbox;

namespace AT.Toolbox
{
  /// <summary>
  /// Сообщение с перечислимым статусом
  /// </summary>
  [Serializable]
  [DataContract]
  public sealed class Info
  {
    private readonly InfoBuffer m_inner_messages = new InfoBuffer();

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info(string message, InfoLevel state)
    {
      this.Message = message;
      this.Level = state;
    }

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info(string link, string message, InfoLevel state)
    {
      this.Link = link;
      this.Message = message;
      this.Level = state;
    }
    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
    public Info(Exception ex)
    {
      this.Message = ex.Message;
      this.Level = InfoLevel.Error;
      this.Details = ex;

      Exception inner = ex.InnerException;

      while (inner != null)
      {
        var inf = new Info(inner.Message, InfoLevel.Error);
        inf.Details = inner;
        m_inner_messages.Add(inf);
        inner = inner.InnerException;
      }
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.Details != null)
        this.Details = this.Details.ToString();
    }

    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      typeof(Info).GetField("m_inner_messages",
        BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, new InfoBuffer());
    }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    [DataMember]
    public string Message { get; private set; }

    /// <summary>
    /// Уровень сообщения
    /// </summary>
    [DataMember]
    public InfoLevel Level { get; private set; }

    /// <summary>
    /// Код сообщения
    /// </summary>
    [DataMember]
    public int? Code { get; private set; }

    /// <summary>
    /// Строка, идентифицирующая объект, при обработке которого возникло сообщение
    /// </summary>
    [DataMember]
    public string Link { get; private set; }

    /// <summary>
    /// Вложенные сообщения
    /// </summary>
    [DataMember]
    public InfoBuffer InnerMessages
    {
      get { return m_inner_messages; }
    }

    /// <summary>
    /// Дополнительные данные, связанные с сообщением
    /// </summary>
    [DataMember]
    public object Details { get; set; }
  }

  /// <summary>
  /// Тип сообщения
  /// </summary>
  [DisplayNameRes("INFO_LEVEL", typeof(InfoLevel))]
  [TypeConverter(typeof(EnumLabelConverter))]
  public enum InfoLevel
  {
    /// <summary>
    /// Отладочное сообщение
    /// </summary>
    [DisplayNameRes("DEBUG", typeof(InfoLevel))]
    Debug = 0,
    /// <summary>
    /// Информационное сообщение
    /// </summary>
    [DisplayNameRes("INFO", typeof(InfoLevel))]
    Info = 1,
    /// <summary>
    /// Предупреждение
    /// </summary>
    [DisplayNameRes("WARNING", typeof(InfoLevel))]
    Warning = 2,
    /// <summary>
    /// Ошибка
    /// </summary>
    [DisplayNameRes("ERROR", typeof(InfoLevel))]
    Error = 3,
    /// <summary>
    /// Критическая ошибка
    /// </summary>
    [DisplayNameRes("FATAL", typeof(InfoLevel))]
    FatalError = 4
  }

  /// <summary>
  /// Буфер сообщений
  /// </summary>
  [Serializable]
  public sealed class InfoBuffer : IEnumerable<Info>
  {
    private List<Info> m_items = new List<Info>();

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info Add(string message, InfoLevel state)
    {
      Info ret = new Info(message, state);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
    public Info Add(string link, string message, InfoLevel state)
    {
      Info ret = new Info(link, message, state);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="state">Уровень сообщения</param>
    /// <param name="format">Формат текстового сообщения</param>
    /// <param name="args">Аргументы формата текстового сообщения</param>
    public Info Add(InfoLevel state, string format, params object[] args)
    {
      Info ret = new Info(string.Format(format, args), state);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="state">Уровень сообщения</param>
    /// <param name="format">Формат текстового сообщения</param>
    /// <param name="args">Аргументы формата текстового сообщения</param>
    public Info Add(string link, InfoLevel state, string format, params object[] args)
    {
      Info ret = new Info(link, string.Format(format, args), state);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="info">Сообщение</param>
    public void Add(Info info)
    {
      m_items.Add(info);
    }

    /// <summary>
    /// Добавляет сообщение об ошибке
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
    public Info Add(Exception ex)
    {
      Info ret = new Info(ex);
      m_items.Add(ret);
      return ret;
    }

    /// <summary>
    /// Конвертирует все сообщения в список указанного типа
    /// </summary>
    /// <typeparam name="TOutput">Тип, к которому необходимо преобразовать сообщения</typeparam>
    /// <param name="converter">Функция, осуществляющая преобразования</param>
    /// <returns>Список сконвертированных в указанный тп сообщений</returns>
    public List<TOutput> ConvertAll<TOutput>(Converter<Info, TOutput> converter)
    {
      return m_items.ConvertAll<TOutput>(converter);
    }

    /// <summary>
    /// Возвращает сообщение по индексу
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns>Сообщение, если индекс не выходит за границы; иначе, бросает исключение</returns>
    public Info this[int index]
    {
      get { return m_items[index]; }
    }

    /// <summary>
    /// Количество сообщений в буфере
    /// </summary>
    public int Count
    {
      get { return m_items.Count; }
    }

    #region IEnumerable<Info> Members

    /// <summary>
    /// Возвращает перечислитель для сообщений
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator<Info> GetEnumerator()
    {
      return m_items.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_items.GetEnumerator();
    }

    #endregion
  }

  /// <summary>
  /// Событие, содержащее буфер сообщений и результат операции
  /// </summary>
  [Serializable]
  public class InfoBufferEventArgs : EventArgs
  {
    private InfoBuffer m_messages = new InfoBuffer();

    /// <summary>
    /// Создаёт новое оповещение
    /// </summary>
    public InfoBufferEventArgs() { }

    /// <summary>
    /// Создаёт новое оповещение на основе существующего списка сообщений
    /// </summary>
    /// <param name="source">Список сообщений, копируемых в оповещение</param>
    internal InfoBufferEventArgs(IEnumerable<Info> source)
    {
      foreach (Info info in source)
      {
        m_messages.Add(info.Message, info.Level);
      }
    }

    /// <summary>
    /// Задаёт результат выполнения операции, в ходе которой возникли сообщения
    /// </summary>
    public bool Result { get; set; }

    /// <summary>
    /// Возвращает сообщения, хранящиеся в оповещении
    /// </summary>
    public InfoBuffer Messages
    {
      get { return m_messages; }
    }
  }

  /// <summary>
  /// Бросается при попытке выполнить операцию, для выполнения которой объект не валиден
  /// </summary>
  public class ValidationFailException : ApplicationException
  {
    /// <summary>
    /// Инициализирует новый экземпляр ValidationFailException
    /// </summary>
    /// <param name="buffer">Список сообщений валидации</param>
    public ValidationFailException(InfoBuffer buffer)
      : this(ToolboxResources.VALIDATION_FAIL, buffer) { }

    /// <summary>
    /// Инициализирует новый экземпляр ValidationFailException
    /// </summary>
    /// <param name="message">Общее сообщение об ошибке</param>
    /// <param name="buffer">Список сообщений валидации</param>
    public ValidationFailException(string message, InfoBuffer buffer)
      : base(message ?? ToolboxResources.VALIDATION_FAIL)
    {
      this.Messages = buffer ?? new InfoBuffer();
    }

    /// <summary>
    /// Список сообщений валидации
    /// </summary>
    public InfoBuffer Messages { get; private set; }
  }
}
