using System.Xml.Serialization;

namespace Music.Catalog.Lab3;
/// <summary>
/// Сериализатор xml 
/// </summary>
public class MCSerializerXml:ISerializer<List<Composition>>
{
    /// <summary>
    /// Полное имя файла для сериализации
    /// </summary>
    private readonly string fileName;
    public MCSerializerXml(string fileName)
    {
        this.fileName = fileName;
    }
    /// <summary>
    /// Десериализация списка композиций
    /// </summary>
    /// <returns>Список композиций</returns>
    public List<Composition> Deserialize()
    {
        if (string.IsNullOrEmpty(fileName)) return null!;
        if(!File.Exists(fileName)) return null!;

        XmlSerializer xs = new XmlSerializer(typeof(List<Composition>));
        using (FileStream file = File.OpenRead(fileName))
        {
            return ((List<Composition>)xs.Deserialize(file)!) ?? new List<Composition>();
        }
    }
    /// <summary>
    /// Сериализация списка композиций
    /// </summary>
    /// <param name="compositions">Список композиций</param>
    public void Serialize(List<Composition> compositions)
    {
        XmlSerializer xs = new XmlSerializer(typeof(List<Composition>));
        using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
        {
            xs.Serialize(sw, compositions);
        }
    }
}
