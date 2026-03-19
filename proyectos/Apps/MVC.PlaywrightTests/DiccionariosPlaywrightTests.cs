using Microsoft.Playwright;

namespace App.MVC.PlaywrightTests;

[Collection("Playwright")]
public class DiccionariosPlaywrightTests : IAsyncLifetime
{
    private readonly PlaywrightFixture _fixture;
    private IPage _page = null!;
    private IBrowserContext _context = null!;

    public DiccionariosPlaywrightTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    // =========================================================================
    // Diccionarios de un idioma
    // =========================================================================

    [Fact]
    public async Task VerDiccionarios_MuestraDiccionariosDelIdioma()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios?codigoIdioma=ES");

        await Assertions.Expect(_page.Locator("text=Diccionario RAE")).ToBeVisibleAsync();
        await Assertions.Expect(_page.Locator("text=ES_RAE")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task VerDiccionarios_TieneBotonBuscarPalabras()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios?codigoIdioma=ES");

        var botonBuscar = _page.GetByRole(AriaRole.Link, new() { Name = "Buscar palabras" });
        await Assertions.Expect(botonBuscar).ToBeVisibleAsync();
    }

    [Fact]
    public async Task VerDiccionarios_TieneEnlaceVolverAIdiomas()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios?codigoIdioma=ES");

        await Assertions.Expect(_page.Locator("text=Volver a idiomas")).ToBeVisibleAsync();
    }

    // =========================================================================
    // Búsqueda de palabras
    // =========================================================================

    [Fact]
    public async Task Buscar_FormularioVacio_MuestraCampoDeBusqueda()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios/Buscar?codigoDiccionario=ES_RAE");

        var input = _page.Locator("input[name='palabra']");
        await Assertions.Expect(input).ToBeVisibleAsync();
        await Assertions.Expect(input).ToHaveAttributeAsync("placeholder", "Escribe una palabra...");

        var boton = _page.GetByRole(AriaRole.Button, new() { Name = "Buscar" });
        await Assertions.Expect(boton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Buscar_PalabraConResultados_MuestraSignificados()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios/Buscar?codigoDiccionario=ES_RAE");

        // Escribir en el campo de búsqueda
        await _page.FillAsync("input[name='palabra']", "casa");

        // Clic en Buscar
        await _page.ClickAsync("button:text('Buscar')");

        // Verificar que aparecen los significados
        await Assertions.Expect(_page.Locator(".significado")).ToHaveCountAsync(2);
        await Assertions.Expect(_page.Locator("text=Edificio para habitar")).ToBeVisibleAsync();
        await Assertions.Expect(_page.Locator("text=Hogar familiar")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Buscar_PalabraSinResultados_MuestraMensajeDeError()
    {
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios/Buscar?codigoDiccionario=ES_RAE");

        await _page.FillAsync("input[name='palabra']", "zzzzz");
        await _page.ClickAsync("button:text('Buscar')");

        await Assertions.Expect(_page.Locator(".alert-warning")).ToBeVisibleAsync();
        await Assertions.Expect(_page.Locator("text=No se encontraron significados")).ToBeVisibleAsync();
    }

    // =========================================================================
    // Flujo completo — Navegación real como haría un usuario
    // =========================================================================

    [Fact]
    public async Task FlujoCompleto_DesdeIdiomasHastaSignificados()
    {
        // 1. Ir a la página principal
        await _page.GotoAsync(_fixture.BaseUrl);
        await Assertions.Expect(_page.Locator("h1")).ToHaveTextAsync("Idiomas disponibles");

        // 2. Clic en "Ver diccionarios" del primer idioma (ES)
        await _page.Locator(".card").First.GetByRole(AriaRole.Link, new() { Name = "Ver diccionarios" }).ClickAsync();

        // 3. Verificar que estamos en la página de diccionarios
        await Assertions.Expect(_page.Locator("text=Diccionario RAE")).ToBeVisibleAsync();

        // 4. Clic en "Buscar palabras"
        await _page.GetByRole(AriaRole.Link, new() { Name = "Buscar palabras" }).ClickAsync();

        // 5. Verificar formulario de búsqueda
        await Assertions.Expect(_page.Locator("input[name='palabra']")).ToBeVisibleAsync();

        // 6. Escribir "casa" y buscar
        await _page.FillAsync("input[name='palabra']", "casa");
        await _page.ClickAsync("button:text('Buscar')");

        // 7. Verificar significados
        await Assertions.Expect(_page.Locator(".significado")).ToHaveCountAsync(2);
        await Assertions.Expect(_page.Locator("text=Edificio para habitar")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Navegacion_VolverAIdiomasDesdeDetalle()
    {
        // Ir a diccionarios
        await _page.GotoAsync($"{_fixture.BaseUrl}/Diccionarios?codigoIdioma=ES");

        // Clic en "Volver a idiomas"
        await _page.Locator("a:text('Volver a idiomas')").ClickAsync();

        // Estamos de vuelta en Home
        await Assertions.Expect(_page.Locator("h1")).ToHaveTextAsync("Idiomas disponibles");
    }
}
