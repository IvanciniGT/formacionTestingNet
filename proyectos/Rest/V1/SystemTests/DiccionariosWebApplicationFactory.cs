using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MariaDb;

namespace Rest.V1.SystemTests;

/// <summary>
/// Factory customizada que levanta un contenedor MariaDB automáticamente con Testcontainers.
/// Al hacer dotnet test: levanta MariaDB → ejecuta tests → tira el contenedor. Todo automático.
/// </summary>
public class DiccionariosWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MariaDbContainer _mariaDbContainer = new MariaDbBuilder()
        .WithImage("mariadb:11")
        .WithDatabase("diccionarios_test")
        .WithUsername("root")
        .WithPassword("test1234")
        .Build();

    // Testcontainers: arranca el contenedor ANTES de los tests
    public async Task InitializeAsync()
    {
        await _mariaDbContainer.StartAsync();
        SeedDatabase();
    }

    // Testcontainers: para y elimina el contenedor DESPUÉS de los tests
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mariaDbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Quitar TODOS los registros del DbContext original (SQLite)
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<DiccionariosDbContext>)
                          || d.ServiceType == typeof(DbContextOptions)
                          || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();
            foreach (var d in descriptors)
                services.Remove(d);

            // Usar la connection string del contenedor levantado por Testcontainers
            var connectionString = _mariaDbContainer.GetConnectionString();

            services.AddDbContext<DiccionariosDbContext>(options =>
                options.UseMySQL(connectionString));
        });

        builder.UseEnvironment("Development");
    }

    private void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DiccionariosDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        var idiomaES = new IdiomaEntity { Codigo = "ES", Nombre = "Español" };
        db.Idiomas.Add(idiomaES);
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
