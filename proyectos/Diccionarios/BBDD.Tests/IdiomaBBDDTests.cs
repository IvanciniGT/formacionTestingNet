using Diccionarios.BBDD;
using Diccionarios.BBDD.Entities;

namespace Diccionarios.BBDD.Tests;

public class IdiomaBBDDTests
{
    // DRY: Configuración común para todos los tests
    private readonly IdiomaEntity _entity;
    private readonly IdiomaBBDD _idioma;

    public IdiomaBBDDTests()
    {
        _entity = new IdiomaEntity
        {
            Id = 1,
            Codigo = "es",
            Nombre = "Español"
        };
        _idioma = new IdiomaBBDD(_entity);
    }

    [Fact]
    public void Constructor_ConEntityValido_CreaInstancia()
    {
        // THEN: Se crea sin problemas (ya se creó en el constructor)
        Assert.NotNull(_idioma);
    }

    [Fact]
    public void Constructor_ConEntityNull_LanzaArgumentNullException()
    {
        // WHEN/THEN: Al crear IdiomaBBDD con null lanza excepción
        Assert.Throws<ArgumentNullException>(() => new IdiomaBBDD(null!));
    }

    [Fact]
    public void Nombre_DevuelveNombreDelEntity()
    {
        Assert.Equal(_entity.Nombre, _idioma.Nombre);
    }

    [Fact]
    public void Codigo_DevuelveCodigoDelEntity()
    {
        Assert.Equal(_entity.Codigo, _idioma.Codigo);
    }

    [Theory]
    [InlineData("es", "Español")]
    [InlineData("en", "English")]
    [InlineData("fr", "Français")]
    [InlineData("de", "Deutsch")]
    public void Propiedades_DevuelvenValoresCorrectos_ParaDistintosIdiomas(string codigo, string nombre)
    {
        // GIVEN: Un entity con los valores dados
        var entity = new IdiomaEntity { Codigo = codigo, Nombre = nombre };

        // WHEN: Creo el IdiomaBBDD
        var idioma = new IdiomaBBDD(entity);

        // THEN: Las propiedades devuelven los valores correctos
        Assert.Equal(codigo, idioma.Codigo);
        Assert.Equal(nombre, idioma.Nombre);
    }
}
