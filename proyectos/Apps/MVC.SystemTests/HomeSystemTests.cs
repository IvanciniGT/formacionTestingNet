using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace App.MVC.SystemTests;

public class HomeSystemTests : IClassFixture<MvcWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HomeSystemTests(MvcWebApplicationFactory factory)
    {
        factory.SeedDatabase();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // =========================================================================
    // Home / Index — Lista de idiomas
    // =========================================================================

    [Fact]
    public async Task Index_Devuelve200()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Index_ContieneIdiomasDelSeed()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        // Razor codifica ñ como &#xF1;
        Assert.Contains("Espa&#xF1;ol", html);
        Assert.Contains("English", html);
    }

    [Fact]
    public async Task Index_ContieneTitulo()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Idiomas disponibles", html);
    }

    [Fact]
    public async Task Index_ContieneEnlacesADiccionarios()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("codigoIdioma=ES", html);
        Assert.Contains("codigoIdioma=EN", html);
    }

    [Fact]
    public async Task Index_ContieneLayoutComun()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Diccionarios MVC", html);
        Assert.Contains("<!DOCTYPE html>", html);
    }
}
