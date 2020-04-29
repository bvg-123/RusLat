using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Реализация детектора сходства растров по пиксельной разнице нормированной цветовой яркости. 
  /// </summary>
  public class EDiffRasterAffinityDetector :RasterAffinityDetector
  {
    /// <summary>
    /// Базовый диапазон яркостей пикселей фона растра, определяемый пока по первому реперному пикселю.
    /// TODO: Как вариант можно определять по соотношению пикселей разной яркости и смотреть, в каком диапазоне яркостей будет наибольшее количество пикселей, тот диапазон и считать фоном.
    /// </summary>
    private double[] BackgroundBase;
    
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public EDiffRasterAffinityDetector () :base()
    {
    } // EDiffRasterAffinityDetector


    /// <summary>
    /// Реализация метода получения значений реперных характеристик устойчивого блока, в качестве которого в данной реализации выступает пиксель растра.
    /// Реперной характеристикой в этой реализации является яркость пикселя в нормированном цветовом пространстве (0-1).
    /// </summary>
    /// <param name="pixel">Пиксель растра, соответствующий устойчивому блоку реперных характеристик.</param>
    /// <returns>Значение реперных характеристик пикселя. В данной реализации - яркость пикселя в нормированном цветовом пространстве (0-1).</returns>
    protected override object GetPixelValue (Raster.Pixel pixel)
    {
      return pixel.E;
    } // GetPixelValue


    /// <summary>
    /// Реализация определения степени корреляции между устойчивыми блоками реперных характеристик, каждый из которых соответствует одному пикселю,
    /// основанная на разнице яркостей соответствующих точек сравниваемых растров. 
    /// </summary>
    /// <param name="affinityBlock1">Один из блоков реперных характеристик (пиксель), между которыми определяется степень корреляции.</param>
    /// <param name="affinityBlock2">Второй из блоков реперных характеристик (пиксель), между которыми определяется степень корреляции.</param>
    /// <returns>Степень корреляции между блоками реперных характеристик (пикселями):
    ///   0 - корреляция отсутствует,
    ///   1 - полная корреляция.
    ///   Чем ближе значение к 1, тем корреляция полнее.
    /// </returns>
    protected override Correlation DefaultAffinityBlockCorrelator (IAffinityBlock affinityBlock1, IAffinityBlock affinityBlock2)
    {
      PixelCoordsKey key = (PixelCoordsKey)affinityBlock1.Key;
      if ((key.X == 0) && (key.Y == 0)) BackgroundBase = new double[] { (double)affinityBlock1.Value-0.1, (double)affinityBlock1.Value+0.1 };
      double e1 = (double)affinityBlock1.Value;
      double e2 = (double)affinityBlock2.Value;
      double importance;
      double affinity;
      if ((BackgroundBase != null) && (BackgroundBase[0] <= e1) && (e1 <= BackgroundBase[1]))
      {
        // Попали по первому определяющему растру в фон. Попадание в фон дает наименьшую степень значимости в корреляции сравниваемых блоков.
        affinity = 0;
        importance = 0;
      }
      else if ((BackgroundBase[0] <= e2) && (e2 <= BackgroundBase[1]))
      {
        // Попали в фон по второму растру, а по первому определяющему попали не в фон. Полное отсутствие корреляции.
        affinity = 0;
        importance = 1;
      }
      else
      {
        // В обоих растрах попали не в фон. Такие блоки рассматриваем, как определяющие.
        affinity = 1-Math.Abs(e1-e2);
        importance = 1;
      }
      return new Correlation(affinity, importance);
    } // DefaultAffinityBlockCorrelator


  } // class EDiffRasterAffinityDetector

} //namespace RusLat.Tools.AffinityDetectors
