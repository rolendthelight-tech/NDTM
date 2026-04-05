using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;

namespace Toolbox.GUI.DX.Docking
{
  public partial class UndockAsFormController : Component
  {
    public delegate bool CanUndockAsForm( DockPanel Pane );
    protected Dictionary<DockPanel, Form> m_undocked_panes = new Dictionary<DockPanel, Form>();
    public CanUndockAsForm ConfirmUndock;

    public UndockAsFormController()
    {
      InitializeComponent();
    }

    public UndockAsFormController(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    DockManager m_Manager;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public DockManager Manager
    {
      get
      {
        return m_Manager;
      }
      set
      {
        if (ReferenceEquals(m_Manager, value))
          return;

        if (m_Manager != null)
        {
          m_Manager.EndDocking -= HandleManagerEndDocking;
          m_Manager.RegisterDockPanel -= m_Manager_RegisterDockPanel;
          m_Manager.Load -= new EventHandler(m_Manager_Load);
        }

        m_Manager = value;

        if (value != null)
        {
          m_Manager.EndDocking += HandleManagerEndDocking;
          m_Manager.RegisterDockPanel += m_Manager_RegisterDockPanel;
          m_Manager.Load += new EventHandler(m_Manager_Load);
        }
      }
    }

    void m_Manager_Load(object sender, EventArgs e)
    {
      if (m_Manager == null)
        return;

      foreach( DockPanel pane in m_Manager.Panels )
      {
        if (pane.Visibility == DockVisibility.Visible)
        {
          if (pane.Dock == DockingStyle.Float)
          {
            TryToUndockPanelAsForm(pane);
          }
        }
      }
    }

    void m_Manager_RegisterDockPanel(object sender, DockPanelEventArgs e)
    {
      e.Panel.VisibilityChanged += Panel_VisibilityChanged;
    }

    void Panel_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
    {
      if (e.Visibility == DockVisibility.Visible)
      {
        if (e.Panel.Dock == DockingStyle.Float)
        {
          TryToUndockPanelAsForm( e.Panel );
        }
      }
    }

    protected void TryToUndockPanelAsForm( DockPanel pane )
    {
      if (DesignMode)
        return;

      if (null == pane || null == pane.FloatForm)
        return;

      if (null != ConfirmUndock && !ConfirmUndock(pane))
        return;

      if( m_undocked_panes.ContainsKey( pane ))
        return;

      UndockForm frm = new UndockForm();
      m_undocked_panes.Add(pane,frm);
      frm.FormClosed += HandleUndockFormClosed;
      frm.Pane = pane;
      frm.Show();
    }

    void HandleUndockFormClosed(object sender, FormClosedEventArgs e)
    {
      UndockForm frm = sender as UndockForm;

      if( null == frm )
        return;

      if (frm.PostDockStyle != DockingStyle.Float)
      {
        frm.Pane.DockTo(frm.PostDockStyle);
        frm.Pane.Visible = true;
      }
      else 
        frm.Pane.Close();

      m_undocked_panes.Remove( frm.Pane );
    }

    void HandleManagerEndDocking(object sender, EndDockingEventArgs e)
    {
      if( e.Canceled )
        return;

      TryToUndockPanelAsForm( e.Panel );
    }

    public void CloseForm( DockPanel map_panel ) 
    { 
      if( m_undocked_panes.ContainsKey( map_panel ) )
        m_undocked_panes[map_panel].Close(  );
    }
  }
}