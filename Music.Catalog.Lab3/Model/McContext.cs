using Microsoft.EntityFrameworkCore;
namespace Music.Catalog.Lab3.Model;
/// <summary>
/// Контекст для работы с бд SQLite
/// </summary>
public class McContext : DbContext
{
    /// <summary>
    /// Имя файла бд
    /// </summary>
    private string fileName = string.Empty;

    public DbSet<Composition> Compositions { get; set; }
    public McContext(string fileName)
    {
        this.fileName = fileName;
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Filename={fileName}");
    }
    
}


