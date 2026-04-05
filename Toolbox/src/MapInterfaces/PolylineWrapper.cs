#region Usings

using System.Drawing;
using System;

#endregion

namespace MapInterfaces
{
  /// <summary>
  /// Обёртка для полилинии
  /// </summary>
  [Serializable]
  public sealed class PolyLineWrapper : GeometryWrapper
  {
    private readonly PointF[] m_points;

    public PolyLineWrapper(PointF[] points)
    {
      if (points == null)
        throw new ArgumentNullException("points");
      
      m_points = points;
    }

    public PointF[] Points
    {
      get { return m_points; }
    }

    public override TType Accept<TType>(IGeometryVisitor<TType> visitor)
    {
      return visitor.Visit(this);
    }
  }
}