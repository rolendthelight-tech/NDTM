using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Toolbox.GUI.DX.Controls
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
        if (m_ctl != null)
        {
          m_ctl.MouseDown -= HandleMouseDown;
          m_ctl.MouseUp -= HandleMouseUp;
          m_ctl.MouseMove -= HandleMouseMove;
        }

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
      var form = m_ctl.FindForm();

      if (form == null)
        return;

      if (Cursors.Hand == Cursor.Current)
      {
        var new_location = new Point(form.Location.X + (E.X - m_last_pos.X), form.Location.Y + (E.Y - m_last_pos.Y));

        if (Screen.PrimaryScreen.WorkingArea.Contains(new Rectangle(new_location, form.Size)))
          form.Location = new_location;
        else
        {
          var max_x = Screen.PrimaryScreen.WorkingArea.Width - form.Width;
          var max_y = Screen.PrimaryScreen.WorkingArea.Height - form.Height;

          if (max_x <= 0 || max_y <= 0)
            return;

          var x = new_location.X;
          var y = new_location.Y;

          if (x < 0)
            x = 0;
          else if (x > max_x)
            x = max_x;

          if (y < 0)
            y = 0;
          else if (y > max_y)
            y = max_y;

          form.Location = new Point(x, y);
        }
        return;
      }

      if (Cursors.SizeNS == Cursor.Current)
      {
        if (m_tops)
        {
          form.Location = new Point(form.Location.X, form.Location.Y + (E.Y - m_last_pos.Y));
          form.Height -= E.Y - m_last_pos.Y;
        }
        else
        {
          form.Height += E.Y - m_last_pos.Y;
          m_last_pos = E.Location;
        }
        return;
      }

      if (Cursors.SizeWE == Cursor.Current)
      {
        if (m_tops)
        {
          form.Location = new Point(form.Location.X + (E.X - m_last_pos.X), form.Location.Y);
          form.Width -= E.X - m_last_pos.X;
        }
        else
        {
          form.Width += E.X - m_last_pos.X;
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
      var form = m_ctl.FindForm();

      if (form == null)
        return;

      if (null != form)
        form.BringToFront();

      if( E.Button != MouseButtons.Left)
        return;

      if (CanSize)
      {
        if (E.Y < SizeBounds || E.Y > form.Height - SizeBounds)
        {
          m_tops = (E.Y < SizeBounds);
          Cursor.Current = Cursors.SizeNS;
          m_last_pos = E.Location;
          return;
        }

        if (E.X < SizeBounds || E.X > form.Width - SizeBounds)
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
