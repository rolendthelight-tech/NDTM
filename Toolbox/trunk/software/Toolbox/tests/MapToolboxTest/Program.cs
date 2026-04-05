using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AT.Toolbox;

namespace MapToolboxTest
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      AppWrapper.Run<MapForm1>(true);
    }
  }
}