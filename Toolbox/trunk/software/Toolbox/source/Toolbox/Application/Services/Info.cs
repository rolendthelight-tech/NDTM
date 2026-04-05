using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Extensions;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Сообщение с перечислимым статусом
  /// </summary>
  [Serializable]
  [DataContract]
	public sealed class Info
  {
	  [DataMember(Name = "InnerMessages")] [NotNull] private InfoBuffer m_inner_messages;
	  [DataMember(Name = "Level")] private readonly InfoLevel m_level;
	  [DataMember(Name = "Code")] private readonly int? m_code;
	  [DataMember(Name = "Message")] [NotNull] private readonly string m_message;
	  [DataMember(Name = "Details")] [CanBeNull] private readonly string m_details_string;
	  [NonSerialized] [CanBeNull] private readonly object m_details;
	  [DataMember(Name = "Link")] [CanBeNull] private readonly string m_link;

	  /// <summary>
	  /// Инициализирует новый экземпляр сообщения
	  /// </summary>
	  /// <param name="message">Текст сообщения</param>
	  /// <param name="state">Уровень сообщения</param>
		/// <param name="details">Дополнительные данные, связанные с сообщением</param>
		/// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
	  public Info([NotNull] string message, InfoLevel state, [CanBeNull] object details = null, [CanBeNull] string link = null)
    {
		  if (message == null) throw new ArgumentNullException("message");

			this.m_message = message;
			this.m_level = state;
			this.m_details = details;
			this.m_link = link;
		  this.m_details_string = GetDetailsString(this.m_details);
			this.m_inner_messages = new InfoBuffer();
    }

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
		[Obsolete("Используйте конструктор с необязательными параметрами: Info(message:message, state:state, link:link)", true)]
    public Info(string link, string message, InfoLevel state)
		{
			throw new NotImplementedException();

			//this.m_link = link;
			//this.m_message = message;
			//this.m_level = state;
			//this.m_details_string = GetDetailsString(this.m_details);
			//this.m_inner_messages = new InfoBuffer();
		}

    /// <summary>
    /// Инициализирует новый экземпляр сообщения
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
    public Info([NotNull] Exception ex)
    {
	    if (ex == null) throw new ArgumentNullException("ex");

			this.m_message = ex.Message;
			this.m_level = InfoLevel.Error;
			this.m_details = ex;
			this.m_details_string = GetDetailsString(this.m_details);
			this.m_inner_messages = new InfoBuffer();

			var aggregate_exception = ex as AggregateException;
      var fail_exception = ex as ValidationFailException;
	    if (aggregate_exception != null)
	    {
		    foreach (var inner in aggregate_exception.InnerExceptions)
		    {
					var info = new Info(inner);
					this.m_inner_messages.Add(info);
				}
	    }
      else if (fail_exception != null)
      {
        foreach (var i in fail_exception.Messages)
          m_inner_messages.Add(i);
      }
      else
      {
        Exception inner = ex.InnerException;

        if (inner != null)
        {
          var info = new Info(inner);
          this.m_inner_messages.Add(info);
        }
      }
    }

	  [CanBeNull]
	  private static string GetDetailsString([CanBeNull] object details)
	  {
		  return details == null ? null : details.ToString();
	  }

    [OnDeserializing]
		private void OnDeserializing(StreamingContext context)
    {
	    this.m_inner_messages = new InfoBuffer();
    }

	  /// <summary>
	  /// Текст сообщения
	  /// </summary>
	  public string Message
	  {
		  get { return this.m_message; }
	  }

	  /// <summary>
	  /// Уровень сообщения
	  /// </summary>
	  public InfoLevel Level
	  {
		  get { return this.m_level; }
	  }

	  /// <summary>
	  /// Код сообщения
	  /// </summary>
	  public int? Code
	  {
		  get { return this.m_code; }
	  }

	  /// <summary>
	  /// Строка, идентифицирующая объект, при обработке которого возникло сообщение
	  /// </summary>
	  [CanBeNull]
	  public string Link
	  {
		  get { return this.m_link; }
	  }

	  /// <summary>
	  /// Вложенные сообщения
	  /// </summary>
    public InfoBuffer InnerMessages
    {
      get { return m_inner_messages; }
    }

	  /// <summary>
	  /// Дополнительные данные, связанные с сообщением
	  /// </summary>
	  [CanBeNull]
	  public object Details
	  {
			get { return this.m_details ?? this.m_details_string; }
	  }

	  public override string ToString()
    {
      return string.Format("{0}: {1}", this.Level, this.Message);
    }
  }

  /// <summary>
  /// Тип сообщения
  /// </summary>
	[DisplayNameFromResource(typeof(ToolboxResources), "INFO_LEVEL")]
  [TypeConverter(typeof(EnumLabelConverter))]
  public enum InfoLevel
  {
    /// <summary>
    /// Отладочное сообщение
    /// </summary>
    [DisplayNameFromResource(typeof(ToolboxResources), "DEBUG")]
    Debug = 0,
    /// <summary>
    /// Информационное сообщение
    /// </summary>
		[DisplayNameFromResource(typeof(ToolboxResources), "INFO")]
    Info = 1,
    /// <summary>
    /// Предупреждение
    /// </summary>
		[DisplayNameFromResource(typeof(ToolboxResources), "WARNING")]
    Warning = 2,
    /// <summary>
    /// Ошибка
    /// </summary>
		[DisplayNameFromResource(typeof(ToolboxResources), "ERROR")]
    Error = 3,
    /// <summary>
    /// Критическая ошибка
    /// </summary>
		[DisplayNameFromResource(typeof(ToolboxResources), "FATAL")]
    FatalError = 4,
  }

  /// <summary>
  /// Буфер сообщений
  /// </summary>
  [Serializable]
	public sealed class InfoBuffer : IEnumerable<Info>, ICollection
	{
		[NonSerialized]
		private static readonly DataContractSerializer _dc_serializer = new DataContractSerializer(typeof(InfoBuffer));

    private readonly List<Info> m_items;

		public InfoBuffer()
		{
			m_items = new List<Info>();
		}

    public InfoBuffer([NotNull] IEnumerable<Info> items)
    {
	    if (items == null) throw new ArgumentNullException("items");

	    m_items = new List<Info>(items);
    }

	  [NotNull]
	  public static InfoBuffer Load([NotNull] string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			using (var sw = new StringReader(text))
			{
				using (var reader = new XmlTextReader(sw) { })
				{
					return (InfoBuffer)_dc_serializer.ReadObject(reader);
				}
			}
		}

		public void Save([NotNull] StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
				throw new ArgumentNullException("stringBuilder");

			using (var sw = new StringWriter(stringBuilder))
			{
				using (var writer = new XmlTextWriter(sw) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t', })
				{
					writer.WriteStartDocument();
					_dc_serializer.WriteObject(writer, this);
				}
			}
		}

	  [NotNull]
	  public string Save()
		{
			var sb = new StringBuilder();
			this.Save(sb);
			return sb.ToString();
		}

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
		/// <param name="details">Дополнительные данные, связанные с сообщением</param>
		/// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
		/// <returns>Добавленное сообщение</returns>
		[NotNull]
    public Info Add([NotNull] string message, InfoLevel state, [CanBeNull] object details = null, [CanBeNull] string link = null)
    {
	    if (message == null) throw new ArgumentNullException("message");

	    var ret = new Info(message: message, state: state, details: details, link: link);
			this.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="state">Уровень сообщения</param>
		/// <returns>Добавленное сообщение</returns>
		[NotNull]
		[Obsolete("Используйте метод Add с необязательными параметрами: Add(message:message, state:state, link:link)", true)]
    public Info Add(string link, string message, InfoLevel state)
		{
			throw new NotImplementedException();

			//Info ret = new Info(link, message, state);
			//this.Add(ret);
			//return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="state">Уровень сообщения</param>
    /// <param name="format">Формат текстового сообщения</param>
    /// <param name="args">Аргументы формата текстового сообщения</param>
		/// <returns>Добавленное сообщение</returns>
		[NotNull]
		[StringFormatMethod("format")]
		public Info Add(InfoLevel state, [NotNull] string format, [NotNull] params object[] args)
    {
	    if (format == null) throw new ArgumentNullException("format");
	    if (args == null) throw new ArgumentNullException("args");

			var ret = new Info(string.Format(format, args), state);
			this.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="link">Строка, идентифицирующая объект, при обработке которого возникло сообщение</param>
    /// <param name="state">Уровень сообщения</param>
    /// <param name="format">Формат текстового сообщения</param>
    /// <param name="args">Аргументы формата текстового сообщения</param>
    /// <returns>Добавленное сообщение</returns>
    [NotNull]
		[StringFormatMethod("format")]
		public Info Add([CanBeNull] string link, InfoLevel state, [NotNull] string format, [NotNull] params object[] args)
    {
	    if (format == null) throw new ArgumentNullException("format");
	    if (args == null) throw new ArgumentNullException("args");

			var ret = new Info(string.Format(format, args), state, link: link);
			this.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет сообщение в буфер
    /// </summary>
    /// <param name="info">Сообщение</param>
		/// <returns>Добавленное сообщение</returns>
    [NotNull]
    public Info Add([NotNull] Info info)
    {
	    if (info == null) throw new ArgumentNullException("info");

			m_items.Add(info);

	    return info;
    }

	  /// <summary>
    /// Добавляет сообщение об ошибке
    /// </summary>
    /// <param name="ex">Исключение, на основе которого формируется сообщение</param>
		/// <returns>Добавленное сообщение</returns>
		[NotNull]
	  public Info Add([NotNull] Exception ex)
    {
		  if (ex == null) throw new ArgumentNullException("ex");

		  var ret = new Info(ex);
			this.Add(ret);
      return ret;
    }

    /// <summary>
    /// Добавляет в буфер предупреждение о не пройденной проверке
    /// </summary>
    /// <param name="message">Текст предупреждения</param>
    /// <returns>Всегда возвращает <code>false</code></returns>
    public bool CheckFailed([NotNull] string message)
    {
	    if (message == null) throw new ArgumentNullException("message");

			this.Add(message, InfoLevel.Warning);
      return false;
    }

    /// <summary>
    /// Конвертирует все сообщения в список указанного типа
    /// </summary>
    /// <typeparam name="TOutput">Тип, к которому необходимо преобразовать сообщения</typeparam>
    /// <param name="converter">Функция, осуществляющая преобразования</param>
    /// <returns>Список сконвертированных в указанный тип сообщений</returns>
    [NotNull]
    public List<TOutput> ConvertAll<TOutput>([NotNull] Converter<Info, TOutput> converter)
    {
	    if (converter == null) throw new ArgumentNullException("converter");

			return m_items.ConvertAll<TOutput>(converter);
    }

	  /// <summary>
    /// Возвращает сообщение по индексу
    /// </summary>
    /// <param name="index">Индекс</param>
    /// <returns>Сообщение, если индекс не выходит за границы; иначе, бросает исключение</returns>
	  [NotNull]
	  public Info this[int index]
    {
      get { return m_items[index]; }
    }

	  void ICollection.CopyTo(Array array, int index)
	  {
		  ((ICollection) m_items).CopyTo(array, index);
	  }

	  /// <summary>
    /// Количество сообщений в буфере
    /// </summary>
    public int Count
    {
      get { return m_items.Count; }
    }

	  object ICollection.SyncRoot
	  {
		  get { return ((ICollection) m_items).SyncRoot; }
	  }

	  bool ICollection.IsSynchronized
	  {
		  get { return ((ICollection) m_items).IsSynchronized; }
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
  /// Событие для предоставления доступа к объекту информации
  /// </summary>
  [Serializable]
  public class InfoEventArgs : EventArgs
  {
	  [NotNull] private readonly Info m_info;

    public InfoEventArgs([NotNull] Info info)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      m_info = info;
    }

	  [NotNull]
	  public Info Info
    {
      get { return m_info; }
    }

    public override string ToString()
    {
      return m_info.ToString();
    }
  }

  /// <summary>
  /// Событие, содержащее буфер сообщений и результат операции
  /// </summary>
  [Serializable]
  public class InfoBufferEventArgs : EventArgs
  {
    private readonly InfoBuffer m_messages = new InfoBuffer();

    /// <summary>
    /// Создаёт новое оповещение
    /// </summary>
    public InfoBufferEventArgs() { }

    /// <summary>
    /// Создаёт новое оповещение на основе существующего списка сообщений
    /// </summary>
    /// <param name="source">Список сообщений, копируемых в оповещение</param>
    internal InfoBufferEventArgs([NotNull] IEnumerable<Info> source)
    {
	    if (source == null) throw new ArgumentNullException("source");

			foreach (Info info in source)
      {
				if (info == null) throw new ArgumentException("Contains null", "source");

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
	[Serializable]
  public class ValidationFailException : ApplicationException
  {
	  private readonly InfoBuffer m_messages;

	  /// <summary>
		/// Инициализирует новый экземпляр <see cref="T:Toolbox.Application.Services.ValidationFailException"/>
    /// </summary>
    /// <param name="buffer">Список сообщений валидации</param>
    public ValidationFailException(InfoBuffer buffer)
      : this(ToolboxResources.VALIDATION_FAIL, buffer) { }

    /// <summary>
		/// Инициализирует новый экземпляр <see cref="T:Toolbox.Application.Services.ValidationFailException"/>
    /// </summary>
    /// <param name="message">Общее сообщение об ошибке</param>
    /// <param name="buffer">Список сообщений валидации</param>
    public ValidationFailException(string message, InfoBuffer buffer)
      : base(message ?? ToolboxResources.VALIDATION_FAIL)
    {
			this.m_messages = buffer ?? new InfoBuffer();
    }

    /// <summary>
    /// Список сообщений валидации
    /// </summary>
    public InfoBuffer Messages
    {
	    get { return m_messages; }
    }
  }
}
