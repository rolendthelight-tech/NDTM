using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AT.Toolbox.Dialogs
{
  using Base;
  using System.ComponentModel;
  


  public partial class StringAndChoiceForm : LocalizableForm 
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

    public StringAndChoiceForm()
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
          bool selected = false;
          if (SelectedDefaultValues != null && SelectedDefaultValues.Contains(o.Key))
            selected = true;

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

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Заголовок формы
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Title
    {
      get { return Text; }
      set { Text = value; }
    }

    /// <summary>
    /// Текст для редактирования одной строкой
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Value
    {
      get { return m_text_edit.Text; }
      set { m_text_edit.Text = value; }
    }

    /// <summary>
    /// Возможно ли редактирование текста
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Editable
    {
      get { return !m_text_edit.Properties.ReadOnly; }
      set { m_text_edit.Properties.ReadOnly = !value; }
    }

    public bool Resizeable
    {
      get { return (FormBorderStyle == FormBorderStyle.FixedToolWindow); }
      set
      {
        if (value)
          FormBorderStyle = FormBorderStyle.SizableToolWindow;
        else
          FormBorderStyle = FormBorderStyle.FixedToolWindow;
      }
    }

    public bool Password
    {
      get
      {
        return (m_text_edit.Properties.PasswordChar != default(char));
      }
      set
      {
        if (value)
          m_text_edit.Properties.PasswordChar = '*';
        else
          m_text_edit.Properties.PasswordChar = default(char);
      }
    }

    #endregion

  }
}
