using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AT.Toolbox
{
  /// <summary>
  /// Контейнер для поддержки истории команд и отслеживания изменений
  /// </summary>
  public class CommandHistory : IRevertibleChangeTracking
  {
    readonly List<ICommand> m_changes = new List<ICommand>();
    readonly List<ICommand> m_cancelled = new List<ICommand>();
    volatile bool m_executing;
    int m_state_index = -1;

    /// <summary>
    /// Происходит при добавлении, отмене или выполнении команды
    /// </summary>
    public event EventHandler<CommandEventArgs> StateChanged;

    /// <summary>
    /// Показывает, есть ли возможность отмены последнеё операции
    /// </summary>
    public bool CanUndo
    {
      get { return m_changes.Count > 0; }
    }

    /// <summary>
    /// Показывает, есть ли возможность возврата последней отменённой операции
    /// </summary>
    public bool CanRedo
    {
      get { return m_cancelled.Count > 0; }
    }

    /// <summary>
    /// Показывает, был ли связанный объект изменён после последнего сохранения
    /// </summary>
    public bool IsChanged
    {
      get { return m_state_index != m_changes.Count - 1; }
    }

    /// <summary>
    /// Возвращает список изменений от первого к последнему
    /// </summary>
    public ICommand[] Changes
    {
      get { return m_changes.ToArray(); }
    }

    /// <summary>
    /// Возвращает список отменённых изменений от отменённого последним к отменённому первым
    /// </summary>
    public ICommand[] CancelledChanges
    {
      get { return m_cancelled.ToArray(); }
    }

    /// <summary>
    /// Добавляет команду в список изменений
    /// </summary>
    /// <param name="command">Выполненная новая команда</param>
    public void AddCommand(ICommand command)
    {
      if (m_executing || command == null || !command.TrackingRequired)
        return;

      if (m_cancelled.Count > 0 && m_state_index >= m_changes.Count)
      {
        m_state_index = -2;
      }
      m_cancelled.Clear();

      m_changes.Add(command);
      this.OnStateChanged(new CommandEventArgs(CommandAction.Add));
    }

    /// <summary>
    /// Отменяет последнюю команду
    /// </summary>
    public void Undo()
    {
      if (this.CanUndo)
      {
        m_executing = true;
        try
        {
          ICommand cmd = m_changes.Last();
          cmd.Undo();
          m_changes.Remove(cmd);
          m_cancelled.Insert(0, cmd);
          this.OnStateChanged(new CommandEventArgs(CommandAction.Undo));
        }
        finally
        {
          m_executing = false;
        }
      }
    }

    /// <summary>
    /// Повторяет последнюю отменённую команду
    /// </summary>
    public void Redo()
    {
      if (this.CanRedo)
      {
        m_executing = true;
        try
        {
          ICommand cmd = m_cancelled[0];
          cmd.Redo();
          m_cancelled.RemoveAt(0);
          m_changes.Add(cmd);
          this.OnStateChanged(new CommandEventArgs(CommandAction.Redo));
        }
        finally
        {
          m_executing = false;
        }
      }
    }

    /// <summary>
    /// Возвращает состояние объекта к отменённому / повторённому состоянию,
    /// соответствующему указанной команде.
    /// </summary>
    /// <param name="terminator">Команда, до которой нужно отменить или вернуть изменения</param>
    public void RunToCommand(ICommand terminator)
    {
      if (terminator == null)
        return;

      int undoIdx = m_changes.IndexOf(terminator);
      int redoIdx = m_cancelled.IndexOf(terminator);

      if (undoIdx >= 0)
      {
        for (int i = m_changes.Count - 1; i >= undoIdx; i--)
        {
          this.Undo();
        }
      }
      else if (redoIdx >= 0)
      {
        for (int i = 0; i <= redoIdx; i++)
        {
          this.Redo();
        }
      }
    }

    #region IRevertibleChangeTracking Members

    /// <summary>
    /// Переводит объект в состояние, когда он был в последний раз помечен как сохранённый
    /// </summary>
    public void RejectChanges()
    {
      // Команды добавлены после отмены
      if (m_state_index < -1)
        return;

      if (m_state_index > m_changes.Count - 1)
      {
        // команды были отменены после сохранения
        while (this.IsChanged)
        {
          this.Redo();
        }
      }
      else if (m_state_index < m_changes.Count - 1)
      {
        // команды были добавлены после сохранения
        while (this.IsChanged)
        {
          this.Undo();
        }
      }
      this.OnStateChanged(new CommandEventArgs(CommandAction.Reset));
    }

    /// <summary>
    /// Помечает объект как сохранённый
    /// </summary>
    public void AcceptChanges()
    {
      m_state_index = m_changes.Count - 1;
      this.OnStateChanged(new CommandEventArgs(CommandAction.Reset));
    }

    /// <summary>
    /// Инициирует событие StateChanged
    /// </summary>
    /// <param name="e">Параметр с типом операции над командой</param>
    protected void OnStateChanged(CommandEventArgs e)
    {
      if (this.StateChanged != null)
      {
        this.StateChanged(this, e);
      }
    }

    #endregion
  }
}
