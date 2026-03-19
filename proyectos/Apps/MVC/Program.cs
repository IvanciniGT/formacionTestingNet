using Diccionarios.Api;
using Diccionarios.BBDD;
using ServicioDiccionarios;
using ServicioDiccionarios.Implementacion;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC con Razor Views
builder.Services.AddControllersWithViews();

// Entity Framework + SQLite
builder.Services.AddDbContext<DiccionariosDbContext>(options =>
    options.UseSqlite("Data Source=diccionarios.db"));

// Inyección de dependencias
builder.Services.AddScoped<IProveedorDiccionarios, ProveedorDiccionariosBBDD>();

// Servicio + Decorator AOP (logging) con Scrutor
builder.Services.AddScoped<IServicio, ServicioImpl>();
builder.Services.Decorate<IServicio, ServicioLoggingDecorator>();

// AutoMapper (perfil de Servicio.Impl para mapear entidades Diccionarios → DTOs)
builder.Services.AddAutoMapper(
    typeof(ServicioDiccionarios.Implementacion.Mappers.DiccionariosMapperProfile));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Para que WebApplicationFactory pueda acceder al tipo Program desde los tests
public partial class Program { }
