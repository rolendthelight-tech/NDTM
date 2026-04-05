using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox
{
  /// <summary>
  /// Структура типовой команды с поддержкой отмены и повтора
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    /// Показывает необходимость отслеживания команды
    /// </summary>
    bool TrackingRequired { get; }
    
    /// <summary>
    /// Отменяет команду
    /// </summary>
    void Undo();
    
    /// <summary>
    /// Повторяет команду
    /// </summary>
    void Redo();
  }

  /// <summary>
  /// Интерфейс, который должны реализовывать классы с поддержкой истории команд
  /// </summary>
  public interface ICommandHistoryOwner
  {
    CommandHistory History { get; }
  }

  /// <summary>
  /// Действие, выполняемое над командой
  /// </summary>
  public enum CommandAction
  {
    /// <summary>
    /// Команда добавлена в контейнер
    /// </summary>
    Add,
    /// <summary>
    /// Команда отменена
    /// </summary>
    Undo,
    /// <summary>
    /// Команда возвращена
    /// </summary>
    Redo,
    /// <summary>
    /// Глобальное действие
    /// </summary>
    Reset
  }

  /// <summary>
  /// Событие, происходящее при выплонении операцией над командой
  /// </summary>
  public class CommandEventArgs : EventArgs
  {
    internal CommandEventArgs(CommandAction action)
    {
      this.Action = action;
    }

    /// <summary>
    /// Действие, выполняемое над командой
    /// </summary>
    public CommandAction Action { get; private set; }
  }
}
