using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestMultiSelect
{
  using AT.Toolbox.Dialogs;


  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      try
      {
        ChoiceForm frm = new ChoiceForm();
        frm.Values.Add( "Test 1", 1 );
        frm.Values.Add("Test 2", 2);
        frm.Values.Add("Test 3", 3);
        frm.Values.Add("Test 4", 4);
        frm.Values.Add("Test 11", 1);
        frm.Values.Add("Test 22", 2);
        frm.Values.Add("Test 33", 3);
        frm.Values.Add("Test 44", 4);
        frm.Values.Add("Test 10", 1);
        frm.Values.Add("Test 20", 2);
        frm.Values.Add("Test 30", 3);
        frm.Values.Add("Test 40", 4);
        frm.Values.Add("Test 19", 1);
        frm.Values.Add("Test 29", 2);
        frm.Values.Add("Test 39", 3);
        frm.Values.Add("Test 49dsduiosydouisyduiysudyosdoysodyuysouyoiyoysoyd", 4);
        frm.AllowMultiSelect = false;
        frm.ShowDialog();
      }
      catch (Exception)
      {


      }

    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      try
      {
        ChoiceForm frm = new ChoiceForm();
        frm.Values.Add("Test 1", 1);
        frm.Values.Add("Test 2", 2);
        frm.Values.Add("Test 3", 3);
        frm.Values.Add("Test 4", 4);
        frm.Values.Add("Test 11", 1);
        frm.Values.Add("Test 22", 2);
        frm.Values.Add("Test 33", 3);
        frm.Values.Add("Test 44", 4);
        frm.Values.Add("Test 10", 1);
        frm.Values.Add("Test 20", 2);
        frm.Values.Add("Test 30", 3);
        frm.Values.Add("Test 40", 4);
        frm.Values.Add("Test 19", 1);
        frm.Values.Add("Test 29", 2);
        frm.Values.Add("Test 39", 3);
        frm.Values.Add("Test 49dsduiosydouisyduiysudyosdoysodyuysouyoiyoysoyd", 4);
        
        frm.AllowMultiSelect = true;
        frm.ShowDialog();
      }
      catch( Exception )
      {
        
        
      }
    }
  }
}
