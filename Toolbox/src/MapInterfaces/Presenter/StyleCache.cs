using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERMS.Core.Common;

namespace MapInterfaces
{
  public class StyleCache
  {
    private readonly Dictionary<string, Dictionary<int, int>> m_object_style_cahce = new Dictionary<string, Dictionary<int, int>>();
    private readonly Dictionary<int, string> m_common_style_cahce = new Dictionary<int, string>();
    private readonly LockSource m_lock = new LockSource();

    public Dictionary<int, int> GetObjectStyle(string layerName)
    {
      Dictionary<int, int> dict_style;

      using (var rl = new ReadLock(m_lock))
      {
        if (m_object_style_cahce.TryGetValue(layerName, out dict_style))
          return dict_style;
      }

      using (var wl = new WriteLock(m_lock))
      {
        if (!m_object_style_cahce.TryGetValue(layerName, out dict_style))
        {
          dict_style = new Dictionary<int, int>();
          m_object_style_cahce.Add(layerName, dict_style);
        }

        return dict_style;
      }
    }

    public bool FindStyle(int styleId, out string styleString)
    {
      using (var rl = new ReadLock(m_lock))
      {
        return m_common_style_cahce.TryGetValue(styleId, out styleString);
      }
    }

    public void SaveStyleString(int styleId, string styleString)
    {
      using (var wl = new WriteLock(m_lock))
      {
        m_common_style_cahce[styleId] = styleString;
      }
    }

    public void FillMissedStyleData(Dictionary<string, Dictionary<int, int>> all_styles, HashSet<int> missed_style_data, Dictionary<int, string> style_data)
    {
      foreach (var kv in all_styles)
      {
        foreach (var st in kv.Value)
        {
          if (missed_style_data.Contains(st.Value)
          || style_data.ContainsKey(st.Value))
            continue;

          string style;

          if (m_common_style_cahce.TryGetValue(st.Value, out style))
            style_data[st.Value] = style;
          else
            missed_style_data.Add(st.Value);
        }
      }
    }

    public void FillMissedStyles(Dictionary<GeometryType, Dictionary<string, List<LayerItemEntry>>> id_groupped, Dictionary<string, int[]> missed_styles, Dictionary<string, Dictionary<int, int>> all_styles)
    {
      using (var wl = new WriteLock(m_lock))
      {
        foreach (var kv in id_groupped)
        {
          foreach (var kv_lr in kv.Value)
          {
            Dictionary<int, int> style_cache = null;
            if (!m_object_style_cahce.TryGetValue(kv_lr.Key, out style_cache))
            {
              style_cache = new Dictionary<int, int>();
              m_object_style_cahce.Add(kv_lr.Key, style_cache);
            }

            HashSet<int> missed_ids = new HashSet<int>();

            Dictionary<int, int> found_styles;

            if (!all_styles.TryGetValue(kv_lr.Key, out found_styles))
            {
              found_styles = new Dictionary<int, int>();
              all_styles.Add(kv_lr.Key, found_styles);
            }

            foreach (var item in kv_lr.Value)
            {
              int style_id;

              if (!style_cache.TryGetValue(item.GeoID, out style_id))
                missed_ids.Add(item.GeoID);
              else
                found_styles[item.GeoID] = style_id;
            }

            if (missed_ids.Count > 0)
              missed_styles.Add(kv_lr.Key, missed_ids.ToArray());
          }
        }
      }
    }

    public void FillRemainingStyles(Dictionary<string, Dictionary<int, int>> allStyles, Dictionary<string, Dictionary<int, int>> remStyles)
    {
      foreach (var kv in remStyles)
      {
        var st = allStyles[kv.Key];

        Dictionary<int, int> layer_styles = null;

        if (!m_object_style_cahce.TryGetValue(kv.Key, out layer_styles))
        {
          layer_styles = new Dictionary<int, int>();
          m_object_style_cahce.Add(kv.Key, layer_styles);
        }

        foreach (var kv2 in kv.Value)
        {
          layer_styles[kv2.Key] = kv2.Value;
          st[kv2.Key] = kv2.Value;
        }
      }
    }
  }
}
