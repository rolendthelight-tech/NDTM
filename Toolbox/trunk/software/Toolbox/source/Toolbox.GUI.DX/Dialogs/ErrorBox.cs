using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Configuration;
using Toolbox.Extensions;
using Toolbox.Log;

namespace Toolbox.GUI.DX.Dialogs
{
  public partial class ErrorBox : XtraForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ErrorBox));

    #region Settings ------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Настройки отображения протокола
    /// </summary>
    [DataContract]
    public class Settings : ConfigurationSection
    {
      [DataMember]
      [DefaultValue("support@auditech.ru")]
      public string RemoteAdress { get; set; }

      public override bool Validate([NotNull] InfoBuffer WarningsAndErrors)
      {
	      if (WarningsAndErrors == null) throw new ArgumentNullException("WarningsAndErrors");

				if( string.IsNullOrEmpty( RemoteAdress ))
          WarningsAndErrors.Add("Не задан адрес службы поддержки", InfoLevel.Warning);

        return true;
      }
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    public static Settings Preferences
    {
      get
      {
        return AppManager.Configurator.GetSection<Settings>();
      }
      set { AppManager.Configurator.SaveSection(value); }
    }

    #endregion

    public ErrorBox()
    {
      InitializeComponent();
    }

    public string Title
    {
      get
      {
        return labelControl1.Text;
      }
      set
      {
        labelControl1.Text = value;
      }
    }

    public string Message
    {
      get
      {
        return memoEdit1.Text;
      }
      set
      {
        memoEdit1.Text = value;
      }
    }

    private void simpleButton2_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    private void simpleButton1_Click(object sender, System.EventArgs e)
    {
      try
      {

        string prepared_header = Title.Replace(Environment.NewLine, "%0A");
        prepared_header = prepared_header.Replace("?", "%3F");
        prepared_header = prepared_header.Replace("&", "%26");

        string prepared_message = Message.Replace( Environment.NewLine, "%0A" );
        prepared_message = prepared_message.Replace("?", "%3F");
        prepared_message = prepared_message.Replace("&", "%26");

        Assembly ass = Assembly.GetEntryAssembly( );

        string app_id = ass.ProductName() + " v. " + ass.ProductVersion() + " (" + ass.FileVersion() + ")" ;

        ProcessStartInfo si = new ProcessStartInfo("mailto:" + Preferences.RemoteAdress + "?subject=" + app_id + ": Ошибка : " + prepared_header + "&body=" + prepared_message);
        si.UseShellExecute = true;
        using(Process.Start( si )){}
        Close();
      }
      catch (Exception ex )
      {
        Log.Error("simpleButton1_Click(): exception", ex);
        MessageBoxEx.ShowError(ex.Message, this);
      }
    }

    private void simpleButton3_Click(object sender, EventArgs e)
    {
      try
      {
        Clipboard.Clear( );
        Clipboard.SetText( Title + Environment.NewLine + Environment.NewLine + Message );
        Close( );
      }
      catch( Exception ex )
      {
        Log.Error("simpleButton3_Click(): exception", ex);
        MessageBoxEx.ShowError( ex.Message, this );
      }
    }

    private void ErrorBox_Load(object sender, EventArgs e)
    {
      simpleButton1.Visible = !string.IsNullOrEmpty( Preferences.RemoteAdress );
    }

    private void HandleFormShown(object sender, EventArgs e)
    {
      this.BringToFront(  );
    }
  }
}