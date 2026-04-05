using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWizard
{
  using AT.Toolbox.Base;
  using AT.Toolbox.Dialogs;

  public partial class Form1 : LocalizableForm 
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Wiz_Click(object sender, EventArgs e)
    {
      WizardForm frm = new WizardForm();
      frm.Pages.Add( new PageController1() );
      frm.Pages.Add(new PageController2());
      frm.Pages.Add(new PageController3());

      frm.ShowDialog();
    }
  }
}
