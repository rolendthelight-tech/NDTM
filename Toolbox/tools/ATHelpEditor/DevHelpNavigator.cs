using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Dialogs.Edit_Forms;
using AT.Toolbox.Files;
using AT.Toolbox.Help;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using AT.Toolbox.Base;
using ATHelpEditor.Properties;
using AT.Toolbox.Log;

namespace ATHelpEditor
{
  public partial class DevHelpNavigator : LocalizableUserControl
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

    protected static readonly ILog m_log = Log.GetLogger("DevHelpNavigator");

    #endregion
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<HelpFileNode> Nodes
    {
      get
      {
        return helpFileNodeBindingSource.DataSource as BindingList<HelpFileNode>;
      }
      set
      {
        helpFileNodeBindingSource.DataSource = value;
      }
    }

    public DevHelpNavigator()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      this.m_cmd_delete.Caption = Resources.NAVIGATOR_DELETE;
      m_cmd_import.Caption = Resources.NAVIGATOR_IMPORT;
      m_cmd_import_prj.Caption = Resources.NAVIGATOR_IMPORT_PROJECT;
      m_cmd_new.Caption = Resources.NAVIGATOR_NEW_HELP_TOPIC;
      colDisplayName.Caption = Resources.NAVIGATOR_DISPLAY_NAME;
      colID.Caption = Resources.NAVIGATOR_ID;
    }

    private void OnNewTopic(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      TopicEditForm frm = new TopicEditForm();

      if (DialogResult.OK != frm.ShowDialog( AppInstance.MainForm )) 
        return;
     
      HelpFileNode nod = new HelpFileNode {DisplayName = frm.TopicName, ID = frm.TopicID};

      if (string.IsNullOrEmpty(nod.OriginalFolder))
      {
        string str = nod.ID + "." + nod.Locale;
        nod.OriginalFolder = Path.Combine(HelpProject.Current.ProjectFolder, str);

        if (!Directory.Exists(nod.OriginalFolder))
        {
          Directory.CreateDirectory(nod.OriginalFolder);

          try
          {
            FSUtils.CopyFolderContents(HelpProject.Current.TemplateFolder, nod.OriginalFolder);
          }
          catch (Exception ex)
          {
            m_log.Error("Can't copy template", ex);
          }
        }
      }

      if( null == treeList1.FocusedNode )
      {
        Nodes.Add(nod);
        return;
      }

      HelpFileNode parent_nod = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;
        
      if( null == parent_nod )
        return;

      nod.ParentID = parent_nod.ID;

      if ((from p in Nodes
           where p.OriginalFolder == nod.OriginalFolder
           select p).FirstOrDefault() == null)
      {
        Nodes.Add(nod);
      }
    }

    private void OnDeleteTopic(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      HelpFileNode node_to_delete = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;
      if (Nodes.Remove(node_to_delete))
      {
        try
        {
          string filePath1 = Path.Combine(node_to_delete.OriginalFolder, "index.htm");
          string filePath2 = Path.Combine(node_to_delete.OriginalFolder, "index.html");
          if (File.Exists(filePath1))
          {
            File.Delete(filePath1);
          }
          if (File.Exists(filePath2))
          {
            File.Delete(filePath2);
          }
          Directory.Delete(node_to_delete.OriginalFolder);
        }
        catch { }
      }
    }

    private void treeList1_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
    {
      HelpFileNode nod = treeList1.GetDataRecordByNode(e.Node) as HelpFileNode;

      if( null == nod )
        return;

      if( !e.Node.HasChildren )
      {
        if (nod.IsAnchor)
          e.NodeImageIndex = 3;
        else
          e.NodeImageIndex = 2;

        return;
      }

      foreach( TreeListNode child in e.Node.Nodes )
      {
        HelpFileNode n = treeList1.GetDataRecordByNode(child) as HelpFileNode;

        if (null == n)
          break;

        if( n.IsAnchor )
        {
          e.NodeImageIndex = 2;
          return;
        }
      }

      if (e.Node.Expanded)
        e.NodeImageIndex = 1;
      else
        e.NodeImageIndex = 0;
    }

    private void treeList1_DoubleClick(object sender, EventArgs e)
    {
      if( null == treeList1.FocusedNode )
        return;

      HelpFileNode nod = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;

      if( null == nod )
        return;

      if( nod.IsAnchor )
      {
        TreeListNode paretn_nod = treeList1.FocusedNode.ParentNode;
        nod = treeList1.GetDataRecordByNode(paretn_nod) as HelpFileNode;
      }
   
      string FileName = Path.Combine(nod.OriginalFolder, "index.html");
      EditHTMLRequired( this , new ParamEventArgs<string>( FileName ));
    }

    public event EventHandler<ParamEventArgs<string>> EditHTMLRequired;

    public void SyncBookmarks(string edit, List<string> value)
    {
      string s = Path.GetDirectoryName(edit);

      string node_id = "";

      foreach (HelpFileNode node in Nodes)
      {
        if( node.OriginalFolder != s )
          continue;

        node_id = node.ID;
        break;
      }

      if( string.IsNullOrEmpty( node_id) )
        return;

      List<HelpFileNode> nodes_to_remove = new List<HelpFileNode>();

      foreach (HelpFileNode node in Nodes)
      {
        if( node.ParentID != node_id || ! node.IsAnchor )
          continue;

        if(!value.Contains(node.ID))
          nodes_to_remove.Add(node);
      }

      foreach (HelpFileNode node in nodes_to_remove)
        Nodes.Remove(node);
    }

    public HelpMapper HelpMap
    {
      get { return helpMapper1;  }
    }

    public void ShowToolTip(string str)
    {
      toolTipController1.ShowHint(str);
    }

    private void Import(object sender, ItemClickEventArgs e)
    {
      FolderBrowserDialog dlg = new FolderBrowserDialog();

     if( DialogResult.OK != dlg.ShowDialog(this))
       return;

      string filename = Path.Combine(dlg.SelectedPath, "index.html");

      if( !File.Exists(filename))
        return;

      StringEditForm frm = new StringEditForm();

      if( DialogResult.OK != frm.ShowDialog(this))
        return;

      string ID = Path.GetFileName(dlg.SelectedPath);
      
      HelpFileNode nod;
      
      if( ID.Contains("."))
      {
        string[] str = ID.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        nod = new HelpFileNode { DisplayName = frm.Value, ID = str[0] , Locale = str[1]};
        nod.OriginalFolder = dlg.SelectedPath;
      }
      else 
        nod = new HelpFileNode { DisplayName = frm.Value, ID = ID };

      if (null == treeList1.FocusedNode)
      {
        Nodes.Add(nod);
        return;
      }

      HelpFileNode parent_nod = treeList1.GetDataRecordByNode(treeList1.FocusedNode) as HelpFileNode;

      if (null == parent_nod)
        return;

      nod.ParentID = parent_nod.ID;

      Nodes.Add(nod);
    }

    private void ImportProject(object sender, ItemClickEventArgs e)
    {
      var dlg = new OpenFileDialog();
      dlg.Title = "Import project file";
      dlg.Filter = "Project Files|*.xml";

      if (DialogResult.OK == dlg.ShowDialog(this))
      {
        HelpProject prj = new HelpProject(dlg.FileName);
        prj.OpenExisting();

        foreach (HelpFileNode node in prj.File.Nodes)
        {
          HelpProject.Current.File.Nodes.Add( node );
        }
      }
    }
  }
}
