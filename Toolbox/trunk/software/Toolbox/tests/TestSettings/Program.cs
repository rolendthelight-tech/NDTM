using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Help;
using AT.Toolbox.Misc;

namespace TestSettings
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      try
      {
        AppInstance.Preferences.ConfigFilePath = ".\\Test.xml";
        AppInstance.Preferences.AllowOnlyOneInstance = true;
        AppInstance.MainForm = new Form1();

        if (AppInstance.Init(null))
        {
          AppInstance.Run();
        }
      }
      catch (Exception ex)
      {

      }
      finally
      {
        AppInstance.Terminate();
      }
    }
  }
}
