using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

using AT.Toolbox.Log;

using DevExpress.XtraGrid;


namespace AT.Toolbox.MSSQL
{
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class SimpleDalc : Component, IBindingListView, ITypedList, ICurrencyManagerProvider, ISupportInitialize
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SimpleDalc).Name);
    
    protected bool m_busy;
    protected bool m_initialized;
    
    protected SqlConnection m_Connection = new SqlConnection();
    protected DataTable m_Data;
    protected BindingSource m_data_source;
    protected SqlDataAdapter m_adapter = new SqlDataAdapter();
    public bool ImmidiateUpdate { get; set; }

    protected Dictionary<string, ParameterValue> m_param_dic = new Dictionary<string, ParameterValue>();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Dictionary<string, ParameterValue> AutoSubstituteParams { get { return m_param_dic; } }
    SqlCommand m_SelectCommand = new SqlCommand();
    
    [Category("Commands")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SqlCommand SelectCommand
    {
      get
      {
        return m_SelectCommand;
      }
    }

    SqlCommand m_UpdateCommand = new SqlCommand();

    [Category("Commands")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SqlCommand UpdateCommand
    {
      get { return m_UpdateCommand; }
    }

    SqlCommand m_InsertCommand= new SqlCommand();

    [Category("Commands")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SqlCommand InsertCommand
    {
      get { return m_InsertCommand; }
    }
    SqlCommand m_DeleteCommand = new SqlCommand();

    [Category("Commands")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SqlCommand DeleteCommand
    {
      get { return m_DeleteCommand; }
    }

    public SimpleDalc()
    {
      ImmidiateUpdate = true;
      Name = "DataTable";
      ListChanged += new ListChangedEventHandler(SimpleDalc_ListChanged);
    }

    void SimpleDalc_ListChanged(object sender, ListChangedEventArgs e)
    {
      if( ImmidiateUpdate )
        Update( );
    }

    protected void TryInitialize()
    {
      if( null == Connection )
        return;

      if( string.IsNullOrEmpty( SelectCommand.CommandText ) ) 
        return;

      if( null != m_Data && m_Data.Columns.Count > 0 )
        return;

      Initialize(  );
    }

    protected void Initialize()
    {
      if (null == m_Connection)
        return;

      m_adapter.SelectCommand = SelectCommand;

      if (!SelectOnly)
      {
        m_adapter.UpdateCommand = UpdateCommand;
        m_adapter.InsertCommand = InsertCommand;
        m_adapter.DeleteCommand = DeleteCommand;
      }

      if (null == m_Data)
      {
        m_Data = new DataTable(Name);
        m_adapter.FillSchema(m_Data, SchemaType.Source);
      }

      if (null == m_data_source)
      {
        m_data_source = new BindingSource(m_Data, null);
        m_data_source.RaiseListChangedEvents = true;
        m_data_source.ListChanged += new ListChangedEventHandler(HandleBindingSourceChanged);
      }
    }

    public SimpleDalc(IContainer container)
    {
      Name = "DataTable";
      container.Add(this);

      if( DesignMode )
        return;

      if (container is Form)
      {
        Form c = container as Form;
        c.Shown += new EventHandler(c_Shown);
        c.Enter += new EventHandler(c_Enter);
        c.GotFocus += new EventHandler(c_GotFocus);
      }
      else if( container is Control )
      {
        Control c = container as Control;
        c.Enter += new EventHandler(c_Enter);
        c.GotFocus += new EventHandler(c_GotFocus);
      }
    }

    void c_Shown(object sender, EventArgs e)
    {
      Fetch();
    }

    void c_GotFocus(object sender, EventArgs e)
    {
      Fetch();
    }

    void c_Enter(object sender, EventArgs e)
    {
      Fetch(  );
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SqlConnection Connection
    {
      get
      {
        return m_Connection;
      }
      set
      {
        if( DesignMode )
          return;

        Log.DebugFormat("Connection.set({0})", value.ConnectionString); // TODO: exposing confidential data !

        m_Connection = value;

        SelectCommand.Connection = value;
        UpdateCommand.Connection = value;
        InsertCommand.Connection = value;
        DeleteCommand.Connection = value;

        TryInitialize( );
      }
    }

    [Category("Connections")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public SqlConnection DesignTimeConnection
    {
      get
      {
        return m_Connection;
      }
      set
      {
        if( !DesignMode )
          return;
       
        Log.DebugFormat("DesignTimeConnection.set({0})", value.ConnectionString); // TODO: exposing confidential data !
        m_Connection = value;
        
        SelectCommand.Connection = value;
        UpdateCommand.Connection = value;
        InsertCommand.Connection = value;
        DeleteCommand.Connection = value;

        TryInitialize( );
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTable Data
    {
      get
      {
        try
        {
          if (null == m_adapter)
            return null;

          TryInitialize();

          return m_Data;  
        }
        catch( Exception ex)
        {
          return null;
        }
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected BindingSource DataSource
    {
      get
      {
        if (null == m_Data)
          return null;

        TryInitialize();

        return m_data_source;
      }
    }

    void HandleBindingSourceChanged( object sender, ListChangedEventArgs e )
    {
      try
      {
        if (e.ListChangedType == ListChangedType.PropertyDescriptorAdded ||
            e.ListChangedType == ListChangedType.PropertyDescriptorChanged ||
            e.ListChangedType == ListChangedType.PropertyDescriptorDeleted)
          return;


        if (!m_busy && ImmidiateUpdate)
          Update();
      }
      catch( Exception ex)
      {
        //Log2.Error("HandleBindingSourceChanged(): exception", ex);
      }
      
    }

    public string Name{get; set;}
    public string DataMember { get; set; }
    public bool SelectOnly { get; set; }

    protected virtual void UpdateParameter( SqlCommand cmd,KeyValuePair<string, ParameterValue> pair )
    {
      if (cmd.Parameters.Contains(pair.Key))
      {
        if (null != pair.Value.Value)
          cmd.Parameters[pair.Key].Value = pair.Value.Value;
        else
        {
          if (pair.Value.PassNullValue)
            cmd.Parameters[pair.Key].Value = DBNull.Value;
        }
      }
    }

    public virtual void UpdateParameters()
    {
      foreach( KeyValuePair<string, ParameterValue> pair in m_param_dic )
      {
        UpdateParameter( SelectCommand, pair );
        UpdateParameter( UpdateCommand, pair );
        UpdateParameter( DeleteCommand, pair );
        UpdateParameter( InsertCommand, pair );
      }
    }

    public void Fetch()
    {
      try
      {
        if (m_busy)
          return;

        if( null == m_adapter || null == Data )
          return;

        UpdateParameters();
        DataTable dt = new DataTable("temp");
        m_busy = true;
        this.m_data_source.SuspendBinding();
        Data.BeginLoadData(  );
        m_adapter.Fill( dt );
        Data.Merge( dt,true, MissingSchemaAction.Add );
      }
      catch( Exception ex)
      {
        //Log2.Error("Fetch(): exception", ex);
      }
      finally
      {
        m_busy = false;
        Data.EndLoadData(  );
        this.m_data_source.ResumeBinding();
      }
    }

    public void Reload()
    {
      try
      {
        if (m_busy)
          return;

        if (null == m_adapter || null == Data)
          return;

        UpdateParameters();
        DataTable dt = new DataTable("temp");
        m_busy = true;
        Data.Clear();
        this.m_data_source.SuspendBinding();
        Data.BeginLoadData();
        m_adapter.Fill(dt);
        Data.Merge(dt, true, MissingSchemaAction.Add);
      }
      catch (Exception ex)
      {
        //Log2.Error("Reload(): exception", ex);
      }
      finally
      {
        m_busy = false;
        Data.EndLoadData();
        this.m_data_source.ResumeBinding();
      }
    }


    public void Update( ) 
    {
      try
      {
        if (m_busy)
          return;

        if (null == m_adapter)
          return;

        UpdateParameters( );
        this.m_data_source.SuspendBinding();
        m_busy = true;
        m_adapter.Update(Data);
      }
      catch( Exception ex)
      {
        //Log2.Error("Update(): exception", ex);
      }
      finally
      {
        this.m_data_source.ResumeBinding();
        m_busy = false;
      }
    }

    public List<object[]> ExecuteReader(string Procedure, ref Dictionary<string, object> values)
    {
      List<object[]> ret_val = new List<object[]>();

      try
      {
        SqlCommand cmd = new SqlCommand(Procedure);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Connection = m_Connection;

        m_Connection.Open();


        foreach (KeyValuePair<string, object> pair in values)
        {
          SqlParameter param = new SqlParameter(pair.Key, pair.Value);
          cmd.Parameters.Add(param);
        }

        using (SqlDataReader rd = cmd.ExecuteReader())
        {
          if (null == rd)
            return ret_val;

          while (rd.Read())
          {
            object[] ret_values = new object[rd.VisibleFieldCount];
            rd.GetValues(ret_values);
            ret_val.Add(ret_values);
          }
        }
      }
      catch (Exception ex)
      {
        //Log2.Error(string.Format("ExecuteReader({0}): exception", Procedure), ex);
      }
      finally
      {
        m_Connection.Close();
      }

      return ret_val;
    }

    public int ExecuteNonQuery( string Procedure, ref Dictionary<string,object> values )
    {
      try
      {
        SqlCommand cmd = new SqlCommand(Procedure);
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Connection = m_Connection;

        m_Connection.Open();

        foreach( KeyValuePair<string, object> pair in values )
        {
          SqlParameter param = new SqlParameter(pair.Key, pair.Value);
          cmd.Parameters.Add( param );
        }

        return cmd.ExecuteNonQuery( );
      }
      catch (Exception ex)
      {
        //Log2.Error(string.Format("ExecuteNonQuery({0}): exception", Procedure), ex);
        return 0;
      }
      finally
      {
        m_Connection.Close();
      }
     
      //return 0;
    }

    public bool CheckTableForExistence( string TableName, string FieldName, object value )
    {
      try
      {
        string text = "SELECT * FROM " + TableName + " WHERE " + FieldName + " ='" + value + "'";
        SqlCommand cmd = new SqlCommand(text);
        
        cmd.Connection = m_Connection;

        m_Connection.Open();

        using (SqlDataReader rd = cmd.ExecuteReader())
        {
          if (null == rd)
            return false;

          if (!rd.Read())
            return false;

          return true;
        }
      }
      catch (Exception ex)
      {
        //Log2.Error(string.Format("CheckTableForExistence({0}, {1}, {2}): exception", TableName, FieldName, value), ex);
        return false;
      }
      finally
      {
        m_Connection.Close();
      }

      //return false;
    }

    public object AddNewRow( Dictionary<string,object> values )
    {
      object ret_val = null;

      try
      {
        m_InsertCommand.Connection = m_Connection;

        foreach( SqlParameter param in m_InsertCommand.Parameters )
        {
          if (string.IsNullOrEmpty(param.SourceColumn))
          {
            if (values.ContainsKey(param.ParameterName.Replace("@", "")))
              param.Value = values[param.ParameterName.Replace("@", "")];
          }
          else
          {
            if( values.ContainsKey( param.SourceColumn ) )
              param.Value = values[param.SourceColumn];
            else
            {
              if( param.IsNullable )
                param.Value = DBNull.Value;
              else
                throw new ArgumentException( "Values do not contain " + param.SourceColumn, "values" );
            }
          }
        }

        m_Connection.Open();
        
        using( SqlDataReader rd  = m_InsertCommand.ExecuteReader() )
        {
          if( null == rd )
            return ret_val;

          if( !rd.Read(  ) )
            return ret_val;

          ret_val = rd.GetValue(Data.Columns.IndexOf(Data.PrimaryKey[0]));
        }
      }
      catch (Exception ex)
      {
        //Log2.Error("AddNewRow(): exception", ex);
      }
      finally
      {
        m_Connection.Close();
        Fetch(  );
      }

      return ret_val;
    }

    protected void InitializeComponent()
    {
    }
    
    #region Implementation of IEnumerable

    public IEnumerator GetEnumerator()
    {
      return DataSource.GetEnumerator( );
    }

    #endregion

    #region Implementation of ICollection

    public void CopyTo( Array array, int index )
    {
      if( null == DataSource )
        return;

      DataSource.CopyTo( array, index );
    }

    public int Count
    {
      get
      {
        if (null == DataSource)
          return 0;

        return DataSource.Count;
      }
    }

    public object SyncRoot
    {
      get
      {
        if (null == DataSource)
          return null;

        return DataSource.SyncRoot;
      }
    }

    public bool IsSynchronized
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.IsSynchronized;
      }
    }

    #endregion
    
    #region Implementation of IList

    public int Add( object value )
    {
      if (null == DataSource)
        return -1;

      return DataSource.Add( value ) ;
    }

    public bool Contains(object value)
    {
      if (null == DataSource)
        return false;

      return DataSource.Contains( value );
    }

    public void Clear( )
    {
      if (null == DataSource)
        return ;

      DataSource.Clear(  );
    }

    public int IndexOf(object value)
    {
      if (null == DataSource)
        return -1;

      return DataSource.IndexOf( value );
    }

    public void Insert( int index, object value )
    {
      if (null == DataSource)
        return ;

      DataSource.Insert( index ,value );
    }

    public void Remove( object value )
    {
      if (null == DataSource)
        return ;

      DataSource.Remove( value );
    }

    public void RemoveAt( int index )
    {
      if (null == DataSource)
        return ;

      DataSource.RemoveAt( index );
    }

    public object this[ int index ]
    {
      get
      {
        if (null == DataSource)
          return null;

        return DataSource[index];
      }
      set
      {
        if (null == DataSource)
          return ;

        DataSource[index] = value;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        if (null == DataSource)
          return true;

        return DataSource.IsReadOnly;
      }
    }

    public bool IsFixedSize
    {
      get
      {
        if (null == DataSource)
          return true;

        return DataSource.IsFixedSize;
      }
    }

    #endregion
    
    #region Implementation of IBindingList

    public event ListChangedEventHandler ListChanged
    {
      add
      {
        if (null == DataSource)
          return ;

        DataSource.ListChanged += value;
      }
      remove
      {
        if (null == DataSource)
          return ;

        DataSource.ListChanged -= value;
      }
    }

    public object AddNew()
    {
      if (null == DataSource)
        return null;

      return DataSource.AddNew( );
    }

    public void AddIndex( PropertyDescriptor property ) { }

    public void ApplySort( PropertyDescriptor property, ListSortDirection direction )
    {
      if (null == DataSource)
        return ;

      DataSource.ApplySort( property,direction );
    }

    public int Find(PropertyDescriptor property, object key)
    {
      if (null == DataSource)
        return -1;

      return DataSource.Find( property, key );
    }

    public void RemoveIndex( PropertyDescriptor property ) { }

    public void RemoveSort( )
    {
      if (null == DataSource)
        return ;

      DataSource.RemoveSort(  );
    }

    public bool AllowNew
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.AllowNew;
      }
    }

    public bool AllowEdit
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.AllowEdit;
      }
    }

    public bool AllowRemove
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.AllowRemove;
      }
    }

    public bool SupportsChangeNotification
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.SupportsChangeNotification;
      }
    }

    public bool SupportsSearching
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.SupportsSearching;
      }
    }

    public bool SupportsSorting
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.SupportsSorting;
      }
    }

    public bool IsSorted
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.IsSorted;
      }
    }

    public PropertyDescriptor SortProperty
    {
      get
      {
        if (null == DataSource)
          return null;

        return DataSource.SortProperty;
      }
    }

    public ListSortDirection SortDirection
    {
      get
      {
        if (null == DataSource)
          return ListSortDirection.Ascending;
        return DataSource.SortDirection;
      }
    }

    #endregion
    
    #region Implementation of IBindingListView

    public void ApplySort( ListSortDescriptionCollection sorts )
    {
      if (null == DataSource)
        return;
      DataSource.ApplySort( sorts );
    }

    public void RemoveFilter( )
    {
      if (null == DataSource)
        return;

      DataSource.RemoveFilter(  );
    }

    public string Filter
    {
      get
      {
        if (null == DataSource)
          return "";

        return DataSource.Filter;
      }
      set
      {
        if (null == DataSource)
          return;

        DataSource.Filter = value;
      }
    }

    public ListSortDescriptionCollection SortDescriptions
    {
      get
      {
        if (null == DataSource)
          return null;

        return DataSource.SortDescriptions;
      }
    }

    public bool SupportsAdvancedSorting
    {
      get
      {
        if (null == DataSource)
          return false ;

        return DataSource.SupportsAdvancedSorting;
      }
    }

    public bool SupportsFiltering
    {
      get
      {
        if (null == DataSource)
          return false;

        return DataSource.SupportsFiltering;
      }
    }

    #endregion
    
    #region Implementation of ITypedList

    public string GetListName( PropertyDescriptor[] listAccessors ) 
    {
      if (null == DataSource)
        return "";

      return DataSource.GetListName( listAccessors );
    }

    public PropertyDescriptorCollection GetItemProperties( PropertyDescriptor[] listAccessors )
    {
      if (null == DataSource)
        return null;

      return DataSource.GetItemProperties(listAccessors);
    }

    #endregion
    
    #region Implementation of ICurrencyManagerProvider

    public CurrencyManager GetRelatedCurrencyManager( string dataMember ) 
    {
      if (null == DataSource)
        return null;

      return DataSource.GetRelatedCurrencyManager( dataMember );
    }

    public CurrencyManager CurrencyManager
    {
      get
      {
        if (null == DataSource)
          return null;
        return DataSource.CurrencyManager;
      }
    }

    #endregion
    
    #region Implementation of ISupportInitialize

    public void BeginInit( ) 
    { 
    }

    public void EndInit( ) 
    {
      if( DesignMode )
        TryInitialize( );
    }

    #endregion


    GridControl m_Target;

    public GridControl Target
    {
      get { return m_Target; }
      set
      {
        m_Target = value;

        if( null != m_Target)
          m_Target.DataSource = this;
      }
    }

    RecordLocker m_RecordLocker;

    public RecordLocker RecordLocker
    {
      get { return m_RecordLocker; }
      set
      {
        m_RecordLocker = value;

        if (null == m_RecordLocker)
          return;

        if (null == m_RecordLocker.DALC)
          m_RecordLocker.DALC = this;
      }
    }
  }

  public class ParameterValue
  {
    public ParameterValue(object o, bool b) 
    {
      Value = o;
      PassNullValue = b; 
    }

    public object Value { get; set; }
    public bool PassNullValue { get; set; }
  }
}