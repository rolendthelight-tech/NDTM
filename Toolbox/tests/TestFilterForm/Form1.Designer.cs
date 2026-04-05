namespace TestFilterForm
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
      this.m_instance_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_db_edit = new DevExpress.XtraEditors.TextEdit();
      this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.m_login_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_pass_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_table_edit = new DevExpress.XtraEditors.TextEdit();
      ((System.ComponentModel.ISupportInitialize)(this.m_instance_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_db_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_login_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_pass_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // m_instance_edit
      // 
      this.m_instance_edit.Location = new System.Drawing.Point(157, 12);
      this.m_instance_edit.Name = "m_instance_edit";
      this.m_instance_edit.Size = new System.Drawing.Size(342, 20);
      this.m_instance_edit.TabIndex = 1;
      // 
      // m_db_edit
      // 
      this.m_db_edit.Location = new System.Drawing.Point(157, 113);
      this.m_db_edit.Name = "m_db_edit";
      this.m_db_edit.Size = new System.Drawing.Size(342, 20);
      this.m_db_edit.TabIndex = 3;
      this.m_db_edit.EditValueChanged += new System.EventHandler(this.textEdit3_EditValueChanged);
      // 
      // radioGroup1
      // 
      this.radioGroup1.Location = new System.Drawing.Point(157, 39);
      this.radioGroup1.Name = "radioGroup1";
      this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Win"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "MSSQL")});
      this.radioGroup1.Size = new System.Drawing.Size(342, 42);
      this.radioGroup1.TabIndex = 4;
      // 
      // simpleButton1
      // 
      this.simpleButton1.Location = new System.Drawing.Point(326, 139);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(173, 23);
      this.simpleButton1.TabIndex = 5;
      this.simpleButton1.Text = "Test connection";
      this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
      // 
      // simpleButton2
      // 
      this.simpleButton2.Location = new System.Drawing.Point(326, 198);
      this.simpleButton2.Name = "simpleButton2";
      this.simpleButton2.Size = new System.Drawing.Size(173, 23);
      this.simpleButton2.TabIndex = 6;
      this.simpleButton2.Text = "Show filter editor";
      this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.logLister1.Location = new System.Drawing.Point(0, 261);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(511, 306);
      this.logLister1.TabIndex = 0;
      // 
      // m_login_edit
      // 
      this.m_login_edit.Location = new System.Drawing.Point(157, 87);
      this.m_login_edit.Name = "m_login_edit";
      this.m_login_edit.Size = new System.Drawing.Size(163, 20);
      this.m_login_edit.TabIndex = 7;
      // 
      // m_pass_edit
      // 
      this.m_pass_edit.Location = new System.Drawing.Point(326, 87);
      this.m_pass_edit.Name = "m_pass_edit";
      this.m_pass_edit.Size = new System.Drawing.Size(173, 20);
      this.m_pass_edit.TabIndex = 8;
      // 
      // m_table_edit
      // 
      this.m_table_edit.Location = new System.Drawing.Point(157, 168);
      this.m_table_edit.Name = "m_table_edit";
      this.m_table_edit.Size = new System.Drawing.Size(342, 20);
      this.m_table_edit.TabIndex = 9;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(511, 567);
      this.Controls.Add(this.m_table_edit);
      this.Controls.Add(this.m_pass_edit);
      this.Controls.Add(this.m_login_edit);
      this.Controls.Add(this.simpleButton2);
      this.Controls.Add(this.simpleButton1);
      this.Controls.Add(this.radioGroup1);
      this.Controls.Add(this.m_db_edit);
      this.Controls.Add(this.m_instance_edit);
      this.Controls.Add(this.logLister1);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.m_instance_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_db_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_login_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_pass_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_table_edit.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraEditors.TextEdit m_instance_edit;
    private DevExpress.XtraEditors.TextEdit m_db_edit;
    private DevExpress.XtraEditors.RadioGroup radioGroup1;
    private DevExpress.XtraEditors.SimpleButton simpleButton1;
    private DevExpress.XtraEditors.SimpleButton simpleButton2;
    private DevExpress.XtraEditors.TextEdit m_login_edit;
    private DevExpress.XtraEditors.TextEdit m_pass_edit;
    private DevExpress.XtraEditors.TextEdit m_table_edit;
  }
}

