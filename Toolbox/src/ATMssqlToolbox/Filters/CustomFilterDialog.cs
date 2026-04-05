using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using AT.Toolbox.Dialogs.Edit_Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace AT.Toolbox.DB
{
  public partial class CustomFilterDialog : XtraForm
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CustomFilterDialog).Name);

    protected BindingList<Filter> m_filters = new BindingList<Filter>();
    protected DataTable m_tbl;

    public CustomFilterDialog()
    {
      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ConnectionString { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SqlConnection Connection { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DataSourceName { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object SourceControl { get; set; }

    protected void PreLoadData()
    {
      if (null != SourceControl)
      {
        m_filter_editor.SourceControl = SourceControl;
        return;
      }

      if (null == Connection)
      {
        Connection = new SqlConnection(ConnectionString);
      }

      m_tbl = new DataTable(DataSourceName);

      SqlDataAdapter ad = new SqlDataAdapter(new SqlCommand(@"SELECT * FROM " + DataSourceName, Connection));

      ad.FillSchema(m_tbl, SchemaType.Source);

      foreach (DataColumn column in m_tbl.Columns)
      {
        try
        {
          Dictionary<string, string> dic = SqlExtendedPropertyHelper.GetExtendedColProperties(Connection, DataSourceName,
                                                                                              column.ColumnName);

          if (dic.ContainsKey(ApplicationSettings.Instance.DefaultLocale))
          {
            column.ExtendedProperties.Add("OriginalName", column.ColumnName);
            column.ColumnName = dic[ApplicationSettings.Instance.DefaultLocale];
          }

          foreach (KeyValuePair<string, string> pair in dic)
            column.ExtendedProperties.Add(pair.Key, pair.Value);
        }
        catch (Exception ex)
        {
          Log.Error("PreLoadData(): exception", ex);
        }
      }

      m_filter_editor.SourceControl = m_tbl;
    }

    protected string FilterExpressionToSystem(string s)
    {
      if (null == m_tbl || string.IsNullOrEmpty(s))
        return s;

      foreach (DataColumn col in m_tbl.Columns)
      {
        if (null == col.ExtendedProperties[ApplicationSettings.Instance.DefaultLocale] || null == col.ExtendedProperties["OriginalName"])
          continue;

        string userName = col.ExtendedProperties[ApplicationSettings.Instance.DefaultLocale].ToString();
        string systemName = col.ExtendedProperties["OriginalName"].ToString();

        s = s.Replace(userName, systemName);
      }

      return s;
    }

    protected string FilterExpressionToUser(string s)
    {
      if (null == m_tbl || string.IsNullOrEmpty(s))
        return s;

      foreach (DataColumn col in m_tbl.Columns)
      {
        if (null == col.ExtendedProperties[ApplicationSettings.Instance.DefaultLocale] || null == col.ExtendedProperties["OriginalName"])
          continue;

        string userName = col.ExtendedProperties[ApplicationSettings.Instance.DefaultLocale].ToString();
        string systemName = col.ExtendedProperties["OriginalName"].ToString();

        s = s.Replace(systemName, userName);
      }

      return s;
    }

    protected void DialogShown(object sender, EventArgs e)
    {
      PreLoadData();

      m_filters = FilterStorages.Instance(".\\test_filters.xml").Filters;
      m_filter_tree.DataSource = m_filters;
    }

    protected void FocusChanged(object sender, FocusedNodeChangedEventArgs e)
    {
      if (null == e.Node)
      {
        m_filter_editor.Visible = false;
        return;
      }

      if (null != e.OldNode)
      {
        Filter f = m_filter_tree.GetDataRecordByNode(e.OldNode) as Filter;

        if (null != f)
          f.FilterExpression = FilterExpressionToSystem(m_filter_editor.FilterString);
      }

      Filter f2 = m_filter_tree.GetDataRecordByNode(e.Node) as Filter;

      if (null != f2)
      {
        if (f2.IsGroup)
        {
          m_filter_editor.Visible = false;
          return;
        }

        m_filter_editor.Visible = true;
        m_filter_editor.FilterString = FilterExpressionToUser(f2.FilterExpression);
      }
    }

    protected void DialogClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult.OK != DialogResult)
      {
        FilterStorages.CurrentInstance.ReLoad();
        return;
      }

      Filter f = m_filter_tree.GetDataRecordByNode(m_filter_tree.FocusedNode) as Filter;

      if (null != f)
        f.FilterExpression = FilterExpressionToSystem(m_filter_editor.FilterString);

      FilterStorages.CurrentInstance.Save();

      if (null != SourceControl)
        m_filter_editor.ApplyFilter();
    }

    protected void FilterNodes(object sender, FilterNodeEventArgs e)
    {
      Filter f2 = m_filter_tree.GetDataRecordByNode(e.Node) as Filter;

      if (null != f2)
        e.Node.Visible = (f2.DataSourceName == DataSourceName);

      e.Handled = true;
    }

    protected void ContextMenOpening(object sender, CancelEventArgs e)
    {
      m_add_filter_cmd.Enabled = false;
      m_add_group_cmd.Enabled = false;
      m_remove_group_cmd.Enabled = false;
      m_remove_filter_cmd.Enabled = false;

      if (null != m_filter_tree.FocusedNode)
      {
        Filter f2 = m_filter_tree.GetDataRecordByNode(m_filter_tree.FocusedNode) as Filter;

        if (f2 == null)
          return;

        if (f2.IsGroup)
        {
          m_add_filter_cmd.Enabled = true;
          m_remove_group_cmd.Enabled = true;
        }
        else
          m_remove_filter_cmd.Enabled = true;
      }
      else
        m_add_group_cmd.Enabled = true;
    }

    protected void AddFilterGroup(object sender, EventArgs e)
    {
      StringEditForm frm = new StringEditForm();
      frm.Value = "";
      frm.Title = "New Filter Group Name";

      if (DialogResult.OK != frm.ShowDialog(this))
        return;

      Filter f = new Filter();
      f.IsGroup = true;
      f.FilterName = frm.Value;
      f.DataSourceName = DataSourceName;

      m_filters.Add(f);
    }

    protected void AddFilter(object sender, EventArgs e)
    {
      Filter f2 = m_filter_tree.GetDataRecordByNode(m_filter_tree.FocusedNode) as Filter;

      if (null == f2)
        return;

      StringEditForm frm = new StringEditForm();
      frm.Value = "";
      frm.Title = "New Filter Name";

      if (DialogResult.OK != frm.ShowDialog(this))
        return;


      Filter f = new Filter();
      f.IsGroup = false;
      f.FilterName = frm.Value;
      f.DataSourceName = DataSourceName;
      f.ParentID = f2.FilterID;

      m_filters.Add(f);
    }

    protected void RemoveFilter(object sender, EventArgs e)
    {
      Filter f2 = m_filter_tree.GetDataRecordByNode(m_filter_tree.FocusedNode) as Filter;

      if (null != f2)
      {
        m_filters.Remove(f2);
      }
    }

    protected void RemoveFilterGroup(object sender, EventArgs e)
    {
      List<Filter> filters_to_remove = new List<Filter>();

      Filter f2 = m_filter_tree.GetDataRecordByNode(m_filter_tree.FocusedNode) as Filter;

      m_filters.RaiseListChangedEvents = false;

      if (null != f2)
      {
        filters_to_remove.Add(f2);

        foreach (TreeListNode child in m_filter_tree.FocusedNode.Nodes)
        {
          f2 = m_filter_tree.GetDataRecordByNode(child) as Filter;

          if (null != f2)
            filters_to_remove.Add(f2);
        }

        foreach (Filter filter in filters_to_remove)
          m_filters.Remove(filter);
      }

      m_filters.RaiseListChangedEvents = true;

      m_filter_tree.RefreshDataSource();
    }
  }
}