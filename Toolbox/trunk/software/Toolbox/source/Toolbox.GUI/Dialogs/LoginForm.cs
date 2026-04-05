using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Log;

namespace Toolbox.GUI.Dialogs
{
  public partial class LoginForm : Form
  {
    public LoginForm()
    {
      InitializeComponent();
    }

    public string Login
    {
      get { return textLogin.Text; }
      set { textLogin.Text = value; }
    }

    public string Password
    {
      get { return textPassword.Text; }
      set { textPassword.Text = value; }
    }

    public bool SaveCredentials
    {
      get { return checkBoxSave.Checked; }
      set { checkBoxSave.Checked = value; }
    }

    protected override void OnFormClosing([NotNull] FormClosingEventArgs e)
    {
	    if (e == null) throw new ArgumentNullException("e");

	    base.OnFormClosing(e);

			if (this.DialogResult == global::System.Windows.Forms.DialogResult.OK)
      {
        var buffer = new InfoBuffer();

        if (string.IsNullOrEmpty(this.Login))
          buffer.Add("Login not supplied", InfoLevel.Warning);

        //if (string.IsNullOrEmpty(this.Password))
        //  buffer.Add("Password not supplied", InfoLevel.Warning);

        if (buffer.Count > 0)
        {
          AppManager.Notificator.ShowBuffer(buffer);
          e.Cancel = true;
        }
      }
    }
  }
}
