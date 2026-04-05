using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MapInterfaces
{
  [Serializable]
  public sealed class PointWrapper : GeometryWrapper
  {
    private readonly PointF m_point;

    public PointWrapper(float x, float y)
    {
      m_point = new PointF(x, y);
    }

    public PointF Point
    {
      get { return m_point; }
    }

    public override TType Accept<TType>(IGeometryVisitor<TType> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
