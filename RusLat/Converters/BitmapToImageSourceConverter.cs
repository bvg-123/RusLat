using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RusLat.Converters
{
  public class BitmapToImageSourceConverter :IValueConverter
  {
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      object result = DependencyProperty.UnsetValue;
      if ((value != null) && (value is System.Drawing.Bitmap))
      {
        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)value;
        result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight((int)Math.Round(bitmap.Width*96.0/bitmap.HorizontalResolution), (int)Math.Round(bitmap.Height*96.0/bitmap.VerticalResolution)));
      }
      return result;
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    } // ConvertBack

  } // class BitmapToImageSourceConverter

} // namespace RusLat.Converters
