using System;
using System.Reflection;
using System.Windows.Forms;

namespace Toolbox.GUI.Extensions
{
	public static class FormExtensions
	{
		private static readonly Func<Form, bool> _form_closed_oracle;

		static FormExtensions()
		{
			var t_Form = typeof (Form);
			var pi_IsClosing = t_Form.GetProperty("IsClosing", BindingFlags.NonPublic | BindingFlags.Instance, null, typeof (bool), Type.EmptyTypes, null);

			_form_closed_oracle =
				pi_IsClosing != null && pi_IsClosing.PropertyType == typeof (bool)
					? (Func<Form, bool>) (frm => (bool) pi_IsClosing.GetValue(frm, null))
					: (frm => false);
		}

		/// <summary>
		/// Пвтается закрыть окно методом Close() и сообщает, закрылось ли оно.
		/// </summary>
		/// <param name="frm">Окно</param>
		/// <returns><code>true</code>, если закрылось</returns>
		public static bool TryClose(this Form frm)
		{
			if (frm == null)
				throw new ArgumentNullException("frm");

			frm.Close();
			return frm.IsDisposed || _form_closed_oracle(frm);
		}
	}
}
