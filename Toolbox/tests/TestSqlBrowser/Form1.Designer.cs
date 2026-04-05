namespace TestSqlBrowser
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
      this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
      this.bar3 = new DevExpress.XtraBars.Bar();
      this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
      this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
      this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
      this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
      this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
      this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
      this.databaseBrowserControl1 = new AT.Toolbox.MSSQL.DatabaseBrowserControl();
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
      this.splitContainerControl1.SuspendLayout();
      this.SuspendLayout();
      // 
      // barManager1
      // 
      this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar3});
      this.barManager1.DockControls.Add(this.barDockControlTop);
      this.barManager1.DockControls.Add(this.barDockControlBottom);
      this.barManager1.DockControls.Add(this.barDockControlLeft);
      this.barManager1.DockControls.Add(this.barDockControlRight);
      this.barManager1.Form = this;
      this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3,
            this.barButtonItem4,
            this.barStaticItem1});
      this.barManager1.MaxItemId = 5;
      this.barManager1.StatusBar = this.bar3;
      // 
      // bar3
      // 
      this.bar3.BarName = "Status bar";
      this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
      this.bar3.DockCol = 0;
      this.bar3.DockRow = 0;
      this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
      this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem4, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1, true)});
      this.bar3.OptionsBar.AllowQuickCustomization = false;
      this.bar3.OptionsBar.DrawDragBorder = false;
      this.bar3.OptionsBar.UseWholeRow = true;
      this.bar3.Text = "Status bar";
      // 
      // barButtonItem1
      // 
      this.barButtonItem1.Caption = "RU";
      this.barButtonItem1.Id = 0;
      this.barButtonItem1.Name = "barButtonItem1";
      this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
      // 
      // barButtonItem2
      // 
      this.barButtonItem2.Caption = "EN";
      this.barButtonItem2.Id = 1;
      this.barButtonItem2.Name = "barButtonItem2";
      this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
      // 
      // barButtonItem3
      // 
      this.barButtonItem3.Caption = "Settings";
      this.barButtonItem3.Id = 2;
      this.barButtonItem3.Name = "barButtonItem3";
      this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemClick);
      // 
      // barButtonItem4
      // 
      this.barButtonItem4.Caption = "Edit";
      this.barButtonItem4.Id = 3;
      this.barButtonItem4.Name = "barButtonItem4";
      // 
      // barStaticItem1
      // 
      this.barStaticItem1.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
      this.barStaticItem1.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.barStaticItem1.Caption = "barStaticItem1";
      this.barStaticItem1.Id = 4;
      this.barStaticItem1.Name = "barStaticItem1";
      this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
      this.barStaticItem1.Width = 32;
      // 
      // splitContainerControl1
      // 
      this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerControl1.Horizontal = false;
      this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
      this.splitContainerControl1.Name = "splitContainerControl1";
      this.splitContainerControl1.Panel1.Controls.Add(this.databaseBrowserControl1);
      this.splitContainerControl1.Panel1.Text = "Panel1";
      this.splitContainerControl1.Panel2.Controls.Add(this.logLister1);
      this.splitContainerControl1.Panel2.Text = "Panel2";
      this.splitContainerControl1.Size = new System.Drawing.Size(800, 530);
      this.splitContainerControl1.SplitterPosition = 299;
      this.splitContainerControl1.TabIndex = 4;
      this.splitContainerControl1.Text = "splitContainerControl1";
      // 
      // databaseBrowserControl1
      // 
      this.databaseBrowserControl1.CacheEntries = true;
      this.databaseBrowserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.databaseBrowserControl1.Location = new System.Drawing.Point(0, 0);
      this.databaseBrowserControl1.Name = "databaseBrowserControl1";
      this.databaseBrowserControl1.Size = new System.Drawing.Size(796, 295);
      this.databaseBrowserControl1.TabIndex = 0;
      this.databaseBrowserControl1.Type = null;
      this.databaseBrowserControl1.UserMode = false;
      this.databaseBrowserControl1.EditValueChanged += new System.EventHandler<System.EventArgs>(this.databaseBrowserControl1_EditValueChanged);
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 0);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(796, 221);
      this.logLister1.TabIndex = 0;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 552);
      this.Controls.Add(this.splitContainerControl1);
      this.Controls.Add(this.barDockControlLeft);
      this.Controls.Add(this.barDockControlRight);
      this.Controls.Add(this.barDockControlBottom);
      this.Controls.Add(this.barDockControlTop);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Shown += new System.EventHandler(this.Form1_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
      this.splitContainerControl1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraBars.BarManager barManager1;
    private DevExpress.XtraBars.Bar bar3;
    private DevExpress.XtraBars.BarDockControl barDockControlTop;
    private DevExpress.XtraBars.BarDockControl barDockControlBottom;
    private DevExpress.XtraBars.BarDockControl barDockControlLeft;
    private DevExpress.XtraBars.BarDockControl barDockControlRight;
    private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
    private AT.Toolbox.MSSQL.DatabaseBrowserControl databaseBrowserControl1;
    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    private DevExpress.XtraBars.BarButtonItem barButtonItem2;
    private DevExpress.XtraBars.BarButtonItem barButtonItem3;
    private DevExpress.XtraBars.BarButtonItem barButtonItem4;
    private DevExpress.XtraBars.BarStaticItem barStaticItem1;

  }
}

