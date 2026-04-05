namespace TestWizard
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
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.Wiz = new DevExpress.XtraEditors.SimpleButton();
      this.SuspendLayout();
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 50);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(616, 339);
      this.logLister1.TabIndex = 0;
      // 
      // Wiz
      // 
      this.Wiz.Location = new System.Drawing.Point(12, 12);
      this.Wiz.Name = "Wiz";
      this.Wiz.Size = new System.Drawing.Size(75, 23);
      this.Wiz.TabIndex = 1;
      this.Wiz.Text = "Wiz";
      this.Wiz.Click += new System.EventHandler(this.Wiz_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(616, 389);
      this.Controls.Add(this.Wiz);
      this.Controls.Add(this.logLister1);
      this.Name = "Form1";
      this.Padding = new System.Windows.Forms.Padding(0, 50, 0, 0);
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraEditors.SimpleButton Wiz;
  }
}

