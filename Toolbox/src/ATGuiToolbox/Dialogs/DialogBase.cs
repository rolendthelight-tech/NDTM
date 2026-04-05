using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using AT.Toolbox.Base;

namespace AT.Toolbox.Dialogs
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