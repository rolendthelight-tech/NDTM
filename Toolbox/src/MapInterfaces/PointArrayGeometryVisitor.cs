using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ERMS.Core.Common;

namespace MapInterfaces
{
  public class PointArrayGeometryVisitor : IGeometryVisitor<PointF[]>
  {
    #region IGeometryVisitor<PointF[]> Members

    public PointF[] Visit(PolyLineWrapper polyline)
    {
      return polyline.Points;
    }

    public PointF[] Visit(PointWrapper pointWrapper)
    {
      return new PointF[] { pointWrapper.Point };
    }

    public PointF[] Visit(MultiPolylineWrapper multiPolyline)
    {
      HashSet<int> idxs = new HashSet<int>();
      List<PointF[]> sorted = new List<PointF[]>();

      for (int i = 1; i < multiPolyline.Polylines.Count; i++)
        idxs.Add(i);

      sorted.Add(multiPolyline.Polylines[0]);

      do
      {
        var first = sorted[0];
        bool once_found = false;

        for (int i = 0; i < multiPolyline.Polylines.Count; i++)
        {
          if (!idxs.Contains(i))
            continue;

          if (ComparePoints(first[0], multiPolyline.Polylines[i].Last()))
          {
            sorted.Insert(0, multiPolyline.Polylines[i]);
            idxs.Remove(i);
            once_found = true;
            break;
          }
        }

        var last = sorted[sorted.Count - 1];
        for (int i = 0; i < multiPolyline.Polylines.Count; i++)
        {
          if (!idxs.Contains(i))
            continue;

          if (ComparePoints(last.Last(), multiPolyline.Polylines[i][0]))
          {
            sorted.Add(multiPolyline.Polylines[i]);
            idxs.Remove(i);
            once_found = true;
            break;
          }
        }

        if (!once_found)
        {
          AppManager.MessageLog.Log("PointArrayGeometryVisitor", new Info("Не удалось связать полилинию", InfoLevel.Warning));
          break;
        }
      }
      while (idxs.Count > 0);

      return sorted.SelectMany(pts => pts).ToArray();
    }

    private bool ComparePoints(PointF previous, PointF next)
    {
      float diff = 0.01F;

      return (Math.Abs(previous.X - next.X) < diff
        && Math.Abs(previous.Y - next.Y) < diff);
    }


    #endregion
  }

  public static class GeometryWrapperExtensions
  {
    public static PointF[] GetPointArray(this GeometryWrapper geometry)
    {
      return geometry.Accept(new PointArrayGeometryVisitor());
    }
  }
}
