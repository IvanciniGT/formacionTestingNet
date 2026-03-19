using Diccionarios.Api;
using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using ServicioDiccionarios;
using ServicioDiccionarios.Implementacion;

namespace App.MVC.PlaywrightTests;

/// <summary>
/// Fixture que arranca la app MVC completa en un puerto real con Kestrel
/// para que Playwright pueda conectarse con un navegador Chromium headless.
/// La BD se reemplaza por EF Core InMemory.
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private WebApplication _app = null!;

    public IBrowser Browser { get; private set; } = null!;
    public string BaseUrl { get; } = "http://localhost:5299";

    public async Task InitializeAsync()
    {
        // 1. Construir la app MVC con la misma config que Program.cs
        //    pero usando InMemory DB y Kestrel en un puerto real
        var mvcProjectDir = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "MVC"));

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = mvcProjectDir
        });

        // Servicios (mismos que Program.cs, con InMemory en vez de SQLite)
        builder.Services.AddControllersWithViews()
            .AddApplicationPart(typeof(App.MVC.Controllers.HomeController).Assembly);
        builder.Services.AddDbContext<DiccionariosDbContext>(options =>
            options.UseInMemoryDatabase("PlaywrightTests"));
        builder.Services.AddScoped<IProveedorDiccionarios, ProveedorDiccionariosBBDD>();
        builder.Services.AddScoped<IServicio, ServicioImpl>();
        builder.Services.Decorate<IServicio, ServicioLoggingDecorator>();
        builder.Services.AddAutoMapper(
            typeof(ServicioDiccionarios.Implementacion.Mappers.DiccionariosMapperProfile));

        builder.WebHost.UseUrls(BaseUrl);

        _app = builder.Build();

        // Middleware (mismo que Program.cs, sin HTTPS redirect)
        _app.UseStaticFiles();
        _app.UseRouting();
        _app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Seed de datos de prueba
        using (var scope = _app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DiccionariosDbContext>();
            db.Database.EnsureCreated();
            SeedDatabase(db);
        }

        await _app.StartAsync();

        // 2. Arrancar Playwright con Chromium headless
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser != null) await Browser.DisposeAsync();
        _playwright?.Dispose();
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    private static void SeedDatabase(DiccionariosDbContext db)
    {
        var idiomaES = new IdiomaEntity { Codigo = "ES", Nombre = "Español" };
        var idiomaEN = new IdiomaEntity { Codigo = "EN", Nombre = "English" };
        db.Idiomas.AddRange(idiomaES, idiomaEN);
        db.SaveChanges();

        var diccionarioRAE = new DiccionarioEntity
        {
            Codigo = "ES_RAE",
            Nombre = "Diccionario RAE",
            IdiomaId = idiomaES.Id
        };
        db.Diccionarios.Add(diccionarioRAE);
        db.SaveChanges();

        var palabraCasa = new PalabraEntity
        {
            Texto = "casa",
            DiccionarioId = diccionarioRAE.Id
        };
        db.Palabras.Add(palabraCasa);
        db.SaveChanges();

        db.Significados.AddRange(
            new SignificadoEntity { Texto = "Edificio para habitar", PalabraId = palabraCasa.Id },
            new SignificadoEntity { Texto = "Hogar familiar", PalabraId = palabraCasa.Id }
        );
        db.SaveChanges();
    }
}

/// <summary>
/// Colección de xUnit que comparte el PlaywrightFixture entre todas las clases de test.
/// Así Chromium y la app MVC se arrancan una sola vez para todos los tests.
/// </summary>
[CollectionDefinition("Playwright")]
public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture> { }
