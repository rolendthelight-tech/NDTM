using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;

using AT.Toolbox.Base;

using DevExpress.XtraEditors;


namespace AT.Toolbox.MSSQL.Authentication
{
  public partial class SqlAuthenticationForm : LocalizableForm
  {
    public SqlAuthenticationForm()
    {
      InitializeComponent();
    }

    public string UserName
    {
      get { return textEdit1.EditValue.ToString(); }
    }

    public string Password
    {
      get { return textEdit2.EditValue.ToString( ); }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      labelControl1.Text = Properties.Resources.SQLEDIT_LOGIN;
      labelControl2.Text = Properties.Resources.SQLEDIT_PASS;
      Text = Properties.Resources.SQLEDIT_AUTH_GROUP;

      simpleButton1.Text = Properties.Resources.OK;
      simpleButton2.Text = Properties.Resources.CANCEL;
    }

    private void HandleValueChanged(object sender, EventArgs e)
    {
      simpleButton1.Enabled = ( textEdit1.EditValue != null && textEdit2.EditValue != null );
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      textEdit1.EditValue = "";
      textEdit2.EditValue = "";
    }
  }
}
