namespace LibreriaMatematica.Test;

public class LibreriaMatematicaTest
{
    private LibreriaMatematica libreriaMatematica;

    [SetUp] // Una vez crea la instancia de la clase, ejecuta esta función antes de cada función de prueba.
            // Lo usamos para preparativos comunes a todas las funciones de prueba.
    public void Setup()
    {
        libreriaMatematica = new LibreriaMatematica();
    }

    [Test] // Esta anotación marca la función como una función de Pruebas, y se ejecuta por dotnet test
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
        Assert.That(resultado, Is.EqualTo(esperado));   // El resultado se espera que sea igual a 15.
    }

    // TDD = TEST FIRST + REFACTORING EN CADA ITERACION
    //[Test] // Esta anotación marca la función como una función de Pruebas, y se ejecuta por dotnet test
    // Para probar con varios datos, y no con 1, marco la función como. [TestCase] y le paso los datos que quiero probar, y el resultado esperado.
    [TestCase(5, 10, -5)]
    [TestCase(15, 10, 5)]
    [TestCase(5, 0, 5)]
    [TestCase(0, 10, -10)]
    public void TestResta( int numero1, int numero2, int esperado)
    {
        //Assert.Pass(); // La prueba pasa sin problemas.
        // DADO: GIVEN: CONTEXTO: En qué condiciones se va a ejecutar la función de prueba.
        // ACCION: WHEN: CUANDO: Qué función se va a ejecutar en la función de prueba.
        int resultado = libreriaMatematica.Restar(numero1, numero2);
        // COMPROBACION: THEN: ENTONCES: Qué resultado se espera de la función de prueba.
        Assert.That(resultado, Is.EqualTo(esperado));   // El resultado se espera que sea igual a -5.

        // Al hacer primerop la prueba, estoy definiendo el API, y el comportamiento de la función, y luego ya implemento la función para que pase la prueba.
    }

    [TearDown] // Una vez ejecutada la función de prueba, ejecuta esta función después de cada función de prueba.
               // Limpiezas tras una prueba... para no dejar basura en el entorno.
    public void TearDown()
    {

    }
}

// Quien ejecuta esas funciones de prueba?
// Es decir... al final lo que tengo es una clase con métodos
// Para ejecutar esos métodos de la clase será necesario hacer:

// LibreriaMatematicaTest test = new LibreriaMatematicaTest();
// test.Test1();

// Hemos escrito ese código. NO!
// Pues magia no hay!
// Alguien ha escrito eso. Quién? NUnit

// Nuesto comando dotnet test , le pide a NUnit que ejecute las pruebas, 
// y NUnit se encarga de buscar todas las clases que tengan métodos con la anotación [Test]
// y NUnit se encarga de crear una instancia de esa clase, y ejecutar ese método.
// Eso se hace para cada método que tenga la anotación [Test] en esa clase, y para cada clase que tenga métodos con la anotación [Test] en el proyecto de pruebas.
// Si una clase tiene 5 métodos con la anotación [Test], NUnit se encargará de crear 5 instancias de esa clase y en cada instancia ejecutar un método diferente.
// Y así sucesivamente para cada clase que tenga métodos con la anotación [Test] en el proyecto de pruebas.