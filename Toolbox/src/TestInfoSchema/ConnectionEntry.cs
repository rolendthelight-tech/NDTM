using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL
{
  [ATConfigurationComposite(Name = "Connection")]
  public class ConnectionEntry
  {
    protected BindingList<ServerEntry> m_entries = new BindingList<ServerEntry>();

    [ATConfigurationList(Name = "CachedServers", ItemType = typeof(ServerEntry))]
    public BindingList<ServerEntry> Entries
    {
      get { return m_entries; }
    }

    [ATConfiguration(Name = "MouldFilePath", Default = "mould.xml")]
    public string MouldFilePath { get; set; }

    [ATConfiguration(Name = "CurrentConnection", Default = "")]
    public string CurrentConnection { get; set; }
  }
}
