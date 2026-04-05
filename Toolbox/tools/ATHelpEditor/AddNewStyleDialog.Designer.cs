namespace ATHelpEditor
{
  partial class AddNewStyleDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddNewStyleDialog));
      this.m_color_edit = new DevExpress.XtraEditors.ColorEdit();
      this.m_font_edit = new DevExpress.XtraEditors.FontEdit();
      this.m_name_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_cmd_ok = new DevExpress.XtraEditors.SimpleButton();
      this.m_cmd_cancel = new DevExpress.XtraEditors.SimpleButton();
      this.m_label_color = new DevExpress.XtraEditors.LabelControl();
      this.m_label_font = new DevExpress.XtraEditors.LabelControl();
      this.m_label_name = new DevExpress.XtraEditors.LabelControl();
      this.m_font_size_list = new DevExpress.XtraEditors.ComboBoxEdit();
      this.m_label_font_size = new DevExpress.XtraEditors.LabelControl();
      this.m_check_bold = new DevExpress.XtraEditors.CheckEdit();
      this.m_check_italic = new DevExpress.XtraEditors.CheckEdit();
      this.m_check_underline = new DevExpress.XtraEditors.CheckEdit();
      this.m_memo_demo = new DevExpress.XtraEditors.MemoEdit();
      this.m_label_underline = new DevExpress.XtraEditors.LabelControl();
      this.m_label_italic = new DevExpress.XtraEditors.LabelControl();
      this.m_label_bold = new DevExpress.XtraEditors.LabelControl();
      this.m_label_demo = new DevExpress.XtraEditors.LabelControl();
      this.m_alignment_edit = new DevExpress.XtraEditors.ComboBoxEdit();
      this.m_label_alignment = new DevExpress.XtraEditors.LabelControl();
      ((System.ComponentModel.ISupportInitialize)(this.m_color_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_name_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_size_list.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_bold.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_italic.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_underline.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_memo_demo.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_alignment_edit.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_color_edit
      // 
      this.m_color_edit.EditValue = System.Drawing.Color.Empty;
      this.m_color_edit.Location = new System.Drawing.Point(127, 12);
      this.m_color_edit.Name = "m_color_edit";
      this.m_color_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_color_edit.Properties.ShowCustomColors = false;
      this.m_color_edit.Properties.ShowSystemColors = false;
      this.m_color_edit.Size = new System.Drawing.Size(153, 20);
      this.m_color_edit.TabIndex = 0;
      this.m_color_edit.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_font_edit
      // 
      this.m_font_edit.EditValue = "Times New Roman";
      this.m_font_edit.Location = new System.Drawing.Point(127, 38);
      this.m_font_edit.Name = "m_font_edit";
      this.m_font_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_font_edit.Size = new System.Drawing.Size(153, 20);
      this.m_font_edit.TabIndex = 1;
      this.m_font_edit.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_name_edit
      // 
      this.m_name_edit.Location = new System.Drawing.Point(127, 64);
      this.m_name_edit.Name = "m_name_edit";
      this.m_name_edit.Size = new System.Drawing.Size(153, 20);
      this.m_name_edit.TabIndex = 2;
      // 
      // m_cmd_ok
      // 
      this.m_cmd_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cmd_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_cmd_ok.Location = new System.Drawing.Point(143, 407);
      this.m_cmd_ok.Name = "m_cmd_ok";
      this.m_cmd_ok.Size = new System.Drawing.Size(75, 23);
      this.m_cmd_ok.TabIndex = 3;
      this.m_cmd_ok.Text = "OK";
      // 
      // m_cmd_cancel
      // 
      this.m_cmd_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_cmd_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cmd_cancel.Location = new System.Drawing.Point(224, 407);
      this.m_cmd_cancel.Name = "m_cmd_cancel";
      this.m_cmd_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_cmd_cancel.TabIndex = 4;
      this.m_cmd_cancel.Text = "Cancel";
      // 
      // m_label_color
      // 
      this.m_label_color.Location = new System.Drawing.Point(26, 15);
      this.m_label_color.Name = "m_label_color";
      this.m_label_color.Size = new System.Drawing.Size(25, 13);
      this.m_label_color.TabIndex = 5;
      this.m_label_color.Text = "Color";
      // 
      // m_label_font
      // 
      this.m_label_font.Location = new System.Drawing.Point(26, 41);
      this.m_label_font.Name = "m_label_font";
      this.m_label_font.Size = new System.Drawing.Size(22, 13);
      this.m_label_font.TabIndex = 6;
      this.m_label_font.Text = "Font";
      // 
      // m_label_name
      // 
      this.m_label_name.Location = new System.Drawing.Point(26, 67);
      this.m_label_name.Name = "m_label_name";
      this.m_label_name.Size = new System.Drawing.Size(27, 13);
      this.m_label_name.TabIndex = 7;
      this.m_label_name.Text = "Name";
      // 
      // m_font_size_list
      // 
      this.m_font_size_list.Location = new System.Drawing.Point(127, 90);
      this.m_font_size_list.Name = "m_font_size_list";
      this.m_font_size_list.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_font_size_list.Properties.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
      this.m_font_size_list.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_font_size_list.Size = new System.Drawing.Size(153, 20);
      this.m_font_size_list.TabIndex = 8;
      this.m_font_size_list.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_label_font_size
      // 
      this.m_label_font_size.Location = new System.Drawing.Point(26, 93);
      this.m_label_font_size.Name = "m_label_font_size";
      this.m_label_font_size.Size = new System.Drawing.Size(43, 13);
      this.m_label_font_size.TabIndex = 9;
      this.m_label_font_size.Text = "Font size";
      // 
      // m_check_bold
      // 
      this.m_check_bold.Location = new System.Drawing.Point(125, 116);
      this.m_check_bold.Name = "m_check_bold";
      this.m_check_bold.Properties.Caption = "";
      this.m_check_bold.Size = new System.Drawing.Size(75, 18);
      this.m_check_bold.TabIndex = 10;
      this.m_check_bold.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_check_italic
      // 
      this.m_check_italic.Location = new System.Drawing.Point(125, 140);
      this.m_check_italic.Name = "m_check_italic";
      this.m_check_italic.Properties.Caption = "";
      this.m_check_italic.Size = new System.Drawing.Size(75, 18);
      this.m_check_italic.TabIndex = 11;
      this.m_check_italic.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_check_underline
      // 
      this.m_check_underline.Location = new System.Drawing.Point(125, 164);
      this.m_check_underline.Name = "m_check_underline";
      this.m_check_underline.Properties.Caption = "";
      this.m_check_underline.Size = new System.Drawing.Size(75, 18);
      this.m_check_underline.TabIndex = 12;
      this.m_check_underline.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      // 
      // m_memo_demo
      // 
      this.m_memo_demo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_memo_demo.EditValue = resources.GetString("m_memo_demo.EditValue");
      this.m_memo_demo.Location = new System.Drawing.Point(26, 233);
      this.m_memo_demo.Name = "m_memo_demo";
      this.m_memo_demo.Properties.ReadOnly = true;
      this.m_memo_demo.Size = new System.Drawing.Size(272, 168);
      this.m_memo_demo.TabIndex = 13;
      // 
      // m_label_underline
      // 
      this.m_label_underline.Location = new System.Drawing.Point(26, 167);
      this.m_label_underline.Name = "m_label_underline";
      this.m_label_underline.Size = new System.Drawing.Size(45, 13);
      this.m_label_underline.TabIndex = 14;
      this.m_label_underline.Text = "Underline";
      // 
      // m_label_italic
      // 
      this.m_label_italic.Location = new System.Drawing.Point(26, 143);
      this.m_label_italic.Name = "m_label_italic";
      this.m_label_italic.Size = new System.Drawing.Size(23, 13);
      this.m_label_italic.TabIndex = 15;
      this.m_label_italic.Text = "Italic";
      // 
      // m_label_bold
      // 
      this.m_label_bold.Location = new System.Drawing.Point(26, 119);
      this.m_label_bold.Name = "m_label_bold";
      this.m_label_bold.Size = new System.Drawing.Size(20, 13);
      this.m_label_bold.TabIndex = 16;
      this.m_label_bold.Text = "Bold";
      // 
      // m_label_demo
      // 
      this.m_label_demo.Location = new System.Drawing.Point(26, 214);
      this.m_label_demo.Name = "m_label_demo";
      this.m_label_demo.Size = new System.Drawing.Size(27, 13);
      this.m_label_demo.TabIndex = 17;
      this.m_label_demo.Text = "Demo";
      // 
      // m_alignment_edit
      // 
      this.m_alignment_edit.Location = new System.Drawing.Point(127, 188);
      this.m_alignment_edit.Name = "m_alignment_edit";
      this.m_alignment_edit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.m_alignment_edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.m_alignment_edit.Size = new System.Drawing.Size(153, 20);
      this.m_alignment_edit.TabIndex = 18;
      this.m_alignment_edit.EditValueChanged += new System.EventHandler(this.HandleUpdateDemo);
      this.m_alignment_edit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.m_alignment_edit_CustomDisplayText);
      // 
      // m_label_alignment
      // 
      this.m_label_alignment.Location = new System.Drawing.Point(26, 191);
      this.m_label_alignment.Name = "m_label_alignment";
      this.m_label_alignment.Size = new System.Drawing.Size(47, 13);
      this.m_label_alignment.TabIndex = 19;
      this.m_label_alignment.Text = "Alignment";
      // 
      // AddNewStyleDialog
      // 
      this.AcceptButton = this.m_cmd_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_cmd_cancel;
      this.ClientSize = new System.Drawing.Size(317, 442);
      this.Controls.Add(this.m_label_alignment);
      this.Controls.Add(this.m_alignment_edit);
      this.Controls.Add(this.m_label_demo);
      this.Controls.Add(this.m_label_bold);
      this.Controls.Add(this.m_label_italic);
      this.Controls.Add(this.m_label_underline);
      this.Controls.Add(this.m_memo_demo);
      this.Controls.Add(this.m_check_underline);
      this.Controls.Add(this.m_check_italic);
      this.Controls.Add(this.m_check_bold);
      this.Controls.Add(this.m_label_font_size);
      this.Controls.Add(this.m_font_size_list);
      this.Controls.Add(this.m_label_name);
      this.Controls.Add(this.m_label_font);
      this.Controls.Add(this.m_label_color);
      this.Controls.Add(this.m_cmd_cancel);
      this.Controls.Add(this.m_cmd_ok);
      this.Controls.Add(this.m_name_edit);
      this.Controls.Add(this.m_font_edit);
      this.Controls.Add(this.m_color_edit);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "AddNewStyleDialog";
      this.Text = "Add new style";
      ((System.ComponentModel.ISupportInitialize)(this.m_color_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_name_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_font_size_list.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_bold.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_italic.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_underline.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_memo_demo.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_alignment_edit.Properties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DevExpress.XtraEditors.ColorEdit m_color_edit;
    private DevExpress.XtraEditors.FontEdit m_font_edit;
    private DevExpress.XtraEditors.TextEdit m_name_edit;
    private DevExpress.XtraEditors.SimpleButton m_cmd_ok;
    private DevExpress.XtraEditors.SimpleButton m_cmd_cancel;
    private DevExpress.XtraEditors.LabelControl m_label_color;
    private DevExpress.XtraEditors.LabelControl m_label_font;
    private DevExpress.XtraEditors.LabelControl m_label_name;
    private DevExpress.XtraEditors.ComboBoxEdit m_font_size_list;
    private DevExpress.XtraEditors.LabelControl m_label_font_size;
    private DevExpress.XtraEditors.CheckEdit m_check_bold;
    private DevExpress.XtraEditors.CheckEdit m_check_italic;
    private DevExpress.XtraEditors.CheckEdit m_check_underline;
    private DevExpress.XtraEditors.MemoEdit m_memo_demo;
    private DevExpress.XtraEditors.LabelControl m_label_underline;
    private DevExpress.XtraEditors.LabelControl m_label_italic;
    private DevExpress.XtraEditors.LabelControl m_label_bold;
    private DevExpress.XtraEditors.LabelControl m_label_demo;
    private DevExpress.XtraEditors.ComboBoxEdit m_alignment_edit;
    private DevExpress.XtraEditors.LabelControl m_label_alignment;
  }
}