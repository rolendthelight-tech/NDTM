namespace AT.Toolbox.Dialogs
{
  partial class DialogBase
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
      this.m_bottom_panel = new DevExpress.XtraEditors.PanelControl();
      this.m_buttons = new AT.Toolbox.Controls.DialogButtonPanel();
      ((System.ComponentModel.ISupportInitialize)(this.m_bottom_panel)).BeginInit();
      this.m_bottom_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_bottom_panel
      // 
      this.m_bottom_panel.Controls.Add(this.m_buttons);
      this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_bottom_panel.Location = new System.Drawing.Point(0, 295);
      this.m_bottom_panel.Name = "m_bottom_panel";
      this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(5);
      this.m_bottom_panel.Size = new System.Drawing.Size(398, 43);
      this.m_bottom_panel.TabIndex = 0;
      // 
      // m_buttons
      // 
      this.m_buttons.AutoSize = true;
      this.m_buttons.Buttons = System.Windows.Forms.MessageBoxButtons.OKCancel;
      this.m_buttons.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_buttons.HelpURI = null;
      this.m_buttons.Location = new System.Drawing.Point(7, 7);
      this.m_buttons.Name = "m_buttons";
      this.m_buttons.Size = new System.Drawing.Size(384, 29);
      this.m_buttons.TabIndex = 0;
      // 
      // DialogBase
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(398, 338);
      this.Controls.Add(this.m_bottom_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "DialogBase";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      ((System.ComponentModel.ISupportInitialize)(this.m_bottom_panel)).EndInit();
      this.m_bottom_panel.ResumeLayout(false);
      this.m_bottom_panel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.PanelControl m_bottom_panel;
    protected AT.Toolbox.Controls.DialogButtonPanel m_buttons;
  }
}