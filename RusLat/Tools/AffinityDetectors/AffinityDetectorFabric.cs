using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Фабрика для создания экземпляров детекторов сходства объектов по реперным характеристикам заданного типа T.
  /// </summary>
  public class AffinityDetectorFabric<T> where T :Raster.Pixel
  {
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AffinityDetectorFabric ()
    {
    } // AffinityDetectorFabric


    /// <summary>
    /// Возвращает экземпляр детектора сходства.
    /// </summary>
    /// <returns>Экземпляр детектора сходства.</returns>
    public IAffinityDetector<T> GetDetector ()
    {
      return (IAffinityDetector<T>)new EDiffRasterAffinityDetector();
    } // GetDetector


  } // class AffinityDetectorFabric<T>

} // namespace RusLat.Tools.AffinityDetectors
