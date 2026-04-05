using System;
using System.ComponentModel;
using Toolbox.Application;

namespace Toolbox.GUI.Base
{
  [DefaultEvent("LanguageChanged")]
  public partial class LanguageSwitcher : Component, ISwitchableLanguage, ISupportInitialize
  {
    public event EventHandler LanguageChanged;

    public LanguageSwitcher()
    {
    }

    public LanguageSwitcher(IContainer container)
    {
      container.Add(this);
    }

    #region ISwitchableLanguage Members

    public void InitLocalization()
    {
      AppSwitchablePool.RegisterSwitchable(this);
      PerformLocalization(this, EventArgs.Empty);
    }

    public void PerformLocalization(object sender, EventArgs e)
    {
      if (this.LanguageChanged != null)
      {
        this.LanguageChanged(sender, e);
      }
    }

    public event EventHandler Terminating;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Terminating != null)
      {
        this.Terminating(this, System.EventArgs.Empty);
      }
      base.Dispose(disposing);
    }

    #endregion

    #region ISupportInitialize Members

    void ISupportInitialize.BeginInit()
    {
    }

    void ISupportInitialize.EndInit()
    {
      this.InitLocalization();
    }

    #endregion
  }
}
