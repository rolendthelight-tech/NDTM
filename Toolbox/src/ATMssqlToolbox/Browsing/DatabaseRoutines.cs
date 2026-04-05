using System;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using AT.Toolbox.Constants;
using DevExpress.XtraBars;

namespace AT.Toolbox.MSSQL
{

  /// <summary>
  /// Интерфейс, используемый DataBaseBrowserControl для взаимодействия с базой
  /// </summary>
  public interface ISpecificDBRoutines
  {
    /// <summary>
    /// Путь к файлу со слепком базы
    /// </summary>
    string MetaDataFile { get; set; }

    /// <summary>
    /// Проверка пригодности базы для использования в текущем приложении
    /// </summary>
    /// <param name="conn">Подключение к создаваемой базе</param>
    /// <param name="worker">Процесс для оповещения</param>
    /// <returns>Уровень допустимости применения базы в приложении</returns>
    SupportInfo Supported(string connectionString, BackgroundWorker worker);

    /// <summary>
    /// Создание объектов базы данных
    /// </summary>
    /// <param name="connectionString">Строка подключения к создаваемой базе</param>
    /// <param name="worker">Процесс для оповещения</param>
    /// <param name="compatibilityModeSql2005">Режим совместимости с MS SQL Server 2005</param>
    /// <param name="useFileStreams">Использовать тип FileStream</param>
    /// <param name="continueOnErrorInScripts">Продолжать при ошибках скриптов</param>
    /// <returns>Успешность развёртывания слепка</returns>
    bool InitStructure(string connectionString, BackgroundWorker worker, bool compatibilityModeSql2005, bool useFileStreams, bool continueOnErrorInScripts);

    /// <summary>
    /// Мягкое обновление базы данных
    /// </summary>
    /// <param name="connectionString">Строка подключения к обновляемой базе</param>
    /// <param name="worker">Процесс для оповещения</param>
    /// <param name="skipFkTest">Пропуск теста внешних ключей</param>
    /// <param name="compatibilityModeSql2005">Режим совместимости с MS SQL Server 2005</param>
    /// <param name="useFileStreams">Использовать тип FileStream</param>
    /// <param name="continueOnErrorInScripts">Продолжать при ошибках скриптов</param>
    /// <param name="reCreateTables">Пересоздавать различающие таблицы</param>
    /// <returns>Успешность обновления по слепку</returns>
    bool UpdateStructure(string connectionString, BackgroundWorker worker, bool skipFkTest, bool compatibilityModeSql2005, bool useFileStreams, bool continueOnErrorInScripts, bool reCreateTables);

    /// <summary>
    /// Принудительная перезагрузка файла слепка
    /// </summary>
    void ReloadMetaData();

    /// <summary>
    /// Получение максимальной версии базы данных
    /// </summary>
    /// <param name="connectionString">Строка полключения</param>
    /// <param name="worker">Процесс для оповещения</param>
    /// <returns>Строковое представление версии</returns>
    string GetMaxVersion(string connectionString, BackgroundWorker worker);

    SupportInfo Build(string connectionString, string version, string savePath, BackgroundWorker worker);

    string GetMetadataInfo(string path);
  }

  /// <summary>
  /// Заглушка для упрощенного взаимодействия с базой
  /// </summary>
  public class DBRoutinesStub : ISpecificDBRoutines
  {
    public string MetaDataFile { get; set; }

    public SupportInfo Supported(string connectionString, BackgroundWorker worker)
    {
      return new SupportInfo();
    }

    public bool InitStructure(string connectionString, BackgroundWorker worker, bool compatibilityModeSql2005, bool useFileStreams, bool continueOnErrorInScripts)
    {
      return false;
    }

    public bool UpdateStructure(string connectionString, BackgroundWorker worker, bool skipFkTest, bool compatibilityModeSql2005, bool useFileStreams, bool continueOnErrorInScripts, bool reCreateTables)
    {
      return false;
    }

    public void ReloadMetaData()
    {
    }

    public string GetMaxVersion(string connectionString, BackgroundWorker worker)
    {
      return "1.0.0.0";
    }

    public SupportInfo Build(string connectionString, string version, string savePath, BackgroundWorker worker)
    {
      return new SupportInfo
      {
        Version = "1.0.0.0"
      };
    }

    public string GetMetadataInfo(string path)
    {
      return "";
    }
  }
}