using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RusLat.Tools
{
  static class ScreenCapturer
  {

    public enum CaptureMode
    {
      Screen,
      Window
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow ();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowRect (IntPtr hWnd, ref Rect rect);

    [DllImport(@"dwmapi.dll")]
    private static extern int DwmGetWindowAttribute (IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }


    /// <summary>
    /// Возвращает картинку со снимком заданной области экрана.
    /// </summary>
    /// <param name="bounds">Область экрана, снимок которой требуется получить.</param>
    /// <returns>Возвращает картинку со снимком заданной области экрана.</returns>
    public static Bitmap Capture (Rectangle bounds)
    {
      Bitmap result = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      using (Graphics g = Graphics.FromImage(result))
      {
        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, new Size(bounds.Size.Width, bounds.Size.Height), CopyPixelOperation.SourceCopy);
      }
      return result;
    } // Capture


    /// <summary>
    /// Возвращает снимок верхнего окна или всего экрана.
    /// </summary>
    /// <param name="screenCaptureMode">Снимок чего нужно получить: окна или экрана.</param>
    /// <returns>Возвращает снимок верхнего окна или всего экрана.</returns>
    public static Bitmap Capture (CaptureMode screenCaptureMode = CaptureMode.Window)
    {
      Rectangle bounds;

      if (screenCaptureMode == CaptureMode.Screen)
      {
        bounds = Screen.GetBounds(Point.Empty);
      }
      else
      {
        var handle = GetForegroundWindow();
        var rect = new Rect();
        
        // Если Win XP и ранее то используем старый способ
        if (Environment.OSVersion.Version.Major < 6)
        {
          GetWindowRect(handle, ref rect);
        }
        else
        {
          var res = -1;
          try
          {
            res = DwmGetWindowAttribute(handle, 9, out rect, Marshal.SizeOf(typeof(Rect)));
          }
          catch { }
          if (res<0) GetWindowRect(handle, ref rect);
        }

        bounds = new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
      }
      return Capture(bounds);
    } // Capture


  } // class ScreenCapturer.Tools

} // namespace RusLat
