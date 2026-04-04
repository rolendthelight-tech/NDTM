namespace AT.Toolbox.Files
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Xml;


  public class FileEntry
  {
    public string FileName { get; set; }

    public Int64 Offset { get; set; }

    public Int32 Length { get; set; }

    public void Inflate( FileStream file )
    {
      Offset = file.Position;

      byte[] b = File.ReadAllBytes( FileName );
      Length = b.Length;

      file.Write( b, 0, b.Length );
    }

    public virtual void SaveToXml( XmlElement FileElm, XmlDocument Doc )
    {
      XmlAttribute start_att = Doc.CreateAttribute( "Start" );
      start_att.Value = Offset.ToString( );

      XmlAttribute length_att = Doc.CreateAttribute( "Length" );
      length_att.Value = Length.ToString( );

      XmlAttribute file_att = Doc.CreateAttribute( "FileName" );
      file_att.Value = Path.GetFileName( FileName );

      FileElm.Attributes.Append( start_att );
      FileElm.Attributes.Append( length_att );
      FileElm.Attributes.Append( file_att );
    }

    public void LoadFromXml( XmlNode FileNode )
    {
      if( null != FileNode.Attributes["Start"] )
        Offset = Int32.Parse( FileNode.Attributes["Start"].Value );

      if( null != FileNode.Attributes["Length"] )
        Length = Int32.Parse( FileNode.Attributes["Length"].Value );

      if( null != FileNode.Attributes["FileName"] )
        FileName = FileNode.Attributes["FileName"].Value;
    }

    public void SaveFullToXml( XmlElement FileNode, XmlDocument Doc )
    {
      XmlAttribute start_att = Doc.CreateAttribute( "Start" );
      start_att.Value = Offset.ToString( );

      XmlAttribute length_att = Doc.CreateAttribute( "Length" );
      length_att.Value = Length.ToString( );

      XmlAttribute file_att = Doc.CreateAttribute( "FileName" );
      file_att.Value = FileName;

      FileNode.Attributes.Append( start_att );
      FileNode.Attributes.Append( length_att );
      FileNode.Attributes.Append( file_att );
    }

    public void LoadFullFromXml( XmlNode FileNode ) { LoadFromXml( FileNode ); }
  }


  public class FileChunk
  {
    public string ID { get; set; }

    public List<FileEntry> Entries { get; protected set; }

    public FileChunk( ) { Entries = new List<FileEntry>( ); }

    public virtual void SaveToXml( XmlElement ChunkElm, XmlDocument Doc )
    {
      XmlAttribute id_att = Doc.CreateAttribute( @"ID" );
      id_att.Value = ID;

      ChunkElm.Attributes.Append( id_att );

      XmlElement list_elm = Doc.CreateElement( "FILES" );
      ChunkElm.AppendChild( list_elm );

      foreach( FileEntry ent in Entries )
      {
        XmlElement file_elm = Doc.CreateElement( "ITEM" );
        ent.SaveToXml( file_elm, Doc );

        list_elm.AppendChild( file_elm );
      }
    }

    public virtual void Inflate( FileStream file_stream )
    {
      foreach( FileEntry ent in Entries )
        ent.Inflate( file_stream );
    }

    public virtual FileChunk CreateNew( ) { return new FileChunk( ); }

    public virtual void LoadFromXml( XmlNode node, XmlDocument doc )
    {
      if( node.Attributes["ID"] != null )
        ID = node.Attributes["ID"].Value;

      XmlNode nod = node.SelectSingleNode( "FILES" );

      if( nod != null && nod.HasChildNodes )
      {
        foreach( XmlNode file_node in nod.ChildNodes )
        {
          FileEntry e = new FileEntry( );
          e.LoadFromXml( file_node );
          Entries.Add( e );
        }
      }
    }

    public virtual void SaveFullToXml( XmlElement node_elm, XmlDocument doc )
    {
      XmlAttribute id_att = doc.CreateAttribute( @"ID" );
      id_att.Value = ID;

      node_elm.Attributes.Append( id_att );

      XmlElement list_elm = doc.CreateElement( "FILES" );
      node_elm.AppendChild( list_elm );

      foreach( FileEntry ent in Entries )
      {
        XmlElement file_elm = doc.CreateElement( "ITEM" );
        ent.SaveFullToXml( file_elm, doc );

        list_elm.AppendChild( file_elm );
      }
    }

    public virtual void LoadFullFromXml( XmlNode node, XmlDocument doc )
    {
      if( node.Attributes["ID"] != null )
        ID = node.Attributes["ID"].Value;

      XmlNode nod = node.SelectSingleNode( "FILES" );

      if( nod != null && nod.HasChildNodes )
      {
        foreach( XmlNode file_node in nod.ChildNodes )
        {
          FileEntry e = new FileEntry( );
          e.LoadFullFromXml( file_node );
          Entries.Add( e );
        }
      }
    }
  }


  public class FlatFile
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FlatFile).Name);

    string m_file_path;

    bool m_new;

    FileStream m_file;

    readonly BindingList<FileChunk> m_nodes = new BindingList<FileChunk>( );

    public BindingList<FileChunk> Chunks
    {
      get { return m_nodes; }
    }

    public bool Create( string FilePath )
    {
      if( null != m_file )
        Close( );

      try
      {
        m_file_path = FilePath;

        if( File.Exists( FilePath ) )
          m_file = new FileStream( FilePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite );
        else
          m_file = new FileStream( FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite );

        m_new = true;

        return true;
      }
      catch( Exception ex)
      {
        Log.Error(string.Format("Create({0}): exception", FilePath), ex);
        return false;
      }
    }

    public void Close( )
    {
      if( null == m_file )
        return;

      m_file.Close( );
      m_file = null;
    }

    public bool Inflate( )
    {
      if( !m_new )
        return false;

      try
      {
        // Saving binary file
        foreach( FileChunk node in m_nodes )
          node.Inflate( m_file );


        // Saving Header
        XmlDocument doc = new XmlDocument( );

        XmlElement root_elm = doc.CreateElement( @"ROOT" );
        doc.AppendChild( root_elm );

        foreach( FileChunk node in m_nodes )
        {
          XmlElement node_elm = doc.CreateElement( @"NODE" );

          node.SaveToXml( node_elm, doc );

          root_elm.AppendChild( node_elm );
        }
        doc.Save(m_file_path + @".xml");

        return true;
      }
      catch( Exception ex)
      {
        Log.Error("Inflate(): exception", ex);
        return false;
      }
    }

    public void Deflate( string ID, string TargetDir )
    {
      FileChunk found = null;

      foreach( FileChunk chunk in m_nodes )
      {
        if( chunk.ID.ToLower( ) != ID.ToLower( ) )
          continue;

        found = chunk;
        break;
      }

      if( null == found )
        return;

      using( FileStream fs = new FileStream( m_file_path, FileMode.Open, FileAccess.Read, FileShare.Read ) )
      {
        foreach( FileEntry entry in found.Entries )
        {
          fs.Seek( entry.Offset, SeekOrigin.Begin );

          byte[] b = new byte[entry.Length];

          fs.Read( b, 0, entry.Length );

          string path = Path.Combine( TargetDir, entry.FileName );
          
          if( File.Exists( path ))
            File.Delete( path );

          File.WriteAllBytes( path, b );
        }
      }
    }

    public void Open( string Name, FileChunk ChunkType )
    {
      m_file_path = Name;

      m_nodes.Clear( );

      string path = Name + ".xml";

      if( !File.Exists( path ) )
        return;

      XmlDocument doc = new XmlDocument( );
      doc.Load( Name + ".xml" );

      try
      {
        foreach( XmlNode node in doc.SelectNodes( "ROOT/NODE" ) )
        {
          FileChunk chunk = ChunkType.CreateNew( );
          chunk.LoadFromXml( node, doc );

          m_nodes.Add( chunk );
        }
      }
      catch( Exception ex)
      {
        Log.Error(string.Format("Open({0}, {1}): exception", Name, ChunkType), ex);
      }
    }
  }
}