using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL
{
  public class CSVMapper : Component, IListSource
  {
    private BindingList<CSVMapperItem> items = new BindingList<CSVMapperItem>();
    private ArrayList listSource;

    public CSVMapper(IContainer container)
      : base()
    {
      container.Add(this);
      listSource = new ArrayList() { this };
    }

    public BindingList<CSVMapperItem> Items
    {
      get { return items; }
    }

    #region IListSource Members

    bool IListSource.ContainsListCollection
    {
      get { return true; }
    }

    IList IListSource.GetList()
    {
      return listSource;
    }

    #endregion
  }

  public class CSVMapperItem
  {
    public int ColumnIndex { get; set; }
    public string ColumnName { get; set; }
  }
}
