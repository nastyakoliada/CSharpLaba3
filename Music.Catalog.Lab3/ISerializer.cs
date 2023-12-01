
namespace Music.Catalog.Lab3;
/// <summary>
/// Интерфейс для сериализации
/// </summary>
public interface ISerializer<T>
{
    /// <summary>
    /// Сериализация объекта
    /// </summary>
    /// <param name="compositions"></param>
    void Serialize(T compositions);
    /// <summary>
    /// Десериализация объекта
    /// </summary>
    /// <returns></returns>
    T Deserialize();
}
