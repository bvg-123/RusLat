using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Базовый класс детекторов сходства пиксельных растров.
  /// </summary>
  public class RasterAffinityDetector :BaseAffinityDetector<Raster.Pixel>
  {
    /// <summary>
    /// Ключ идентификации устойчиввых блоков, в качестве которых выступают непосредственно сами пиксели растра.
    /// </summary>
    public class PixelCoordsKey
    {
      /// <summary>
      /// Координата пикселя растра по горизонтали.
      /// </summary>
      public readonly int X;

      /// <summary>
      /// Координата пикселя растра по вертикали.
      /// </summary>
      public readonly int Y;


      /// <summary>
      /// Конструктор.
      /// </summary>
      /// <param name="x">Координата пикселя растра по горизонтали.</param>
      /// <param name="y">Координата пикселя растра по вертикали.</param>
      public PixelCoordsKey (int x, int y)
      {
        X = x;
        Y = y;
      } // PixelCoordsKey


      public override bool Equals (object obj)
      {
        bool result = false;
        if (obj is PixelCoordsKey)
        {
          PixelCoordsKey value = (PixelCoordsKey)obj;
          result = X.Equals(value.X) && Y.Equals(value.Y); 
        }
        return result;
      } // Equals


      public override int GetHashCode ()
      {
        return X.GetHashCode()^Y.GetHashCode();
      } // GetHashCode


      public override string ToString ()
      {
        return $"({X},{Y})";
      } // ToString

    } // PixelCoordsKey


    /// <summary>
    /// Конструктор.
    /// </summary>
    public RasterAffinityDetector () :base()
    {
      AffinityBlockBuilder = DefaultAffinityBlockBuilder;
      AffinityBlockCorrelator = DefaultAffinityBlockCorrelator;
    } // RasterAffinityDetector


    /// <summary>
    /// Дефолтовая реализация разбиения растра на устойчивые блоки, каждый из которых соответствует одному пикселю.
    /// Эту реализацию можно переопределить либо в наследниках, либо во внешних вызывающих классах. 
    /// </summary>
    /// <param name="features">Совокупность реперных характеристик, в качестве которых в данной реализации выступают пиксели растра.</param>
    /// <returns>Совокупность отдельных устойчивых блоков реперных характеристик. В качестве таких блоков в данной реализации выступают пиксели растра.</returns>
    protected virtual IEnumerator<IAffinityBlock> DefaultAffinityBlockBuilder (IEnumerable<Raster.Pixel> features)
    {
      return new PixelAffinityBlockBuilder(features.GetEnumerator(), GetPixelKey, GetPixelValue);
    } // DefaultAffinityBlockBuilder


    /// <summary>
    /// Дефолтовая реализация метода идентификации устойчивых блоков, в качестве которых в данной базовой реализации выступают сами пиксели растра.
    /// Идентификация производится по координатам пикселей.
    /// </summary>
    /// <param name="pixel">Пиксель растра, соответствующий устойчивому блоку реперных характеристик.</param>
    /// <returns>Совокупность ключей, однозначно идентифицирующих данный устойчивый блок через координаты пикселя, являющегося этим устойчивым блоком.</returns>
    protected virtual object GetPixelKey (Raster.Pixel pixel)
    {
      return new PixelCoordsKey(pixel.X, pixel.Y);
    } // GetPixelKey


    /// <summary>
    /// Дефолтовая реализация метода получения значений реперных характеристик устойчивого блока, в качестве которого в данной реализации выступает пиксель растра.
    /// Реперной характеристикой в этой дефолтовой реализации будет цвет пикселя.
    /// </summary>
    /// <param name="pixel">Пиксель растра, соответствующий устойчивому блоку реперных характеристик.</param>
    /// <returns>Значение реперных характеристик пикселя. В данной базовой реализации - цвет пикселя.</returns>
    protected virtual object GetPixelValue (Raster.Pixel pixel)
    {
      return pixel.ColorRGB;
    } // GetPixelValue


    /// <summary>
    /// Дефолтовая реализация определения степени корреляции между устойчивыми блоками реперных характеристик, каждый из которых соответствует одному пикселю.
    /// Корреляция определяется по совпадению цветов пикселей:
    ///   0 - цвета соответствующих пикселей не совпадают,
    ///   1 - цвета соответствующих пикселей совпадают.
    /// </summary>
    /// <param name="affinityBlock1">Один из блоков реперных характеристик (пиксель), между которыми определяется степень корреляции.</param>
    /// <param name="affinityBlock2">Второй из блоков реперных характеристик (пиксель), между которыми определяется степень корреляции.</param>
    /// <returns>Степень корреляции между блоками реперных характеристик (пикселями).</returns>
    protected virtual Correlation DefaultAffinityBlockCorrelator (IAffinityBlock affinityBlock1, IAffinityBlock affinityBlock2)
    {
      Correlation result;
      Int32 color1 = (Int32)affinityBlock1.Value;
      Int32 color2 = (Int32)affinityBlock2.Value;
      if (color1 == color2) result = new Correlation(1, 1);
        else result = new Correlation(0, 1);
      return result;
    } // DefaultAffinityBlockCorrelator


  } // class RasterAffinityDetector

} // namespace RusLat.Tools.AffinityDetectors
