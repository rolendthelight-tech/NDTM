namespace Shell.UI
{
  /// <summary>
  /// Имя источника данных GUOP совпадает с GUOP.GUOPContext.DataSourceName.
  /// Не обращаться к GUOPContext при старте Program.MainCore: статический конструктор GUOPContext
  /// (Init/UpdateReferences в связке с RecordManager) может выполниться до настройки источников и упасть.
  /// </summary>
  internal static class GuopDataSource
  {
    public const string Name = "GUOP";
  }
}
