using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using AT.Toolbox.Extensions;
using AT.Toolbox.Files;

using DevExpress.XtraEditors;


namespace HelpMapCreator
{
  public partial class Form1 : XtraForm
  {
    private static readonly log4net.ILog Log2 = log4net.LogManager.GetLogger("Form1");

    public Form1()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      List<Assembly> asses = new List<Assembly>();

      foreach (string path in FSUtils.FindFiles(buttonEdit1.EditValue.ToString(  ), @".\.(exe|dll)$", true)) 
      {
        try
        {
        Assembly assembly = Assembly.LoadFile( path );
        
        if (!assembly.IsAtToolbox() )
          continue;
        
        asses.Add(  assembly );
        }
        catch( Exception ex)
        {
          Log2.Error("simpleButton1_Click(): exception", ex);
        }
      }

      List<Type> PossibleHelpTypes = new List<Type>();
      
      foreach( Assembly assembly in asses )
      {
        try
        {
          int b = 10;
          foreach( Type t in assembly.GetTypes( ) )
          {
            if( null == t.BaseType )
              continue;

            if (t.BaseType.Name == "LocalizableForm" || t.BaseType.Name == "LocalizableUserControl" || t.BaseType.Name == "PropertyPage")
            {
              PossibleHelpTypes.Add( t );
            }
          }
        }
        catch (Exception ex)
        {
          Log2.Error("simpleButton1_Click(): exception 2", ex);
        }
      }

      int a = 10;
    }

    private void buttonEdit1_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
      FolderBrowserDialog ofd = new FolderBrowserDialog();

      if( DialogResult.OK != ofd.ShowDialog(  ) )
        return;

      buttonEdit1.EditValue = ofd.SelectedPath;
    }
  }
}
