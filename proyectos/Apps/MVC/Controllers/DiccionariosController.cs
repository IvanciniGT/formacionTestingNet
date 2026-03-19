using Microsoft.AspNetCore.Mvc;
using ServicioDiccionarios;
using App.MVC.ViewModels;

namespace App.MVC.Controllers;

/// <summary>
/// Controller MVC para diccionarios: ver diccionarios de un idioma y buscar palabras.
/// </summary>
public class DiccionariosController : Controller
{
    private readonly IServicio _servicio;

    public DiccionariosController(IServicio servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los diccionarios disponibles para un idioma.
    /// GET /Diccionarios?codigoIdioma=ES
    /// </summary>
    public IActionResult Index(string codigoIdioma)
    {
        var diccionarios = _servicio.GetDiccionarios(codigoIdioma);

        if (diccionarios == null)
            return NotFound($"Idioma '{codigoIdioma}' no encontrado");

        var viewModel = diccionarios.Select(d => new DiccionarioViewModel
        {
            Codigo = d.Codigo,
            Nombre = d.Nombre,
            Idioma = d.Idioma
        }).ToList();

        ViewBag.CodigoIdioma = codigoIdioma;
        return View(viewModel);
    }

    /// <summary>
    /// Muestra el formulario de búsqueda y los resultados.
    /// GET /Diccionarios/Buscar?codigoDiccionario=ES_RAE&palabra=casa
    /// </summary>
    public IActionResult Buscar(string codigoDiccionario, string? palabra)
    {
        var diccionario = _servicio.GetDiccionario(codigoDiccionario);
        if (diccionario == null)
            return NotFound($"Diccionario '{codigoDiccionario}' no encontrado");

        var viewModel = new BusquedaViewModel
        {
            CodigoDiccionario = codigoDiccionario,
            NombreDiccionario = diccionario.Nombre
        };

        if (!string.IsNullOrWhiteSpace(palabra))
        {
            viewModel.Palabra = palabra;
            var significados = _servicio.GetSignificadosEnDiccionario(codigoDiccionario, palabra);

            if (significados == null || significados.Count == 0)
            {
                viewModel.SinResultados = true;
            }
            else
            {
                viewModel.Significados = significados.Select(s => s.Texto).ToList();
            }
        }

        return View(viewModel);
    }
}
