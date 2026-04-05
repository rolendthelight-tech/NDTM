using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Toolbox.GUI.DX.Dialogs.EditForms
{
  public partial class StringEditForm : XtraForm
  {
    public StringEditForm()
    {
      InitializeComponent();
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

    /// <summary>
    /// Значок
    /// </summary>
    public new Image Icon
    {
      get { return m_picture_box.Image; }
      set { m_picture_box.Image = value; }
    }

    public bool Resizeable
    {
      get { return (FormBorderStyle == FormBorderStyle.SizableToolWindow || FormBorderStyle == FormBorderStyle.Sizable); }
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
        return ( m_text_edit.Properties.PasswordChar != default( char ) );
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