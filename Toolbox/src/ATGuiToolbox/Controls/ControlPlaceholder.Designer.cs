namespace AT.Toolbox.Controls
{
  partial class ControlPlaceholder
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
      this.SuspendLayout();
      // 
      // ControlPlaceholder
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ControlPlaceholder";
      this.Size = new System.Drawing.Size(507, 351);
      this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.HandleControlsChanged);
      this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.HandleControlsChanged);
      this.ResumeLayout(false);

    }

    #endregion

  }
}
