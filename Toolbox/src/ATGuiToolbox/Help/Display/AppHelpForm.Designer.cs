namespace AT.Toolbox.Help
{
  partial class AppHelpForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppHelpForm));
      this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
      this.bar1 = new DevExpress.XtraBars.Bar();
      this.m_cmd_back = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_forward = new DevExpress.XtraBars.BarButtonItem();
      this.bar2 = new DevExpress.XtraBars.Bar();
      this.m_cmd_settings = new DevExpress.XtraBars.BarButtonItem();
      this.m_cmd_about = new DevExpress.XtraBars.BarButtonItem();
      this.bar3 = new DevExpress.XtraBars.Bar();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_help_navigator = new AT.Toolbox.Help.UserHelpNavigator();
      this.m_help_browser = new AT.Toolbox.Help.HelpBrowser();
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
      this.splitContainerControl1.SuspendLayout();
      this.SuspendLayout();
      // 
      // barManager1
      // 
      this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar2,
            this.bar3});
      this.barManager1.DockControls.Add(this.barDockControlTop);
      this.barManager1.DockControls.Add(this.barDockControlBottom);
      this.barManager1.DockControls.Add(this.barDockControlLeft);
      this.barManager1.DockControls.Add(this.barDockControlRight);
      this.barManager1.Form = this;
      this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.m_cmd_back,
            this.m_cmd_forward,
            this.m_cmd_settings,
            this.m_cmd_about});
      this.barManager1.MainMenu = this.bar2;
      this.barManager1.MaxItemId = 4;
      this.barManager1.StatusBar = this.bar3;
      // 
      // bar1
      // 
      this.bar1.BarName = "Tools";
      this.bar1.DockCol = 0;
      this.bar1.DockRow = 1;
      this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_back),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_forward)});
      this.bar1.Text = "Tools";
      // 
      // m_cmd_back
      // 
      this.m_cmd_back.Caption = "Back";
      this.m_cmd_back.Enabled = false;
      this.m_cmd_back.Glyph = global::AT.Toolbox.Properties.Resources.rewind_32;
      this.m_cmd_back.Id = 0;
      this.m_cmd_back.Name = "m_cmd_back";
      this.m_cmd_back.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_back_ItemClick);
      // 
      // m_cmd_forward
      // 
      this.m_cmd_forward.Caption = "Forward";
      this.m_cmd_forward.Enabled = false;
      this.m_cmd_forward.Glyph = global::AT.Toolbox.Properties.Resources.foward_32;
      this.m_cmd_forward.Id = 1;
      this.m_cmd_forward.Name = "m_cmd_forward";
      this.m_cmd_forward.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_forward_ItemClick);
      // 
      // bar2
      // 
      this.bar2.BarName = "Main menu";
      this.bar2.DockCol = 0;
      this.bar2.DockRow = 0;
      this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_settings),
            new DevExpress.XtraBars.LinkPersistInfo(this.m_cmd_about)});
      this.bar2.OptionsBar.MultiLine = true;
      this.bar2.OptionsBar.UseWholeRow = true;
      this.bar2.Text = "Main menu";
      // 
      // m_cmd_settings
      // 
      this.m_cmd_settings.Caption = "Settings";
      this.m_cmd_settings.Id = 2;
      this.m_cmd_settings.Name = "m_cmd_settings";
      this.m_cmd_settings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_settings_ItemClick);
      // 
      // m_cmd_about
      // 
      this.m_cmd_about.Caption = "About";
      this.m_cmd_about.Id = 3;
      this.m_cmd_about.Name = "m_cmd_about";
      this.m_cmd_about.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.m_cmd_about_ItemClick);
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
      // splitContainerControl1
      // 
      this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
      this.splitContainerControl1.Location = new System.Drawing.Point(0, 67);
      this.splitContainerControl1.Name = "splitContainerControl1";
      this.splitContainerControl1.Panel1.Controls.Add(this.m_help_navigator);
      this.splitContainerControl1.Panel1.Text = "Panel1";
      this.splitContainerControl1.Panel2.Controls.Add(this.m_help_browser);
      this.splitContainerControl1.Panel2.Text = "Panel2";
      this.splitContainerControl1.Size = new System.Drawing.Size(923, 497);
      this.splitContainerControl1.SplitterPosition = 305;
      this.splitContainerControl1.TabIndex = 4;
      this.splitContainerControl1.Text = "splitContainerControl1";
      // 
      // m_help_navigator
      // 
      this.m_help_navigator.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_help_navigator.Location = new System.Drawing.Point(0, 0);
      this.m_help_navigator.Name = "m_help_navigator";
      this.m_help_navigator.Size = new System.Drawing.Size(301, 493);
      this.m_help_navigator.TabIndex = 0;
      // 
      // m_help_browser
      // 
      this.m_help_browser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_help_browser.Location = new System.Drawing.Point(0, 0);
      this.m_help_browser.Name = "m_help_browser";
      this.m_help_browser.Size = new System.Drawing.Size(608, 493);
      this.m_help_browser.TabIndex = 0;
      // 
      // AppHelpForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(923, 586);
      this.Controls.Add(this.splitContainerControl1);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "AppHelpForm";
      this.Text = "AT Help Browser";
      this.Shown += new System.EventHandler(this.HandleFormShown);
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
      this.splitContainerControl1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraBars.BarManager barManager1;
    private DevExpress.XtraBars.Bar bar1;
    private DevExpress.XtraBars.Bar bar2;
    private DevExpress.XtraBars.Bar bar3;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
    private UserHelpNavigator m_help_navigator;
    private HelpBrowser m_help_browser;
    private DevExpress.XtraBars.BarButtonItem m_cmd_back;
    private DevExpress.XtraBars.BarButtonItem m_cmd_forward;
    private DevExpress.XtraBars.BarButtonItem m_cmd_settings;
    private DevExpress.XtraBars.BarButtonItem m_cmd_about;
  }
}