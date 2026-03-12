namespace UI.Api;

/// <summary>
/// Interfaz para la capa de presentación de la aplicación.
/// Define el contrato para mostrar información al usuario,
/// independiente del medio (consola, GUI, web, etc.).
/// </summary>
public interface IUIApp
{
    void MostrarMensajeBienvenida();
    void MostrarMensajeDespedida();
    void MostrarSignificadosDePalabra(string palabra, IList<string> significados);
    void MostrarErrorNoHayDiccionario(string idioma);
    void MostrarErrorNoHayPalabra(string palabra, string idioma);
    void MostrarErrorArgumentosIncorrectos();
    void MostrarBuscando(string palabra, string idioma);
    void MostrarDiccionarioCargado(string idioma);
    void MostrarErrorInterno(string mensaje);
    void MostrarErrorSinSignificados(string palabra);
}
