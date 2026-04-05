using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	/// <summary>
	/// Задаёт отображаемую подсказку для свойства, поля, элемента перечисления или типа
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
	public class DisplayTipAttribute : Attribute
	{
		[NotNull] private readonly string m_display_tip;

		public DisplayTipAttribute(string displayTip)
		{
			this.m_display_tip = displayTip;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DisplayTipAttribute attribute = obj as DisplayTipAttribute;
			return ((attribute != null) && (attribute.DisplayTip == this.DisplayTip));
		}

		public override int GetHashCode()
		{
			return this.DisplayTip.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return false;
		}

		public virtual string DisplayTip
		{
			get { return this.DisplayTipValue; }
		}

		protected string DisplayTipValue
		{
			get { return this.m_display_tip; }
		}
	}
}