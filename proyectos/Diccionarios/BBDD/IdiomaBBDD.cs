using Diccionarios.Api;
using Diccionarios.BBDD.Entities;

namespace Diccionarios.BBDD;

/// <summary>
/// Implementación de IIdioma que encapsula información de un idioma desde la base de datos
/// </summary>
public class IdiomaBBDD : IIdioma
{
    private readonly IdiomaEntity _idiomaEntity;

    public IdiomaBBDD(IdiomaEntity idiomaEntity)
    {
        _idiomaEntity = idiomaEntity ?? throw new ArgumentNullException(nameof(idiomaEntity));
    }

    public string Nombre => _idiomaEntity.Nombre;
    public string Codigo => _idiomaEntity.Codigo;
}