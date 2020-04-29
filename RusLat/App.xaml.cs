using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using RusLat.Settings;
using RusLat.Tools;

namespace RusLat
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private const string CommandHelp = "help";
    private const string CommandHelpAlt = "?";
    private const string CommandReset = "reset";
    private const string CommandDebug = "debug";
    private const string CommandQuit = "quit";
    private const string CommandQuitAlt = "exit";

    /// <summary>
    /// Список поддерживаемых приложением ключей командной строки.
    /// </summary>
    private Dictionary<string, string> Commands = new Dictionary<string, string>()
    {
      { CommandHelp, "Отображает справку по ключам командной строки приложения."}
     ,{ CommandHelpAlt, "Отображает справку по ключам командной строки приложения." }
     ,{ CommandReset, "Позволяет задать подсвечиваемый язык ввода, как при первом запуске приложения."}
     ,{ CommandDebug, "Наличие этого ключа включает отладочное окно, а его отсутствие - выключает."}
     ,{ CommandQuit, "Завершает работу приложения."}
     ,{ CommandQuitAlt, "Завершает работу приложения."}
    }; // Commands



    /// <summary>
    /// Признак включения режима отладки.
    /// В режиме отладки отображается отладочное окно.
    /// </summary>
    public bool IsDebug
    {
      get
      {
        return _IsDebug;
      }
      private set
      {
        if (_IsDebug != value)
        {
          _IsDebug = value;
          if (IsDebug) DebugWindow.Show();
          else DebugWindow.Hide();
        }
      }
    }
    private bool _IsDebug;

    /// <summary>
    /// Отладочное окно. 
    /// </summary>
    public Window DebugWindow { get; private set; }

    /// <summary>
    /// Настройки приложения.
    /// </summary>
    public static AppSettings Settings { get { return (AppSettings)App.Current.Resources["AppSettings"]; } }


    /// <summary>
    /// Вызывается при запуске приложения.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Startup (object sender, StartupEventArgs e)
    {
      Settings.Load();
      DebugWindow = new Windows.DebugWindow();
      ApplySwitches(e.Args);
      Window startWindow;
      if (ImageMask.Instance.Exists())
      {
        startWindow = new Windows.MainWindow();
      }
      else
      {
        ShowHelp(false);
        startWindow = new Windows.ImageMaskWindow();
      }
      startWindow.Show();
    } // Application_Startup


    /// <summary>
    /// Вызывается в первом запущенном экземпляре приложения при запусках последующих экземпляров приложения.
    /// Выполняет команду, переданную из очередного запущенного экземпляра приложения через параметры командной строки.
    /// </summary>
    /// <param name="args">Параметры командной строки от последующего запускаемого экземпляра приложения.</param>
    public void Activate (string[] args)
    {
      ApplySwitches(args);
    } // Activate


    /// <summary>
    /// При наличии команды debug показывает отладочное окно, а при отсутствии команды debug скрывает отладочное окно.
    /// </summary>
    /// <param name="args">Массив строк-аргументов с командами.</param>
    private void ApplySwitches (string[] args)
    {
      if (args.HasCommand(CommandQuit) || args.HasCommand(CommandQuitAlt)) Shutdown();
      if (args.HasCommand(CommandHelp) || args.HasCommand(CommandHelpAlt)) ShowHelp(true);
      IsDebug = args.HasCommand(CommandDebug);
      if (args.HasCommand(CommandReset)) Reset();
    } // ApplySwitches


    /// <summary>
    /// Отображает справку по ключам командной строки приложения.
    /// </summary>
    /// <param name="forceShutdown">Признак необходимости завершения работы приложения после отображения справки.</param>
    private void ShowHelp (bool forceShutdown)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Описание ключей командной строки запуска приложения:");
      foreach (KeyValuePair<string, string> item in Commands)
      {
        sb.AppendLine($"{item.Key}: {item.Value}");
      }
      MessageBox.Show(sb.ToString());
      if (forceShutdown) Shutdown();
    } // ShowHelp


    /// <summary>
    /// Удаляет сохраненную маску-изображение подсвечиваемого языка ввода, что позволяет
    /// заново установить подсвечиваемый язык ввода, как при первом запуске приложения.
    /// </summary>
    private void Reset ()
    {
      ImageMask.Instance.Delete();
      if (MainWindow is Windows.MainWindow)
      {
        MainWindow = new Windows.ImageMaskWindow();
        MainWindow.Show();
      }
    } // Reset


  } // class App


  /// <summary>
  /// Класс для объявления методов-расширений по мере надобности.
  /// </summary>
  static partial class Extension
  {
    /// <summary>
    /// Проверяет наличие указанной команды в заданном массиве строк-аргументов.
    /// Команда - это /command, -command или просто command вне зависимости от регистра.
    /// </summary>
    /// <param name="args">Массив строк-аргументов, среди которых производится поиск команды.</param>
    /// <param name="command">Искомая команда. Если название команды начинается с минуса или слэша, то производится поиск команды AS IS.
    /// Если же имя команды не начаниется с минуса или слэша, то производится поиск всех вариантов команды /command, -command или просто command.</param>
    /// <returns>Возвращает true, если указнная команда имеется в заданном массиве строк-аргументов. Если де команда отсутствуеит, то возвращается false.</returns>
    public static bool HasCommand (this string[] args, string command)
    {
      bool result = args.Contains(command, StringComparer.OrdinalIgnoreCase);
      if (!result && !command.StartsWith("/") && !command.StartsWith("-")) result = args.HasCommand("/"+command) || args.HasCommand("-"+command);
      return result;
    } // HasCommand


  } // class Extension

} // namespace RusLat
