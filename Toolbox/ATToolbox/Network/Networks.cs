namespace AT.Toolbox.Network
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.Sql;
  using System.Runtime.InteropServices;
  using System.Security;

  using Constants;

  using Log;


  /// <summary>
  /// ��������������� ����� ��� ������ � �����
  /// </summary>
  public static class NetLister
  {
    //TODO: �����������

    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(NetLister).Name);


    #region Dll Imports ---------------------------------------------------------------------------------------------------------

    [DllImport( "Netapi32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false,
      ThrowOnUnmappableChar = true )]
    [SuppressUnmanagedCodeSecurity]
    public static extern int NetServerEnum( string ServerName,
                                            // must be null
                                            int dwLevel,
                                            ref IntPtr pBuf,
                                            int dwPrefMaxLen,
                                            out int dwEntriesRead,
                                            out int dwTotalEntries,
                                            int dwServerType,
                                            string domain,
                                            // null for login domain
                                            out int dwResumeHandle );

    [DllImport( "Netapi32", SetLastError = true )]
    [SuppressUnmanagedCodeSecurity]
    public static extern int NetApiBufferFree( IntPtr pBuf );


    [StructLayout( LayoutKind.Sequential )]
    public struct ServerInfo
    {
      internal int sv100_platform_id;

      [MarshalAs( UnmanagedType.LPWStr )]
      internal string sv100_name;
    }

    #endregion


    #region Public Methods ------------------------------------------------------------------------------------------------------

    public static List<string> GetNetworkComputers( ) { return GetNetworkComputers( ComputerTypes.Workstation | ComputerTypes.Server ); }

    /// <summary>
    /// ��������� ������ ����������� � ����
    /// </summary>
    /// <returns>������ ��� �����������</returns>
    public static List<string> GetNetworkComputers( ComputerTypes Type )
    {
      const int MAX_PREFERRED_LENGTH = -1;

      IntPtr buffer = IntPtr.Zero;
      List<string> ret_val = new List<string>( );

      try
      {
        int entriesRead;
        int totalEntries;
        int resHandle;
        int sizeofINFO = Marshal.SizeOf( typeof( ServerInfo ) );

        int result = NetServerEnum( null,
                                    100,
                                    ref buffer,
                                    MAX_PREFERRED_LENGTH,
                                    out entriesRead,
                                    out totalEntries,
                                    (int) Type,
                                    null,
                                    out resHandle );

        if( 0 != result )
        {
          Log.Error( "GetNetworkComputers(): NetServerEnum returned " + result );
          return ret_val;
        }

        for( int i = 0; i < totalEntries; i++ )
        {
          IntPtr tmpBuffer = IntPtr.Add( buffer, i * sizeofINFO );

          ServerInfo svrInfo = (ServerInfo) Marshal.PtrToStructure( tmpBuffer, typeof( ServerInfo ) );

          ret_val.Add( svrInfo.sv100_name );
        }
      }
      catch( Exception ex )
      {
        Log.Error( "GetNetworkComputers(): exception", ex );
      }
      finally
      {
        NetApiBufferFree( buffer );
      }

      return ret_val;
    }

    /// <summary>
    /// ��������� ������ SQL-��������
    /// </summary>
    /// <returns>������ ��� SQL-��������</returns>
    public static List<string> GetSQLServers( )
    {
      List<string> ret_val = new List<string>( );

      try
      {
#if NETFRAMEWORK
        SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
        DataTable table = instance.GetDataSources( );

        foreach( DataRow row in table.Rows )
        {
          string instanceName = row["InstanceName"].ToString();
          string val = (string) row["ServerName"] 
            + (!string.IsNullOrEmpty( instanceName ) ? "\\" + instanceName : "");

          if( !ret_val.Contains( val ) )
            ret_val.Add( val );
        }
#else
        Log.Info("GetSQLServers(): SqlDataSourceEnumerator not supported on this target framework; returning empty list as a temporary fallback.");
#endif
      }
      catch( Exception ex )
      {
        Log.Error( "GetSQLServers(): exception", ex );
      }

      return ret_val;
    }

    #endregion
  }
}