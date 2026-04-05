using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Drawing;
using ERMS.Core.Common;
using ERMS.Core.DAL;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

namespace MapInterfaces
{
  [ServiceContract]
  public interface IGeographyService
  {
    [OperationContract]
    Dictionary<string, MapLayer> GetLayerTables();

    /// <summary>
    /// Возвращает географические объекты по прямоугольнику.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="onlyFilterNoRect">true, если нужно фильтровать только по фильтру, а не по прямоугольнику</param>
    /// <returns></returns>
    [OperationContract]
    Dictionary<GeometryType, RectangleQueryResult> GetEntriesByRect(RectangleQuery query);

    /// <summary>
    /// Возвращает количество точек, которые вернёт запрос на получение геометрии
    /// </summary>
    /// <param name="geoIdentifiers">По каждому типу геометрии - массив географических идентификаторов,
    /// по которым будет запрашиватся геометрия</param>
    /// <returns>По каждому типу геометрии - словарь, ключом которого является географический идентификатор,
    /// а значением - количество точек, входящих в географический объект по этому идентификатору</returns>
    [OperationContract]
    Dictionary<GeometryType, Dictionary<int, int>> GetPointsCount(Dictionary<GeometryType, int[]> geoIdentifiers);

    /// <summary>
    /// Возвращает геометрию географического объекта (PointWrapper для точки, PolylineWrapper для полилинии)
    /// </summary>
    /// <param name="geoIdentifiers"></param>
    /// <returns></returns>
    [OperationContract]
    Dictionary<GeometryType, Dictionary<int, GeometryWrapper>> GetGeometry(Dictionary<GeometryType, int[]> geoIdentifiers);

    /// <summary>
    /// Возвращает стили для географических объектов. 
    /// </summary>
    /// <param name="geoIdentifiers">список айдишников гео объектов</param>
    /// <returns>словарь, где ключом является имя слоя, а значением - словарь (id гео объекта, id стиля)</returns>
    [OperationContract]
    Dictionary<string, Dictionary<int, int>> GetLayerStyles(Dictionary<string, int[]> geoIdentifiers);

    /// <summary>
    /// возвращает словарь, где ключом является ID стиля, а значением - строка стиля
    /// </summary>
    /// <param name="styleIDs">список айдишников стилей</param>
    /// <returns></returns>
    [OperationContract]
    Dictionary<int, string> GetStyles(int[] styleIDs);

    /// <summary>
    /// возвращает словарь, где ключ - имя слоя, а значение - это словарь: ключ - PK, отвечающий за стиль
    /// для конкретного объекта, значение - строка Description или ToString(), если Description = "";
    /// </summary>
    /// <param name="entityIdentifiers">Список идентификаторов БО</param>
    /// <returns></returns>
    [OperationContract]
    Dictionary<string, Dictionary<int, string>> GetDescriptions(Dictionary<string, int[]> entityIdentifiers);

    /// <summary>
    /// Сохранение объекта или создание нового
    /// </summary>
    /// <param name="BusinessObjectID">ID БО</param>
    /// <param name="geometryWrapper">Геометрия гео объекта</param>
    /// <param name="layerName">имя слоя</param>
    /// <param name="isNew">создаем новый или меняем старый</param>
    /// <returns>возвращает DTO, из которой можно получить GeoID, стиль и строку стиля</returns>
    [OperationContract]
    MapObjectStyle SaveObject(int BusinessObjectID, GeometryWrapper geometryWrapper, string layerName, bool isNew);

    /// <summary>
    /// Возвращает гео идентификатор по идентификатора БО
    /// </summary>
    /// <param name="layerName">имя слоя</param>
    /// <param name="EntityID">идентификатор БО</param>
    /// <returns></returns>
    [OperationContract]
    int[] GeoIDByEntityID(string layerName, int EntityID);

    /// <summary>
    /// удаление объекта. если с данный гео объектом связано несколько БО, тогда точка (geoPoint)
    /// не удаляется из базы, если только один - тогда удаляется.
    /// </summary>
    /// <param name="geoID">идентификатор гео объекта</param>
    /// <param name="EntityID">идентификатор БО</param>
    /// <param name="layerName">имя слоя</param>
    /// <returns></returns>
    [OperationContract]
    bool DeleteObject(int geoID, int EntityID, string layerName);

    /// <summary>
    /// возвращает БО по его идентификатору
    /// </summary>
    /// <param name="entityID"></param>
    /// <returns></returns>
    [OperationContract]
    Package<IBindable> FindByEntityID(int entityID);

    /// <summary>
    /// Возвращает идентификатор БО по идентификатору гео объекта
    /// </summary>
    /// <param name="geoID">идентификатор БО</param>
    /// <param name="layerName">имя слоя</param>
    /// <returns></returns>
    [OperationContract]
    int EntityIdByGeoId(int geoID, string layerName);

    /// <summary>
    /// Возвращает геометрию по идентификатору БО. Сейчас работает только для полилиний
    /// и используется для фильтрации по маршруту 
    /// </summary>
    /// <param name="layerName">имя слоя</param>
    /// <param name="entityID">идентификатор БО</param>
    /// <returns></returns>
    [OperationContract]
    GeometryWrapper GetMapItemGeometryByEntityID(string layerName, int entityID);

    [OperationContract]
    ToolTipEntry GetToolTip(string layer, int geoID);
  }

  public interface IMapDescription
  {
    string GetDescription(string layerName);
  }

  [DataContract]
  public abstract class StringRepresentator
  {
    public override string ToString()
    {
      return string.Join(Environment.NewLine,
        (from pi in this.GetType().GetProperties()
         where pi.GetIndexParameters().Length == 0
         select string.Format("{0} = {1}",
         pi.Name, pi.GetValue(this, null) ?? "NULL")).ToArray());
    }
  }

  [DisplayNameRes("Тип геометрии", typeof(GeometryType))]
  public enum GeometryType
  {
    [DisplayNameRes("Точки", typeof(GeometryType))]
    Point,
    [DisplayNameRes("Полилинии", typeof(GeometryType))]
    Polyline,
    [DisplayNameRes("Фигуры", typeof(GeometryType))]
    Shape
  }

  [DataContract]
  public sealed class MapLayer : StringRepresentator
  {
    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string TableName { get; set; }

    [DataMember]
    public GeometryType GeometryType { get; set; }

    [DataMember]
    public int DefaultStyleID { get; set; }

    [DataMember]
    public int PointsCount { get; set; }
  }

  [DataContract]
  public sealed class MapObjectStyle : StringRepresentator
  {
    [DataMember]
    public int GeoID { get; set; }

    [DataMember]
    public int StyleID { get; set; }

    [DataMember]
    public string StyleString { get; set; }
  }

  /// <summary>
  /// Запрос на получение идентификаторов по прямоугольнику
  /// </summary>
  [DataContract]
  public sealed class RectangleQuery : StringRepresentator
  {
    public RectangleQuery()
    {
      this.Layers = new HashSet<string>();
      this.Filters = new Dictionary<string, int[]>();
    }

    /// <summary>
    /// Слои карты, для которых нужно получить идентификаторы
    /// </summary>
    [DataMember]
    public HashSet<string> Layers { get; private set; }

    /// <summary>
    /// Прямоугольник карты, для которого нужно получить идентифиаторы
    /// </summary>
    [DataMember]
    public RectangleF? DisplayRectangle { get; set; }

    /// <summary>
    /// Фильтры по идентификаторам БО
    /// </summary>
    [DataMember]
    public Dictionary<string, int[]> Filters { get; private set; }
  }

  [DataContract]
  public sealed class RectangleQueryResult : StringRepresentator
  {
    [DataMember(Name = "Dump")]
    private byte[] m_data;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (m_data != null)
      {
        using (MemoryStream ms = new MemoryStream(m_data))
        {
          BinaryFormatter bf = new BinaryFormatter();

          LongitudalTable table = (LongitudalTable)bf.Deserialize(ms);

          this.Entries = new LayerItemEntry[table.RowCount];

          for (int i = 0; i < table.RowCount; i++)
          {
            var entry = new LayerItemEntry();
            entry.EntityID = (int)table[i][0];
            entry.GeoID = (int)table[i][1];
            entry.LayerName = (string)table[i][2];

            this.Entries[i] = entry;
          }
        }
      }
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.Entries == null)
        return;
      
      DataTable dt = new DataTable();
      dt.Columns.Add(new DataColumn("EntityID", typeof(int))
      {
        AllowDBNull = false
      });
      dt.Columns.Add(new DataColumn("GeoID", typeof(int))
      {
        AllowDBNull = false
      });
      dt.Columns.Add(new DataColumn("LayerName", typeof(string))
      {
        AllowDBNull = false
      });

      foreach (var entry in this.Entries)
        dt.Rows.Add(entry.EntityID, entry.GeoID, entry.LayerName);

      using (var ms = new MemoryStream())
      {
        var table = new LongitudalTable(dt);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, table);

        m_data = ms.ToArray();
      }
    }
    
    public LayerItemEntry[] Entries { get; set; }
  }

  [DataContract]
  public sealed class LayerItemEntry : StringRepresentator
  {
    [DataMember]
    public int GeoID { get; set; }

    [DataMember]
    public int EntityID { get; set; }

    [DataMember]
    public string LayerName { get; set; }
  }
}
