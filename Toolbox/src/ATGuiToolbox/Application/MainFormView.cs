using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Dialogs;
using System.ComponentModel;

namespace AT.Toolbox
{
  /// <summary>
  /// Реализация всех интерфейсов представлений,
  /// используемых сервисами AppManager, на базе WindowsForms / DevExpress
  /// </summary>
  public class MainFormView : IAppInstanceView, INotificationView, ITaskView, ILoginServiceView
  {
    private readonly Form m_main_form;

    public MainFormView(Form mainForm)
    {
      if (mainForm == null)
        throw new ArgumentNullException("mainForm");

      m_main_form = mainForm;
    }

    #region IAppInstanceView Members

    public string StartupPath
    {
      get { return Application.StartupPath; }
    }

    public void Restart(CommandLineArgs args)
    {
      Application.Restart();
    }

    public void Exit()
    {
      Application.Exit();
    }

    #endregion

    #region ISynchronizeProvider Members

    public ISynchronizeInvoke Invoker
    {
      get { return m_main_form; }
    }

    #endregion

    #region INotificationView Members

    public bool Alert(Info info, bool confirm)
    {
      MessageBoxEx mb = new MessageBoxEx();
      mb.Message = info.Message;
      mb.Caption = info.Level.GetLabel();
      if (info.Code.HasValue)
        mb.Caption += " C" + info.Code;
      mb.Text = mb.Caption;
      mb.Buttons = confirm ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK;

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
          mb.StandardIcon = MessageBoxEx.Icons.Error;
          break;
      }

      return mb.ShowDialog(m_main_form) == DialogResult.OK;
    }

    public bool Alert(string summary, InfoBuffer buffer, bool confirm)
    {
      InfoListForm form = new InfoListForm();

      form.Accept(buffer);

      if (!string.IsNullOrEmpty(summary))
        form.SetSummary(summary);

      form.Buttons = confirm ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK;

      return form.ShowDialog(m_main_form) == DialogResult.OK;
    }

    #endregion

    #region ILoginServiceView Members

    public bool ShowAuthentificationForm(out UserCredentials crd)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region ITaskView Members

    public bool ShowProgressIndicator(TaskInfo task)
    {
      using (var frm = new MinimizableTaskForm(task))
      {
        frm.ShowDialog(m_main_form);

        return frm.IsCompleted;
      }
    }

    #endregion
  }
}
