using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using AT.Toolbox;
using log4net;

namespace AT.Toolbox
{
  public interface IConfigFile
  {
    string this[string section] { get; set; }

    bool TryGetSection(string sectionName, out string sectionXml);

    void Save();
  }

  public class ConfigFile : IConfigFile
  {
    private readonly string m_file_name;
    private readonly object m_lock = new object();
    private Dictionary<string, string> m_file_cache;

    private readonly ILog _log = LogManager.GetLogger("ConfigFile");

    public ConfigFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentNullException("fileName");

      m_file_name = fileName;
    }

    public string this[string section]
    {
      get { return this.Sections[section]; }
      set { this.Sections[section] = value; }
    }

    public bool TryGetSection(string sectionName, out string sectionXml)
    {
      return this.Sections.TryGetValue(sectionName, out sectionXml);
    }

    public void Save()
    {
      lock (m_lock)
      {
        this.LoadFile();

        var file_name = this.GetSaveFilePath();
        
        _log.Debug("Save(): " + file_name);

        if (File.Exists(file_name))
          File.Delete(file_name);

        var dir = Path.GetDirectoryName(file_name);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        using (FileStream fs = new FileStream(file_name, FileMode.Create, FileAccess.Write))
        {
          using (XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8) { Formatting = Formatting.Indented })
          {
            writer.WriteStartDocument();
            writer.WriteStartElement("Configuration");

            foreach (var section in this.Sections.Values)
            {
              using (StringReader sr = new StringReader(section))
              {
                using (XmlTextReader xtr = new XmlTextReader(sr))
                {
                  if (section.StartsWith("<?"))
                    xtr.Read();

                  writer.WriteNode(xtr, true);
                }
              }
            }

            writer.WriteEndElement();
          }
        }
      }
    }

    private Dictionary<string, string> Sections
    {
      get
      {
        if (m_file_cache != null)
          return m_file_cache;

        lock (m_lock)
        {
          LoadFile();
          return m_file_cache;
        }
      }
    }

    private void LoadFile()
    {
      if (m_file_cache == null)
      {
        m_file_cache = new Dictionary<string, string>();

        var file_name = this.GetLoadFilePath();

        _log.Debug("LoadFile(): " + file_name);

        if (!string.IsNullOrEmpty(file_name) && File.Exists(file_name))
        {
          using (FileStream fs = new FileStream(file_name, FileMode.Open, FileAccess.Read))
          {
            using (XmlTextReader reader = new XmlTextReader(fs))
            {
              while (reader.Read())
              {
                if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                  break;
              }

              while (!reader.EOF)
              {
                if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                  m_file_cache.Add(reader.Name, reader.ReadOuterXml());
                else
                  reader.Read();
              }
            }
          }
        }
      }
    }

    private string GetLoadFilePath()
    {
      return new ConfigFileFinder(null, m_file_name).InputConfigFilePath;
    }

    private string GetSaveFilePath()
    {
      return new ConfigFileFinder(null, m_file_name).OutputConfigFilePath;
    }
  }
}
