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
public class ProveedorDiccionariosBBDDTests : IDisposable
{
    private readonly DiccionariosDbContext _context;
    private readonly IProveedorDiccionarios _suministrador;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProveedorDiccionariosBBDD> _logger;

    public ProveedorDiccionariosBBDDTests()
    {
        // Configurar Entity Framework InMemory para pruebas
        var options = new DbContextOptionsBuilder<DiccionariosDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nueva DB para cada prueba
            .Options;

        _context = new DiccionariosDbContext(options);

        // Configurar IConfiguration fake para pruebas
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

    [Fact]
    public void TienesDiccionarioDe_ConIdiomaExistente_DeberiaRetornarTrue()
    {
        // Arrange
        string idioma = "ES";

        // Act
        bool resultado = _suministrador.TienesDiccionarioDe(idioma);

        // Assert
        Assert.True(resultado, "Debería encontrar el diccionario de español");
    }

    [Fact]
    public void TienesDiccionarioDe_ConIdiomaInexistente_DeberiaRetornarFalse()
    {
        // Arrange
        string idioma = "FR";

        // Act
        bool resultado = _suministrador.TienesDiccionarioDe(idioma);

        // Assert
        Assert.False(resultado, "No debería encontrar el diccionario de francés");
    }

    [Fact]
    public void DameDiccionarioDe_ConIdiomaExistente_DeberiaRetornarDiccionario()
    {
        // Arrange
        string idioma = "ES";

        // Act
        var diccionario = _suministrador.DameDiccionarioDe(idioma);

        // Assert
        Assert.NotNull(diccionario);
        Assert.Equal("ES", diccionario.Idioma);
    }

    [Fact]
    public void DameDiccionarioDe_ConIdiomaInexistente_DeberiaRetornarNull()
    {
        // Arrange
        string idioma = "FR";

        // Act
        var diccionario = _suministrador.DameDiccionarioDe(idioma);

        // Assert
        Assert.Null(diccionario);
    }

    [Fact]
    public void DiccionarioRetornado_DeberiaImplementarInterfazCorrectamente()
    {
        // Arrange
        string idioma = "ES";
        var diccionario = _suministrador.DameDiccionarioDe(idioma);

        // Act & Assert
        Assert.NotNull(diccionario);
        
        // Probar que existe una palabra
        bool existeCasa = diccionario.Existe("casa");
        Assert.True(existeCasa, "La palabra 'casa' debería existir");

        // Probar que no existe una palabra inexistente
        bool existeInexistente = diccionario.Existe("palabrainexistente");
        Assert.False(existeInexistente, "Una palabra inexistente no debería existir");

        // Probar obtener significados (puede ser de cualquier diccionario español)
        var significados = diccionario.GetSignificados("casa");
        Assert.NotNull(significados);
        Assert.True(significados.Count > 0, "Debería haber al menos un significado para 'casa'");
        
        // Verificar que al menos uno de los significados conocidos está presente
        bool tieneSignificadoConocido = significados.Any(s => 
            s.Contains("Edificio") || s.Contains("Vivienda") || s.Contains("habitar") || s.Contains("familiar"));
        Assert.True(tieneSignificadoConocido, "Debería contener un significado conocido para 'casa'");

        // Probar obtener significados de palabra inexistente
        var significadosInexistentes = diccionario.GetSignificados("palabrainexistente");
        Assert.Null(significadosInexistentes);
    }

    // ===== TESTS PARA NUEVOS MÉTODOS API v1.1.0 =====

    [Fact]
    public void GetDiccionarios_ConIdiomaExistente_DeberiaRetornarMultiplesDiccionarios()
    {
        // Arrange
        string idioma = "ES";

        // Act
        var diccionarios = _suministrador.GetDiccionarios(idioma);

        // Assert
        Assert.NotNull(diccionarios);
        Assert.Equal(2, diccionarios.Count); // Esperamos ES_RAE y ES_LAROUSSE
        
        var codigos = diccionarios.Select(d => d.Codigo).ToList();
        Assert.Contains("ES_RAE", codigos);
        Assert.Contains("ES_LAROUSSE", codigos);
    }

    [Fact]
    public void GetDiccionarios_ConIdiomaInexistente_DeberiaRetornarNull()
    {
        // Arrange
        string idioma = "FR"; // No existe en nuestros datos de prueba

        // Act
        var diccionarios = _suministrador.GetDiccionarios(idioma);

        // Assert
        Assert.Null(diccionarios);
    }

    [Fact]
    public void GetDiccionarioPorCodigo_ConCodigoExistente_DeberiaRetornarDiccionarioEspecifico()
    {
        // Arrange
        string codigo = "ES_RAE";

        // Act
        var diccionario = _suministrador.GetDiccionarioPorCodigo(codigo);

        // Assert
        Assert.NotNull(diccionario);
        Assert.Equal("ES_RAE", diccionario.Codigo);
        Assert.Equal("ES", diccionario.Idioma);
        
        // Verificar funcionalidad básica del diccionario
        Assert.True(diccionario.Existe("casa"));
        Assert.True(diccionario.Existe("hidalgo")); // Palabra específica de RAE
        Assert.False(diccionario.Existe("champán")); // No debe estar en RAE
    }

    [Fact]
    public void GetDiccionarioPorCodigo_ConCodigoInexistente_DeberiaRetornarNull()
    {
        // Arrange
        string codigo = "FR_INEXISTENTE";

        // Act
        var diccionario = _suministrador.GetDiccionarioPorCodigo(codigo);

        // Assert
        Assert.Null(diccionario);
    }

    [Fact]
    public void GetIdiomas_DeberiaRetornarTodosLosIdiomas()
    {
        // Act
        var idiomas = _suministrador.GetIdiomas();

        // Assert
        Assert.NotNull(idiomas);
        Assert.Equal(2, idiomas.Count); // Esperamos ES y EN en nuestros datos de prueba
        
        var codigos = idiomas.Select(i => i.Codigo).ToList();
        Assert.Contains("ES", codigos);
        Assert.Contains("EN", codigos);
        
        var espa = idiomas.First(i => i.Codigo == "ES");
        Assert.Equal("Español", espa.Nombre);
        
        var eng = idiomas.First(i => i.Codigo == "EN");
        Assert.Equal("English", eng.Nombre);
    }

    [Fact]
    public void Diccionario_Codigo_DeberiaRetornarCodigoEspecifico()
    {
        // Arrange & Act
        var diccionarios = _suministrador.GetDiccionarios("ES");
        
        // Assert
        Assert.NotNull(diccionarios);
        
        foreach (var diccionario in diccionarios)
        {
            // Verificar que el código NO es el por defecto "DIC_ES"
            Assert.NotEqual("DIC_ES", diccionario.Codigo);
            
            // Verificar que es uno de los códigos específicos
            Assert.True(diccionario.Codigo == "ES_RAE" || diccionario.Codigo == "ES_LAROUSSE",
                $"Código inesperado: {diccionario.Codigo}");
        }
    }

    [Fact]
    public void PalabrasComunes_DeberianExistirEnMultiplesDiccionarios()
    {
        // Arrange
        var diccionarios = _suministrador.GetDiccionarios("ES");
        Assert.NotNull(diccionarios);
        Assert.Equal(2, diccionarios.Count);

        // Act & Assert - Verificar que "casa" existe en ambos diccionarios
        foreach (var diccionario in diccionarios)
        {
            bool existeCasa = diccionario.Existe("casa");
            Assert.True(existeCasa, $"La palabra 'casa' debería existir en {diccionario.Codigo}");
            
            var significados = diccionario.GetSignificados("casa");
            Assert.NotNull(significados);
            Assert.True(significados.Count > 0, $"'casa' debería tener significados en {diccionario.Codigo}");
        }
    }

    [Fact]
    public void PalabrasEspecificas_DeberianExistirSoloEnSuDiccionario()
    {
        // Arrange
        var diccionarios = _suministrador.GetDiccionarios("ES");
        Assert.NotNull(diccionarios);
        
        var dicRae = diccionarios.First(d => d.Codigo == "ES_RAE");
        var dicLarousse = diccionarios.First(d => d.Codigo == "ES_LAROUSSE");

        // Act & Assert - "hidalgo" solo debe existir en RAE
        Assert.True(dicRae.Existe("hidalgo"), "hidalgo debería existir en ES_RAE");
        Assert.False(dicLarousse.Existe("hidalgo"), "hidalgo NO debería existir en ES_LAROUSSE");

        // Act & Assert - "champán" solo debe existir en Larousse
        Assert.False(dicRae.Existe("champán"), "champán NO debería existir en ES_RAE");
        Assert.True(dicLarousse.Existe("champán"), "champán debería existir en ES_LAROUSSE");
    }

}