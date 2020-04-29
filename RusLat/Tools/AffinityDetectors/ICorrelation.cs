using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Интерфейс степени корреляции устойчивых блоков реперных характеристик сравниваемых объектов.
  /// </summary>
  public interface ICorrelation
  {
    /// <summary>
    /// Возвращает степень корреляции устойчивых блоков реперных характеристик последних сравниваемых объектов.
    /// </summary>
    /// <param name="keys">Ключ, однозначно определяющий конкретные устойчивые блоки соответствующих друг другу реперных характеристик последних сравниваемых объектов,
    /// для которых возвращается степень их корреляции межлу собой.
    /// В качестве такого ключа могут, например, выступать координаты точек растра в случае, когда в качестве устойчивых блоков выступают сами же писксели, а в качестве их реперных характеристик 
    /// выступает, например, их яркость.</param>
    /// <returns>Степень корреляции устойчивых блоков реперных характеристик последних сравниваемых объектов.</returns>
    Correlation GetCorrelation (object key);

  } // interface ICorrelation<T>

} // namespace RusLat.Tools.AffinityDetectors
