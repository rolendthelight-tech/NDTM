using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Base;
using AT.Toolbox.Constants;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Dialogs.Edit_Forms;
using ATHelpEditor;
using DevExpress.XtraEditors;
using ATHelpEditor.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace ATHelpEditor
{
  public partial class HtmlWisiwygEdit : LocalizableForm 
  {
    private mshtml.IHTMLDocument2 m_doc;
    public string FileToEdit = "about:blank";
    public string ID = "";

    public HtmlWisiwygEdit()
    {
      InitializeComponent();
      LinkRequired += delegate { };
      BookmarkCreated += delegate { };
      SyncBookmarks += delegate { };
      Changed += OnChanged;
      m_combo_style.Items.Clear();
      m_combo_style.Items.AddRange(Program.Preferences.TextBlockStyles);
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      m_save_cmd.Caption = Resources.WISIWYG_SAVE;
      m_cmd_doc_properties.Caption = Resources.WISIWYG_PROPERTIES;
      m_fore_color_edit.Caption = Resources.WISIWYG_FORE_COLOR;
      m_back_color_edit.Caption = Resources.WISIWYG_BACK_COLOR;
      m_cmd_insert_hr.Caption = Resources.WISIWYG_INSERT_HR;
      m_cmd_insert_ol.Caption = Resources.WISIWYG_INSERT_OL;
      m_cmd_insert_ul.Caption = Resources.WISIWYG_INSERT_UL;
      m_bold_check.Caption = Resources.WISIWYG_BOLD;
      m_italic_cmd.Caption = Resources.WISIWYG_ITALIC;
      m_underline_cmd.Caption = Resources.WISIWYG_UNDERLINE;
      m_cmd_indent.Caption = Resources.WISIWYG_INDENT;
      m_cmd_unindent.Caption = Resources.WISIWYG_UNINDENT;
      m_cmd_align_center.Caption = Resources.WISIWYG_ALIGN_CENTER;
      m_cmd_align_justify.Caption = Resources.WISIWYG_ALIGN_JUSTIFY;
      m_cmd_align_left.Caption = Resources.WISIWYG_ALIGN_LEFT;
      m_cmd_align_right.Caption = Resources.WISIWYG_ALIGN_RIGHT;
      m_font_edit.Caption = Resources.WISIWYG_FONT;
      m_font_size_edit.Caption = Resources.WISIWYG_FONT_SIZE;
      m_cmd_insert_bookmark.Caption = Resources.WISIWYG_INSERT_BOOKMARK;
      m_cmd_insert_image.Caption = Resources.WISIWYG_INSERT_IMAGE;
      m_cmd_insert_link.Caption = Resources.WISIWYG_INSERT_LINK;
      m_bar_color.Text = Resources.WISIWYG_BAR_COLOR;
      m_bar_file.Text = Resources.WISIWYG_BAR_FILE;
      m_bar_font.Text = Resources.WISIWYG_BAR_FONT;
      m_bar_format.Text = Resources.WISIWYG_BAR_FORMAT;
      m_bar_tools.Text = Resources.WISIWYG_BAR_TOOLS;
      m_tab_source.Text = Resources.WISIWYG_CODE;
      m_tab_wisiwyg.Text = Resources.WISIWYG_DESIGNER;

      (m_bold_check.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_BOLD;
      (m_italic_cmd.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_ITALIC;
      (m_underline_cmd.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_UNDERLINE;
      (m_cmd_indent.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INDENT;
      (m_cmd_unindent.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_UNINDENT;
      (m_cmd_align_center.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_ALIGN_CENTER;
      (m_cmd_align_justify.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_ALIGN_JUSTIFY;
      (m_cmd_align_left.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_ALIGN_LEFT;
      (m_cmd_align_right.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_ALIGN_RIGHT;
      (m_cmd_insert_bookmark.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_BOOKMARK;
      (m_cmd_insert_image.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_IMAGE;
      (m_cmd_insert_link.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_LINK;
      (m_cmd_insert_hr.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_HR;
      (m_cmd_insert_ol.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_OL;
      (m_cmd_insert_ul.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.WISIWYG_INSERT_UL;
    }

    public static Color GetColor(object o)
    {
      if (null == o || (o is DBNull))
        return Color.Empty;

      string s;

      if (o is string)
      {
        s = (string)o;
        s = s.Replace("#", "");
      }
      else
        s = string.Format("{0:X6}", (int)o);

      string r_s = "0x" + s.Substring(0, 2);
      string g_s = "0x" + s.Substring(2, 2);
      string b_s = "0x" + s.Substring(4, 2);

      Int32Converter conv = new Int32Converter();

      int r_i = (int)conv.ConvertFrom(r_s);
      int g_i = (int)conv.ConvertFrom(g_s);
      int b_i = (int)conv.ConvertFrom(b_s);

      return Color.FromArgb(r_i, g_i, b_i);
    }

    private void UpdateUI()
    {
      if (null == m_doc)
        return;

      try
      {
        m_bold_check.Checked = m_doc.queryCommandState("Bold");
        m_italic_cmd.Checked = m_doc.queryCommandState("Italic");
        m_underline_cmd.Checked = m_doc.queryCommandState("Underline");

        object o = m_doc.queryCommandValue("FontName");

        if (null != o && !(o is DBNull))
          m_font_edit.EditValue = o.ToString();

        o = m_doc.queryCommandValue("FontSize");

        if (null != o && !(o is DBNull))
          m_font_size_edit.EditValue = o.ToString();

        o = m_doc.queryCommandValue("ForeColor");

        if (null != o && !(o is DBNull))
          m_fore_color_edit.EditValue = GetColor(o);

        o = m_doc.queryCommandValue("BackColor");

        if (null != o && !(o is DBNull))
          m_back_color_edit.EditValue = GetColor(o);

        m_cmd_insert_bookmark.Checked = m_doc.queryCommandEnabled("UnBookmark");
      }
      catch (Exception) { }
    }

    private void OnChanged(object sender, EventArgs e)
    {
      if( IsChanged )
      {
        m_save_cmd.Enabled = true;
        Text = FileToEdit + " *";
      } 
    }

    public string RootDirectory { get; set; }
    
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      m_browser_editor.Navigate(FileToEdit);
      m_browser_editor.Editable = true;
      m_browser_editor.ContentChanged += webBrowser1_ContentChanged;
      m_doc = m_browser_editor.DomDocument;
      
      Text = FileToEdit;

      timer1.Enabled = true;
      timer2.Enabled = true;
    }

    void webBrowser1_ContentChanged(object sender, EventArgs e)
    {
      IsChanged = true;
    }

    bool m_changed;

    public bool IsChanged
    {
      get
      {
        return m_changed;
      }
      set
      {
        if( m_changed == value )
          return;

        m_changed = value;

        Changed(this, EventArgs.Empty);
      }
    }

    public event EventHandler<EventArgs> Changed;

#region Menu Item Command Handling 

    private void ToggleSelectionBold(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("Bold", false, null);
      IsChanged = true;
    }

    private void ToggleSelectionItalic(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("Italic", false, null);
      IsChanged = true;
    }


    private void ToggleSelectionUnderline(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("Underline", false, null);
      IsChanged = true;
    }

    private void SetFontSize(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      String fnt = m_font_size_edit.EditValue.ToString();

      if (null != fnt)
      {
        m_doc.execCommand("FontSize", false, fnt);
        IsChanged = true;
      }
    }

    private void SetFont(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      String fnt = m_font_edit.EditValue.ToString();

      if (null != fnt)
      {
        m_doc.execCommand("FontName", false, fnt);
        IsChanged = true;
      }
    }

    private void SetForeColor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      Color cl = (Color)m_fore_color_edit.EditValue;

      string s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);

      m_doc.execCommand("ForeColor", false, s);

      IsChanged = true;
    }

    private void SetBackColor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      Color cl = (Color)m_back_color_edit.EditValue;

      string s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);

      m_doc.execCommand("BackColor", false, s);

      IsChanged = true;
    }

#endregion


    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();

      if( DialogResult.OK == ofd.ShowDialog() )
        m_browser_editor.Navigate( ofd.FileName );
    }

    private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SavePage();
    }

    private void OnInsertLink(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      LinkRequired(this, new ParamEventArgs<string>(""));
    }

    public event EventHandler<ParamEventArgs<string>> LinkRequired;
    
    private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
    {
      try
      {
        if (e.PrevPage == m_tab_source)
        {
          m_doc.execCommand("SelectAll", false, false);
          m_doc.execCommand("Delete", false, false);
          m_doc.write(memoEdit1.Text);
        }
        else
          memoEdit1.Text = m_browser_editor.DocumentText;
      }
      catch (Exception)
      {
        
      }
    }

    public void InsertHyperlink(string value)
    {
      m_doc.execCommand("CreateLink", false, value);
      IsChanged = true;
    }

    private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Filter = "Image Files|*.bmp;*.png;*.gif;*.jpg|All Files|*.*";

      if( DialogResult.OK == ofd.ShowDialog() )
      {
        string URL =  Path.GetDirectoryName(FileToEdit) ;
        string filename = Path.GetFileName(ofd.FileName);
        string URL2 = Path.Combine(URL, filename);

        while( File.Exists( URL2 ) )
        {
          MessageBoxEx mbe = new MessageBoxEx("Ошибка", "Файл с таким именем уже существует в каталоге " + URL + "\nНажмите ДА для перезаписи, \nНет для ввода другого имени, \nОтмена для отмены вставки картинки", MessageBoxButtons.YesNoCancel, MessageBoxEx.Icons.Error );

          DialogResult res = mbe.ShowDialog( this );

          if( DialogResult.Cancel == res )
            return;
          if( DialogResult.OK == res )
            break;
          
          StringEditForm frm = new StringEditForm(  );
          frm.Title = "Введите имя";
          
          string str2 = Path.GetFileNameWithoutExtension( filename ) + "_(1)" + Path.GetExtension( filename );

          frm.Value = str2;

          if( DialogResult.OK != frm.ShowDialog( this ) )
            return;

          URL2 = Path.Combine(URL, frm.Value );
        }

        if (URL2 != ofd.FileName)
          File.Copy(ofd.FileName, URL2, true);

        m_doc.execCommand("InsertImage", false, URL2);
        IsChanged = true;

        Invalidate();
      }
    }

    private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("Indent", false, false);
      IsChanged = true;
    }

    private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("Outdent", false, false);
      IsChanged = true;
    }

    private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("JustifyFull", false, false);
      IsChanged = true;
    }

    private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("JustifyLeft", false, false);
      IsChanged = true;
    }

    private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("JustifyRight", false, false);
      IsChanged = true;
    }

    private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("JustifyCenter", false, false);
      IsChanged = true;
    }

    private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("InsertHorizontalRule", false, false);
      IsChanged = true;
    }

    private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("InsertOrderedList", false, false);
      IsChanged = true;
    }

    private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      m_doc.execCommand("InsertUnorderedList", false, false);
      IsChanged = true;
    }

    public event EventHandler<ParamEventArgs<string>> BookmarkCreated;
    public event EventHandler<ParamEventArgs<List<string>>> SyncBookmarks;

    public void SavePage()
    {
      string new_file_name = FileToEdit;

      if ("about:blank" == new_file_name)
      {
        SaveFileDialog sfd = new SaveFileDialog();

        if (DialogResult.OK != sfd.ShowDialog())
          return;

        new_file_name = sfd.FileName;
      }

       //FileStream fs;
      
      /*if( File.Exists( new_file_name )) 
        fs = new FileStream(new_file_name, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
      else
        fs = new FileStream(new_file_name, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
      */
      string directory_name = new FileInfo(new_file_name).DirectoryName;
      if (!Directory.Exists(directory_name))
      {
        Directory.CreateDirectory(directory_name);
      }
      using (StreamWriter writer = new StreamWriter(new_file_name, false, Encoding.Unicode))//(fs, Encoding.Unicode))
      {
        writer.WriteLine(m_browser_editor.DocumentText);
      }

      //fs.Close();

      IsChanged = false;
      this.Text = this.Text.Replace(" *", "");
    }

    private void HtmlWisiwygEdit_FormClosing(object sender, FormClosingEventArgs e)
    {
      timer1.Enabled = false;

      if (!IsChanged) 
        return;

      MessageBoxEx message_box = new MessageBoxEx();
      
      message_box.Caption = Resources.UNSAVED_CHANGES;
      message_box.Message = Resources.MODIFIED_FILES;
      
      message_box.Buttons = MessageBoxButtons.YesNoCancel;
      message_box.StandardIcon = MessageBoxEx.Icons.Warning;

      switch (message_box.ShowDialog(this))
      {
        case DialogResult.Yes :
          SavePage();
          return;
        case DialogResult.No:
          return;
        case DialogResult.Cancel:
          e.Cancel = true;
          return;
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void barCheckItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      if(m_cmd_insert_bookmark.Checked)
      {
        StringEditForm frm = new StringEditForm();
        frm.Resizeable = false;
        frm.Title = "Bookmark ID";
        frm.Icon = Properties.Resources._48_pin_blue;

        if (DialogResult.OK == frm.ShowDialog(this))
        {
          mshtml.IHTMLTxtRange txt = (mshtml.IHTMLTxtRange) m_doc.selection.createRange();
          string sel = txt.text;

          m_doc.execCommand("CreateBookmark", false, frm.Value);
          string val = Path.GetDirectoryName(FileToEdit) + ";" + frm.Value + ";" + sel;
          BookmarkCreated(this, new ParamEventArgs<string>(val));
        }
      }
      else
      {
        object o2 = m_doc.execCommand("UnBookmark", false, false );
        SyncElements();
      }
      
      IsChanged = true;
    }

    protected void SyncElements()
    {
      List<string> bookmark_titles = new List<string>();

      foreach (object anchor in m_doc.anchors)
      {
        if (null == anchor)
          continue;

        mshtml.HTMLAnchorElementClass a = anchor as mshtml.HTMLAnchorElementClass;

        if (null == a)
          continue;

        if (!string.IsNullOrEmpty(a.IHTMLAnchorElement_name))
          bookmark_titles.Add(a.IHTMLAnchorElement_name);
      }

      SyncBookmarks(this, new ParamEventArgs<List<string>>(bookmark_titles));
    }

    private void timer2_Tick(object sender, EventArgs e)
    {
      SyncElements();
    }

    private void OnHelpRequested(object sender, HelpEventArgs hlpevent)
    {
      int a = 10;
      a.ToString();
    }

    private void DocumentStyleSetup(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      DocPropertiesForm frm = new DocPropertiesForm();
      frm.Document = m_doc;
      if (frm.ShowDialog(this) == DialogResult.OK)
      {
        this.IsChanged = true;
      }
    }

    private void m_bookmark_check_KeyDown(object sender, KeyEventArgs e)
    {
      this.IsChanged = true;
    }

    private void CreateNewStyle(object sender, ButtonPressedEventArgs e)
    {
      if (e.Button.Kind == ButtonPredefines.Ellipsis)
      {
        AddNewStyleDialog dlg = new AddNewStyleDialog();

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          m_combo_style.Items.Clear();
          m_combo_style.Items.AddRange(Program.Preferences.TextBlockStyles);
        }
      }
    }

    private void m_bar_style_EditValueChanged(object sender, EventArgs e)
    {
      TextBlockStyle newStyle = m_bar_style.EditValue as TextBlockStyle;
      if (newStyle != null)
      {
        switch (newStyle.Alignment)
        {
          case HorzAlignment.Near:
            m_doc.execCommand("JustifyLeft", false, false);
            break;
          case HorzAlignment.Center:
            m_doc.execCommand("JustifyCenter", false, false);
            break;
          case HorzAlignment.Far:
            m_doc.execCommand("JustifyRight", false, false);
            break;
        }
        if (newStyle.FontSize > 0 && newStyle.FontSize < 8)
        {
          m_doc.execCommand("FontSize", false, newStyle.FontSize.ToString());
        }
        if (!string.IsNullOrEmpty(newStyle.FontName))
        {
          m_doc.execCommand("FontName", false, newStyle.FontName);
        }
        if (newStyle.Bold != (bool)m_doc.queryCommandValue("Bold"))
        {
          m_doc.execCommand("Bold", false, null);
        }
        if (newStyle.Italic != (bool)m_doc.queryCommandValue("Italic"))
        {
          m_doc.execCommand("Italic", false, null);
        }
        if (newStyle.Underline != (bool)m_doc.queryCommandValue("Underline"))
        {
          m_doc.execCommand("Underline", false, null);
        }
        if (newStyle.Color != null)
        {
          string s = string.Format("{0:X2}{1:X2}{2:X2}", newStyle.Color.R, newStyle.Color.G, newStyle.Color.B);

          m_doc.execCommand("ForeColor", false, s);
        }
        this.IsChanged = true;
      }
    }
  }
}
