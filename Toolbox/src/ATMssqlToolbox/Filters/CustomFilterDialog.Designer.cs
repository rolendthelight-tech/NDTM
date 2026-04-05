namespace AT.Toolbox.DB
{
  partial class CustomFilterDialog
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
      this.m_filter_tree = new DevExpress.XtraTreeList.TreeList();
      this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_add_group_cmd = new System.Windows.Forms.ToolStripMenuItem();
      this.m_remove_group_cmd = new System.Windows.Forms.ToolStripMenuItem();
      this.m_add_filter_cmd = new System.Windows.Forms.ToolStripMenuItem();
      this.m_remove_filter_cmd = new System.Windows.Forms.ToolStripMenuItem();
      this.m_filter_editor = new DevExpress.XtraEditors.FilterControl();
      this.m_splitter = new DevExpress.XtraEditors.SplitContainerControl();
      this.m_btn_cancel = new DevExpress.XtraEditors.SimpleButton();
      this.m_btn_ok = new DevExpress.XtraEditors.SimpleButton();
      ((System.ComponentModel.ISupportInitialize)(this.m_filter_tree)).BeginInit();
      this.m_context_menu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
      this.m_splitter.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_filter_tree
      // 
      this.m_filter_tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1});
      this.m_filter_tree.ContextMenuStrip = this.m_context_menu;
      this.m_filter_tree.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_filter_tree.ImeMode = System.Windows.Forms.ImeMode.Disable;
      this.m_filter_tree.KeyFieldName = "FilterID";
      this.m_filter_tree.Location = new System.Drawing.Point(0, 0);
      this.m_filter_tree.Name = "m_filter_tree";
      this.m_filter_tree.OptionsBehavior.Editable = false;
      this.m_filter_tree.OptionsBehavior.EnableFiltering = true;
      this.m_filter_tree.OptionsBehavior.ImmediateEditor = false;
      this.m_filter_tree.OptionsSelection.UseIndicatorForSelection = true;
      this.m_filter_tree.OptionsView.ShowColumns = false;
      this.m_filter_tree.OptionsView.ShowHorzLines = false;
      this.m_filter_tree.OptionsView.ShowIndicator = false;
      this.m_filter_tree.Size = new System.Drawing.Size(223, 396);
      this.m_filter_tree.TabIndex = 0;
      this.m_filter_tree.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.FocusChanged);
      this.m_filter_tree.FilterNode += new DevExpress.XtraTreeList.FilterNodeEventHandler(this.FilterNodes);
      // 
      // treeListColumn1
      // 
      this.treeListColumn1.Caption = "treeListColumn1";
      this.treeListColumn1.FieldName = "FilterName";
      this.treeListColumn1.Name = "treeListColumn1";
      this.treeListColumn1.Visible = true;
      this.treeListColumn1.VisibleIndex = 0;
      // 
      // m_context_menu
      // 
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_add_group_cmd,
            this.m_remove_group_cmd,
            this.m_add_filter_cmd,
            this.m_remove_filter_cmd});
      this.m_context_menu.Name = "contextMenuStrip1";
      this.m_context_menu.Size = new System.Drawing.Size(184, 92);
      this.m_context_menu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenOpening);
      // 
      // m_add_group_cmd
      // 
      this.m_add_group_cmd.Name = "m_add_group_cmd";
      this.m_add_group_cmd.Size = new System.Drawing.Size(183, 22);
      this.m_add_group_cmd.Text = "Add Filter Group";
      this.m_add_group_cmd.Click += new System.EventHandler(this.AddFilterGroup);
      // 
      // m_remove_group_cmd
      // 
      this.m_remove_group_cmd.Name = "m_remove_group_cmd";
      this.m_remove_group_cmd.Size = new System.Drawing.Size(183, 22);
      this.m_remove_group_cmd.Text = "Remove Filter Group";
      this.m_remove_group_cmd.Click += new System.EventHandler(this.RemoveFilterGroup);
      // 
      // m_add_filter_cmd
      // 
      this.m_add_filter_cmd.Name = "m_add_filter_cmd";
      this.m_add_filter_cmd.Size = new System.Drawing.Size(183, 22);
      this.m_add_filter_cmd.Text = "Add Filter";
      this.m_add_filter_cmd.Click += new System.EventHandler(this.AddFilter);
      // 
      // m_remove_filter_cmd
      // 
      this.m_remove_filter_cmd.Name = "m_remove_filter_cmd";
      this.m_remove_filter_cmd.Size = new System.Drawing.Size(183, 22);
      this.m_remove_filter_cmd.Text = "Remove Filter";
      this.m_remove_filter_cmd.Click += new System.EventHandler(this.RemoveFilter);
      // 
      // m_filter_editor
      // 
      this.m_filter_editor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_filter_editor.Location = new System.Drawing.Point(0, 0);
      this.m_filter_editor.Name = "m_filter_editor";
      this.m_filter_editor.Size = new System.Drawing.Size(394, 396);
      this.m_filter_editor.TabIndex = 0;
      this.m_filter_editor.Text = "filterControl1";
      this.m_filter_editor.Visible = false;
      // 
      // m_splitter
      // 
      this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_splitter.Location = new System.Drawing.Point(0, 0);
      this.m_splitter.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
      this.m_splitter.Name = "m_splitter";
      this.m_splitter.Panel1.Controls.Add(this.m_filter_tree);
      this.m_splitter.Panel1.Text = "Panel1";
      this.m_splitter.Panel2.Controls.Add(this.m_filter_editor);
      this.m_splitter.Panel2.Text = "Panel2";
      this.m_splitter.Size = new System.Drawing.Size(633, 400);
      this.m_splitter.SplitterPosition = 227;
      this.m_splitter.TabIndex = 0;
      this.m_splitter.Text = "splitContainerControl1";
      // 
      // m_btn_cancel
      // 
      this.m_btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_btn_cancel.Location = new System.Drawing.Point(535, 410);
      this.m_btn_cancel.Name = "m_btn_cancel";
      this.m_btn_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_btn_cancel.TabIndex = 1;
      this.m_btn_cancel.Text = "Cancel";
      // 
      // m_btn_ok
      // 
      this.m_btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btn_ok.Location = new System.Drawing.Point(454, 410);
      this.m_btn_ok.Name = "m_btn_ok";
      this.m_btn_ok.Size = new System.Drawing.Size(75, 23);
      this.m_btn_ok.TabIndex = 2;
      this.m_btn_ok.Text = "OK";
     
      // 
      // CustomFilterDialog
      // 
      this.AcceptButton = this.m_btn_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_btn_cancel;
      this.ClientSize = new System.Drawing.Size(633, 440);
      this.Controls.Add(this.m_btn_ok);
      this.Controls.Add(this.m_btn_cancel);
      this.Controls.Add(this.m_splitter);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "CustomFilterDialog";
      this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 40);
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Custom Filter Dialog";
      this.Load += new System.EventHandler(this.DialogShown);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogClosing);
      ((System.ComponentModel.ISupportInitialize)(this.m_filter_tree)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
      this.m_splitter.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private DevExpress.XtraEditors.SplitContainerControl m_splitter;
    private DevExpress.XtraEditors.FilterControl m_filter_editor;
    private DevExpress.XtraEditors.SimpleButton m_btn_cancel;
    private DevExpress.XtraEditors.SimpleButton m_btn_ok;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_add_group_cmd;
    private System.Windows.Forms.ToolStripMenuItem m_remove_group_cmd;
    private System.Windows.Forms.ToolStripMenuItem m_add_filter_cmd;
    private System.Windows.Forms.ToolStripMenuItem m_remove_filter_cmd;
    private DevExpress.XtraTreeList.TreeList m_filter_tree;
    private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
  }
}