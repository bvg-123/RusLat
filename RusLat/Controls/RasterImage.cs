using RusLat.Tools;
using RusLat.Tools.AffinityDetectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RusLat.Controls
{
  public class RasterImage :FrameworkElement
  {
    /// <summary>
    /// Коэффициент увеличения отображаемого изображения растра.
    /// </summary>
    private const int Zoom = 4;

    /// <summary>
    /// Зазор между отображаемыми элементами растра в DIP-ах.
    /// </summary>
    private const int Space = 1;

    /// <summary>
    /// Цвет межпиксельного зазора. 
    /// </summary>
    private Brush BackgroundBrush;
    
    /// <summary>
    /// Цвет для рамки отображаемого пикселя растра, над которым находится указатель мыши.
    /// </summary>
    private Pen SelectionPen;
    
    /// <summary>
    /// Область отображения пикселя растра, над которой находится указатель мыши.
    /// </summary>
    private Rect Selection;

    /// <summary>
    /// Отображаемый растр.
    /// </summary>
    public Raster Source { get { return (Raster)GetValue(SourceProperty); } set { SetValue(SourceProperty, value); }}
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Raster), typeof(RasterImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender|FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Интерфейс для получения степени корреляции отдельных пикселей маски с текущим изображением состояния языка ввода. 
    /// </summary>
    public ICorrelation Correlation { get { return (ICorrelation)GetValue(CorrelationProperty); } set { SetValue(CorrelationProperty, value); } }
    public static readonly DependencyProperty CorrelationProperty = DependencyProperty.Register("Correlation", typeof(ICorrelation), typeof(RasterImage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Цвет пикселя растра, над которым находится указатель мыши.
    /// </summary>
    public Raster.Pixel SelectedPixel { get { return (Raster.Pixel)GetValue(SelectedPixelProperty); } set { SetValue(SelectedPixelProperty, value); }}
    public static readonly DependencyProperty SelectedPixelProperty = DependencyProperty.Register("SelectedPixel", typeof(Raster.Pixel), typeof(RasterImage), new PropertyMetadata(null));

    /// <summary>
    /// Коордитнаты пикселя растра, над которым находится указатпель мыши.
    /// </summary>
    public System.Drawing.Point SelectedCoords { get { return (System.Drawing.Point)GetValue(SelectedCoordsProperty); } set { SetValue(SelectedCoordsProperty, value); }}
    public static readonly DependencyProperty SelectedCoordsProperty = DependencyProperty.Register("SelectedCoords", typeof(System.Drawing.Point), typeof(RasterImage), new PropertyMetadata(null));

    /// <summary>
    /// Степень корреляции пикселя растра маски, над которым находится указатель мыши, с соответствующим пикселем последнего скана.
    /// </summary>
    public Correlation SelectedCorrelation { get { return (Correlation)GetValue(SelectedCorrelationProperty); } set { SetValue(SelectedCorrelationProperty, value); }}
    public static readonly DependencyProperty SelectedCorrelationProperty = DependencyProperty.Register("SelectedCorrelation", typeof(Correlation), typeof(RasterImage), new PropertyMetadata(null));


    /// <summary>
    /// Конструктор.
    /// </summary>
    public RasterImage ()
    {
      BackgroundBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00));
      BackgroundBrush.Freeze();
      SelectionPen = new Pen(new SolidColorBrush(Colors.Yellow), 1);
      SelectionPen.Freeze();
    } // RasterImage


    /// <summary>
    /// Возвращает размер изображения-источника в DIP-ах.
    /// </summary>
    /// <returns>Возвращает размер изображения-источника в DIP-ах.</returns>
    private Size GetSourceSizeDIPed ()
    {
      Size result;
      if (Source != null)
      {
        result = new Size(Source.Width*Zoom/DPI.Scale+Source.Width*Space+1, Source.Height*Zoom/DPI.Scale+Source.Height*Space+1);
      }
      else
      {
        result = new Size(20, 20);
      }
      return result;
    } // GetSourceSizeDIPed


    protected override Size MeasureOverride (Size availableSize)
    {
      return GetSourceSizeDIPed();
    } // MeasureOverride


    protected override void OnRender (DrawingContext dc)
    {
      base.OnRender(dc);
      if (Source != null)
      {
        Size clientArea = GetSourceSizeDIPed();
        Rect r = new Rect(0, 0, clientArea.Width, clientArea.Height);
        dc.DrawRectangle(BackgroundBrush, null, r);
        int y = 0;
        int x = 0;
        r.X = Space;
        r.Y = Space;
        double szDip = Zoom/DPI.Scale;
        r.Width = szDip;
        r.Height = szDip;
        foreach (Raster.Pixel pixel in Source)
        {
          if (x == Source.Width)
          {
            x = 0;
            y++;
            r.X = Space;
            r.Y = r.Y+szDip+Space;
          }
          dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(pixel.Red, pixel.Green, pixel.Blue)), null, r);
          if (Correlation != null)
          {
            // Обводим отображаемые пиксели цветом, зависящим от степени корреляции.
            Correlation correlation = Correlation.GetCorrelation(new RasterAffinityDetector.PixelCoordsKey(x, y));
            if (correlation.Importance < 0.5)
            {
              // Малозначимые блоки никак не выделяем.
            }
            else
            {
              // Значащие блоки с высокой степень корреляции отмечаем красной точкой, а с низкой степенью корреляции - темно-синей. 
              //Rect rBounds = new Rect(r.X-Space, r.Y-Space, r.Width+2*Space, r.Height+2*Space);
              Rect rBounds = new Rect(r.X+r.Width/2, r.Y+r.Height/2, 0.5, 0.5);
              Pen pen;
              if (correlation.Value > 0.5)
              {
                pen = new Pen(new SolidColorBrush(Colors.Red), Space);
              }
              else
              {
                pen = new Pen(new SolidColorBrush(Colors.Blue), Space);
              }
              dc.DrawRectangle(null, pen, rBounds);
            }
          }
          r.X = r.X+szDip+Space;
          x++;
        }
        if (Selection != null)
        {
          // Выделяем желтой рамкой текущий выбранный пиксель, над которым находится указатель мыши.
          dc.DrawRectangle(null, SelectionPen, Selection);
        }
      }
    } // OnRender


    protected override void OnMouseMove (MouseEventArgs e)
    {
      if (Source != null)
      {
        Point p = e.GetPosition(this);
        double szDip = Zoom/DPI.Scale;  // размер отображаемой точки растра в DIP-ах
        int x = (int)Math.Truncate((p.X-Space)/(szDip+Space)); // координата выбранной мышью точки отображаемого растра по горизонтали
        int y = (int)Math.Truncate((p.Y-Space)/(szDip+Space)); // координата выбранной мышью точки отображаемого растра по вертикали
        double xDip = x*(szDip+Space)+Space;  // начало области отображаемой точки растра в DIP-ах по горизонтали
        double yDip = y*(szDip+Space)+Space;  // начало области отображаемой точки растра в DIP-ах по вертикали
        Selection = new Rect(xDip, yDip, szDip, szDip);
        SelectedPixel = Source.Pixels[x, y];
        SelectedCoords = new System.Drawing.Point(x, y);
        if (Correlation != null) SelectedCorrelation = Correlation.GetCorrelation(new RasterAffinityDetector.PixelCoordsKey(x, y));
          else SelectedCorrelation = null;
        InvalidateVisual();
      }
    } // OnMouseMove


  } // class RasterImage

} // namespace RusLat.Controls
