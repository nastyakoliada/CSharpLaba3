using Music.Catalog.Lab3;

namespace Testing
{
    /// <summary>
    /// Фейковый сериализатор для проверки прохождения команд на сериализацию
    /// </summary>
    internal class MockSerializer : ISerializer<List<Composition>>
    {
        /// <summary>
        /// Начальный список композиций, как бы десериализированный
        /// </summary>
        public List<Composition> Compositions { get; set; } = new List<Composition>()
        {
            new Composition
            {
                Author = "Sia",
                SongName = "Forever",
            },
            new Composition
            {
                Author = "Billie Eilish",
                SongName = "Lovely",
            }
        };
        /// <summary>
        /// Список композиций, который нужно сериализовать
        /// </summary>
        public List<Composition> SerializedCompositions { get; set; } = null!;
        /// <summary>
        /// Метод интерфейса <see cref="ISerializer{T}"/>
        /// </summary>
        /// <returns>Возвращает предопреленный список композиций</returns>
        public List<Composition> Deserialize()
        {
            return Compositions;
        }
        /// <summary>
        /// Метод интерфейса <see cref="ISerializer{T}"/> сохранаяет полученный список композиций для сериализации
        /// </summary>
        public void Serialize(List<Composition> compositions)
        {
            SerializedCompositions = compositions;
        }
    }
}
