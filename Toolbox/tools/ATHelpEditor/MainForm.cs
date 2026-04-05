using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Help;
using ATHelpEditor;
using DevExpress.XtraEditors;
using AT.Toolbox.Dialogs;
using ATHelpEditor.Properties;
using AT.Toolbox.Base;

namespace ATHelpEditor
{
  public partial class MainForm : LocalizableForm
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void OnNewProject(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      var dlg = new SaveFileDialog();
      dlg.Title = "New project file";
      dlg.Filter = "Project Files|*.xml";

      if (System.Windows.Forms.DialogResult.OK != dlg.ShowDialog())
        return;

      {
        HelpProject.New(dlg.FileName);
        m_dev_help_browser.Enabled = true;

        m_dev_help_browser.Nodes = HelpProject.Current.File.Nodes;
        m_dev_help_browser.EditHTMLRequired += new EventHandler<AT.Toolbox.ParamEventArgs<string>>(devHelpBrowser1_EditHTMLRequired);
      }
    }

    void devHelpBrowser1_EditHTMLRequired(object sender, AT.Toolbox.ParamEventArgs<string> e)
    {
      foreach (Form form in MdiChildren)
      {
        if( !(form is HtmlWisiwygEdit )) 
          continue;

        HtmlWisiwygEdit editor = form as HtmlWisiwygEdit;

        if (editor.FileToEdit == e.Value)
        {
          ActivateMdiChild(editor);
          return;
        }
      }

      HtmlWisiwygEdit ed = new HtmlWisiwygEdit();
      ed.MdiParent = this;
      ed.SyncBookmarks += new EventHandler<ParamEventArgs<List<string>>>(ed_SyncBookmarks);
      ed.BookmarkCreated += ed_BookmarkCreated;
      ed.LinkRequired += ed_LinkRequired;
      ed.FileToEdit = e.Value;
      ed.Show();
    }

    void ed_SyncBookmarks(object sender, ParamEventArgs<List<string>> e)
    {
      HtmlWisiwygEdit ed = sender as HtmlWisiwygEdit;

      if( null == ed )
        return;

      m_dev_help_browser.SyncBookmarks(ed.FileToEdit, e.Value);
    }

    void ed_LinkRequired(object sender, ParamEventArgs<string> e)
    {
      URIEdit frm = new URIEdit();
      frm.Nodes = m_dev_help_browser.Nodes;

      if( System.Windows.Forms.DialogResult.OK == frm.ShowDialog(this))
      {
        HtmlWisiwygEdit ed = sender as HtmlWisiwygEdit;
        ed.InsertHyperlink( frm.URI );
      }
    }

    void ed_BookmarkCreated(object sender, AT.Toolbox.ParamEventArgs<string> e)
    {
      try
      {
        string[] str = e.Value.Split(";".ToCharArray());

        foreach (HelpFileNode node in HelpProject.Current.File.Nodes)
        {
          if (str[0] != node.OriginalFolder)
            continue;

          HelpFileNode nod = new HelpFileNode();
          nod.IsAnchor = true;
          nod.ParentID = node.ID;
          nod.ID = str[1];
          nod.DisplayName = str[2];
          nod.OriginalFolder = "";

          HelpProject.Current.File.Nodes.Add(nod);
          break;
        }
      }
      catch { }
    }

    private void OnCompile(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (HelpProject.Current == null)
        return;

      SaveFileDialog sfd = new SaveFileDialog();

      sfd.Filter = "AudiTech help file|*.athelp";

      if (System.Windows.Forms.DialogResult.OK == sfd.ShowDialog())
        HelpProject.Current.File.Compile(sfd.FileName);
    }

    private void CompileAndTest(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (HelpProject.Current == null)
        return;

      string path = Path.Combine(AppInstance.Preferences.WorkingFolder, "test.athelp");
      HelpProject.Current.File.Compile(path);

      AppWrapper.ShowHelp( "root" );
    }

    private void m_save_all_cmd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      foreach (Form form in MdiChildren)
      {
        if( !(form is HtmlWisiwygEdit ) )
          continue;

        HtmlWisiwygEdit ed = form as HtmlWisiwygEdit;

        ed.SavePage();
      }
    }

    private void OnSaveProject(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if (HelpProject.Current != null)
      {
        HelpProject.Current.Save();
      }
    }

    private void OpenProject(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      var dlg = new OpenFileDialog();
      dlg.Title = "Open project file";
      dlg.Filter = "Project Files|*.xml";

      if (System.Windows.Forms.DialogResult.OK != dlg.ShowDialog())
        return;

      HelpProject.Open(dlg.FileName);

      if (HelpProject.Current != null)
      {
        m_dev_help_browser.Enabled = true;

        m_dev_help_browser.Nodes = HelpProject.Current.File.Nodes;
        m_dev_help_browser.EditHTMLRequired += new EventHandler<AT.Toolbox.ParamEventArgs<string>>(devHelpBrowser1_EditHTMLRequired);
      }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (HelpProject.Current!= null && HelpProject.Current.Changed)
      {
        MessageBoxEx message_box = new MessageBoxEx();

        message_box.Caption = Resources.UNSAVED_CHANGES;
        message_box.Message = Resources.MODIFIED_PROJECT;

        message_box.Buttons = MessageBoxButtons.YesNoCancel;
        message_box.StandardIcon = MessageBoxEx.Icons.Warning;

        switch (message_box.ShowDialog(this))
        {
          case DialogResult.Yes:
            HelpProject.Current.Save();
            break;
          case DialogResult.Cancel:
            e.Cancel = true;
            break;
        }
      }
    }

    private void m_language_switcher_LanguageChanged(object sender, EventArgs e)
    {
      m_cmd_new_project.Caption = Resources.MF_NEW_PROJECT;
      m_cmd_compile.Caption = Resources.MF_COMPILE;
      m_cmd_compile_test.Caption = Resources.MF_COMPILE_TEST;
      m_cmd_dave_project.Caption = Resources.MF_SAVE_PROJECT;
      m_cmd_new_project.Caption = Resources.MF_NEW_PROJECT;
      m_cmd_open_project.Caption = Resources.MF_OPEN_PROJECT;
      m_cmd_settings.Caption = Resources.MF_SETTINGS;
      m_save_all_cmd.Caption = Resources.MF_SAVE_ALL;
      m_browser_panel.Text = Resources.MF_NODES;
      m_log_panel.Text = Resources.MF_LOG;
    }

    private void m_cmd_settings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      new SettingsForm().ShowDialog(this);
    }
  }
}