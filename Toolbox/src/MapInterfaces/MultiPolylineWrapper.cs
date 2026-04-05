using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MapInterfaces
{
  /// <summary>
  /// Обёртка для списка полилиний
  /// </summary>
  [Serializable]
  public sealed class MultiPolylineWrapper : GeometryWrapper
  {
    private readonly List<PointF[]> _polylines;

    public MultiPolylineWrapper(List<PointF[]> polylines)
    {
      if (polylines == null)
        throw new ArgumentNullException("points");

      _polylines = polylines;
    }

    public List<PointF[]> Polylines
    {
      get { return _polylines; }
    }

    public override TType Accept<TType>(IGeometryVisitor<TType> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
