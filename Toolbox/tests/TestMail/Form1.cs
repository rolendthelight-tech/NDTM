using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Log;
using AT.Toolbox.Network;
using Logger = AT.Toolbox.Log.Log;

namespace TestMail
{
  public partial class Form1 : Form
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

    protected readonly ILog m_log = Logger.GetLogger();

    #endregion


    private PopSmtpMessenger<int, int> m_messenger = new PopSmtpMessenger<int, int>();

    public Form1()
    {
      InitializeComponent();
    }

    private void simpleButton1_Click(object sender, EventArgs e)
    {
      m_messenger.Preferences.Username = "serpentthegreen";
      m_messenger.Preferences.Password = "h0ly0verk1ll";
      m_messenger.Preferences.PopServer = "pop.yandex.ru";
      m_messenger.Preferences.PopPort = 995;
      m_messenger.Preferences.PopUseSSL = true;

      m_messenger.Connect();
    }

    private void simpleButton2_Click(object sender, EventArgs e)
    {
      m_messenger.Disconnect();
    }

    private void simpleButton3_Click(object sender, EventArgs e)
    {
      int i1;
      int i2;
      m_messenger.Recieve( out i1, out i2);

     // m_log.Info("Emails : " + i );
    }
  }
}
