namespace Diccionarios.Api;

/// <summary>
/// Proveedor de diccionarios - punto de entrada para acceder a los diccionarios del sistema.
/// Abstrae la fuente de datos (ficheros, BBDD, etc.) siguiendo el principio de inversión de dependencias.
/// </summary>
/// <remarks>
/// Los métodos con implementación por defecto permiten evolucionar la API manteniendo
/// compatibilidad hacia atrás con implementaciones existentes.
/// </remarks>
public interface IProveedorDiccionarios
{
    /// <summary>Comprueba si existe un diccionario para el idioma especificado.</summary>
    /// <param name="idioma">Código del idioma (ej: "ES", "EN")</param>
    bool TienesDiccionarioDe(string idioma);

    /// <summary>Obtiene el diccionario de un idioma.</summary>
    /// <param name="idioma">Código del idioma</param>
    /// <returns>Diccionario o null si no existe</returns>
    [Obsolete("Usar GetDiccionarios() para soportar múltiples diccionarios por idioma")]
    IDiccionario? DameDiccionarioDe(string idioma);

    /// <summary>Obtiene un diccionario por su código único.</summary>
    /// <param name="codigoDiccionario">Código del diccionario (ej: "DIC_ES")</param>
    IDiccionario? GetDiccionarioPorCodigo(string codigoDiccionario)
    {
        throw new NotImplementedException("Debe implementarse en la clase derivada.");
    }

    /// <summary>Obtiene todos los diccionarios disponibles para un idioma.</summary>
    /// <param name="codigoIdioma">Código del idioma</param>
    IList<IDiccionario>? GetDiccionarios(string codigoIdioma)
    {
        if (TienesDiccionarioDe(codigoIdioma))
        {
            var diccionario = DameDiccionarioDe(codigoIdioma);
            if (diccionario != null)
                return new List<IDiccionario> { diccionario };
        }
        return null;
    }

    /// <summary>Obtiene todos los idiomas disponibles.</summary>
    IList<IIdioma> GetIdiomas()
    {
        throw new NotImplementedException("Debe implementarse en la clase derivada.");
    }
}