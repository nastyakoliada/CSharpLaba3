namespace Music.Catalog.Lab3;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
/// <summary>
/// Класс содержит перечень музыкальных композиций и предоставляет методы по работе с ним.
/// Поддерживается сериализация в xml и json
/// </summary>
public class MusicCatalog : IMusicCatalog
{
    /// <summary>
    /// Имя файла дл сериализации
    /// </summary>
    private string fileName = string.Empty;
    /// <summary>
    /// Определяет тип сериализации на основе расширения имени файла сериализации
    /// </summary>
    private bool IsXml => string.Equals(Path.GetExtension(fileName), ".XML", StringComparison.OrdinalIgnoreCase);
    /// <summary>
    /// Конструктор с указанием имени файла для сериализации
    /// </summary>
    /// <param name="fileName"></param>
    public MusicCatalog(string fileName)
    {
        this.fileName=fileName;

        if (File.Exists(fileName))
        {
            LoadFromFile();
        }
        else
        {
            Compositions = new List<Composition>();
        }
    }    
    
    /// <summary>
    /// Перечень композиций
    /// </summary>
    private List<Composition> Compositions { get; set; } = null!;

    #region IMusicCatalog interface implementation
    /// <summary>
    /// Метод доавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    public void AddComposition(Composition composition)
    {
        Compositions.Add(composition);
        Serialize();
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Composition>  EnumerateAllCompositions() => Compositions.OrderBy(c => c.Author).ThenBy(c => c.SongName);

    /// <summary>
    /// Метод возвращает enumerator для перебора композиций, удовлетворяющих
    /// критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Enumerator для перебора</returns>
    public IEnumerable<Composition> Search(string query) => Compositions
            .Where(c => c.Author.Contains(query,StringComparison.OrdinalIgnoreCase) 
            || c.SongName.Contains(query,StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName);
    
    /// <summary>
    /// Метод удаляет из каталога композиции, удовлетворяющие критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска</param>
    /// <returns>Количество удаленных композиций</returns>
    public int Remove(string query)
    {
        var removeList = Search(query).ToList();
        
        foreach(var item in removeList)
        {
            Compositions.Remove(item);
        }
        Serialize();

        return removeList.Count;
       
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Сериализация каталога.
    /// </summary>
    private void Serialize()
    {
        if (string.IsNullOrEmpty(fileName)) return;

        if (IsXml)
            SerializeXml();
        else
            SerializeJson();
    }
    /// <summary>
    /// Сериализация xml
    /// </summary>
    private void SerializeXml()
    {
        XmlSerializer xs = new XmlSerializer(typeof(List<Composition>));
        using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
        {
            xs.Serialize(sw, Compositions);
        }
    }
    /// <summary>
    /// Сериализация json
    /// </summary>
    private void SerializeJson()
    {
        using (FileStream file = File.Create(fileName))
        {
            JsonSerializer.Serialize(file, Compositions, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true,
            }); ;
        }
    }
    #endregion

    #region Deserialization
    /// <summary>
    /// Десериализация из файла
    /// </summary>
    private void LoadFromFile()
    {
        if (string.IsNullOrEmpty(fileName)) return;
        if (IsXml)
            LoadFromXml();
        else
            LoadFromJSon();

    }
    /// <summary>
    /// Десериализация json
    /// </summary>
    private void LoadFromJSon()
    {
        if (string.IsNullOrEmpty(fileName)) return;
        using (FileStream file = File.OpenRead(fileName))
        {
            Compositions =
                (List<Composition>)JsonSerializer.Deserialize(file, typeof(List<Composition>),
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                })!
                ?? new List<Composition>();

        }

    }
    /// <summary>
    /// Десериализация xml
    /// </summary>
    private void LoadFromXml()
    {
        if (string.IsNullOrEmpty(fileName)) return;

        XmlSerializer xs = new XmlSerializer(typeof(List<Composition>));
        using (FileStream file = File.OpenRead(fileName))
        {
            Compositions = ((List<Composition>)xs.Deserialize(file)!) ?? new List<Composition>();
        }

    }
    #endregion
}
