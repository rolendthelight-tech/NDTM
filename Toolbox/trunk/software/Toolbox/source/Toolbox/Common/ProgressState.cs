using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Toolbox.Common
{
	/// <summary>
	/// Статус удалённой операции
	/// </summary>
	[DataContract]
	public sealed class ProgressState
	{
		/// <summary>
		/// Процент выполнения
		/// </summary>
		[DataMember]
		public int Percentage { get; set; }

		/// <summary>
		/// Описание статуса
		/// </summary>
		[DataMember]
		public string Description { get; set; }
	}
}
