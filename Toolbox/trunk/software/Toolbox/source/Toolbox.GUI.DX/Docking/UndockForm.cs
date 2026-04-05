using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using Toolbox.GUI.DX.Base;

namespace Toolbox.GUI.DX.Docking
{
  public partial class UndockForm : LocalizableForm 
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(UndockForm));

    DockPanel m_Pane;

    public bool InitSuccessfull = false;

    public DockPanel Pane
    {
      get
      {
        return m_Pane;
      }
      set
      {
        m_Pane = value;

        try
        {
          foreach( Control ctl in m_Pane.Controls )
          {
            if( !( ctl is ControlContainer ) )
              continue;

            ControlContainer cont = ctl as ControlContainer;

            if( 0 == cont.Controls.Count )
              continue;

            Control inner_control = cont.Controls[0];

            cont.Controls.Remove( inner_control );
            Controls.Add( inner_control );
            inner_control.Dock = DockStyle.Fill;
          }

          Text = m_Pane.Text;
          Location = m_Pane.FloatLocation;
          Size = m_Pane.FloatSize;

          m_Pane.FloatForm.Visible = false;
          InitSuccessfull = true;

          try
          {
            ImageList list = m_Pane.Images as ImageList;

            if (null == list)
              return;

            Bitmap bmp = list.Images[m_Pane.ImageIndex] as Bitmap;

            if (null == bmp)
              return;

            Icon = Icon.FromHandle(bmp.GetHicon());
          }
          catch (Exception ex)
          {
            Log.Error("Pane.set(): exception", ex);
          }
        }
        catch (Exception ex)
        {
          Log.Error("Pane.set(): exception 2", ex);
          Close();
        }
      }
    }

    public DockingStyle PostDockStyle = DockingStyle.Float;

    public UndockForm()
    {
      InitializeComponent();
    }

    private void UndockForm_Shown(object sender, EventArgs e)
    {

    }

    private void UndockForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      m_Pane.FloatSize = Size ;
      m_Pane.FloatLocation = Location;

      foreach (Control ctl in m_Pane.Controls)
      {
        if (!(ctl is ControlContainer))
          continue;

        ControlContainer cont = ctl as ControlContainer;

        Control inner_control = Controls[0];
        Controls.Remove(inner_control);
        cont.Controls.Add(inner_control);
        inner_control.Dock = DockStyle.Fill;
      }
    }

    private void UndockForm_Load(object sender, EventArgs e)
    {

    }

    private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      PostDockStyle = DockingStyle.Left;
      Close(  );
    }

    private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      PostDockStyle = DockingStyle.Right;
      Close();
    }

    private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      PostDockStyle = DockingStyle.Top;
      Close();
    }

    private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
      PostDockStyle = DockingStyle.Bottom;
      Close();
    }
  }
}