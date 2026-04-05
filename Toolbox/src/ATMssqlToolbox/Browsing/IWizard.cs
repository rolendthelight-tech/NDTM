using System.Collections.Generic;

namespace AT.Toolbox.MSSQL.Browsing
{
  /// <summary>
  /// Интерфейс, реализуемый формой процесса
  /// </summary>
  public interface IWizard
  {
    /// <summary>
    /// Получает метаданные о процессе
    /// </summary>
    /// <returns>Словарь вида имя таблицы -> Список разрешений</returns>
    Dictionary<string, string[]> GetRequiredPermissions();
  }
}
