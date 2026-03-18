using Diccionarios.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Diccionarios.BBDD.Entities;

namespace Diccionarios.BBDD;

/// <summary>
/// Proveedor de diccionarios basado en Entity Framework Core + SQLite.
/// </summary>
/// <remarks>
/// Características:
/// - Inicialización lazy de la base de datos (no bloquea el constructor)
/// - Usa índices funcionales UPPER() para búsquedas case-insensitive eficientes
/// - Los diccionarios se resuelven bajo demanda con Include() para evitar N+1
/// </remarks>
public class ProveedorDiccionariosBBDD : IProveedorDiccionarios
{
    private readonly DiccionariosDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProveedorDiccionariosBBDD> _logger;
    private readonly Lazy<Task> _inicializacionLazy;

    public ProveedorDiccionariosBBDD(
        DiccionariosDbContext context, 
        IConfiguration configuration, 
        ILogger<ProveedorDiccionariosBBDD> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Inicialización lazy para no bloquear el constructor
        _inicializacionLazy = new Lazy<Task>(InicializarBaseDeDatosAsync);
        
        _logger.LogInformation("ProveedorDiccionariosBBDD inicializado correctamente");
    }

    private async Task InicializarBaseDeDatosAsync()
    {
        // Usar un logger null para el inicializador para evitar ruido en logs
        var nullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<DatabaseInitializer>.Instance;
        
        var initializer = new DatabaseInitializer(_context, nullLogger);
        await initializer.InicializarSiEsNecesarioAsync();
    }

    private async Task EnsureInitializedAsync()
    {
        await _inicializacionLazy.Value;
    }

    public bool TienesDiccionarioDe(string idioma)
    {
        if (string.IsNullOrEmpty(idioma)) return false;
        
        // Asegurar que la base de datos esté inicializada
        EnsureInitializedAsync().GetAwaiter().GetResult();
        
        var codigoIdioma = idioma.ToUpperInvariant();
        
        // ✅ OPTIMIZACIÓN: Usar UPPER() para aprovechar el índice funcional IX_Idiomas_Codigo_Upper
        // Esto permite búsquedas case-insensitive sin depender de normalización previa
        var existe = _context.Idiomas
            .Any(i => i.Codigo.ToUpper() == codigoIdioma);
        
        _logger.LogDebug("Idioma '{Idioma}' {Estado} en la base de datos", 
            idioma, existe ? "encontrado" : "no encontrado");
            
        return existe;
    }

    public IDiccionario? DameDiccionarioDe(string idioma)
    {
        if (string.IsNullOrEmpty(idioma)) return null;

        // Asegurar que la base de datos esté inicializada
        EnsureInitializedAsync().GetAwaiter().GetResult();

        var codigoIdioma = idioma.ToUpperInvariant();

        var diccionarioEntity = _context.Diccionarios
            .Include(d => d.Idioma)
            // Usa índice funcional IX_Idiomas_Codigo_Upper para búsqueda case-insensitive
            .FirstOrDefault(d => d.Idioma.Codigo.ToUpper() == codigoIdioma);

        if (diccionarioEntity == null)
        {
            _logger.LogWarning("No se encontró diccionario para el idioma '{Idioma}'", idioma);
            return null;
        }

        _logger.LogInformation("Diccionario encontrado para idioma '{Idioma}': {Nombre}",
            idioma, diccionarioEntity.Nombre);

        // Crear logger específico para el diccionario
        var diccionarioLogger = _logger as ILogger<DiccionarioBBDD> ??
            Microsoft.Extensions.Logging.Abstractions.NullLogger<DiccionarioBBDD>.Instance;

        return new DiccionarioBBDD(_context, diccionarioEntity, diccionarioLogger);
    }

    public IList<IDiccionario>? GetDiccionarios(string codigoIdioma)
    {
        if (string.IsNullOrEmpty(codigoIdioma)) return null;

        // Asegurar que la base de datos esté inicializada
        EnsureInitializedAsync().GetAwaiter().GetResult();

        var codigoIdiomaUpper = codigoIdioma.ToUpperInvariant();

        var diccionariosEntity = _context.Diccionarios
            .Include(d => d.Idioma)
            .Where(d => d.Idioma.Codigo.ToUpper() == codigoIdiomaUpper)
            .ToList();

        if (!diccionariosEntity.Any())
        {
            _logger.LogWarning("No se encontraron diccionarios para el idioma '{Idioma}'", codigoIdioma);
            return null;
        }

        _logger.LogInformation("Encontrados {Count} diccionarios para el idioma '{Idioma}'", 
            diccionariosEntity.Count, codigoIdioma);

        var diccionarios = new List<IDiccionario>();
        foreach (var diccionarioEntity in diccionariosEntity)
        {
            var diccionarioLogger = _logger as ILogger<DiccionarioBBDD> ??
                Microsoft.Extensions.Logging.Abstractions.NullLogger<DiccionarioBBDD>.Instance;
            
            diccionarios.Add(new DiccionarioBBDD(_context, diccionarioEntity, diccionarioLogger));
        }

        return diccionarios;
    }

    public IDiccionario? GetDiccionarioPorCodigo(string codigoDiccionario)
    {
        if (string.IsNullOrEmpty(codigoDiccionario)) return null;

        // Asegurar que la base de datos esté inicializada
        EnsureInitializedAsync().GetAwaiter().GetResult();

        var codigoUpper = codigoDiccionario.ToUpperInvariant();

        var diccionarioEntity = _context.Diccionarios
            .Include(d => d.Idioma)
            .FirstOrDefault(d => d.Codigo.ToUpper() == codigoUpper);

        if (diccionarioEntity == null)
        {
            _logger.LogWarning("No se encontró diccionario con código '{Codigo}'", codigoDiccionario);
            return null;
        }

        _logger.LogInformation("Diccionario encontrado con código '{Codigo}': {Nombre}",
            codigoDiccionario, diccionarioEntity.Nombre);

        var diccionarioLogger = _logger as ILogger<DiccionarioBBDD> ??
            Microsoft.Extensions.Logging.Abstractions.NullLogger<DiccionarioBBDD>.Instance;

        return new DiccionarioBBDD(_context, diccionarioEntity, diccionarioLogger);
    }

    public IList<IIdioma> GetIdiomas()
    {
        // Asegurar que la base de datos esté inicializada
        EnsureInitializedAsync().GetAwaiter().GetResult();

        var idiomasEntity = _context.Idiomas.ToList();

        if (!idiomasEntity.Any())
        {
            _logger.LogWarning("No se encontraron idiomas en la base de datos");
            return new List<IIdioma>();
        }

        var idiomas = new List<IIdioma>();
        foreach (var idiomaEntity in idiomasEntity)
        {
            idiomas.Add(new IdiomaBBDD(idiomaEntity));
        }

        _logger.LogInformation("Retornando {Count} idiomas disponibles", idiomas.Count);
        
        return idiomas;
    }
}
    // Ir al contexto de BBDD y obtener los idiomas disponibles: List<IdiomaEntity>
    // Convertir cada IdiomaEntity en un IdiomaBBDD (que implementa IIdioma) mapeo de Entidad al DTO
    // Esta función, ya que recibe un logger.. quiero que haga loging de cosas?
        // Qué debería logear?
        // - Si no hay idiomas, logear un warning
        // - Cuántos idiomas se han encontrado y se van a retornar, logear un info
    // Retornar la lista de IIdioma al consumidor


    // COPILOT (chatbot) --Contexto -> LLM (IA)
    //                   --Prompt --->
    // Ficheros en el proyecto:
    //. - Arquitectura
    //. - Decisiones de implementación de test