using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Base;
using ATHelpEditor.Properties;
using DevExpress.XtraPrinting;
using DevExpress.Utils;

namespace ATHelpEditor
{
  public partial class AddNewStyleDialog : LocalizableForm
  {
    public AddNewStyleDialog()
    {
      InitializeComponent();

      m_font_edit.EditValue = m_memo_demo.Font.Name;
      m_font_size_list.EditValue = Math.Round((m_memo_demo.Font.Size - 3) / 3);
      m_alignment_edit.Properties.Items.AddRange(Enum.GetValues(typeof(HorzAlignment)));
      m_alignment_edit.EditValue = m_memo_demo.Properties.Appearance.TextOptions.HAlignment;
      m_color_edit.EditValue = m_memo_demo.Properties.Appearance.ForeColor;
    }

    private void HandleUpdateDemo(object sender, EventArgs e)
    {
      FontStyle font_style = FontStyle.Regular;

      if (m_check_bold.Checked)
      {
        font_style = font_style | FontStyle.Bold;
      }
      if (m_check_italic.Checked)
      {
        font_style = font_style | FontStyle.Italic;
      }
      if (m_check_underline.Checked)
      {
        font_style = font_style | FontStyle.Underline;
      }

      try
      {
        m_memo_demo.Font = new Font(m_font_edit.EditValue.ToString(),
          int.Parse(m_font_size_list.EditValue.ToString()) * 3 + 3, font_style);
        m_memo_demo.Properties.Appearance.TextOptions.HAlignment = (HorzAlignment)m_alignment_edit.EditValue;
        m_memo_demo.Properties.Appearance.ForeColor = (Color)m_color_edit.EditValue;
      }
      catch { }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_label_alignment.Text = Resources.ADDSTYLE_ALIGNMENT;
      m_label_bold.Text = Resources.WISIWYG_BOLD;
      m_label_color.Text = Resources.ADDSTYLE_COLOR;
      m_label_demo.Text = Resources.ADDSTYLE_DEMO;
      m_label_font.Text = Resources.ADDSTYLE_FONT;
      m_label_font_size.Text = Resources.ADDSTYLE_FONTSIZE;
      m_label_italic.Text = Resources.WISIWYG_ITALIC;
      m_label_name.Text = Resources.ADDSTYLE_NAME;
      m_label_underline.Text = Resources.WISIWYG_UNDERLINE;
      m_cmd_cancel.Text = Resources.BUTTON_CANCEL;
    }

    private void CheckFailed(string message, CancelEventArgs e)
    {
      MessageBox.Show(message, Resources.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      e.Cancel = true;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (this.DialogResult == DialogResult.OK)
      {
        if (string.IsNullOrEmpty(m_name_edit.Text))
        {
          this.CheckFailed(Resources.ADDSTYLE_NONAME, e);
        }
        else
        {
          foreach (TextBlockStyle currentStyle in Program.Preferences.TextBlockStyles)
          {
            if (currentStyle.Name == m_name_edit.Text)
            {
              this.CheckFailed(Resources.ADDSTYLE_DUPLICATENAME, e);
              break;
            }
          }
        }
        if (!e.Cancel)
        {
          TextBlockStyle newStyle = new TextBlockStyle();
          newStyle.Name = m_name_edit.Text;
          newStyle.FontSize = int.Parse(m_font_size_list.EditValue.ToString());
          newStyle.Bold = m_check_bold.Checked;
          newStyle.Italic = m_check_italic.Checked;
          newStyle.Underline = m_check_underline.Checked;
          newStyle.Alignment = m_memo_demo.Properties.Appearance.TextOptions.HAlignment;
          newStyle.Color = (Color)m_color_edit.EditValue;
          newStyle.FontName = m_font_edit.EditValue.ToString();
          Program.Preferences.TextBlockStyles.Add(newStyle);
        }
      }
      base.OnFormClosing(e);
    }

    private void m_alignment_edit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
    {
      try
      {
        HorzAlignment alignment = (HorzAlignment)e.Value;
        switch (alignment)
        {
          case HorzAlignment.Center:
            e.DisplayText = Resources.ALIGNMENT_CENTER;
            break;
          case HorzAlignment.Default:
            e.DisplayText = Resources.ALIGNMENT_DEFAULT;
            break;
          case HorzAlignment.Far:
            e.DisplayText = Resources.ALIGNMENT_FAR;
            break;
          case HorzAlignment.Near:
            e.DisplayText = Resources.ALIGNMENT_NEAR;
            break;
        }
      }
      catch { }
    }
  }
}
