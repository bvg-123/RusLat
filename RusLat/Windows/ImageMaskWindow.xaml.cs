using RusLat.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RusLat.Windows
{
  /// <summary>
  /// Окно для выбора изображения подсвечиваемого языка.
  /// </summary>
  public partial class ImageMaskWindow :Window
  {
    /// <summary>
    /// Режим автоматического выбора области отображения индикатора языка ввода. 
    /// </summary>
    public const bool AutoSelectionMode = true;
    

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ImageMaskWindow ()
    {
      InitializeComponent();
    } // ImageMaskWindow


    /// <summary>
    /// Вызывается при нажатии кнопки выбора изображения подсвечиваемого языка.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectImageMaskButton_Click (object sender, RoutedEventArgs e)
    {
      System.Drawing.Rectangle selectedArea;
#pragma warning disable 0162
      if (AutoSelectionMode) selectedArea = GetAutoSelectedArea();
        else selectedArea = GetManualSelectedArea();
#pragma warning restore 0162
      ImageMask.Instance.SetMask(ScreenCapturer.Capture(selectedArea), selectedArea);
      MainWindow window = new MainWindow();
      App.Current.MainWindow = window;
      window.Show();
    } // SelectImageMaskButton_Click


    /// <summary>
    /// Возвращает область экрана с индикатором языка ввода.
    /// Область выбирается вручную мышью.
    /// </summary>
    /// <returns>Возвращает область экрана с индикатором языка ввода.</returns>
    private System.Drawing.Rectangle GetManualSelectedArea ()
    {
      System.Drawing.Rectangle result;
      ScreenWindow screenWindow = new ScreenWindow();
      screenWindow.ShowDialog();
      result = screenWindow.SelectedArea;
      Close();
      return result;
    } // GetManualSelectedArea


    /// <summary>
    /// Возвращает область экрана с индикатором языка ввода.
    /// Расположение области определяется автоматически по имени класса окна.
    /// </summary>
    /// <returns>Возвращает область экрана с индикатором языка ввода.</returns>
    public static System.Drawing.Rectangle GetAutoSelectedArea ()
    {
      // Windows 10: ClassName = InputIndicatorButton, Window Caption = пусто
      // Windows 7: ClassName = CiceroUIWndFrame, Window Caption = TF_FloatingLangBar_WndTitle
      System.Drawing.Rectangle result = default(System.Drawing.Rectangle);
      User32.EnumerateWindows((parentHwnd, hWnd, className, caption) =>
      {
        bool @continue = true;
//        string s = $"0x{parentHwnd:X} 0x{hWnd:X} {className} {caption}\r\n";
//        File.AppendAllText("log.txt", s);
        if (className == "InputIndicatorButton")
        {
          if (String.IsNullOrEmpty(caption))
          {
            @continue = false;
          }
        }
        if (className == "CiceroUIWndFrame")
        {
          if (caption == "TF_FloatingLangBar_WndTitle")
          {
            @continue = false;
          }
        }
        if (!@continue)
        {
          result = User32.GetWindowArea(hWnd);
        }
        return @continue;
      });

      if (result != default(System.Drawing.Rectangle))
      {
        result.Y = result.Y+2;
        result.Height = result.Height-2;
      }
      return result;
    } // GetAutoSelectedArea


  } // class ImageMaskWindow

} // namespace RusLat.Windows
