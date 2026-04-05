using System;
using System.Collections.Generic;
using AT.Toolbox.Constants;
using AT.Toolbox.Network;
using DevExpress.XtraEditors;

namespace TestNetworkListers
{
  public partial class Form1 : XtraForm
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      List<string> strings =  NetLister.GetNetworkComputers();

      listBoxControl1.Items.Clear();

      foreach (string s in strings)
        listBoxControl1.Items.Add(s);
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      List<string> strings = NetLister.GetSQLServers();

      listBoxControl1.Items.Clear();

      foreach (string s in strings)
        listBoxControl1.Items.Add(s);
    }

    private void simpleButton3_Click(object sender, EventArgs e)
    {
      List<string> strings = NetLister.GetNetworkComputers(ComputerTypes.Server);

      listBoxControl1.Items.Clear();

      foreach (string s in strings)
        listBoxControl1.Items.Add(s);
    }

    private void simpleButton4_Click(object sender, EventArgs e)
    {
      List<string> strings = NetLister.GetNetworkComputers(ComputerTypes.Sqlserver);

      listBoxControl1.Items.Clear();

      foreach (string s in strings)
        listBoxControl1.Items.Add(s);
    }

    private void simpleButton5_Click(object sender, EventArgs e)
    {
      List<string> strings = NetLister.GetNetworkComputers(ComputerTypes.DomainController);

      listBoxControl1.Items.Clear();

      foreach (string s in strings)
        listBoxControl1.Items.Add(s);
    }
  }
}
