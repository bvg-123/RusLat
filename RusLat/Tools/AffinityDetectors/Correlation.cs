using System;

namespace RusLat.Tools.AffinityDetectors
{
  public class Correlation
  {
    /// <summary>
    /// Степень корреляции блоков реперных характеристик сравниваемых объектов (0-1):
    ///   0 - отсутствие корреляции
    ///   1 - полная корреляция
    /// </summary>
    public double Value { get; private set; }


    /// <summary>
    /// Уровень значимости степени корреляции блоков реперных характеристик сравниваемых объектов (0-1):
    ///   0 - коррелируемые блоки не влияют на степень сходства объектов,
    ///   1 - коррелируемые блоки максимально влияют на степень сходства объектов.
    /// </summary>
    public double Importance { get; private set; }


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Степень корреляции блоков реперных характеристик сравниваемых объектов (0-1):
    ///   0 - отсутствие корреляции
    ///   1 - полная корреляция
    /// </param>
    /// <param name="importance">Уровень значимости степени корреляции блоков реперных характеристик сравниваемых объектов (0-1):
    ///   0 - коррелируемые блоки не влияют на степень сходства объектов,
    ///   1 - коррелируемые блоки максимально влияют на степень сходства объектов.
    /// </param>
    public Correlation (double value, double importance)
    {
      Value = value;
      Importance = importance;
    } // Correlation


    public override string ToString ()
    {
      return FormattableString.Invariant($"Value={Value:0.00}, Importance={Importance:0.00}");
    } // ToString


  } // class Correlation

} // namespace RusLat.Tools.AffinityDetectors