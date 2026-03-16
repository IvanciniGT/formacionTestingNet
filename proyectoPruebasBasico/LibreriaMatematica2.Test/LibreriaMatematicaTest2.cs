namespace LibreriaMatematica2.Test;
using LibreriaMatematica;

public class LibreriaMatematicaTest2
{

    // Ahora vamos con XUnit... Cambia un poco la sintaxis.. pero los conceptos NADA!

    private LibreriaMatematica libreriaMatematica;

    public LibreriaMatematicaTest2()
    {
        libreriaMatematica = new LibreriaMatematica();
    }

    [Fact] // Esta anotación marca la función como una función de Pruebas, y se ejecuta por dotnet test
    public void Test1()
    {
        //Assert.Pass(); // La prueba pasa sin problemas.
        // DADO: GIVEN: CONTEXTO: En qué condiciones se va a ejecutar la función de prueba.
        int numero1 = 5;
        int numero2 = 10;
        // ACCION: WHEN: CUANDO: Qué función se va a ejecutar en la función de prueba.
        int resultado = libreriaMatematica.Sumar(numero1, numero2);
        // COMPROBACION: THEN: ENTONCES: Qué resultado se espera de la función de prueba.
        int esperado = 15;  
        Assert.Equal(esperado, resultado);   // El resultado se espera que sea igual a 15.
    }

    // TDD = TEST FIRST + REFACTORING EN CADA ITERACION
    //[Fact] // Esta anotación marca la función como una función de Pruebas, y se ejecuta por dotnet test
    // Para probar con varios datos, y no con 1, marco la función como. [TestCase] y le paso los datos que quiero probar, y el resultado esperado.
    [Theory]
    [InlineData(5, 10, -5)]
    [InlineData(15, 10, 5)]
    [InlineData(5, 0, 5)]
    [InlineData(0, 10, -10)]
    public void TestResta( int numero1, int numero2, int esperado)
    {
        //Assert.Pass(); // La prueba pasa sin problemas.
        // DADO: GIVEN: CONTEXTO: En qué condiciones se va a ejecutar la función de prueba.
        // ACCION: WHEN: CUANDO: Qué función se va a ejecutar en la función de prueba.
        int resultado = libreriaMatematica.Restar(numero1, numero2);
        // COMPROBACION: THEN: ENTONCES: Qué resultado se espera de la función de prueba.
        Assert.Equal(esperado, resultado);   // El resultado se espera que sea igual a -5.

        // Al hacer primerop la prueba, estoy definiendo el API, y el comportamiento de la función, y luego ya implemento la función para que pase la prueba.
    }
}
