using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Log;
using Toolbox.Properties;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
  public static class SystemExtensions
  {
    #region Call helpers

    /// <summary>
    /// Выполняет вызов обработчика события с синхронизацией
    /// </summary>
    /// <typeparam name="TArgs">Тип события</typeparam>
    /// <param name="eventHandler">Обрабатываемый делегат</param>
    /// <param name="sender">Компонент, инициировавший событие</param>
    /// <param name="args">Событие</param>
    public static void InvokeSynchronized<TArgs>([CanBeNull] this Delegate eventHandler, [CanBeNull] object sender, [CanBeNull] TArgs args)
      where TArgs : EventArgs
    {
      if (eventHandler == null)
        return;

      foreach (var dlgt in eventHandler.GetInvocationList())
      {
				var invoker = dlgt.Target as ISynchronizeInvoke ?? AppManager.Instance.Invoker;

        if (invoker != null && invoker.InvokeRequired)
        {
          invoker.Invoke(dlgt, new object[] { sender, args });
        }
        else
        {
          var handler = dlgt as EventHandler<TArgs>;

          if (handler != null)
            handler(sender, args);
          else
          {
            try
            {
              dlgt.DynamicInvoke(sender, args);
            }
            catch (Exception ex)
            {
              throw new ArgumentException(string.Format(Resources.INVALID_EVENT_HANDLER,
								eventHandler.GetType(), typeof(TArgs)), "eventHandler", ex);
            }
          }
        }
      }
    }

    /// <summary>
    /// Выполнение действия в потоке пользовательского интерфейса
    /// </summary>
    /// <param name="control">Элемент для синхронизации</param>
    /// <param name="code">Выполняемый код</param>
    static public void UIThread([NotNull] this ISynchronizeInvoke control, [NotNull] Action code)
    {
	    if (control == null) throw new ArgumentNullException("control");
	    if (code == null) throw new ArgumentNullException("code");

	    if (control.InvokeRequired)
      {
        control.BeginInvoke(code, ArrayExtensions.Empty<object>());
        return;
      }
      code.Invoke();
    }

    static public void UIThreadInvoke([NotNull] this ISynchronizeInvoke control, [NotNull] Action code)
    {
	    if (control == null) throw new ArgumentNullException("control");
	    if (code == null) throw new ArgumentNullException("code");

	    if (control.InvokeRequired)
      {
        control.Invoke(code, ArrayExtensions.Empty<object>());
        return;
      }
      code.Invoke();
    }

    #endregion

    #region Misc

    private interface ISchemaValidator
    {
      bool Validate();
    }

    private class XmlSchemaValidator<TType> : ISchemaValidator
    {
	    [NotNull] private static readonly XmlTypeMapping _mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(TType));

      private readonly XmlDocument m_document;
      private readonly InfoBuffer m_buffer;
      private bool m_valid;

      public XmlSchemaValidator([NotNull] XmlDocument document, [NotNull] InfoBuffer buffer)
      {
        if (document == null)
          throw new ArgumentNullException("document");

        if (buffer == null)
          throw  new ArgumentNullException("buffer");

        m_document = document;
        m_buffer = buffer;
      }

      public bool Validate()
      {
        m_valid = true;
        var schemas = new XmlSchemas();
        var exporter = new XmlSchemaExporter(schemas);

        exporter.ExportTypeMapping(_mapping);

        foreach (XmlSchema xml_schema in schemas)
        {
					//if (!m_document.Schemas.Contains(xml_schema.TargetNamespace))
          m_document.Schemas.Add(xml_schema);
        }

        m_document.Validate(this.ValidationEventLogger);

        return m_valid;
      }

      private void ValidationEventLogger([NotNull] object sender, [NotNull] ValidationEventArgs args)
      {
	      if (sender == null) throw new ArgumentNullException("sender");
	      if (args == null) throw new ArgumentNullException("args");

	      if (args.Severity == XmlSeverityType.Error)
          m_valid = false;

        m_buffer.Add(new Info(message: args.Message, state: args.Severity == XmlSeverityType.Error ? InfoLevel.Error : InfoLevel.Warning, details: args.Exception));
      }
    }

    /// <summary>
    /// Проверка xml-документа на соответствие схеме его сериализации
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <param name="document">Проверяемый xml-документ</param>
    /// <param name="buffer">Буфер, в который помещаются ошибки и предупреждения</param>
    /// <returns><code>true</code>, если документ соответствует схеме. Иначе, <code>false</code></returns>
    public static bool Validate<TType>([NotNull] this XmlDocument document, [NotNull] InfoBuffer buffer)
    {
	    if (document == null) throw new ArgumentNullException("document");
	    if (buffer == null) throw new ArgumentNullException("buffer");

	    return new XmlSchemaValidator<TType>(document, buffer).Validate();
    }

	  /// <summary>
    /// Проверка xml-документа на соответствие схеме его сериализации
    /// </summary>
    /// <param name="document">Проверяемый xml-документ</param>
    /// <param name="serializeType">Тип, сериализуемый через XmlSerializer</param>
    /// <param name="buffer">Буфер, в который помещаются ошибки и предупреждения</param>
    /// <returns><code>true</code>, если документ соответствует схеме. Иначе, <code>false</code></returns>
    public static bool Validate([NotNull] this XmlDocument document, [NotNull] Type serializeType, [NotNull] InfoBuffer buffer)
    {
		  if (document == null) throw new ArgumentNullException("document");
		  if (serializeType == null) throw new ArgumentNullException("serializeType");
		  if (buffer == null) throw new ArgumentNullException("buffer");

		  var validator = (ISchemaValidator)Activator.CreateInstance(typeof (XmlSchemaValidator<>).MakeGenericType(serializeType),
        document, buffer);

      return validator.Validate();
    }

    /// <summary>
    /// Строго типизированный доступ к сервису через провайдер
    /// </summary>
    /// <typeparam name="TService">Тип сервиса</typeparam>
    /// <param name="provider">Провайдер сервиса</param>
    /// <returns>Экземпляр сервиса</returns>
		[CanBeNull]
		public static TService GetService<TService>([NotNull] this IServiceProvider provider)
    {
	    if (provider == null) throw new ArgumentNullException("provider");

	    return (TService)provider.GetService(typeof(TService));
    }

	  [NotNull] static readonly Dictionary<string, string> _escapes = new Dictionary<string, string>
		    {
			    {"(^\\\\n)|[^\\\\](\\\\n)", "\n"},
			    {"(^\\\\r)|[^\\\\](\\\\r)", "\r"},
			    {"(^\\\\t)|[^\\\\](\\\\t)", "\t"},
			    {"(^\\\\s)|[^\\\\](\\\\s)", " "}
		    };

    /// <summary>
    /// Подстановка значений управляющих символов
    /// </summary>
    /// <param name="text">Текст с обозначениями управляющих символов</param>
    /// <returns>Текст с управляющими символами</returns>
		[NotNull]
		[Pure]
    public static string ReplaceEscapes([NotNull] string text)
    {
	    if (text == null) throw new ArgumentNullException("text");

	    foreach (var pair in _escapes)
      {
	      string value = pair.Value;
	      text = Regex.Replace(
		      text,
					pair.Key,
		      m =>
			      {
				      string v = m.Groups[0].Value;
				      return (v.Length > 2 ? v.Substring(0, v.Length - 2) : string.Empty) + value;
			      });
      }

	    return text;
    }

    /// <summary>
    /// Конвертация форматирующей строки с набором полей
    /// </summary>
    /// <param name="format">Форматирующая строка, содержащая в фигурных скобках ссылки на имена полей</param>
    /// <param name="fieldNames">Массив имён полей</param>
    /// <param name="indexes">Список, заполняемый индексами имён полей</param>
    /// <returns>Форматирующая строка, в которой имена полей заменены индексами в заполненном списке</returns>
    [NotNull]
		public static string ConvertFormatString([NotNull] string format, [NotNull] string[] fieldNames, [NotNull] IList<int> indexes)
    {
	    if (format == null) throw new ArgumentNullException("format");
	    if (fieldNames == null) throw new ArgumentNullException("fieldNames");
	    if (indexes == null) throw new ArgumentNullException("indexes");

	    indexes.Clear();

      for (int i = 0; i < fieldNames.Length; i++)
      {
        string pattern1 = "{" + fieldNames[i] + "}";
        string pattern2 = "{" + fieldNames[i] + ":";
        bool hit1 = format.Contains(pattern1);
        bool hit2 = format.Contains(pattern2);

        if (hit1 || hit2)
        {
          var idx = indexes.Count;

          if (hit1)
            format = format.Replace(pattern1, "{" + idx + "}");

          if (hit2)
            format = format.Replace(pattern2, "{" + idx + ":");

          indexes.Add(i);
        }
      }

      return format;
    }

		[Pure]
		public static int GetCategoryIndex([CanBeNull] string category, [NotNull] IList<string> order)
    {
	    if (order == null) throw new ArgumentNullException("order");

	    var ret = order.IndexOf(category);

      if (ret < 0)
        return int.MaxValue;

      return ret;
    }

		[Pure]
		public static bool AreEqual([CanBeNull] object obj1, [CanBeNull] object obj2, bool emptyStringIsNull)
    {
      if (emptyStringIsNull)
      {
        if (obj1 == null)
          obj1 = string.Empty;
        if (obj2 == null)
          obj2 = string.Empty;
      }

      if (ReferenceEquals(obj1, obj2))
        return true;

      if (obj1 == null)
        return obj2 == null;
      return obj1.Equals(obj2);
    }

    #endregion
  }
}
