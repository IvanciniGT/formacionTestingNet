namespace LibreriaMatematica2.Test;
using LibreriaMatematica;
public class LibreriaMatematicaStub : ILibreriaMatematica
{

    public int Sumar(int a, int b)
    {
        if (a != 50 || b != 70) 
        {
            throw new ArgumentException("Solo se permiten los números 50 y 70");
        }
        return 33;
    }

    public int Restar(int a, int b)
    {
        return 11;
    }
}