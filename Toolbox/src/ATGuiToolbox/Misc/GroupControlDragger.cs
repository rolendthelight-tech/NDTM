using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Dialogs
{
  public partial class GroupControlDragger : Component
  {
    private GroupControl m_ctl;
    private bool m_tops;
    private Point m_last_pos;
    private int m_size_bounds = 3;

    public GroupControlDragger()
    {
      InitializeComponent();
    }

    public GroupControlDragger(IContainer Container)
    {
      Container.Add(this);

      InitializeComponent();
    }

    public GroupControl GroupControlToDragBy
    {
      get
      {
        return m_ctl;
      }
      set
      {
        m_ctl = value;

        if (null == m_ctl)
          return;

        m_ctl.MouseDown += HandleMouseDown;
        m_ctl.MouseUp += HandleMouseUp;
        m_ctl.MouseMove += HandleMouseMove;
      }
    }

    public bool CanMove { get; set; }

    public bool CanSize { get; set; }

    public int SizeBounds
    {
      get { return m_size_bounds; }
      set { m_size_bounds = value; }
    }

    private void HandleMouseMove(object Sender, MouseEventArgs E)
    {
      if (Cursors.Hand == Cursor.Current)
      {
        m_ctl.Parent.Location = new Point(m_ctl.Parent.Location.X + (E.X - m_last_pos.X), m_ctl.Parent.Location.Y + (E.Y - m_last_pos.Y));
        return;
      }

      if (Cursors.SizeNS == Cursor.Current)
      {
        if (m_tops)
        {
          m_ctl.Parent.Location = new Point(m_ctl.Parent.Location.X, m_ctl.Parent.Location.Y + (E.Y - m_last_pos.Y));
          m_ctl.Parent.Height -= E.Y - m_last_pos.Y;
        }
        else
        {
          m_ctl.Parent.Height += E.Y - m_last_pos.Y;
          m_last_pos = E.Location;
        }
        return;
      }

      if (Cursors.SizeWE == Cursor.Current)
      {
        if (m_tops)
        {
          m_ctl.Parent.Location = new Point(m_ctl.Parent.Location.X + (E.X - m_last_pos.X), m_ctl.Parent.Location.Y);
          m_ctl.Parent.Width -= E.X - m_last_pos.X;
        }
        else
        {
          m_ctl.Parent.Width += E.X - m_last_pos.X;
          m_last_pos = E.Location;
        }
        return;
      }
    }

    private void HandleMouseUp(object Sender, MouseEventArgs E)
    {
      Cursor.Current = Cursors.Default;
    }

    private void HandleMouseDown(object Sender, MouseEventArgs E)
    {
      if (null != m_ctl.Parent)
        m_ctl.Parent.BringToFront();

      if( E.Button != MouseButtons.Left)
        return;

      if (CanSize)
      {
        if (E.Y < SizeBounds || E.Y > m_ctl.Parent.Height - SizeBounds)
        {
          m_tops = (E.Y < SizeBounds);
          Cursor.Current = Cursors.SizeNS;
          m_last_pos = E.Location;
          return;
        }

        if (E.X < SizeBounds || E.X > m_ctl.Parent.Width - SizeBounds)
        {
          m_tops = (E.X < SizeBounds);
          Cursor.Current = Cursors.SizeWE;
          m_last_pos = E.Location;
          return;
        }
      }

      if (CanMove)
      {
        if (E.Y < m_ctl.Height - m_ctl.DisplayRectangle.Height)
        {
          Cursor.Current = Cursors.Hand;
          m_last_pos = E.Location;
          return;
        }
      }
    }
  }
}
