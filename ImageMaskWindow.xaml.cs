using RusLat.Tools;
using System;
using System.Collections.Generic;
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

namespace RusLat
{
  /// <summary>
  /// Окно для выбора изображения подсвечиваемого языка.
  /// </summary>
  public partial class ImageMaskWindow :Window
  {
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
      ScreenWindow screenWindow = new ScreenWindow();
      screenWindow.ShowDialog();
      ImageMask.Instance.SetMask(ScreenCapturer.Capture(screenWindow.SelectedArea), screenWindow.SelectedArea);
      Close();
      MainWindow window = new MainWindow();
      window.Show();
    } // SelectImageMaskButton_Click


  } // class ImageMaskWindow

} // namespace RusLat
