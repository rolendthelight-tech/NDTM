using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Help
{
  public partial class HelpBrowser : XtraUserControl
  {
    public HelpBrowser()
    {
      InitializeComponent();
      OnHelpNavigate += delegate { };
    }

    public void Open(string fileName)
    {
      webBrowser1.Navigate(fileName);
    }

    private void HandleNavigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      if (! e.Url.AbsoluteUri.StartsWith("athelp://"))
        return;

      e.Cancel = true;


      OnHelpNavigate(this, new ParamEventArgs<string>(e.Url.AbsoluteUri.Replace("athelp://", "")));
    }

    public event EventHandler<ParamEventArgs<string>> OnHelpNavigate;
  }
}