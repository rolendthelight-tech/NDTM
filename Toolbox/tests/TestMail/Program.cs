using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AT.Toolbox;

namespace TestMail
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      if (!AppInstance.Init(args)) 
        return;

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
