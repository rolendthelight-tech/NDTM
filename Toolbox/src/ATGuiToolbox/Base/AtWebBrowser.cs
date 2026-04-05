using System;
using System.ComponentModel;
using System.Windows.Forms;
using AT.Toolbox.Constants;
using AT.Toolbox.Log;

namespace AT.Toolbox.Base
{
  public partial class AtWebBrowser : WebBrowser
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AtWebBrowser).Name);

    private bool m_editable;

    public AtWebBrowser()
    {
      InitializeComponent();
      
      ContentChanged += delegate { };
    }

    [Category("Key")]
    [Browsable(true)]
    public new event KeyEventHandler KeyDown;

    public override bool PreProcessMessage(ref Message msg)
    {
      if (msg.Msg == (int) MessageCodes.WmChar)
      {
        if (Editable)
          ContentChanged(this, EventArgs.Empty);
      }
      if (msg.Msg == (int)MessageCodes.WmKeyDown)
      {
        if (this.KeyDown != null)
        {
          try
          {
            int keyCode = (int)msg.WParam;
            Keys key = (Keys)keyCode;
            KeyEventArgs args = new KeyEventArgs(key);
            this.KeyDown(this, args);
          }
          catch (Exception ex)
          {
            Log.Error("PreProcessMessage(): exception", ex);
          }
        }
      }

      return base.PreProcessMessage(ref msg);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Editable
    {
      get { return m_editable; }
      set
      {
        m_editable = value;

        if (null == DomDocument)
          return;

        DomDocument.designMode = m_editable ? "On" : "Off";
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public mshtml.IHTMLDocument2 DomDocument
    {
      get
      {
        if (null == Document)
          Navigate("about:blank");

        if (null == Document)
          return null;

        mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2) Document.DomDocument;

        return doc;
      }
    }

    public event EventHandler ContentChanged;
  }
}