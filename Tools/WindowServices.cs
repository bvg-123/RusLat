using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools
{
  public static class WindowsServices
  {
    const int WS_EX_TRANSPARENT = 0x00000020;
    const int GWL_EXSTYLE = (-20);

    [DllImport("user32.dll")]
    static extern int GetWindowLong (IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    static extern int SetWindowLong (IntPtr hwnd, int index, int newStyle);

    public static void SetWindowExTransparent (IntPtr hwnd)
    {
      var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
      SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow ();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId (IntPtr hWnd, IntPtr ProcessId);

    [DllImport("user32.dll")]
    static extern Int32 GetKeyboardLayout (uint idThread);

    public static int GetCurrentLang ()
    {
      return GetKeyboardLayout(GetWindowThreadProcessId(GetActiveWindow(), IntPtr.Zero));
    }

  } // class WindowsServices

} // namespace RusLat.Tools
