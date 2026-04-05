using AT.Toolbox.Help;

namespace ATHelpEditor
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
      this.m_defaultLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
      this.m_bar_manager = new DevExpress.XtraBars.BarManager(this.components);
      this.bar1 = new DevExpress.XtraBars.Bar();
      this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
      this.m_cmd_new_project = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_open_project = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_dave_project = new DevExpress.XtraBars.BarButtonItem();
      this.m_save_all_cmd = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_compile = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_compile_test = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_settings = new DevExpress.XtraBars.BarButtonItem();
      this.bar3 = new DevExpress.XtraBars.Bar();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.m_dock_manager = new DevExpress.XtraBars.Docking.DockManager(this.components);
      this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
      this.m_browser_panel = new DevExpress.XtraBars.Docking.DockPanel();
      this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
      this.m_dev_help_browser = new ATHelpEditor.DevHelpNavigator();
      this.dockPanel2 = new DevExpress.XtraBars.Docking.DockPanel();
      this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
      this.helpBrowser1 = new AT.Toolbox.Help.HelpBrowser();
      this.m_log_panel = new DevExpress.XtraBars.Docking.DockPanel();
      this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.m_Tabbed_MDI_manager = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
      this.m_language_switcher = new AT.Toolbox.Base.LanguageSwitcher(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_dock_manager)).BeginInit();
      this.panelContainer1.SuspendLayout();
      this.m_browser_panel.SuspendLayout();
      this.controlContainer1.SuspendLayout();
      this.dockPanel2.SuspendLayout();
      this.dockPanel2_Container.SuspendLayout();
      this.m_log_panel.SuspendLayout();
      this.dockPanel1_Container.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_Tabbed_MDI_manager)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_language_switcher)).BeginInit();
      this.SuspendLayout();
      // 
      // m_defaultLookAndFeel
      // 
      this.m_defaultLookAndFeel.LookAndFeel.SkinName = "Lilian";
      // 
      // m_bar_manager
      // 
      this.m_bar_manager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar3});
      this.m_bar_manager.DockControls.Add(this.barDockControlTop);
      this.m_bar_manager.DockControls.Add(this.barDockControlBottom);
      this.m_bar_manager.DockControls.Add(this.barDockControlLeft);
      this.m_bar_manager.DockControls.Add(this.barDockControlRight);
      this.m_bar_manager.DockManager = this.m_dock_manager;
      this.m_bar_manager.Form = this;
      this.m_bar_manager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_cmd_new_project,
            this.m_cmd_compile,
            this.m_cmd_compile_test,
            this.m_save_all_cmd,
            this.m_cmd_dave_project,
            this.m_cmd_open_project,
            this.m_cmd_settings,
            this.barSubItem1});
      this.m_bar_manager.MaxItemId = 8;
      this.m_bar_manager.StatusBar = this.bar3;
      // 
      // bar1
      // 
      this.bar1.BarName = "Tools";
      this.bar1.DockCol = 0;
      this.bar1.DockRow = 0;
      this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_save_all_cmd, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_compile, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_compile_test),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_settings, true)});
      this.bar1.Text = "Tools";
      // 
      // barSubItem1
      // 
      this.barSubItem1.Caption = "Project";
      this.barSubItem1.Id = 7;
      this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_new_project),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_open_project),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_dave_project)});
      this.barSubItem1.Name = "barSubItem1";
      // 
      // m_cmd_new_project
      // 
      this.m_cmd_new_project.Caption = "New Project";
      this.m_cmd_new_project.Id = 0;
      this.m_cmd_new_project.Name = "m_cmd_new_project";
      this.m_cmd_new_project.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnNewProject);
      // 
      // m_cmd_open_project
      // 
      this.m_cmd_open_project.Caption = "Open Project";
      this.m_cmd_open_project.Id = 5;
      this.m_cmd_open_project.Name = "m_cmd_open_project";
      this.m_cmd_open_project.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OpenProject);
      // 
      // m_cmd_dave_project
      // 
      this.m_cmd_dave_project.Caption = "Save Project";
      this.m_cmd_dave_project.Id = 4;
      this.m_cmd_dave_project.Name = "m_cmd_dave_project";
      this.m_cmd_dave_project.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnSaveProject);
      // 
      // m_save_all_cmd
      // 
      this.m_save_all_cmd.Caption = "SaveAll";
      this.m_save_all_cmd.Id = 3;
      this.m_save_all_cmd.Name = "m_save_all_cmd";
      this.m_save_all_cmd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_save_all_cmd_ItemClick);
      // 
      // m_cmd_compile
      // 
      this.m_cmd_compile.Caption = "Compile";
      this.m_cmd_compile.Id = 1;
      this.m_cmd_compile.Name = "m_cmd_compile";
      this.m_cmd_compile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnCompile);
      // 
      // m_cmd_compile_test
      // 
      this.m_cmd_compile_test.Caption = "CompileAndTest";
      this.m_cmd_compile_test.Id = 2;
      this.m_cmd_compile_test.Name = "m_cmd_compile_test";
      this.m_cmd_compile_test.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.CompileAndTest);
      // 
      // m_cmd_settings
      // 
      this.m_cmd_settings.Caption = "Settings";
      this.m_cmd_settings.Id = 6;
      this.m_cmd_settings.Name = "m_cmd_settings";
      this.m_cmd_settings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_settings_ItemClick);
      // 
      // bar3
      // 
      this.bar3.BarName = "Status bar";
      this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
      this.bar3.DockCol = 0;
      this.bar3.DockRow = 0;
      this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
      this.bar3.OptionsBar.AllowQuickCustomization = false;
      this.bar3.OptionsBar.DrawDragBorder = false;
      this.bar3.OptionsBar.UseWholeRow = true;
      this.bar3.Text = "Status bar";
      // 
      // m_dock_manager
      // 
      this.m_dock_manager.Form = this;
      this.m_dock_manager.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.panelContainer1,
            this.m_log_panel});
      this.m_dock_manager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
      // 
      // panelContainer1
      // 
      this.panelContainer1.Controls.Add(this.m_browser_panel);
      this.panelContainer1.Controls.Add(this.dockPanel2);
      this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
      this.panelContainer1.ID = new System.Guid("efbf22c4-0ed7-4218-9855-4a47083b5eda");
      this.panelContainer1.Location = new System.Drawing.Point(841, 25);
      this.panelContainer1.Name = "panelContainer1";
      this.panelContainer1.Size = new System.Drawing.Size(345, 404);
      this.panelContainer1.Text = "panelContainer1";
      // 
      // m_browser_panel
      // 
      this.m_browser_panel.Controls.Add(this.controlContainer1);
      this.m_browser_panel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
      this.m_browser_panel.ID = new System.Guid("7049e5be-fce2-4d84-a8ab-9e71255b7a36");
      this.m_browser_panel.Location = new System.Drawing.Point(0, 0);
      this.m_browser_panel.Name = "m_browser_panel";
      this.m_browser_panel.Size = new System.Drawing.Size(345, 32767);
      this.m_browser_panel.Text = "Help File Nodes";
      // 
      // controlContainer1
      // 
      this.controlContainer1.Controls.Add(this.m_dev_help_browser);
      this.controlContainer1.Location = new System.Drawing.Point(3, 25);
      this.controlContainer1.Name = "controlContainer1";
      this.controlContainer1.Size = new System.Drawing.Size(339, 32767);
      this.controlContainer1.TabIndex = 0;
      // 
      // m_dev_help_browser
      // 
      this.m_dev_help_browser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_dev_help_browser.Enabled = false;
      this.m_dev_help_browser.Location = new System.Drawing.Point(0, 0);
      this.m_dev_help_browser.Name = "m_dev_help_browser";
      this.m_dev_help_browser.Size = new System.Drawing.Size(339, 32767);
      this.m_dev_help_browser.TabIndex = 0;
      // 
      // dockPanel2
      // 
      this.dockPanel2.Controls.Add(this.dockPanel2_Container);
      this.dockPanel2.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
      this.dockPanel2.ID = new System.Guid("07ab04a7-c1a9-44f3-aff7-b8d5e0d0feae");
      this.dockPanel2.Location = new System.Drawing.Point(0, 32767);
      this.dockPanel2.Name = "dockPanel2";
      this.dockPanel2.Size = new System.Drawing.Size(345, 0);
      this.dockPanel2.Text = "Help";
      // 
      // dockPanel2_Container
      // 
      this.dockPanel2_Container.Controls.Add(this.helpBrowser1);
      this.dockPanel2_Container.Location = new System.Drawing.Point(3, -32768);
      this.dockPanel2_Container.Name = "dockPanel2_Container";
      this.dockPanel2_Container.Size = new System.Drawing.Size(339, 0);
      this.dockPanel2_Container.TabIndex = 0;
      // 
      // helpBrowser1
      // 
      this.helpBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.helpBrowser1.Location = new System.Drawing.Point(0, 0);
      this.helpBrowser1.Name = "helpBrowser1";
      this.helpBrowser1.Size = new System.Drawing.Size(339, 0);
      this.helpBrowser1.TabIndex = 0;
      // 
      // m_log_panel
      // 
      this.m_log_panel.Controls.Add(this.dockPanel1_Container);
      this.m_log_panel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
      this.m_log_panel.ID = new System.Guid("6fb1e559-1db9-4ecd-9c27-156a5ef04f38");
      this.m_log_panel.Location = new System.Drawing.Point(0, 429);
      this.m_log_panel.Name = "m_log_panel";
      this.m_log_panel.Size = new System.Drawing.Size(1186, 200);
      this.m_log_panel.Text = "Log";
      // 
      // dockPanel1_Container
      // 
      this.dockPanel1_Container.Controls.Add(this.logLister1);
      this.dockPanel1_Container.Location = new System.Drawing.Point(3, 25);
      this.dockPanel1_Container.Name = "dockPanel1_Container";
      this.dockPanel1_Container.Size = new System.Drawing.Size(835, 172);
      this.dockPanel1_Container.TabIndex = 0;
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 0);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(835, 172);
      this.logLister1.TabIndex = 0;
      // 
      // m_Tabbed_MDI_manager
      // 
      this.m_Tabbed_MDI_manager.MdiParent = this;
      // 
      // m_language_switcher
      // 
      this.m_language_switcher.LanguageChanged += new System.EventHandler(this.m_language_switcher_LanguageChanged);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1186, 653);
      this.Controls.Add(this.panelContainer1);
      this.Controls.Add(this.m_log_panel);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.IsMdiContainer = true;
      this.Name = "MainForm";
      this.Text = "AT Help Editor";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.m_bar_manager)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_dock_manager)).EndInit();
      this.panelContainer1.ResumeLayout(false);
      this.m_browser_panel.ResumeLayout(false);
      this.controlContainer1.ResumeLayout(false);
      this.dockPanel2.ResumeLayout(false);
      this.dockPanel2_Container.ResumeLayout(false);
      this.m_log_panel.ResumeLayout(false);
      this.dockPanel1_Container.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_Tabbed_MDI_manager)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_language_switcher)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.LookAndFeel.DefaultLookAndFeel m_defaultLookAndFeel;
    private DevExpress.XtraBars.BarManager m_bar_manager;
    private DevExpress.XtraBars.Bar bar1;
    private DevExpress.XtraBars.Bar bar3;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraBars.Docking.DockManager m_dock_manager;
    private DevExpress.XtraBars.Docking.DockPanel m_log_panel;
    private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager m_Tabbed_MDI_manager;
    private DevExpress.XtraBars.Docking.DockPanel m_browser_panel;
    private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
    private ATHelpEditor.DevHelpNavigator m_dev_help_browser;
    private DevExpress.XtraBars.BarButtonItem m_cmd_new_project;
    private DevExpress.XtraBars.BarButtonItem m_cmd_compile;
    private DevExpress.XtraBars.BarButtonItem m_cmd_compile_test;
    private DevExpress.XtraBars.BarButtonItem m_save_all_cmd;
    private DevExpress.XtraBars.BarButtonItem m_cmd_dave_project;
    private DevExpress.XtraBars.BarButtonItem m_cmd_open_project;
    private DevExpress.XtraBars.Docking.DockPanel dockPanel2;
    private DevExpress.XtraBars.Docking.ControlContainer dockPanel2_Container;
    private DevExpress.XtraBars.Docking.DockPanel panelContainer1;
    private HelpBrowser helpBrowser1;
    private AT.Toolbox.Base.LanguageSwitcher m_language_switcher;
    private DevExpress.XtraBars.BarButtonItem m_cmd_settings;
    private DevExpress.XtraBars.BarSubItem barSubItem1;
  }
}