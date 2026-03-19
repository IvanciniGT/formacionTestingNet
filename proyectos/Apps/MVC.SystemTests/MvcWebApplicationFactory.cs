using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.MVC.SystemTests;

public class MvcWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Eliminar el DbContext de SQLite registrado en Program.cs
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<DiccionariosDbContext>)
                          || d.ServiceType == typeof(DbContextOptions)
                          || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();
            foreach (var d in descriptors)
                services.Remove(d);

            // Usar EF Core InMemory para los tests
            services.AddDbContext<DiccionariosDbContext>(options =>
                options.UseInMemoryDatabase("DiccionariosMvcTests"));
        });

        builder.UseEnvironment("Development");
    }

    public void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DiccionariosDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

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
