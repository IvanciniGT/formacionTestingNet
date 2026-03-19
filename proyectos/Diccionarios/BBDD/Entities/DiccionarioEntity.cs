namespace Diccionarios.BBDD.Entities;

/// <summary>
/// Entidad EF Core que representa un diccionario (ej: RAE, Oxford).
/// Pertenece a un idioma y contiene múltiples palabras (relación 1:N).
/// </summary>
public class DiccionarioEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int IdiomaId { get; set; }
    
    // Navigation properties
    public IdiomaEntity Idioma { get; set; } = null!;
    public ICollection<PalabraEntity> Palabras { get; set; } = new List<PalabraEntity>();
}