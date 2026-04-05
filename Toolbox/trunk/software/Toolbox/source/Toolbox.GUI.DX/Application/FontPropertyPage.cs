using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.GUI.DX.Base;
using Toolbox.GUI.DX.Properties;
using Toolbox.GUI.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Toolbox.GUI.DX.Application
{
  public partial class FontPropertyPage : PropertyPage
  {
    private readonly SettingsBindingSource<FontSettings> m_app_settings;
    private readonly Graphics g = Graphics.FromHwnd(IntPtr.Zero);

    public FontPropertyPage()
    {
      InitializeComponent();

      m_app_settings = new SettingsBindingSource<FontSettings>(m_binding_source);
    }

    public override string Group
    {
      get { return Resources.SETTINGS_GENERIC; }
    }

    public override string PageName
    {
      get
      {
        return Resources.FONT_SETTINGS;
      }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      m_font_id_edit.DataSource = FontSettings.FontSetup.ToList();
    }

    public override void GetCurrentSettings()
    {
      base.GetCurrentSettings();

      m_app_settings.GetCurrentSettings();

      m_app_settings.EditingSection.InitFonts();

      this.OnPageSettings.Add(m_app_settings.EditingSection);
    }

    private void m_font_view_CalcRowHeight(object sender, RowHeightEventArgs e)
    {
      var fi = m_font_view.GetRow(e.RowHandle) as FontInfo;

      if (fi != null && fi.Font != null)
        e.RowHeight = (int)Math.Ceiling(g.MeasureString("A", fi.Font).Height);
    }

    public override void ApplySettings()
    {
      m_app_settings.AssignSettings(); 
      base.ApplySettings();

    }

    private void m_font_view_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
    {
      if (e.Column.FieldName == "ID")
        return;

      var value = m_font_view.GetRow(e.RowHandle) as FontInfo;

      if (value != null)
      {
        e.Appearance.Font = value.Font;
        e.Appearance.ForeColor = value.Color;
      }
    }

    private void m_font_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      var value = m_font_view.GetRow(m_font_view.FocusedRowHandle) as FontInfo;

      if (value != null)
      {
        using (var dlg = new FontDialog())
        {
          dlg.Font = value.Font;
          dlg.Color = value.Color.IsEmpty ? Color.Black : value.Color;
          dlg.ShowColor = true;

          if (dlg.ShowDialog(this) == DialogResult.OK)
          {
            value.Font = dlg.Font;
            value.Color = dlg.Color;

            m_font_view.RefreshData();
          }
        }
      }
    }

    private void m_font_edit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
    {
      var font = e.Value as Font;

      if (font != null)
      {
        var list = new List<string>();
        list.Add(font.Name);

        if (font.Bold)
          list.Add("жирный");

        if (font.Italic)
          list.Add("курсив");

        if (font.Underline)
          list.Add("подчёркнутый");

        list.Add(string.Format("{0}pt", font.Size));

        e.DisplayText = string.Join(", ", list);
      }
    }
  }
}
