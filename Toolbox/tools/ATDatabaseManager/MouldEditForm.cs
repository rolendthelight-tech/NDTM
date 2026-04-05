using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.MSSQL.DbMould;
using AT.Toolbox.Base;
using ATDatabaseManager.Properties;
using System.IO;
using AT.Toolbox.Dialogs;
using System.Collections;
using AT.Toolbox.MSSQL;

namespace ATDatabaseManager
{
  public partial class MouldEditForm : LocalizableForm
  {
    public MouldEditForm()
    {
      InitializeComponent();
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      this.Text = Resources.EditMould;
      m_label_title.Text = Resources.MouldActions;
      m_check_replace.Text = Resources.ReplaceCode;
      m_label_generate_path.Text = Resources.GeneratePath;
      m_label_save_path.Text = Resources.SavePath;
      m_btn_test_fd.Text = Resources.TestFD;
      m_btn_generate.Text = Resources.Generate;
      m_btn_save.Text = Resources.Save;
      colCaption.Caption = Resources.Caption;
      colColumnName.Caption = Resources.ColumnName;
      colEditableOnCreate.Caption = Resources.EditableOnCreate;
      colReadOnly.Caption = Resources.ReadOnly;
      colTableName.Caption = Resources.TableName;
      colVisible.Caption = Resources.Visible;
      colCategory.Caption = Resources.Category;
    }

    public DatabaseMould Mould { get; set; }

    public FieldDescriptionContainer FieldDescriptions { get; set; }

    public string MouldFilePath
    {
      get { return m_save_path_edit.Text; }
      set { m_save_path_edit.Text = value; }
    }

    private void m_bindable_tree_AfterSelect(object sender, TreeViewEventArgs e)
    {
      m_property_grid.SelectedObject = e.Node.Tag;
      m_property_grid.RetrieveFields();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (this.Mould != null)
      {
        m_bindable_tree.BindObject(this.Mould).Expand();
        
        if (File.Exists(m_save_path_edit.Text + ".fd"))
        {
          try
          {
            this.FieldDescriptions = FieldDescriptionContainer.LoadFromFile(m_save_path_edit.Text + ".fd");
            this.FieldDescriptions.Mould = this.Mould;
          }
          catch { }
        }
        if (this.FieldDescriptions == null)
        {
          this.FieldDescriptions = new FieldDescriptionContainer();
          this.FieldDescriptions.Mould = this.Mould;
        }
        m_binding_source.DataSource = this.FieldDescriptions;
        
        List<string> tableNames = new List<string>();
        foreach (TableMould table in this.Mould.Tables)
        {
          tableNames.Add(table.Name);
        }
        foreach (ViewMould view in this.Mould.Views)
        {
          tableNames.Add(view.Name);
        }
        tableNames.Sort();

        m_table_edit.Items.AddRange(tableNames);

        List<string> columnNames = new List<string>();
        Dictionary<string, object> dic = new Dictionary<string, object>();

        foreach (TableMould table in this.Mould.Tables)
        {
          foreach (ColumnMould column in table.Columns)
          {
            dic[column.Name] = null;
          }
        }

        foreach (ViewMould view in this.Mould.Views)
        {
          foreach (ColumnMould column in view.Columns)
          {
            dic[column.Name] = null;
          }
        }

        foreach (string columnName in dic.Keys)
        {
          columnNames.Add(columnName);
        }
        m_column_edit.Items.AddRange(columnNames);
      }
      else
      {
        m_btn_save.Enabled = false;
        m_btn_generate.Enabled = false;
      }
    }

    private void m_generate_path_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      FolderBrowserDialog dlg = new FolderBrowserDialog();
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_generate_path_edit.Text = dlg.SelectedPath;
      }
    }

    private void m_save_path_edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      this.SelectSavePath();
    }

    private void SelectSavePath()
    {
      SaveFileDialog dlg = new SaveFileDialog();
      dlg.Filter = "Xml files|*.xml";

      if (!string.IsNullOrEmpty(this.MouldFilePath))
      {
        dlg.InitialDirectory = Path.GetDirectoryName(this.MouldFilePath);
      }

      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_save_path_edit.Text = dlg.FileName;
      }
    }

    private void m_btn_save_Click(object sender, EventArgs e)
    {
      while (string.IsNullOrEmpty(m_save_path_edit.Text))
      {
        MessageBoxEx mb = new MessageBoxEx(Resources.Warning, Resources.EmptyPath, MessageBoxButtons.OK, MessageBoxEx.Icons.Warning);
        mb.ShowDialog(this);
        
        this.SelectSavePath();
      }
      this.Mould.Save(m_save_path_edit.Text);
      this.FieldDescriptions.Save(m_save_path_edit.Text + ".fd");
    }

    private void m_btn_generate_Click(object sender, EventArgs e)
    {
      while (string.IsNullOrEmpty(m_generate_path_edit.Text))
      {
        MessageBoxEx mb = new MessageBoxEx(Resources.Warning, Resources.EmptyPath, MessageBoxButtons.OK, MessageBoxEx.Icons.Warning);
        mb.ShowDialog(this);

        FolderBrowserDialog dlg = new FolderBrowserDialog();
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
          m_generate_path_edit.Text = dlg.SelectedPath;
        }
        else
        {
          return;
        }
      }
      MouldCsGenerator generator = new MouldCsGenerator(this.Mould);
      generator.DestinationNamespace = this.Mould.MappingName;
      generator.Path = m_generate_path_edit.Text;
      generator.ReplaceCustomizableCode = m_check_replace.Checked;
      generator.Generate(true);
    }

    private void m_property_grid_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e)
    {
      try
      {
        if (m_property_grid.SelectedObject != null && m_property_grid.SelectedObject as IList == null)
        {
          m_bindable_tree.SelectedNode.Text = m_property_grid.SelectedObject.ToString();
        }
      }
      catch { }
    }

    private void m_btn_test_fd_Click(object sender, EventArgs e)
    {
      InfoEventArgs args = new InfoEventArgs();
      if (this.Mould != null)
      {
        foreach (TableMould table in this.Mould.Tables)
        {
          foreach (ColumnMould column in table.Columns)
          {
            FieldDescription foundDescription = this.FieldDescriptions.FindFieldDescription(table.Name, column.Name);

            if (foundDescription == null || string.IsNullOrEmpty(foundDescription.ColumnName))
            {
              args.Messages.Add(new Info(string.Format(Resources.FD_NOT_FOUND, table.Name, column.Name), InfoLevel.Error));
            }
            /*else if (string.IsNullOrEmpty(foundDescription.TableName))
            {
              args.Messages.Add(new Info(string.Format(Resources.FD_ONLY_DEFALUT, table.Name, column.Name), InfoLevel.Warning));
            }*/
          }
        }

        foreach (ViewMould view in this.Mould.Views)
        {
          foreach (ColumnMould column in view.Columns)
          {
            FieldDescription foundDescription = this.FieldDescriptions.FindFieldDescription(view.Name, column.Name);

            if (foundDescription == null || string.IsNullOrEmpty(foundDescription.ColumnName))
            {
              args.Messages.Add(new Info(string.Format(Resources.FD_NOT_FOUND, view.Name, column.Name), InfoLevel.Error));
            }
            /*else if (string.IsNullOrEmpty(foundDescription.TableName))
            {
              args.Messages.Add(new Info(string.Format(Resources.FD_ONLY_DEFALUT, view.Name, column.Name), InfoLevel.Warning));
            }*/
          }
        }

        MemoEditForm mf = new MemoEditForm();

        mf.Value = string.Join(Environment.NewLine,
          args.Messages.ConvertAll<string>((p) => p.InfoLevel.ToString().ToUpper() + ": " + p.Message).ToArray());
        mf.Text = Resources.TestFD;
        mf.Show(this.ParentForm);
      }
    }

    private void MouldEditForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.S)
      {
        m_btn_save_Click(this, EventArgs.Empty);
      }
    }
  }
}
