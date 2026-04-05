namespace Toolbox.GUI.DX.Controls
{
  partial class TwoPaneSelector
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
      this.m_full_table = new System.Windows.Forms.TableLayoutPanel();
      this.m_full_list_caption = new DevExpress.XtraEditors.GroupControl();
      this.m_full_list = new DevExpress.XtraEditors.ListBoxControl();
      this.m_selected_list_caption = new DevExpress.XtraEditors.GroupControl();
      this.m_selected_list = new DevExpress.XtraEditors.ListBoxControl();
      this.m_center_table = new System.Windows.Forms.TableLayoutPanel();
      this.m_button_panel = new DevExpress.XtraEditors.PanelControl();
      this.m_move_down_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_move_up_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_add_all_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_remove_all_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_remove_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_add_btn = new DevExpress.XtraEditors.SimpleButton();
      this.m_full_table.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_full_list_caption)).BeginInit();
      this.m_full_list_caption.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_full_list)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_selected_list_caption)).BeginInit();
      this.m_selected_list_caption.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_selected_list)).BeginInit();
      this.m_center_table.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_button_panel)).BeginInit();
      this.m_button_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_full_table
      // 
      this.m_full_table.ColumnCount = 3;
      this.m_full_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_full_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.m_full_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_full_table.Controls.Add(this.m_full_list_caption, 0, 0);
      this.m_full_table.Controls.Add(this.m_selected_list_caption, 2, 0);
      this.m_full_table.Controls.Add(this.m_center_table, 1, 0);
      this.m_full_table.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_full_table.Location = new System.Drawing.Point(0, 0);
      this.m_full_table.Name = "m_full_table";
      this.m_full_table.RowCount = 1;
      this.m_full_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.m_full_table.Size = new System.Drawing.Size(450, 307);
      this.m_full_table.TabIndex = 1;
      // 
      // m_full_list_caption
      // 
      this.m_full_list_caption.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.m_full_list_caption.AppearanceCaption.Options.UseFont = true;
      this.m_full_list_caption.Controls.Add(this.m_full_list);
      this.m_full_list_caption.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_full_list_caption.Location = new System.Drawing.Point(3, 3);
      this.m_full_list_caption.Name = "m_full_list_caption";
      this.m_full_list_caption.Size = new System.Drawing.Size(194, 301);
      this.m_full_list_caption.TabIndex = 0;
      this.m_full_list_caption.Text = "Full List";
      // 
      // m_full_list
      // 
      this.m_full_list.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_full_list.Location = new System.Drawing.Point(2, 22);
      this.m_full_list.Name = "m_full_list";
      this.m_full_list.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
      this.m_full_list.Size = new System.Drawing.Size(190, 277);
      this.m_full_list.TabIndex = 0;
      // 
      // m_selected_list_caption
      // 
      this.m_selected_list_caption.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
      this.m_selected_list_caption.AppearanceCaption.Options.UseFont = true;
      this.m_selected_list_caption.Controls.Add(this.m_selected_list);
      this.m_selected_list_caption.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_selected_list_caption.Location = new System.Drawing.Point(253, 3);
      this.m_selected_list_caption.Name = "m_selected_list_caption";
      this.m_selected_list_caption.Size = new System.Drawing.Size(194, 301);
      this.m_selected_list_caption.TabIndex = 1;
      this.m_selected_list_caption.Text = "Selected Items";
      // 
      // m_selected_list
      // 
      this.m_selected_list.DisplayMember = "Caption";
      this.m_selected_list.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_selected_list.Location = new System.Drawing.Point(2, 22);
      this.m_selected_list.Name = "m_selected_list";
      this.m_selected_list.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.m_selected_list.Size = new System.Drawing.Size(190, 277);
      this.m_selected_list.SortOrder = System.Windows.Forms.SortOrder.Ascending;
      this.m_selected_list.TabIndex = 1;
      this.m_selected_list.ValueMember = "Order";
      // 
      // m_center_table
      // 
      this.m_center_table.ColumnCount = 1;
      this.m_center_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.m_center_table.Controls.Add(this.m_button_panel, 0, 1);
      this.m_center_table.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_center_table.Location = new System.Drawing.Point(203, 3);
      this.m_center_table.Name = "m_center_table";
      this.m_center_table.RowCount = 3;
      this.m_center_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_center_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
      this.m_center_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_center_table.Size = new System.Drawing.Size(44, 301);
      this.m_center_table.TabIndex = 2;
      // 
      // m_button_panel
      // 
      this.m_button_panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
      this.m_button_panel.Controls.Add(this.m_move_down_btn);
      this.m_button_panel.Controls.Add(this.m_move_up_btn);
      this.m_button_panel.Controls.Add(this.m_add_all_btn);
      this.m_button_panel.Controls.Add(this.m_remove_all_btn);
      this.m_button_panel.Controls.Add(this.m_remove_btn);
      this.m_button_panel.Controls.Add(this.m_add_btn);
      this.m_button_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_button_panel.Location = new System.Drawing.Point(3, 28);
      this.m_button_panel.Name = "m_button_panel";
      this.m_button_panel.Size = new System.Drawing.Size(38, 244);
      this.m_button_panel.TabIndex = 3;
      // 
      // m_move_down_btn
      // 
      this.m_move_down_btn.Location = new System.Drawing.Point(0, 207);
      this.m_move_down_btn.Name = "m_move_down_btn";
      this.m_move_down_btn.Size = new System.Drawing.Size(38, 38);
      this.m_move_down_btn.TabIndex = 5;
      this.m_move_down_btn.Click += new System.EventHandler(this.HandleMoveDown);
      // 
      // m_move_up_btn
      // 
      this.m_move_up_btn.Location = new System.Drawing.Point(0, 169);
      this.m_move_up_btn.Name = "m_move_up_btn";
      this.m_move_up_btn.Size = new System.Drawing.Size(38, 38);
      this.m_move_up_btn.TabIndex = 4;
      this.m_move_up_btn.Click += new System.EventHandler(this.HandleMoveUp);
      // 
      // m_add_all_btn
      // 
      this.m_add_all_btn.Location = new System.Drawing.Point(0, 43);
      this.m_add_all_btn.Name = "m_add_all_btn";
      this.m_add_all_btn.Size = new System.Drawing.Size(38, 38);
      this.m_add_all_btn.TabIndex = 3;
      this.m_add_all_btn.Click += new System.EventHandler(this.HandleAddAll);
      // 
      // m_remove_all_btn
      // 
      this.m_remove_all_btn.Location = new System.Drawing.Point(0, 125);
      this.m_remove_all_btn.Name = "m_remove_all_btn";
      this.m_remove_all_btn.Size = new System.Drawing.Size(38, 38);
      this.m_remove_all_btn.TabIndex = 2;
      this.m_remove_all_btn.Click += new System.EventHandler(this.HandleRemoveAll);
      // 
      // m_remove_btn
      // 
      this.m_remove_btn.Location = new System.Drawing.Point(0, 87);
      this.m_remove_btn.Name = "m_remove_btn";
      this.m_remove_btn.Size = new System.Drawing.Size(38, 38);
      this.m_remove_btn.TabIndex = 1;
      this.m_remove_btn.Click += new System.EventHandler(this.HandleRemove);
      // 
      // m_add_btn
      // 
      this.m_add_btn.Location = new System.Drawing.Point(0, 5);
      this.m_add_btn.Name = "m_add_btn";
      this.m_add_btn.Size = new System.Drawing.Size(38, 38);
      this.m_add_btn.TabIndex = 0;
      this.m_add_btn.Click += new System.EventHandler(this.HandleAdd);
      // 
      // TwoPaneSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_full_table);
      this.MinimumSize = new System.Drawing.Size(450, 307);
      this.Name = "TwoPaneSelector";
      this.Size = new System.Drawing.Size(450, 307);
      this.m_full_table.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_full_list_caption)).EndInit();
      this.m_full_list_caption.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_full_list)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_selected_list_caption)).EndInit();
      this.m_selected_list_caption.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_selected_list)).EndInit();
      this.m_center_table.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_button_panel)).EndInit();
      this.m_button_panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel m_full_table;
    private DevExpress.XtraEditors.GroupControl m_full_list_caption;
    private DevExpress.XtraEditors.ListBoxControl m_full_list;
    private DevExpress.XtraEditors.GroupControl m_selected_list_caption;
    private DevExpress.XtraEditors.ListBoxControl m_selected_list;
    private System.Windows.Forms.TableLayoutPanel m_center_table;
    private DevExpress.XtraEditors.PanelControl m_button_panel;
    private DevExpress.XtraEditors.SimpleButton m_move_down_btn;
    private DevExpress.XtraEditors.SimpleButton m_move_up_btn;
    private DevExpress.XtraEditors.SimpleButton m_add_all_btn;
    private DevExpress.XtraEditors.SimpleButton m_remove_all_btn;
    private DevExpress.XtraEditors.SimpleButton m_remove_btn;
    private DevExpress.XtraEditors.SimpleButton m_add_btn;

  }
}
