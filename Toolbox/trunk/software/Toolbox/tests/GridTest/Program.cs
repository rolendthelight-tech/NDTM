using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GridTest
{
  using AT.Toolbox;

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
        AppWrapper.Run<Form1>(args, null , true);
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
