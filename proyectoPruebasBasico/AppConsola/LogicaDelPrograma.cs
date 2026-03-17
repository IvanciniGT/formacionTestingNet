namespace AppConsola;
using LibreriaMatematica;
using Microsoft.Extensions.Hosting;

public class LogicaDelPrograma : IHostedService
{
    private ILibreriaMatematica matematica;
    private IHostApplicationLifetime appLifetime;

    public LogicaDelPrograma(ILibreriaMatematica matematica, IHostApplicationLifetime appLifetime)
    {
        this.matematica = matematica;
        this.appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ejecutar();
        appLifetime.StopApplication(); // Termina la aplicación cuando acaba
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void ejecutar()
    {
        Console.WriteLine("Hello, cuentecitas!");

        // Pedido al usuario 2 númemros:
        Console.WriteLine("Ingrese el primer número:");
        string input1 = Console.ReadLine(); //50
        Console.WriteLine("Ingrese el segundo número:");
        string input2 = Console.ReadLine(); //70

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

        // Chao pescao!
        Console.WriteLine("¡Hasta luego, cuentecitas!");
    }
}