namespace Diccionarios.Api;

/// <summary>
/// Representa un idioma disponible en el sistema de diccionarios.
/// </summary>
public interface IIdioma
{
    /// <summary>Nombre descriptivo del idioma (ej: "Español", "English")</summary>
    string Nombre { get; }
    
    /// <summary>Código ISO del idioma (ej: "ES", "EN")</summary>
    string Codigo { get; }
}
