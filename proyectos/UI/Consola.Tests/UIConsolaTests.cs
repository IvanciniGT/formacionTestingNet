using UI.Consola;

namespace UI.Consola.Tests;

/// <summary>
/// Tests para UIConsola: se redirige Console.Out a un StringWriter
/// para verificar que cada método escribe el texto esperado.
/// </summary>
public class UIConsolaTests : IDisposable
{
    private readonly UIConsola _sut;
    private readonly StringWriter _output;
    private readonly TextWriter _originalOutput;

    public UIConsolaTests()
    {
        _sut = new UIConsola();
        _output = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_output);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _output.Dispose();
    }

    private string GetOutput() => _output.ToString();

    [Fact]
    public void MostrarMensajeBienvenida_EscribeBienvenidaEnConsola()
    {
        _sut.MostrarMensajeBienvenida();

        var output = GetOutput();
        Assert.Contains("Bienvenido", output);
        Assert.Contains("Diccionario", output);
    }

    [Fact]
    public void MostrarMensajeDespedida_EscribeDespedidaEnConsola()
    {
        _sut.MostrarMensajeDespedida();

        var output = GetOutput();
        Assert.Contains("Gracias", output);
        Assert.Contains("Diccionario", output);
    }

    [Fact]
    public void MostrarSignificadosDePalabra_MuestraPalabraYSignificados()
    {
        var significados = new List<string> { "Edificio para habitar", "Hogar familiar" };

        _sut.MostrarSignificadosDePalabra("casa", significados);

        var output = GetOutput();
        Assert.Contains("casa", output);
        Assert.Contains("Edificio para habitar", output);
        Assert.Contains("Hogar familiar", output);
    }

    [Fact]
    public void MostrarSignificadosDePalabra_MuestraNumeracion()
    {
        var significados = new List<string> { "Significado 1", "Significado 2" };

        _sut.MostrarSignificadosDePalabra("test", significados);

        var output = GetOutput();
        Assert.Contains("1.", output);
        Assert.Contains("2.", output);
    }

    [Fact]
    public void MostrarErrorNoHayDiccionario_MuestraIdiomaYMensajeError()
    {
        _sut.MostrarErrorNoHayDiccionario("FR");

        var output = GetOutput();
        Assert.Contains("FR", output);
        Assert.Contains(".txt", output);
    }

    [Fact]
    public void MostrarErrorNoHayPalabra_MuestraPalabraEIdioma()
    {
        _sut.MostrarErrorNoHayPalabra("xyz", "ES");

        var output = GetOutput();
        Assert.Contains("xyz", output);
        Assert.Contains("ES", output);
    }

    [Fact]
    public void MostrarErrorArgumentosIncorrectos_MuestraUsoEjemplo()
    {
        _sut.MostrarErrorArgumentosIncorrectos();

        var output = GetOutput();
        Assert.Contains("2 argumentos", output);
        Assert.Contains("Ejemplo", output);
    }

    [Fact]
    public void MostrarBuscando_MuestraPalabraEIdioma()
    {
        _sut.MostrarBuscando("casa", "ES");

        var output = GetOutput();
        Assert.Contains("casa", output);
        Assert.Contains("ES", output);
    }

    [Fact]
    public void MostrarDiccionarioCargado_MuestraIdioma()
    {
        _sut.MostrarDiccionarioCargado("ES");

        var output = GetOutput();
        Assert.Contains("ES", output);
        Assert.Contains("cargado", output.ToLower());
    }

    [Fact]
    public void MostrarErrorInterno_MuestraMensajeDeError()
    {
        _sut.MostrarErrorInterno("fallo de conexión");

        var output = GetOutput();
        Assert.Contains("fallo de conexión", output);
    }

    [Fact]
    public void MostrarErrorSinSignificados_MuestraPalabra()
    {
        _sut.MostrarErrorSinSignificados("abcdef");

        var output = GetOutput();
        Assert.Contains("abcdef", output);
    }
}
