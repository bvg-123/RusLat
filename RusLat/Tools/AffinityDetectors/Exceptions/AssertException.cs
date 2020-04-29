using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors.Exceptions
{
  /// <summary>
  /// Исключение, связанное с нарушением логики работы или взаимодействия частей приложения,
  /// приводящей к возникновению невозможной ситуации.
  /// </summary>
  public class AssertException :Exception
  {
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AssertException () : base("ASSERT")
    {
    } // AssertException


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст ислючения.</param>
    public AssertException (string message) : base("ASSERT: "+message)
    {
    } // AssertException


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст исключения.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public AssertException (string message, Exception innerException) : base("ASSERT: "+message, innerException)
    {
    } // AssertException


  } // class AssertException

} // namespace RusLat.Tools.AffinityDetectors.Exceptions
