using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Dialogs.Wizard
{
  public partial class WizardPage : XtraUserControl, IWizardPage
  {
    public WizardPage()
    {
      InitializeComponent();
    }

    #region „лены IWizardPage

    public virtual bool CanNavigateForwards
    {
      get { throw new NotImplementedException(); }
    }

    public virtual bool CanNavigateBackwards
    {
      get { throw new NotImplementedException(); }
    }

    public virtual bool CanFinish
    {
      get { throw new NotImplementedException(); }
    }

    public virtual string Title
    {
      get { return ""; }
    }

    public Control Control
    {
      get { return this; }
    }

    public virtual bool BeforeNavigateForwards()
    {
      return true;
    }

    public virtual bool BeforeShow()
    {
      return true;
    }

    public virtual void RefreshData()
    {
      
    }

    public virtual bool Finish()
    {
      return true;
    }

    public virtual bool AssignValues()
    {
      throw new System.NotImplementedException();
    }

    public event EventHandler Edited;

    public event EventHandler Shown;

    protected void OnEdited(EventArgs e)
    {
      if (this.Edited != null)
      {
        this.Edited(this, e);
      }
    }

    protected void OnShown(EventArgs e)
    {
      if (this.Shown != null)
      {
        this.Shown(this, e);
      }
    }

    public BindingSource BindingSource
    {
      get
      {
        return m_binding_source;
      }
      set
      {
        m_binding_source = value;
      }
    }


    #endregion

    #region IWizardPage Members




    #endregion
  }
}
