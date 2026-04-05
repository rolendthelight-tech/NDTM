namespace TestSettings
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
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
      this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
      this.SuspendLayout();
      // 
      // simpleButton1
      // 
      this.simpleButton1.Location = new System.Drawing.Point(7, 127);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(170, 23);
      this.simpleButton1.TabIndex = 0;
      this.simpleButton1.Text = "Show Settings";
      this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
      // 
      // simpleButton2
      // 
      this.simpleButton2.Location = new System.Drawing.Point(13, 13);
      this.simpleButton2.Name = "simpleButton2";
      this.simpleButton2.Size = new System.Drawing.Size(75, 23);
      this.simpleButton2.TabIndex = 1;
      this.simpleButton2.Text = "simpleButton2";
      this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
      // 
      // simpleButton3
      // 
      this.simpleButton3.Location = new System.Drawing.Point(13, 43);
      this.simpleButton3.Name = "simpleButton3";
      this.simpleButton3.Size = new System.Drawing.Size(75, 23);
      this.simpleButton3.TabIndex = 2;
      this.simpleButton3.Text = "simpleButton3";
      this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
      // 
      // defaultLookAndFeel1
      // 
      this.defaultLookAndFeel1.LookAndFeel.SkinName = "Black";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(189, 162);
      this.Controls.Add(this.simpleButton3);
      this.Controls.Add(this.simpleButton2);
      this.Controls.Add(this.simpleButton1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SimpleButton simpleButton1;
    private DevExpress.XtraEditors.SimpleButton simpleButton2;
    private DevExpress.XtraEditors.SimpleButton simpleButton3;
    private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
  }
}

