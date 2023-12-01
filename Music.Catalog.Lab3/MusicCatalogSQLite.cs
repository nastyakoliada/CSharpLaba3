

namespace Music.Catalog.Lab3;
/// <summary>
/// Класс музыкального каталога, котрый хранится в бд sqlite. База данных каталога создается на основе файла- шаблона
/// базы данных. Файл шаблона sqlite_template.db должен находиться в текущей папке приложения.
/// Пользовыатель указывает путь к файлу музыкального каталога, если такой файл не существует,
/// приложение копирует файл шаблона бд в файл с указанным именем. 
/// 
/// </summary>
public class MusicCatalogSQLite:IMusicCatalog
{
    /// <summary>
    /// Имя файла-шаблона базы данных
    /// </summary>
    private const string TEMPLATE_DB_FILE = "sqlite_template.db";

    /// <summary>
    /// Имя файла бд
    /// </summary>
    private string fileName = string.Empty;
    /// <summary>
    /// Конструктор для создания каталога
    /// </summary>
    /// <param name="path">Имя файла каталога</param>
    public MusicCatalogSQLite(string path)
    {
        if (!File.Exists(path))
        {
            File.Copy(
                Path.Combine(Environment.CurrentDirectory??"", TEMPLATE_DB_FILE),
                path, false);
        }
        fileName = path;
    }

    #region IMusicCatalog interface implementation
    /// <summary>
    /// Метод доавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    public void AddComposition(Composition composition)
    {
        using(Model.McContext context = new Model.McContext(fileName))
        {
            context.Compositions?.Add(
                new Model.Composition
                {
                    Author = composition.Author,
                    SongName = composition.SongName,
                }
                );
            context.SaveChanges();
        }
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Composition> EnumerateAllCompositions()
    {
        using (Model.McContext context = new Model.McContext(fileName))
        {
            return context.Compositions
                .OrderBy(c => c.Author)
                .ThenBy(c => c.SongName)
                .Select(c => new Composition
                {
                    Author = c.Author ?? "",
                    SongName = c.SongName ?? "",
                }).ToList();
        }
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора композиций, удовлетворяющих
    /// критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Enumerator для перебора</returns>
    public int Remove(string query)
    {
        using (Model.McContext context = new Model.McContext(fileName))
        {
            var listToRemove = context.Compositions
                .Where(c => (c.Author!.Contains(query))
                || (c.SongName!.Contains(query))).ToList();

            context.Compositions.RemoveRange(listToRemove);
            context.SaveChanges();
            return listToRemove.Count;
        }
    }
    /// <summary>
    /// Метод удаляет из каталога композиции, удовлетворяющие критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска</param>
    /// <returns>Количество удаленных композиций</returns>
    public IEnumerable<Composition> Search(string query)
    {
        using (Model.McContext context = new Model.McContext(fileName))
        {
            return context.Compositions
                .Where(c => (c.Author!.Contains(query))
                || (c.SongName!.Contains(query)))
                .OrderBy(c => c.Author)
                .ThenBy(c => c.SongName)
                .Select(c => new Composition
                {
                    Author = c.Author ?? "",
                    SongName = c.SongName ?? "",
                }).ToList();
        }
    }
    #endregion
}
