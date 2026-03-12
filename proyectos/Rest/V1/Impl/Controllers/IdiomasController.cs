using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ServicioDiccionarios;
using DiccionariosRestV1Api;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DiccionariosRestV1.Controllers;

/// <summary>
/// Controller REST para gestión de idiomas.
/// Endpoints: GET /api/v1/idiomas
/// </summary>
/// <remarks>
/// Los XML comments en cada método generan documentación Swagger automática
/// gracias a NSwag (UseOpenApi/UseSwaggerUi en Program.cs).
/// </remarks>
[ApiController]
[Route("api/v1/idiomas")]
public class IdiomasController : ControllerBase
{
    private readonly IServicio _servicioDiccionarios;
    private readonly IMapper _mapper;
    private readonly ILogger<IdiomasController> _logger;

    public IdiomasController(
        IServicio servicioDiccionarios,
        IMapper mapper,
        ILogger<IdiomasController> logger)
    {
        _servicioDiccionarios = servicioDiccionarios;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los idiomas disponibles
    /// </summary>
    /// <returns>Lista de idiomas</returns>
    /// <response code="200">Lista de idiomas disponibles</response>
    [HttpGet]
    [ProducesResponseType(typeof(IList<IdiomaRestV1DTO>), 200)]
    public ActionResult<IList<IdiomaRestV1DTO>> GetIdiomas()
    {
        try
        {
            _logger.LogInformation("Obteniendo lista de idiomas disponibles");
            
            var idiomas = _servicioDiccionarios.GetIdiomas();
            var idiomasRest = _mapper.Map<IList<IdiomaRestV1DTO>>(idiomas);
            
            _logger.LogInformation("Devueltos {Count} idiomas", idiomasRest.Count);
            return Ok(idiomasRest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener idiomas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene todos los diccionarios disponibles para un idioma
    /// </summary>
    /// <param name="codigoIdioma">Código del idioma (ej: ES, EN)</param>
    /// <returns>Lista de diccionarios del idioma</returns>
    /// <response code="200">Lista de diccionarios</response>
    /// <response code="404">Idioma no encontrado</response>
    [HttpGet("{codigoIdioma}/diccionarios")]
    [ProducesResponseType(typeof(IList<DiccionarioRestV1DTO>), 200)]
    [ProducesResponseType(404)]
    public ActionResult<IList<DiccionarioRestV1DTO>> GetDiccionariosPorIdioma(
        [Required] string codigoIdioma)
    {
        try
        {
            _logger.LogInformation("Obteniendo diccionarios para idioma {CodigoIdioma}", codigoIdioma);
            
            var diccionarios = _servicioDiccionarios.GetDiccionarios(codigoIdioma);
            
            if (diccionarios == null)
            {
                _logger.LogWarning("Idioma no encontrado: {CodigoIdioma}", codigoIdioma);
                return NotFound($"Idioma '{codigoIdioma}' no encontrado");
            }

            var diccionariosRest = _mapper.Map<IList<DiccionarioRestV1DTO>>(diccionarios);
            
            _logger.LogInformation("Devueltos {Count} diccionarios para idioma {CodigoIdioma}", 
                diccionariosRest.Count, codigoIdioma);
            return Ok(diccionariosRest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener diccionarios para idioma {CodigoIdioma}", codigoIdioma);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los significados de una palabra en todos los diccionarios de un idioma
    /// </summary>
    /// <param name="codigoIdioma">Código del idioma</param>
    /// <param name="palabra">Palabra a buscar</param>
    /// <returns>Lista de significados de todos los diccionarios</returns>
    /// <response code="200">Significados encontrados</response>
    /// <response code="404">No se encontraron significados</response>
    [HttpGet("{codigoIdioma}/significados")]
    [ProducesResponseType(typeof(IList<SignificadoRestV1DTO>), 200)]
    [ProducesResponseType(404)]
    public ActionResult<IList<SignificadoRestV1DTO>> GetSignificadosPorIdioma(
        [Required] string codigoIdioma,
        [Required][FromQuery] string palabra)
    {
        try
        {
            _logger.LogInformation("Buscando significados de '{Palabra}' en idioma {CodigoIdioma}", 
                palabra, codigoIdioma);
            
            var significados = _servicioDiccionarios.GetSignificadosEnIdioma(codigoIdioma, palabra);
            
            if (significados == null)
            {
                _logger.LogWarning("No se encontraron significados para '{Palabra}' en idioma {CodigoIdioma}", 
                    palabra, codigoIdioma);
                return NotFound($"No se encontraron significados para '{palabra}' en idioma '{codigoIdioma}'");
            }

            var significadosRest = _mapper.Map<IList<SignificadoRestV1DTO>>(significados);
            
            _logger.LogInformation("Devueltos {Count} significados para '{Palabra}' en idioma {CodigoIdioma}", 
                significadosRest.Count, palabra, codigoIdioma);
            return Ok(significadosRest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar significados de '{Palabra}' en idioma {CodigoIdioma}", 
                palabra, codigoIdioma);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Verifica si una palabra existe en algún diccionario del idioma
    /// </summary>
    /// <param name="codigoIdioma">Código del idioma</param>
    /// <param name="palabra">Palabra a verificar</param>
    /// <response code="200">La palabra existe</response>
    /// <response code="404">La palabra no existe</response>
    [HttpHead("{codigoIdioma}/existe")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult ExistePalabraEnIdioma(
        [Required] string codigoIdioma,
        [Required] [FromQuery] string palabra)
    {
        try
        {
            _logger.LogDebug("Verificando existencia de '{Palabra}' en idioma {CodigoIdioma}", 
                palabra, codigoIdioma);
            
            var existe = _servicioDiccionarios.ExistePalabraEnIdioma(codigoIdioma, palabra);
            
            if (existe)
            {
                _logger.LogDebug("Palabra '{Palabra}' existe en idioma {CodigoIdioma}", palabra, codigoIdioma);
                return Ok();
            }
            else
            {
                _logger.LogDebug("Palabra '{Palabra}' no existe en idioma {CodigoIdioma}", palabra, codigoIdioma);
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de '{Palabra}' en idioma {CodigoIdioma}", 
                palabra, codigoIdioma);
            return StatusCode(500);
        }
    }
}
