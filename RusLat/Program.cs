using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RusLat
{
  class Program
  {
    /// <summary>
    /// Имя ipc-порта, используемого первым запущенным экземпляром приложения для получения информации от последующих
    /// запускаемых экземпляров приложения.
    /// </summary>
    private const string IpcPort = "RusLat";

    /// <summary>
    /// Первый экземпляр запущенного приложения.
    /// </summary>
    private static RusLat.App App;


    /// <summary>
    /// Точка запуска приложения. Переопределяем штатную логику запуска через App для реализации механизма
    /// запуска только одного экземпляра приложения и механизма отправки информации из новых запускаемых экземпляров
    /// приложения первому запущенному (включение/выключение отладочного окна, сброс маски, завершение работы первого запущенного экземпляра приложения).
    /// </summary>
    [System.STAThreadAttribute()]
    public static void Main ()
    {
      try
      {
        // Пробуем открыть ipc-порт для получения информации от новых запускаемых экземплятров приложения.
        IpcServerChannel channel = new IpcServerChannel(IpcPort);
        ChannelServices.RegisterChannel(channel, false);
        RemotingConfiguration.RegisterActivatedServiceType(typeof(Proxy));

        // Удалось успешно открыть ipc-порт приложения, значит ренее запущенного экземпляра приложения нет
        // и данный запущенный экземпляр является первым.
        // Запускаем штатную бизнеc-логику первого экземпляра приложения.
        App = new RusLat.App();
        App.InitializeComponent();
        App.Run();
      }
      catch (RemotingException err)
      {
        if (err.HResult == -2146233077)
        {
          // Ранее был уже запущен экземпляр приложения.
          // Активируем его и передаем ему параметры параметры командной строки запуска нового экземпляра приложения,
          // после чего завершаем работу нового экземпляра приложения.
          ActivateFirstInstance(Environment.GetCommandLineArgs());
        }
        else
        {
          // Не удалось открыть ipc-порт приложения из-за какой-то неизвестной ошибки.
          // Отображаем информацию об ошибке и завершаем работу приложения.
          MessageBox.Show($"Ошибка запуска приложения RusLat:\r\n{err}");
          throw;
        }
      }
      catch (Exception err)
      {
        // Произошла какая-то неизвестная ошибка при запуске или необработанная ошибка в процессе работы приложения.
        // Отображаем информацию об ошибке и тихо (без crash-window) завершаем работу приложения.
        MessageBox.Show($"Ошибка приложения RusLat:\r\n{err}");
      }
    } // Main


    /// <summary>
    /// Активирует ранее запущенный экземпляр приложения и передает ему параметры командной строки запуска нового экземпляра приложения.
    /// </summary>
    /// <param name="args">Параметры командной строки нового экземпляра, которые будут переданы первому запущенному экземпляру приложения.</param>
    private static void ActivateFirstInstance(string[] args)
    {
      IpcClientChannel channel = new IpcClientChannel();
      ChannelServices.RegisterChannel(channel, false);
      RemotingConfiguration.RegisterActivatedClientType(typeof(Proxy), String.Format("ipc://{0}", IpcPort));
      Proxy proxy = new Proxy();
      proxy.Activate(args);
    } // ActivateFirstInstance


    /// <summary>
    /// Прокси-класс, обеспечивающий взаимодействие новых запускаемых экземпляров приложения с первым запущенным экземпляром.
    /// </summary>
    private class Proxy : MarshalByRefObject
    {
      /// <summary>
      /// Активирует первый запущенный экземпляр приложения по команде от нового запускаемого экземпляра приложения.
      /// Производит вызов метода Activate первого запущенного экземпляра приложения в контексте потока, в котором
      /// был создан этот первый экземпляр.
      /// </summary>
      /// <param name="args">Аргументы командной строки нового запускаемого экземпляра приложения.</param>
      public void Activate (string[] args)
      {
        if (App != null)
        {
          App.Dispatcher.Invoke((Action)(() =>
          {
            App.Activate(args);
          }));
        }
      } // Activate

    } // class Proxy


  } // class Program

} // namespace RusLat
