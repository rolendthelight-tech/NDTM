namespace AT.Toolbox.Controls
{
  partial class LogPropertyPage
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
      this.m_recent_count_edit = new DevExpress.XtraEditors.SpinEdit();
      this.m_log_lister_settings = new System.Windows.Forms.BindingSource(this.components);
      this.m_label_recent_count2 = new DevExpress.XtraEditors.LabelControl();
      this.m_display_group = new DevExpress.XtraEditors.GroupControl();
      this.m_level_edit = new DevExpress.XtraEditors.LookUpEdit();
      this.m_level_label = new DevExpress.XtraEditors.LabelControl();
      this.m_debug_color_edit = new DevExpress.XtraEditors.ColorEdit();
      this.m_info_color_edit = new DevExpress.XtraEditors.ColorEdit();
      this.m_warning_color_edit = new DevExpress.XtraEditors.ColorEdit();
      this.m_error_color_edit = new DevExpress.XtraEditors.ColorEdit();
      this.m_info_label = new DevExpress.XtraEditors.LabelControl();
      this.m_debug_label = new DevExpress.XtraEditors.LabelControl();
      this.m_warning_label = new DevExpress.XtraEditors.LabelControl();
      this.m_error_label = new DevExpress.XtraEditors.LabelControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_recent_count_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_log_lister_settings)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_display_group)).BeginInit();
      this.m_display_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_level_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_debug_color_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_info_color_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_warning_color_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_error_color_edit.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_recent_count_edit
      // 
      this.m_recent_count_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "RecentLogCount", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
      this.m_recent_count_edit.EditValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.m_recent_count_edit.Location = new System.Drawing.Point(145, 75);
      this.m_recent_count_edit.Name = "m_recent_count_edit";
      this.m_recent_count_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.m_recent_count_edit.Properties.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.m_recent_count_edit.Properties.MaxValue = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.m_recent_count_edit.Properties.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.m_recent_count_edit.Size = new System.Drawing.Size(145, 20);
      this.m_recent_count_edit.TabIndex = 7;
      // 
      // m_log_lister_settings
      // 
      this.m_log_lister_settings.DataSource = typeof(AT.Toolbox.Controls.LogListerSettings);
      // 
      // m_label_recent_count2
      // 
      this.m_label_recent_count2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_label_recent_count2.Location = new System.Drawing.Point(5, 75);
      this.m_label_recent_count2.Name = "m_label_recent_count2";
      this.m_label_recent_count2.Size = new System.Drawing.Size(135, 20);
      this.m_label_recent_count2.TabIndex = 6;
      this.m_label_recent_count2.Text = "Recent count";
      // 
      // m_display_group
      // 
      this.m_display_group.Controls.Add(this.m_level_edit);
      this.m_display_group.Controls.Add(this.m_level_label);
      this.m_display_group.Controls.Add(this.m_debug_color_edit);
      this.m_display_group.Controls.Add(this.m_recent_count_edit);
      this.m_display_group.Controls.Add(this.m_info_color_edit);
      this.m_display_group.Controls.Add(this.m_label_recent_count2);
      this.m_display_group.Controls.Add(this.m_warning_color_edit);
      this.m_display_group.Controls.Add(this.m_error_color_edit);
      this.m_display_group.Controls.Add(this.m_info_label);
      this.m_display_group.Controls.Add(this.m_debug_label);
      this.m_display_group.Controls.Add(this.m_warning_label);
      this.m_display_group.Controls.Add(this.m_error_label);
      this.m_display_group.Dock = System.Windows.Forms.DockStyle.Top;
      this.m_display_group.Location = new System.Drawing.Point(0, 0);
      this.m_display_group.Name = "m_display_group";
      this.m_display_group.Size = new System.Drawing.Size(600, 114);
      this.m_display_group.TabIndex = 2;
      this.m_display_group.Text = "Log display settings";
      // 
      // m_level_edit
      // 
      this.m_level_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "DefaultLevel", true));
      this.m_level_edit.Location = new System.Drawing.Point(448, 75);
      this.m_level_edit.Name = "m_level_edit";
      this.m_level_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_level_edit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name1")});
      this.m_level_edit.Properties.DisplayMember = "Value";
      this.m_level_edit.Properties.ShowHeader = false;
      this.m_level_edit.Properties.ValueMember = "Key";
      this.m_level_edit.Size = new System.Drawing.Size(146, 20);
      this.m_level_edit.TabIndex = 20;
      // 
      // m_level_label
      // 
      this.m_level_label.Location = new System.Drawing.Point(307, 78);
      this.m_level_label.Name = "m_level_label";
      this.m_level_label.Size = new System.Drawing.Size(59, 13);
      this.m_level_label.TabIndex = 19;
      this.m_level_label.Text = "Minimal level";
      // 
      // m_debug_color_edit
      // 
      this.m_debug_color_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "DebugColor", true));
      this.m_debug_color_edit.EditValue = System.Drawing.Color.Empty;
      this.m_debug_color_edit.Location = new System.Drawing.Point(448, 49);
      this.m_debug_color_edit.Name = "m_debug_color_edit";
      this.m_debug_color_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_debug_color_edit.Size = new System.Drawing.Size(146, 20);
      this.m_debug_color_edit.TabIndex = 18;
      // 
      // m_info_color_edit
      // 
      this.m_info_color_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "InfoColor", true));
      this.m_info_color_edit.EditValue = System.Drawing.Color.Empty;
      this.m_info_color_edit.Location = new System.Drawing.Point(448, 23);
      this.m_info_color_edit.Name = "m_info_color_edit";
      this.m_info_color_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_info_color_edit.Size = new System.Drawing.Size(146, 20);
      this.m_info_color_edit.TabIndex = 17;
      // 
      // m_warning_color_edit
      // 
      this.m_warning_color_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "WarningColor", true));
      this.m_warning_color_edit.EditValue = System.Drawing.Color.Empty;
      this.m_warning_color_edit.Location = new System.Drawing.Point(145, 49);
      this.m_warning_color_edit.Name = "m_warning_color_edit";
      this.m_warning_color_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_warning_color_edit.Size = new System.Drawing.Size(146, 20);
      this.m_warning_color_edit.TabIndex = 16;
      // 
      // m_error_color_edit
      // 
      this.m_error_color_edit.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.m_log_lister_settings, "ErrorColor", true));
      this.m_error_color_edit.EditValue = System.Drawing.Color.Empty;
      this.m_error_color_edit.Location = new System.Drawing.Point(145, 23);
      this.m_error_color_edit.Name = "m_error_color_edit";
      this.m_error_color_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_error_color_edit.Size = new System.Drawing.Size(146, 20);
      this.m_error_color_edit.TabIndex = 3;
      // 
      // m_info_label
      // 
      this.m_info_label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_info_label.Location = new System.Drawing.Point(307, 26);
      this.m_info_label.Name = "m_info_label";
      this.m_info_label.Size = new System.Drawing.Size(135, 20);
      this.m_info_label.TabIndex = 15;
      this.m_info_label.Text = "Info color";
      // 
      // m_debug_label
      // 
      this.m_debug_label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_debug_label.Location = new System.Drawing.Point(307, 49);
      this.m_debug_label.Name = "m_debug_label";
      this.m_debug_label.Size = new System.Drawing.Size(134, 20);
      this.m_debug_label.TabIndex = 12;
      this.m_debug_label.Text = "Color";
      // 
      // m_warning_label
      // 
      this.m_warning_label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_warning_label.Location = new System.Drawing.Point(5, 49);
      this.m_warning_label.Name = "m_warning_label";
      this.m_warning_label.Size = new System.Drawing.Size(135, 20);
      this.m_warning_label.TabIndex = 10;
      this.m_warning_label.Text = "Warning color";
      // 
      // m_error_label
      // 
      this.m_error_label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_error_label.Location = new System.Drawing.Point(5, 23);
      this.m_error_label.Name = "m_error_label";
      this.m_error_label.Size = new System.Drawing.Size(135, 20);
      this.m_error_label.TabIndex = 8;
      this.m_error_label.Text = "Error color";
      // 
      // LogPropertyPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_display_group);
      this.Name = "LogPropertyPage";
      this.Size = new System.Drawing.Size(600, 400);
      ((System.ComponentModel.ISupportInitialize)(this.m_recent_count_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_log_lister_settings)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_display_group)).EndInit();
      this.m_display_group.ResumeLayout(false);
      this.m_display_group.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_level_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_debug_color_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_info_color_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_warning_color_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_error_color_edit.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.LabelControl m_label_recent_count2;
    private DevExpress.XtraEditors.GroupControl m_display_group;
    private DevExpress.XtraEditors.LabelControl m_info_label;
    private DevExpress.XtraEditors.LabelControl m_debug_label;
    private DevExpress.XtraEditors.LabelControl m_warning_label;
    private DevExpress.XtraEditors.LabelControl m_error_label;
    private DevExpress.XtraEditors.SpinEdit m_recent_count_edit;
    private DevExpress.XtraEditors.ColorEdit m_debug_color_edit;
    private DevExpress.XtraEditors.ColorEdit m_info_color_edit;
    private DevExpress.XtraEditors.ColorEdit m_warning_color_edit;
    private DevExpress.XtraEditors.ColorEdit m_error_color_edit;
    private System.Windows.Forms.BindingSource m_log_lister_settings;
    private DevExpress.XtraEditors.LookUpEdit m_level_edit;
    private DevExpress.XtraEditors.LabelControl m_level_label;
  }
}
