using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.ServiceModel;
using ERMS.Core.DAL;
using ERMS.Core.Common;

namespace MapInterfaces
{
  [ServiceContract(Name = "LayerDataSourceService")]
  public interface ILayerDataSource
  {
    /// <summary>
    /// Запрос группы объектов по диапазону координат
    /// </summary>
    /// <param name="object_type">Тип объекта</param>
    /// <param name="rect">Диапазон координат</param>
    /// <returns>Список объектов</returns>
    [OperationContract]
    List<MappableObject> QueryObjectsByRect(string object_type, RectangleF rect);

    /// <summary>
    /// Поиск объекта по его геоидентификатору
    /// </summary>
    /// <param name="object_type">Тип объекта</param>
    /// <param name="geo_id">Географический идентификатор</param>
    /// <returns>Объект</returns>
    [OperationContract]
    MappableObject FindObject(string object_type, int geo_id);

    /// <summary>
    /// Поиск объекта по координатам
    /// </summary>
    /// <param name="object_type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [OperationContract]
    MappableObject FindObjectByCoord(string object_type, double x, double y);

    /// <summary>
    /// Сохранение изменений в объекте
    /// </summary>
    /// <param name="mappable_object">Объект</param>
    [OperationContract]
    MapUpdateResult SaveObject(string object_type, MappableObject mappable_object);

    [OperationContract]
    MapUpdateResult DeleteObject(string object_type, MappableObject mappable_object);

    [OperationContract]
    List<MappableObject> QueryObjectsByIDArray(string object_type, int[] geo_ids);
  }

  [DataContract]
  public class MapUpdateResult 
  {
    [DataMember]
    public MappableObject UpdatedObject { get; set; }

    private InfoBuffer m_messages;

    [DataMember]
    public InfoBuffer Messages
    {
      get
      {
        if (m_messages == null)
          m_messages = new InfoBuffer();

        return m_messages;
      }
    }
  }

  [DataContract]
  public sealed class MappableObject
  {
    [DataMember]
    public string StyleString { get; set; }

    [DataMember]
    public int GeoID { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string TableName { get; set; }

    [DataMember]
    private GeometryWrapper Geometry { get; set; }
  
    public GeometryWrapper GetGeometry()
    {
      return this.Geometry;
    }

    public void SetGeometry(GeometryWrapper geometry)
    {
      this.Geometry = geometry;
    }
  }
}