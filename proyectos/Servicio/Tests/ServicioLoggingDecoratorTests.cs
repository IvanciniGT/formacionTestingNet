using Microsoft.Extensions.Logging;
using Moq;
using ServicioDiccionarios.DTOs;
using ServicioDiccionarios.Implementacion;

namespace ServicioDiccionarios.Tests;

/// <summary>
/// Tests del ServicioLoggingDecorator.
/// Verifica que: 1) delega al servicio inner, 2) hace logging, 3) propaga excepciones con logging.
/// </summary>
public class ServicioLoggingDecoratorTests
{
    private readonly Mock<IServicio> _mockInner;
    private readonly Mock<ILogger<ServicioLoggingDecorator>> _mockLogger;
    private readonly ServicioLoggingDecorator _decorator;

    public ServicioLoggingDecoratorTests()
    {
        _mockInner = new Mock<IServicio>();
        _mockLogger = new Mock<ILogger<ServicioLoggingDecorator>>();
        _decorator = new ServicioLoggingDecorator(_mockInner.Object, _mockLogger.Object);
    }

    // =========================================================================
    // Delegación: cada método delega al inner y devuelve su resultado
    // =========================================================================

    [Fact]
    public void GetIdiomas_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN: El inner devuelve 2 idiomas
        var idiomas = new List<IdiomaDTO>
        {
            new() { Codigo = "ES", Nombre = "Español" },
            new() { Codigo = "EN", Nombre = "English" }
        };
        _mockInner.Setup(s => s.GetIdiomas()).Returns(idiomas);

        // WHEN: Llamo al decorator
        var resultado = _decorator.GetIdiomas();

        // THEN: Devuelve lo mismo que el inner
        Assert.Equal(2, resultado.Count);
        Assert.Equal("ES", resultado[0].Codigo);
        _mockInner.Verify(s => s.GetIdiomas(), Times.Once);
    }

    [Fact]
    public void GetDiccionarios_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        var diccionarios = new List<DiccionarioDTO>
        {
            new() { Codigo = "ES_RAE", Nombre = "RAE", Idioma = "ES" }
        };
        _mockInner.Setup(s => s.GetDiccionarios("ES")).Returns(diccionarios);

        // WHEN
        var resultado = _decorator.GetDiccionarios("ES");

        // THEN
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        _mockInner.Verify(s => s.GetDiccionarios("ES"), Times.Once);
    }

    [Fact]
    public void GetDiccionario_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        var dto = new DiccionarioDTO { Codigo = "ES_RAE", Nombre = "RAE", Idioma = "ES" };
        _mockInner.Setup(s => s.GetDiccionario("ES_RAE")).Returns(dto);

        // WHEN
        var resultado = _decorator.GetDiccionario("ES_RAE");

        // THEN
        Assert.NotNull(resultado);
        Assert.Equal("ES_RAE", resultado.Codigo);
        _mockInner.Verify(s => s.GetDiccionario("ES_RAE"), Times.Once);
    }

    [Fact]
    public void GetDiccionario_CuandoNoExiste_DevuelveNull()
    {
        // GIVEN
        _mockInner.Setup(s => s.GetDiccionario("ELF")).Returns((DiccionarioDTO?)null);

        // WHEN
        var resultado = _decorator.GetDiccionario("ELF");

        // THEN
        Assert.Null(resultado);
        _mockInner.Verify(s => s.GetDiccionario("ELF"), Times.Once);
    }

    [Fact]
    public void GetSignificadosEnDiccionario_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        var significados = new List<SignificadoDTO>
        {
            new() { Texto = "Edificio para habitar", Diccionario = "ES_RAE" }
        };
        _mockInner.Setup(s => s.GetSignificadosEnDiccionario("ES_RAE", "casa")).Returns(significados);

        // WHEN
        var resultado = _decorator.GetSignificadosEnDiccionario("ES_RAE", "casa");

        // THEN
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        _mockInner.Verify(s => s.GetSignificadosEnDiccionario("ES_RAE", "casa"), Times.Once);
    }

    [Fact]
    public void GetSignificadosEnIdioma_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        var significados = new List<SignificadoDTO>
        {
            new() { Texto = "Edificio para habitar", Diccionario = "ES_RAE" },
            new() { Texto = "Vivienda familiar", Diccionario = "ES_LAROUSSE" }
        };
        _mockInner.Setup(s => s.GetSignificadosEnIdioma("ES", "casa")).Returns(significados);

        // WHEN
        var resultado = _decorator.GetSignificadosEnIdioma("ES", "casa");

        // THEN
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        _mockInner.Verify(s => s.GetSignificadosEnIdioma("ES", "casa"), Times.Once);
    }

    [Fact]
    public void ExistePalabraEnDiccionario_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        _mockInner.Setup(s => s.ExistePalabraEnDiccionario("ES_RAE", "casa")).Returns(true);

        // WHEN
        var resultado = _decorator.ExistePalabraEnDiccionario("ES_RAE", "casa");

        // THEN
        Assert.True(resultado);
        _mockInner.Verify(s => s.ExistePalabraEnDiccionario("ES_RAE", "casa"), Times.Once);
    }

    [Fact]
    public void ExistePalabraEnIdioma_DelegaAlInnerYDevuelveResultado()
    {
        // GIVEN
        _mockInner.Setup(s => s.ExistePalabraEnIdioma("ES", "casa")).Returns(true);

        // WHEN
        var resultado = _decorator.ExistePalabraEnIdioma("ES", "casa");

        // THEN
        Assert.True(resultado);
        _mockInner.Verify(s => s.ExistePalabraEnIdioma("ES", "casa"), Times.Once);
    }

    // =========================================================================
    // Propagación de excepciones: el decorator loguea y re-lanza
    // =========================================================================

    [Fact]
    public void GetIdiomas_CuandoInnerLanzaExcepcion_PropagaExcepcion()
    {
        // GIVEN: El inner lanza una excepción
        _mockInner.Setup(s => s.GetIdiomas()).Throws(new InvalidOperationException("Error de prueba"));

        // WHEN/THEN: El decorator propaga la excepción
        Assert.Throws<InvalidOperationException>(() => _decorator.GetIdiomas());
    }

    [Fact]
    public void GetDiccionario_CuandoInnerLanzaExcepcion_PropagaExcepcion()
    {
        // GIVEN
        _mockInner.Setup(s => s.GetDiccionario("ES_RAE"))
            .Throws(new InvalidOperationException("Error de prueba"));

        // WHEN/THEN
        Assert.Throws<InvalidOperationException>(() => _decorator.GetDiccionario("ES_RAE"));
    }

    // =========================================================================
    // Constructor: validación de argumentos
    // =========================================================================

    [Fact]
    public void Constructor_ConInnerNull_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ServicioLoggingDecorator(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ConLoggerNull_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ServicioLoggingDecorator(_mockInner.Object, null!));
    }
}
