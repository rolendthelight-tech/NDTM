namespace Toolbox.GUI.Controls
{
  partial class ControlPlaceholder
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private global::System.ComponentModel.IContainer components = null;

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
      this.SuspendLayout();
      // 
      // ControlPlaceholder
      // 
			this.AutoScaleDimensions = new global::System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "ControlPlaceholder";
			this.Size = new global::System.Drawing.Size(507, 351);
			this.ControlAdded += new global::System.Windows.Forms.ControlEventHandler(this.HandleControlsChanged);
			this.ControlRemoved += new global::System.Windows.Forms.ControlEventHandler(this.HandleControlsChanged);
      this.ResumeLayout(false);

    }

    #endregion

  }
}
