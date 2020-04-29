using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBitmaps
{
  [TestClass]
  public class TestBitmap
  {
    [TestMethod]
    public void TestTransparentBitmap ()
    {
      using (Bitmap bitmap = new Bitmap(50, 20))
      {
        bitmap.Save("Transparent.png");
      }
    } // TestTransparentBitmap


    [TestMethod]
    public void TestPixeledBitmap ()
    {
      using (Bitmap bitmap = new Bitmap(50, 20))
      {
        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        unsafe
        {
          byte* p = (byte*)data.Scan0.ToPointer();
          //*p = 255; // B
          p++;
          //*p = 255; // G
          p++;
          *p = 255;   // R
          p--;
          p--;
          p += 3*10;
          *p = 255; // B: 10-я точка по горизонтали
          p -= 3*10;
          *(p+3*20) = 255;  // B: 20-я точка по горизонтали.
          *(p+data.Stride*10) = 255;  // B: 0-я точка по горизонтали, 10-я точка по вертикали.
          *(p+data.Stride*10+3*20) = 255;  // B: 20-я точка по горизонтали, 10-я точка по вертикали.
        }
        bitmap.UnlockBits(data);
        bitmap.Save("Pixeled.png");
      }
    } // TestPixeledBitmap

  } // class TestBitmap

} // namespace TestBitmaps
