namespace AT.Toolbox
{
  partial class AppPropertyPage
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.settingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_allow_one_instance = new DevExpress.XtraEditors.CheckEdit();
      this.m_locale_label = new DevExpress.XtraEditors.LabelControl();
      this.m_restart_on_critical = new DevExpress.XtraEditors.CheckEdit();
      this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
      this.m_locale = new DevExpress.XtraEditors.LookUpEdit();
      this.m_dont_close = new DevExpress.XtraEditors.CheckEdit();
      this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
      this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
      this.m_email_edit = new DevExpress.XtraEditors.TextEdit();
      this.errorBoxBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_allow_one_instance.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_restart_on_critical.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
      this.groupControl1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_locale.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_dont_close.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
      this.groupControl2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_email_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorBoxBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // settingsBindingSource
      // 
      this.settingsBindingSource.DataSource = typeof(AT.Toolbox.ApplicationSettings);
      // 
      // m_allow_one_instance
      // 
      this.m_allow_one_instance.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.settingsBindingSource, "AllowOnlyOneInstance", true));
      this.m_allow_one_instance.Location = new System.Drawing.Point(149, 58);
      this.m_allow_one_instance.Name = "m_allow_one_instance";
      this.m_allow_one_instance.Properties.Caption = "Allow only one instance";
      this.m_allow_one_instance.Size = new System.Drawing.Size(347, 18);
      this.m_allow_one_instance.TabIndex = 2;
      this.m_allow_one_instance.Validated += new System.EventHandler(this.HandleAllowOneInstanceChanged);
      // 
      // m_locale_label
      // 
      this.m_locale_label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_locale_label.Location = new System.Drawing.Point(5, 25);
      this.m_locale_label.Name = "m_locale_label";
      this.m_locale_label.Size = new System.Drawing.Size(140, 20);
      this.m_locale_label.TabIndex = 5;
      this.m_locale_label.Text = "Locale";
      // 
      // m_restart_on_critical
      // 
      this.m_restart_on_critical.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.settingsBindingSource, "AutoRestartOnCriticalFailure", true));
      this.m_restart_on_critical.Location = new System.Drawing.Point(149, 83);
      this.m_restart_on_critical.Name = "m_restart_on_critical";
      this.m_restart_on_critical.Properties.Caption = "Restart on critical failure";
      this.m_restart_on_critical.Size = new System.Drawing.Size(347, 18);
      this.m_restart_on_critical.TabIndex = 7;
      this.m_restart_on_critical.Validated += new System.EventHandler(this.HandleRestartOnCriticalChanged);
      // 
      // groupControl1
      // 
      this.groupControl1.Controls.Add(this.m_locale);
      this.groupControl1.Controls.Add(this.m_dont_close);
      this.groupControl1.Controls.Add(this.m_restart_on_critical);
      this.groupControl1.Controls.Add(this.m_locale_label);
      this.groupControl1.Controls.Add(this.m_allow_one_instance);
      this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupControl1.Location = new System.Drawing.Point(0, 0);
      this.groupControl1.Name = "groupControl1";
      this.groupControl1.Size = new System.Drawing.Size(528, 153);
      this.groupControl1.TabIndex = 8;
      this.groupControl1.Text = "Базовые настройки";
      // 
      // m_locale
      // 
      this.m_locale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_locale.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.settingsBindingSource, "DefaultLocale", true));
      this.m_locale.Location = new System.Drawing.Point(151, 25);
      this.m_locale.Name = "m_locale";
      this.m_locale.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_locale.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name1")});
      this.m_locale.Properties.DisplayMember = "Value";
      this.m_locale.Properties.DropDownRows = 3;
      this.m_locale.Properties.ShowFooter = false;
      this.m_locale.Properties.ShowHeader = false;
      this.m_locale.Properties.ValueMember = "Key";
      this.m_locale.Size = new System.Drawing.Size(374, 20);
      this.m_locale.TabIndex = 9;
      // 
      // m_dont_close
      // 
      this.m_dont_close.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.settingsBindingSource, "CloseOnCriticalError", true));
      this.m_dont_close.Location = new System.Drawing.Point(149, 107);
      this.m_dont_close.Name = "m_dont_close";
      this.m_dont_close.Properties.Caption = "Close after critical failure";
      this.m_dont_close.Size = new System.Drawing.Size(347, 18);
      this.m_dont_close.TabIndex = 8;
      // 
      // groupControl2
      // 
      this.groupControl2.Controls.Add(this.labelControl1);
      this.groupControl2.Controls.Add(this.m_email_edit);
      this.groupControl2.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupControl2.Location = new System.Drawing.Point(0, 153);
      this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.groupControl2.Name = "groupControl2";
      this.groupControl2.Size = new System.Drawing.Size(528, 55);
      this.groupControl2.TabIndex = 9;
      this.groupControl2.Text = "Поддержка";
      // 
      // labelControl1
      // 
      this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl1.Location = new System.Drawing.Point(5, 27);
      this.labelControl1.Name = "labelControl1";
      this.labelControl1.Size = new System.Drawing.Size(140, 20);
      this.labelControl1.TabIndex = 9;
      this.labelControl1.Text = "e-mail службы поддержки";
      // 
      // m_email_edit
      // 
      this.m_email_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_email_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.errorBoxBindingSource, "RemoteAdress", true));
      this.m_email_edit.Location = new System.Drawing.Point(151, 27);
      this.m_email_edit.Name = "m_email_edit";
      this.m_email_edit.Size = new System.Drawing.Size(374, 20);
      this.m_email_edit.TabIndex = 8;
      // 
      // errorBoxBindingSource
      // 
      this.errorBoxBindingSource.AllowNew = false;
      this.errorBoxBindingSource.DataSource = typeof(AT.Toolbox.Dialogs.ErrorBox.Settings);
      // 
      // AppPropertyPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupControl2);
      this.Controls.Add(this.groupControl1);
      this.Name = "AppPropertyPage";
      this.Size = new System.Drawing.Size(528, 411);
      ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_allow_one_instance.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_restart_on_critical.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
      this.groupControl1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_locale.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_dont_close.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
      this.groupControl2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_email_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorBoxBindingSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.CheckEdit m_allow_one_instance;
    private DevExpress.XtraEditors.LabelControl m_locale_label;
    private DevExpress.XtraEditors.CheckEdit m_restart_on_critical;
    private System.Windows.Forms.BindingSource settingsBindingSource;
    private DevExpress.XtraEditors.GroupControl groupControl1;
    private DevExpress.XtraEditors.GroupControl groupControl2;
    private DevExpress.XtraEditors.LabelControl labelControl1;
    private DevExpress.XtraEditors.TextEdit m_email_edit;
    private System.Windows.Forms.BindingSource errorBoxBindingSource;
    private DevExpress.XtraEditors.CheckEdit m_dont_close;
    private DevExpress.XtraEditors.LookUpEdit m_locale;
  }
}
