using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;
using AT.Toolbox.Dialogs.Edit_Forms;
using AT.Toolbox.MSSQL.Authentication;
using AT.Toolbox.Extensions;
using DevExpress.XtraGrid.Views.Grid;

namespace AT.Toolbox.MSSQL
{
  using Constants;
  using DevExpress.XtraBars;
  using DevExpress.Utils;
  using AT.Toolbox.Settings;
  using System.IO;
  using AT.Toolbox.Files;
  using AT.Toolbox.MSSQL.Properties;
  using System.Runtime.Serialization;

  public partial class DatabaseBrowserControl : LocalizableUserControl
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DatabaseBrowserControl).Name);

    protected string m_cached_string;
    protected bool m_changed;
    protected bool m_multibase;
    protected BindingList<DatabaseEntry> m_bases = new BindingList<DatabaseEntry>();
    private bool userMode;


    #region Settings ------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Настройки
    /// </summary>

    [DataContract]
    public class Settings : ConfigurationSection
    {
      protected BindingList<ServerEntry> m_entries = new BindingList<ServerEntry>();

      [OnDeserializing]
      private void OnDeserializing(StreamingContext context)
      {
        if (m_entries == null)
          m_entries = new BindingList<ServerEntry>();
      }

      [DataMember]
      public BindingList<ServerEntry> Entries
      {
        get { return m_entries; }
      }

      [DataMember]
      [DefaultValue("mould.mould")]
      public string MouldFilePath { get; set; }

      [DataMember]
      [DefaultValue(false)]
      public bool DeveloperMode { get; set; }

      [DataMember]
      [DefaultValue("")]
      public string CurrentConnection { get; set; }

      [DataMember]
      [DefaultValue(false)]
      public bool SaveAuthData { get; set; }

      public event EventHandler PreferencesChanged;

      protected override void BeforeSave()
      {
        base.BeforeSave();

        if (SaveAuthData)
          return;

        SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();

        foreach (ServerEntry entry in m_entries)
        {
          b.ConnectionString = entry.ConnectionString;
          b.UserID = "";
          b.Password = "";

          entry.ConnectionString = b.ConnectionString;
        }

        b.ConnectionString = CurrentConnection;
        b.UserID = "";
        b.Password = "";

        CurrentConnection = b.ConnectionString;
      }

      protected void FireChanged()
      {
        if (null != PreferencesChanged)
          PreferencesChanged(this, EventArgs.Empty);
      }

      public event EventHandler SettingsReady;

      public override void ApplySettings()
      {
        base.ApplySettings();

        if (this.SettingsReady != null)
          this.SettingsReady(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    public static Settings Preferences
    {
      get
      {
        return AppManager.Configurator.GetSection<Settings>();
      }
    }

    #endregion

    public bool UserMode
    {
      get { return userMode; }
      set
      {
        userMode = value;

        m_add_db_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
        m_remove_db_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
        m_reset_db_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;

        m_mould_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
        m_backup_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
        m_restore_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
        m_import_data_cmd.Visibility = userMode ? BarItemVisibility.Never : BarItemVisibility.Always;
      }
    }

    [DefaultValue(false)]
    public bool CacheEntries { get; set; }

    [DefaultValue(false)]
    public bool AllowMultipleBases
    {
      get
      {
        return m_multibase;
      }
      set
      {
        m_multibase = value;

        if (m_multibase)
          UsageEditor.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
        else
          UsageEditor.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ISpecificDBRoutines RoutineHandler { get; set; }


    [Description("Gets or sets the editor's value."),
       Bindable(true), Localizable(true),
       RefreshProperties(RefreshProperties.All), DefaultValue(null)]
    public string CurrentDatabase
    {
      get
      {
        if (m_changed)
          return GetSelectedConnectionString(true);
        return
          m_cached_string;
      }
      set
      {
        try
        {
          m_cached_string = value;

          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value);

          string initial_catalog = builder.InitialCatalog;
          builder.InitialCatalog = "";
          builder.ConnectionString = builder.ConnectionString.Replace("Initial Catalog=", "");

          if (!string.IsNullOrEmpty(builder.ConnectionString))
          {
            DatabaseEntry entry = this.FindOrCreateServer(builder.ConnectionString).FindOrCreateDatabase(initial_catalog);
            entry.Used = true;
            if (!m_bases.Contains(entry))
            {
              m_bases.Add(entry);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error("CurrentDatabase.set(): exception", ex);
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> SelectedBases
    {
      get
      {
        return (from ServerEntry ent in Preferences.Entries
                from DatabaseEntry db in ent.Bases
                where db.Used
                let builder = new SqlConnectionStringBuilder(ent.ConnectionString) { InitialCatalog=db.Name }
                select builder.ConnectionString).ToList();
      }
    }

    public string Type { get; set; }

    public DatabaseBrowserControl()
    {
      InitializeComponent();
      try
      {
        EditValueChanged += delegate { m_changed = true; };
        Preferences.SettingsReady += new EventHandler(Preferences_SettingsReady);
        this.FillMenuFromAssemblies();
      }
      catch (Exception ex)
      {
        Log.Error("DatabaseBrowserControl(): exception", ex);
      }
    }

    private void FillMenuFromAssemblies()
    {
      AssemblyCollector<IDataImporter> collector = new AssemblyCollector<IDataImporter>();

      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!asm.IsAtToolbox())
          continue;

        collector.Collect(asm, AssemblyCollectMode.Interface);
      }

      foreach (IDataImporter importer in collector.Classes)
      {
        string displayName = importer.ToString();
        
        if (importer.GetType().IsDefined(typeof(DisplayNameAttribute), true))
        {
          displayName = (importer.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
            as DisplayNameAttribute).DisplayName;
        }
        BarButtonItem item = new BarButtonItem(m_bar_manager, displayName);
        ImportWrapper wrapper = new ImportWrapper();
        wrapper.Importer = importer;
        wrapper.Control = this;
        item.ItemClick += wrapper.HandleItemClickEvent;
        m_import_data_cmd.ItemLinks.Add(item);
      }
      var types = from asm in AppDomain.CurrentDomain.GetAssemblies()
                  where asm.IsAtToolbox()
                  from Type type in asm.GetTypes()
                  where !type.IsAbstract && typeof(IDataImportWork).IsAssignableFrom(type)
                  select new ImportWorkWrapper
                  {
                    Control = this,
                    ImportType = type,
                    Caption = type.IsDefined(typeof(DisplayNameAttribute), true) ?
                      (type.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
                      as DisplayNameAttribute).DisplayName : type.Name,
                  };

      foreach (ImportWorkWrapper item in types)
      {
        BarButtonItem btn = new BarButtonItem(m_bar_manager, item.Caption);
        btn.ItemClick += item.HandleItemClickEvent;
        m_import_data_cmd.ItemLinks.Add(btn);
      }
    }

    void Preferences_SettingsReady(object sender, EventArgs e)
    {
      if (null != RoutineHandler)
        RoutineHandler.MetaDataFile = Preferences.MouldFilePath;
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);

      m_db_name_col.Caption = Properties.Resources.DB_BROWSER_COL_NAME;
      m_db_used_col.Caption = Properties.Resources.DB_BROWSER_COL_USED;
      m_db_version_column.Caption = Resources.DB_BROWSER_COL_VERSION;

      //

      m_add_server_cmd.Caption = Properties.Resources.DB_BROWSER_ADD_SERVER;
      m_add_server_tip_title.Text = Properties.Resources.DB_BROWSER_ADD_SERVER;
      m_add_server_tip.Text = Properties.Resources.DB_BROWSER_ADD_SERVER_TIP;
      m_add_server_tip.LeftIndent = 6;
      m_add_server_supertip.Items.Add(m_add_server_tip_title);
      m_add_server_supertip.Items.Add(m_add_server_tip);

      m_add_server_cmd.SuperTip = m_add_server_supertip;

      //

      m_refresh_cmd.Caption = Properties.Resources.DB_BROWSER_REFRESH_ALL;// "Refresh All";
      m_refresh_server_tip_title.Text = Properties.Resources.DB_BROWSER_REFRESH_ALL; //"Refresh list";
      m_refresh_server_tip.Text = Properties.Resources.DB_BROWSER_REFRESH_ALL_TIP; //"Refreshes database list for all servers";
      m_refresh_server_tip.LeftIndent = 6;
      m_refresh_server_supertip.Items.Add(m_refresh_server_tip_title);
      m_refresh_server_supertip.Items.Add(m_refresh_server_tip);

      m_refresh_cmd.SuperTip = m_refresh_server_supertip;

      //

      m_remove_server_cmd.Caption = Properties.Resources.DB_BROWSER_REMOVE_SERVER;// "Refresh All";
      m_remove_server_tip_title.Text = Properties.Resources.DB_BROWSER_REMOVE_SERVER; //"Refresh list";
      m_remove_server_tip.Text = Properties.Resources.DB_BROWSER_REMOVE_SERVER_TIP; //"Refreshes database list for all servers";
      m_remove_server_tip.LeftIndent = 6;
      m_remove_server_supertip.Items.Add(m_remove_server_tip_title);
      m_remove_server_supertip.Items.Add(m_remove_server_tip);

      m_remove_server_cmd.SuperTip = m_remove_server_supertip;

      m_cmd_script.Caption = Resources.DB_BROWSER_RUN_SCRIPT;
      m_cmd_script.Hint = Resources.DB_BROWSER_RUN_SCRIPT_TIP;

      m_db_ok_col.Caption = Resources.DB_BROWSER_COL_STATUS;
      //
      (m_add_db_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_ADD_DB;
      (m_add_db_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_ADD_DB_TIP;

      (m_remove_db_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_REMOVE_DB;
      (m_remove_db_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_REMOVE_DB_TIP;

      (m_reset_db_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_RESET_DB;
      (m_reset_db_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_RESET_DB_TIP;

      (m_import_data_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_IMPORT_DATA;
      (m_import_data_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_IMPORT_DATA_TIP;

      m_mould_cmd.Caption = Properties.Resources.DB_BROWSER_TEST;
      (m_mould_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_MOULD;
      (m_mould_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_MOULD_TIP;

      m_backup_cmd.Caption = Properties.Resources.DB_BROWSER_BACKUP;
      (m_backup_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_BACKUP;
      (m_backup_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_BACKUP_TIP;

      m_restore_cmd.Caption = Properties.Resources.DB_BROWSER_RESTORE;
      (m_restore_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_RESTORE;
      (m_restore_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_RESTORE_TIP;

      m_refresh_server_cmd.Caption = Properties.Resources.DB_BROWSER_REFRESH_SERVER;
      (m_refresh_server_cmd.SuperTip.Items[0] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_REFRESH_SERVER;
      (m_refresh_server_cmd.SuperTip.Items[1] as ToolTipItem).Text = Properties.Resources.DB_BROWSER_REFRESH_SERVER_TIP;
    }

    protected ServerEntry FindOrCreateServer(string ConnectionString)
    {
      ServerEntry existing_entry = (from p in Preferences.Entries
                                    where p.ConnectionString == ConnectionString
                                    select p).FirstOrDefault();

      if (null == existing_entry)
      {
        existing_entry = new ServerEntry();
        existing_entry.ConnectionString = ConnectionString;

        Preferences.Entries.Add(existing_entry);
      }

      return existing_entry;
    }

    protected void RefreshServer(int RowID)
    {
      ServerEntry entry = m_server_view.GetRow(RowID) as ServerEntry;

      if (null == entry)
        return;

      try
      {
        SqlGetBasesWork wrk = new SqlGetBasesWork();
        BackgroundWorkerForm frm = new BackgroundWorkerForm();

        entry.ConnectionString = CheckConnectionString( entry.ConnectionString );

        wrk.Routines = RoutineHandler;
        wrk.ConnectionString = entry.ConnectionString;
        frm.Work = wrk;

        frm.ShowDialog(this);

        entry.Bases.Clear();

        if (null == wrk.Entries)
          return;

        foreach (DatabaseEntry baseEntry in wrk.Entries)
        {
          entry.Bases.Add(baseEntry);
          m_bases.Add(baseEntry);
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("RefreshServer({0}): exception", RowID), ex);
      }
      finally
      {
        serverEntryBindingSource.ResetCurrentItem();
        m_server_view.ExpandMasterRow(RowID);
      }
    }

    private ServerEntry GetSelectedServer()
    {
      GridView view = m_server_grid.FocusedView as GridView;

      if (null == view)
        return null;

      ServerEntry ent = view.GetRow(view.FocusedRowHandle) as ServerEntry;

      if (null == ent)
      {
        ent = view.SourceRow as ServerEntry;

        if (null == ent)
          return ent;
      }

      return ent;
    }

    private DatabaseEntry GetSelectedDatabase()
    {
      ServerEntry stub;
      return this.GetSelectedDatabase(out stub);
    }

    private DatabaseEntry GetSelectedDatabase(out ServerEntry ServerEntry)
    {
      ServerEntry = null;

      GridView view = m_server_grid.FocusedView as GridView;

      if (null == view)
        return null;

      DatabaseEntry database_entry = view.GetRow(view.FocusedRowHandle) as DatabaseEntry;

      if (null == database_entry)
        return database_entry;

      ServerEntry = view.SourceRow as ServerEntry;

      return database_entry;
    }

    protected string GetSelectedConnectionString(bool UsedOnly)
    {
      if (!UsedOnly)
      {
        GridView view = m_server_grid.FocusedView as GridView;

        if (null == view)
          return null;

        DatabaseEntry database_entry = view.GetRow(view.FocusedRowHandle) as DatabaseEntry;

        if (null == database_entry)
          return null;

        ServerEntry ent = view.SourceRow as ServerEntry;

        if (null == ent)
          return null;

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ent.ConnectionString);
        builder.InitialCatalog = database_entry.Name;
        builder.MultipleActiveResultSets = true;
        return builder.ConnectionString;
      }

      return (from ServerEntry ent in Preferences.Entries
              from DatabaseEntry db in ent.Bases
              where db.Used
              let builder = new SqlConnectionStringBuilder(ent.ConnectionString) { InitialCatalog = db.Name }
              select builder.ConnectionString).FirstOrDefault();

    }

    protected void HandleListChange(object sender, ListChangedEventArgs e)
    {
      if (e.ListChangedType != ListChangedType.ItemChanged
        || e.PropertyDescriptor.Name != "Used")
        return;

      if (m_multibase)
        return;

      if (InvokeRequired)
      {
        EventHandler<ListChangedEventArgs> h = HandleListChange;
        h.Invoke(sender, e);
        return;
      }

      try
      {
        DatabaseEntry ent = m_bases[e.NewIndex];

        if (ent.Used)
        {
          foreach (DatabaseEntry ent2 in m_bases)
          {
            if (!ReferenceEquals(ent, ent2))
              ent2.Used = false;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("HandleListChange(): exception", ex);
      }
    }

    protected void HandleAddServer(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      AddServerForm frm = new AddServerForm();

      if (DialogResult.OK != frm.ShowDialog(this))
        return;

      FindOrCreateServer(frm.ConnectionString);
      RefreshServer(m_server_view.GetRowHandle(Preferences.Entries.Count - 1));
    }

    protected void HandleRefreshServer(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      RefreshServer(m_server_view.FocusedRowHandle);
    }

    protected void HandleRefreshAll(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      for (int i = 0; i < Preferences.Entries.Count; i++)
        RefreshServer(m_server_view.GetRowHandle(i));

      if (this.EditValueChanged != null)
        this.EditValueChanged(this, EventArgs.Empty);
    }

    protected void HandleRemoveServer(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      ServerEntry entry = m_server_view.GetRow(m_server_view.FocusedRowHandle) as ServerEntry;
      bool changed = false;

      if (null == entry)
        return;

      foreach (DatabaseEntry ent in entry.Bases)
      {
        m_bases.Remove(ent);

        if (ent.Used)
          changed = true;
      }

      Preferences.Entries.Remove(entry);
      if (changed)
      {
        EditValueChanged(this, EventArgs.Empty);
      }
    }

    protected void HandleUsageCheckedChanged(object sender, EventArgs e)
    {
      m_server_grid.FocusedView.CloseEditor();
      EditValueChanged(this, EventArgs.Empty);
    }

    protected void HandleTableListLostFocus(object sender, EventArgs e)
    {
      m_remove_server_cmd.Enabled = false;
      m_refresh_server_cmd.Enabled = false;
      m_add_db_cmd.Enabled = false;

      m_remove_db_cmd.Enabled = true;
      m_reset_db_cmd.Enabled = true;
      m_import_data_cmd.Enabled = true;
      m_mould_cmd.Enabled = true;
      m_backup_cmd.Enabled = true;
      m_restore_cmd.Enabled = true;
      m_cmd_script.Enabled = true;
    }

    protected void HandleServerListGotFocus(object sender, EventArgs e)
    {
      m_remove_server_cmd.Enabled = true;
      m_refresh_server_cmd.Enabled = true;
      m_add_db_cmd.Enabled = true;

      m_remove_db_cmd.Enabled = false;
      m_reset_db_cmd.Enabled = false;
      m_import_data_cmd.Enabled = false;
      m_mould_cmd.Enabled = false;
      m_backup_cmd.Enabled = false;
      m_restore_cmd.Enabled = false;
      m_cmd_script.Enabled = false;
    }

    protected void HandleAddDatabase(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      ServerEntry ent = GetSelectedServer();

      if (null == ent)
        return;

      StringEditForm frm = new StringEditForm();
      frm.Icon = Properties.Resources.p_48_database_add;
      frm.Title = Properties.Resources.DB_BROWSER_ADD_DATABASE;

      //StringAndChoiceForm frm = new StringAndChoiceForm();
      ////frm.Values.Add(Properties.Resources.DB_BROWSER_SOFT_UPDATE, "SoftUpdate");
      ////frm.Values.Add(Properties.Resources.DB_BROWSER_PERFORM_BACKUP, "PerformBackup");
      ////frm.Values.Add(Properties.Resources.DB_BROWSER_SKIP_FK_TEST, "SkipFkTest");
      //frm.Values.Add(Properties.Resources.DB_BROWSER_COMPATIBILITY_MODE_MS_SQL_SERVER_2005, "CompatibilityModeSql2005");
      //frm.Values.Add(Properties.Resources.DB_BROWSER_USE_FILESTREAM, "UseFileStreams");
      //frm.Values.Add(Properties.Resources.DB_BROWSER_CONTINUE_ON_ERROR_IN_SCRIPTS, "ContinueOnErrorInScripts");
      //frm.Text = Properties.Resources.DB_BROWSER_ADD_DATABASE;
      //frm.AllowMultiSelect = true;
      //frm.SelectedDefaultValues = new List<string>
      //{
      //  //Properties.Resources.DB_BROWSER_SOFT_UPDATE,
      //  Properties.Resources.DB_BROWSER_USE_FILESTREAM
      //};

      if (DialogResult.OK != frm.ShowDialog(this))
        return;

      DatabaseEntry db = null;
      bool remove_db = false;
      try
      {
        try
        {
          ent.AddDatabase(frm.Value);
          db = ent.FindOrCreateDatabase(frm.Value);
          m_bases.Add(db);
          remove_db = true;
        }
        catch (Exception ex)
        {
          Log.Error("HandleAddDatabase(): exception", ex);
          remove_db = false;
          return;
        }

        ent.ConnectionString = CheckConnectionString(ent.ConnectionString);

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ent.ConnectionString)
          {
            InitialCatalog = frm.Value
          };

        SqlUpdateStructureWork wrk = new SqlUpdateStructureWork
          {
            SoftUpdate = false,
            CompatibilityModeSql2005 = false,
            UseFileStreams = true,
            ContinueOnErrorInScripts = false,
            SkipFkTest = false,
            Routines = this.RoutineHandler,
            Database = db,
            ConnectionString = builder.ConnectionString,
            ReCreateTables = false
          };

        ////wrk.SoftUpdate = frm.SelectedValues.Contains("SoftUpdate");
        //wrk.CompatibilityModeSql2005 = frm.SelectedValues.Contains("CompatibilityModeSql2005");
        //wrk.UseFileStreams = frm.SelectedValues.Contains("UseFileStreams");
        //wrk.ContinueOnErrorInScripts = frm.SelectedValues.Contains("ContinueOnErrorInScripts");
        ////wrk.SkipFkTest = frm.SelectedValues.Contains("SkipFkTest");

        BackgroundWorkerForm frm2 = new BackgroundWorkerForm {Work = wrk};
        remove_db = frm2.ShowDialog(this) != DialogResult.OK;
      }
      catch (Exception ex)
      {
        remove_db = true;
        throw;
      }
      finally
      {
        if (remove_db)
          try
          {
            ent.RemoveDatabase(db);
          }
          catch (Exception ex)
          {
            Log.Error(string.Format("HandleAddDatabase(): RemoveDatabase(\"{0}\"): exception", db == null ? "" : db.Name), ex);
          }
      }
    }

    protected void HandleRemoveDatabase(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      ServerEntry ent;
      DatabaseEntry database_entry = GetSelectedDatabase(out ent);

      if (null == ent || null == database_entry)
        return;

      if (MessageBox.Show(Resources.DB_BROWSER_REMOVE_DB_CONFIRM, 
          Resources.LOG_LEVEL_WARN, 
          MessageBoxButtons.OKCancel, 
          MessageBoxIcon.Question, 
          MessageBoxDefaultButton.Button2) != DialogResult.OK)
        return;

      try
      {
        ent.RemoveDatabase(database_entry);
      }
      catch (Exception ex)
      {
        Log.Error("HandleRemoveDatabase(): exception", ex);
      }
      finally
      {
        EditValueChanged(this, EventArgs.Empty);
      }
    }

    protected void HandleResetDatabase(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      string conn_string = GetSelectedConnectionString(false);

      if (string.IsNullOrEmpty(conn_string))
        return;

      ChoiceForm frm = new ChoiceForm();
      frm.Values.Add(Properties.Resources.DB_BROWSER_SOFT_UPDATE, "SoftUpdate");
      frm.Values.Add(Properties.Resources.DB_BROWSER_PERFORM_BACKUP, "PerformBackup");
      frm.Values.Add(Properties.Resources.DB_BROWSER_SKIP_FK_TEST, "SkipFkTest");
      frm.Values.Add(Properties.Resources.DB_BROWSER_COMPATIBILITY_MODE_MS_SQL_SERVER_2005, "CompatibilityModeSql2005");
      frm.Values.Add(Properties.Resources.DB_BROWSER_USE_FILESTREAM, "UseFileStreams");
      frm.Values.Add(Properties.Resources.DB_BROWSER_CONTINUE_ON_ERROR_IN_SCRIPTS, "ContinueOnErrorInScripts");
      frm.Values.Add(Properties.Resources.DB_BROWSER_RECREATE_TABLES, "ReCreateTables");
      frm.Text = Properties.Resources.DB_BROWSER_RESET_DB;
      frm.AllowMultiSelect = true;
      frm.SelectedDefaultValues = new List<string>
      {
        Properties.Resources.DB_BROWSER_SOFT_UPDATE,
        Properties.Resources.DB_BROWSER_USE_FILESTREAM
      };

      if (frm.ShowDialog(this) != DialogResult.OK)
      {
        return;
      }

      if (frm.SelectedValues.Contains("PerformBackup"))
      {
        if (!BackupSelectedDatabase())
        {
          return;
        }
      }

      SqlUpdateStructureWork wrk = new SqlUpdateStructureWork
        {
          SoftUpdate = frm.SelectedValues.Contains("SoftUpdate"),
          CompatibilityModeSql2005 = frm.SelectedValues.Contains("CompatibilityModeSql2005"),
          UseFileStreams = frm.SelectedValues.Contains("UseFileStreams"),
          SkipFkTest = frm.SelectedValues.Contains("SkipFkTest"),
          ContinueOnErrorInScripts = frm.SelectedValues.Contains("ContinueOnErrorInScripts"),
          ReCreateTables = frm.SelectedValues.Contains("ReCreateTables"),
          ConnectionString = this.CheckConnectionString(conn_string),
          Routines = this.RoutineHandler,
          Database = this.GetSelectedDatabase()
        };
      BackgroundWorkerForm bwf = new BackgroundWorkerForm {Work = wrk};

      bwf.ShowDialog(this);
    }

    protected void HandleUsageValidating(object sender, CancelEventArgs e)
    {
      GridView view = m_server_grid.FocusedView as GridView;

      if (null == view)
        return;

      DatabaseEntry database_entry = view.GetRow(view.FocusedRowHandle) as DatabaseEntry;

      if (null == database_entry)
        return;

      e.Cancel = (database_entry.Supported < (int)Support.Partial);
    }

    protected void HandleLoad(object sender, EventArgs e)
    {
      if (DesignMode)
        return;

      if (null == RoutineHandler)
        throw new ApplicationException("No Routine handler set");

      Type = "Common";

      serverEntryBindingSource.DataSource = Preferences.Entries;

      m_bases.RaiseListChangedEvents = true;
      m_bases.ListChanged += HandleListChange;

      if (this.CacheEntries)
      {
        foreach (ServerEntry entry in Preferences.Entries)
        {
          foreach (DatabaseEntry baseEntry in entry.Bases)
            m_bases.Add(baseEntry);
        }
      }
      else
      {
        this.FindForm().FormClosing += delegate
        {
          Preferences.Entries.Clear();
        };
      }
    }

    protected void HandleExtendedInfo(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      GridView view = m_server_grid.FocusedView as GridView;

      if (null == view)
        return;

      DatabaseEntry database_entry = view.GetRow(view.FocusedRowHandle) as DatabaseEntry;

      if (null == database_entry)
        return;

      if (string.IsNullOrEmpty(database_entry.TestResults))
        return;

      MemoEditForm frm = new MemoEditForm();
      frm.StartPosition = FormStartPosition.CenterParent;
      frm.Text = Properties.Resources.DB_BROWSER_COL_TEST_RESULTS;
      frm.Value = database_entry.TestResults;
      frm.ShowDialog(this);
    }

    public event EventHandler<EventArgs> EditValueChanged;

    private void m_backup_cmd_ItemClick(object sender, ItemClickEventArgs e)
    {
      BackupSelectedDatabase();
    }

    private bool RunScript()
    {
      string conn_string = GetSelectedConnectionString(false);

      if (string.IsNullOrEmpty(conn_string))
        return false;

      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = "SQL Script|*.sql";

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          var frm = new BackgroundWorkerForm();
          frm.Work = new RunScriptWork
          {
            ConnectionString = conn_string,
            FileName = dlg.FileName
          };

          return frm.ShowDialog(this) == DialogResult.OK;
        }
      }

      return false;
    }

    private static readonly Regex _file_name_escaper_regex = new Regex(
      string.Format(@"[{0}]",
                    Regex.Escape(string.Join(@"", Path.GetInvalidFileNameChars().Select(c => c.ToString()).ToArray()))),
      RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private bool BackupSelectedDatabase()
    {
      string conn_string = GetSelectedConnectionString(false);

      if (string.IsNullOrEmpty(conn_string))
        return false;

      conn_string = CheckConnectionString(conn_string);

      DateTime dt = DateTime.Now;
      SqlConnectionStringBuilder db_string_builder = new SqlConnectionStringBuilder(conn_string);
      string db_name = db_string_builder.InitialCatalog;
      string db_server_name = db_string_builder.DataSource.Split(new char[]{'\\'},2)[0];

      bool db_local_availlable = (db_server_name == ".") || (db_server_name == "?") || (db_server_name == "(local)") || (db_server_name == Environment.MachineName);

      SaveFileDialog dlg = new SaveFileDialog();
      dlg.Filter = "Backup files (*.bak)|*.bak";
      dlg.FileName = string.Format("{0}.{1:D2}.{2:D2}.{3:D2}.bak", _file_name_escaper_regex.Replace(db_name, @"_"), dt.Day, dt.Month, dt.Year);
      dlg.OverwritePrompt = true;
      dlg.CreatePrompt = false;
      dlg.AddExtension = true;
      dlg.DefaultExt = ".bak";
      bool success = false;

      while (!success)
      {
        if (dlg.ShowDialog(this) != DialogResult.OK)
        {
          return false;
        }
        if (!dlg.FileName.EndsWith(".bak"))
          dlg.FileName += ".bak";
        if (!db_local_availlable)
        {
          //var split_path = dlg.FileName.Split(new char[] { '\\' }, StringSplitOptions.None);
          //if (split_path.Length < 4 || split_path[0] != "" || split_path[1] != "" || split_path[2] == "." || split_path[2] == "?" || split_path[3] != "") // Альтернатива регулярному выражению (делает то же самое)
          if (!((new System.Text.RegularExpressions.Regex(@"\A\\\\(?![.?][\\/])[^\\/]+\\[^\\/]+")).Match(dlg.FileName).Success)) // Путь должен быть сетевым, независимым от локальной адресации
          {
            MessageBoxEx mb = new MessageBoxEx(Resources.DB_BROWSER_BACKUP,
              Resources.DB_BROWSER_ONLY_UNC, MessageBoxButtons.OK, MessageBoxEx.Icons.Warning);
            mb.ShowDialog(this);
          }
          else
          {
            success = true;
          }
        }
        else
          success = true;
      }

      if (success)
      {

        SqlBackupWork wrk = new SqlBackupWork();
        wrk.ConnectionString = conn_string;
        wrk.Path = dlg.FileName;
        BackgroundWorkerForm frm = new BackgroundWorkerForm();
        frm.Work = wrk;
        return frm.ShowDialog(this) == DialogResult.OK;
      }
      return success;
    }

    private void m_restore_cmd_ItemClick(object sender, ItemClickEventArgs e)
    {
      string conn_string = GetSelectedConnectionString(false);

      if (string.IsNullOrEmpty(conn_string))
        return;

      conn_string = CheckConnectionString(conn_string);

      SqlConnectionStringBuilder db_string_builder = new SqlConnectionStringBuilder(conn_string);
      string db_server_name = db_string_builder.DataSource.Split(new char[] { '\\' }, 2)[0];

      bool db_local_availlable = (db_server_name == ".") || (db_server_name == "?") || (db_server_name == "(local)") || (db_server_name == Environment.MachineName);
      
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = "Backup files (*.bak)|*.bak";
      bool success = false;

      while (!success)
      {
        if (dlg.ShowDialog(this) != DialogResult.OK)
        {
          return;
        }
        if (!db_local_availlable)
        {
          if (!((new Regex(@"^\\\\(?![.?][\\/])[^\\/]+\\[^\\/]+")).Match(dlg.FileName).Success)) // Путь должен быть сетевым, независимым от локальной адресации
          {
            MessageBoxEx mb = new MessageBoxEx(Resources.DB_BROWSER_RESTORE,
              Resources.DB_BROWSER_ONLY_UNC, MessageBoxButtons.OK, MessageBoxEx.Icons.Warning);
            mb.ShowDialog(this);
          }
          else
          {
            success = true;
          }
        }
        else
          success = true;
      }

      if (success)
      {
        SqlRestoreWork wrk = new SqlRestoreWork();
        wrk.ConnectionString = conn_string;
        wrk.Path = dlg.FileName;
        BackgroundWorkerForm frm = new BackgroundWorkerForm();
        frm.Work = wrk;
        frm.ShowDialog(this);
      }
    }

    private void HandleDataBaseMould(object sender, ItemClickEventArgs e)
    {
      string conn_string = GetSelectedConnectionString(false);

      if (string.IsNullOrEmpty(conn_string))
        return;

      conn_string = CheckConnectionString( conn_string );

      SqlGetMaxVersionWork wrk = new SqlGetMaxVersionWork(this.RoutineHandler);
      wrk.ConnectionString = conn_string;
      BackgroundWorkerForm frm = new BackgroundWorkerForm();
      frm.Work = wrk;
      if (frm.ShowDialog(this) == DialogResult.OK)
      {
        BuildMouldForm bmf = new BuildMouldForm();
        bmf.RoutineHandler = this.RoutineHandler;
        bmf.OldVersion = wrk.Version;
        bmf.NewVersion = wrk.Version;
        bmf.MouldPath = this.RoutineHandler.MetaDataFile;
        if (bmf.ShowDialog(this) == DialogResult.OK)
        {
          SqlBuildMouldWork smw = new SqlBuildMouldWork(this.RoutineHandler);
          smw.Version = bmf.NewVersion;
          smw.MouldPath = bmf.NewMould;
          smw.ConnectionString = conn_string;
          smw.TestScriptObjects = true;
          smw.Database = this.GetSelectedDatabase();

          BackgroundWorkerForm frm2 = new BackgroundWorkerForm();
          frm2.Work = smw;
          frm2.ShowDialog(this);
          if (Path.GetFullPath(this.RoutineHandler.MetaDataFile)
            == Path.GetFullPath(smw.MouldPath))
          {
            this.RoutineHandler.ReloadMetaData();
          }
        }
      }
    }

    public string CheckConnectionString( string value )
    {
      SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(value);

      if (!b.IntegratedSecurity && string.IsNullOrEmpty(b.UserID))
      {
        SqlAuthenticationForm sql_frm = new SqlAuthenticationForm( );

        if( DialogResult.OK == sql_frm.ShowDialog( this ) )
        {
          b.UserID = sql_frm.UserName;
          b.Password = sql_frm.Password;
        }
      }

      return b.ConnectionString;
    }

    private void m_cmd_script_ItemClick(object sender, ItemClickEventArgs e)
    {
      this.RunScript();
    }

    protected void HandleConvertDatabaseToSql2005(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      ServerEntry ent = GetSelectedServer();

      if (null == ent)
        return;

      string initial_catalog;
      //bool softUpdate;
      bool compatibilityModeSql2005;
      bool useFileStreams;
      bool continueOnErrorInScripts;
      //bool skipFkTest;

      {
        StringAndChoiceForm frm = new StringAndChoiceForm
          {
            Icon = Properties.Resources.p_48_database_add,
            Title = Properties.Resources.DB_BROWSER_ADD_DATABASE,
            AllowMultiSelect = true
          };

        //frm.Values.Add(Properties.Resources.DB_BROWSER_SOFT_UPDATE, "SoftUpdate");
        //frm.Values.Add(Properties.Resources.DB_BROWSER_PERFORM_BACKUP, "PerformBackup");
        //frm.Values.Add(Properties.Resources.DB_BROWSER_SKIP_FK_TEST, "SkipFkTest");
        frm.Values.Add(Properties.Resources.DB_BROWSER_COMPATIBILITY_MODE_MS_SQL_SERVER_2005, "CompatibilityModeSql2005");
        frm.Values.Add(Properties.Resources.DB_BROWSER_USE_FILESTREAM, "UseFileStreams");
        frm.Values.Add(Properties.Resources.DB_BROWSER_CONTINUE_ON_ERROR_IN_SCRIPTS, "ContinueOnErrorInScripts");
        frm.Text = Properties.Resources.DB_BROWSER_TRANSFORMATION_MOULD;
        frm.SelectedDefaultValues = new List<string>
        {
          //Properties.Resources.DB_BROWSER_SOFT_UPDATE,
          Properties.Resources.DB_BROWSER_COMPATIBILITY_MODE_MS_SQL_SERVER_2005,
          Properties.Resources.DB_BROWSER_USE_FILESTREAM,
          Properties.Resources.DB_BROWSER_CONTINUE_ON_ERROR_IN_SCRIPTS
        };
        frm.Value = "Имя временной БД";

        if (DialogResult.OK != frm.ShowDialog(this))
          return;

        initial_catalog = frm.Value;
        //softUpdate = frm.SelectedValues.Contains("SoftUpdate");
        compatibilityModeSql2005 = frm.SelectedValues.Contains("CompatibilityModeSql2005");
        useFileStreams = frm.SelectedValues.Contains("UseFileStreams");
        continueOnErrorInScripts = frm.SelectedValues.Contains("ContinueOnErrorInScripts");
        //skipFkTest = frm.SelectedValues.Contains("SkipFkTest");
      }

      DatabaseEntry db = null;
      bool remove_db = true;
      try
      {
        try
        {
          ent.AddDatabase(initial_catalog);
          db = ent.FindOrCreateDatabase(initial_catalog);
          m_bases.Add(db);
          remove_db = true;
        }
        catch (Exception ex)
        {
          Log.Error("HandleConvertDatabaseToSql2005(): exception", ex);
          remove_db = false;
          return;
        }

        ent.ConnectionString = CheckConnectionString(ent.ConnectionString);

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ent.ConnectionString)
          {
            InitialCatalog = initial_catalog
          };

        { // Инициализация временной БД
          SqlUpdateStructureWork wrk = new SqlUpdateStructureWork
            {
              SoftUpdate = false,
              Routines = this.RoutineHandler,
              Database = db,
              ConnectionString = builder.ConnectionString,
              CompatibilityModeSql2005 = compatibilityModeSql2005,
              UseFileStreams = useFileStreams,
              ContinueOnErrorInScripts = continueOnErrorInScripts
            };

          //wrk.SoftUpdate = softUpdate;
          //wrk.SkipFkTest = skipFkTest;

          BackgroundWorkerForm frm2 = new BackgroundWorkerForm {Work = wrk};
          if (frm2.ShowDialog(this) != DialogResult.OK)
            throw new Exception("Не удалость инициализировать временную БД");
        }

        { // Построение слепка по временной БД
          SqlGetMaxVersionWork wrk = new SqlGetMaxVersionWork(this.RoutineHandler)
            {
              ConnectionString = builder.ConnectionString
            };
          BackgroundWorkerForm frm = new BackgroundWorkerForm {Work = wrk};
          if (frm.ShowDialog(this) == DialogResult.OK)
          {
            BuildMouldForm bmf = new BuildMouldForm
              {
                RoutineHandler = this.RoutineHandler,
                OldVersion = wrk.Version,
                NewVersion = wrk.Version,
                MouldPath = this.RoutineHandler.MetaDataFile
              };
            if (bmf.ShowDialog(this) == DialogResult.OK)
            {
              SqlBuildMouldWork smw = new SqlBuildMouldWork(this.RoutineHandler)
                {
                  Version = bmf.NewVersion,
                  MouldPath = bmf.NewMould,
                  ConnectionString = builder.ConnectionString,
                  TestScriptObjects = true,
                  Database = db
                };
              BackgroundWorkerForm frm2 = new BackgroundWorkerForm {Work = smw};
              if (frm2.ShowDialog(this) != DialogResult.OK)
                throw new Exception("Не удалость построить слепок по временной БД");
              if (Path.GetFullPath(this.RoutineHandler.MetaDataFile)
                 == Path.GetFullPath(smw.MouldPath))
              {
                this.RoutineHandler.ReloadMetaData();
              }
            }
            else
              throw new Exception("Построение слепка отменено");
          }
          else
            throw new Exception("Не удалость получить данные о временной БД для построения слепка");
        }
      }
      catch (Exception ex)
      {
        Log.Error("HandleConvertDatabaseToSql2005(): exception", ex);
      }
      finally
      {
        if (remove_db)
          try
          {
            ent.RemoveDatabase(db);
          }
          catch (Exception ex)
          {
            Log.Error(string.Format("HandleConvertDatabaseToSql2005(): RemoveDatabase(\"{0}\"): exception", db.Name), ex);
          }
      }
    }

    public void HandleConvertDatabaseToSql2005Pub(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      HandleConvertDatabaseToSql2005(sender, e);
    }
  }
}
