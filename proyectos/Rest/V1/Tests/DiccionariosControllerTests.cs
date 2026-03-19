using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServicioDiccionarios;
using ServicioDiccionarios.DTOs;
using DiccionariosRestV1Api;
using DiccionariosRestV1.Controllers;
using DiccionariosRestV1.Mappers;

namespace Rest.V1.Tests;

public class DiccionariosControllerTests
{
    private readonly Mock<IServicio> _mockServicio;
    private readonly IMapper _mapper;
    private readonly DiccionariosController _controller;

    public DiccionariosControllerTests()
    {
        _mockServicio = new Mock<IServicio>();

        // Mapper REAL (con sus pruebas unitarias propias)
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RestV1MapperProfile>());
        _mapper = config.CreateMapper();

        // Logger: Fake a consola
        var logger = LoggerFactory.Create(b => b.AddConsole())
            .CreateLogger<DiccionariosController>();

        _controller = new DiccionariosController(_mockServicio.Object, _mapper, logger);
    }

    // =========================================================================
    // GetDiccionario: Happy Path - Diccionario encontrado → 200
    // =========================================================================
    [Fact]
    public void GetDiccionario_DiccionarioExiste_Devuelve200ConDiccionario()
    {
        // GIVEN: Un servicio que tiene el diccionario ES_RAE
        _mockServicio
            .Setup(s => s.GetDiccionario("ES_RAE"))
            .Returns(new DiccionarioDTO
            {
                Codigo = "ES_RAE",
                Nombre = "Diccionario RAE",
                Idioma = "ES"
            });

        // WHEN: Pido el diccionario ES_RAE al controlador
        var resultado = _controller.GetDiccionario("ES_RAE");

        // THEN: Me devuelve un 200 OK con el diccionario
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var diccionario = Assert.IsType<DiccionarioRestV1DTO>(okResult.Value);
        Assert.Equal("ES_RAE", diccionario.Codigo);
        Assert.Equal("Diccionario RAE", diccionario.Nombre);
        Assert.Equal("ES", diccionario.Idioma);
    }

    // =========================================================================
    // GetDiccionario: Fail - Diccionario no encontrado → 404
    // =========================================================================
    [Fact]
    public void GetDiccionario_DiccionarioNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que NO tiene el diccionario de los elfos
        _mockServicio
            .Setup(s => s.GetDiccionario("ELF"))
            .Returns((DiccionarioDTO?)null);

        // WHEN: Le pido al controlador el diccionario de los elfos
        var resultado = _controller.GetDiccionario("ELF");

        // THEN: Me devuelve un 404 Not Found
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    // =========================================================================
    // GetSignificadosPorDiccionario: Happy Path - Palabra encontrada → 200
    // =========================================================================
    [Fact]
    public void GetSignificados_PalabraExiste_Devuelve200ConSignificados()
    {
        // GIVEN: Un servicio que tiene significados para "casa" en ES_RAE
        _mockServicio
            .Setup(s => s.GetSignificadosEnDiccionario("ES_RAE", "casa"))
            .Returns(new List<SignificadoDTO>
            {
                new() { Texto = "Edificio para habitar", Diccionario = "ES_RAE" },
                new() { Texto = "Hogar familiar", Diccionario = "ES_RAE" }
            });

        // WHEN: Pido los significados de "casa" en ES_RAE
        var resultado = _controller.GetSignificadosPorDiccionario("ES_RAE", "casa");

        // THEN: Me devuelve un 200 OK con los significados
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var significados = Assert.IsAssignableFrom<IList<SignificadoRestV1DTO>>(okResult.Value);
        Assert.Equal(2, significados.Count);
        Assert.Equal("Edificio para habitar", significados[0].Texto);
        Assert.Equal("Hogar familiar", significados[1].Texto);
    }

    // =========================================================================
    // GetSignificadosPorDiccionario: Fail - Palabra no encontrada → 404
    // =========================================================================
    [Fact]
    public void GetSignificados_PalabraNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que no encuentra la palabra "zzz" en ES_RAE
        _mockServicio
            .Setup(s => s.GetSignificadosEnDiccionario("ES_RAE", "zzz"))
            .Returns((List<SignificadoDTO>?)null);

        // WHEN: Pido los significados de "zzz" en ES_RAE
        var resultado = _controller.GetSignificadosPorDiccionario("ES_RAE", "zzz");

        // THEN: Me devuelve un 404
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    // =========================================================================
    // ExistePalabraEnDiccionario: Happy Path → 200
    // =========================================================================
    [Fact]
    public void ExistePalabra_PalabraExiste_Devuelve200()
    {
        // GIVEN: La palabra "casa" existe en ES_RAE
        _mockServicio
            .Setup(s => s.ExistePalabraEnDiccionario("ES_RAE", "casa"))
            .Returns(true);

        // WHEN: Verifico si existe
        var resultado = _controller.ExistePalabraEnDiccionario("ES_RAE", "casa");

        // THEN: Devuelve 200
        Assert.IsType<OkResult>(resultado);
    }

    // =========================================================================
    // ExistePalabraEnDiccionario: Fail → 404
    // =========================================================================
    [Fact]
    public void ExistePalabra_PalabraNoExiste_Devuelve404()
    {
        // GIVEN: La palabra "zzz" no existe en ES_RAE
        _mockServicio
            .Setup(s => s.ExistePalabraEnDiccionario("ES_RAE", "zzz"))
            .Returns(false);

        // WHEN: Verifico si existe
        var resultado = _controller.ExistePalabraEnDiccionario("ES_RAE", "zzz");

        // THEN: Devuelve 404
        Assert.IsType<NotFoundResult>(resultado);
    }
}
