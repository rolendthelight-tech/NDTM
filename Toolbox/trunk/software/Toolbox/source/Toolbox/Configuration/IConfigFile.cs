using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Configuration
{
  public interface IConfigFile
  {
    string this[[NotNull] string section] { get; set; }

    bool TryGetSection([NotNull] string sectionName, [CanBeNull] out string sectionXml);

    void Save();
  }

  public class ConfigFile : IConfigFile
  {
    private readonly string m_file_name;
    private readonly object m_lock = new object();
    private Dictionary<string, string> m_file_cache;

	  [NotNull] private readonly ILog _log = LogManager.GetLogger(typeof(ConfigFile));

		public ConfigFile([NotNull] [PathReference] string fileName)
    {
      if (fileName == null) throw new ArgumentNullException("fileName");
      if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Empty", "fileName");

      m_file_name = fileName;
    }

    public string this[[NotNull] string section]
    {
      get
      {
	      if (section == null) throw new ArgumentNullException("section");

				return this.Sections[section];
      }
	    set
	    {
		    if (section == null) throw new ArgumentNullException("section");

				this.Sections[section] = value;
	    }
    }

    public bool TryGetSection([NotNull] string sectionName, [CanBeNull] out string sectionXml)
    {
	    if (sectionName == null) throw new ArgumentNullException("sectionName");

			return this.Sections.TryGetValue(sectionName, out sectionXml);
    }

	  public void Save()
    {
      lock (m_lock)
      {
        this.LoadFile();

        var file_name = this.GetSaveFilePath();

        _log.DebugFormat("Save(): {0}", file_name);

        if (File.Exists(file_name))
          File.Delete(file_name);

        var dir = Path.GetDirectoryName(file_name);
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);

        using (var fs = new FileStream(file_name, FileMode.Create, FileAccess.Write))
        {
					using (var writer = new XmlTextWriter(fs, Encoding.UTF8) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1, })
          {
            writer.WriteStartDocument();
            writer.WriteStartElement("Configuration");

            foreach (var section in this.Sections.Values)
            {
              using (var sr = new StringReader(section))
              {
                using (var xtr = new XmlTextReader(sr))
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

        _log.DebugFormat("LoadFile(): {0}", file_name);
      }
    }

	  [NotNull]
	  private string GetLoadFilePath()
    {
      return new ConfigFileFinder(null, m_file_name).InputConfigFilePath;
    }

	  [NotNull]
	  private string GetSaveFilePath()
    {
      return new ConfigFileFinder(null, m_file_name).OutputConfigFilePath;
    }
  }
}
