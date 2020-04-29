using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RusLat.Tools
{
  /// <summary>
  /// Класс для чтения и записи ini-файлов.
  /// </summary>
  public class IniFile :IEnumerator, IEnumerable
  {
    /// <summary>
    /// Имя ini-файла.
    /// </summary>
    public string FileName { get; protected set; }

    /// <summary>
    /// Дата/время модификации загруженного ini-файла
    /// </summary>
    public DateTime Modified { get; protected set; }

    /// <summary>
    /// Данные ini-файла
    /// </summary>
    /// <param name="key">Имя ключа данных</param>
    /// <param name="section">Имя секции, к которой относится ключ данных.
    /// Если не задано, то берется ключ данных, не привязанных ни к какой секции (секция с пустыми именем).</param>
    /// <returns></returns>
    public string this[string key, string section = ""]
    {
      get
      {
        string result = null;
        // Задан только один параметр - это ключ данных, не привязанных ни к какой секции
        if (Sections[section].ContainsKey(key)) result = (string)Sections[section][key];
        return result;
      }

      set
      {
        Sections[section][key] = value;
      }

    } // this[string key]


    /// <summary>
    /// Класс секции ini-файла.
    /// </summary>
    public class Section :Dictionary<string, object>
    {
      /// <summary>
      /// Имя секции.
      /// </summary>
      public string Name { get; protected set; }

      /// <summary>
      /// Номер строки с объявлением секции.
      /// </summary>
      public int LineNumber { get; protected set; }

      /// <summary>
      /// Имя файла с объявлением секции.
      /// </summary>
      public string FileName { get; protected set; }


      /// <summary>
      /// Конструктор.
      /// </summary>
      /// <param name="name">Имя секции.</param>
      /// <param name="lineNumber">Номер строки с объявлением секции.</param>
      /// <param name="fileName">Имя файла с объявлением секции.</param>
      public Section (string name, int lineNumber, string fileName)
      {
        Name = name;
        LineNumber = lineNumber;
        FileName = fileName;
      } // Section

    } // class Section


    /// <summary>
    /// Словарь с данными ini-файла, разбитыми на секции. Ключ - название секции.
    /// </summary>
    protected Dictionary<string, Section> Sections;

    /// <summary>
    /// Итератор по секциям 
    /// </summary>
    protected IEnumerator SectionsEnumerator; 


    /// <summary>
    /// Текущая анализируемая строка ini-файла. Используется в процессе работы метода Load().
    /// </summary>
    protected string CurrentLine;


    /// <summary>
    /// Порядковый номер текущей анализируемой строки ini-файла (считая от 1). Используется в процессе работы метода Load().
    /// </summary>
    protected int CurrentLineIndex;


    /// <summary>
    /// Добавляет заданную секцию в список имеющихся секций. Если секция с тем же именем уже имеется,
    /// то возникает исключение.
    /// </summary>
    /// <param name="section">Добавляемая секция.</param>
    /// <returns>Добавленная секция.</returns>
    protected virtual Section AppendSection (Section section)
    {
      if (Sections.ContainsKey(section.Name)) throw new Exception(String.Format("В файле \"{0}\" в строке {1} встретилось объявление секции \"{2}\", которая уже была ранее объявлена. Повторное объявление уже существующей секции не допускается.", FileName, CurrentLineIndex, section.Name));
      Sections[section.Name] = section;
      return section;
    } // AppendSection


    /// <summary>
    /// Вызывается на каждое объявление очередной секции ini-файла. 
    /// В базовой реализации создает словарь ключ-значение для хранения данных секции,
    /// добавляется его в словарь секций Sections под именем sectionDecalration и возвращает этот словарь.
    /// </summary>
    /// <param name="sectionDeclaration">Объявление секции (та часть, которая внутри квадратных скобок): [section]</param>
    /// <returns>Текущая секция (словарь ключ-значение для занесения данных секции). Если секция не подразумевает никаких данных внутре себя
    /// и состоит только из объявления секции, то должен возвращаеться null.</returns>
    protected virtual Section OnDeclareSection (string sectionDeclaration)
    {
      Section result = new Section(sectionDeclaration, CurrentLineIndex, FileName);
      AppendSection(result);
      return result;
    } // OnDeclareSection


    /// <summary>
    /// Вызывается на каждое объявление очередной строки данных очередной секции ini-файла.
    /// В базовой реализации производит анализ строки данных вида key=value и добавляет результат анализа в текущую секцию. 
    /// </summary>
    /// <param name="currentSectionName">Название текущей секции ini-файла, строка которой обрабатывается.</param>
    /// <param name="currentSection">Данные текущей секции</param>
    protected virtual void OnDeclareData (string currentSectionName, Section currentSection)
    {
      Match match = Regex.Match(CurrentLine, @"^([^=]*)=([^;]*)(?:[ \t]*;.*)?$(?#<=необязательный комментарий до конца строки, отделенный пробелами или табуляциями от ключа и его значения)", RegexOptions.IgnoreCase);
      string key;
      string value;
      if (match.Success)
      {
        key = match.Groups[1].Value;
        value = match.Groups[2].Value.TrimEnd(' ', '\t');
      }
      else
      {
        key = CurrentLine;
        value = null;
      }
      if (!String.IsNullOrWhiteSpace(key)) // Пропускаем пустые строки 
      { 
        if (currentSection.ContainsKey(key))
        {
          object prevValue = currentSection[key];
          if ((prevValue == null) || (prevValue.GetType() == typeof(string)))
          {
            currentSection[key] = new List<string>(new string[] { (string)currentSection[key], value });
          }
          else
          {
            (currentSection[key] as List<string>).Add(value);
          }
        }
        else
        {
          currentSection.Add(key, value);
        }
      }
    } // OnDeclareData


    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fileName">Имя ini-файла.</param>
    public IniFile (string fileName)
    {
      FileName = Path.GetFullPath(fileName);
      Sections = new Dictionary<string, Section>();
      Sections[""] = new Section(String.Empty, 0, FileName);
    } // IniFile


    /// <summary>
    /// Читает содержимое из заданного ini-файла в виде словаря пар ключ=значение
    /// </summary>
    public virtual void Load ()
    {
      Sections.Clear();
      string currentSectionName = "";
      Section currentSection = new Section(currentSectionName, 0, FileName);
      Sections[currentSectionName] = currentSection; 
      Modified = new DateTime(0);
      if (!File.Exists(FileName)) throw new Exception(String.Format("Файл {0} не найден", Path.GetFullPath(FileName)));
      Modified = File.GetLastWriteTime(FileName);
      string[] lines = File.ReadAllLines(FileName, Encoding.UTF8);

      bool isPrevLineContinue = false;    // признак продолжения предыдущей строки, оконченной обратным слэшом
      CurrentLineIndex = 1;
      while (CurrentLineIndex <= lines.Length)
      {
        if (isPrevLineContinue) CurrentLine = CurrentLine+lines[CurrentLineIndex-1];
          else CurrentLine = lines[CurrentLineIndex-1];
        bool skipLine = String.IsNullOrWhiteSpace(CurrentLine) || CurrentLine.StartsWith(";") || CurrentLine.StartsWith("#");
        if (!skipLine)
        {
          Match match = Regex.Match(CurrentLine, @"^\[([^\]]+)\]", RegexOptions.IgnoreCase);
          if (match.Success)
          {
            // Объявление новой секции
            currentSectionName = match.Groups[1].Value;
            currentSection = OnDeclareSection(currentSectionName);
          }
          else
          {
            // Очередная строка данных в текущей секции
            if (currentSection == null) throw new Exception(String.Format("В файле \"{0}\" в секции \"{1}\" объявлена строка данных \"{2}\" (номер строки {3}), хотя указанная секция не подразумевает наличия в ней строк данных.", FileName, currentSectionName, CurrentLine, CurrentLineIndex));
            if (CurrentLine.EndsWith("\\"))
            {
              // Если строка заканчивается обратным слэшом, то ее продолжение идет на следующей строке.
              // Формируем сначала суммарную строку и только после этого ее обрабатываем.
              CurrentLine = CurrentLine.TrimEnd('\\');
              isPrevLineContinue = true;
            }
            else
            {
              isPrevLineContinue = false;
              OnDeclareData(currentSectionName, currentSection);
            }
          }
        }
        CurrentLineIndex++;
      }
      if (isPrevLineContinue)
      {
        // Обрабатываем последнюю составную строку
        OnDeclareData(currentSectionName, currentSection);
      }
    } // Load


    /// <summary>
    /// Выгружает на диск текущее содержимое ini-файла.
    /// TODO: не сохраняются комментарии.
    /// </summary>
    public void Flush()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, Section> sectionItem in Sections)
      {
        if (!String.IsNullOrEmpty(sectionItem.Key))
        {
          stringBuilder.AppendLine("[" + sectionItem.Key + "]");
        }
        foreach (KeyValuePair<string, object> item in sectionItem.Value)
        {
          if (item.Value.GetType() == typeof(string))
          {
            stringBuilder.AppendLine(item.Key + "=" + item.Value);
          }
          else
          {
            foreach (string value in (item.Value as List<string>))
            {
              stringBuilder.AppendLine(item.Key + "=" + value);
            }
          }
        }

      }
      File.WriteAllText(FileName, stringBuilder.ToString(), Encoding.UTF8);
      Modified = File.GetLastWriteTime(FileName);
    } // Flush


    /// <summary>
    /// Возвращает список названий секций.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetSectionNames ()
    {
      return Sections.Keys;
    } // GetSectionNames


    /// <summary>
    /// Возвращает секцию по ее имени.
    /// </summary>
    /// <param name="name">Имя секции</param>
    /// <returns>Объект секции</returns>
    public Section GetSection (string name)
    {
      if (!Sections.ContainsKey(name)) throw new Exception(String.Format("Секция {0} не найдена в списке объявленных секций файла {1}", name, FileName));
      return Sections[name];
    } // GetSection


    /// <summary>
    /// Возвращает отсортированный по возрастанию список ключей
    /// </summary>
    /// <param name="section">Имя секции, ключи которой возвращаются. Если не задано, то возвращаются
    /// ключи, не привязанные ни к какой секции.</param>
    /// <returns>Возвращает отсортированный по возрастанию список ключей</returns>
    public IOrderedEnumerable<string> GetSortedKeys (string section = "")
    {
      return Sections[section].Keys.OrderBy(key => int.Parse(key), Comparer<int>.Create((x, y) => x-y));           
    } // GetSortedKeys


    /// <summary>
    /// Возвращает список ключей
    /// </summary>
    /// <param name="section">Имя секции, ключи которой возвращаются. Если не задано, то возвращаются
    /// ключи, не привязанные ни к какой секции.</param>
    /// <returns>Возвращает список ключей</returns>
    public IEnumerable<string> GetKeys (string section = "")
    {
      return Sections[section].Keys;
    } // GetKeys


    /// <summary>
    /// Возвращает соответствующее ключу значение (одну строку или спиcок List<string> строк).
    /// Если указанного ключа нет, то возвращается null.
    /// </summary>
    /// <param name="key">Ключ для получения значения</param>
    /// <param name="section">Имя секции, значение ключа которой возвращаются. Если не задано, то возвращается
    /// значение ключа, не привязанного ни к какой секции.</param>
    /// <returns>Возвращает соответствующее ключу значение (одну строку или спиcок List<string> строк)</returns>
    public object GetValue (string key, string section = "")
    {
      object result = null;
      if (Sections[section].ContainsKey(key)) result = Sections[section][key];
      return result;
    } // GetValue


    /// <summary>
    /// IEnumerator.Current
    /// </summary>
    public object Current
    {
      get { return ((KeyValuePair<string, Section>)SectionsEnumerator.Current).Value; }
    } // Current

    
    /// <summary>
    /// IEnumerator.MoveNext
    /// </summary>
    /// <returns></returns>
    public bool MoveNext ()
    {
      return SectionsEnumerator.MoveNext();
    } // MoveNext

    
    /// <summary>
    /// IEnumerator.Reset
    /// </summary>
    public void Reset ()
    {
      SectionsEnumerator.Reset();
    } // Reset


    /// <summary>
    /// IEnumerable.GetEnumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator ()
    {
      SectionsEnumerator = Sections.GetEnumerator();
      return (this as IEnumerator);
    }
  } // class IniFile

} // namespace RusLat.Tools
