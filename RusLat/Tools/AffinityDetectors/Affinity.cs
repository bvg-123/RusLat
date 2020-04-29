using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors
{
  /// <summary>
  /// Степень сходства.
  /// </summary>
  public class Affinity
  {
    /// <summary>
    /// Нормированная величина степени сходства:
    ///   0 - совершенно не похоже,
    ///   1 - полностью идентично.
    /// </summary>
    public double Value { get; private set; }

    /// <summary>
    /// Степень достоверности величины сходства:
    ///   0 - совершенно сомнительно,
    ///   1 - весьма вероятно.
    /// </summary>
    public double Reliability { get; private set; }


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Величина степени сходства (0-1).</param>
    /// <param name="reliability">Степень достоверности ввеличины сходства (0-1). Необязательный параметр.
    /// Если не указан, то предполагается весьма вкроятное надежное сходство.</param>
    public Affinity (double value, double reliability = 1)
    {
      Value = value;
      Reliability = reliability;
    } // Affinity


    /// <summary>
    /// Определяет равенство степеней сходства по их значениям. 
    /// </summary>
    /// <param name="obj">Степерь сходства с которой производится сравнение.</param>
    /// <returns>Результат совпадения текущей степень сходства с указнной.</returns>
    public override bool Equals (object obj)
    {
      bool result = false;
      if ((obj != null) && (obj is Affinity))
      {
        Affinity affinity = (Affinity)obj;
        result = Value.Equals(affinity.Value) && Reliability.Equals(affinity.Reliability);
      }
      return result;
    } // Equals


    public override int GetHashCode ()
    {
      return Value.GetHashCode()^Reliability.GetHashCode();
    } // GetHashCode


    /// <summary>
    /// Возвращает строковое представление степени сходства.
    /// </summary>
    /// <returns>Строковое представление степени сходства.</returns>
    public override string ToString ()
    {
      return ((FormattableString)$"{Value:0.00} (rel={Reliability:0.00})").ToString(CultureInfo.InvariantCulture);
    } // ToString


  } // class Affinity.AffinityDetectors

} // namespace RusLat.Tools
