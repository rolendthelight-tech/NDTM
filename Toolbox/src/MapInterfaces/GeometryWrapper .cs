using System.Drawing;
using System;
using System.Runtime.Serialization;

namespace MapInterfaces
{
  /// <summary>
  /// Обёртка для геометрии
  /// </summary>
  [Serializable]
  [DataContract]
  [KnownType(typeof(PointWrapper))]
  [KnownType(typeof(PolyLineWrapper))]
  [KnownType(typeof(MultiPolylineWrapper))]
  public abstract class GeometryWrapper
  {
    internal GeometryWrapper() { }

    public abstract TType Accept<TType>(IGeometryVisitor<TType> visitor);
  }

  public interface IGeometryVisitor<TType>
  {
    TType Visit(PolyLineWrapper polyline);

    TType Visit(PointWrapper pointWrapper);

    TType Visit(MultiPolylineWrapper multiPolyline);
  }
}
