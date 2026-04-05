using System;
using System.ComponentModel;
using System.Data.SqlClient;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using System.Collections.Generic;

namespace AT.Toolbox.MSSQL.Browsing
{
  public partial class SimpleConnectionControl : LocalizableUserControl
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SimpleConnectionControl).Name);

    public SimpleConnectionControl()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      SqlCheckConnectionWork wrk = new SqlCheckConnectionWork();
      wrk.CloseOnFinish = true;
      BackgroundWorkerForm frm = new BackgroundWorkerForm();

      wrk.ConnectionString = sqlServerEditControl1.ConnectionString;
      frm.Work = wrk;

      frm.ShowDialog(this);

      comboBoxEdit1.Enabled = false;
      labelControl3.Enabled = false;
      comboBoxEdit1.Enabled = wrk.Sucess;
      labelControl3.Enabled = comboBoxEdit1.Enabled;
    }

    public ISpecificDBRoutines RoutineHandler { get; set; }

    private void gridLookUpEdit1_EnabledChanged(object sender, EventArgs e)
    {
      if (comboBoxEdit1.Enabled)
      {
        SqlGetBasesWork wrk = new SqlGetBasesWork();
        BackgroundWorkerForm frm = new BackgroundWorkerForm();

        wrk.Routines = RoutineHandler;
        wrk.ConnectionString = sqlServerEditControl1.ConnectionString;
        frm.Work = wrk;

        frm.ShowDialog(this);

        if (null == wrk.Entries)
          return;

        comboBoxEdit1.Properties.Items.Clear();
        
        if( 0 == wrk.Entries.Count )
        {
          Log.Error("gridLookUpEdit1_EnabledChanged(): no valid databases");
          //Log.Log.GetLogger(  ).Error( "Нет корректных баз данных" ); // TODO: MessageService
        }
        foreach (DatabaseEntry baseEntry in wrk.Entries)
        {
          if( 3 == baseEntry.Supported )
            comboBoxEdit1.Properties.Items.Add(baseEntry.Name);
        }
      }
    }

    protected string GetSelectedConnectionString()
    {
      try
      {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder( sqlServerEditControl1.ConnectionString );
        builder.InitialCatalog = comboBoxEdit1.Text;
        builder.MultipleActiveResultSets = true;
        return builder.ConnectionString;
      }
      catch (Exception ex)
      {
        Log.Error("GetSelectedConnectionString(): exception", ex);
      }
      
      return null;
    }

    [Description("Gets or sets the editor's value."),
         Bindable(true), Localizable(true),
         RefreshProperties(RefreshProperties.All), DefaultValue(null)]
    public string CurrentDatabase
    {
      get
      {
        return GetSelectedConnectionString();
      }
      set
      {
        try
        {
          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value);

          string initial_catalog = builder.InitialCatalog;
          builder.Remove("Initial Catalog");

          if (!string.IsNullOrEmpty(builder.ConnectionString))
          {
            sqlServerEditControl1.ConnectionString = builder.ConnectionString;
          }

          comboBoxEdit1.EditValue = initial_catalog;
        }
        catch (Exception ex)
        {
          Log.Error("CurrentDatabase.set(): exception", ex);
        }
      }
    }

    public event EventHandler EditValueChanged;

    private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
    {
      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }

    private void sqlServerEditControl1_EditValueChanged(object sender, EventArgs e)
    {
      comboBoxEdit1.EditValue = null;
      comboBoxEdit1.Properties.Items.Clear();
      comboBoxEdit1.Enabled = false;
      labelControl3.Enabled = false;

      if (null != EditValueChanged)
        EditValueChanged(this, e);
    }
  }
}
