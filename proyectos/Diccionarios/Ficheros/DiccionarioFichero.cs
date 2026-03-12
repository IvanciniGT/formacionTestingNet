namespace Diccionarios.Ficheros;

using Diccionarios.Api;

/// <summary>
/// Implementación de IDiccionario que almacena palabras y definiciones en memoria.
/// Es utilizado internamente por ProveedorDiccionariosFicheros.
/// </summary>
public class DiccionarioFichero : IDiccionario
{
    private readonly Dictionary<string, IList<string>> _palabrasYDefiniciones;

    public DiccionarioFichero(string idioma, Dictionary<string, IList<string>> palabrasYDefiniciones)
    {
        Idioma = idioma;
        _palabrasYDefiniciones = palabrasYDefiniciones;
    }

    public string Idioma { get; }

    public bool Existe(string palabra)
    {
        if (palabra == null) return false;
        var palabraNormalizada = Utilidades.NormalizarPalabra(palabra);
        return !string.IsNullOrEmpty(palabraNormalizada) && _palabrasYDefiniciones.ContainsKey(palabraNormalizada);
    }

    public IList<string>? GetSignificados(string palabra)
    {
        if (palabra == null) return null;
        var palabraNormalizada = Utilidades.NormalizarPalabra(palabra);
        return !string.IsNullOrEmpty(palabraNormalizada) && _palabrasYDefiniciones.TryGetValue(palabraNormalizada, out var significados)
            ? significados
            : null;
    }
}
