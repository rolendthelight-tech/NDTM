using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает операции по привязке данных для типизированной коллекции
  /// </summary>
  public interface IBindableObjectList : IBindingList, IDisposable
  {
    /// <summary>
    /// Имя таблицы, с которой связана коллекция
    /// </summary>
    string TableName { get; }
    
    /// <summary>
    /// Дочерние коллекции данных
    /// </summary>
    List<IBindableObjectList> Children { get; }

    /// <summary>
    /// Родительская коллекция данных
    /// </summary>
    IBindableObjectList ParentList { get; set; }

    /// <summary>
    /// Имя поля первичного ключа
    /// </summary>
    string KeyFieldName { get; }

    string RelationField { get; set; }

    FilterBase StaticFilter { get; set; }

    /// <summary>
    /// Индекс текущего объекта
    /// </summary>
    int CurrentObjectIdx { get; set; }

    /// <summary>
    /// Ссылка на текущий объект
    /// </summary>
    IBindable CurrentObject { get; }

    /// <summary>
    /// Определяет, куда добавлять новую запись, в начало или в конец
    /// </summary>
    bool Append { get; set; }

    /// <summary>
    /// Список всех идентификаторов
    /// </summary>
    /// <returns>Идентификаторы, загруженные на текущий момент</returns>
    IEnumerable GetIdsFilter();

    /// <summary>
    /// Загружает данные из базы
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool DataBind();

    /// <summary>
    /// Загружает из базы данные, удовлетворяющие запросу
    /// </summary>
    /// <param name="builder">Запрос</param>
    /// <returns>True, если операция прошла успешно</returns>
    bool DataBind(QueryObject builder);

    /// <summary>
    /// Записывает текущую запись в базу данных
    /// </summary>
    /// <returns>True, если операция прошла успешно</returns>
    bool Write();

    /// <summary>
    /// Метод предназначен для множественного обновления записей в базе данных за один запрос
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <param name="parameters">Значения обновляемых плоей</param>
    bool UpdateRecordSet(QueryObject query, Dictionary<string, object> parameters);

    /// <summary>
    /// Метод предназначен для множественного удаления записей из базы данных за один запрос
    /// </summary>
    /// <param name="query">Запрос. Внимание: не все возможности запроса могут поддерживаться!</param>
    bool DeleteRecordSet(QueryObject query);
  }
}
