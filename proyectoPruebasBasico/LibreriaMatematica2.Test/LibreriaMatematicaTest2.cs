namespace LibreriaMatematica2.Test;
using LibreriaMatematica;

public class LibreriaMatematicaTest2
{
    private readonly LibreriaMatematica libreriaMatematica = new();

    [Theory]
    [InlineData(5, 10, 15)]
    [InlineData(-5, 10, 5)]
    [InlineData(0, 0, 0)]
    [InlineData(-3, -7, -10)]
    [InlineData(123, -23, 100)]
    public void Sumar_devuelve_el_resultado_esperado(int numero1, int numero2, int esperado)
    {
        int resultado = libreriaMatematica.Sumar(numero1, numero2);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData(5, 10, -5)]
    [InlineData(15, 10, 5)]
    [InlineData(5, 0, 5)]
    [InlineData(0, 10, -10)]
    [InlineData(-10, -5, -5)]
    [InlineData(-10, 5, -15)]
    public void Restar_devuelve_el_resultado_esperado(int numero1, int numero2, int esperado)
    {
        int resultado = libreriaMatematica.Restar(numero1, numero2);

        Assert.Equal(esperado, resultado);
    }
}
