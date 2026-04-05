namespace TestLog
{
  partial class Form1
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
      this.bar1 = new DevExpress.XtraBars.Bar();
      this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
      this.contextMenuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_notifyIcon
      // 
      this.m_notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
      this.m_notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notifyIcon.Icon")));
      // 
      // timer1
      // 
      this.timer1.Interval = 1000;
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 34);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(951, 518);
      this.logLister1.TabIndex = 0;
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(124, 26);
      // 
      // restoreToolStripMenuItem
      // 
      this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
      this.restoreToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
      this.restoreToolStripMenuItem.Text = "Restore";
      this.restoreToolStripMenuItem.Click += new System.EventHandler(this.HandleRestoreCmd);
      // 
      // barManager1
      // 
      this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
      this.barManager1.DockControls.Add(this.barDockControlTop);
      this.barManager1.DockControls.Add(this.barDockControlBottom);
      this.barManager1.DockControls.Add(this.barDockControlLeft);
      this.barManager1.DockControls.Add(this.barDockControlRight);
      this.barManager1.Form = this;
      this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem2,
            this.barButtonItem3,
            this.barButtonItem4,
            this.barButtonItem1,
            this.barButtonItem5});
      this.barManager1.MaxItemId = 6;
      // 
      // bar1
      // 
      this.bar1.BarName = "Tools";
      this.bar1.DockCol = 0;
      this.bar1.DockRow = 0;
      this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
      this.bar1.FloatLocation = new System.Drawing.Point(359, 155);
      this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem5, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem4)});
      this.bar1.OptionsBar.AllowQuickCustomization = false;
      this.bar1.OptionsBar.DisableClose = true;
      this.bar1.OptionsBar.UseWholeRow = true;
      this.bar1.Text = "Tools";
      // 
      // barButtonItem2
      // 
      this.barButtonItem2.Caption = "Start";
      this.barButtonItem2.Glyph = global::TestLog.Properties.Resources.media_play;
      this.barButtonItem2.Id = 1;
      this.barButtonItem2.Name = "barButtonItem2";
      this.barButtonItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleStart);
      // 
      // barButtonItem3
      // 
      this.barButtonItem3.Caption = "Stop";
      this.barButtonItem3.Glyph = global::TestLog.Properties.Resources.media_stop_red;
      this.barButtonItem3.Id = 2;
      this.barButtonItem3.Name = "barButtonItem3";
      this.barButtonItem3.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleStop);
      // 
      // barButtonItem1
      // 
      this.barButtonItem1.Caption = "Settings";
      this.barButtonItem1.Glyph = global::TestLog.Properties.Resources.preferences;
      this.barButtonItem1.Id = 4;
      this.barButtonItem1.Name = "barButtonItem1";
      this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandlePreferences);
      // 
      // barButtonItem4
      // 
      this.barButtonItem4.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
      this.barButtonItem4.Caption = "About";
      this.barButtonItem4.Glyph = global::TestLog.Properties.Resources.about;
      this.barButtonItem4.Id = 3;
      this.barButtonItem4.Name = "barButtonItem4";
      this.barButtonItem4.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
      this.barButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.HandleAbout);
      // 
      // barButtonItem5
      // 
      this.barButtonItem5.Caption = "barButtonItem5";
      this.barButtonItem5.Glyph = global::TestLog.Properties.Resources.media_stop_red;
      this.barButtonItem5.Id = 5;
      this.barButtonItem5.Name = "barButtonItem5";
      this.barButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem5_ItemClick);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(951, 552);
      this.Controls.Add(this.logLister1);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.MinimizeFormToTray = true;
      this.Name = "Form1";
      this.NotifyIcon = ((System.Drawing.Icon)(resources.GetObject("$this.NotifyIcon")));
      this.NotifyMenu = this.contextMenuStrip1;
      this.Text = "Form1";
      this.contextMenuStrip1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.Controls.LogLister logLister1;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
    private DevExpress.XtraBars.BarManager barManager1;
    private DevExpress.XtraBars.Bar bar1;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraBars.BarButtonItem barButtonItem2;
    private DevExpress.XtraBars.BarButtonItem barButtonItem3;
    private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    private DevExpress.XtraBars.BarButtonItem barButtonItem4;
    private DevExpress.XtraBars.BarButtonItem barButtonItem5;
  }
}

