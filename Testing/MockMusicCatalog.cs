using Music.Catalog.Lab3;

namespace Testing
{
    /// <summary>
    /// Фейковый музыкальный каталог. Используется для проверки работы коммандера, который
    /// взаимедойствует с пользователм
    /// </summary>
    internal class MockMusicCatalog : IMusicCatalog
    {
        /// <summary>
        /// Список композиций для проверки прохождения команды add
        /// </summary>
        public List<Composition> Compositions { get; set; } = new List<Composition>();
        /// <summary>
        /// Полученный запрос на поиск  - для проверки команды search
        /// </summary>
        public string SearhQuery { get; set; } = null!;
        /// <summary>
        /// Полученный запрос на удаление композиции - для проверки прохождения команды Remove
        /// </summary>
        public string RemoveQuery { get; set; } = null!;
        /// <summary>
        /// Счетчик запросов полного списка - для проверки прохожденния команды list
        /// </summary>
        public int ListCallsNumber { get; set; }
        
        /// <summary>
        /// Метод интерфейса <see cref="IMusicCatalog"/>
        /// </summary>
        /// <param name="composition"></param>
        public void AddComposition(Composition composition)
        {
            Compositions.Add(composition);
        }
        /// <summary>
        /// Метод интерфейса <see cref="IMusicCatalog"/>
        /// </summary>

        public IEnumerable<Composition> EnumerateAllCompositions()
        {
            ListCallsNumber++;
            return Compositions;
        }
        /// <summary>
        /// Метод интерфейса <see cref="IMusicCatalog"/>
        /// </summary>

        public int Remove(string query)
        {
            RemoveQuery = query;
            return 0;
        }
        /// <summary>
        /// Метод интерфейса <see cref="IMusicCatalog"/>
        /// </summary>

        public IEnumerable<Composition> Search(string query)
        {
            SearhQuery = query;
            return Compositions;
        }
    }
}
