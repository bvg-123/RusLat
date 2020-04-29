using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Базовый абстрактный класс для разбиения совокупности реперных характеристик на устойчивые блоки.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class BaseAffinityBlockBuilder<T> :IEnumerator<IAffinityBlock>
  {
    /// <summary>
    /// Исходная совокупность реперных характеристик, разбиваемая на устойчивые блоки.
    /// </summary>
    protected IEnumerator<T> Source;

    /// <summary>
    /// Очередной блок.
    /// В данной реализации используется тот факт, что по ходу перебора пикселей растра Source экземпляр текущего пикселя не меняется,
    /// а меняются лишь значения его свойств, поэтому в целях оптимизации использования кучи запоминаем ссылку на этот экземпляр 
    /// пикселя и используем в дальнейшем только его свойства.
    /// </summary>
    public IAffinityBlock Current { get; protected set; }

    object IEnumerator.Current { get { return Current; } }


    public bool MoveNext()
    {
      return Source.MoveNext();
    } // MoveNext


    public void Reset()
    {
      Source.Reset();
    } // Reset


    public void Dispose()
    {
      Source = null;
    } // Dispose


  } // class BaseAffinityBlockBuilder

} // namespace RusLat.Tools.AffinityDetectors
