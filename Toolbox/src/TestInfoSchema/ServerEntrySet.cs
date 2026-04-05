using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using AT.Toolbox.Settings;

namespace AT.Toolbox.MSSQL
{
  public class ServerEntrySet : Component, IListSource, ISupportInitialize
  {
    #region Settings

    [ATConfigSection(Name = "ServerEntrySet", XPath = "DATABASE\\SERVERBROWSER")]
    public class Settings : SettingBase
    {
      private Dictionary<string, ConnectionEntry> m_servers = new Dictionary<string, ConnectionEntry>();

      [ATConfigurationDictionary(Name = "Servers", KeyType = typeof(string), ItemType = typeof(ConnectionEntry))]
      public Dictionary<string, ConnectionEntry> Servers
      {
        get { return m_servers; }
      }

      [ATConfiguration(Name = "SaveAuthData", Default = "false")]
      public bool SaveAuthData { get; set; }

      [ATConfiguration(Name = "DeveloperMode", Default = "false")]
      public bool DeveloperMode { get; set; }

      public Settings(Settings value)
      {
        ApplyNoFiring(value);
      }

      public Settings() { }
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    private static ATConfigSection conf_section = new ATConfigSection() 
    {
      Name = "ServerEntrySet",
      XPath = "DATABASE\\SERVERBROWSER"
    };

    /// <summary>
    /// Настройки 
    /// </summary>
    public static Settings Preferences
    {
      get
      {
        if (null == AppInstance.Configurator[conf_section])
          AppInstance.Configurator[conf_section] = new Settings();

        return AppInstance.Configurator[conf_section] as Settings;
      }
    }

    #endregion

    #region Fields

    BindingList<ServerEntry> m_servers = new BindingList<ServerEntry>();

    #endregion

    #region Properties

    public string ConfigurationKey { get; set; }

    public BindingList<ServerEntry> Entries
    {
      get { return m_servers; }
    }

    #endregion

    #region Methods

    public ServerEntry FindOrCreateServer(string ConnectionString)
    {
      ServerEntry existing_entry = (from p in m_servers
                                    where p.ConnectionString == ConnectionString
                                    select p).FirstOrDefault();

      if (null == existing_entry)
      {
        existing_entry = new ServerEntry();
        existing_entry.ConnectionString = ConnectionString;

        m_servers.Add(existing_entry);
      }

      return existing_entry;
    }

    #endregion

    #region IListSource Members

    public bool ContainsListCollection
    {
      get { return false; }
    }

    public System.Collections.IList GetList()
    {
      return m_servers;
    }

    #endregion

    #region ISupportInitialize Members

    void ISupportInitialize.BeginInit()
    {
      if (this.ConfigurationKey == null)
        this.ConfigurationKey = "";
    }

    void ISupportInitialize.EndInit()
    {
      if (Preferences == null)
        return;

      if (this.ConfigurationKey == null)
        return;

      if (!Preferences.Servers.ContainsKey(this.ConfigurationKey))
      {
        Preferences.Servers.Add(this.ConfigurationKey, new ConnectionEntry());
      }
      m_servers = Preferences.Servers[this.ConfigurationKey].Entries;
    }

    #endregion
  }
}
