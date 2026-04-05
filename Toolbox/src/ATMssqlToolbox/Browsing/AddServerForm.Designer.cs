namespace AT.Toolbox.MSSQL
{
  partial class AddServerForm
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
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.m_check_btn = new DevExpress.XtraEditors.SimpleButton();
      this.sqlServerEditControl1 = new AT.Toolbox.MSSQL.SqlServerEditControl();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(234, 221);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 0;
      this.m_ok_btn.Text = "OK";
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(315, 221);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 1;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::AT.Toolbox.MSSQL.Properties.Resources.p_48_server_add;
      this.pictureBox1.Location = new System.Drawing.Point(6, 8);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(50, 50);
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // m_check_btn
      // 
      this.m_check_btn.Location = new System.Drawing.Point(12, 221);
      this.m_check_btn.Name = "m_check_btn";
      this.m_check_btn.Size = new System.Drawing.Size(194, 23);
      this.m_check_btn.TabIndex = 4;
      this.m_check_btn.Text = "Check";
      this.m_check_btn.Click += new System.EventHandler(this.HandleCheckConnection);
      // 
      // sqlServerEditControl1
      // 
      this.sqlServerEditControl1.ConnectionString = "Data Source=.;Initial Catalog=master;Integrated Security=True;Persist Security In" +
          "fo=False";
      this.sqlServerEditControl1.HelpURI = null;
      this.sqlServerEditControl1.Location = new System.Drawing.Point(68, 6);
      this.sqlServerEditControl1.MinimumSize = new System.Drawing.Size(300, 200);
      this.sqlServerEditControl1.Name = "sqlServerEditControl1";
      this.sqlServerEditControl1.Size = new System.Drawing.Size(322, 209);
      this.sqlServerEditControl1.TabIndex = 2;
      // 
      // AddServerForm
      // 
      this.AcceptButton = this.m_ok_btn;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_cancel_btn;
      this.ClientSize = new System.Drawing.Size(394, 251);
      this.Controls.Add(this.m_check_btn);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.sqlServerEditControl1);
      this.Controls.Add(this.m_cancel_btn);
      this.Controls.Add(this.m_ok_btn);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "AddServerForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "AddServerForm";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private SqlServerEditControl sqlServerEditControl1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private DevExpress.XtraEditors.SimpleButton m_check_btn;
  }
}