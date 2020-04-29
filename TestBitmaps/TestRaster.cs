using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RusLat.Tools;

namespace TestBitmaps
{
  [TestClass]
  public unsafe class TestRaster
  {
    [TestMethod]
    public void TestRasterBase ()
    {
      using (Bitmap bitmap = new Bitmap(50, 20))
      {
        using (Raster raster = new Raster(bitmap))
        {
          *raster.Red[20, 10] = 255;
          *raster.Green[21, 11] = 255;
          *raster.Blue[22, 12] = 255;
          *raster.ColorRGB[23, 13] = 0xFF0000; // Red
          *raster.ColorRGB[24, 13] = 0x00FF00; // Green
          *raster.ColorRGB[25, 13] = 0x0000FF; // Blue
        }
        bitmap.Save("RasterBase.png");
      }
    } // TestRasterBase


    /// <summary>
    /// Не работает.
    /// </summary>
    [TestMethod]
    public void TestRasterEnumerator ()
    {
      using (Bitmap bitmap = new Bitmap(50, 20))
      {
        using (Raster raster = new Raster(bitmap))
        {
          foreach (Raster.Pixel pixel in raster)
          {
            pixel.ColorRGB = 0xFF0000; // 0xRRGGBB
          }
        }
        bitmap.Save("RasterEnum.png");
      }
    } // TestRasterEnumerator


  } // class TestRaster

} // namespace TestBitmaps
