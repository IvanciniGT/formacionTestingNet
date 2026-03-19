using Microsoft.Playwright;

namespace App.MVC.PlaywrightTests;

[Collection("Playwright")]
public class HomePlaywrightTests : IAsyncLifetime
{
    private readonly PlaywrightFixture _fixture;
    private IPage _page = null!;
    private IBrowserContext _context = null!;

    public HomePlaywrightTests(PlaywrightFixture fixture)
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
    // Página principal — Idiomas
    // =========================================================================

    [Fact]
    public async Task PaginaPrincipal_MuestraListaDeIdiomas()
    {
        await _page.GotoAsync(_fixture.BaseUrl);

        // Verifica que se ven los idiomas
        await Assertions.Expect(_page.Locator("h2")).ToHaveCountAsync(2);

        // Verifica textos visibles
        await Assertions.Expect(_page.Locator("text=English")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task PaginaPrincipal_TieneTituloCorrecto()
    {
        await _page.GotoAsync(_fixture.BaseUrl);

        await Assertions.Expect(_page).ToHaveTitleAsync("Idiomas - Diccionarios MVC");
    }

    [Fact]
    public async Task PaginaPrincipal_TieneEnlacesVerDiccionarios()
    {
        await _page.GotoAsync(_fixture.BaseUrl);

        var botones = _page.GetByRole(AriaRole.Link, new() { Name = "Ver diccionarios" });
        await Assertions.Expect(botones).ToHaveCountAsync(2);
    }

    [Fact]
    public async Task PaginaPrincipal_MuestraCabeceraConEnlaceInicio()
    {
        await _page.GotoAsync(_fixture.BaseUrl);

        var header = _page.Locator("header a");
        await Assertions.Expect(header).ToHaveTextAsync("Diccionarios MVC");
    }
}
