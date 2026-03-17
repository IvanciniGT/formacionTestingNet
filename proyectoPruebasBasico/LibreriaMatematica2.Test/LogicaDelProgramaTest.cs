namespace LibreriaMatematica2.Test;
using LibreriaMatematica;
using AppConsola;
using Microsoft.Extensions.Hosting;
using Moq;

public class LogicaDelProgramaTest
{
    private LogicaDelPrograma logicaDelProgramaSistema;
    private LogicaDelPrograma logicaDelProgramaAislada;
    private Mock<ILibreriaMatematica> mockMatematica;
    private Mock<IHostApplicationLifetime> mockLifetime;

    // Ahora vamos con XUnit... Cambia un poco la sintaxis.. pero los conceptos NADA!

    public LogicaDelProgramaTest()
    {
        // Mock del IHostApplicationLifetime (necesario para LogicaDelPrograma)
        mockLifetime = new Mock<IHostApplicationLifetime>();
        
        LibreriaMatematica libreriaAUsar = new LibreriaMatematica();
        logicaDelProgramaSistema = new LogicaDelPrograma(libreriaAUsar, mockLifetime.Object);

        // En lugar de un cutrestub manual, usamos Moq!
        // Mock es un objeto que simula el comportamiento de otro objeto.
        mockMatematica = new Mock<ILibreriaMatematica>(); // En realidad lo que me crea es un dummy
        
        
        // .Object nos da la instancia que implementa ILibreriaMatematica
        logicaDelProgramaAislada = new LogicaDelPrograma(mockMatematica.Object, mockLifetime.Object);
    }

    [Fact] 
    public void VerSiElProgramaFunciona()
    {

        // Dado que el programa interactua con la consola... Le pego unn cambiazo a la consola.
        //  Cambio el STDIN, para simular la entrada del usuario, y el STDOUT, para capturar la salida del programa, y luego poder hacer aserciones sobre esa salida.

        // Hago que el STDOut sea un StringWriter, para capturar la salida del programa.
        using var sw = new StringWriter();
        Console.SetOut(sw);
        // Para mandarle números al programa, hago que el STDIN sea un StringReader, y le paso los números que quiero que el programa lea, separados por un salto de línea.
        using var sr = new StringReader("5\n10\n");
        Console.SetIn(sr);

        //Assert.Pass(); // La prueba pasa sin problemas.
        // DADO: GIVEN: CONTEXTO: En qué condiciones se va a ejecutar la función de prueba.
        // ACCION: WHEN: CUANDO: Qué función se va a ejecutar en la función de prueba.
        logicaDelProgramaSistema.ejecutar();
        // COMPROBACION: THEN: ENTONCES: Qué resultado se espera de la función de prueba.
        var resultado = sw.ToString().Trim();
        // Asegurate que contiene el mensaje de bienvenida, el mensaje de despedida, y el mensaje de error por no ingresar números válidos.
        Assert.Contains("Hello, cuentecitas!", resultado);
        Assert.Contains("¡Hasta luego, cuentecitas!", resultado);
        Assert.Contains("Ingrese el primer número:", resultado);
        Assert.Contains("Ingrese el segundo número:", resultado);
        Assert.Contains("La suma de 5 y 10 es: 15", resultado);
        Assert.Contains("La resta de 5 y 10 es: -5", resultado);
    }
    // Pero esto es una prueba de Sistema y en este caso igual a la de intengración.
    [Fact] 
    public void VerSiElProgramaFuncionaAislada()
    {

        // Dado que el programa interactua con la consola... Le pego unn cambiazo a la consola.
        //  Cambio el STDIN, para simular la entrada del usuario, y el STDOUT, para capturar la salida del programa, y luego poder hacer aserciones sobre esa salida.
        // Configuramos el comportamiento del mock:
        // "Cuando llamen a Sumar con cualquier int, devuelve 33"
        mockMatematica.Setup(m => m.Sumar(It.IsAny<int>(), It.IsAny<int>())).Returns(33);
        // "Cuando llamen a Restar con cualquier int, devuelve 11"
        mockMatematica.Setup(m => m.Restar(It.IsAny<int>(), It.IsAny<int>())).Returns(11);

        // Hago que el STDOut sea un StringWriter, para capturar la salida del programa.
        using var sw = new StringWriter();
        Console.SetOut(sw);
        // Para mandarle números al programa, hago que el STDIN sea un StringReader, y le paso los números que quiero que el programa lea, separados por un salto de línea.
        using var sr = new StringReader("50\n70\n");
        Console.SetIn(sr);

        //Assert.Pass(); // La prueba pasa sin problemas.
        // DADO: GIVEN: CONTEXTO: En qué condiciones se va a ejecutar la función de prueba.
        // ACCION: WHEN: CUANDO: Qué función se va a ejecutar en la función de prueba.
        logicaDelProgramaAislada.ejecutar();
        // COMPROBACION: THEN: ENTONCES: Qué resultado se espera de la función de prueba.
        var resultado = sw.ToString().Trim();
        // Asegurate que contiene el mensaje de bienvenida, el mensaje de despedida, y el mensaje de error por no ingresar números válidos.
        Assert.Contains("Hello, cuentecitas!", resultado);
        Assert.Contains("¡Hasta luego, cuentecitas!", resultado);
        Assert.Contains("Ingrese el primer número:", resultado);
        Assert.Contains("Ingrese el segundo número:", resultado);
        Assert.Contains("La suma de 50 y 70 es: 33", resultado);
        Assert.Contains("La resta de 50 y 70 es: 11", resultado);
        // ME aseguro que el mock se haya llamado con los números que le pasé al programa.
        mockMatematica.Verify(m => m.Sumar(50, 70), Times.Once);
        mockMatematica.Verify(m => m.Restar(50, 70), Times.Once);
    }
    // Prueba unitaria.
}

/*

        // Pedido al usuario 2 númemros:
        Console.WriteLine("Ingrese el primer número:");
        string input1 = Console.ReadLine();
        Console.WriteLine("Ingrese el segundo número:");
        string input2 = Console.ReadLine();

        // Verifico que son números y los convierto a enteros:
        if (!int.TryParse(input1, out int numero1) || !int.TryParse(input2, out int numero2))
        {
            Console.WriteLine("Por favor, ingrese números válidos.");
            return;
        } else
        {
            // Realizo la suma:
            int resultado = matematica.Sumar(numero1, numero2);

            // Muestro el resultado:
            Console.WriteLine($"La suma de {numero1} y {numero2} es: {resultado}");

            // Realizo la resta:
            int resultadoResta = matematica.Restar(numero1, numero2);

            // Muestro el resultado de la resta:
            Console.WriteLine($"La resta de {numero1} y {numero2} es: {resultadoResta}");
        }

        // Chao pescao!*/