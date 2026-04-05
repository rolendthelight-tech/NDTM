using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;
using ATHelpEditor.Properties;

namespace ATHelpEditor
{
  public partial class DocPropertiesForm : LocalizableForm
  {
    public DocPropertiesForm()
    {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public mshtml.IHTMLDocument2 Document { get; set; }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      this.Text = Resources.DOC_PROPERTIES;
      m_group_color.Text = Resources.DOC_PROPERTIES_COLOR;
      m_group_margin.Text = Resources.DOC_PROPERTIES_MARGIN;
      m_group_margin_properties.Text = Resources.DOC_PROPERTIES_MARGIN_PROP;

      m_label_alink_color.Text = Resources.DOC_PROPERTIES_COLOR_ALINK;
      m_label_link_color.Text = Resources.DOC_PROPERTIES_COLOR_LINK;
      m_label_vlink_color.Text = Resources.DOC_PROPERTIES_COLOR_VLINK;
      m_label_bg_color.Text = Resources.DOC_PROPERTIES_BG_COLOR;
      m_label_text_color.Text = Resources.DOC_PROPERTIES_FORE_COLOR;

      m_label_left_margin.Text = Resources.DOC_PROPERTIES_MARGIN_LEFT;
      m_label_right_margin.Text = Resources.DOC_PROPERTIES_MARGIN_RIGHT;
      m_label_top_margin.Text = Resources.DOC_PROPERTIES_MARGIN_TOP;
      m_label_bottom_margin.Text = Resources.DOC_PROPERTIES_MARGIN_BOTTOM;
      m_label_margin_height.Text = Resources.DOC_PROPERTIES_MARGIN_HEIGHT;
      m_label_margin_width.Text = Resources.DOC_PROPERTIES_MARGIN_WIDTH;

      m_btn_cancel.Text = Resources.BUTTON_CANCEL;
    }
    
    private void buttonOK_Click(object sender, EventArgs e)
    {
      if (null == Document)
        return;

      Color cl = (Color)m_bg_color_edit.EditValue;
      string s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);
      Document.body.setAttribute("bgcolor", s, 0);

      cl = (Color)m_text_color_edit.EditValue;
      s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);
      Document.body.setAttribute("text", s, 0);

      cl = (Color)m_link_color_edit.EditValue;
      s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);
      Document.body.setAttribute("link", s, 0);

      cl = (Color)m_alink_color_edit.EditValue;
      s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);
      Document.body.setAttribute("alink", s, 0);

      cl = (Color)m_vlink_color_edit.EditValue;
      s = string.Format("{0:X2}{1:X2}{2:X2}", cl.R, cl.G, cl.B);
      Document.body.setAttribute("vlink", s, 0);

      int i = (int) m_top_margin_edit.EditValue;
      Document.body.setAttribute("topmargin", i, 0);

      i = (int)m_left_margin_edit.EditValue;
      Document.body.setAttribute("leftmargin", i, 0);

      i = (int)m_right_margin_edit.EditValue;
      Document.body.setAttribute("rightmargin", i, 0);

      i = (int)m_bottom_margin_edit.EditValue;
      Document.body.setAttribute("bottommargin", i, 0);

      i = (int)m_margin_width_edit.EditValue;
      Document.body.setAttribute("marginwidth", i, 0);

      i = (int)m_margin_height_edit.EditValue;
      Document.body.setAttribute("marginheight", i, 0);
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      
    }

    private void DocPropertiesForm_Scroll(object sender, ScrollEventArgs e)
    {
    
    }

    private void DocPropertiesForm_Shown(object sender, EventArgs e)
    {
      if (null == Document)
        return;

      object o = Document.body.getAttribute("bgcolor", 0);
      m_bg_color_edit.EditValue = null == o ? Color.White : HtmlWisiwygEdit.GetColor(o);

      o = Document.body.getAttribute("link", 0);
      m_link_color_edit.EditValue = null == o ? Color.Blue : HtmlWisiwygEdit.GetColor(o);

      o = Document.body.getAttribute("alink", 0);
      m_alink_color_edit.EditValue = ( null == o ) ? Color.Red : HtmlWisiwygEdit.GetColor(o);

      o = Document.body.getAttribute("vlink", 0);
      m_vlink_color_edit.EditValue = ( null == o ) ? Color.Maroon : HtmlWisiwygEdit.GetColor(o);

      o = Document.body.getAttribute("text", 0);
      m_text_color_edit.EditValue =( null == o ) ? Color.Black : HtmlWisiwygEdit.GetColor(o);



      o = Document.body.getAttribute("topmargin", 0);
      if (null == o || o is DBNull)
        m_top_margin_edit.EditValue = 0;
      else 
      m_top_margin_edit.EditValue = Int32.Parse(o.ToString());
      
      o = Document.body.getAttribute("leftmargin", 0);

      if (null == o || o is DBNull)
        m_left_margin_edit.EditValue = 0;
      else
        m_left_margin_edit.EditValue = Int32.Parse(o.ToString());

      o = Document.body.getAttribute("rightmargin", 0);
      
      if (null == o || o is DBNull)
        m_right_margin_edit.EditValue = 0;
      else
        m_right_margin_edit.EditValue = Int32.Parse(o.ToString());

      o = Document.body.getAttribute("bottommargin", 0);

      if (null == o || o is DBNull)
        m_bottom_margin_edit.EditValue = 0;
      else
        m_bottom_margin_edit.EditValue = Int32.Parse(o.ToString());

      o = Document.body.getAttribute("marginwidth", 0);

      if (null == o || o is DBNull)
        m_margin_width_edit.EditValue = 0;
      else
        m_margin_width_edit.EditValue = Int32.Parse(o.ToString());

      o = Document.body.getAttribute("marginheight", 0);
      
      if (null == o || o is DBNull)
        m_margin_height_edit.EditValue = 0;
      else
        m_margin_height_edit.EditValue = Int32.Parse(o.ToString());

      
    }
  }
}