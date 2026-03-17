using LibreriaMatematica;
using AppConsola;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// El HOST es el contenedor de inversión de control
// ÉL controla el flujo de ejecución del programa, no yo.
// 
// ¿Qué me aporta el Host?
// ========================
// 1. INYECCIÓN DE DEPENDENCIAS (DI) - Contenedor de servicios
//    - Registrar servicios: AddSingleton, AddScoped, AddTransient
//    - Resolución automática de dependencias en constructores
//
// 2. CONFIGURACIÓN (IConfiguration)
//    - Lee automáticamente appsettings.json, appsettings.{Environment}.json
//    - Variables de entorno
//    - Argumentos de línea de comandos
//    - User secrets (para desarrollo)
//
// 3. LOGGING (ILogger<T>)
//    - Sistema de logging integrado
//    - Múltiples proveedores: consola, ficheros, Application Insights...
//    - Niveles: Trace, Debug, Information, Warning, Error, Critical
//
// 4. CICLO DE VIDA DE LA APLICACIÓN (IHostApplicationLifetime)
//    - Eventos: ApplicationStarted, ApplicationStopping, ApplicationStopped
//    - Graceful shutdown (cierre ordenado)
//
// 5. HOSTED SERVICES (IHostedService / BackgroundService)
//    - Tareas en segundo plano
//    - Servicios que se ejecutan durante toda la vida de la app
//
// 6. ENTORNOS (IHostEnvironment)
//    - Development, Staging, Production
//    - Configuración diferente según entorno

var builder = Host.CreateApplicationBuilder(args);

// Registro mis servicios en el contenedor
builder.Services.AddSingleton<ILibreriaMatematica, LibreriaMatematica.LibreriaMatematica>();
builder.Services.AddHostedService<LogicaDelPrograma>();  // El Host ejecutará esto automáticamente

// Construyo y ejecuto el Host - YO YA NO CONTROLO NADA 🎉
var host = builder.Build();
await host.RunAsync();