using AT.Toolbox.Help;

namespace ATHelpEditor
{
  partial class URIEdit
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
      this.m_check_edit_resource = new DevExpress.XtraEditors.CheckEdit();
      this.buttonEdit1 = new DevExpress.XtraEditors.ButtonEdit();
      this.m_check_edit_copy = new DevExpress.XtraEditors.CheckEdit();
      this.treeList1 = new DevExpress.XtraTreeList.TreeList();
      this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.colIndex = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.helpNodeViewBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.m_check_edit_topic = new DevExpress.XtraEditors.CheckEdit();
      this.m_btn_ok = new DevExpress.XtraEditors.SimpleButton();
      this.m_btn_cancel = new DevExpress.XtraEditors.SimpleButton();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_resource.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_copy.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpNodeViewBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_topic.Properties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // m_check_edit_resource
      // 
      this.m_check_edit_resource.EditValue = true;
      this.m_check_edit_resource.Location = new System.Drawing.Point(77, 14);
      this.m_check_edit_resource.Name = "m_check_edit_resource";
      this.m_check_edit_resource.Properties.Caption = "Файл или страница";
      this.m_check_edit_resource.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
      this.m_check_edit_resource.Size = new System.Drawing.Size(138, 19);
      this.m_check_edit_resource.TabIndex = 1;
      this.m_check_edit_resource.CheckedChanged += new System.EventHandler(this.URIModeOn);
      // 
      // buttonEdit1
      // 
      this.buttonEdit1.Location = new System.Drawing.Point(221, 12);
      this.buttonEdit1.Name = "buttonEdit1";
      this.buttonEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
      this.buttonEdit1.Size = new System.Drawing.Size(303, 20);
      this.buttonEdit1.TabIndex = 2;
      // 
      // m_check_edit_copy
      // 
      this.m_check_edit_copy.Location = new System.Drawing.Point(219, 38);
      this.m_check_edit_copy.Name = "m_check_edit_copy";
      this.m_check_edit_copy.Properties.Caption = "Copy Locally";
      this.m_check_edit_copy.Size = new System.Drawing.Size(305, 19);
      this.m_check_edit_copy.TabIndex = 3;
      this.m_check_edit_copy.CheckedChanged += new System.EventHandler(this.checkEdit2_CheckedChanged);
      // 
      // treeList1
      // 
      this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colIndex});
      this.treeList1.DataSource = this.helpNodeViewBindingSource;
      this.treeList1.Enabled = false;
      this.treeList1.Location = new System.Drawing.Point(221, 78);
      this.treeList1.Name = "treeList1";
      this.treeList1.Size = new System.Drawing.Size(303, 408);
      this.treeList1.TabIndex = 4;
      // 
      // colName
      // 
      this.colName.Caption = "Name";
      this.colName.FieldName = "DisplayName";
      this.colName.Name = "colName";
      this.colName.OptionsColumn.AllowEdit = false;
      this.colName.OptionsColumn.ReadOnly = true;
      this.colName.Visible = true;
      this.colName.VisibleIndex = 0;
      this.colName.Width = 87;
      // 
      // colIndex
      // 
      this.colIndex.Caption = "Index";
      this.colIndex.FieldName = "ID";
      this.colIndex.Name = "colIndex";
      this.colIndex.OptionsColumn.AllowEdit = false;
      this.colIndex.OptionsColumn.ReadOnly = true;
      this.colIndex.Visible = true;
      this.colIndex.VisibleIndex = 1;
      this.colIndex.Width = 86;
      // 
      // helpNodeViewBindingSource
      // 
      this.helpNodeViewBindingSource.DataSource = typeof(AT.Toolbox.Help.HelpFileNode);
      // 
      // m_check_edit_topic
      // 
      this.m_check_edit_topic.Location = new System.Drawing.Point(77, 76);
      this.m_check_edit_topic.Name = "m_check_edit_topic";
      this.m_check_edit_topic.Properties.Caption = "Тема справки";
      this.m_check_edit_topic.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
      this.m_check_edit_topic.Size = new System.Drawing.Size(138, 19);
      this.m_check_edit_topic.TabIndex = 5;
      this.m_check_edit_topic.CheckedChanged += new System.EventHandler(this.OnHelpMode);
      // 
      // m_btn_ok
      // 
      this.m_btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btn_ok.Location = new System.Drawing.Point(368, 492);
      this.m_btn_ok.Name = "m_btn_ok";
      this.m_btn_ok.Size = new System.Drawing.Size(75, 23);
      this.m_btn_ok.TabIndex = 6;
      this.m_btn_ok.Text = "ok";
      // 
      // m_btn_cancel
      // 
      this.m_btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_btn_cancel.Location = new System.Drawing.Point(449, 492);
      this.m_btn_cancel.Name = "m_btn_cancel";
      this.m_btn_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_btn_cancel.TabIndex = 7;
      this.m_btn_cancel.Text = "CANCEL";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::ATHelpEditor.Properties.Resources._48_hyperlink_add;
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(50, 50);
      this.pictureBox1.TabIndex = 8;
      this.pictureBox1.TabStop = false;
      // 
      // URIEdit
      // 
      this.AcceptButton = this.m_btn_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_btn_cancel;
      this.ClientSize = new System.Drawing.Size(536, 522);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.m_btn_cancel);
      this.Controls.Add(this.m_btn_ok);
      this.Controls.Add(this.m_check_edit_topic);
      this.Controls.Add(this.treeList1);
      this.Controls.Add(this.m_check_edit_copy);
      this.Controls.Add(this.buttonEdit1);
      this.Controls.Add(this.m_check_edit_resource);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "URIEdit";
      this.Text = "URIEdit";
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_resource.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_copy.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.helpNodeViewBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_check_edit_topic.Properties)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.CheckEdit m_check_edit_resource;
    private DevExpress.XtraEditors.ButtonEdit buttonEdit1;
    private DevExpress.XtraEditors.CheckEdit m_check_edit_copy;
    private DevExpress.XtraTreeList.TreeList treeList1;
    private DevExpress.XtraEditors.CheckEdit m_check_edit_topic;
    private DevExpress.XtraEditors.SimpleButton m_btn_ok;
    private DevExpress.XtraEditors.SimpleButton m_btn_cancel;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colIndex;
    private System.Windows.Forms.BindingSource helpNodeViewBindingSource;
    private System.Windows.Forms.PictureBox pictureBox1;
  }
}