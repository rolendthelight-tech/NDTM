using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Misc;
using AT.Toolbox.Settings;

namespace TestSettings
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      SettingsForm frm = new SettingsForm();
      frm.ShowDialog(this);
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      AppSwitchablePool.SwitchLocale(CultureInfo.GetCultureInfo("RU"));
    }

    private void simpleButton3_Click(object sender, EventArgs e)
    {
      AppSwitchablePool.SwitchLocale(CultureInfo.GetCultureInfo("EN-us"));
    }
  }
}
