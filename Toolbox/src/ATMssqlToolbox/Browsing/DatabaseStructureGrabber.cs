using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections;

namespace AT.Toolbox.MSSQL.Browsing
{
  using System.IO;
  using System.Xml.Serialization;
  //using Microsoft.SqlServer.Management.Smo;
  using System.ComponentModel;
  using AT.Toolbox.MSSQL.Properties;
  using System.Data;

  // Эти классы устарели. Функциональность перенесена в DbMould.MouldBuilder
  /*public class DatabaseDicData
  {
    public Dictionary<string, DatabaseStructureEntry> Tables { get; set; }
    public Dictionary<string, DatabaseStructureEntry> Views { get; set; }
    public Dictionary<string, DatabaseStructureEntry> Procedures { get; set; }


    public Dictionary<string, DatabaseStructureEntry> TablesExt { get; set; }
    public Dictionary<string, DatabaseStructureEntry> ViewsExt { get; set; }

    public DatabaseDicData(DatabaseStructureData data)
    {
      Tables = new Dictionary<string, DatabaseStructureEntry>();
      Views = new Dictionary<string, DatabaseStructureEntry>();
      Procedures = new Dictionary<string, DatabaseStructureEntry>();

      foreach (DatabaseStructureEntry e in data.Tables)
      {
        if (!Tables.ContainsKey(e.Name))
          Tables.Add(e.Name, e);
      }

      foreach (DatabaseStructureEntry e in data.Views)
      {
        if (!Tables.ContainsKey(e.Name))
          Views.Add(e.Name, e);
      }

      foreach (DatabaseStructureEntry e in data.Procedures)
      {
        if (!Tables.ContainsKey(e.Name))
          Procedures.Add(e.Name, e);
      }
    }
  }

  [Serializable]
  public class DatabaseStructureData
  {
    public List<DatabaseStructureEntry> Tables { get; set; }
    public List<DatabaseStructureEntry> Views { get; set; }
    public List<DatabaseStructureEntry> Procedures { get; set; }


    public List<DatabaseStructureEntry> TablesExt { get; set; }
    public List<DatabaseStructureEntry> ViewsExt { get; set; }

    public DatabaseStructureData()
    {
      Tables = new List<DatabaseStructureEntry>();
      Views = new List<DatabaseStructureEntry>();
      Procedures = new List<DatabaseStructureEntry>();
      TablesExt = new List<DatabaseStructureEntry>();
      ViewsExt = new List<DatabaseStructureEntry>();
    }

    public void Save(string FileName)
    {
      if (File.Exists(FileName))
      {
        File.Delete(FileName);
      }
      
      XmlSerializer s = new XmlSerializer(typeof(DatabaseStructureData));
      using (FileStream stream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
      {
        s.Serialize(stream, this);
      }
    }

    public static DatabaseStructureData Load(string FileName)
    {
      XmlSerializer s = new XmlSerializer(typeof(DatabaseStructureData));
      using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        return s.Deserialize(stream) as DatabaseStructureData;
      }
    }
  }

  [Serializable]
  public class DatabaseStructureEntry
  {
    public string Name { get; set; }
    public string Version { get; set; }
    public List<string> Script { get; set; }
    public List<ExtendedPropertyInfo> ExtendedProperties { get; set; }
  }

  [Serializable]
  public class ExtendedPropertyInfo
  {
    public string Name { get; set; }
    public object Value { get; set; }
  }

  public static class DatabaseStructureGrabber
  {
    private static ScriptingOptions m_body_so = new ScriptingOptions { Default = true, IncludeHeaders = false, ScriptDrops = false, AppendToFile = false, DriAll = false, DriPrimaryKey = true };
    private static ScriptingOptions m_ext_so = new ScriptingOptions { Default = false, PrimaryObject = false, IncludeHeaders = false, ScriptDrops = false, AppendToFile = false, DriPrimaryKey = false, DriForeignKeys = true, DriAllConstraints = true, DriChecks = true };

    private static String[] Script(IScriptable ToScript, ScriptingOptions So)
    {
      List<string> strings = new List<string>();

      StringCollection lines = ToScript.Script(So);

      foreach (string s in lines)
        strings.Add(s);

      return strings.ToArray();
    }

    private static void ScriptSMOObject(IScriptable ToScript, out DatabaseStructureEntry Body, out DatabaseStructureEntry Ext)
    {
      Body = null;
      Ext = null;

      try
      {
        string[] script_body = Script(ToScript, m_body_so);
        string[] script_ext = Script(ToScript, m_ext_so);

        if (script_body.Length > 0)
        {
          Body = new DatabaseStructureEntry { Script = new List<string>(), ExtendedProperties = new List<ExtendedPropertyInfo>() };
          foreach (string s in script_body)
          {
            if (!string.IsNullOrEmpty(s))
              Body.Script.Add(s);
          }
        }

        if (script_ext.Length > 0)
        {
          Ext = new DatabaseStructureEntry { Script = new List<string>(), ExtendedProperties = new List<ExtendedPropertyInfo>() };

          foreach (string s in script_ext)
          {
            if (!string.IsNullOrEmpty(s))
              Ext.Script.Add(s);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Log.GetLogger("AppFramework").Error(Resources.FAIL, ex);
      }
    }

    private static void ScriptExtendedProperties(NamedSmoObject NamedObject, DatabaseStructureEntry dbse)
    {
      IExtendedProperties extended_properties = NamedObject as IExtendedProperties;
      string version = null;
      if (extended_properties != null)
      {
        ExtendedProperty version_property = extended_properties.ExtendedProperties["Version"];
        version = null == version_property ? "0.0.0.0" : version_property.Value.ToString();
        dbse.Version = version;

        foreach (ExtendedProperty property in extended_properties.ExtendedProperties)
        {
          dbse.ExtendedProperties.Add(new ExtendedPropertyInfo() { Name = property.Name, Value = property.Value });
        }
      }
    }

    private static void ScriptAll(NamedSmoObject NamedObject, List<DatabaseStructureEntry> Main, List<DatabaseStructureEntry> Ext)
    {
      if (!(NamedObject is IScriptable))
        return;

      DatabaseStructureEntry body;
      DatabaseStructureEntry ext;

      ScriptSMOObject(NamedObject as IScriptable, out body, out ext);

      if (null == body)
        return;

      body.Name = NamedObject.Name;
      ScriptExtendedProperties(NamedObject, body);

      Main.Add(body);

      if (null == Ext || null == ext)
        return;

      ext.Name = NamedObject.Name;
      ScriptExtendedProperties(NamedObject, ext);

      Ext.Add(ext);
    }

    public static void GrabStructure(SqlConnection Conn, BackgroundWorker worker)
    {
      DatabaseStructureData data = new DatabaseStructureData();

      SmoUtil.ProcessServerObjects(Conn, worker, Resources.DB_GRABBER_GETTING_DATA,
        delegate(NamedSmoObject smoObject)
        {
          if (smoObject is Table)
          {
            ScriptAll(smoObject, data.Tables, data.TablesExt);
          }
          else if (smoObject is View)
          {
            ScriptAll(smoObject, data.Views, data.ViewsExt);
          }
          else if (smoObject is StoredProcedure)
          {
            ScriptAll(smoObject, data.Procedures, null);
          }
        });
      data.Save(DatabaseBrowserControl.Preferences.ConfigFilePath);
    }
  }*/
}
