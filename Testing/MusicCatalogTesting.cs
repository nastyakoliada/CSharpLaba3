namespace Testing;
using Music.Catalog.Lab3;

/// <summary>
/// ����� ��� ������������ ������������ �������� � ���� 3. 
/// ��� ������������ ������������ �������� ����������. � ������� ������������ ��� ����������
/// ��� ��������������.
/// </summary>
public class MusicCatalogTesting
{
    MusicCatalog catalog = null!;
    /// <summary>
    /// ��������������� ����� ������� ���������� �������
    /// </summary>
    private MockSerializer CreateTestCatalog()
    {
        MockSerializer ts = new();
        catalog = new MusicCatalog(ts);
        return ts;
    }
    /// <summary>
    /// ������������ ���������� ��������
    /// </summary>
    [Fact]
    public void AddTesting()
    {
        // ������� �������, �������� �������� ������������ ��� ��������
        var ts = CreateTestCatalog();
        // ��������� ����� ����������
        catalog.AddComposition(new Composition { Author = "Billy Joel", SongName = "Piano man" });

        //���������, ��� � ������������� ������ ����� ������������ ���� ����������
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
        // ����������, ��� �� ��� 3
        Assert.Equal(3,ts.SerializedCompositions.Count());
    }
    // ������������ �� ��, ��� �������� ������� �� ��������� ���� ����������
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
    /// ������������ �������� ���������� �� ��������
    /// </summary>
    [Fact]
    public void RemoveTesting()
    {
        // ������� �������, � �������������� �������� �������������
        var ts = CreateTestCatalog();
        // ���������, ��� ����� ��������, ������������ ������� ��� ���������� ������ ���� ����������
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
    /// ������������ ������ � ��������
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