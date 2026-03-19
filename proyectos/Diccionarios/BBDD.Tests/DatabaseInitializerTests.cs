using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Diccionarios.BBDD.Tests;

public class DatabaseInitializerTests : IDisposable
{
    private readonly DiccionariosDbContext _context;
    private readonly DatabaseInitializer _initializer;

    public DatabaseInitializerTests()
    {
        var options = new DbContextOptionsBuilder<DiccionariosDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DiccionariosDbContext(options);
        var logger = NullLogger<DatabaseInitializer>.Instance;
        _initializer = new DatabaseInitializer(_context, logger);
    }

    [Fact]
    public async Task InicializarSiEsNecesario_BBDDVacia_CreaDatos()
    {
        // GIVEN: Una base de datos vacía

        // WHEN: Inicializo
        await _initializer.InicializarSiEsNecesarioAsync();

        // THEN: Se crean idiomas, diccionarios, palabras y significados
        Assert.True(await _context.Idiomas.AnyAsync());
        Assert.True(await _context.Diccionarios.AnyAsync());
        Assert.True(await _context.Palabras.AnyAsync());
        Assert.True(await _context.Significados.AnyAsync());
    }

    [Fact]
    public async Task InicializarSiEsNecesario_BBDDVacia_Crea3Idiomas()
    {
        // WHEN
        await _initializer.InicializarSiEsNecesarioAsync();

        // THEN: 3 idiomas: ES, EN, FR
        var idiomas = await _context.Idiomas.ToListAsync();
        Assert.Equal(3, idiomas.Count);
        Assert.Contains(idiomas, i => i.Codigo == "ES");
        Assert.Contains(idiomas, i => i.Codigo == "EN");
        Assert.Contains(idiomas, i => i.Codigo == "FR");
    }

    [Fact]
    public async Task InicializarSiEsNecesario_BBDDVacia_Crea6Diccionarios()
    {
        // WHEN
        await _initializer.InicializarSiEsNecesarioAsync();

        // THEN: 6 diccionarios (2 por idioma)
        var diccionarios = await _context.Diccionarios.ToListAsync();
        Assert.Equal(6, diccionarios.Count);
        Assert.Contains(diccionarios, d => d.Codigo == "ES_RAE");
        Assert.Contains(diccionarios, d => d.Codigo == "ES_LAROUSSE");
        Assert.Contains(diccionarios, d => d.Codigo == "EN_OXFORD");
        Assert.Contains(diccionarios, d => d.Codigo == "EN_MERRIAM");
        Assert.Contains(diccionarios, d => d.Codigo == "FR_LAROUSSE");
        Assert.Contains(diccionarios, d => d.Codigo == "FR_ROBERT");
    }

    [Fact]
    public async Task InicializarSiEsNecesario_BBDDConDatos_NoCreaDuplicados()
    {
        // GIVEN: Ya hay datos en la BBDD
        _context.Idiomas.Add(new IdiomaEntity { Codigo = "ES", Nombre = "Español" });
        await _context.SaveChangesAsync();

        // WHEN: Inicializo de nuevo
        await _initializer.InicializarSiEsNecesarioAsync();

        // THEN: Solo hay 1 idioma (no se duplicaron)
        var count = await _context.Idiomas.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task InicializarSiEsNecesario_LlamarDosVeces_EsIdempotente()
    {
        // WHEN: Inicializo dos veces
        await _initializer.InicializarSiEsNecesarioAsync();
        var countDespuesDePrimera = await _context.Idiomas.CountAsync();

        await _initializer.InicializarSiEsNecesarioAsync();
        var countDespuesDeSegunda = await _context.Idiomas.CountAsync();

        // THEN: Mismos datos
        Assert.Equal(countDespuesDePrimera, countDespuesDeSegunda);
    }

    [Fact]
    public void Constructor_ConContextNull_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DatabaseInitializer(null!, NullLogger<DatabaseInitializer>.Instance));
    }

    [Fact]
    public void Constructor_ConLoggerNull_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DatabaseInitializer(_context, null!));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
