using RusLat.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RusLat.Converters
{
  public class PixelConverter :IValueConverter
  {
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      object result = DependencyProperty.UnsetValue;
      if ((value != null) && (value is Raster.Pixel))
      {
        Raster.Pixel pixel = (Raster.Pixel)value;
        if (parameter is string)
        {
          if ((string)parameter == "R") result = pixel.Color.R.ToString("X2");
          if ((string)parameter == "G") result = pixel.Color.G.ToString("X2");
          if ((string)parameter == "B") result = pixel.Color.B.ToString("X2");
          if ((string)parameter == "E") result = pixel.E.ToString(@"0.00", CultureInfo.InvariantCulture);
        }
      }
      return result;
    } // Convert


    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    } // ConvertBack

  } // class PixelConverter

} // namespace RusLat.Converters
