using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Misc;
using DevExpress.XtraEditors;

namespace AT.Toolbox.MSSQL
{
  public partial class AddServerForm : LocalizableForm 
  {
    public AddServerForm()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      Text = Properties.Resources.SQL_ADD_SERVER;
      m_check_btn.Text = Properties.Resources.SQL_CHECK_SERVER;
      m_ok_btn.Text = Properties.Resources.OK;
      m_cancel_btn.Text = Properties.Resources.CANCEL;
    }
    
    public string ConnectionString
    {
      get
      {
        return sqlServerEditControl1.ConnectionString;
      }
    }

    private void HandleCheckConnection(object sender, EventArgs e)
    {
      SqlCheckConnectionWork  wrk = new SqlCheckConnectionWork();
      BackgroundWorkerForm frm = new BackgroundWorkerForm();

      wrk.ConnectionString = sqlServerEditControl1.ConnectionString;
      frm.Work = wrk;

      frm.ShowDialog(this);
    }
  }
}