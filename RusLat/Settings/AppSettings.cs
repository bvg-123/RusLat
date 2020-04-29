using Microsoft.Win32;
using RusLat.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace RusLat.Settings
{
  /// <summary>
  /// Настройки приложения.
  /// </summary>
  public class AppSettings :PropertyChangedBase
  {
    /// <summary>
    /// Имя файла с настройками приложения.
    /// </summary>
    private string SettingsFileName = @"%APPDATA%\RusLat\Settings.ini";

    /// <summary>
    /// Файл с настройками приложения.
    /// </summary>
    private IniFile SettingsFile;

    /// <summary>
    /// Цвет подсветки.
    /// </summary>
    public Color SelectedColor { get { return _SelectedColor; } set { SetProperty(ref _SelectedColor, value); }}
    private Color _SelectedColor;

    /// <summary>
    /// Степень прозрачности подсветки.
    /// </summary>
    public double SelectedOpacity { get { return _SelectedOpacity; } set { SetProperty(ref _SelectedOpacity, value); }}
    private double _SelectedOpacity;


    /// <summary>
    /// Конструктор.
    /// </summary>
    public AppSettings ()
    {
      _SelectedColor = Colors.Green;
      _SelectedOpacity = 0.1;
      SettingsFileName = Environment.ExpandEnvironmentVariables(SettingsFileName);
      Directory.CreateDirectory(Path.GetDirectoryName(SettingsFileName));
      SettingsFile = new IniFile(SettingsFileName);
      Load();
    } // AppSettings


    /// <summary>
    /// Вызывается при изменении значения свойства.
    /// Вызывает обработчики события изменения значения свойства PropertyChanged.
    /// Сохраняет новое значение свойства в файле настроек. 
    /// </summary>
    /// <param name="field">Поле, в котором хранится значение свойства.</param>
    /// <param name="newValue">Новое значение свойства.</param>
    /// <param name="propertyName">Имя свойства. Необязательный параметр. Если не задан, то по-умолчанию равен имени свойства, из setter-а которого был произведен вызов данного метода.</param>
    protected override void OnPropertyChanged (string propertyName, object oldValue, object newValue)
    {
      base.OnPropertyChanged(propertyName, oldValue, newValue);
      SettingsFile[propertyName] = Serialize(propertyName, newValue);
      Save();
    } // OnPropertyChanged


    /// <summary>
    /// Сериализует значение указанного свойства в строку.
    /// </summary>
    /// <param name="propertyName">Название сериализуемого свойства.</param>
    /// <param name="value">Сериализуемое значение свойства.</param>
    /// <returns>Сериализованное в строку значение свойства.</returns>
    protected virtual string Serialize (string propertyName, object value)
    {
      string result;
      if (propertyName == "SelectedColor") result = ((Color)value).ToString();
        else result = String.Format(CultureInfo.InvariantCulture, "{0}", value);
      return result;
    } // Serialize


    /// <summary>
    /// Десериализует значение указанного свойства из файла настроек. 
    /// </summary>
    /// <typeparam name="T">Тип значения свойства.</typeparam>
    /// <param name="propertyName">Название свойства.</param>
    /// <param name="defaultValue">Необязательный параметр. Значение свойства в случае отсутствия значения в файле настроек.</param>
    /// <returns></returns>
    protected virtual T Deserialize<T> (string propertyName, T defaultValue = default(T))
    {
      object result = defaultValue;
      string value = SettingsFile[propertyName];
      if (value != null)
      {
        if (propertyName == "SelectedColor")
        {
          result = ColorConverter.ConvertFromString(value);
        }
        else if (propertyName == "SelectedOpacity")
        {
          result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
      }
      return (T)result;
    } // Deserialize


    /// <summary>
    /// Загружает текущие настройки из файла. Событие изменения значений свойств при этом не происходит.
    /// </summary>
    public void Load ()
    {
      if (File.Exists(SettingsFileName))
      {
        SettingsFile.Load();
        _SelectedColor = Deserialize<Color>("SelectedColor", (Color)_SelectedColor);
        _SelectedOpacity = Deserialize<double>("SelectedOpacity", _SelectedOpacity);
      }
    } // Load


    /// <summary>
    /// Записывает текущие настройки в файл на диске.
    /// </summary>
    public void Save ()
    {
      SettingsFile.Flush();
    } // Save


  } // class AppSettings

} // namespace RusLat.Settings
