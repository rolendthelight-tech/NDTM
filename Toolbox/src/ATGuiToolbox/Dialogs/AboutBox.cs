using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Dialogs
{
  using Base;

  using Properties;

  //TODO: Локализация

  /// <summary>
  /// Диалог "О программе".
  /// </summary>
  public partial class AboutBox : LocalizableForm
  {
    public AboutBox()
    {
      InitializeComponent();
    }

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Дополнительная информация
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AdditionalInfo { get; set; }

    /// <summary>
    /// Картинка. 120х500 0Не масштабируется
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image Picture { get; set; }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    private void HandleShown(object sender, EventArgs e)
    {
      if (this.Owner != null)
        m_main_group.Text = Resources.ABOUT_TITLE + "  " + this.Owner.Text;
      
      if (!string.IsNullOrEmpty(AdditionalInfo))
        m_info_edit.Text = AdditionalInfo;

      if (null != Picture)
        m_picture_box.Image = Picture;

      m_module_grid.DataSource = ApplicationInfo.OtherModuleAttributes;
    }

    #endregion

    private void HandleCloseButton(object sender, EventArgs e)
    {
      Close();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_main_group.Text = Resources.ABOUT_TITLE + "  " + ApplicationInfo.MainAttributes.ProductName;
      m_copyright.Text = ApplicationInfo.MainAttributes.Copyright + "  " + ApplicationInfo.MainAttributes.Company;
      m_version.Text = Resources.VERSION_TITLE + "  " + ApplicationInfo.MainAttributes.Version;
      m_add_info.Text = Resources.INFO_TITLE;
      m_modules.Text = Resources.MODULES_TITLE;
      m_close_btn.Text = Resources.CLOSE;
      m_info_edit.Text = ApplicationInfo.MainAttributes.Description;

      m_col_ok.Caption = Resources.ICON;
      m_col_company.Caption = Resources.DEVELOPED_TITLE;
      m_col_version.Caption = Resources.VERSION_TITLE;
      m_col_name.Caption = Resources.NAME;
    }
   
  }
}