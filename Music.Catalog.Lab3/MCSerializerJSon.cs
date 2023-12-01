using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Music.Catalog.Lab3;
/// <summary>
/// Сериализатор JSon
/// </summary>
public class MCSerializerJSon : ISerializer<List<Composition>>
{
    /// <summary>
    ///  Полное имя файла для сериализации
    /// </summary>
    private readonly string fileName;
    /// <summary>
    /// Параметризованный конструктор
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    public MCSerializerJSon(string fileName)
    {
        this.fileName=fileName;
    }
    /// <summary>
    /// Десериализация списка композиций
    /// </summary>
    /// <returns>Список композиций</returns>
    public List<Composition> Deserialize()
    {
        if (string.IsNullOrEmpty(fileName)) return null!;
        if (!File.Exists(fileName)) return null!;

        using (FileStream file = File.OpenRead(fileName))
        {
            return (List<Composition>)JsonSerializer.Deserialize(file, typeof(List<Composition>),
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                })!
                ?? new List<Composition>();

        }
    }
    /// <summary>
    /// Сериализация списка композиций
    /// </summary>
    /// <param name="compositions">Список композиций</param>

    public void Serialize(List<Composition> compositions)
    {
        using (FileStream file = File.Create(fileName))
        {
            JsonSerializer.Serialize(file, compositions, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true,
            }); ;
        }
    }
}
