using RusLat.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RusLat.Windows
{
  /// <summary>
  /// Основное окно приложения, подсвечивающее экран определенным оттенком при включении заданного языка ввода.  
  /// </summary>
  public partial class MainWindow :Window
  {
    private Timer Timer;

    public MainWindow ()
    {
      InitializeComponent();
      Left = 0;
      Top = 0;
      Height = SystemParameters.WorkArea.Height;
      Width = SystemParameters.WorkArea.Width;
      Timer = new Timer(1000);
      Timer.Elapsed += Timer_Elapsed;
      Timer.AutoReset = true;
      Timer.Start();
    } // MainWindow


    private void Timer_Elapsed (object sender, ElapsedEventArgs e)
    {
      Dispatcher.Invoke(() =>
      {
        if (ImageMask.Instance.CheckMask())
        {
          Panel.Background = new SolidColorBrush(App.Settings.SelectedColor);
          Panel.Opacity = App.Settings.SelectedOpacity;
        }
        else
        {
          Panel.Background = System.Windows.Media.Brushes.Transparent;
        }
      });
    } // Timer_Elapsed


    protected override void OnSourceInitialized (EventArgs e)
    {
      base.OnSourceInitialized(e);
      var hwnd = new WindowInteropHelper(this).Handle;
      User32.SetWindowExTransparent(hwnd);
    } // OnSourceInitialized


  } // partial class MainWindow

} // namespace RusLat.Windows
