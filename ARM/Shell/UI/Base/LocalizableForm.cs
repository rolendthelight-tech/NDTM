namespace AT.Toolbox.Base
{
  using System;
  using System.Windows.Forms;

  /// <summary>
  /// Base form class that provides basic event handling
  /// </summary>
  partial class LocalizableForm : Form
  {
    // Store cursor safely - use Default instead of Current which may be null
    protected Cursor m_cur = Cursors.Default;

    /// <summary>
    /// Initializes a new instance of the LocalizableForm class
    /// </summary>
    public LocalizableForm()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Handles the HelpRequested event
    /// </summary>
    private void LocalizableForm_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      hlpevent.Handled = false;
    }

    /// <summary>
    /// Override OnLoad to safely initialize form without triggering premature events
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);
        // Restore cursor safely
        if (m_cur != null)
        {
          this.Cursor = m_cur;
        }
      }
      catch (NullReferenceException)
      {
        // Suppress NullReferenceException during form initialization
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"LocalizableForm.OnLoad exception: {ex.Message}");
      }
    }

    /// <summary>
    /// Override OnSizeChanged to safely handle resize events
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
      try
      {
        base.OnSizeChanged(e);
      }
      catch (NullReferenceException)
      {
        // Suppress NullReferenceException during form resize
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"LocalizableForm.OnSizeChanged exception: {ex.Message}");
      }
    }

    /// <summary>
    /// Override OnClosed to safely handle any exceptions that may occur in parent class event handlers
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
      try
      {
        base.OnClosed(e);
      }
      catch (NullReferenceException)
      {
        // Suppress NullReferenceException that can occur when parent class event handlers reference null events
      }
    }

    /// <summary>
    /// Override OnFormClosed to safely handle any exceptions that may occur in parent class event handlers
    /// </summary>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      try
      {
        base.OnFormClosed(e);
      }
      catch (NullReferenceException)
      {
        // Suppress NullReferenceException that can occur when parent class event handlers reference null events
      }
    }
  }
}
