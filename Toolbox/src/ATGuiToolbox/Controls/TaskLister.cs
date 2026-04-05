using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using AT.Toolbox;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Properties;

namespace AT.Toolbox.Controls
{
  /// <summary>
  /// Элемент пользовательского интерфейса для отображения списка выполняющихся задач
  /// </summary>
  public partial class TaskLister : LocalizableUserControl
  {
    public TaskLister()
    {
      InitializeComponent();

      this.Disposed += delegate { AppManager.TaskManager.TaskListChanged -= this.HandleTaskListChanged; };
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      AppManager.TaskManager.TaskListChanged += this.HandleTaskListChanged;
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      colStatus.Caption = Resources.STATUS;
      colName.Caption = Resources.TASK;

      var list = new List<KeyValuePair<TaskStatus, string>>();

      foreach (TaskStatus status in Enum.GetValues(typeof(TaskStatus)))
        list.Add(new KeyValuePair<TaskStatus,string>(status, status.GetLabel()));

      m_status_edit.DataSource = list;
    }

    private void m_task_view_DoubleClick(object sender, EventArgs e)
    {
      var hit = m_task_view.CalcHitInfo(m_task_grid.PointToClient(Control.MousePosition));

      if (!hit.InRowCell)
        return;

      var task = m_task_view.GetRow(m_task_view.FocusedRowHandle) as TaskInfo;

      if (task == null)
        return;

      using (var frm = new MinimizableTaskForm(task))
      {
        frm.ShowDialog(this.FindForm());
      }
    }

    private void HandleTaskListChanged(object sender, EventArgs e)
    {
      m_task_grid.DataSource = AppManager.TaskManager.GetRunningTasks(); 
    }

    private void m_button_kill_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      var task = m_task_view.GetRow(m_task_view.FocusedRowHandle) as TaskInfo;

      if (task == null)
        return;

      if (!AppManager.Notificator.Confirm(new Info(string.Format(Resources.KILL_TASK_CONFIRM, task.Name), InfoLevel.Warning)))
        return;

      task.Kill();
    }
  }
}
