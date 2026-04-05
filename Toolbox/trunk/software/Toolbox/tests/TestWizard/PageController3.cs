using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestWizard
{
  using System.Windows.Forms;
  using AT.Toolbox.Dialogs;

  class PageController3 : IWizardPage 
  {
     private Page3 pg = new Page3();

    public PageController3()
    {
      pg.Load += new EventHandler(pg_Load);
      Shown += delegate { };
    }

    private void pg_Load(object sender, EventArgs e)
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
      get { return pg; }
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
