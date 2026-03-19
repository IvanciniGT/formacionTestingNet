namespace App.MVC.ViewModels;

public class BusquedaViewModel
{
    public string CodigoDiccionario { get; set; } = string.Empty;
    public string NombreDiccionario { get; set; } = string.Empty;
    public string Palabra { get; set; } = string.Empty;
    public List<string> Significados { get; set; } = new();
    public bool SinResultados { get; set; }
}
