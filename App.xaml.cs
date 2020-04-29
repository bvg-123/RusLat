using RusLat.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RusLat
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App :Application
  {
    /// <summary>
    /// Вызывается при запуске приложения.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Startup (object sender, StartupEventArgs e)
    {
      FindLangBar();
      Window startWindow;
      if (ImageMask.Instance.Exists()) startWindow = new MainWindow();
        else startWindow = new ImageMaskWindow();
      startWindow.Show();
    } // Application_Startup


    /// <summary>
    /// Производит автоматический поиск области с индикатором текущего языка (раскладеик клавиатуры).
    /// </summary>
    private void FindLangBar ()
    {
      // Windows 10: ClassName = InputIndicatorButton, Window Caption = пусто
      // Windows 7: ClassName = CiceroUIWndFrame, Window Caption = TF_FloatingLangBar_WndTitle

    } // FindLangBar


  } // class App

} // namespace RusLat
