using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Делегат для разбиения совокупности реперных характеристик на отдельные устойчивые блоки характеристик по тому или иному критерию.
  /// </summary>
  /// <typeparam name="T">Тип индивидуальных реперных характеристик. Например, точка растра.</typeparam>
  /// <param name="features">Совокупность реперных характеристик.</param>
  /// <returns>Совокупность отдельных устойчивых блоков реперных характеристик.</returns>
  public delegate IEnumerator<IAffinityBlock> AffinityBlockBuilderDelegate<T> (IEnumerable<T> features);

  /// <summary>
  /// Делегат для определения степени корреляции между устойчивыми блоками реперных характеристик. 
  /// </summary>
  /// <param name="affinityBlock1">Один из блоков реперных характеристик, между которыми определяется степень корреляции.</param>
  /// <param name="affinityBlock2">Второй из блоков реперных характеристик, между которыми определяется степень корреляции.</param>
  /// <returns>Степень корреляции между блоками реперных характеристик:
  ///   0 - корреляция отсутствует,
  ///   1 - полная корреляция.
  ///   Чем ближе значение к 1, тем корреляция полнее.
  /// </returns>
  public delegate Correlation AffinityBlockCorrelatorDelegate (IAffinityBlock affinityBlock1, IAffinityBlock affinityBlock2);


  /// <summary>
  /// Интерфейс для выявления сходства объектов по совокупности реперных характеристик определенного типа. 
  /// </summary>
  public interface IAffinityDetector<T>
  {
    /// <summary>
    /// Метод разбиения совокупности реперных характеристик на отдельные устойчивые блоки характеристик по тому или иному критерию.
    /// </summary>
    AffinityBlockBuilderDelegate<T> AffinityBlockBuilder { get; set; }

    /// <summary>
    /// Метод определения степени корреляции между блоками реперных характеристик.
    /// </summary>
    AffinityBlockCorrelatorDelegate AffinityBlockCorrelator { get; set; }

    /// <summary>
    /// Инициализирует механизм выявления сходства, например, загружает нейросеть и модели типичных растров.
    /// </summary>
    void Init ();

    /// <summary>
    /// Деинициализирует механизм выявления сходства, например, обновляет библиотеку моделей типичных растров или фиксирует улучшенные коэффициенты нейросети.
    /// </summary>
    void Done ();

    /// <summary>
    /// Определяет степень сходства двух заданных объектов.
    /// </summary>
    /// <param name="features1">Совокупность реперных характеристик одного из сравниваемых объектов.</param>
    /// <param name="features2">Совокупность реперных характеристик другого сравниваемого объекта.</param>
    /// <returns>Степень сходства заданных объектов.</returns>
    Affinity Detect (IEnumerable<T> features1, IEnumerable<T> features2);

    /// <summary>
    /// Совершенствует механизм выявления сходства путем обучения.
    /// </summary>
    /// <param name="affinity">Эталонное значение степени сходства объектов, указанных в последнем вызове метода Detect.</param>
    void Learn (Affinity affinity);

  } // interface IAffinityDetector

} // namespace RusLat.Tools.AffinityDetectors
