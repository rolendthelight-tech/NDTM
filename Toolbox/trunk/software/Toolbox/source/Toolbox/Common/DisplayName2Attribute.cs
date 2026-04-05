using System;
using System.ComponentModel;

namespace Toolbox.Common
{
	/// <summary>
	/// Задаёт отображаемое имя свойства, поля, элемента перечисления или типа
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
	public class DisplayName2Attribute : DisplayNameAttribute
	{
		public DisplayName2Attribute(string displayName) : base(displayName)
		{
		}
	}
}