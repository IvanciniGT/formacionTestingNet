using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace App.MVC.SystemTests;

public class DiccionariosSystemTests : IClassFixture<MvcWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DiccionariosSystemTests(MvcWebApplicationFactory factory)
    {
        factory.SeedDatabase();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // =========================================================================
    // Diccionarios / Index — Lista de diccionarios por idioma
    // =========================================================================

    [Fact]
    public async Task Index_IdiomaExiste_Devuelve200ConDiccionarios()
    {
        var response = await _client.GetAsync("/Diccionarios?codigoIdioma=ES");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Diccionario RAE", html);
        Assert.Contains("ES_RAE", html);
    }

    [Fact]
    public async Task Index_IdiomaExisteSinDiccionarios_Devuelve404()
    {
        // EN existe en el seed pero no tiene diccionarios →
        // el servicio devuelve null → controller retorna 404
        var response = await _client.GetAsync("/Diccionarios?codigoIdioma=EN");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Index_IdiomaNoExiste_Devuelve404()
    {
        var response = await _client.GetAsync("/Diccionarios?codigoIdioma=ELF");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Index_ContieneEnlaceVolverAIdiomas()
    {
        var response = await _client.GetAsync("/Diccionarios?codigoIdioma=ES");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Volver a idiomas", html);
    }

    [Fact]
    public async Task Index_ContieneEnlaceBuscarPalabras()
    {
        var response = await _client.GetAsync("/Diccionarios?codigoIdioma=ES");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Buscar palabras", html);
        Assert.Contains("codigoDiccionario=ES_RAE", html);
    }

    // =========================================================================
    // Diccionarios / Buscar — Formulario de búsqueda + resultados
    // =========================================================================

    [Fact]
    public async Task Buscar_SinPalabra_DevuelveFormularioVacio()
    {
        var response = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=ES_RAE");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Diccionario RAE", html);
        Assert.Contains("placeholder=\"Escribe una palabra...\"", html);
    }

    [Fact]
    public async Task Buscar_PalabraExiste_DevuelveSignificados()
    {
        var response = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=ES_RAE&palabra=casa");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Edificio para habitar", html);
        Assert.Contains("Hogar familiar", html);
        Assert.Contains("casa", html);
    }

    [Fact]
    public async Task Buscar_PalabraNoExiste_DevuelveSinResultados()
    {
        var response = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=ES_RAE&palabra=zzzzz");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("No se encontraron significados", html);
    }

    [Fact]
    public async Task Buscar_DiccionarioNoExiste_Devuelve404()
    {
        var response = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=FALSO");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // =========================================================================
    // Navegación completa — flujo usuario
    // =========================================================================

    [Fact]
    public async Task FlujoCompleto_NavegaDesdeIdiomasHastaSignificados()
    {
        // 1. Página principal → lista idiomas
        var r1 = await _client.GetAsync("/");
        var html1 = await r1.Content.ReadAsStringAsync();
        Assert.Contains("Espa&#xF1;ol", html1);

        // 2. Diccionarios de ES → lista diccionarios
        var r2 = await _client.GetAsync("/Diccionarios?codigoIdioma=ES");
        var html2 = await r2.Content.ReadAsStringAsync();
        Assert.Contains("Diccionario RAE", html2);

        // 3. Buscar en ES_RAE → formulario
        var r3 = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=ES_RAE");
        Assert.Equal(HttpStatusCode.OK, r3.StatusCode);

        // 4. Buscar "casa" → significados
        var r4 = await _client.GetAsync("/Diccionarios/Buscar?codigoDiccionario=ES_RAE&palabra=casa");
        var html4 = await r4.Content.ReadAsStringAsync();
        Assert.Contains("Edificio para habitar", html4);
        Assert.Contains("Hogar familiar", html4);
    }
}
