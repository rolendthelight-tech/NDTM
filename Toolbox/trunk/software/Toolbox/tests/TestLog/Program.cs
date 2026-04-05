using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using AT.Toolbox;
using AT.Toolbox.Files;
using AT.Toolbox.Log;
using AT.Toolbox.Misc;
using AT.Toolbox.Settings;

namespace TestLog
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        AppWrapper.Run<Form1>(args, null, true);
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
