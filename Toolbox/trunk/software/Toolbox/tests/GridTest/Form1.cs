using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AT.Toolbox.Base;

namespace GridTest
{
  using AT.Toolbox.Dialogs;

  public partial class Form1 : LocalizableForm 
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
      //this.testTableAdapter.Fill(this.testDataSet.Test);
    }

    private void gridControl1_Validated(object sender, EventArgs e)
    {
      //this.testTableAdapter.Update(this.testDataSet.Test);
    }

    private void testBindingSource_AddingNew(object sender, AddingNewEventArgs e)
    {
      int a = 10;
    }

    private void testBindingSource_ListChanged(object sender, ListChangedEventArgs e)
    {
      uint a = 10;
    }

    private void gridControl1_KeyUp(object sender, KeyEventArgs e)
    {
      if( e.KeyCode == Keys.Delete )
      {
        gridView1.DeleteSelectedRows();
      }
    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      testBindingSource.Undo();
    }

    private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      testBindingSource.Redo();
    }

    private void editToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MultiEditForm frm = new MultiEditForm();
      frm.Grid = this.gridControl1;
      frm.ExceptFields.Add("Test");
      frm.ShowDialog(this);
    }
  }

  public class data
  {
    public int ID{ get; set;}
    public string Test { get; set; }
    public string Test2 { get; set; }
  }
}

