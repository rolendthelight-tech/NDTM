using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Common
{
  /// <summary>
  ///   Сообщение об окончании редактирования объекта
  /// </summary>
  public class EditEndEventArgs : EventArgs
  {
    /// <summary>
    ///   Конструктор
    /// </summary>
    /// <param name = "AreChangesAccepted"><code>true</code> — изменения применены</param>
    public EditEndEventArgs(bool AreChangesAccepted)
    {
      ChangesAccepted = AreChangesAccepted;
    }

    /// <summary>
    ///   Применены или отменены изменения
    /// </summary>
    public bool ChangesAccepted { get; protected set; }
  }
}
