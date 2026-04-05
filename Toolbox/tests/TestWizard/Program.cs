using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TestWizard
{
  using AT.Toolbox;
  using AT.Toolbox.Dialogs;

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
        AppInstance.MainForm = new Form1();

        while (!AppInstance.Init(null))
        {
          MessageBoxEx box = new MessageBoxEx();

          box.Caption = "Initialization failure";
          box.StandardIcon = MessageBoxEx.Icons.Stop;
          box.Message = "Failed to init Application instance.\nWould You like to check settings ?";
          box.Buttons = MessageBoxButtons.YesNo;
          box.StartPosition = FormStartPosition.CenterScreen;
          box.TopMost = true;

          if (DialogResult.Yes == box.ShowDialog())
          {
            SettingsForm frm = new SettingsForm();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
          }
          else
            return;
        }

        AppInstance.Run();

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
