using System.ComponentModel;

namespace AT.Toolbox.MSSQL.ORM.DAL
{
  public interface IDbContext
  {
    /// <summary>
    /// Проверка подключения по всем таблицам, объявленным в контексте
    /// </summary>
    /// <returns>True, если все таблицы подключились удачно</returns>
    bool TestConnection(BackgroundWorker worker);
  }
}