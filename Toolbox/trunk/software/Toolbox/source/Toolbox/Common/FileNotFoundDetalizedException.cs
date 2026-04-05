using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Properties;

namespace Toolbox.Common
{
	/// <summary>
	/// Это исключение создаётся, когда попытка доступа к файлу, не существующему на диске, заканчивается неудачей.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	[Serializable]
	public class FileNotFoundDetalizedException : FileNotFoundException
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="T:Toolbox.Common.FileNotFoundDetalizedException"/> со строкой сообщения, установленной на <paramref name="message"/>, описывающей имя файла, который не может быть найден, и его HRESULT, для которой задано COR_E_FILENOTFOUND.
		/// </summary>
		/// <param name="message">Описание ошибки. Содержимое параметра <paramref name="message"/> должно быть понятным пользователю. Вызывающий оператор этого конструктора необходим, чтобы убедиться, локализована ли данная строка для текущего языка и региональных параметров системы.</param>
		/// <param name="fileName">Полное имя файла.</param>
		public FileNotFoundDetalizedException([NotNull] string message, [NotNull] [PathReference] string fileName)
			: base(message: message, fileName: fileName)
		{
			if (message == null) throw new ArgumentNullException("message");
			if (fileName == null) throw new ArgumentNullException("fileName");
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="T:Toolbox.Common.FileNotFoundDetalizedException"/> с указанным сообщением об ошибке и ссылкой на внутреннее исключение, которое стало причиной данного исключения.
		/// </summary>
		/// <param name="message">Сообщение об ошибке с объяснением причин исключения.</param>
		/// <param name="fileName">Полное имя файла.</param>
		/// <param name="innerException">Исключение, которое вызвало текущее исключение. Если значение параметра <paramref name="innerException"/> не равно null, текущее исключение сгенерировано в блоке catch, обрабатывающем внутреннее исключение.</param>
		public FileNotFoundDetalizedException([NotNull] string message, [NotNull] [PathReference] string fileName, [NotNull] Exception innerException)
			: base(message: message, fileName: fileName, innerException: innerException)
		{
			if (message == null) throw new ArgumentNullException("message");
			if (fileName == null) throw new ArgumentNullException("fileName");
			if (innerException == null) throw new ArgumentNullException("innerException");
		}

		/// <summary>
		/// Выполняет инициализацию нового экземпляра класса <see cref="T:Toolbox.Common.FileNotFoundDetalizedException"/> с указанными сведениями о сериализации и контексте.
		/// </summary>
		/// <param name="info">Объект, содержащий сериализованные данные объекта о возникающем исключении.</param>
		/// <param name="context">Объект, содержащий контекстные сведения об источнике или назначении.</param>
		protected FileNotFoundDetalizedException([NotNull] SerializationInfo info, StreamingContext context)
			: base(info: info, context: context)
		{
		}

		/// <summary>
		/// Возвращает сообщение об ошибке с объяснением причин исключения.
		/// </summary>
		/// <returns>
		/// Сообщение об ошибке.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string Message
		{
			get
			{
				return string.Format(Resources.FileNotFoundDetalizedException_Message_format, Environment.NewLine, base.Message, base.FileName);
			}
		}
	}
}
