using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RusLat.Tools.AffinityDetectors.Exceptions;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Базовый класс абстрактного детектора сходства.
  /// </summary>
  /// <typeparam name="T">Тип реперных характеристик сравниваемых объектов.</typeparam>
  public abstract class BaseAffinityDetector<T> :IAffinityDetector<T>, ICorrelation
  {
    /// <summary>
    /// Метод разбиения совокупности реперных характеристик на отдельные устойчивые блоки характеристик по тому или иному критерию.
    /// </summary>
    public AffinityBlockBuilderDelegate<T> AffinityBlockBuilder { get; set; }

    /// <summary>
    /// Метод определения степени корреляции между блоками реперных характеристик.
    /// </summary>
    public AffinityBlockCorrelatorDelegate AffinityBlockCorrelator { get; set; }

    /// <summary>
    /// Совокупность блоков реперных характеристик, построенных из совокупности реперных характеристик одного из сравниваемых объектов
    /// </summary>
    protected IEnumerator<IAffinityBlock> AffinityBlocks1 { get; set; }

    /// <summary>
    /// Совокупность блоков реперных характеристик, построенных из совокупности реперных характеристик второго из сравниваемых объектов
    /// </summary>
    protected IEnumerator<IAffinityBlock> AffinityBlocks2 { get; set; }

    /// <summary>
    /// Выявленная степень сходства последних сравниваемых объектов. 
    /// </summary>
    protected Affinity DetectedAffinity { get; set; }

    /// <summary>
    /// Матрица корреляций соответствующих друг другу устойчивых блоков реперных характеристик сравниваемых последними объектов.
    /// </summary>
    protected Dictionary<object, Correlation> CorrelationMatrix { get; private set; }


    /// <summary>
    /// Инициализирует механизм выявления сходства, например, загружает нейросеть и модели типичных растров.
    /// </summary>
    public virtual void Init ()
    {
      CorrelationMatrix = new Dictionary<object, Correlation>();
    } // Init


    /// <summary>
    /// Деинициализирует механизм выявления сходства, например, обновляет библиотеку моделей типичных растров или фиксирует улучшенные коэффициенты нейросети.
    /// </summary>
    public virtual void Done ()
    {
      if (AffinityBlocks1 != null)
      {
        AffinityBlocks1.Dispose();
        AffinityBlocks1 = null;
      }
      if (AffinityBlocks2 != null)
      {
        AffinityBlocks2.Dispose();
        AffinityBlocks2 = null;
      }
      DetectedAffinity = null;
    } // Done


    /// <summary>
    /// Определяет степень сходства двух заданных объектов.
    /// </summary>
    /// <param name="features1">Совокупность реперных характеристик одного из сравниваемых объектов.</param>
    /// <param name="features2">Совокупность реперных характеристик другого сравниваемого объекта.</param>
    /// <returns>Степень сходства заданных объектов.</returns>
    public virtual Affinity Detect (IEnumerable<T> features1, IEnumerable<T> features2)
    {
      if (CorrelationMatrix == null) throw new AssertException("Отсутствует матрица корреляций, т.к. не был вызван метод Init базового класса детектора сходства.");
      if (AffinityBlockBuilder == null) throw new AffinityDetectorException("В свойстве AffinityBlockBuilder не задан метод разбиения реперных характеристик сравниваемых объектов на устойчивые блоки реперных характеристик.");
      if (AffinityBlockCorrelator == null) throw new AffinityDetectorException("В свойстве AffinityBlockCorrelator не задан метод определения степени корреляции между устойчивыми блоками реперных характеристик сравниваемых объектов.");
      AffinityBlocks1 = AffinityBlockBuilder(features1);
      AffinityBlocks2 = AffinityBlockBuilder(features2);
      int size = 0;
      int diffs = 0;
      double reliability = 0;
      while (AffinityBlocks1.MoveNext() && AffinityBlocks2.MoveNext())
      {
        Correlation correlation = AffinityBlockCorrelatorInvoke(AffinityBlocks1.Current, AffinityBlocks2.Current);
        if (correlation.Importance > 0.5)
        {
          // Учитываем только значимые блоки и пропускаем малозначительные, например, относящиеся к фону.
          if (correlation.Value > 0.5)
          {
            // Соответствующие устойчивые блоки реперных характеристик коррелируют между собой.
            reliability = reliability+correlation.Value;
          }
          else
          {
            // Соответствующие устойчивые блоки реперных характеристик не коррелируют между собой.
            diffs++;
          }
          size++;
        }
      }
      if (size == diffs)
      {
        // Сравниваемые объекты гарантированно не похожи между собой.
        reliability = 1;
        DetectedAffinity = new Affinity(0, reliability);
      }
      else
      {
        // Сравниваемые объекты имеют некоторое сходство по своим реперным характеристикам.
        reliability = reliability/(size-diffs);
        double affinity = 1-(double)diffs/(double)size;
        DetectedAffinity = new Affinity(affinity, reliability);
      }
      return DetectedAffinity;
    } // Detect


    /// <summary>
    /// Совершенствует механизм выявления сходства путем обучения.
    /// </summary>
    /// <param name="affinity">Эталонное значение степени сходства объектов, указанных в последнем вызове метода Detect.</param>
    public virtual void Learn (Affinity affinity)
    {
      if (DetectedAffinity == null) throw new AffinityDetectorException("Обучение детектора сходства возможно только после проведения нулевой итерации сравнения объектов.");
      
      // В базовой абстрактной реализации никакие методики обучения не поддерживаются.
    } // Learn


    /// <summary>
    /// Возвращает степень корреляции между устойчивыми блоками реперных характеристик, получаемую через AffinityBlockCorrelator.
    /// Фиксирует полученное значение корреляции для его получения в методе Correlation.
    /// </summary>
    /// <param name="affinityBlock1">Один из блоков реперных характеристик, между которыми определяется степень корреляции.</param>
    /// <param name="affinityBlock2">Второй из блоков реперных характеристик, между которыми определяется степень корреляции.</param>
    /// <returns>Степень корреляции между блоками реперных характеристик</returns>
    protected virtual Correlation AffinityBlockCorrelatorInvoke (IAffinityBlock affinityBlock1, IAffinityBlock affinityBlock2)
    {
      Correlation correlation = AffinityBlockCorrelator(affinityBlock1, affinityBlock2);
      ValidateKeys(affinityBlock1.Key, affinityBlock2.Key);
      CorrelationMatrix[affinityBlock1.Key] = correlation;
      return correlation;
    } // AffinityBlockCorrelatorInvoke


    /// <summary>
    /// Проверяет равенство ключей соответствующих друг другу блоков.
    /// Если ключи отличаются, значит в логике разбиения на блоки имеется ошибка,
    /// т.к. соотвествующие друг другу блоки сравниваемых объектов должны иметь совпадающие ключи. 
    /// </summary>
    /// <param name="key1">Ключ некоторого блока первого объекта.</param>
    /// <param name="key2">Ключ соответствующего ему блока второго объекта.</param>
    private void ValidateKeys (object key1, object key2)
    {
      try
      {
        if ((key1 == null) || (key2 == null)) throw new AssertException();
        if (!key1.Equals(key2)) throw new AssertException();
      }
      catch (AssertException)
      {
        throw new AssertException($"Ключи соответствующих друг другу блоков сравниваемых объектов должны совпадать:\r\nkey1={key1}\r\nkey2={key2}");
      }
    } // ValidateKeys


    /// <summary>
    /// Возвращает степень корреляции устойчивых блоков реперных характеристик последних сравниваемых объектов.
    /// </summary>
    /// <param name="key">Ключ, однозначно определяющий конкретные устойчивые блоки соответствующих друг другу реперных характеристик последних сравниваемых объектов,
    /// для которых возвращается степень их корреляции межлу собой.
    /// В качестве такого ключа могут, например, выступать координаты точек растра в случае, когда в качестве устойчивых блоков выступают сами же писксели, а в качестве их реперных характеристик 
    /// выступает, например, их яркость.</param>
    /// <returns>Степень корреляции устойчивых блоков реперных характеристик последних сравниваемых объектов (0-1):
    /// 0 - полное отсутствие корреляции,
    /// 1 - полная корреляция.
    /// Чем ближе к 1 значение, то выше степень корреляции и, соответственно, сходства блоков.</returns>
    public Correlation GetCorrelation (object key)
    {
      Correlation result;
      result = CorrelationMatrix[key];
      return result;
    } // GetCorrelation


  } // class BaseAffinityDetector

} // namespace RusLat.Tools.AffinityDetectors
