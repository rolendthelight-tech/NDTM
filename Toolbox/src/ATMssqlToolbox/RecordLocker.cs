using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using AT.Toolbox.MSSQL;


namespace AT.Toolbox.MSSQL
{
  public partial class RecordLocker : Component
  {
    public RecordLocker()
    {
      InitializeComponent();
    }

    public RecordLocker(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    SimpleDalc m_dalc;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SimpleDalc DALC
    {
      get { return m_dalc; }
      set
      {
        m_dalc = value;

        if( null == m_dalc )
          return;

        if( null == m_dalc.RecordLocker )
          m_dalc.RecordLocker = this;
      }
    }

    public bool AcquireLock( object ID )
    {
      if (null == m_dalc)
        return false;

      string TableName = m_dalc.Name;

      Dictionary<string, object> values = new Dictionary<string, object>();
      values.Add( "@TableName", TableName );
      values.Add( "@ID" ,ID );

      List<object[]> ret_val = m_dalc.ExecuteReader( "GetLock", ref values );

      if (ret_val.Count > 0)
        return false;

      m_dalc.ExecuteNonQuery("SetLock", ref values);

      return true;
    }

    public void ReleaseLock( object ID )
    {
      if( null == m_dalc )
        return;

      string TableName = m_dalc.Name;

      Dictionary<string, object> values = new Dictionary<string, object>();
      values.Add("@TableName", TableName);
      values.Add("@ID", ID);
      
      m_dalc.ExecuteNonQuery("ReleaseLock", ref values);
    }
  }
}
