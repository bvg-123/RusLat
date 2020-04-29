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

namespace RusLat.Windows
{
  /// <summary>
  /// Окно для получения выделенного мышью куска экрана. Исчезает при отпускании левой кнопки мыши.
  /// </summary>
  public partial class ScreenWindow :Window
  {
    /// <summary>
    /// Выделенная область.
    /// </summary>
    public System.Drawing.Rectangle SelectedArea;
    
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ScreenWindow ()
    {
      InitializeComponent();
      SelectedArea = new System.Drawing.Rectangle();
    } // ScreenWindow


    /// <summary>
    /// Вызывается при нажатии левой кнопки мыши. 
    /// Фиксирует координаты точки нажатия мыши.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      Point p = e.GetPosition((UIElement)Selection.Parent);
      Canvas.SetLeft(Selection, p.X);
      Canvas.SetTop(Selection, p.Y);
      Point p_screen = ((UIElement)Selection.Parent).PointToScreen(p);
      SelectedArea.X = (int)(p_screen.X);
      SelectedArea.Y = (int)(p_screen.Y);
    } // Window_MouseLeftButtonDown


    /// <summary>
    /// Вызывается при движении мыши по окну.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseMove (object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        // При движении мыши с нажатой левой кнопкой выделяем прямоугольную область между точкой нажатия левой кнопки мыши и текущие положением указателя мыши.
        Point p = e.GetPosition((UIElement)Selection.Parent);
        double w = p.X-Canvas.GetLeft(Selection)+1;
        double h = p.Y-Canvas.GetTop(Selection)+1;
        if ((w >= 0) && (h >= 0))
        {
          Selection.Width = w;
          Selection.Height = h;
        }
      }
    } // Window_MouseMove



    /// <summary>
    /// Вызывается при отпускании кнопки левой кнопки мыши.
    /// Фиксирует координаты точки отпускания мыши и закрывает окно.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
    {
      Point p = e.GetPosition((UIElement)Selection.Parent);
      double w = p.X-Canvas.GetLeft(Selection)+1;
      double h = p.Y-Canvas.GetTop(Selection)+1;
      if ((w >= 0) && (h >= 0))
      {
        Selection.Width = w;
        Selection.Height = h;
        SelectedArea.Width = (int)(w*DPI.Scale);
        SelectedArea.Height = (int)(h*DPI.Scale);
        Close();
      }
    } // Window_MouseLeftButtonUp


  } // class ScreenWindow

} // namespace RusLat.Windows
