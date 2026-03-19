namespace Diccionarios.BBDD.Entities;

/// <summary>
/// Entidad EF Core que representa un idioma (ej: Español, Inglés).
/// Un idioma tiene múltiples diccionarios asociados (relación 1:N).
/// </summary>
public class IdiomaEntity
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<DiccionarioEntity> Diccionarios { get; set; } = new List<DiccionarioEntity>();
}