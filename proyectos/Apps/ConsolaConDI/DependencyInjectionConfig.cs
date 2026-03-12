using Microsoft.Extensions.DependencyInjection;
using Diccionarios.Api;
using Diccionarios.Ficheros;

namespace DiccionariosAppConsolaConDI;

/// <summary>
/// Configuración manual de Inyección de Dependencias.
/// </summary>
/// <remarks>
/// Este archivo demuestra cómo configurar DI sin usar un Host builder.
/// Es útil para aplicaciones pequeñas o cuando se necesita control total.
/// 
/// Patrón utilizado: Composition Root
/// - Todas las dependencias se configuran en un único lugar
/// - La aplicación recibe las dependencias ya resueltas
/// </remarks>
public static class DependencyInjectionConfig
{
    /// <summary>
    /// Configura y construye el contenedor de servicios.
    /// </summary>
    public static ServiceProvider ConfigurarServicios()
    {
        var services = new ServiceCollection();
        
        // Registro de IProveedorDiccionarios → ProveedorDiccionariosFicheros
        // Lifetime: Singleton (una única instancia durante toda la aplicación)
        services.AddSingleton<IProveedorDiccionarios>(provider =>
        {
            var rutaDiccionarios = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Diccionarios");
            return new ProveedorDiccionariosFicheros(rutaDiccionarios);
        });
        
        return services.BuildServiceProvider();
    }
}