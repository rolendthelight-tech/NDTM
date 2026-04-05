using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ERMS.Core.Common;

namespace MapInterfaces
{
  public class ToolTipCache
  {
    private readonly SharedSource<string, Dictionary<int, ToolTipEntry>> m_data =
      new SharedSource<string, Dictionary<int, ToolTipEntry>>((lr) => new Dictionary<int, ToolTipEntry>());
    private readonly LockSource m_lock = new LockSource();

    public ToolTipEntry GetToolTip(string layerName, int geoID, Func<int,ToolTipEntry> lookupMethod )
    {
      if (string.IsNullOrEmpty(layerName) || lookupMethod == null)
        return null;

      var dic = m_data.Lookup(layerName);
      ToolTipEntry ret= null;

      using (var rl = new ReadLock(m_lock))
      {
        if (dic.TryGetValue(geoID, out ret))
          return ret;
      }

      using (var wl = new WriteLock(m_lock))
      {
        if (dic.TryGetValue(geoID, out ret))
          return ret;

        ret = lookupMethod(geoID);

        if (ret != null)
          ret.GeoID = geoID;

        dic.Add(geoID, ret);

        return ret;
      }
    }

    public void Remove(string layerName, int geoID)
    {
      if (string.IsNullOrEmpty(layerName))
        return;
      
      using (var wl = new WriteLock(m_lock))
      {
        m_data.Lookup(layerName).Remove(geoID);
      }
    }

    public void Clear()
    {
      using (var wl = new WriteLock(m_lock))
      {
        m_data.Clear();
      }
    }
  }

  [DataContract]
  public sealed class ToolTipEntry
  {
    [DataMember]
    public byte[] MiniatureData { get; set; }

    [DataMember]
    public string Description { get; set; }

    public int GeoID { get; internal set; }
  }

  public class ToolTipEventArgs : EventArgs
  {
    public ToolTipEntry ToolTip { get; set; }
  }
}
