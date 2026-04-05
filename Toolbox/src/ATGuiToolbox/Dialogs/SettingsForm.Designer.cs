namespace AT.Toolbox.Dialogs
{
  using System.Drawing;


  partial class SettingsForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
      this.m_splitter = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_nav_bar = new DevExpress.XtraNavBar.NavBarControl();
      this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_restart_warning = new DevExpress.XtraEditors.LabelControl();
      this.m_validate_button = new DevExpress.XtraEditors.SimpleButton();
      this.m_error_splitter = new System.Windows.Forms.SplitContainer();
      this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
      this.gridControl1 = new DevExpress.XtraGrid.GridControl();
      this.settingErrorBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
      this.colMessage = new DevExpress.XtraGrid.Columns.GridColumn();
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
      this.m_splitter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_nav_bar)).BeginInit();
      this.m_error_splitter.Panel1.SuspendLayout();
      this.m_error_splitter.Panel2.SuspendLayout();
      this.m_error_splitter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
      this.groupControl1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.settingErrorBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_splitter
      // 
      this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_splitter.Location = new System.Drawing.Point(0, 0);
      this.m_splitter.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.m_splitter.Name = "m_splitter";
      this.m_splitter.Panel1.Controls.Add(this.m_nav_bar);
      this.m_splitter.Panel1.Text = "Panel1";
      this.m_splitter.Panel2.Padding = new System.Windows.Forms.Padding(4);
      this.m_splitter.Panel2.ShowCaption = true;
      this.m_splitter.Panel2.Text = "Panel2";
      this.m_splitter.Size = new System.Drawing.Size(874, 612);
      this.m_splitter.SplitterPosition = 263;
      this.m_splitter.TabIndex = 4;
      this.m_splitter.Text = "splitContainerControl1";
      // 
      // m_nav_bar
      // 
      this.m_nav_bar.ActiveGroup = null;
      this.m_nav_bar.ContentButtonHint = null;
      this.m_nav_bar.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_nav_bar.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.navBarItem1});
      this.m_nav_bar.Location = new System.Drawing.Point(0, 0);
      this.m_nav_bar.Name = "m_nav_bar";
      this.m_nav_bar.OptionsNavPane.ExpandedWidth = 219;
      this.m_nav_bar.Size = new System.Drawing.Size(263, 612);
      this.m_nav_bar.TabIndex = 0;
      this.m_nav_bar.Text = "navBarControl1";
      // 
      // navBarItem1
      // 
      this.navBarItem1.Caption = "navBarItem1";
      this.navBarItem1.LargeImage = global::AT.Toolbox.Properties.Resources.p_32_log_config;
      this.navBarItem1.Name = "navBarItem1";
      this.navBarItem1.SmallImage = global::AT.Toolbox.Properties.Resources.p_32_log_config;
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cancel_btn.Location = new System.Drawing.Point(695, 619);
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 5;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_ok_btn.Location = new System.Drawing.Point(614, 619);
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 6;
      this.m_ok_btn.Text = "OK";
      // 
      // m_restart_warning
      // 
      this.m_restart_warning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_restart_warning.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_restart_warning.Location = new System.Drawing.Point(4, 619);
      this.m_restart_warning.Name = "m_restart_warning";
      this.m_restart_warning.Size = new System.Drawing.Size(604, 23);
      this.m_restart_warning.TabIndex = 7;
      // 
      // m_validate_button
      // 
      this.m_validate_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_validate_button.Location = new System.Drawing.Point(776, 619);
      this.m_validate_button.Name = "m_validate_button";
      this.m_validate_button.Size = new System.Drawing.Size(75, 23);
      this.m_validate_button.TabIndex = 8;
      this.m_validate_button.Text = "Validate";
      this.m_validate_button.Click += new System.EventHandler(this.HandleValidate);
      // 
      // m_error_splitter
      // 
      this.m_error_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_error_splitter.Location = new System.Drawing.Point(0, 0);
      this.m_error_splitter.Name = "m_error_splitter";
      this.m_error_splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // m_error_splitter.Panel1
      // 
      this.m_error_splitter.Panel1.Controls.Add(this.m_splitter);
      // 
      // m_error_splitter.Panel2
      // 
      this.m_error_splitter.Panel2.Controls.Add(this.groupControl1);
      this.m_error_splitter.Panel2Collapsed = true;
      this.m_error_splitter.Size = new System.Drawing.Size(874, 612);
      this.m_error_splitter.SplitterDistance = 482;
      this.m_error_splitter.TabIndex = 9;
      // 
      // groupControl1
      // 
      this.groupControl1.Controls.Add(this.gridControl1);
      this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupControl1.Location = new System.Drawing.Point(0, 0);
      this.groupControl1.Name = "groupControl1";
      this.groupControl1.Size = new System.Drawing.Size(150, 46);
      this.groupControl1.TabIndex = 1;
      this.groupControl1.Text = "Ошибки и предупреждения в настройках ( Двойной щелчок для перехода на страницу)";
      // 
      // gridControl1
      // 
      this.gridControl1.DataSource = this.settingErrorBindingSource;
      this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridControl1.Location = new System.Drawing.Point(2, 22);
      this.gridControl1.MainView = this.gridView1;
      this.gridControl1.Name = "gridControl1";
      this.gridControl1.Size = new System.Drawing.Size(146, 22);
      this.gridControl1.TabIndex = 0;
      this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
      // 
      // settingErrorBindingSource
      // 
      this.settingErrorBindingSource.DataSource = typeof(AT.Toolbox.Dialogs.SettingsForm.SettingError);
      // 
      // gridView1
      // 
      this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMessage});
      this.gridView1.GridControl = this.gridControl1;
      this.gridView1.Name = "gridView1";
      this.gridView1.OptionsView.ShowColumnHeaders = false;
      this.gridView1.OptionsView.ShowGroupPanel = false;
      this.gridView1.DoubleClick += new System.EventHandler(this.HandleNavDoubleClick);
      // 
      // colMessage
      // 
      this.colMessage.FieldName = "Message";
      this.colMessage.Name = "colMessage";
      this.colMessage.OptionsColumn.AllowEdit = false;
      this.colMessage.OptionsColumn.ReadOnly = true;
      this.colMessage.Visible = true;
      this.colMessage.VisibleIndex = 0;
      // 
      // SettingsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(874, 647);
      this.Controls.Add(this.m_error_splitter);
      this.Controls.Add(this.m_validate_button);
      this.Controls.Add(this.m_restart_warning);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_cancel_btn);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "SettingsForm";
      this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 35);
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "SettingsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
      this.m_splitter.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_nav_bar)).EndInit();
      this.m_error_splitter.Panel1.ResumeLayout(false);
      this.m_error_splitter.Panel2.ResumeLayout(false);
      this.m_error_splitter.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
      this.groupControl1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.settingErrorBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SplitContainerControl m_splitter;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.LabelControl m_restart_warning;
    private DevExpress.XtraEditors.SimpleButton m_validate_button;
    private DevExpress.XtraNavBar.NavBarControl m_nav_bar;
    private DevExpress.XtraNavBar.NavBarItem navBarItem1;
    private System.Windows.Forms.SplitContainer m_error_splitter;
    private DevExpress.XtraGrid.GridControl gridControl1;
    private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    private System.Windows.Forms.BindingSource settingErrorBindingSource;
    private DevExpress.XtraGrid.Columns.GridColumn colMessage;
    private DevExpress.XtraEditors.GroupControl groupControl1;
  }
}