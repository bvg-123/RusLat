﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RusLat.Tools
{
  /// <summary>
  /// Маска изображения переключателя языка ввода с включенным языком, который необходимо визуально подсвечивать.
  /// </summary>
  public class ImageMask
  {
    /// <summary>
    /// Имя файла с маской изображения языка ввода, при наличии которого нужно включать визуальную подсветку.
    /// </summary>
    private const string MaskFileName = "Mask.png";

    /// <summary>
    /// Строка запроса заголовка метаданных изображнения для хранения положения записанной в изображении области маски.
    /// </summary>
    private const string BoundsMetadataQuery = "/iTXt/Keyword";

    /// <summary>
    /// Экземпляр класса маски изображения переключателя языка ввода с включенным языком, который необходимо визуально подсвечивать.
    /// Синглетон. Потокобезопасный.
    /// </summary>
    public static ImageMask Instance
    {
      get
      {
        if (_ImageMask == null)
        {
          lock (_SyncRoot)
          {
            if (_ImageMask == null) _ImageMask = new ImageMask();
          }
        }
        return _ImageMask;
      } // get Instance

    } // Instance
    private static ImageMask _ImageMask;

    /// <summary>
    /// Объект для синхронизации обращений к экзепляру маски изображения из разных потоков.
    /// </summary>
    private static object _SyncRoot = new object();


    /// <summary>
    /// Маска-изображение, соответствующее подсвечиваемому языку ввода.
    /// </summary>
    private Raster Mask;
    private System.Drawing.Bitmap MaskBitmap;

    /// <summary>
    /// Положение маски на экране в экранных координатах.
    /// </summary>
    private System.Drawing.Rectangle Bounds;


    /// <summary>
    /// Конструктор.
    /// </summary>
    public ImageMask ()
    {
      if (File.Exists(MaskFileName))
      {
        using (FileStream stream = File.Open(MaskFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          BitmapDecoder decoder = PngBitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
          BitmapFrame frame = decoder.Frames[0];
          Bounds.Width = frame.PixelWidth;
          Bounds.Height = frame.PixelHeight;
          BitmapMetadata metadata = (BitmapMetadata)frame.Metadata;
          object obj = metadata.GetQuery(BoundsMetadataQuery);
          if ((obj != null) && (obj is string))
          {
            string[] items = ((string)(obj)).Split(',');
            if (items.Length == 2)
            {
              Bounds.X = int.Parse(items[0]);
              Bounds.Y = int.Parse(items[1]);
              MaskBitmap = GetBitmap(frame);
              Mask = new Raster(MaskBitmap);
            }
          }
        }
      }
    } // ImageMask


    /// <summary>
    /// Проверяет наличие ранее созданной маски индикатора языка ввода.
    /// </summary>
    /// <returns>Возвращает true, при наличии ранее созданной маски языка ввода. При ее отсутствии возвращает false.</returns>
    public bool Exists ()
    {
      bool result;
      result = (Mask != null);
      return result;
    } // Exists


    /// <summary>
    /// Устанавливает заданную маску индикатора языка ввода.
    /// </summary>
    /// <param name="maskBitmap">Маска-изображение, соответствующее подсвечиваемому языку ввода.</param>
    /// <param name="bounds">Положение маски на экране в экранных координатах.</param>
    public void SetMask (System.Drawing.Bitmap maskBitmap, System.Drawing.Rectangle bounds)
    {
      if (Path.GetExtension(MaskFileName).ToLower() != ".png") throw new FileFormatException(new Uri(MaskFileName), $"Неподдерживаемое расширение файла {MaskFileName}. Предполагается расширение .png.");
      if (Mask != null) Mask.Dispose();
      if (MaskBitmap != null) MaskBitmap.Dispose();
      MaskBitmap = maskBitmap;
      Mask = new Raster(MaskBitmap);
      Bounds = bounds;
      using (FileStream stream = File.Create(MaskFileName))
      {
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        BitmapMetadata metadata = new BitmapMetadata("png");
        metadata.SetQuery(BoundsMetadataQuery, $"{bounds.X},{bounds.Y}".ToCharArray());
        BitmapFrame frame = BitmapFrame.Create(Mask.Source, null, metadata, null);
        encoder.Frames.Add(frame);
        encoder.Save(stream);
/*
        MaskBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;
        PngBitmapDecoder pngDecoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        BitmapFrame frame = pngDecoder.Frames[0];
        InPlaceBitmapMetadataWriter inplaceMetadataWriter = frame.CreateInPlaceBitmapMetadataWriter();
        if (inplaceMetadataWriter.TrySave() == true)
        {
          inplaceMetadataWriter.SetQuery(BoundsMetadataQuery, $"{bounds.X},{bounds.Y}".ToCharArray());
        }
*/
      }
    } // SetMask


    /// <summary>
    /// Проверяет область экрана, соответствующую маске, на совпадение с маской. 
    /// </summary>
    /// <returns>Возвращает true, если изображение в области экрана, соответствующей маске, совпадает с изображением-маской.</returns>
    public bool CheckMask ()
    {
      bool result = Exists();
      if (result)
      {
        using (System.Drawing.Bitmap bitmap = ScreenCapturer.Capture(Bounds))
        {
//          bitmap.Save("qq1.png", System.Drawing.Imaging.ImageFormat.Png);
          using (Raster raster = new Raster(bitmap))
          {
            double different = Mask.Compare(raster);
            result = (different < 0.08);
          }
//          bitmap.Save("qq2.png", System.Drawing.Imaging.ImageFormat.Png);
        }
      }
      return result;
    } // CheckMask


    /// <summary>
    /// Возвращает Bitmap по заданному BitmapSource.
    /// </summary>
    /// <param name="bitmapSource"></param>
    /// <returns></returns>
    private System.Drawing.Bitmap GetBitmap (BitmapSource bitmapSource)
    {
      System.Drawing.Bitmap bitmap;
      using (Stream stream = new MemoryStream())
      {
        BitmapEncoder enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bitmapSource));
        enc.Save(stream);
        bitmap = new System.Drawing.Bitmap(stream);
      }
      return bitmap;
    } // GetBitmap


  } // class ImageMask

} // namespace RusLat.Tools
