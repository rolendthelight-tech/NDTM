using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Base;
using System.Data.SqlClient;
using RegisterDataImport.Properties;
using AT.Toolbox.Dialogs;

namespace AT.Toolbox.MSSQL
{
  public partial class CSVImportForm : LocalizableForm
  {
    SqlConnection m_connection;
    
    public CSVImportForm()
    {
      InitializeComponent();
      m_separator_edit.Text = "\t";
      m_encoding_edit.Properties.Items.AddRange(System.Text.Encoding.GetEncodings().Select((p) => p.Name).ToArray());
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      this.Text = Resources.ImportForm_Title;
      this.m_button_cancel.Text = Resources.Cancel;
      this.colColumnIndex.Caption = Resources.ImportForm_ColumnIndex;
      this.colColumnName.Caption = Resources.ImportForm_ColumnName;
      this.m_label_encoding.Text = Resources.ImportForm_Encoding;
      this.m_label_file.Text = Resources.ImportForm_FileName;
      this.m_label_separator.Text = Resources.ImportForm_Separator;
      this.m_label_table.Text = Resources.ImportForm_Table;
      this.m_cmd_retrieve_default.Text = Resources.ImportForm_RetrieveDeafult;
    }

    private void m_file_edit_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = "Text files (.txt;.csv)|*.txt;*.csv|CSV files (.csv)|*.csv|All files|*.*";
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        m_file_edit.EditValue = dlg.FileName;
      }
    }

    public void SetupConnection(SqlConnection connection)
    {
      m_connection = connection;
      SqlCommand cmd = new SqlCommand("SELECT [name] FROM [sysobjects] WHERE objectproperty([id], \'IsUserTable\') <> 0", connection);
      SqlDataAdapter adapter = new SqlDataAdapter(cmd);
      DataTable tableList = new DataTable();
      adapter.Fill(tableList);

      foreach (DataRow row in tableList.Rows)
      {
        m_table_edit.Properties.Items.Add(row["name"]);
      }
    }

    public string TableName
    {
      get { return (m_table_edit.EditValue ?? "").ToString(); }
    }

    public Dictionary<string, int> ColumnMapping
    {
      get
      {
        Dictionary<string, int> ret = new Dictionary<string, int>();
        foreach (CSVMapperItem item in m_csv_mapper.Items)
        {
          ret.Add(item.ColumnName, item.ColumnIndex);
        }
        return ret;
      }
    }

    public string FileName
    {
      get { return (m_file_edit.EditValue ?? "").ToString(); }
    }

    public string Encoding
    {
      get { return (m_encoding_edit.EditValue ?? "").ToString(); }
      set { m_encoding_edit.EditValue = value; }
    }

    public string Separator
    {
      get { return (m_separator_edit.EditValue ?? "").ToString(); }
    }

    public DataTable GetDataTable()
    {
      DataTable ret = new DataTable(this.TableName);
      SqlCommand cmd = new SqlCommand("SELECT * FROM [" + m_table_edit.Text + "]", m_connection);
      SqlDataAdapter adapter = new SqlDataAdapter(cmd);

      adapter.FillSchema(ret, SchemaType.Source);
      adapter.Fill(ret);

      return ret;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (this.DialogResult == DialogResult.OK)
      {
        List<string> warnings = new List<string>();
        if (this.TableName == "")
        {
          warnings.Add(Resources.ImportForm_NoTable);
        }
        if (m_grid_view.RowCount == 0)
        {
          warnings.Add(Resources.ImportForm_NoColumns);
        }
        else
        {
          try
          {
            Dictionary<string, int> test_data = this.ColumnMapping;
          }
          catch(Exception ex)
          {
            warnings.Add(ex.Message);
          }
        }
        if (this.FileName == "")
        {
          warnings.Add(Resources.ImportForm_NoFile);
        }
        if (this.Encoding == "")
        {
          warnings.Add(Resources.ImportForm_NoEncoding);
        }
        if (this.Separator == "")
        {
          warnings.Add(Resources.ImportForm_NoSeparator);
        }

        if (warnings.Count > 0)
        {
          MessageBoxEx messageBox = new MessageBoxEx();
          messageBox.StandardIcon = MessageBoxEx.Icons.Warning;
          messageBox.Message = string.Join(Environment.NewLine, warnings.ToArray());
          messageBox.Caption = Resources.Warning;
          messageBox.ShowDialog(this);
          e.Cancel = true;
        }
      }
      base.OnFormClosing(e);
    }

    private void m_table_edit_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_column_edit.Items.Clear();
      SqlCommand cmd = new SqlCommand("SELECT * FROM [" + m_table_edit.Text + "]", m_connection);
      DataTable tableHeader = new DataTable();
      SqlDataAdapter adapter = new SqlDataAdapter(cmd);
      adapter.FillSchema(tableHeader, SchemaType.Source);

      foreach (DataColumn column in tableHeader.Columns)
      {
        if (column.AutoIncrement)
          continue;

        m_column_edit.Items.Add(column.ColumnName);
      }
    }

    private void m_separator_edit_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Tab && e.Control)
      {
        m_separator_edit.Text = "\t";
      }
    }

    private void m_cmd_retrieve_default_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(m_table_edit.Text))
        return;

      SqlCommand cmd = new SqlCommand("SELECT * FROM [" + m_table_edit.Text + "]", m_connection);
      DataTable tableHeader = new DataTable();
      SqlDataAdapter adapter = new SqlDataAdapter(cmd);
      adapter.FillSchema(tableHeader, SchemaType.Source);

      m_csv_mapper.Items.Clear();
      for (int i = 0; i < tableHeader.Columns.Count; i++)
      {
        if (tableHeader.Columns[i].AutoIncrement)
          continue;

        CSVMapperItem item = new CSVMapperItem();
        item.ColumnIndex = i;
        item.ColumnName = tableHeader.Columns[i].ColumnName;
        m_csv_mapper.Items.Add(item);
      }
    }
  }
}
