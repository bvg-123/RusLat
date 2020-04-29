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

namespace RusLat
{
  
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow :Window
  {
    private Timer Timer;

    // TODO: Сделать настройку цвета подсветки.
    private readonly System.Windows.Media.Brush SelectedBrush = System.Windows.Media.Brushes.Green;

    public MainWindow ()
    {
      InitializeComponent();
      Left = 0;
      Top = 0;
      Height =  SystemParameters.WorkArea.Height;
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
        int lang = WindowsServices.GetCurrentLang();

        if (ImageMask.Instance.CheckMask()) Panel.Background = SelectedBrush;
        else Panel.Background = System.Windows.Media.Brushes.Transparent;
      });
    } // Timer_Elapsed


    protected override void OnSourceInitialized (EventArgs e)
    {
      base.OnSourceInitialized(e);
      var hwnd = new WindowInteropHelper(this).Handle;
      WindowsServices.SetWindowExTransparent(hwnd);
    } // OnSourceInitialized


  } // partial class MainWindow

} // namespace RusLat
