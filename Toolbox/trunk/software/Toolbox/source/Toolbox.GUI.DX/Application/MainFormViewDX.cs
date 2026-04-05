using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using Toolbox.GUI.Application;
using Toolbox.GUI.DX.Dialogs;
using Toolbox.GUI.DX.Dialogs.Work;
using Toolbox.Log;

namespace Toolbox.GUI.DX.Application
{
	public class MainFormViewDX : MainFormView, IAppInstanceView, INotificationView, ITaskView, ILoginServiceView
	{
		public MainFormViewDX([NotNull] Form mainForm) : base(mainForm)
		{
			if (mainForm == null) throw new ArgumentNullException("mainForm");
		}

		override public bool Alert([NotNull] Info info, bool confirm)
		{
			if (info == null) throw new ArgumentNullException("info");

			using (var mb = new MessageBoxEx())
			{
				mb.Message = info.Message;
				mb.Caption = info.Level.GetLabel();
				mb.Buttons = confirm ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK;
				if (info.Code.HasValue)
					mb.Caption += " C" + info.Code;
				mb.Text = mb.Caption;

				switch (info.Level)
				{
					case InfoLevel.Info:
					case InfoLevel.Debug:
						mb.StandardIcon = MessageBoxEx.Icons.Information;
						break;

					case InfoLevel.Warning:
						mb.StandardIcon = MessageBoxEx.Icons.Warning;
						break;

					case InfoLevel.Error:
					case InfoLevel.FatalError:
						mb.StandardIcon = MessageBoxEx.Icons.Error;
						break;
				}

				return mb.ShowDialog(m_main_form) == DialogResult.OK;
			}
		}

		public override bool Alert([CanBeNull] string summary, [NotNull] InfoBuffer buffer, bool confirm)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");

			using (var form = new InfoListForm())
			{
				form.Accept(buffer);

				if (!string.IsNullOrEmpty(summary))
					form.SetSummary(summary);

				form.Buttons = confirm ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK;

				return form.ShowDialog(m_main_form) == DialogResult.OK;
			}
		}

		override public bool ShowProgressIndicator([NotNull] TaskInfo task)
		{
			if (task == null) throw new ArgumentNullException("task");

			if (m_main_form.InvokeRequired)
				return (bool)m_main_form.Invoke((Func<TaskInfo, bool>)ShowProgressIndicator, task);

			using (var frm = new MinimizableTaskForm(task))
			{
				frm.ShowDialog(m_main_form);
				return frm.IsCompleted;
			}
		}
	}
}
