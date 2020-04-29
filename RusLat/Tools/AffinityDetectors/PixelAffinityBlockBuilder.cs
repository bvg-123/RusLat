using System;
using System.Collections;
using System.Collections.Generic;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Класс для разбиения совокупности пикселей растра на устойчивые блоки, в качестве которых выступают те же самые пиксели.
  /// </summary>
  public class PixelAffinityBlockBuilder :BaseAffinityBlockBuilder<Raster.Pixel>
  {
    private class PixelAffinityBlock :IAffinityBlock
    {
      /// <summary>
      /// Текущий пиксель растра.
      /// </summary>
      private Raster.Pixel Pixel;
      
      /// <summary>
      /// Метод получения ключа, представляющего собой положение пикселя в растре в виде его координат.
      /// </summary>
      private Func<Raster.Pixel, object> GetKey;

      /// <summary>
      /// Метод получения совокупности значений реперных характеристик пикселя в растре.
      /// </summary>
      private Func<Raster.Pixel, object> GetValue;

      /// <summary>
      /// Возвращает положение пикселя в растре в виде его координат.
      /// </summary>
      public object Key { get { return GetKey(Pixel); }}

      /// <summary>
      /// Возвращает значение яркости пикселя, в качестве единственной реперной характеристики данной реализации устойчивого блока.
      /// </summary>
      public object Value { get { return GetValue(Pixel); }}


      /// <summary>
      /// Конструктор.
      /// </summary>
      /// <param name="pixel">Пиксель растра, представляющий данный устойчивый блок.
      /// Важно! При переборе пикселей растра через RasterEnumerator экземпляр текущего пикселя не меняется, меняются только значения его свойств.
      /// Это существенно влияет на реализацию получения значений Keys и Values. По сути данный класс является просто оберткой над классом пикселя,
      /// обеспечивающей требуемую степень абстракции.</param>
      /// <param name="getKey">Метод получения ключа, представляющего собой положение пикселя в растре в виде его координат.</param>
      /// <param name="getValue">Метод получения совокупности значений реперных характеристик пикселя в растре.</param>
      public PixelAffinityBlock (Raster.Pixel pixel, Func<Raster.Pixel, object> getKey, Func<Raster.Pixel, object> getValue)
      {
        Pixel = pixel;
        GetKey = getKey;
        GetValue = getValue;
      } // PixelAffinityBlock


    } // class PixelAffinityBlock




    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="source">Исходная совокупность пикселей, разбиваемых на устойчивые блоки.</param>
    /// <param name="getKey">Метод получения ключа, представляющего собой положение пикселя в растре в виде его координат.</param>
    /// <param name="getValue">Метод получения совокупности значений реперных характеристик пикселя в растре.</param>
    public PixelAffinityBlockBuilder (IEnumerator<Raster.Pixel> source, Func<Raster.Pixel, object> getKey, Func<Raster.Pixel, object> getValue)
    {
      Source = source;
      Current = new PixelAffinityBlock(Source.Current, getKey, getValue);
    } // PixelAffinityBlockBuilder


  } // class PixelAffinityBlockBuilder

} // namespace RusLat.Tools.AffinityDetectors
