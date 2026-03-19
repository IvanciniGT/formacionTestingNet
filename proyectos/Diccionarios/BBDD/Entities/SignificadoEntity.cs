namespace Diccionarios.BBDD.Entities;

/// <summary>
/// Entidad EF Core que representa un significado/definición de una palabra.
/// Cada palabra puede tener múltiples significados.
/// </summary>
public class SignificadoEntity
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int PalabraId { get; set; }
    
    // Navigation properties
    public PalabraEntity Palabra { get; set; } = null!;
}