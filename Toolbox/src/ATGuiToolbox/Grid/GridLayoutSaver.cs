using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Grid
{
  public partial class GridLayoutSaver : Component
  {
    public GridLayoutSaver()
    {
      InitializeComponent();
    }

    public GridLayoutSaver(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }
  }
}
