namespace AT.Toolbox.MSSQL.Browsing
{
  partial class SimpleConnectionControl
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
      this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
      this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.sqlServerEditControl1 = new AT.Toolbox.MSSQL.SqlServerEditControl();
      ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // labelControl3
      // 
      this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl3.Enabled = false;
      this.labelControl3.Location = new System.Drawing.Point(12, 242);
      this.labelControl3.Name = "labelControl3";
      this.labelControl3.Size = new System.Drawing.Size(89, 20);
      this.labelControl3.TabIndex = 8;
      this.labelControl3.Text = "База Данных";
      // 
      // comboBoxEdit1
      // 
      this.comboBoxEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxEdit1.Enabled = false;
      this.comboBoxEdit1.Location = new System.Drawing.Point(108, 242);
      this.comboBoxEdit1.Name = "comboBoxEdit1";
      this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      this.comboBoxEdit1.Size = new System.Drawing.Size(231, 20);
      this.comboBoxEdit1.TabIndex = 11;
      this.comboBoxEdit1.EditValueChanged += new System.EventHandler(this.comboBoxEdit1_EditValueChanged);
      this.comboBoxEdit1.EnabledChanged += new System.EventHandler(this.gridLookUpEdit1_EnabledChanged);
      // 
      // simpleButton1
      // 
      this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.simpleButton1.Location = new System.Drawing.Point(107, 209);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(232, 23);
      this.simpleButton1.TabIndex = 10;
      this.simpleButton1.Text = "Получить список баз данных";
      this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
      // 
      // sqlServerEditControl1
      // 
      this.sqlServerEditControl1.ConnectionString = "Data Source=.;Initial Catalog=master;Integrated Security=True;Persist Security In" +
          "fo=False";
      this.sqlServerEditControl1.Dock = System.Windows.Forms.DockStyle.Top;
      this.sqlServerEditControl1.HelpURI = null;
      this.sqlServerEditControl1.Location = new System.Drawing.Point(0, 0);
      this.sqlServerEditControl1.MinimumSize = new System.Drawing.Size(300, 180);
      this.sqlServerEditControl1.Name = "sqlServerEditControl1";
      this.sqlServerEditControl1.Size = new System.Drawing.Size(342, 203);
      this.sqlServerEditControl1.TabIndex = 3;
      this.sqlServerEditControl1.EditValueChanged += new System.EventHandler(this.sqlServerEditControl1_EditValueChanged);
      // 
      // SimpleConnectionControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.comboBoxEdit1);
      this.Controls.Add(this.simpleButton1);
      this.Controls.Add(this.labelControl3);
      this.Controls.Add(this.sqlServerEditControl1);
      this.MinimumSize = new System.Drawing.Size(342, 250);
      this.Name = "SimpleConnectionControl";
      this.Size = new System.Drawing.Size(342, 266);
      ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private SqlServerEditControl sqlServerEditControl1;
    private DevExpress.XtraEditors.LabelControl labelControl3;
    private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
    private DevExpress.XtraEditors.SimpleButton simpleButton1;
  }
}
