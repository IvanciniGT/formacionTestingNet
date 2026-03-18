namespace Diccionarios.BBDD.Tests;

using Diccionarios.BBDD.Entities;

public class IdiomaEntityTests
{
    [Fact]
    public void CrearIdiomaEntity_DeberiaTenerPropiedades()
    {
        // Arrange
        int id = 1;
        string codigo = "es";
        string nombre = "Español";

        // Act
        var idioma = new IdiomaEntity
        {
            Id = id,
            Codigo = codigo,
            Nombre = nombre
        };

        // Assert
        Assert.Equal(id, idioma.Id);
        Assert.Equal(codigo, idioma.Codigo);
        Assert.Equal(nombre, idioma.Nombre);
    }

}
