using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools.AffinityDetectors.Exceptions
{
  /// <summary>
  /// Исключение, связанное с некорректными значениями входных свойств детектора сходства.
  /// </summary>
  public class AffinityDetectorException :Exception
  {
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AffinityDetectorException () : base()
    {
    } // AffinityDetectorException


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст ислючения.</param>
    public AffinityDetectorException (string message) : base(message)
    {
    } // AffinityDetectorException


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Текст исключения.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public AffinityDetectorException (string message, Exception innerException) : base(message, innerException)
    {
    } // AffinityDetectorException


  } // class AffinityDetectorException

} // namespace RusLat.Tools.AffinityDetectors.Exceptions
