using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает привязку данных с использованием глобального идентификатора
  /// </summary>
  public interface IBindableEntity : IBindable
  {
    /// <summary>
    /// Значение глобального идентификатора
    /// </summary>
    Guid ObjectGuid { get; set; }
  }

  /// <summary>
  /// Интерфейс, который реализуют классы, поддерживающие уникальность набора реквизитов
  /// </summary>
  public interface ISupportFindOrCreate : IBindable
  {
    /// <summary>
    /// Ищет в базе данных запись по набору реквизитов, если не найдена, создает новую
    /// </summary>
    /// <returns></returns>
    object FindOrCreate();
  }

  public interface IColorRow : IBindable
  {
    /// <summary>
    /// Обеспечивает работу с цветом строки в гриде
    /// </summary>
    Color GetColor();
  }

  /// <summary>
  /// Интерфейс для бизнес-объектов,
  /// требующих очистки перед архивацией
  /// </summary>
  public interface ISupportClear : IBindable
  {
    /// <summary>
    /// Очищает связанные объекты
    /// </summary>
    void Clear();
  }

  public interface IMasterRecord : IBindable
  {
    /// <summary>
    /// Обеспечивает работу списками связанных документов
    /// </summary>
    int GetRelationCount();
    bool IsMasterRowEmpty(int relationIdx);
    string GetRelationName(int relationIdx);
    IList GetChildList(int relationIdx);
  }

  /// <summary>
  /// Обеспечивает работу с номером документа
  /// </summary>
  public interface IDocument : IBindableEntity
  {
    int Number { get; set; }
  }

  /// <summary>
  /// Определяет собственный способ визуального отображения полей в истории
  /// </summary>
  public interface ISupportFieldAliases : IBindable
  {
    object GetFieldAlias(string fieldName);
    bool HasAlias(string fieldName);
  }

  /// <summary>
  /// Обеспечивает поддержку склонения
  /// </summary>
  public interface ISupportDeclension : IBindableEntity
  {
    string FullName { get; set; }
  }
}
