using System.ComponentModel;
using System.Drawing;
using System;
using Toolbox.GUI.DX.Base;

namespace Toolbox.GUI.DX.Dialogs.EditForms
{
  /// <summary>
  /// Форма для редактирования многострочного текста
  /// </summary>
  public partial class MemoEditForm : LocalizableForm
  {
    //TODO: Локализация

    public MemoEditForm()
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
    /// Текст для редактирования массивом строк
    /// </summary>
		[Obsolete("Польза от этого поля сомнительна. Скорее всего лучше использовать поле Value.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] Lines
    {
      get { return m_text_edit.Lines; }
      set { m_text_edit.Lines = value; }
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

    #endregion
  }
}