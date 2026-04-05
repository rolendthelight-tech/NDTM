using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.DB;
using AT.Toolbox.Dialogs;

namespace TestFilterForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      m_instance_edit.EditValue = "BORISY4";
      radioGroup1.EditValue = 1;
      m_db_edit.EditValue = "iii";
      m_login_edit.EditValue = "sa";
      m_pass_edit.EditValue = "sa";
      m_table_edit.EditValue = "Files";
    }

    private void textEdit2_EditValueChanged(object sender, EventArgs e)
    {

    }

    private void textEdit3_EditValueChanged(object sender, EventArgs e)
    {

    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      CustomFilterDialog dlg = new CustomFilterDialog();
      dlg.ConnectionString = ConnectionString();
      dlg.DataSourceName = m_table_edit.EditValue.ToString();
      dlg.ShowDialog(this);
    }

    protected string ConnectionString()
    {
      SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();

      b.DataSource = m_instance_edit.EditValue.ToString();
      b.IntegratedSecurity = (0 == (int)radioGroup1.EditValue);
      b.InitialCatalog = m_db_edit.EditValue.ToString();
      b.UserID = m_login_edit.EditValue.ToString();
      b.Password = m_pass_edit.EditValue.ToString();

      return b.ConnectionString;
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {

      SqlConnection conn = new SqlConnection(ConnectionString());

      try
      {
        conn.Open();
        conn.Close();
      }
      catch (Exception)
      {
        MessageBoxEx box = new MessageBoxEx();
        box.Message = "Failure";
        box.StandardIcon = MessageBoxEx.Icons.Error;
        return;
      }

      MessageBoxEx box2 = new MessageBoxEx();
      box2.Message = "Success";
      box2.StandardIcon = MessageBoxEx.Icons.Ok;
    }
  }
}

