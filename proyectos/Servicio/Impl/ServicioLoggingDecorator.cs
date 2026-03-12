using Microsoft.Extensions.Logging;
using ServicioDiccionarios.DTOs;
using System.Diagnostics;

namespace ServicioDiccionarios.Implementacion;

/// <summary>
/// Decorador que añade logging a cualquier implementación de IServicio.
/// Implementa el patrón Decorator para separar el cross-cutting concern del logging
/// de la lógica de negocio.
/// </summary>
public class ServicioLoggingDecorator : IServicio
{
    private readonly IServicio _inner;
    private readonly ILogger<ServicioLoggingDecorator> _logger;

    public ServicioLoggingDecorator(
        IServicio inner,
        ILogger<ServicioLoggingDecorator> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public List<IdiomaDTO> GetIdiomas()
    {
        return ExecuteWithLogging(
            nameof(GetIdiomas),
            () => _inner.GetIdiomas(),
            result => $"Obtenidos {result.Count} idiomas"
        );
    }

    public List<DiccionarioDTO>? GetDiccionarios(string codigoIdioma)
    {
        return ExecuteWithLogging(
            nameof(GetDiccionarios),
            () => _inner.GetDiccionarios(codigoIdioma),
            result => result != null 
                ? $"Obtenidos {result.Count} diccionarios para '{codigoIdioma}'" 
                : $"No hay diccionarios para '{codigoIdioma}'",
            ("codigoIdioma", codigoIdioma)
        );
    }

    public DiccionarioDTO? GetDiccionario(string codigoDiccionario)
    {
        return ExecuteWithLogging(
            nameof(GetDiccionario),
            () => _inner.GetDiccionario(codigoDiccionario),
            result => result != null 
                ? $"Diccionario '{codigoDiccionario}' encontrado" 
                : $"Diccionario '{codigoDiccionario}' no encontrado",
            ("codigoDiccionario", codigoDiccionario)
        );
    }

    public List<SignificadoDTO>? GetSignificadosEnDiccionario(string codigoDiccionario, string palabra)
    {
        return ExecuteWithLogging(
            nameof(GetSignificadosEnDiccionario),
            () => _inner.GetSignificadosEnDiccionario(codigoDiccionario, palabra),
            result => result != null 
                ? $"Encontrados {result.Count} significados de '{palabra}' en '{codigoDiccionario}'" 
                : $"No se encontró '{palabra}' en '{codigoDiccionario}'",
            ("codigoDiccionario", codigoDiccionario), ("palabra", palabra)
        );
    }

    public List<SignificadoDTO>? GetSignificadosEnIdioma(string codigoIdioma, string palabra)
    {
        return ExecuteWithLogging(
            nameof(GetSignificadosEnIdioma),
            () => _inner.GetSignificadosEnIdioma(codigoIdioma, palabra),
            result => result != null 
                ? $"Encontrados {result.Count} significados de '{palabra}' en idioma '{codigoIdioma}'" 
                : $"No se encontró '{palabra}' en idioma '{codigoIdioma}'",
            ("codigoIdioma", codigoIdioma), ("palabra", palabra)
        );
    }

    public bool ExistePalabraEnDiccionario(string codigoDiccionario, string palabra)
    {
        return ExecuteWithLogging(
            nameof(ExistePalabraEnDiccionario),
            () => _inner.ExistePalabraEnDiccionario(codigoDiccionario, palabra),
            result => $"'{palabra}' {(result ? "existe" : "no existe")} en '{codigoDiccionario}'",
            ("codigoDiccionario", codigoDiccionario), ("palabra", palabra)
        );
    }

    public bool ExistePalabraEnIdioma(string codigoIdioma, string palabra)
    {
        return ExecuteWithLogging(
            nameof(ExistePalabraEnIdioma),
            () => _inner.ExistePalabraEnIdioma(codigoIdioma, palabra),
            result => $"'{palabra}' {(result ? "existe" : "no existe")} en idioma '{codigoIdioma}'",
            ("codigoIdioma", codigoIdioma), ("palabra", palabra)
        );
    }

    private T ExecuteWithLogging<T>(
        string methodName,
        Func<T> action,
        Func<T, string> successMessage,
        params (string name, object? value)[] parameters)
    {
        var paramsString = parameters.Length > 0 
            ? string.Join(", ", parameters.Select(p => $"{p.name}={p.value}")) 
            : "";
        
        _logger.LogInformation("Iniciando {Method}({Params})", methodName, paramsString);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = action();
            stopwatch.Stop();
            
            _logger.LogInformation("{Method} completado en {ElapsedMs}ms: {Result}", 
                methodName, stopwatch.ElapsedMilliseconds, successMessage(result));
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Error en {Method}({Params}) después de {ElapsedMs}ms", 
                methodName, paramsString, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
