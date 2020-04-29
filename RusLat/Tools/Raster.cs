using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RusLat.Tools
{
  /// <summary>
  /// Класс для работы с бинарным представлением растра.
  /// </summary>
  public unsafe class Raster :IDisposable, IEnumerable<Raster.Pixel>
  {
    private bool IsDisposed;
    private readonly Bitmap Bitmap;
    private readonly BitmapData Data;

    /// <summary>
    /// Ширина растра (в пикселях).
    /// </summary>
    public int Width { get { return Bitmap.Width; } }

    /// <summary>
    /// Высота растра (в пикселях).
    /// </summary>
    public int Height { get { return Bitmap.Height; } }

    /// <summary>
    /// Количество пикселей в растре.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Матрица голубой составляющей цветов растра.
    /// </summary>
    public readonly ColorPane8 Blue;

    /// <summary>
    /// Матрица зеленой составляющей цветов растра.
    /// </summary>
    public readonly ColorPane8 Green;

    /// <summary>
    /// Матрица красной составляющей цветов растра.
    /// </summary>
    public readonly ColorPane8 Red;

    /// <summary>
    /// Матрица цветов точек растра (младший байт - голубой, второй байт - зеленый, третий байт - красный, старший четвертый байт не используется)
    /// </summary>
    public readonly ColorPane32 ColorRGB;

    /// <summary>
    /// Матрица точек растра.
    /// </summary>
    public readonly PixelPane Pixels;


    public BitmapSource Source
    {
      get
      {
        return BitmapSource.Create(Width, Height, Bitmap.HorizontalResolution, Bitmap.VerticalResolution, PixelFormats.Bgr32, null, Data.Scan0, Data.Stride*Data.Height, Data.Stride);
      }
    } // Source


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="bitmap">Изображение, с бинарным представлением которого будет производиться работа.</param>
    public Raster (Bitmap bitmap)
    {
      Bitmap = bitmap;
      Data = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
      byte* p = (byte*)Data.Scan0.ToPointer();
      Blue = new ColorPane8(p, Data.Stride);
      Green = new ColorPane8(p+1, Data.Stride);
      Red = new ColorPane8(p+2, Data.Stride);
      ColorRGB = new ColorPane32(p, Data.Stride);
      Pixels = new PixelPane(p, Data.Stride);
      Size = Width*Height;
    } // Raster


    public void Dispose ()
    {
      if (!IsDisposed)
      {
        if ((Bitmap != null) && (Data != null)) Bitmap.UnlockBits(Data);
        IsDisposed = true;
      }
    } // Dispose


    /// <summary>
    /// Определяет степень сходства данного растра с переданным растром.
    /// </summary>
    /// <param name="raster">Растр, с котоорым производится сравнение.</param>
    /// <returns>Коэффициент сходства:
    /// 1 - полностью совпадают
    /// 0 - совершенно не совпадают.
    /// Промежуточное значение между 0 и 1 - показатель степени сходства. Чем ближе к 1 тем более похожи растры.
    /// </returns>
    public double Compare (Raster raster)
    {
      IEnumerator<Raster.Pixel> rasterEnumerator = raster.GetEnumerator();
      IEnumerator<Raster.Pixel> thisEnumerator = this.GetEnumerator();
      int diffs = this.Size;
      while (rasterEnumerator.MoveNext() && thisEnumerator.MoveNext())
      {
        if (thisEnumerator.Current.IsEqual(rasterEnumerator.Current, 70))
        {
          diffs--;
        }
        else
        {
          rasterEnumerator.Current.ColorRGB = 0xFF0000;
        }
      }
      return ((double)diffs)/(double)Size;
    } // Compare


    public IEnumerator<Pixel> GetEnumerator ()
    {
      return new RasterEnumerator(this);
    } // GetEnumerator


    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    } // GetEnumerator


    public class ColorPane8
    {
      private readonly byte* Pane;
      private readonly int Stride;

      public byte* this[int x, int y]
      {
        get
        {
          return Pane+CoordsToByteOffset(x, y);
        } // get this[]
      } // this[]


      public ColorPane8 (byte* pane, int stride)
      {
        Pane = pane;
        Stride = stride;
      } // ColorPane8


      private int CoordsToByteOffset (int x, int y)
      {
        return y*Stride+(x<<2);
      } // CoordsToByteOffset


    } // class ColorPane8


    public class ColorPane32 :ColorPane8
    {
      public new Int32* this[int x, int y]
      {
        get
        {
          return (Int32*)base[x, y];
        } // get this[]
      } // this[]


      public ColorPane32 (byte* pane, int stride) : base(pane, stride)
      {
      } // ColorPane32

    } // class ColorPane32


    public class PixelPane :ColorPane8
    {
      public new Pixel this[int x, int y]
      {
        get
        {
          Pixel result = new Pixel(() => x, () => y);
          result.Ptr = base[x, y];
          return result;
        } // get this[]
      } // this[]


      public PixelPane (byte* pane, int stride) : base(pane, stride)
      {
      } // PixelPane

    } // class PixelPane


    /// <summary>
    /// Класс пикселя растра, позволяющий быстро получить или задать его цвет.
    /// </summary>
    public class Pixel
    {
      /// <summary>
      /// Указатель на положение пикселя в растре.
      /// </summary>
      public byte* Ptr;

      /// <summary>
      /// Метод получения координаты пикселя по горизонтали.
      /// </summary>
      private Func<int> GetX;

      /// <summary>
      /// Метод получения координаты пикселя по вертикали.
      /// </summary>
      private Func<int> GetY;

      /// <summary>
      /// Возвращает или задает цвет пикселя растра: 0xRRGGBB.
      /// </summary>
      public Int32 ColorRGB { get { return *(Int32*)Ptr; } set { *(Int32*)Ptr = value; } }

      /// <summary>
      /// Возвращает значение голубой составляющей пикселя.
      /// </summary>
      public byte Blue { get { return *Ptr; } }

      /// <summary>
      /// Возвращает значение зеленой составляющей пикселя.
      /// </summary>
      public byte Green { get { return *(Ptr+1); } }

      /// <summary>
      /// Возвращает значение красной составляющей пикселя.
      /// </summary>
      public byte Red { get { return *(Ptr+2); } }

      /// <summary>
      /// Возвращает или задает цвет пикселя растра в виде стандартной WPF-структуры Color.
      /// </summary>
      public System.Windows.Media.Color Color { get { return System.Windows.Media.Color.FromRgb(Red, Green, Blue); }}

      /// <summary>
      /// Возвращает яркость пикселя в нормированном цветовом пространстве 0-1.
      /// </summary>
      public double E { get { return (Math.Sqrt(Red*Red+Green*Green+Blue*Blue)/443.405d); }}

      /// <summary>
      /// Координата пикселя в растре по горизонтали.
      /// </summary>
      public int X => GetX();

      /// <summary>
      /// Координата пикселя в растре по вертикали.
      /// </summary>
      public int Y => GetY();


      /// <summary>
      /// Конструктор.
      /// </summary>
      /// <param name="getX">Метод получения координаты пикселя по горизонтали.</param>
      /// <param name="getY">Метод получения координаты пикселя по вертикали.</param>
      public Pixel (Func<int> getX, Func<int> getY)
      {
        GetX = getX;
        GetY = getY;
      } // Pixel


      /// <summary>
      /// Возвращает степень близости переданного и данного пикселей в цветовом пространстве.
      /// </summary>
      /// <param name="pixel">Пиксель, по отношению к которому определяется степень близости данного пискеля.</param>
      /// <returns>Возвращает степень близости переданного и данного пикселей в цветовом пространстве.</returns>
      public double Compare (Pixel pixel)
      {
        int b = this.Blue-pixel.Blue;
        int g = this.Green-pixel.Green;
        int r = this.Red-pixel.Red;
        return Math.Sqrt(b*b+g*g+r*r);
      } // Compare


      /// <summary>
      /// Возвращает true, если цвета пикселей совпадают в пределах заданной степени точности. 
      /// </summary>
      /// <param name="pixel">Пиксель, с которым производится сравнение.</param>
      /// <param name="presize">Степень точности сравнения.</param>
      /// <returns>Возвращает true, если цвета пикселей совпадают в пределах заданной степени точности.</returns>
      public bool IsEqual (Pixel pixel, int presize)
      {
        return Math.Abs(this.Blue-pixel.Blue)+Math.Abs(this.Green-pixel.Green)+Math.Abs(this.Red-pixel.Red) < presize;
      } // IsEqual


    } // class Pixel


    public class RasterEnumerator :IEnumerator<Pixel>, IEnumerator
    {
      private byte* Ptr;
      private Raster Raster;
      private int x;
      private int y;
      private byte* LinePtr;
      private Pixel Pixel;

      public RasterEnumerator (Raster raster)
      {
        Raster = raster;
        Reset();
      } // RasterEnumerator


      public Pixel Current
      {
        get
        {
          return Pixel;
        }
      } // Current


      object IEnumerator.Current
      {
        get
        {
          return Current;
        }
      } // Current


      public void Dispose ()
      {
        Pixel = null;
      } // Dispose


      public bool MoveNext ()
      {
        bool result = true;
        x++;
        if (x == Raster.Width)
        {
          y++;
          if (y == Raster.Height)
          {
            result = false;
          }
          else
          {
            x = 0;
            LinePtr += Raster.Data.Stride;
            if (y == 1) LinePtr += 4;  // корректируем указатель, т.к. перебор начали не с первого пикселя, а с несуществующего минус первого в силу устройства итератора (см. Reset)
            Ptr = LinePtr;
            Pixel.Ptr = Ptr;
          }
        }
        else
        {
          Ptr += 4;
          Pixel.Ptr = Ptr;
        }
        return result;
      } // MoveNext


      public void Reset ()
      {
        y = 0;
        x = -1;
        LinePtr = Raster.Blue[0, 0]-4;  // -4 т.к. в начальном состоянии итератор стоит до первого элемента, а на первый элемент попадает только после первого MoveNext.
        Ptr = LinePtr;
        Pixel = new Pixel(() => x, () => y);
        Pixel.Ptr = Ptr;
      } // Reset

    } // RasterEnumerator

  } // class Raster


} // namespace RusLat.Tools
