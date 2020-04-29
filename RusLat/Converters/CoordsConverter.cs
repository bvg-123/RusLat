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
  public class CoordsConverter :IValueConverter
  {
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      object result = DependencyProperty.UnsetValue;
      if ((value != null) && (value is System.Drawing.Point))
      {
        System.Drawing.Point p = (System.Drawing.Point)value;
        result = $"{p.X},{p.Y}";
      }
      return result;
    } // Convert


    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    } // ConvertBack

  } // class CoordsConverter

} // namespace RusLat.Converters
