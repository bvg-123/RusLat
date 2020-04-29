using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RusLat.Tools
{
  /// <summary>
  /// Методы для раьботы с DPI, отличными от 96 стандартных.
  /// </summary>
  public static class DPI
  {
    /// <summary>
    /// Коэффициент, на который нужно умножить стандартные координаты в 96dpi для получения координат при масштабированном в Windows разрешении.
    /// </summary>
    public static double Scale
    {
      get
      {
        System.Windows.Media.Matrix matrix;
        using (var source = new System.Windows.Interop.HwndSource(new System.Windows.Interop.HwndSourceParameters()))
        {
          matrix = source.CompositionTarget.TransformToDevice;
        }
        return matrix.M11;
      } // get Scale
    } // Scale

  } // class DPI

} // namespace RusLat.Tools
