using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Dialogs.Work;
using Toolbox.GUI.DX.Properties;
using Toolbox.Log;

namespace Toolbox.GUI.DX.Controls
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

    public event EventHandler<OpeningTaskViewEventArgs> TaskOpening;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      AppManager.TaskManager.TaskListChanged += this.HandleTaskListChanged;
      this.HandleTaskListChanged(this, EventArgs.Empty);
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      colStatus.Caption = Resources.STATUS;
      colName.Caption = Resources.TASK;
      colDescriprion.Caption = Resources.TASK_STATE;
      colKill.Caption = " ";

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

      if (this.TaskOpening != null)
      {
        var args = new OpeningTaskViewEventArgs(task);
        this.TaskOpening(this, args);

        if (args.Handled)
          return;
      }

      using (var frm = new MinimizableTaskForm(task))
      {
        frm.ShowDialog(this.FindForm());
      }
    }

    private void HandleTaskListChanged(object sender, EventArgs e)
    {
      m_task_grid.DataSource = new BindingList<TaskInfo>(AppManager.TaskManager.GetRunningTasks());
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

    private void m_task_view_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
    {
      if (e.Column != colPercentage)
        return;

      var row = m_task_view.GetRow(e.RowHandle) as TaskInfo;

      if (row == null)
        return;

      if (row.PercentNotification)
        e.RepositoryItem = m_progress_bar;
      else
        e.RepositoryItem = m_marquee;
    }
  }

  public class OpeningTaskViewEventArgs : EventArgs
  {
    private readonly TaskInfo m_task;

    public OpeningTaskViewEventArgs(TaskInfo taskInfo)
    {
      if (taskInfo == null)
        throw new ArgumentNullException("taskInfo");

      m_task = taskInfo;
    }

    public TaskInfo Task
    {
      get { return m_task; }
    }

    public bool Handled { get; set; }
  }
}
