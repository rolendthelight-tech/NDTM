using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using AT.Toolbox.Files;

namespace AT.Toolbox.Help
{
  public class HelpFileNode : FileChunk, INotifyPropertyChanged
  {
    private string parentId;
    private string originalFolder;
    private string displayName;
    private string locale;
    private bool isAnchor;

    public String ParentID
    {
      get { return parentId; }
      set
      {
        parentId = value;
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged(this, new PropertyChangedEventArgs("ParentID"));
        }
      }
    }
    public string OriginalFolder
    {
      get { return originalFolder; }
      set
      {
        originalFolder = value;
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged(this, new PropertyChangedEventArgs("OriginalFolder"));
        }
      }
    }
    public string DisplayName
    {
      get { return displayName; }
      set
      {
        displayName = value;
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
        }
      }
    }
    public string Locale
    {
      get { return locale; }
      set
      {
        locale = value;
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged(this, new PropertyChangedEventArgs("Locale"));
        }
      }
    }
    public bool IsAnchor
    {
      get { return isAnchor; }
      set
      {
        isAnchor = value;
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged(this, new PropertyChangedEventArgs("IsAnchor"));
        }
      }
    }

    public HelpFileNode()
    {
      IsAnchor = false;
      Locale = "RU";
    }

    public override void SaveToXml(XmlElement ChunkElm, XmlDocument Doc)
    {
      base.SaveToXml(ChunkElm, Doc);

      XmlAttribute parent_id_att = Doc.CreateAttribute("ParentID");
      parent_id_att.Value = ParentID;

      XmlAttribute display_name_att = Doc.CreateAttribute("DisplayName");
      display_name_att.Value = DisplayName;

      XmlAttribute locale_att = Doc.CreateAttribute("Locale");
      locale_att.Value = Locale;

      XmlAttribute anchor_att = Doc.CreateAttribute("Anchor");
      anchor_att.Value = IsAnchor.ToString();

      ChunkElm.Attributes.Append(locale_att);
      ChunkElm.Attributes.Append(parent_id_att);
      ChunkElm.Attributes.Append(display_name_att);
      ChunkElm.Attributes.Append(anchor_att);
    }

    public override void SaveFullToXml(XmlElement node_elm, XmlDocument doc)
    {
      base.SaveFullToXml(node_elm, doc);

      XmlAttribute parent_id_att = doc.CreateAttribute("ParentID");
      parent_id_att.Value = ParentID;

      XmlAttribute display_name_att = doc.CreateAttribute("DisplayName");
      display_name_att.Value = DisplayName;

      XmlAttribute locale_att = doc.CreateAttribute("Locale");
      locale_att.Value = Locale;

      XmlAttribute anchor_att = doc.CreateAttribute("Anchor");
      anchor_att.Value = IsAnchor.ToString();

      XmlAttribute original_folder_att = doc.CreateAttribute("OriginalFolder");
      original_folder_att.Value = OriginalFolder;

      node_elm.Attributes.Append(locale_att);
      node_elm.Attributes.Append(parent_id_att);
      node_elm.Attributes.Append(display_name_att);
      node_elm.Attributes.Append(anchor_att);
      node_elm.Attributes.Append(original_folder_att);
    }

    public override void LoadFullFromXml(XmlNode node, XmlDocument doc)
    {
      base.LoadFromXml(node, doc);

      if (null != node.Attributes["ParentID"])
        ParentID = node.Attributes["ParentID"].Value;

      if (null != node.Attributes["DisplayName"])
        DisplayName = node.Attributes["DisplayName"].Value;

      if (null != node.Attributes["Locale"])
        Locale = node.Attributes["Locale"].Value;

      if (null != node.Attributes["Anchor"])
        IsAnchor = bool.Parse(node.Attributes["Anchor"].Value);

      if (null != node.Attributes["OriginalFolder"])
        OriginalFolder = node.Attributes["OriginalFolder"].Value;
    }


    public override void Inflate(FileStream file_stream)
    {
      if (IsAnchor)
        return;

      List<string> paths = new List<string>(FSUtils.FindFiles(OriginalFolder, null, true));

      Entries.Clear();

      foreach (string path in paths)
      {
        FileEntry e = new FileEntry();
        e.FileName = path;
        Entries.Add(e);
      }

      base.Inflate(file_stream);
    }

    public override FileChunk CreateNew()
    {
      return new HelpFileNode();
    }

    public override void LoadFromXml(XmlNode node, XmlDocument doc)
    {
      base.LoadFromXml(node, doc);

      if (null != node.Attributes["ParentID"])
        ParentID = node.Attributes["ParentID"].Value;

      if (null != node.Attributes["DisplayName"])
        DisplayName = node.Attributes["DisplayName"].Value;

      if (null != node.Attributes["Locale"])
        Locale = node.Attributes["Locale"].Value;

      if (null != node.Attributes["Anchor"])
        IsAnchor = bool.Parse(node.Attributes["Anchor"].Value);
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.parentId))
        return this.ID;
      return this.parentId + "/" + this.ID;
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }

  public class HelpFile
  {
    protected BindingList<HelpFileNode> m_nodes = new BindingList<HelpFileNode>();
    protected FlatFile m_file = new FlatFile();

    public BindingList<HelpFileNode> Nodes
    {
      get { return m_nodes; }
    }

    public void Compile(string name)
    {
      m_file.Chunks.Clear();

      // TODO: Сделать по-человечески
      // Replacing pic URIS
      foreach (HelpFileNode node in m_nodes)
      {
        string doc = "";

        using (StreamReader rd = new StreamReader(Path.Combine(node.OriginalFolder, "index.html"), Encoding.Unicode))
        {
          doc = rd.ReadToEnd( );
          doc = doc.Replace( node.OriginalFolder + "\\", ".\\" );
        }

        if( string.IsNullOrEmpty( doc ))
          continue;

        using (StreamWriter writer = new StreamWriter(Path.Combine(node.OriginalFolder, "index.html"), false, Encoding.Unicode))//(fs, Encoding.Unicode))
        {
          writer.Write(doc);
        }

        m_file.Chunks.Add(node);
      }

      m_file.Create(name);
      m_file.Inflate();
      m_file.Close();

      // Replacing pic URIS Back
      foreach (HelpFileNode node in m_nodes)
      {
        string doc = "";

        using (StreamReader rd = new StreamReader(Path.Combine(node.OriginalFolder, "index.html"), Encoding.Unicode))
        {
          doc = rd.ReadToEnd();
          doc = doc.Replace(".\\", node.OriginalFolder + "\\");
        }

        if (string.IsNullOrEmpty(doc))
          continue;

        using (StreamWriter writer = new StreamWriter(Path.Combine(node.OriginalFolder, "index.html"), false, Encoding.Unicode))//(fs, Encoding.Unicode))
        {
          writer.Write(doc);
        }
      }
    }

    public virtual void Open(string name)
    {
      m_file.Open(name, new HelpFileNode());

      foreach (HelpFileNode nod in m_file.Chunks)
        m_nodes.Add(nod);
    }

    public void Deflate(string value, string folder)
    {
      m_file.Deflate(value, folder);
    }

    string Trim(string rootFolder, string path)
    {
      int pos = path.IndexOf(rootFolder);
      if (pos < 0)
        return path;
      return path.Substring( Math.Min(path.Length, pos + rootFolder.Length + 1) );
    }

    public void SaveProjectFile(string rootFolder, string path)
    {
      XmlDocument doc = new XmlDocument();

      XmlElement root_elm = doc.CreateElement(@"ROOT");
      doc.AppendChild(root_elm);

      foreach (HelpFileNode node in Nodes)
      {
        XmlElement node_elm = doc.CreateElement(@"NODE");

        {
          string t = node.OriginalFolder;
          node.OriginalFolder = Trim(rootFolder, node.OriginalFolder);
          node.SaveFullToXml(node_elm, doc);
          node.OriginalFolder = t;
        }

        root_elm.AppendChild(node_elm);
      }

      doc.Save(path);
    }

    public void OpenProjectFile(string rootFolder, string path)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(path);

      foreach (XmlNode node in doc.SelectNodes("ROOT/NODE"))
      {
        HelpFileNode chunk = new HelpFileNode();
        chunk.LoadFullFromXml(node, doc);
        chunk.OriginalFolder = Path.Combine(rootFolder, chunk.OriginalFolder);

        Nodes.Add(chunk);
      }
    }

    public string GetContextHelpText(string s)
    {
      if (s.Contains("#"))
      {
        string[] strs = s.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

        Deflate(strs[0], ApplicationSettings.Instance.TempPath);

        if (File.Exists(Path.Combine(ApplicationSettings.Instance.TempPath, "index.html")))
        {
          using (StreamReader rd = new StreamReader(Path.Combine(ApplicationSettings.Instance.TempPath, "index.html"), Encoding.UTF8))
          {
            string str = rd.ReadToEnd();

            int startIndex = 0;

            List<string> lines = new List<string>();

            do
            {
              startIndex = str.IndexOf("<A name=" + strs[1], startIndex);

              if (-1 != startIndex)
              {
                int EndIndex = str.IndexOf("</A>", startIndex);

                if (-1 == EndIndex)
                {
                  // TODO:ERROR
                  return "";
                }

                string str2 = str.Substring(startIndex, (EndIndex - startIndex));

                str2 = str2.Replace("<A name=" + strs[1] + ">", "");
                str2 = str2.Replace("</A>", "");

                lines.Add(str2);

                startIndex = EndIndex;
              }
            } while (startIndex != -1);

            return string.Join("\r\n", lines.ToArray());
          }
        }
      }
      else
      {
      }

      return "";
    }

    public void Close()
    {
      m_file.Close();
    }
  }
}