using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Base
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
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
