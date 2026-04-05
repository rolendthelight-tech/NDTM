using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using AT.Toolbox.Log;
using DevExpress.XtraEditors.Container;


namespace AT.Toolbox.Base
{
  using Settings;

  [ExcludeFromLoading]
  public partial class PropertyPage : LocalizableUserControl, IPropertyPage
  {
    protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(PropertyPage).Name);

    private bool m_loaded;

    public PropertyPage()
    {
      InitializeComponent();
      
      OnPageSettings = new List<ConfigurationSection>();
    }

    public event EventHandler Changed;

    public virtual string Group { get { return ""; } }

    public virtual string PageName { get { return ""; } }

    public virtual Image Image { get { return Properties.Resources.p_32_document_config; } }

    public Control Page { get { return this; } }

    public virtual bool RestartRequired { get { return false; } }

    public virtual bool ValidateSetting( InfoBuffer Messages )
    {
      if( null != OnPageSettings && OnPageSettings.Count > 0 )
      {
        bool ret_val = true;

        foreach( ConfigurationSection list in OnPageSettings )
        {
          ret_val &= list.Validate(Messages);
        }

        return ret_val;
      }

      return true;
    }

    public virtual bool UIThreadValidation
    {
      get { return true; }
    }

    public virtual void ApplySettings()
    {
      this.OnPageSettings.ForEach(s => s.ApplySettings());
    }

    public virtual void GetCurrentSettings()
    {
    }

    public bool IsSourceOfMessage(Info info)
    {
      return this.OnPageSettings.Any(s => s.ToString() == info.Link);
    }
  
    protected List<ConfigurationSection> OnPageSettings { get; private set; }

    protected void HandleUserChangedValue(object sender, EventArgs e)
    {
      if (this.Changed != null)
        this.Changed(this, e);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (!m_loaded)
      {
        SignToValidated(this);
        m_loaded = true;
      }
    }

    private void SignToValidated( Control ctl )
    {
      foreach (Control control in ctl.Controls)
      {
        SignToValidated( control );
      }

      if (ctl is DevExpress.XtraEditors.BaseEdit)
      {
        ((DevExpress.XtraEditors.BaseEdit)ctl).EditValueChanged += HandleUserChangedValue;
      }
    }
  }
}

