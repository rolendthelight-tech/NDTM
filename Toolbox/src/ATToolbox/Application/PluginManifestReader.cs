using System.IO;
using System.Xml;

namespace AT.Toolbox
{
  public class PluginManifestReader
  {
    protected XmlDocument XDoc;
    protected string ManifestFilePath { get; private set; }

    protected virtual PluginInfo CreatePluginInfo()
    {
      return new PluginInfo();
    }

    public virtual PluginInfo ReadManifest(string filePath)
    {
      ManifestFilePath = filePath;

      PluginInfo info = CreatePluginInfo();

      XDoc = new XmlDocument();
      XDoc.Load(filePath);
      
      var node = XDoc.SelectSingleNode("/plugin/@assembly");
      
      if (node != null)
      {
        info.AssemblyFile = node.InnerText;
        
        if (!Path.IsPathRooted(info.AssemblyFile))
        {
          info.AssemblyFile = Path.Combine(Path.GetDirectoryName(filePath), info.AssemblyFile);
        }
      }
      else
      {
        return null;
      }

      node = XDoc.SelectSingleNode("/plugin/@name");

      info.Name = node != null
                    ? node.InnerText
                    : Path.GetFileNameWithoutExtension(info.AssemblyFile);

      return info;
    }
  }
}