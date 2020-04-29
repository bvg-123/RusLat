using RusLat.Tools.AffinityDetectors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools
{
  /// <summary>
  /// Отладчик приложения.
  /// </summary>
  public class Debugger :PropertyChangedBase
  {
    /// <summary>
    /// Текущий экземпляр отладчика.
    /// </summary>
    public static Debugger Current { get { return (Debugger)App.Current.Resources["Debugger"]; }}

    /// <summary>
    /// Область маски.
    /// </summary>
    public string MaskArea { get { return _MaskArea; } protected set { SetProperty(ref _MaskArea, value); }}
    private string _MaskArea;

    /// <summary>
    /// Изображение маски.
    /// </summary>
    public System.Drawing.Bitmap Mask { get { return _Mask; } protected set { SetProperty(ref _Mask, value); }}
    private System.Drawing.Bitmap _Mask;

    /// <summary>
    /// Изображение последнего скана.
    /// </summary>
    public System.Drawing.Bitmap Scan { get { return _Scan; } protected set { SetProperty(ref _Scan, value); } }
    private System.Drawing.Bitmap _Scan;

    /// <summary>
    /// Растр маски.
    /// </summary>
    public Raster Raster { get { return _Raster; } protected set { SetProperty(ref _Raster, value); }}
    private Raster _Raster;

    /// <summary>
    /// Степень сходства маски с текущим изображением языка ввода.
    /// </summary>
    public Affinity Affinity { get { return _Affinity; } protected set { SetProperty(ref _Affinity, value); } }
    private Affinity _Affinity;


    /// <summary>
    /// Интерфейс для получения значений матрицы корреляций детектора сходства.
    /// </summary>
    public ICorrelation Correlation { get { return _Correlation; } protected set { SetProperty(ref _Correlation, value); } }
    private ICorrelation _Correlation;


    /// <summary>
    /// Конструктор.
    /// </summary>
    public Debugger ()
    {
    } // Debugger


    /// <summary>
    /// Трассирует в отладочное окно указанное значение области маски.
    /// </summary>
    /// <param name="bounds">Область маски.</param>
    public void TraceBounds (System.Drawing.Rectangle bounds)
    {
      MaskArea = $"{bounds.X},{bounds.Y} {bounds.Width}x{bounds.Height}";
    } // TraceBounds


    /// <summary>
    /// Трассирует в отладочное окно указанную маску.
    /// </summary>
    /// <param name="bitmap">Трассируемая маска.</param>
    public void TraceMask (System.Drawing.Bitmap bitmap)
    {
      Mask = bitmap;
    } // TraceMask


    /// <summary>
    /// Трассирует в отладочное окно указанный скан.
    /// </summary>
    /// <param name="bitmap">Трассируемый скан.</param>
    public void TraceScan (System.Drawing.Bitmap bitmap)
    {
      Scan = bitmap;
    } // TraceScan


    /// <summary>
    /// Трассирует в отладочное окно указанный растр.
    /// </summary>
    /// <param name="raster">Трассируемый растр.</param>
    public void TraceRaster (Raster raster)
    {
      Raster = raster;
    } // TraceRaster


    /// <summary>
    /// Трассирует в отладочное окно указанную степень сходства.
    /// </summary>
    /// <param name="affinity">Трассируемая степень сходства.</param>
    public void TraceAffinity (Affinity affinity)
    {
      Affinity = affinity;
    } // TraceAffinity


    public void TraceCorrelations (ICorrelation correlation)
    {
      Correlation = correlation;
    } // TraceCorrelations

  } // class Debugger

} // namespace RusLat.Tools
