namespace Diccionarios.Ficheros;

using Diccionarios.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Proveedor de diccionarios basado en ficheros de texto.
/// Los ficheros deben estar en formato: palabra=significado1|significado2|...
/// </summary>
/// <remarks>
/// Implementa lazy loading con WeakReference para optimizar memoria:
/// los diccionarios se cargan bajo demanda y pueden ser liberados por el GC.
/// </remarks>
public class ProveedorDiccionariosFicheros : IProveedorDiccionarios
{
    private readonly string _rutaCarpetaDiccionarios;
    private readonly Dictionary<string, WeakReference<IDiccionario>> _cacheDiccionarios = new();
    private readonly ILogger<ProveedorDiccionariosFicheros> _logger;

    /// <summary>
    /// Constructor principal con inyección de dependencias.
    /// </summary>
    /// <param name="configuration">Configuración que debe contener DiccionariosConfig:RutaCarpetaDiccionarios</param>
    /// <param name="logger">Logger para trazabilidad</param>
    public ProveedorDiccionariosFicheros(IConfiguration configuration, ILogger<ProveedorDiccionariosFicheros> logger)
    {
        _logger = logger;
        
        try
        {
            // Leer configuración directamente
            var rutaRelativa = configuration["DiccionariosConfig:RutaCarpetaDiccionarios"];
            
            if (string.IsNullOrWhiteSpace(rutaRelativa))
            {
                throw new InvalidOperationException("No se encontró la configuración 'DiccionariosConfig:RutaCarpetaDiccionarios' en appsettings.json");
            }

            _rutaCarpetaDiccionarios = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
            
            _logger.LogInformation("ProveedorDiccionariosFicheros inicializado con ruta: {RutaCarpeta}", _rutaCarpetaDiccionarios);
            
            // Verificar que la carpeta existe
            if (!Directory.Exists(_rutaCarpetaDiccionarios))
            {
                _logger.LogWarning("La carpeta de diccionarios no existe: {RutaCarpeta}", _rutaCarpetaDiccionarios);
                // Pararmos la ejecución
                throw new InvalidOperationException($"La carpeta de diccionarios no existe: {_rutaCarpetaDiccionarios}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar ProveedorDiccionariosFicheros");
            throw;
        }
    }

    /// <summary>Constructor para tests - no requiere configuración ni logging.</summary>
    public ProveedorDiccionariosFicheros(string rutaCarpetaDiccionarios)
    {
        _rutaCarpetaDiccionarios = rutaCarpetaDiccionarios;
        _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ProveedorDiccionariosFicheros>.Instance;
    }

    public IDiccionario? DameDiccionarioDe(string idioma)
    {
        _logger.LogDebug("Solicitando diccionario para idioma: {Idioma}", idioma);
        
        if (!TienesDiccionarioDe(idioma))
        {
            _logger.LogWarning("No se encontró diccionario para el idioma: {Idioma}", idioma);
            return null;
        }
        
        if (!TengoUnIdiomaEnCache(idioma))
        {
            _logger.LogInformation("Cargando diccionario para idioma: {Idioma}", idioma);
            MeterDiccionarioEnCache(idioma);
        }
        else
        {
            _logger.LogDebug("Diccionario para idioma {Idioma} encontrado en cache", idioma);
        }
        
        return SacarDiccionarioDeCache(idioma);
    }

    public bool TienesDiccionarioDe(string idioma)
    {
        return TengoUnIdiomaEnCache(idioma) || Utilidades.TengoUnArchivoParaElIdioma(idioma, _rutaCarpetaDiccionarios);
    }

    #region Cache Management
    
    private bool TengoUnIdiomaEnCache(string idioma)
    {
        return _cacheDiccionarios.TryGetValue(idioma, out var referenciaDebilAlDiccionario) &&
               referenciaDebilAlDiccionario.TryGetTarget(out var diccionarioEnCache);
    }

    private void MeterDiccionarioEnCache(string idioma)
    {
        var palabrasYDefiniciones = LeerElFicheroDePalabrasYProcesarlo(idioma);
        crearYMeterDiccionarioEnCache(idioma, palabrasYDefiniciones);
    }

    private void crearYMeterDiccionarioEnCache(string idioma, Dictionary<string, IList<string>> palabrasYDefiniciones)
    {
        var diccionarioADevolver = new DiccionarioFichero(idioma, palabrasYDefiniciones);
        // WeakReference permite que el GC libere el diccionario si hay presión de memoria
        _cacheDiccionarios[idioma] = new WeakReference<IDiccionario>(diccionarioADevolver);
    }

    private IDiccionario SacarDiccionarioDeCache(string idioma)
    {
        _cacheDiccionarios.TryGetValue(idioma, out var referenciaDebilAlDiccionario);
        referenciaDebilAlDiccionario.TryGetTarget(out var diccionarioEnCache);
        return diccionarioEnCache;
    }
    
    #endregion

    #region File Parsing
    
    /// <summary>
    /// Lee y parsea el fichero de diccionario usando LINQ (enfoque funcional).
    /// Formato esperado: palabra=significado1|significado2|...
    /// Líneas vacías y comentarios (#) son ignorados.
    /// </summary>
    private Dictionary<string, IList<string>> LeerElFicheroDePalabrasYProcesarlo(string idioma)
    {
        var rutaFicheroDiccionario = Path.Combine(_rutaCarpetaDiccionarios, $"{idioma}.txt");

        return File.ReadLines(rutaFicheroDiccionario)
            .Where(linea => !string.IsNullOrWhiteSpace(linea) && !linea.TrimStart().StartsWith('#'))
            .Select(linea => linea.Split('=', 2))
            .ToDictionary(
                partes => Utilidades.NormalizarPalabra(partes[0]),
                partes => (IList<string>)partes[1].Split('|', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
    }
    
    #endregion


}
