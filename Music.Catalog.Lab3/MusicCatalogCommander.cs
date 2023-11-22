﻿using static System.Console;

namespace Music.Catalog.Lab3;
/// <summary>
/// Класс представляет собой взаимеодействие с пользователем по работе с музыкальным каталогом
/// через консоль
/// 
/// Пользователь должен указать тип сериализации и имя каталога.
/// После этого сериализация происходит в файл с именем "Имя каталога".xml или .json.
/// Если указан тип сериализации SQLite, то сериализация происходит в файл "Имя каталога".db.
/// Для создания файла "Имя каталога".db используется шаблон базы данных файл sqlite_template.db, который
/// должен находиться в папке приложения. Проект содержит этот шаблон.
/// Файлы для сериализации располагаются в папке ApplicationData текущего пользователя
/// </summary>
public class MusicCatalogCommander
{
    /// <summary>
    /// Имя папки, которая будет создана в ApplicationData папке для хранения файлов музыкального кталога
    /// </summary>
    private const string MUSIC_CATALOG_DIRECTORY = "MusicCatalog";

    /// <summary>
    /// Показывает справку по работе с программой
    /// </summary>
    public static void ShowNotes()
    {
        WriteLine("""
            Программа Музыкальный каталог.
            Для работы с программой используйте следующие команды:
                Add - добавляет композицию в каталог;
                List - выводит в консоль все композиции из каталога;
                Search - выводит в консоль композиции, удовлетворяющие критерию поиска;
                Remove - удаляет из каталога композиции, удовлетворяющие критерию поиска;
                Quit - завершает работу с каталогом.
            """);

    }
    /// <summary>
    /// Метод запускает работу
    /// </summary>
    public static void Run()
    {
        ShowNotes();
        //Запросим тип каталога
        int type = 0;
        do
        {
            int.TryParse(ReadString("Укажите тип каталога (1-xml, 2-json,3-SQLite)"), out type);
        } while (type < 1 || type > 3);
        //Запросим имя каталога
        string catalogName = ReadString("\nУкажите имя музыкального каталога?");        
    
        //Создаем каталог и передаем его коммандеру
        MusicCatalogCommander commander = new (CreateMusicCatalog(catalogName,type));

        //Цикл работы с коммандером
        while (!commander.IsRedyToExit )
        {
            try
            {
                WriteLine("\nВведите команду: ");

                if (commander.Commands.TryGetValue(
                    (ReadLine() ?? "").ToUpper(),
                    out Action? action))
                {
                    action();
                }
                else
                {
                    WriteLine("Введена неверная команда. Попробуйте снова.");
                }
            }
            catch(Exception e) 
            {
                WriteLine("\nОшибка !");
                WriteLine(e.Message);
            }
        }
    }
    
    /// <summary>
    /// Экземпляр музыкального каталога
    /// </summary>
    private  IMusicCatalog catalog = null!;
    /// <summary>
    /// Сопоставление команд пользователя методам класса
    /// </summary>
    private readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>();
    /// <summary>
    /// Конструктор по умолчанию. Заполняет сопоставление команд методам класса
    /// </summary>
    public MusicCatalogCommander(IMusicCatalog catalog)
    {
        Commands.Add("ADD", Add);
        Commands.Add("LIST", List);
        Commands.Add("REMOVE", Remove);
        Commands.Add("SEARCH", Search);
        Commands.Add("QUIT", Quit);
        this.catalog = catalog;
    }
    /// <summary>
    /// Возвращает запрашиваемую строку у пользователя
    /// </summary>
    /// <param name="question">Текст того, что пользователь должен ввести</param>
    /// <returns>Введенная пользователем строка</returns>
    private static string ReadString(string question)
    {
        string line;
        do
        {
            WriteLine(question);
        }
        while (string.IsNullOrEmpty(line = ReadLine()!));
        return line;

    }
    #region Методы - команды пользователя
    /// <summary>
    /// Выполнение команды пользователя по занесении композиции в каталог
    /// </summary>
    public void Add()
    {

        catalog.AddComposition(
            new Composition
            {
                Author = ReadString("Имя автора:"),
                SongName = ReadString("Название композиции:"),
            });
    }

    /// <summary>
    /// Выполняет команду вывода полного содержимого каталога
    /// </summary>
    public void List()
    {
        PrintSongs("\nСписок всех песен:", catalog.EnumerateAllCompositions());
    }
    /// <summary>
    /// Метод выводит на консоль перечнь композиций
    /// </summary>
    /// <param name="header">Заголовок перечня композиций</param>
    /// <param name="songs">Enumerator для перебора композиций</param>
    private static void PrintSongs(string header,IEnumerable<Composition> songs)
    {
        WriteLine(header);
        foreach (var comp in songs)
        {
            WriteLine($"{comp.Author} - {comp.SongName}");
        }
    }
    /// <summary>
    /// Выполняет команду пользователя на удаление композиций из каталога, удовлетворяющих
    /// заданному критерию поиска
    /// </summary>
    public void Remove()
    {
        WriteLine($"Удалено {catalog.Remove(ReadString("Что удаляем?:"))} песен.");
    }
    /// <summary>
    ///  Выполняет команду пользователя на вывод на консоль перечня комспозиций, удовлетворяющих
    ///  заданному критерию поиска
    /// </summary>
    public void Search()
    {
        PrintSongs("\nРезультат поиска:", catalog.Search(ReadString("Что ищем ?:")));
    }
    /// <summary>
    /// Выполняет команду пользователя о завершении работы
    /// </summary>
    public void Quit()
    {
        IsRedyToExit = true;
    }
    public bool IsRedyToExit {get; private set;}
    #endregion

    #region Методы для создания папки хранения каталога, имения файла музыкального каталога
    /// <summary>
    /// Создает класс для работы с музыкальным каталогам по указанным параметрам
    /// </summary>
    /// <param name="catalogName">Имя каталога</param>
    /// <param name="serializationType">Тип сериализации</param>
    /// <returns></returns>
    private static IMusicCatalog CreateMusicCatalog(string catalogName, int serializationType)
    {
        string mcPath = MusicCatalogPath;

        if (!Path.Exists(mcPath)) Directory.CreateDirectory(mcPath);

        return serializationType switch
        {
            1 or 2 => new MusicCatalog(MusicCatalogFullName(catalogName + (serializationType == 1 ? ".xml" : ".json"))),
            _ => new MusicCatalogSQLite(MusicCatalogFullName(catalogName+".db")),
        };
    }
    /// <summary>
    /// Возвращает полное имя файла музыкального каталога
    /// </summary>
    /// <param name="fileName">Имя файла без пути</param>
    /// <returns>Полное имя файла</returns>
    private static string MusicCatalogFullName(string fileName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    MUSIC_CATALOG_DIRECTORY, fileName);
    }
    /// <summary>
    /// Папка, в которой будет сохраняться музыкальный каталог
    /// </summary>
    private static string MusicCatalogPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                MUSIC_CATALOG_DIRECTORY);
    #endregion

}

