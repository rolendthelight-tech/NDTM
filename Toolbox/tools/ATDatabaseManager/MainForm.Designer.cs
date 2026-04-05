namespace ATDatabaseManager
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      DevExpress.LookAndFeel.DefaultLookAndFeel m_look_and_feel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_database_browser = new AT.Toolbox.MSSQL.DatabaseBrowserControl();
      this.m_log_lister = new AT.Toolbox.Controls.LogLister();
      this.m_bar_manager = new DevExpress.XtraBars.BarManager(this.components);
      this.m_bar_top = new DevExpress.XtraBars.Bar();
      this.m_cmd_settings = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_select_mould = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_save = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_edit = new DevExpress.XtraBars.BarButtonItem();
      this.m_menu_help = new DevExpress.XtraBars.BarSubItem();
      this.m_cmd_help = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_about = new DevExpress.XtraBars.BarButtonItem();
      this.m_bar_bottom = new DevExpress.XtraBars.Bar();
      this.m_cmd_en = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_ru = new DevExpress.XtraBars.BarButtonItem();
      this.m_text_current_connection = new DevExpress.XtraBars.BarStaticItem();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      m_look_and_feel = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
      this.splitContainerControl1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainerControl1
      // 
      this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
      this.splitContainerControl1.Horizontal = false;
      this.splitContainerControl1.Location = new System.Drawing.Point(0, 22);
      this.splitContainerControl1.Name = "splitContainerControl1";
      this.splitContainerControl1.Panel1.Controls.Add(this.m_database_browser);
      this.splitContainerControl1.Panel1.Text = "Panel1";
      this.splitContainerControl1.Panel2.Controls.Add(this.m_log_lister);
      this.splitContainerControl1.Panel2.Text = "Panel2";
      this.splitContainerControl1.Size = new System.Drawing.Size(738, 459);
      this.splitContainerControl1.SplitterPosition = 138;
      this.splitContainerControl1.TabIndex = 0;
      this.splitContainerControl1.Text = "splitContainerControl1";
      // 
      // m_database_browser
      // 
      this.m_database_browser.CacheEntries = true;
      this.m_database_browser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_database_browser.Location = new System.Drawing.Point(0, 0);
      this.m_database_browser.Name = "m_database_browser";
      this.m_database_browser.Size = new System.Drawing.Size(734, 311);
      this.m_database_browser.TabIndex = 0;
      this.m_database_browser.Type = null;
      this.m_database_browser.UserMode = false;
      this.m_database_browser.EditValueChanged += new System.EventHandler<System.EventArgs>(this.m_database_browser_EditValueChanged);
      // 
      // m_log_lister
      // 
      this.m_log_lister.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_log_lister.Location = new System.Drawing.Point(0, 0);
      this.m_log_lister.Name = "m_log_lister";
      this.m_log_lister.Size = new System.Drawing.Size(734, 134);
      this.m_log_lister.TabIndex = 0;
      // 
      // m_bar_manager
      // 
      this.m_bar_manager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.m_bar_top,
            this.m_bar_bottom});
      this.m_bar_manager.DockControls.Add(this.barDockControlTop);
      this.m_bar_manager.DockControls.Add(this.barDockControlBottom);
      this.m_bar_manager.DockControls.Add(this.barDockControlLeft);
      this.m_bar_manager.DockControls.Add(this.barDockControlRight);
      this.m_bar_manager.Form = this;
      this.m_bar_manager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_cmd_settings,
            this.m_cmd_en,
            this.m_cmd_ru,
            this.m_text_current_connection,
            this.m_cmd_select_mould,
            this.m_cmd_save,
            this.m_cmd_help,
            this.m_cmd_about,
            this.m_menu_help,
            this.m_cmd_edit});
      this.m_bar_manager.MainMenu = this.m_bar_top;
      this.m_bar_manager.MaxItemId = 10;
      this.m_bar_manager.StatusBar = this.m_bar_bottom;
      // 
      // m_bar_top
      // 
      this.m_bar_top.BarName = "Main menu";
      this.m_bar_top.DockCol = 0;
      this.m_bar_top.DockRow = 0;
      this.m_bar_top.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.m_bar_top.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_settings),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_select_mould),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_save),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_edit),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_menu_help)});
      this.m_bar_top.OptionsBar.MultiLine = true;
      this.m_bar_top.OptionsBar.UseWholeRow = true;
      this.m_bar_top.Text = "Main menu";
      // 
      // m_cmd_settings
      // 
      this.m_cmd_settings.Caption = "Settings";
      this.m_cmd_settings.Id = 0;
      this.m_cmd_settings.Name = "m_cmd_settings";
      this.m_cmd_settings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_settings_ItemClick);
      // 
      // m_cmd_select_mould
      // 
      this.m_cmd_select_mould.Caption = "Select mould";
      this.m_cmd_select_mould.Id = 4;
      this.m_cmd_select_mould.Name = "m_cmd_select_mould";
      this.m_cmd_select_mould.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
      // 
      // m_cmd_save
      // 
      this.m_cmd_save.Caption = "Save mould";
      this.m_cmd_save.Id = 5;
      this.m_cmd_save.Name = "m_cmd_save";
      this.m_cmd_save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_save_ItemClick);
      // 
      // m_cmd_edit
      // 
      this.m_cmd_edit.Caption = "Edit mould";
      this.m_cmd_edit.Id = 9;
      this.m_cmd_edit.Name = "m_cmd_edit";
      this.m_cmd_edit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_edit_ItemClick);
      // 
      // m_menu_help
      // 
      this.m_menu_help.Caption = "Help";
      this.m_menu_help.Id = 8;
      this.m_menu_help.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_help),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_about)});
      this.m_menu_help.Name = "m_menu_help";
      // 
      // m_cmd_help
      // 
      this.m_cmd_help.Caption = "Help";
      this.m_cmd_help.Id = 6;
      this.m_cmd_help.Name = "m_cmd_help";
      this.m_cmd_help.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_help_ItemClick);
      // 
      // m_cmd_about
      // 
      this.m_cmd_about.Caption = "About";
      this.m_cmd_about.Id = 7;
      this.m_cmd_about.Name = "m_cmd_about";
      this.m_cmd_about.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_about_ItemClick);
      // 
      // m_bar_bottom
      // 
      this.m_bar_bottom.BarName = "Status bar";
      this.m_bar_bottom.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
      this.m_bar_bottom.DockCol = 0;
      this.m_bar_bottom.DockRow = 0;
      this.m_bar_bottom.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
      this.m_bar_bottom.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_en),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_ru),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_text_current_connection)});
      this.m_bar_bottom.OptionsBar.AllowQuickCustomization = false;
      this.m_bar_bottom.OptionsBar.DrawDragBorder = false;
      this.m_bar_bottom.OptionsBar.UseWholeRow = true;
      this.m_bar_bottom.Text = "Status bar";
      // 
      // m_cmd_en
      // 
      this.m_cmd_en.Border = DevExpress.XtraEditors.Controls.BorderStyles.Default;
      this.m_cmd_en.Caption = "EN";
      this.m_cmd_en.Id = 1;
      this.m_cmd_en.Name = "m_cmd_en";
      this.m_cmd_en.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_en_ItemClick);
      // 
      // m_cmd_ru
      // 
      this.m_cmd_ru.Border = DevExpress.XtraEditors.Controls.BorderStyles.Default;
      this.m_cmd_ru.Caption = "RU";
      this.m_cmd_ru.Id = 2;
      this.m_cmd_ru.Name = "m_cmd_ru";
      this.m_cmd_ru.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_ru_ItemClick);
      // 
      // m_text_current_connection
      // 
      this.m_text_current_connection.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
      this.m_text_current_connection.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.m_text_current_connection.Id = 3;
      this.m_text_current_connection.Name = "m_text_current_connection";
      this.m_text_current_connection.TextAlignment = System.Drawing.StringAlignment.Near;
      this.m_text_current_connection.Width = 32;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(738, 507);
      this.Controls.Add(this.splitContainerControl1);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "AT Database manager";
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
      this.splitContainerControl1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
    private AT.Toolbox.MSSQL.DatabaseBrowserControl m_database_browser;
    private AT.Toolbox.Controls.LogLister m_log_lister;
    private DevExpress.XtraBars.BarManager m_bar_manager;
    private DevExpress.XtraBars.Bar m_bar_top;
    private DevExpress.XtraBars.BarButtonItem m_cmd_settings;
    private DevExpress.XtraBars.Bar m_bar_bottom;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraBars.BarButtonItem m_cmd_en;
    private DevExpress.XtraBars.BarButtonItem m_cmd_ru;
    private DevExpress.XtraBars.BarStaticItem m_text_current_connection;
    private DevExpress.XtraBars.BarButtonItem m_cmd_select_mould;
    private DevExpress.XtraBars.BarButtonItem m_cmd_save;
    private DevExpress.XtraBars.BarButtonItem m_cmd_help;
    private DevExpress.XtraBars.BarButtonItem m_cmd_about;
    private DevExpress.XtraBars.BarSubItem m_menu_help;
    private DevExpress.XtraBars.BarButtonItem m_cmd_edit;
  }
}

