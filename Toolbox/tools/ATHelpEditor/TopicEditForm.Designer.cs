namespace ATHelpEditor
{
  partial class TopicEditForm
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
      this.m_label_topic_id = new DevExpress.XtraEditors.LabelControl();
      this.m_topic_ID_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_topic_name_edit = new DevExpress.XtraEditors.TextEdit();
      this.m_label_topic_name = new DevExpress.XtraEditors.LabelControl();
      this.m_cancel_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_ok_btn = new DevExpress.XtraEditors.SimpleButton();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.m_topic_ID_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_topic_name_edit.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_label_topic_id
      // 
      this.m_label_topic_id.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_label_topic_id.Location = new System.Drawing.Point(81, 12);
      this.m_label_topic_id.Name = "m_label_topic_id";
      this.m_label_topic_id.Size = new System.Drawing.Size(124, 20);
      this.m_label_topic_id.TabIndex = 0;
      this.m_label_topic_id.Text = "Topic ID";
      // 
      // m_topic_ID_edit
      // 
      this.m_topic_ID_edit.Location = new System.Drawing.Point(211, 12);
      this.m_topic_ID_edit.Name = "m_topic_ID_edit";
      this.m_topic_ID_edit.Size = new System.Drawing.Size(233, 20);
      this.m_topic_ID_edit.TabIndex = 1;
      // 
      // m_topic_name_edit
      // 
      this.m_topic_name_edit.Location = new System.Drawing.Point(211, 38);
      this.m_topic_name_edit.Name = "m_topic_name_edit";
      this.m_topic_name_edit.Size = new System.Drawing.Size(233, 20);
      this.m_topic_name_edit.TabIndex = 3;
      // 
      // m_label_topic_name
      // 
      this.m_label_topic_name.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
      this.m_label_topic_name.Location = new System.Drawing.Point(81, 38);
      this.m_label_topic_name.Name = "m_label_topic_name";
      this.m_label_topic_name.Size = new System.Drawing.Size(124, 20);
      this.m_label_topic_name.TabIndex = 2;
      this.m_label_topic_name.Text = "Topic Name";
      // 
      // m_cancel_btn
      // 
      this.m_cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel_btn.Location = new System.Drawing.Point(369, 64);
      this.m_cancel_btn.Name = "m_cancel_btn";
      this.m_cancel_btn.Size = new System.Drawing.Size(75, 23);
      this.m_cancel_btn.TabIndex = 4;
      this.m_cancel_btn.Text = "Cancel";
      // 
      // m_ok_btn
      // 
      this.m_ok_btn.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok_btn.Location = new System.Drawing.Point(288, 64);
      this.m_ok_btn.Name = "m_ok_btn";
      this.m_ok_btn.Size = new System.Drawing.Size(75, 23);
      this.m_ok_btn.TabIndex = 5;
      this.m_ok_btn.Text = "OK";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::ATHelpEditor.Properties.Resources._48_help;
      this.pictureBox1.Location = new System.Drawing.Point(13, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(50, 50);
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      // 
      // TopicEditForm
      // 
      this.AcceptButton = this.m_ok_btn;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_cancel_btn;
      this.ClientSize = new System.Drawing.Size(452, 93);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.m_ok_btn);
      this.Controls.Add(this.m_cancel_btn);
      this.Controls.Add(this.m_topic_name_edit);
      this.Controls.Add(this.m_label_topic_name);
      this.Controls.Add(this.m_topic_ID_edit);
      this.Controls.Add(this.m_label_topic_id);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "TopicEditForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "TopicEditForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_topic_ID_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_topic_name_edit.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.LabelControl m_label_topic_id;
    private DevExpress.XtraEditors.TextEdit m_topic_ID_edit;
    private DevExpress.XtraEditors.TextEdit m_topic_name_edit;
    private DevExpress.XtraEditors.LabelControl m_label_topic_name;
    private DevExpress.XtraEditors.SimpleButton m_cancel_btn;
    private DevExpress.XtraEditors.SimpleButton m_ok_btn;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}