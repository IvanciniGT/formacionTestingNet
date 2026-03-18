using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Diccionarios.Api;
using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;

namespace Diccionarios.BBDD.Tests;

/// <summary>
/// Pruebas TDD para ProveedorDiccionariosBBDD
/// ESTAS PRUEBAS VAN A FALLAR INICIALMENTE - ¡ES LO ESPERADO EN TDD! 🔴
/// </summary>
public class ProveedorDiccionariosBBDDTests : ProveedorDiccionariosTestsBase,IDisposable
{
    private readonly DiccionariosDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProveedorDiccionariosBBDD> _logger;

    public ProveedorDiccionariosBBDDTests()
    {
        // Configurar Entity Framework InMemory para pruebas = FAKE de la BBDD
        var options = new DbContextOptionsBuilder<DiccionariosDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nueva DB para cada prueba
            .Options;

        _context = new DiccionariosDbContext(options);

        // Configurar IConfiguration Stub para pruebas que apunta a un FAKE
        var configDict = new Dictionary<string, string>
        {
            {"DiccionariosConfig:ConnectionString", "InMemory"}
        };
        
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Configurar Logger fake
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<ProveedorDiccionariosBBDD>();

        // ¡ESTA LÍNEA VA A FALLAR! - El ProveedorDiccionariosBBDD no existe aún 🔴
        _suministrador = new ProveedorDiccionariosBBDD(_context, _configuration, _logger);

        // Sembrar datos de prueba
        SembrarDatosDePrueba();
    }

    private void SembrarDatosDePrueba()
    {
        // Crear idiomas de prueba
        var idiomas = new[]
        {
            new IdiomaEntity { Codigo = "ES", Nombre = "Español" },
            new IdiomaEntity { Codigo = "EN", Nombre = "English" }
        };
        _context.Idiomas.AddRange(idiomas);
        _context.SaveChanges();

        // Crear múltiples diccionarios por idioma con códigos específicos
        var diccionarios = new[]
        {
            new DiccionarioEntity { IdiomaId = idiomas[0].Id, Nombre = "Diccionario RAE", Codigo = "ES_RAE" },
            new DiccionarioEntity { IdiomaId = idiomas[0].Id, Nombre = "Diccionario Larousse", Codigo = "ES_LAROUSSE" },
            new DiccionarioEntity { IdiomaId = idiomas[1].Id, Nombre = "Oxford Dictionary", Codigo = "EN_OXFORD" }
        };
        _context.Diccionarios.AddRange(diccionarios);
        _context.SaveChanges();

        // Crear palabras de prueba - algunas comunes entre diccionarios del mismo idioma
        var palabras = new[]
        {
            // Palabras COMUNES en español (en ambos diccionarios)
            new PalabraEntity { Texto = "casa", DiccionarioId = diccionarios[0].Id }, // ES_RAE
            new PalabraEntity { Texto = "casa", DiccionarioId = diccionarios[1].Id }, // ES_LAROUSSE
            new PalabraEntity { Texto = "agua", DiccionarioId = diccionarios[0].Id }, // ES_RAE
            new PalabraEntity { Texto = "agua", DiccionarioId = diccionarios[1].Id }, // ES_LAROUSSE
            
            // Palabras ESPECÍFICAS por diccionario
            new PalabraEntity { Texto = "hidalgo", DiccionarioId = diccionarios[0].Id }, // Solo en ES_RAE
            new PalabraEntity { Texto = "champán", DiccionarioId = diccionarios[1].Id }, // Solo en ES_LAROUSSE
            
            // Palabras en inglés
            new PalabraEntity { Texto = "house", DiccionarioId = diccionarios[2].Id } // EN_OXFORD
        };
        _context.Palabras.AddRange(palabras);
        _context.SaveChanges();

        // Crear significados para las palabras
        var significados = new[]
        {
            // Significados para "casa" en ES_RAE
            new SignificadoEntity { Texto = "Edificio para habitar", PalabraId = palabras[0].Id },
            new SignificadoEntity { Texto = "Familia o linaje", PalabraId = palabras[0].Id },
            
            // Significados para "casa" en ES_LAROUSSE
            new SignificadoEntity { Texto = "Vivienda familiar", PalabraId = palabras[1].Id },
            
            // Significados para "agua" en ES_RAE
            new SignificadoEntity { Texto = "Líquido inodoro, incoloro e insípido", PalabraId = palabras[2].Id },
            
            // Significados para "hidalgo" (solo ES_RAE)
            new SignificadoEntity { Texto = "Persona de noble linaje", PalabraId = palabras[4].Id },
            
            // Significados para "house" en inglés
            new SignificadoEntity { Texto = "A building for human habitation", PalabraId = palabras[6].Id }
        };
        _context.Significados.AddRange(significados);
        _context.SaveChanges();
        
        // Crear índices funcionales para optimización de búsquedas case-insensitive
        _context.CreateFunctionalIndexes();
    }

    public void Dispose()
    {
        _context.Dispose(); // Cerrar contexto y limpiar
    }

}