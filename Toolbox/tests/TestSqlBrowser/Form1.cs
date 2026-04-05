using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Base;
using AT.Toolbox.MSSQL;

namespace TestSqlBrowser
{
  using AT.Toolbox.Dialogs;
  using AT.Toolbox.MSSQL.DbMould;

  public partial class Form1 : LocalizableForm
  {
    public Form1()
    {
      InitializeComponent();
      this.databaseBrowserControl1.RoutineHandler = new DbMouldRoutines();
    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AppSwitchablePool.SwitchLocale(new CultureInfo("RU"));
    }

    private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AppSwitchablePool.SwitchLocale(new CultureInfo("EN"));
    }

    private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      SettingsForm frm = new SettingsForm();
      frm.ShowDialog(this);
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
      //this.databaseBrowserControl1.CurrentDatabase =
              //@"Data Source=.\SQLEXPRESS;Initial Catalog=Speechbase;Integrated Security=True;Persist Security Info=False";

      barStaticItem1.Caption = databaseBrowserControl1.CurrentDatabase;
    }

    private void databaseBrowserControl1_EditValueChanged(object sender, EventArgs e)
    {
      barStaticItem1.Caption = databaseBrowserControl1.CurrentDatabase;
    }
  }
}
