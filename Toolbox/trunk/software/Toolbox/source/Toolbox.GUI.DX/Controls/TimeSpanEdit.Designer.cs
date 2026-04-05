namespace Toolbox.GUI.DX.Controls
{
  partial class TimeSpanEdit
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support — do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
      this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
      ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
      this.SuspendLayout();
      // 
      // comboBoxEdit1
      // 
      this.comboBoxEdit1.Dock = System.Windows.Forms.DockStyle.Right;
      this.comboBoxEdit1.Location = new System.Drawing.Point(81, 0);
      this.comboBoxEdit1.Margin = new System.Windows.Forms.Padding(0);
      this.comboBoxEdit1.Name = "comboBoxEdit1";
      this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
      this.comboBoxEdit1.Properties.Items.AddRange(new object[] {
            "Millisecond",
            "Second",
            "Minute",
            "Hour",
            "Day",
            "Week",
            "Month",
            "Year"});
      this.comboBoxEdit1.Size = new System.Drawing.Size(119, 20);
      this.comboBoxEdit1.TabIndex = 0;
      // 
      // spinEdit1
      // 
      this.spinEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.spinEdit1.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
      this.spinEdit1.Location = new System.Drawing.Point(0, 0);
      this.spinEdit1.Margin = new System.Windows.Forms.Padding(0);
      this.spinEdit1.Name = "spinEdit1";
      this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.spinEdit1.Size = new System.Drawing.Size(81, 20);
      this.spinEdit1.TabIndex = 1;
      // 
      // TimeSpanEdit
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.spinEdit1);
      this.Controls.Add(this.comboBoxEdit1);
      this.MinimumSize = new System.Drawing.Size(200, 20);
      this.Name = "TimeSpanEdit";
      this.Size = new System.Drawing.Size(200, 20);
      ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
    private DevExpress.XtraEditors.SpinEdit spinEdit1;

  }
}
