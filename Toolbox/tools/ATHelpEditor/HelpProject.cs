using System;
using System.IO;
using AT.Toolbox.Help;
using AT.Toolbox.Log;
using Logger = AT.Toolbox.Log.Log;
using System.Windows.Forms;
using ATHelpEditor.Properties;

namespace ATHelpEditor
{
  public class HelpProject
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

    protected static readonly ILog m_log = Logger.GetLogger("HelpProject");

    #endregion

    protected static HelpProject m_instance;
    private string m_path;
    private bool loading;

    public static HelpProject Current
    {
      get
      {
        return m_instance;
      }
    }

    public static HelpProject Open(string directory_name)
    {
      try
      {
        m_instance = new HelpProject(directory_name);
        m_instance.OpenExisting();
      }
      catch (Exception ex)
      {
        m_log.Error(Resources.PROJECT_ERROR, ex);
      }

      return m_instance;
    }

    public static HelpProject New(string path)
    {
      if (Program.Preferences.TemplateFolder == null)
      {
        Program.Preferences.TemplateFolder = @"HelpTmp";
      }
      m_instance = new HelpProject(path);
      m_instance.File.Nodes.Add(new HelpFileNode() { ID = "root", OriginalFolder = Path.Combine(m_instance.ProjectFolder, "root") });
      return m_instance;
    }

    public HelpProject(string path)
    {
      m_path = path;
      this.ProjectFolder = Path.GetDirectoryName(path);
      this.TemplateFolder = Path.Combine(Application.StartupPath, Program.Preferences.TemplateFolder);
      this.File = new HelpFile();
      this.File.Nodes.ListChanged += Nodes_ListChanged;
    }

    public string ProjectFolder { get; protected set; }
    public string TemplateFolder { get; set; }
    public bool Changed { get; set; }
    public HelpFile File { get; protected set; }


    public void OpenExisting()
    {
      loading = true;
      try
      {
        if (System.IO.File.Exists(m_path))
          this.File.OpenProjectFile(ProjectFolder, m_path);

        if (null == m_instance.File || 0 == m_instance.File.Nodes.Count)
        {
          m_instance = null;
          throw new InvalidDataException(Resources.PROJECT_ERROR_EMPTY);
        }
      }
      finally
      {
        loading = false;
      }
    }

    public void Save()
    {
      this.File.SaveProjectFile(ProjectFolder, m_path);
    }

    public void Append(HelpProject prj) { }

    private void Nodes_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      if (!loading)
      {
        this.Changed = true;
      }
    }
  }
}