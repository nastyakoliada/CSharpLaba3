namespace Music.Catalog.Lab3;
/// <summary>
/// Класс содержит перечень музыкальных композиций и предоставляет методы по работе с ним.
/// Поддерживается сериализация с исполльзование <see cref="ISerializer{T}"/>
/// </summary>
public class MusicCatalog : IMusicCatalog
{
    /// <summary>
    /// Сериализатор для использования
    /// </summary>
    private readonly ISerializer<List<Composition>> serializer = null!;
    /// <summary>
    /// Конструктор с указанием сериализатора
    /// </summary>
    /// <param name="serializer">Сериализатор для использования в дальнейшем</param>
    public MusicCatalog(ISerializer<List<Composition>> serializer)
    {
        this.serializer = serializer;
        Compositions = serializer.Deserialize()  ?? new List<Composition>();
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

    /// <summary>
    /// Сериализация каталога.
    /// </summary>
    private void Serialize()
    {
        serializer?.Serialize(Compositions);
    }
    
}
