namespace TestMail
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
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
      this.SuspendLayout();
      // 
      // simpleButton1
      // 
      this.simpleButton1.Location = new System.Drawing.Point(4, 311);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(75, 23);
      this.simpleButton1.TabIndex = 1;
      this.simpleButton1.Text = "Connect";
      this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 0);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(736, 304);
      this.logLister1.TabIndex = 0;
      // 
      // simpleButton2
      // 
      this.simpleButton2.Location = new System.Drawing.Point(86, 311);
      this.simpleButton2.Name = "simpleButton2";
      this.simpleButton2.Size = new System.Drawing.Size(75, 23);
      this.simpleButton2.TabIndex = 2;
      this.simpleButton2.Text = "Disconnect";
      this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
      // 
      // simpleButton3
      // 
      this.simpleButton3.Location = new System.Drawing.Point(168, 311);
      this.simpleButton3.Name = "simpleButton3";
      this.simpleButton3.Size = new System.Drawing.Size(75, 23);
      this.simpleButton3.TabIndex = 3;
      this.simpleButton3.Text = "Check emails";
      this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(736, 354);
      this.Controls.Add(this.simpleButton3);
      this.Controls.Add(this.simpleButton2);
      this.Controls.Add(this.simpleButton1);
      this.Controls.Add(this.logLister1);
      this.Name = "Form1";
      this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 50);
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraEditors.SimpleButton simpleButton1;
    private DevExpress.XtraEditors.SimpleButton simpleButton2;
    private DevExpress.XtraEditors.SimpleButton simpleButton3;
  }
}

