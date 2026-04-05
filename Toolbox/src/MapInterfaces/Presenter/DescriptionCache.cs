using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERMS.Core.Common;

namespace MapInterfaces
{
  public class DescriptionCache
  {
    private readonly SharedSource<string, Dictionary<int, string>> m_description_cache = new SharedSource<string, Dictionary<int, string>>(lr
      => new Dictionary<int, string>());
    private readonly LockSource m_lock = new LockSource();

    public void DeleteDescription(string layerName, int geoId)
    {
      using (var wl = new WriteLock(m_lock))
      {
        m_description_cache.Lookup(layerName).Remove(geoId);
      }
    }

    public void FillRemainingDescriptions(Dictionary<string, Dictionary<int, string>> allDescriptions, Dictionary<string, Dictionary<int, string>> newFound)
    {
      using (var wl = new WriteLock(m_lock))
      {
        foreach (var kv in newFound)
        {
          Dictionary<int, string> current_cache = m_description_cache.Lookup(kv.Key);
          Dictionary<int, string> current_total = allDescriptions[kv.Key];

          foreach (var kv_item in kv.Value)
          {
            current_cache[kv_item.Key] = kv_item.Value;
            current_total[kv_item.Key] = kv_item.Value;
          }
        }
      }
    }

    public void FindMissedDescriptions(Dictionary<GeometryType, Dictionary<string, List<LayerItemEntry>>> idGroupped,
      Dictionary<string, int[]> missedDescriptions,
      Dictionary<string, Dictionary<int, string>> allDescriptions)
    {
      using (var rl = new ReadLock(m_lock))
      {
        foreach (var kv in idGroupped)
        {
          foreach (var kv_lr in kv.Value)
          {
            Dictionary<int, string> current_cache = m_description_cache.Lookup(kv_lr.Key);
            HashSet<int> missed_desc_ids = new HashSet<int>();
            Dictionary<int, string> found_desc = new Dictionary<int, string>();

            foreach (var entry in kv_lr.Value)
            {
              if (missed_desc_ids.Contains(entry.EntityID)
                || found_desc.ContainsKey(entry.EntityID))
                continue;

              string descr = null;

              if (current_cache.TryGetValue(entry.EntityID, out descr))
                found_desc.Add(entry.EntityID, descr);
              else
                missed_desc_ids.Add(entry.EntityID);
            }
            if (missed_desc_ids.Count > 0)
              missedDescriptions.Add(kv_lr.Key, missed_desc_ids.ToArray());

            allDescriptions.Add(kv_lr.Key, found_desc);
          }
        }
      }
    }
  }
}
