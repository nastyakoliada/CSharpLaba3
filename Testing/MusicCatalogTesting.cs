namespace Testing;
using Music.Catalog.Lab3;

/// <summary>
/// Класс для тестирования музыкального каталога к лабе 3. 
/// При тестировании используется фейковый сериалайзе. В котором предопрелены две композиции
/// для десериализации.
/// </summary>
public class MusicCatalogTesting
{
    MusicCatalog catalog = null!;
    /// <summary>
    /// Вспомогательный метод создает заполннный каталог
    /// </summary>
    private MockSerializer CreateTestCatalog()
    {
        MockSerializer ts = new();
        catalog = new MusicCatalog(ts);
        return ts;
    }
    /// <summary>
    /// Тестирование заполнение каталога
    /// </summary>
    [Fact]
    public void AddTesting()
    {
        // Создаем каталог, получаем фейковый сериализатор для проверок
        var ts = CreateTestCatalog();
        // Добавляем новую композицию
        catalog.AddComposition(new Composition { Author = "Billy Joel", SongName = "Piano man" });

        //Проверяем, что в сериализаторе прошел вызов сериализации всех композиций
        Assert.Collection<Composition>(ts.SerializedCompositions,
            c =>
            {
                Assert.Equal("Sia", c.Author);
                Assert.Equal("Forever", c.SongName);
            },
            c => {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            },
            c => {
                Assert.Equal("Billy Joel", c.Author);
                Assert.Equal("Piano man", c.SongName);
            }
            );
        // Проверияем, что их там 3
        Assert.Equal(3,ts.SerializedCompositions.Count());
    }
    // Тестирование на то, что работает команда на получение всех композиций
    [Fact]
    public void EnumerateTesting()
    {
        CreateTestCatalog();
        Assert.Collection<Composition>(catalog.EnumerateAllCompositions(),
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            },
            c =>
            {
                Assert.Equal("Sia", c.Author);
                Assert.Equal("Forever", c.SongName);
            }
            );
    }
    /// <summary>
    /// Тестирование удаления композиций из каталога
    /// </summary>
    [Fact]
    public void RemoveTesting()
    {
        // создаем каталог, с использованием фекового сериализатора
        var ts = CreateTestCatalog();
        // Проверяем, что после удаления, сериализатор получил для сохранения только одну композицию
        catalog.Remove("Sia");
        Assert.Collection<Composition>(ts.SerializedCompositions,
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            }
            );
        Assert.Single(ts.SerializedCompositions);
    }
    /// <summary>
    /// Тестировнаие поиска в каталоге
    /// </summary>
    [Fact]
    public void SearchTesting()
    {
        CreateTestCatalog();

        var sch = catalog.Search("Bil");

        Assert.Collection<Composition>(sch,
            c =>
            {
                Assert.Equal("Billie Eilish", c.Author);
                Assert.Equal("Lovely", c.SongName);
            }
            );
        Assert.Single(sch);
    }
}