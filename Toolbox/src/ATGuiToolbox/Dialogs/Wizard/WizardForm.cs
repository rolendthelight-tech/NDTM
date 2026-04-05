using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Dialogs
{
  public partial class WizardForm : XtraForm
  {
    protected int m_page_index = 0;

    public BindingSource Binding
    {
      get
      {
        return m_binding; 
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public List<IWizardPage> Pages{get; protected set;}

    public WizardForm()
    {
      Pages = new List<IWizardPage>();
      InitializeComponent();
    }

    private void WizardForm_Shown(object sender, EventArgs e)
    {
      foreach (IWizardPage page in Pages)
      {
        page.Edited +=new EventHandler(HandlePageEdited);
        page.Shown  += new EventHandler(HandlePageShown);
      }

      if( Pages.Count > 0 )
        ShowPage( 0 );
    }

    private void HandlePageShown(object sender, EventArgs e)
    {
      UpdateButtons(m_page_index);
    }

    private void HandlePageEdited(object sender, EventArgs e)
    {
      
    }

    protected void UpdateButtons( int i  )
    {
      IWizardPage page = Pages[i];

      if (i == 0)
        m_back_button.Enabled = false;
      else
        m_back_button.Enabled = page.CanNavigateBackwards;

      m_fwd_button.Enabled = page.CanNavigateForwards;

      if (Pages.Count - 1 == i)
        m_fwd_button.Enabled = false;

      m_finish_btn.Enabled = page.CanFinish;
    }

    public void ShowPage( int i  )
    {
      if( Pages.Count <= i || i < 0)
        return;

      IWizardPage page = Pages[i];
      m_page_index = i;
      m_page_pane.Controls.Clear(); 
      m_page_pane.Controls.Add( page.Control );
      page.RefreshData();
      page.Control.Dock = DockStyle.Fill;
      labelControl2.Text = page.Title;
      this.UpdateButtons(i);
    }

    private void HandleForwards(object sender, EventArgs e)
    {
      if (m_page_index >= this.Pages.Count - 1)
        return;

      if (Pages[m_page_index].AssignValues())
      {
        m_page_index++;
        ShowPage(m_page_index);
      }
    }

    private void m_back_button_Click(object sender, EventArgs e)
    {
      if (m_page_index == 0)
        return;

      m_page_index--;
      this.ShowPage(m_page_index);
    }

    private void simpleButton4_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void m_finish_btn_Click(object sender, EventArgs e)
    {
      if (Pages[m_page_index].Finish())
      {
        DialogResult = DialogResult.OK;
        Close();
      }
    }
  }

  public interface IWizardPage
  {
    bool CanNavigateForwards { get; }
    bool CanNavigateBackwards { get; }
    bool CanFinish { get; }
    Control Control { get; }

    bool BeforeNavigateForwards();
    bool BeforeShow();

    event EventHandler Edited;
    event EventHandler Shown;

    BindingSource BindingSource { get; set; }
    string Title { get; }
    bool AssignValues();
    void RefreshData();
    bool Finish();
  }
}