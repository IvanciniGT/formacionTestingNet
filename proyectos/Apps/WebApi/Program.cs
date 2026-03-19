// =============================================================================
// API REST de Diccionarios - Configuración de la aplicación
// =============================================================================
// Este archivo configura:
// - Controllers desde assembly externo (Rest.V1.Impl)
// - NSwag/OpenAPI para documentación Swagger
// - Entity Framework + SQLite
// - Inyección de dependencias con patrón Decorator (AOP via Scrutor)
// - AutoMapper para mapeo de DTOs
// =============================================================================

using Diccionarios.Api;
using Diccionarios.BBDD;
using ServicioDiccionarios;
using ServicioDiccionarios.Implementacion;
using DiccionariosRestV1.Mappers;
using DiccionariosRestV1.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// CONTROLLERS: Cargar desde assembly externo Rest.V1.Impl
// -----------------------------------------------------------------------------
builder.Services.AddControllers()
    .AddApplicationPart(typeof(IdiomasController).Assembly);

// -----------------------------------------------------------------------------
// SWAGGER/OPENAPI: Documentación interactiva de la API
// Accesible en /swagger cuando app.Environment.IsDevelopment()
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Diccionarios API";
    config.Version = "v1";
    config.Description = "API REST para consulta de diccionarios multiidioma";
});

// -----------------------------------------------------------------------------
// ENTITY FRAMEWORK: Contexto de base de datos SQLite
// -----------------------------------------------------------------------------
builder.Services.AddDbContext<DiccionariosDbContext>(options =>
    options.UseSqlite("Data Source=diccionarios.db"));

// -----------------------------------------------------------------------------
// INYECCIÓN DE DEPENDENCIAS
// -----------------------------------------------------------------------------
// Proveedor de diccionarios (implementación BBDD)
builder.Services.AddScoped<IProveedorDiccionarios, ProveedorDiccionariosBBDD>();

// Servicio de negocio con patrón Decorator para AOP (logging)
// Scrutor permite registrar decoradores de forma declarativa:
// 1. Primero se registra la implementación base (ServicioImpl)
// 2. Luego se "decora" con ServicioLoggingDecorator que añade logging
//    sin modificar el código de ServicioImpl (Principio Open/Closed)
builder.Services.AddScoped<IServicio, ServicioImpl>();
builder.Services.Decorate<IServicio, ServicioLoggingDecorator>();

// -----------------------------------------------------------------------------
// AUTOMAPPER: Perfiles de mapeo
// -----------------------------------------------------------------------------
builder.Services.AddAutoMapper(
    typeof(ServicioDiccionarios.Implementacion.Mappers.DiccionariosMapperProfile),
    typeof(RestV1MapperProfile));

var app = builder.Build();

// -----------------------------------------------------------------------------
// MIDDLEWARE PIPELINE
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();      // Genera /swagger/v1/swagger.json
    app.UseSwaggerUi();    // UI interactiva en /swagger
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Necesario para que WebApplicationFactory<Program> pueda acceder a la clase Program
// desde los proyectos de pruebas de sistema.
public partial class Program { }
