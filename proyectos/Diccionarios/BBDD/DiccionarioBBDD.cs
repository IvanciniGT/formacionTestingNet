using Diccionarios.Api;
using Diccionarios.BBDD.Entities;
using Microsoft.Extensions.Logging;

namespace Diccionarios.BBDD;

public class DiccionarioBBDD : IDiccionario
{
    private readonly DiccionariosDbContext _context;
    private readonly DiccionarioEntity _diccionarioEntity;
    private readonly ILogger<DiccionarioBBDD> _logger;

    public DiccionarioBBDD(DiccionariosDbContext context, DiccionarioEntity diccionarioEntity, ILogger<DiccionarioBBDD> logger)
    {
        _context = context;
        _diccionarioEntity = diccionarioEntity;
        _logger = logger;
    }

    public string Idioma => _diccionarioEntity.Idioma.Codigo;
    public string Codigo => _diccionarioEntity.Codigo;
    public string Nombre => _diccionarioEntity.Nombre;

    public bool Existe(string palabra)
    {
        if (palabra == null) return false;
        
        var palabraNormalizada = NormalizarPalabra(palabra);
        
        var existe = _context.Palabras
            .Any(p => p.DiccionarioId == _diccionarioEntity.Id && 
                     p.Texto.ToUpper() == palabraNormalizada);
        
        _logger.LogDebug("Palabra '{Palabra}' {Estado} en diccionario {Idioma}", 
            palabra, existe ? "existe" : "no existe", Idioma);
            
        return existe;
    }

    public IList<string>? GetSignificados(string palabra)
    {
        if (palabra == null) return null;
        
        var palabraNormalizada = NormalizarPalabra(palabra);
        
        var significados = _context.Palabras
            .Where(p => p.DiccionarioId == _diccionarioEntity.Id && 
                       p.Texto.ToUpper() == palabraNormalizada)
            .SelectMany(p => p.Significados)
            .Select(s => s.Texto)
            .ToList();
            
        if (!significados.Any()) return null;
        
        _logger.LogDebug("Encontrados {Count} significados para '{Palabra}' en {Idioma}", 
            significados.Count, palabra, Idioma);
            
        return significados;
    }
    
    private static string NormalizarPalabra(string palabra)
    {
        if (string.IsNullOrEmpty(palabra))
            return string.Empty;
            
        return palabra.Trim().ToUpperInvariant();
    }
}