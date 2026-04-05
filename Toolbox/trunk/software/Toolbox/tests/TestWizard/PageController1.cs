using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestWizard
{
  using System.Windows.Forms;
  using AT.Toolbox.Dialogs;

  class PageController1 : IWizardPage 
  {
    private Page1 pg = new Page1();

    public PageController1()
    {
      pg.Load += new EventHandler(pg_Load);
      Shown += delegate { };
    }

    void pg_Load(object sender, EventArgs e)
    {
      Shown(this, EventArgs.Empty);
    }

    public bool CanNavigateForwards
    {
      get { return true; }
    }

    public bool CanNavigateBackwards
    {
      get { return true; }
    }

    public bool CanFinish
    {
      get { return true;}
    }

    public Control Control
    {
      get { return new Page1(); }
    }

    public bool BeforeNavigateForwards()
    {
      return true;
    }

    public bool BeforeShow()
    {
      return true;
    }

    public event EventHandler Edited;
    public event EventHandler Shown;

    public BindingSource BindingSource
    {
      get
      {
        return null;
      }
      set
      {

      }
    }
  }
}
