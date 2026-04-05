using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Toolbox.Threading;

namespace Toolbox.Data
{
  /// <summary>
  /// Контейнер для поддержки истории команд и отслеживания изменений
  /// </summary>
  public class CommandHistory : IRevertibleChangeTracking
  {
    readonly List<ICommand> m_changes = new List<ICommand>();
    readonly List<ICommand> m_cancelled = new List<ICommand>();
    readonly ThreadField<bool> m_executing = new ThreadField<bool>();
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
    /// Задаёт и устанавливает максимальный размер буфера отмены
    /// </summary>
    public int MaxBufferSize { get; set; }

    /// <summary>
    /// Возвращает значение, показывающее, может ли вызывающий код добавлять команду
    /// </summary>
    public bool CanAddCommand
    {
      get { return !m_executing.Instance; }
    }

    /// <summary>
    /// Добавляет команду в список изменений
    /// </summary>
    /// <param name="command">Выполненная новая команда</param>
    public void AddCommand(ICommand command)
    {
      if (m_executing.Instance || command == null || !command.TrackingRequired)
        return;

      if (m_cancelled.Count > 0 && m_state_index >= m_changes.Count)
      {
        m_state_index = -2;
      }
      m_cancelled.Clear();

      m_changes.Add(command);

      var buf_size = this.MaxBufferSize;

      if (buf_size > 0 && m_changes.Count > buf_size)
        m_changes.RemoveAt(0);

      this.OnStateChanged(new CommandEventArgs(CommandAction.Add) { Command = command });
    }

    /// <summary>
    /// Отменяет последнюю команду
    /// </summary>
    public void Undo()
    {
      if (this.CanUndo)
      {
        m_executing.Instance = true;
        try
        {
          ICommand cmd = m_changes.Last();
          cmd.Undo();
          m_changes.Remove(cmd);
          m_cancelled.Insert(0, cmd);
          this.OnStateChanged(new CommandEventArgs(CommandAction.Undo) { Command = cmd });
        }
        finally
        {
          m_executing.Instance = false;
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
        m_executing.Instance = true;
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
          m_executing.Instance = false;
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

    /// <summary>
    /// Сброс истории
    /// </summary>
    public void Reset()
    {
      m_state_index = -1;
      m_changes.Clear();
      m_cancelled.Clear();

      this.OnStateChanged(new CommandEventArgs(CommandAction.Reset));
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
        while (this.IsChanged && m_cancelled.Count > 0)
        {
          this.Redo();
        }
      }
      else if (m_state_index < m_changes.Count - 1)
      {
        // команды были добавлены после сохранения
        while (this.IsChanged && m_changes.Count > 0)
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
