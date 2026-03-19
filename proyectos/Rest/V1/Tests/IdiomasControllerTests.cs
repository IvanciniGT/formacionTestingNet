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

public class IdiomasControllerTests
{
    private readonly Mock<IServicio> _mockServicio;
    private readonly IMapper _mapper;
    private readonly IdiomasController _controller;

    public IdiomasControllerTests()
    {
        _mockServicio = new Mock<IServicio>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<RestV1MapperProfile>());
        _mapper = config.CreateMapper();

        var logger = LoggerFactory.Create(b => b.AddConsole())
            .CreateLogger<IdiomasController>();

        _controller = new IdiomasController(_mockServicio.Object, _mapper, logger);
    }

    // =========================================================================
    // GetIdiomas
    // =========================================================================

    [Fact]
    public void GetIdiomas_HayIdiomas_Devuelve200ConLista()
    {
        // GIVEN: Un servicio con 2 idiomas
        _mockServicio.Setup(s => s.GetIdiomas()).Returns(new List<IdiomaDTO>
        {
            new() { Codigo = "ES", Nombre = "Español" },
            new() { Codigo = "EN", Nombre = "English" }
        });

        // WHEN: Pido la lista de idiomas
        var resultado = _controller.GetIdiomas();

        // THEN: Devuelve 200 con 2 idiomas
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var idiomas = Assert.IsAssignableFrom<IList<IdiomaRestV1DTO>>(okResult.Value);
        Assert.Equal(2, idiomas.Count);
        Assert.Equal("ES", idiomas[0].Codigo);
        Assert.Equal("Español", idiomas[0].Nombre);
    }

    [Fact]
    public void GetIdiomas_NoHayIdiomas_Devuelve200ConListaVacia()
    {
        // GIVEN: Un servicio sin idiomas
        _mockServicio.Setup(s => s.GetIdiomas()).Returns(new List<IdiomaDTO>());

        // WHEN: Pido la lista de idiomas
        var resultado = _controller.GetIdiomas();

        // THEN: Devuelve 200 con lista vacía
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var idiomas = Assert.IsAssignableFrom<IList<IdiomaRestV1DTO>>(okResult.Value);
        Assert.Empty(idiomas);
    }

    // =========================================================================
    // GetDiccionariosPorIdioma
    // =========================================================================

    [Fact]
    public void GetDiccionariosPorIdioma_IdiomaExiste_Devuelve200ConDiccionarios()
    {
        // GIVEN: Un servicio con diccionarios para ES
        _mockServicio.Setup(s => s.GetDiccionarios("ES")).Returns(new List<DiccionarioDTO>
        {
            new() { Codigo = "ES_RAE", Nombre = "Diccionario RAE", Idioma = "ES" },
            new() { Codigo = "ES_LAROUSSE", Nombre = "Larousse", Idioma = "ES" }
        });

        // WHEN: Pido los diccionarios del idioma ES
        var resultado = _controller.GetDiccionariosPorIdioma("ES");

        // THEN: Devuelve 200 con 2 diccionarios
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var diccionarios = Assert.IsAssignableFrom<IList<DiccionarioRestV1DTO>>(okResult.Value);
        Assert.Equal(2, diccionarios.Count);
        Assert.Equal("ES_RAE", diccionarios[0].Codigo);
    }

    [Fact]
    public void GetDiccionariosPorIdioma_IdiomaNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que no tiene el idioma ELF
        _mockServicio.Setup(s => s.GetDiccionarios("ELF")).Returns((List<DiccionarioDTO>?)null);

        // WHEN: Pido los diccionarios de ELF
        var resultado = _controller.GetDiccionariosPorIdioma("ELF");

        // THEN: Devuelve 404
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    // =========================================================================
    // GetSignificadosPorIdioma
    // =========================================================================

    [Fact]
    public void GetSignificadosPorIdioma_PalabraExiste_Devuelve200ConSignificados()
    {
        // GIVEN: Un servicio con significados para "casa" en idioma ES
        _mockServicio.Setup(s => s.GetSignificadosEnIdioma("ES", "casa"))
            .Returns(new List<SignificadoDTO>
            {
                new() { Texto = "Edificio para habitar", Diccionario = "ES_RAE" },
                new() { Texto = "Vivienda familiar", Diccionario = "ES_LAROUSSE" }
            });

        // WHEN: Pido los significados de "casa" en idioma ES
        var resultado = _controller.GetSignificadosPorIdioma("ES", "casa");

        // THEN: Devuelve 200 con significados de ambos diccionarios
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var significados = Assert.IsAssignableFrom<IList<SignificadoRestV1DTO>>(okResult.Value);
        Assert.Equal(2, significados.Count);
    }

    [Fact]
    public void GetSignificadosPorIdioma_PalabraNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que no encuentra la palabra
        _mockServicio.Setup(s => s.GetSignificadosEnIdioma("ES", "zzz"))
            .Returns((List<SignificadoDTO>?)null);

        // WHEN: Busco "zzz"
        var resultado = _controller.GetSignificadosPorIdioma("ES", "zzz");

        // THEN: Devuelve 404
        Assert.IsType<NotFoundObjectResult>(resultado.Result);
    }

    // =========================================================================
    // ExistePalabraEnIdioma
    // =========================================================================

    [Fact]
    public void ExistePalabraEnIdioma_PalabraExiste_Devuelve200()
    {
        // GIVEN: La palabra "casa" existe en el idioma ES
        _mockServicio.Setup(s => s.ExistePalabraEnIdioma("ES", "casa")).Returns(true);

        // WHEN: Verifico si existe
        var resultado = _controller.ExistePalabraEnIdioma("ES", "casa");

        // THEN: Devuelve 200
        Assert.IsType<OkResult>(resultado);
    }

    [Fact]
    public void ExistePalabraEnIdioma_PalabraNoExiste_Devuelve404()
    {
        // GIVEN: La palabra "zzz" no existe en el idioma ES
        _mockServicio.Setup(s => s.ExistePalabraEnIdioma("ES", "zzz")).Returns(false);

        // WHEN: Verifico si existe
        var resultado = _controller.ExistePalabraEnIdioma("ES", "zzz");

        // THEN: Devuelve 404
        Assert.IsType<NotFoundResult>(resultado);
    }
}
