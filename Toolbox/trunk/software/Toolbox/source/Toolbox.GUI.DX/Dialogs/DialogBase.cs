using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using Toolbox.GUI.DX.Base;

namespace Toolbox.GUI.DX.Dialogs
{
  public partial class DialogBase : LocalizableForm
  {
    public DialogBase()
    {
      InitializeComponent();
      base.Text = "";
    }

    public bool HasBottomBorder
    {
      get { return m_bottom_panel.BorderStyle != BorderStyles.NoBorder; }
      set { m_bottom_panel.BorderStyle = value ? BorderStyles.Default : BorderStyles.NoBorder; }
    }

    public MessageBoxButtons Buttons
    {
      get { return m_buttons.Buttons; }
      set { m_buttons.Buttons = value; }
    }
  }
}