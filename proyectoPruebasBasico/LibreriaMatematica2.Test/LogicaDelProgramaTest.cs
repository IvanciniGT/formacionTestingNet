using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace LibreriaMatematica2.Test;
using LibreriaMatematica;
using AppConsola;
using Microsoft.Extensions.Hosting;
using Moq;

public class LogicaDelProgramaTest
{
    [Fact]
    public void Ejecutar_con_libreria_real_muestra_los_resultados_esperados()
    {
        var sut = CrearSut(new global::LibreriaMatematica.LibreriaMatematica(), out _);

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var input = new StringReader("5" + Environment.NewLine + "10" + Environment.NewLine);
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            sut.ejecutar();

            string resultado = output.ToString();
            Assert.Contains("Hello, cuentecitas!", resultado);
            Assert.Contains("Ingrese el primer número:", resultado);
            Assert.Contains("Ingrese el segundo número:", resultado);
            Assert.Contains("La suma de 5 y 10 es: 15", resultado);
            Assert.Contains("La resta de 5 y 10 es: -5", resultado);
            Assert.Contains("¡Hasta luego, cuentecitas!", resultado);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Ejecutar_con_dependencia_mockeada_usa_la_libreria_inyectada()
    {
        var mockMatematica = new Mock<ILibreriaMatematica>();
        mockMatematica.Setup(m => m.Sumar(50, 70)).Returns(33);
        mockMatematica.Setup(m => m.Restar(50, 70)).Returns(11);

        var sut = CrearSut(mockMatematica.Object, out _);

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var input = new StringReader("50" + Environment.NewLine + "70" + Environment.NewLine);
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            sut.ejecutar();

            string resultado = output.ToString();
            Assert.Contains("La suma de 50 y 70 es: 33", resultado);
            Assert.Contains("La resta de 50 y 70 es: 11", resultado);
            mockMatematica.Verify(m => m.Sumar(50, 70), Times.Once);
            mockMatematica.Verify(m => m.Restar(50, 70), Times.Once);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Ejecutar_con_primer_numero_invalido_muestra_error_y_no_hace_operaciones()
    {
        var mockMatematica = new Mock<ILibreriaMatematica>(MockBehavior.Strict);
        var sut = CrearSut(mockMatematica.Object, out _);

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var input = new StringReader("abc" + Environment.NewLine + "10" + Environment.NewLine);
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            sut.ejecutar();

            string resultado = output.ToString();
            Assert.Contains("Por favor, ingrese números válidos.", resultado);
            Assert.DoesNotContain("¡Hasta luego, cuentecitas!", resultado);
            mockMatematica.Verify(m => m.Sumar(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockMatematica.Verify(m => m.Restar(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Ejecutar_con_segundo_numero_invalido_muestra_error_y_no_hace_operaciones()
    {
        var mockMatematica = new Mock<ILibreriaMatematica>(MockBehavior.Strict);
        var sut = CrearSut(mockMatematica.Object, out _);

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var input = new StringReader("5" + Environment.NewLine + "xyz" + Environment.NewLine);
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            sut.ejecutar();

            string resultado = output.ToString();
            Assert.Contains("Por favor, ingrese números válidos.", resultado);
            Assert.DoesNotContain("¡Hasta luego, cuentecitas!", resultado);
            mockMatematica.Verify(m => m.Sumar(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockMatematica.Verify(m => m.Restar(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task StartAsync_ejecuta_la_logica_y_detiene_la_aplicacion()
    {
        var mockMatematica = new Mock<ILibreriaMatematica>();
        mockMatematica.Setup(m => m.Sumar(8, 2)).Returns(10);
        mockMatematica.Setup(m => m.Restar(8, 2)).Returns(6);

        var sut = CrearSut(mockMatematica.Object, out var mockLifetime);

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        using var input = new StringReader("8" + Environment.NewLine + "2" + Environment.NewLine);
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            await sut.StartAsync(CancellationToken.None);

            string resultado = output.ToString();
            Assert.Contains("La suma de 8 y 2 es: 10", resultado);
            Assert.Contains("La resta de 8 y 2 es: 6", resultado);
            mockMatematica.Verify(m => m.Sumar(8, 2), Times.Once);
            mockMatematica.Verify(m => m.Restar(8, 2), Times.Once);
            mockLifetime.Verify(l => l.StopApplication(), Times.Once);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task StopAsync_completa_correctamente()
    {
        var sut = CrearSut(new global::LibreriaMatematica.LibreriaMatematica(), out _);

        Task resultado = sut.StopAsync(CancellationToken.None);
        await resultado;

        Assert.True(resultado.IsCompletedSuccessfully);
    }

    private static LogicaDelPrograma CrearSut(ILibreriaMatematica matematica, out Mock<IHostApplicationLifetime> mockLifetime)
    {
        mockLifetime = new Mock<IHostApplicationLifetime>();
        return new LogicaDelPrograma(matematica, mockLifetime.Object);
    }
}