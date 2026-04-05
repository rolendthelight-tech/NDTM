using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AT.Toolbox.Base;
using AT.Toolbox.Constants;
using AT.Toolbox.Network;
using DevExpress.XtraEditors.Controls;

namespace AT.Toolbox.MSSQL
{
  public partial class SqlServerEditControl : LocalizableUserControl
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SqlServerEditControl).Name);

    SqlConnectionStringBuilder m_builder = new SqlConnectionStringBuilder();

    public SqlServerEditControl()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      labelControl7.Text = Properties.Resources.SQLEDIT_LOCATION_GROUP;
      labelControl6.Text = Properties.Resources.SQLEDIT_AUTH_GROUP;
      labelControl1.Text = Properties.Resources.SQLEDIT_COMPUTER;
      checkEdit1.Text = Properties.Resources.SQLEDIT_ALTSEARCH;

      labelControl3.Text = Properties.Resources.SQLEDIT_AUTHTYPE;
      labelControl4.Text = Properties.Resources.SQLEDIT_LOGIN;
      labelControl5.Text = Properties.Resources.SQLEDIT_PASS;

      m_auth_edit.Properties.Items.Clear();
      m_auth_edit.Properties.Items.Add(Properties.Resources.SQLEDIT_AUTHTYPE_WIN);
      m_auth_edit.Properties.Items.Add(Properties.Resources.SQLEDIT_AUTHTYPE_SQL);

      m_auth_edit.SelectedIndex = 0;
    }

    private void checkEdit1_CheckedChanged(object sender, EventArgs e)
    {
      RefreshPClist();
    }

    protected void RefreshPClist()
    {
      UseWaitCursor = true ;
   
      List<string> strings; 

      if( checkEdit1.Checked )
        strings = NetLister.GetSQLServers();
      else
        strings = NetLister.GetNetworkComputers(ComputerTypes.Sqlserver);

      m_server_edit.Properties.Items.Clear();

      foreach (string s in strings)
        m_server_edit.Properties.Items.Add(s);
      
      UseWaitCursor = false;
    }

    private void comboBoxEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if( e.Button.Kind == ButtonPredefines.Combo &&  m_server_edit.Properties.Items.Count == 0 )
      {
        RefreshPClist();
        m_server_edit.ShowPopup();
      }
      
      if( e.Button.Kind == ButtonPredefines.Glyph )
      {
        m_server_edit.ClosePopup();
        RefreshPClist();
      }
    }

    public string ConnectionString
    {
      get
      {
        try
        {
          m_builder.DataSource = m_server_edit.EditValue.ToString();
          if (string.IsNullOrEmpty(m_builder.DataSource))
          {
            m_builder.DataSource = ".";
          }
          m_builder.InitialCatalog = "master";
          if( 0 == m_auth_edit.SelectedIndex )
          {
            m_builder.IntegratedSecurity = true;
            m_builder.PersistSecurityInfo = false;
          }
          else
          {
            m_builder.IntegratedSecurity = false ;
            m_builder.PersistSecurityInfo = true;

            m_builder.UserID = m_username_edit.Text;
            m_builder.Password = m_pass_edit.Text;
          }
          if (m_networklibrary_edit.EditValue != null)
            m_builder.NetworkLibrary = m_networklibrary_edit.EditValue.ToString();
          else
            m_builder.Remove("Network Library"); 

          return m_builder.ConnectionString;
        }
        catch (Exception ex)
        {
          Log.Error("ConnectionString.get(): exception", ex);
          //ex.ToString();
        }
        return "";
      }
      set
      {
        try
        {
          m_builder.ConnectionString = value;
          var copy_m_builder = new SqlConnectionStringBuilder();
          copy_m_builder.ConnectionString = value;

          string DataSource = copy_m_builder.DataSource;

          //string[] values = DataSource.Split(@"\".ToCharArray());

          //string ServerName = values[0];
          //string InstanceName = "";

          //if (values.Length > 1)
          //  InstanceName = values[1];

          m_server_edit.EditValue = DataSource;

          if (copy_m_builder.IntegratedSecurity)
            m_auth_edit.SelectedIndex = 0;
          else
          {
            m_auth_edit.SelectedIndex = 1;

            m_username_edit.EditValue = copy_m_builder.UserID;
            m_pass_edit.EditValue = copy_m_builder.Password;
          }
          if (string.IsNullOrEmpty(copy_m_builder.NetworkLibrary))
            m_networklibrary_edit.EditValue = null;
          else
            m_networklibrary_edit.EditValue = copy_m_builder.NetworkLibrary; 

        }
        catch( Exception ex )
        {
          Log.Error("ConnectionString.set(): exception", ex);
          //ex.ToString();
        }
      }
    }

    private void m_auth_edit_SelectedIndexChanged(object sender, EventArgs e)
    {
      bool SQLAuth = (1 == m_auth_edit.SelectedIndex);

      if( !SQLAuth )
      {
        m_username_edit.EditValue = "";
        m_pass_edit.EditValue = "";
      }

      m_username_edit.Enabled = SQLAuth;
      m_pass_edit.Enabled = SQLAuth;
      labelControl4.Enabled = SQLAuth;
      labelControl5.Enabled = SQLAuth;
    }
    public event EventHandler EditValueChanged;

    private void m_username_edit_Validated(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }

    private void m_pass_edit_Validated(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }

    private void m_server_edit_EditValueChanged(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }

    private void m_auth_edit_EditValueChanged(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }

    private void load_connection_types(object sender, EventArgs e)
    {
      List<ObjectTextStore> network_libraries = new List<ObjectTextStore>();
      network_libraries.Add(new ObjectTextStore(null, "<по умолчанию>"));
      //if (ConnectionTypeSharedMemoryAvailable())
      //{
        network_libraries.Add(new ObjectTextStore("dbmslpcn", "Общая память"));
      //}
      network_libraries.Add(new ObjectTextStore("dbnmpntw", "Именованные каналы"));
      network_libraries.Add(new ObjectTextStore("dbmssocn", "TCP/IP (Winsock)"));
      network_libraries.Add(new ObjectTextStore("dbmsrpcn", "Multiprotocol"));
      network_libraries.Add(new ObjectTextStore("dbmsadsn", "AppleTalk"));
      network_libraries.Add(new ObjectTextStore("dbmsgnet", "VIA"));
      network_libraries.Add(new ObjectTextStore("dbmsspxn", "IPX/SPX"));
      network_libraries.Add(new ObjectTextStore("Dbmsvinn", "Banyan Vines"));
      objectTextStoreBindingSource.DataSource = network_libraries;
    }

    //private bool ConnectionTypeSharedMemoryAvailable()
    //{
    //  SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(sqlServerEditControl1.ConnectionString);
    //  string data_source = builder.DataSource;
    //  string machine_name = data_source.Split(new Char[] { '\\' }, 2)[0];
    //  return ((machine_name == ".") ||
    //    (machine_name == "(local)") ||
    //    (machine_name == Environment.MachineName));
    //}

    private void m_networklibrary_edit_EditValueChanged(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }
  }
}

