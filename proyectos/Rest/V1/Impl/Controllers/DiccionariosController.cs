using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ServicioDiccionarios;
using DiccionariosRestV1Api;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DiccionariosRestV1.Controllers;

/// <summary>
/// Controller REST para consulta de diccionarios y búsqueda de palabras.
/// Endpoints: GET /api/v1/diccionarios/{codigo}, GET /api/v1/diccionarios/{codigo}/significados
/// </summary>
[ApiController]
[Route("api/v1/diccionarios")]
public class DiccionariosController : ControllerBase
{
    private readonly IServicio _servicioDiccionarios;
    private readonly IMapper _mapper;
    private readonly ILogger<DiccionariosController> _logger;

    public DiccionariosController(
        IServicio servicioDiccionarios,
        IMapper mapper,
        ILogger<DiccionariosController> logger)
    {
        _servicioDiccionarios = servicioDiccionarios;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene un diccionario específico por su código
    /// </summary>
    /// <param name="codigoDiccionario">Código único del diccionario (ej: ES_RAE)</param>
    /// <returns>Diccionario encontrado</returns>
    /// <response code="200">Diccionario encontrado</response>
    /// <response code="404">Diccionario no encontrado</response>
    [HttpGet("{codigoDiccionario}")]
    [ProducesResponseType(typeof(DiccionarioRestV1DTO), 200)]
    [ProducesResponseType(404)]
    public ActionResult<DiccionarioRestV1DTO> GetDiccionario(
        [Required] string codigoDiccionario)
    {
        try
        {
            _logger.LogInformation("Obteniendo diccionario {CodigoDiccionario}", codigoDiccionario);
            
            var diccionario = _servicioDiccionarios.GetDiccionario(codigoDiccionario);
            
            if (diccionario == null)
            {
                _logger.LogWarning("Diccionario no encontrado: {CodigoDiccionario}", codigoDiccionario);
                return NotFound($"Diccionario '{codigoDiccionario}' no encontrado");
            }

            var diccionarioRest = _mapper.Map<DiccionarioRestV1DTO>(diccionario);
            
            _logger.LogInformation("Diccionario {CodigoDiccionario} devuelto", codigoDiccionario);
            return Ok(diccionarioRest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener diccionario {CodigoDiccionario}", codigoDiccionario);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los significados de una palabra en un diccionario específico
    /// </summary>
    /// <param name="codigoDiccionario">Código del diccionario</param>
    /// <param name="palabra">Palabra a buscar</param>
    /// <returns>Lista de significados</returns>
    /// <response code="200">Significados encontrados</response>
    /// <response code="404">Palabra no encontrada</response>
    [HttpGet("{codigoDiccionario}/significados")]
    [ProducesResponseType(typeof(IList<SignificadoRestV1DTO>), 200)]
    [ProducesResponseType(404)]
    public ActionResult<IList<SignificadoRestV1DTO>> GetSignificadosPorDiccionario(
        [Required] string codigoDiccionario,
        [Required] [FromQuery] string palabra)
    {
        try
        {
            _logger.LogInformation("Buscando significados de '{Palabra}' en diccionario {CodigoDiccionario}", 
                palabra, codigoDiccionario);
            
            var significados = _servicioDiccionarios.GetSignificadosEnDiccionario(codigoDiccionario, palabra);
            
            if (significados == null)
            {
                _logger.LogWarning("No se encontraron significados para '{Palabra}' en diccionario {CodigoDiccionario}", 
                    palabra, codigoDiccionario);
                return NotFound($"No se encontraron significados para '{palabra}' en diccionario '{codigoDiccionario}'");
            }

            var significadosRest = _mapper.Map<IList<SignificadoRestV1DTO>>(significados);
            
            _logger.LogInformation("Devueltos {Count} significados para '{Palabra}' en diccionario {CodigoDiccionario}", 
                significadosRest.Count, palabra, codigoDiccionario);
            return Ok(significadosRest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar significados de '{Palabra}' en diccionario {CodigoDiccionario}", 
                palabra, codigoDiccionario);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Verifica si una palabra existe en un diccionario específico
    /// </summary>
    /// <param name="codigoDiccionario">Código del diccionario</param>
    /// <param name="palabra">Palabra a verificar</param>
    /// <response code="200">La palabra existe</response>
    /// <response code="404">La palabra no existe</response>
    [HttpHead("{codigoDiccionario}/existe")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult ExistePalabraEnDiccionario(
        [Required] string codigoDiccionario,
        [Required] [FromQuery] string palabra)
    {
        try
        {
            _logger.LogDebug("Verificando existencia de '{Palabra}' en diccionario {CodigoDiccionario}", 
                palabra, codigoDiccionario);
            
            var existe = _servicioDiccionarios.ExistePalabraEnDiccionario(codigoDiccionario, palabra);
            
            if (existe)
            {
                _logger.LogDebug("Palabra '{Palabra}' existe en diccionario {CodigoDiccionario}", 
                    palabra, codigoDiccionario);
                return Ok();
            }
            else
            {
                _logger.LogDebug("Palabra '{Palabra}' no existe en diccionario {CodigoDiccionario}", 
                    palabra, codigoDiccionario);
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de '{Palabra}' en diccionario {CodigoDiccionario}", 
                palabra, codigoDiccionario);
            return StatusCode(500);
        }
    }
}
