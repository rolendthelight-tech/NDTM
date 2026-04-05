namespace TestNetworkListers
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
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
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
    /// Required method for Designer support — do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
      this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
      this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
      this.logLister1 = new AT.Toolbox.Controls.LogLister();
      this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
      this.simpleButton5 = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
      this.splitContainerControl1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainerControl1
      // 
      this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainerControl1.Horizontal = false;
      this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
      this.splitContainerControl1.Name = "splitContainerControl1";
      this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton5);
      this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton4);
      this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton3);
      this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton2);
      this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton1);
      this.splitContainerControl1.Panel1.Controls.Add(this.listBoxControl1);
      this.splitContainerControl1.Panel1.Text = "Panel1";
      this.splitContainerControl1.Panel2.Controls.Add(this.logLister1);
      this.splitContainerControl1.Panel2.Text = "Panel2";
      this.splitContainerControl1.Size = new System.Drawing.Size(657, 520);
      this.splitContainerControl1.SplitterPosition = 319;
      this.splitContainerControl1.TabIndex = 0;
      this.splitContainerControl1.Text = "splitContainerControl1";
      // 
      // listBoxControl1
      // 
      this.listBoxControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBoxControl1.Location = new System.Drawing.Point(-2, 1);
      this.listBoxControl1.Name = "listBoxControl1";
      this.listBoxControl1.Size = new System.Drawing.Size(495, 311);
      this.listBoxControl1.TabIndex = 0;
      // 
      // simpleButton1
      // 
      this.simpleButton1.Location = new System.Drawing.Point(499, 1);
      this.simpleButton1.Name = "simpleButton1";
      this.simpleButton1.Size = new System.Drawing.Size(152, 23);
      this.simpleButton1.TabIndex = 1;
      this.simpleButton1.Text = "Computers";
      this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
      // 
      // simpleButton2
      // 
      this.simpleButton2.Location = new System.Drawing.Point(499, 30);
      this.simpleButton2.Name = "simpleButton2";
      this.simpleButton2.Size = new System.Drawing.Size(152, 23);
      this.simpleButton2.TabIndex = 2;
      this.simpleButton2.Text = "SqlServers";
      this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
      // 
      // simpleButton3
      // 
      this.simpleButton3.Location = new System.Drawing.Point(499, 59);
      this.simpleButton3.Name = "simpleButton3";
      this.simpleButton3.Size = new System.Drawing.Size(152, 23);
      this.simpleButton3.TabIndex = 3;
      this.simpleButton3.Text = "Servers";
      this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
      // 
      // logLister1
      // 
      this.logLister1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.logLister1.Location = new System.Drawing.Point(0, 0);
      this.logLister1.Name = "logLister1";
      this.logLister1.Size = new System.Drawing.Size(653, 191);
      this.logLister1.TabIndex = 0;
      // 
      // simpleButton4
      // 
      this.simpleButton4.Location = new System.Drawing.Point(499, 88);
      this.simpleButton4.Name = "simpleButton4";
      this.simpleButton4.Size = new System.Drawing.Size(152, 23);
      this.simpleButton4.TabIndex = 4;
      this.simpleButton4.Text = "Sql Server Alt";
      this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
      // 
      // simpleButton5
      // 
      this.simpleButton5.Location = new System.Drawing.Point(499, 117);
      this.simpleButton5.Name = "simpleButton5";
      this.simpleButton5.Size = new System.Drawing.Size(152, 23);
      this.simpleButton5.TabIndex = 5;
      this.simpleButton5.Text = "Domain Controller";
      this.simpleButton5.Click += new System.EventHandler(this.simpleButton5_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(657, 520);
      this.Controls.Add(this.splitContainerControl1);
      this.Name = "Form1";
      this.Text = "TestListers";
      ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
      this.splitContainerControl1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
    private DevExpress.XtraEditors.SimpleButton simpleButton2;
    private DevExpress.XtraEditors.SimpleButton simpleButton1;
    private DevExpress.XtraEditors.ListBoxControl listBoxControl1;
    private AT.Toolbox.Controls.LogLister logLister1;
    private DevExpress.XtraEditors.SimpleButton simpleButton3;
    private DevExpress.XtraEditors.SimpleButton simpleButton4;
    private DevExpress.XtraEditors.SimpleButton simpleButton5;
  }
}

