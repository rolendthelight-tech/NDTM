using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using AT.Toolbox.MSSQL.DAL.RecordBinding;
using System.Reflection;

namespace AT.Toolbox.MSSQL.UI
{
  public class ContextBindingSource : BindingSource
  {
    DbContext context;
    bool resetting = false;

    public ContextBindingSource()
      : base()
    {
    }

    public ContextBindingSource(IContainer container)
      : base()
    {
    }

    public ContextBindingSource(DbContext dataSource, string dataMember)
      : base(dataSource, dataMember)
    {
    }

    [Category("Data")]
    public DbContext Context
    {
      get { return context; }
      set
      {
        context = value;
        if (context != null)
        {
          base.DataSource = context;
        }
        else
        {
          base.DataSource = null;
          base.DataMember = null;
        }
      }
    }

    [Browsable(false)]
    public new object DataSource
    {
      get
      {
        return base.DataSource;
      }
      set
      {
        if (this.DesignMode || this.context == null)
        {
          base.DataSource = value;
          if (value == null)
          {
            this.context = null;
          }
        }
      }
    }

    public override void ApplySort(PropertyDescriptor property, ListSortDirection sort)
    {
      base.ApplySort(property, sort);
    }

    protected override void OnListChanged(ListChangedEventArgs e)
    {
      if (!resetting)
      {
        if (e.ListChangedType == ListChangedType.Reset)
        {
          resetting = true;
          string tmpMember = this.DataMember;
          base.DataSource = null;
          base.OnListChanged(e);
          base.DataSource = context;
          this.DataMember = tmpMember;
          resetting = false;
          return;
        }
        else
        {
          base.OnListChanged(e);

          if (e.ListChangedType == ListChangedType.ItemAdded)
          {
            IBindableObjectList list = this.List as IBindableObjectList;
            if (list != null && !this.AllowNew)
            {
              this.Position = list.CurrentObjectIdx;
            }
          }
        }
      }
    }
    
    protected override void OnPositionChanged(EventArgs e)
    {
      base.OnPositionChanged(e);

      IBindableObjectList list = this.List as IBindableObjectList;
      if (list != null)
      {
        list.CurrentObjectIdx = this.Position;
      }
    }
  }
}
