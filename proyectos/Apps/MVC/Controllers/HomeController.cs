using Microsoft.AspNetCore.Mvc;
using ServicioDiccionarios;
using App.MVC.ViewModels;

namespace App.MVC.Controllers;

/// <summary>
/// Controller MVC para la página de inicio.
/// Muestra la lista de idiomas disponibles.
/// </summary>
public class HomeController : Controller
{
    private readonly IServicio _servicio;

    public HomeController(IServicio servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Página principal: lista de idiomas disponibles
    /// </summary>
    public IActionResult Index()
    {
        var idiomas = _servicio.GetIdiomas();
        var viewModel = idiomas.Select(i => new IdiomaViewModel
        {
            Codigo = i.Codigo,
            Nombre = i.Nombre
        }).ToList();

        return View(viewModel);
    }
}
