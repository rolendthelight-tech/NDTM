namespace AT.Toolbox.Constants
{
  using System;


  public enum Support
  {
    Full = 3,
    Partial = 2,
    Unknown = 1,
    None = 0
  }


  public enum MessageCodes
  {
    WmDestroy = 0x2,

    WmSetFocus = 0x0007,

    WmPaint = 0x000F,
    WmParentNotify = 0x0210,

    WmSetCursor = 0x0020,

    WmNotify = 0x004E,

    WmKeyDown = 0x0100,
    WmKeyUp = 0x0101,
    WmChar = 0x0102,

    WmCommand = 0x0112,

    WmMouseMove = 0x0200,
    WmMouseLbuttonDown = 0x0201,
    WmMouseLbuttonUp = 0x0202
  }


  public enum VirtualKeyCodes
  {
    Up = 38,
    Down = 40, //
    Left = 37, //
    Right = 39, // 
    Home = 36, // 
    End = 35, //
    PageUp = 34, // 
    PageDown = 33 // 
  }


  public enum CommandCodes
  {
    ScMinimize = 0xf020
  }


  [Flags]
  public enum ComputerTypes
  {
    Workstation = 0x00000001,
    Server = 0x00000002,
    Sqlserver = 0x00000004,
    DomainController = 0x00000008,
    DomainBackupController = 0x00000010,
    TimeSource = 0x00000020,
    Afp = 0x00000040,
    Novell = 0x00000080,
    DomainMember = 0x00000100,
    PruintqServer = 0x00000200,
    DialinServer = 0x00000400,
    XenixServer = 0x00000800,
    UnixServer = 0x00000800,
    Nt = 0x00001000,
    Wfw = 0x00002000,
    ServerMfpn = 0x00004000,
    ServerNt = 0x00008000,
    PotentialBrowser = 0x00010000,
    BackupBrowser = 0x00020000,
    MasterBrowser = 0x00040000,
    DomainMaster = 0x00080000,
    ServerOsf = 0x00100000,
    ServerVms = 0x00200000,
    Windows = 0x00400000, /* Windows95 and above */
    Dfs = 0x00800000, /* Root of a DFS tree */
    ClusterNt = 0x01000000, /* NT Cluster */
    TerminalServer = 0x02000000, /* Terminal Server(Hydra) */
    ClusterVsNt = 0x04000000, /* NT Cluster Virtual Server Name */
    Dce = 0x10000000, /* IBM DSS (Directory and Security Services) or equivalent */
    AlternateXport = 0x20000000, /* return list for alternate transport */
    LocalListOnly = 0x40000000 /* Return local list only */
    //DomainEnum = 0x80000000,
    //All = 0xFFFFFFFF  /* handy for NetServerEnum2 */
  }
}