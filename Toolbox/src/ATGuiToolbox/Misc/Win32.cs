using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace AT.Toolbox.Misc
{
  public static class Win32
  {
    public static int EM_CHARFROMPOS = 0x00D7;
    public static int EM_REPLACESEL = 0x00C2;
    public const int 
      GWL_WNDPROC         = (-4),
      GWL_HINSTANCE       = (-6),
      GWL_HWNDPARENT      = (-8),
      GWL_STYLE           = (-16),
      GWL_EXSTYLE         = (-20),
      GWL_USERDATA        = (-21),
      GWL_ID              = (-12),

        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_STATICEDGE = 0x00020000,
        WS_BORDER = 0x00800000,
        WM_CREATE =  0x0001,
        WM_NOTIFY = 0x004E,
        WM_MENUCOMMAND = 0x0126,
        WM_COMMAND = 0x0112,
        WM_DESTROY = 0x0002,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_CONTEXTMENU = 0x007B,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_MOUSEACTIVATE = 0x0021,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSELAST = 0x020A,
        WM_CAPTURECHANGED = 0x0215,
        WM_USER = 1024,
        EM_FORMATRANGE = WM_USER + 57,
        EM_SETZOOM = WM_USER + 225,
        MM_ISOTROPIC = 7,
        ROP_DSTINVERT = 0x00550009,
        WM_PARENTNOTUFY = 0x0210;

    [StructLayout(LayoutKind.Sequential)]
    public class SIZE
    {
      public int cx;
      public int cy;

      public SIZE()
      {
      }
      public SIZE(int cx, int cy)
      {
        this.cx = cx;
        this.cy = cy;
      }
    }
    public struct STRUCT_CHARRANGE
    {
      public int cpMin;
      public int cpMax;
    }
    public struct STRUCT_RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
    }
    public struct STRUCT_FORMATRANGE
    {
      public IntPtr hdc;
      public IntPtr hdcTarget;
      public STRUCT_RECT rc;
      public STRUCT_RECT rcPage;
      public STRUCT_CHARRANGE chrg;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PARAFORMAT2
    {
      public int cbSize;
      public int dwMask;
      public short wNumbering;
      public short wReserved;
      public int dxStartIndent;
      public int dxRightIndent;
      public int dxOffset;
      public short wAlignment;
      public short cTabCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      public int[] rgxTabs;
      public int dySpaceBefore;
      public int dySpaceAfter;
      public int dyLineSpacing;
      public short sStyle;
      public byte bLineSpacingRule;
      public byte bOutlineLevel;
      public short wShadingWeight;
      public short wShadingStyle;
      public short wNumberingStart;
      public short wNumberingStyle;
      public short wNumberingTab;
      public short wBorderSpace;
      public short wBorderWidth;
      public short wBorders;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct CharFormat2
    {
      public int cbSize;
      public int dwMask;
      public int dwEffects;
      public int yHeight;
      public int yOffset;
      public int crTextColor;
      public byte bCharSet;
      public byte bPitchAndFamily;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
      public string szFaceName;
      public short wWeight;
      public short sSpacing;
      public int crBackColor;
      public int lcid;
      public int dwReserved;
      public short sStyle;
      public short wKerning;
      public byte bUnderlineType;
      public byte bAnimation;
      public byte bRevAuthor;
      public byte bReserved1;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
      public IntPtr hwndFrom;
      public IntPtr idFrom;
      public uint   code;         // NM_ code
    };
    
    public static IntPtr MakeLParam(int low, int high)
    {
      return (IntPtr)((high << 16) | (low & 0xffff));
    }
    public static int HiWord(IntPtr n)
    {
      return HiWord(unchecked((int)(long)n));
    }
    static int HiWord(int n)
    {
      return (n >> 16) & 0xffff;
    }
    public static int LoWord(IntPtr n)
    {
      return LoWord(unchecked((int)(long)n));
    }
    static int LoWord(int n)
    {
      return n & 0xffff;
    }
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

    [DllImport("user32.dll")]
    public static extern int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, uint wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int DefWindowProc( IntPtr hWnd, uint Msg, uint wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

    [DllImport("user32")]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, IntPtr lParam);

    [DllImport("user32")]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string s);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern int SetMapMode(HandleRef hDC, int nMapMode);

    [DllImport("gdi32.dll")]
    public static extern bool SetWindowExtEx(HandleRef hDC, int x, int y, [In, Out] SIZE size);

    [DllImport("gdi32.dll")]
    public static extern bool SetViewportExtEx(HandleRef hDC, int x, int y, SIZE size);

    [DllImport("gdi32.dll")]
    public static extern bool SetViewportOrgEx(IntPtr hDC, int x, int y, [In, Out] SIZE size);

    [DllImport("gdi32.dll")]
    public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int keyCode);

    [DllImport("user32.dll")]
    public static extern void WaitMessage();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr GlobalAlloc(int uFlags, IntPtr dwBytes);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr GlobalFree(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr GlobalLock(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern bool GlobalUnlock(HandleRef handle);


    public static MouseButtons GetMouseButtons()
    {
      MouseButtons buttons = MouseButtons.None;
      if (GetAsyncKeyState(1) < 0)
        buttons |= MouseButtons.Left;
      if (GetAsyncKeyState(2) < 0)
        buttons |= MouseButtons.Right;
      if (GetAsyncKeyState(4) < 0)
        buttons |= MouseButtons.Middle;
      if (GetAsyncKeyState(5) < 0)
        buttons |= MouseButtons.XButton1;
      if (GetAsyncKeyState(6) < 0)
        buttons |= MouseButtons.XButton2;
      return buttons;
    }


    public delegate int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam);
  }
}
