using App.MVC.Controllers;
using App.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServicioDiccionarios;
using ServicioDiccionarios.DTOs;
using Xunit;

namespace App.MVC.Tests;

public class DiccionariosControllerTests
{
    // =========================================================================
    // Index: lista diccionarios de un idioma
    // =========================================================================

    [Fact]
    public void Index_IdiomaExiste_DevuelveVistaConDiccionarios()
    {
        // GIVEN: Un servicio que devuelve 1 diccionario para ES
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionarios("ES")).Returns(new List<DiccionarioDTO>
        {
            new() { Codigo = "ES_RAE", Nombre = "Diccionario RAE", Idioma = "ES" }
        });
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se piden los diccionarios de ES
        var resultado = controller.Index("ES");

        // THEN: Devuelve vista con 1 DiccionarioViewModel
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<List<DiccionarioViewModel>>(viewResult.Model);
        Assert.Single(modelo);
        Assert.Equal("ES_RAE", modelo[0].Codigo);
        Assert.Equal("Diccionario RAE", modelo[0].Nombre);
        Assert.Equal("ES", (string)viewResult.ViewData["CodigoIdioma"]!);
    }

    [Fact]
    public void Index_IdiomaNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que no conoce el idioma ELF
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionarios("ELF")).Returns((List<DiccionarioDTO>?)null);
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se piden los diccionarios de ELF
        var resultado = controller.Index("ELF");

        // THEN: Devuelve 404
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    // =========================================================================
    // Buscar: formulario + resultados
    // =========================================================================

    [Fact]
    public void Buscar_DiccionarioNoExiste_Devuelve404()
    {
        // GIVEN: Un servicio que no conoce el diccionario
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionario("FALSO")).Returns((DiccionarioDTO?)null);
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se busca en un diccionario inexistente
        var resultado = controller.Buscar("FALSO", null);

        // THEN: 404
        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public void Buscar_SinPalabra_DevuelveFormularioVacio()
    {
        // GIVEN: Un diccionario que sí existe
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionario("ES_RAE"))
            .Returns(new DiccionarioDTO { Codigo = "ES_RAE", Nombre = "RAE", Idioma = "ES" });
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se accede sin palabra (solo formulario)
        var resultado = controller.Buscar("ES_RAE", null);

        // THEN: Vista con el formulario vacío
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<BusquedaViewModel>(viewResult.Model);
        Assert.Equal("ES_RAE", modelo.CodigoDiccionario);
        Assert.Equal("RAE", modelo.NombreDiccionario);
        Assert.Empty(modelo.Significados);
        Assert.False(modelo.SinResultados);
    }

    [Fact]
    public void Buscar_PalabraExiste_DevuelveSignificados()
    {
        // GIVEN: Un servicio que devuelve significados para "casa"
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionario("ES_RAE"))
            .Returns(new DiccionarioDTO { Codigo = "ES_RAE", Nombre = "RAE", Idioma = "ES" });
        mockServicio.Setup(s => s.GetSignificadosEnDiccionario("ES_RAE", "casa"))
            .Returns(new List<SignificadoDTO>
            {
                new() { Texto = "Edificio para habitar", Diccionario = "RAE" },
                new() { Texto = "Hogar familiar", Diccionario = "RAE" }
            });
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se busca "casa" en ES_RAE
        var resultado = controller.Buscar("ES_RAE", "casa");

        // THEN: Vista con 2 significados
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<BusquedaViewModel>(viewResult.Model);
        Assert.Equal("casa", modelo.Palabra);
        Assert.Equal(2, modelo.Significados.Count);
        Assert.Equal("Edificio para habitar", modelo.Significados[0]);
        Assert.Equal("Hogar familiar", modelo.Significados[1]);
        Assert.False(modelo.SinResultados);
    }

    [Fact]
    public void Buscar_PalabraNoExiste_DevuelveSinResultados()
    {
        // GIVEN: Un servicio que no encuentra la palabra "zzz"
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetDiccionario("ES_RAE"))
            .Returns(new DiccionarioDTO { Codigo = "ES_RAE", Nombre = "RAE", Idioma = "ES" });
        mockServicio.Setup(s => s.GetSignificadosEnDiccionario("ES_RAE", "zzz"))
            .Returns((List<SignificadoDTO>?)null);
        var controller = new DiccionariosController(mockServicio.Object);

        // WHEN: Se busca "zzz"
        var resultado = controller.Buscar("ES_RAE", "zzz");

        // THEN: Vista con flag SinResultados
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<BusquedaViewModel>(viewResult.Model);
        Assert.Equal("zzz", modelo.Palabra);
        Assert.True(modelo.SinResultados);
        Assert.Empty(modelo.Significados);
    }
}
