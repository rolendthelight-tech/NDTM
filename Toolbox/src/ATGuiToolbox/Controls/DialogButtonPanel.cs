using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AT.Toolbox.Properties;
using AT.Toolbox.Base;
using DevExpress.Data;

namespace AT.Toolbox.Controls
{
  public partial class DialogButtonPanel : LocalizableUserControl
  {
    private Form m_form;
    private readonly List<SimpleButton> m_visible_buttons = new List<SimpleButton>();
    private readonly SimpleButton[] m_buttons;
    private MessageBoxButtons m_buttons_enum = MessageBoxButtons.OK;
    private bool m_reset_required;
    private MessageBoxDefaultButton m_default_button;
    
    public DialogButtonPanel()
    {
      InitializeComponent();

      m_buttons = new[]
      {
        m_ok_btn,
        m_cancel_btn,
        m_yes_btn,
        m_no_btn,
        m_abort_btn,
        m_retry_btn,
        m_ignore_btn
      };

      this.SetButtonVisibility();
    }

    #region Public properties

    [Category("Behavior")]
    [DefaultValue(MessageBoxButtons.OK)]
    public MessageBoxButtons Buttons
    {
      get { return m_buttons_enum; }
      set
      {
        m_buttons_enum = value;
        SetButtonVisibility();
      }
    }

    [Category("Behavior")]
    [DefaultValue(MessageBoxDefaultButton.Button1)]
    public MessageBoxDefaultButton DefaultButton
    {
      get { return m_default_button; }
      set
      {
        m_default_button = value;
        this.SetDefaultButton();
      }
    }

    public void EnableButton(DialogResult btn, bool en)
    {
      foreach (var btn_m in m_buttons)
      {
        if (btn_m.DialogResult == btn)
          btn_m.Enabled = en;
      }
    }
    #endregion

    #region Handlers

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        m_form = this.FindForm();
      }
      catch { }
      if (m_reset_required)
      {
        this.SetButtonVisibility();
      }
      base.OnLoad(e);
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      m_no_btn.Text = Resources.NO;
      m_ok_btn.Text = Resources.OK;
      m_cancel_btn.Text = Resources.CANCEL;
      m_yes_btn.Text = Resources.YES;
      m_abort_btn.Text = Resources.ABORT;
      m_retry_btn.Text = Resources.RETRY;
      m_ignore_btn.Text = Resources.IGNORE;
    }

    #endregion

    #region Implementation

    private void SetButtonVisibility()
    {
      lock (m_buttons)
      {
        this.SuspendLayout();

        foreach (var btn in m_buttons)
        {
          btn.Visible = false;
        }
        m_visible_buttons.Clear();

        switch (m_buttons_enum)
        {
          case MessageBoxButtons.OK:
            m_visible_buttons.Add(m_ok_btn);
            this.SetupFormButtons(m_ok_btn, null);
            break;

          case MessageBoxButtons.OKCancel:
            m_visible_buttons.AddRange(new[] { m_ok_btn, m_cancel_btn });
            this.SetupFormButtons(m_ok_btn, m_cancel_btn);
            break;

          case MessageBoxButtons.RetryCancel:
            m_visible_buttons.AddRange(new[] { m_retry_btn, m_cancel_btn });
            this.SetupFormButtons(m_retry_btn, m_cancel_btn);
            break;

          case MessageBoxButtons.YesNo:
            m_visible_buttons.AddRange(new[] { m_yes_btn, m_no_btn });
            this.SetupFormButtons(m_yes_btn, m_no_btn);
            break;

          case MessageBoxButtons.YesNoCancel:
            m_visible_buttons.AddRange(new[] { m_yes_btn, m_no_btn, m_cancel_btn });
            this.SetupFormButtons(m_yes_btn, m_cancel_btn);
            break;

          case MessageBoxButtons.AbortRetryIgnore:
            m_visible_buttons.AddRange(new[] { m_abort_btn, m_retry_btn, m_ignore_btn });
            this.SetupFormButtons(m_ignore_btn, m_abort_btn);
            break;
        }

        foreach (var btn in m_visible_buttons)
          btn.Visible = true;

        this.SetDefaultButton();

        this.ResumeLayout();
        this.PerformLayout();
      }
    }

    private void SetupFormButtons(SimpleButton accept, SimpleButton cancel)
    {
      if (m_form != null)
      {
        m_form.AcceptButton = accept;
        m_form.CancelButton = cancel;
      }
      else
        m_reset_required = true;
    }

    private void SetDefaultButton()
    {
      if (m_default_button == MessageBoxDefaultButton.Button1
        && m_visible_buttons.Count > 0)
        m_visible_buttons[0].Focus();

      else if (m_default_button == MessageBoxDefaultButton.Button2
        && m_visible_buttons.Count > 1)
        m_visible_buttons[1].Focus();

      else if (m_default_button == MessageBoxDefaultButton.Button3
        && m_visible_buttons.Count > 2)
        m_visible_buttons[2].Focus();
    }

    #endregion
  }
}
