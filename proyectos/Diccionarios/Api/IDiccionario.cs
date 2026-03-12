namespace Diccionarios.Api;

/// <summary>
/// Representa un diccionario de un idioma específico.
/// Permite consultar existencia de palabras y obtener significados.
/// </summary>
public interface IDiccionario
{
    /// <summary>Código del idioma del diccionario (ej: "ES", "EN")</summary>
    string Idioma { get; }
    
    /// <summary>Código único del diccionario. Implementación por defecto: "DIC_" + Idioma</summary>
    string Codigo { get => "DIC_" + Idioma; }
    
    /// <summary>Nombre descriptivo del diccionario. Implementación por defecto: "Diccionario " + Codigo</summary>
    string Nombre { get => "Diccionario " + Codigo; }

    /// <summary>
    /// Comprueba si una palabra existe en el diccionario.
    /// </summary>
    /// <param name="palabra">Palabra a buscar</param>
    /// <returns>true si la palabra existe, false en caso contrario</returns>
    bool Existe(string palabra);

    /// <summary>
    /// Obtiene los significados de una palabra.
    /// </summary>
    /// <param name="palabra">Palabra a buscar</param>
    /// <returns>Lista de significados o null si la palabra no existe</returns>
    IList<string>? GetSignificados(string palabra);
}
