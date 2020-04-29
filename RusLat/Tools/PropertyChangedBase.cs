using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RusLat.Tools
{
  public abstract class PropertyChangedBase :INotifyPropertyChanged
  {
    /// <summary>
    /// Событие, происходящее при изменении значения свойства.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;


    /// <summary>
    /// Вызывает обработчики события изменения значения свойства PropertyChanged.
    /// </summary>
    /// <param name="propertyName">Имя изменившегося свойства.</param>
    /// <param name="oldValue">Старое значение изменившегося свойства.</param>
    /// <param name="newValue">Новое значение изименившегося свойства.</param>
    protected virtual void OnPropertyChanged (string propertyName, object oldValue, object newValue)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    } // OnPropertyChanged


    /// <summary>
    /// Записывает в поле новое значение свойства и вызывает событие PropertyChanged, если новое значение отличается от старого.
    /// </summary>
    /// <typeparam name="T">Тип значения свойства.</typeparam>
    /// <param name="field">Поле, в котором хранится значение свойства.</param>
    /// <param name="newValue">Новое значение свойства.</param>
    /// <param name="propertyName">Имя свойства. Необязательный параметр. Если не задан, то по-умолчанию равен имени свойства, из setter-а которого был произведен вызов данного метода.</param>
    protected virtual void SetProperty<T>(ref T field, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
      if (!EqualityComparer<T>.Default.Equals(field, newValue))
      {
        T oldValue = field;
        field = newValue;
        OnPropertyChanged(propertyName, oldValue, newValue);
      }
    } // SetProperty


  } // class PropertyChangedBase

} // namespace RusLat.Tools
