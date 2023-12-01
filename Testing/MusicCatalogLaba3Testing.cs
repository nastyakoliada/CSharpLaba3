using Music.Catalog.Lab3;

namespace Testing;
/// <summary>
/// Класс для тестировнаия различных сериализаций музыкального каталога
/// При тестировании  проверяется, что сериализация происходит.
/// 1. Провереятся пустота каталога
/// 2. В каталог заносятся композиции
/// 3. Создается новый каталог, он должен десериализоваться тем, что сохранилось в первый каталог
/// 4. Проверяется равенство первого и второго каталога
/// 4. Далее проверяются команды на поиск и удаление композиций из каталога
/// </summary>
public class MusicCatalogLaba3Testing
{
    /// <summary>
    /// Список для тестирования
    /// </summary>
    private List<Composition> _compositions = new List<Composition> { 
        new Composition
        {
            Author = "Пол Маккартни",
            SongName = "Герл",
        },
        new Composition
        {
            Author = "АББА",
            SongName = "Мери Крисмас"
        }
    };
    /// <summary>
    /// Тестировнаие xml сериализации
    /// </summary>
    [Fact]
    public void TestXmlSerialization()
    {
        TestSerialization(() => new MusicCatalog(new MCSerializerXml("МузКаталог.xml")));
    }
    /// <summary>
    /// Тестирование json сериализации
    /// </summary>

    [Fact]
    public void TestJsonSerialization()
    {
        TestSerialization(() => new MusicCatalog(new MCSerializerJSon("МузКаталог.json")));
    }
    /// <summary>
    /// Тестирование SQLite
    /// </summary>
    [Fact]
    public void TestSQLiteSerialization()
    {
        TestSerialization(() => new MusicCatalogSQLite("МузКаталог.db"));
    }
    /// <summary>
    /// Тестирование на сериализацию указанного экземпляра <see cref="IMusicCatalog"/>
    /// </summary>
    /// <param name="createNew">Делегат для создания экземпляра <see cref="IMusicCatalog"/></param>
    private void TestSerialization(Func<IMusicCatalog> createNew)
    {
        IMusicCatalog catalog = createNew();

        // Очистим от предыдущих проверок
        catalog.Remove(_compositions[0].Author);
        catalog.Remove(_compositions[1].Author);

        //Проверяем, что каталог пустой
        Assert.Empty(catalog.EnumerateAllCompositions());

        // Заносим в каталог новые данные
        foreach (var composition in _compositions) catalog.AddComposition(composition);

        // Проверяем, что там есть то, что занесли
        TestExistance(catalog);

        // Создаем второй каталог и проверяем, что там есть то, что заносили в первый
        IMusicCatalog catalog2 = createNew();

        //Проверяем что там есть то, что занесли в первый
        TestExistance(catalog2);

        // Теперь удалим первого и проверим, что оастался второй

        catalog2.Remove(_compositions[0].Author);

        Assert.Single(catalog2.EnumerateAllCompositions());
        Assert.Collection<Composition>(catalog2.EnumerateAllCompositions(),
            c => Assert.Equal(_compositions[1].Author, c.Author));

        // Теперь удалим второго и убедимся, что коллекция пустая
        catalog2.Remove(_compositions[1].Author);
        Assert.Empty(catalog2.EnumerateAllCompositions());

    }
    /// <summary>
    /// Метод для проверки, что в каталоге есть то и только то, что в него занесли
    /// </summary>
    /// <param name="catalog"></param>
    private void TestExistance(IMusicCatalog catalog)
    {
        //проверяем, что они там есть и по алфавиту

        Assert.Collection<Composition>(catalog.EnumerateAllCompositions(),
            c =>
            {
                Assert.Equal(_compositions[1].Author, c.Author);
                Assert.Equal(_compositions[1].SongName, c.SongName);
            },
            c =>
            {
                Assert.Equal(_compositions[0].Author, c.Author);
                Assert.Equal(_compositions[0].SongName, c.SongName);
            }
            );

        //Проверяем, что их там 2 штуки
        Assert.Equal(_compositions.Count(), catalog.EnumerateAllCompositions().Count());
    }

    /// <summary>
    /// Тетирование коммандера с фейковы музыкальнмы каталогом
    /// Проверяем, что команды пользователя из консоли доходят в нужные методы каталога
    /// </summary>
    [Fact]
    public void TestCommanderRun()
    {
        //Создаем фековый каталог, который будет получать вызодвы из консоли
        MockMusicCatalog catalog = new MockMusicCatalog();

        MusicCatalogCommander commander = new MusicCatalogCommander(catalog);

        //Создаем источник команд
        using(var sr = new StringReader("""
            Add
            Pol Mccartney
            Girl
            Search
            Pol
            Remove
            Girl
            list
            quit
            """))
        {
            //Устанавливаем, что ввод с консоли будет из созданного источника команд
            Console.SetIn(sr);
            commander.CommandsLoop();
        }
        // После отработки всех команд коммандром, проверяем, что все команды прошли через фейковый каталог
        Assert.Single(catalog.Compositions);
        // Проверяем, что в каталоге есть занесенная композиция
        Assert.Collection<Composition>(catalog.Compositions,
            c =>
            {
                Assert.Equal("Pol Mccartney", c.Author);
                Assert.Equal("Girl", c.SongName);
            });
        // Проверяем, что прошла команда Search
        Assert.Equal("Pol", catalog.SearhQuery);
        // Проверяем, что прошла команда Remove
        Assert.Equal("Girl", catalog.RemoveQuery);
        // Проверяем, что прошла команда List
        Assert.Equal(1, catalog.ListCallsNumber);

    }
}
