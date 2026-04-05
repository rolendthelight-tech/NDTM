namespace AT.Toolbox.MSSQL.Authentication
{
  partial class SqlAuthenticationForm
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
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
      this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
      this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
      this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::AT.Toolbox.MSSQL.Properties.Resources.p_48_login;
      this.pictureBox1.Location = new System.Drawing.Point(13, 13);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(47, 50);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // textEdit1
      // 
      this.textEdit1.Location = new System.Drawing.Point(175, 13);
      this.textEdit1.Name = "textEdit1";
      this.textEdit1.Size = new System.Drawing.Size(213, 20);
      this.textEdit1.TabIndex = 1;
      this.textEdit1.EditValueChanged += new System.EventHandler(this.HandleValueChanged);
      // 
      // textEdit2
      // 
      this.textEdit2.Location = new System.Drawing.Point(175, 43);
      this.textEdit2.Name = "textEdit2";
      this.textEdit2.Properties.PasswordChar = '*';
      this.textEdit2.Size = new System.Drawing.Size(213, 20);
      this.textEdit2.TabIndex = 2;
      this.textEdit2.EditValueChanged += new System.EventHandler(this.HandleValueChanged);
      // 
      // labelControl1
      // 
      this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl1.CausesValidation = false;
      this.labelControl1.Location = new System.Drawing.Point(67, 13);
      this.labelControl1.Name = "labelControl1";
      this.labelControl1.Size = new System.Drawing.Size(102, 20);
      this.labelControl1.TabIndex = 3;
      this.labelControl1.Text = "labelControl1";
      // 
      // labelControl2
      // 
      this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.labelControl2.CausesValidation = false;
      this.labelControl2.Location = new System.Drawing.Point(67, 43);
      this.labelControl2.Name = "labelControl2";
      this.labelControl2.Size = new System.Drawing.Size(102, 20);
      this.labelControl2.TabIndex = 4;
      this.labelControl2.Text = "labelControl2";
      // 
      // simpleButton1
      // 
      this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.simpleButton1.Enabled = false;
      this.simpleButton1.Location = new System.Drawing.Point(229, 75);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(75, 23);
      this.simpleButton1.TabIndex = 5;
      this.simpleButton1.Text = "simpleButton1";
      // 
      // simpleButton2
      // 
      this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.simpleButton2.Location = new System.Drawing.Point(310, 75);
      this.simpleButton2.Name = "simpleButton2";
      this.simpleButton2.Size = new System.Drawing.Size(75, 23);
      this.simpleButton2.TabIndex = 6;
      this.simpleButton2.Text = "simpleButton2";
      this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
      // 
      // SqlAuthenticationForm
      // 
      this.AcceptButton = this.simpleButton1;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.simpleButton2;
      this.CausesValidation = false;
      this.ClientSize = new System.Drawing.Size(397, 109);
      this.Controls.Add(this.simpleButton2);
      this.Controls.Add(this.simpleButton1);
      this.Controls.Add(this.labelControl2);
      this.Controls.Add(this.labelControl1);
      this.Controls.Add(this.textEdit2);
      this.Controls.Add(this.textEdit1);
      this.Controls.Add(this.pictureBox1);
      this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "SqlAuthenticationForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SqlAuthentificationForm";
      this.TopMost = true;
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private DevExpress.XtraEditors.TextEdit textEdit1;
    private DevExpress.XtraEditors.TextEdit textEdit2;
    private DevExpress.XtraEditors.LabelControl labelControl1;
    private DevExpress.XtraEditors.LabelControl labelControl2;
    private DevExpress.XtraEditors.SimpleButton simpleButton1;
    private DevExpress.XtraEditors.SimpleButton simpleButton2;
  }
}