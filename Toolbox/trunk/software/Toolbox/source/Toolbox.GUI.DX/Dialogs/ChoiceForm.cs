using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Toolbox.GUI.DX.Base;

namespace Toolbox.GUI.DX.Dialogs
{
	public partial class ChoiceForm : LocalizableForm 
  {
    protected RadioGroup m_group;
    protected CheckedListBoxControl m_box;
    protected Dictionary<string, object> m_values = new Dictionary<string, object>();

    public bool AllowMultiSelect { get; set; }

    public Dictionary<string, object> Values
    {
      get { return m_values; }
    }

    public List<string> SelectedDefaultValues { get; set; }

    /// <summary>
    /// Значок
    /// </summary>
    public new Image Icon
    {
      get { return m_picture_box.Image; }
      set { m_picture_box.Image = value; }
    }

    public object SelectedValue
    {
      get
      {
        return null != m_box ? null : m_group.EditValue;
      }
    }

    public List<object> SelectedValues
    {
      get
      {
        if( null != m_box )
        {
          List<object> ret_val = new List<object>();

          foreach( ListBoxItem itm in m_box.CheckedItems )
            ret_val.Add( m_values[itm.Value.ToString( )] );

          return ret_val;
        }

        return null;
      }
    }

    public ChoiceForm()
    {
      InitializeComponent();
    }

    private void HandleLoad(object sender, EventArgs e)
    {
      if( m_values.Count <= 1 )
      {
        Close();
        throw new ApplicationException("Form should have more then 1 vaues defined");
      }

      if (AllowMultiSelect)
      {
        m_box = new CheckedListBoxControl();

        panelControl1.Controls.Add(m_box);
        m_box.Dock = DockStyle.Fill;

        foreach (KeyValuePair<string, object> o in m_values)
        {
        	bool selected = SelectedDefaultValues != null && SelectedDefaultValues.Contains(o.Key);

        	m_box.Items.Add(o.Key, selected);

          Size sz = TextRenderer.MeasureText(o.Key, m_box.Font); //Fix так как автоматически размеры считаются некорректно.

          int w = sz.Width + this.panelControl1.Location.X + 51;
          if (w > Width)
            Width = w;
        }

        Size sz2 = m_box.GetItemRectangle(0).Size;
        Height = (sz2.Height + 2) * m_box.Items.Count + this.panelControl1.Location.Y + 69;
      }
      else
      {
        m_group = new RadioGroup();
        panelControl1.Controls.Add(m_group);
        m_group.Dock = DockStyle.Fill;

        foreach (KeyValuePair<string, object> o in m_values)
        {
          m_group.Properties.Items.Add(new RadioGroupItem(o.Value, o.Key));

          Size sz = TextRenderer.MeasureText(o.Key, m_group.Font); //Fix так как автоматически размеры считаются некорректно.

          int w = sz.Width + this.panelControl1.Location.X + 51;
          if (w > Width)
            Width = w;
        }


        Size sz2 = TextRenderer.MeasureText(m_group.Properties.Items[0].Description, m_group.Font);
        Height = (sz2.Height * 2) * m_group.Properties.Items.Count + this.panelControl1.Location.Y + 44;
      }

    }
  }
}
