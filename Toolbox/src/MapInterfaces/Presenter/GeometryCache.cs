using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERMS.Core.Common;

namespace MapInterfaces
{
  public class GeometryCache
  {
    private readonly Dictionary<int, PointWrapper> m_point_cache = new Dictionary<int, PointWrapper>();
    private readonly Dictionary<int, PolyLineWrapper> m_polyline_cache = new Dictionary<int, PolyLineWrapper>();
    private readonly LockSource m_lock = new LockSource();
    
    public Dictionary<GeometryType, int[]> FindMissedIdentifiers(Dictionary<GeometryType,
      Dictionary<string, List<LayerItemEntry>>> id_groupped,
      Dictionary<GeometryType, Dictionary<int, GeometryWrapper>> total)
    {
      using (var rl = new ReadLock(m_lock))
      {
        var id_missed = new Dictionary<GeometryType, int[]>();

        foreach (var kv in id_groupped)
        {
          HashSet<int> missed_ids = new HashSet<int>();
          var total_ids = new Dictionary<int, GeometryWrapper>();

          switch (kv.Key)
          {
            case GeometryType.Point:
              this.FillMissedIDs(kv.Value, missed_ids, total_ids, m_point_cache.ContainsKey, id => m_point_cache[id]);
              break;
            case GeometryType.Polyline:
              this.FillMissedIDs(kv.Value, missed_ids, total_ids, m_polyline_cache.ContainsKey, id => m_polyline_cache[id]);
              break;
          }

          if (missed_ids.Count != 0)
            id_missed.Add(kv.Key, missed_ids.ToArray());

          total.Add(kv.Key, total_ids);
        }
        return id_missed;
      }
    }

    private void FillMissedIDs(Dictionary<string, List<LayerItemEntry>> layers,
      HashSet<int> missedIds,
      Dictionary<int, GeometryWrapper> total,
      Func<int, bool> contains,
      Func<int, GeometryWrapper> get)
    {
      foreach (var kv_lr in layers)
      {
        foreach (var entry in kv_lr.Value)
        {
          if (!contains(entry.GeoID))
            missedIds.Add(entry.GeoID);
          else if (!total.ContainsKey(entry.GeoID))
            total.Add(entry.GeoID, get(entry.GeoID));
        }
      }
    }

    public void FillRemainingGeometry(Dictionary<GeometryType, Dictionary<int, GeometryWrapper>> all_geometry, Dictionary<GeometryType, Dictionary<int, GeometryWrapper>> geometry)
    {
      using (var wl = new WriteLock(m_lock))
      {
        foreach (var kv in geometry)
        {
          foreach (var kv_geo in kv.Value)
          {
            switch (kv.Key)
            {
              case GeometryType.Point:
                m_point_cache[kv_geo.Key] = (PointWrapper)kv_geo.Value;
                break;
              case GeometryType.Polyline:
                m_polyline_cache[kv_geo.Key] = (PolyLineWrapper)kv_geo.Value;
                break;
            }
            all_geometry[kv.Key][kv_geo.Key] = kv_geo.Value;
          }
        }
      }
    }

    public void Save(int geoId, GeometryWrapper geometry)
    {
      if (geometry == null)
        throw new ArgumentNullException("geometry");

      var point = geometry as PointWrapper;

      if (point != null)
      {
        using (var wl = new WriteLock(m_lock))
        {
          if (m_point_cache.ContainsKey(geoId))
            m_point_cache[geoId] = point;
          else
            m_point_cache.Add(geoId, point);
        }
      }
    }

    public bool FindPoint(int geoId, out PointWrapper point)
    {
      using (var rl = new ReadLock(m_lock))
      {
        return m_point_cache.TryGetValue(geoId, out point);
      }
    }

    public bool FindPolyline(int geoId, out PolyLineWrapper polyline)
    {
      using (var rl = new ReadLock(m_lock))
      {
        return m_polyline_cache.TryGetValue(geoId, out polyline);
      }
    }

    public void RemovePoint(int geoId)
    {
      using (var wl = new WriteLock(m_lock))
      {
        m_point_cache.Remove(geoId);
      }
    }
  }
}
