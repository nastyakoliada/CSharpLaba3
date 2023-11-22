using System.ComponentModel.DataAnnotations;

namespace Music.Catalog.Lab3.Model;
/// <summary>
/// Музакальная композиция из модели данных
/// </summary>
public class Composition
{
    public int ID { get; set; }
    
    [Required]
    public string? Author { get; set; }
    [Required]
    public string? SongName { get; set; }
}
