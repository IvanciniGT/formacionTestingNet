using App.MVC.Controllers;
using App.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServicioDiccionarios;
using ServicioDiccionarios.DTOs;
using Xunit;

namespace App.MVC.Tests;

public class HomeControllerTests
{
    [Fact]
    public void Index_DevuelveVistaConIdiomas()
    {
        // GIVEN: Un servicio que devuelve 2 idiomas
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetIdiomas()).Returns(new List<IdiomaDTO>
        {
            new() { Codigo = "ES", Nombre = "Español" },
            new() { Codigo = "EN", Nombre = "Inglés" }
        });
        var controller = new HomeController(mockServicio.Object);

        // WHEN: Se pide la página de inicio
        var resultado = controller.Index();

        // THEN: Devuelve una vista con 2 IdiomaViewModel
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<List<IdiomaViewModel>>(viewResult.Model);
        Assert.Equal(2, modelo.Count);
        Assert.Equal("ES", modelo[0].Codigo);
        Assert.Equal("Español", modelo[0].Nombre);
        Assert.Equal("EN", modelo[1].Codigo);
    }

    [Fact]
    public void Index_SinIdiomas_DevuelveVistaConListaVacia()
    {
        // GIVEN: Un servicio que no tiene idiomas
        var mockServicio = new Mock<IServicio>();
        mockServicio.Setup(s => s.GetIdiomas()).Returns(new List<IdiomaDTO>());
        var controller = new HomeController(mockServicio.Object);

        // WHEN: Se pide la página de inicio
        var resultado = controller.Index();

        // THEN: Devuelve una vista con lista vacía
        var viewResult = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<List<IdiomaViewModel>>(viewResult.Model);
        Assert.Empty(modelo);
    }
}
