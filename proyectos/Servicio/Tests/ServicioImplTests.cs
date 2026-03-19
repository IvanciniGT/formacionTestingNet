using AutoMapper;
using Diccionarios.Api;
using Moq;
using ServicioDiccionarios.DTOs;
using ServicioDiccionarios.Implementacion;
using ServicioDiccionarios.Implementacion.Mappers;

namespace ServicioDiccionarios.Tests;

public class ServicioImplTests
{
    private readonly Mock<IProveedorDiccionarios> _mockSuministrador;
    private readonly IMapper _mapper;
    private readonly ServicioImpl _servicio;

    public ServicioImplTests()
    {
        _mockSuministrador = new Mock<IProveedorDiccionarios>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DiccionariosMapperProfile>());
        _mapper = config.CreateMapper();
        
        _servicio = new ServicioImpl(_mockSuministrador.Object, _mapper);
    }

    [Fact]
    public void GetIdiomas_DeberiaRetornarListaDeIdiomasDTO()
    {
        // Arrange
        var mockIdiomas = new List<IIdioma>
        {
            CreateMockIdioma("ES", "Español"),
            CreateMockIdioma("EN", "English")
        };
        _mockSuministrador.Setup(x => x.GetIdiomas()).Returns(mockIdiomas);

        // Act
        var resultado = _servicio.GetIdiomas();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("ES", resultado[0].Codigo);
        Assert.Equal("Español", resultado[0].Nombre);
        Assert.Equal("EN", resultado[1].Codigo);
        Assert.Equal("English", resultado[1].Nombre);
    }

    [Fact]
    public void GetDiccionarios_ConIdiomaValido_DeberiaRetornarDiccionarios()
    {
        // Arrange
        var codigoIdioma = "ES";
        var mockDiccionarios = new List<IDiccionario>
        {
            CreateMockDiccionario("ES_RAE", "ES").Object,
            CreateMockDiccionario("ES_LAROUSSE", "ES").Object
        };
        _mockSuministrador.Setup(x => x.GetDiccionarios(codigoIdioma)).Returns(mockDiccionarios);

        // Act
        var resultado = _servicio.GetDiccionarios(codigoIdioma);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("ES_RAE", resultado[0].Codigo);
        Assert.Equal("ES", resultado[0].Idioma);
    }

    [Fact]
    public void GetDiccionarios_ConIdiomaInvalido_DeberiaRetornarNull()
    {
        // Arrange
        var codigoIdioma = "FR";
        _mockSuministrador.Setup(x => x.GetDiccionarios(codigoIdioma)).Returns((IList<IDiccionario>?)null);

        // Act
        var resultado = _servicio.GetDiccionarios(codigoIdioma);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void GetDiccionario_ConCodigoValido_DeberiaRetornarDiccionario()
    {
        // Arrange
        var codigoDiccionario = "ES_RAE";
        var mockDiccionario = CreateMockDiccionario(codigoDiccionario, "ES");
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo(codigoDiccionario)).Returns(mockDiccionario.Object);

        // Act
        var resultado = _servicio.GetDiccionario(codigoDiccionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(codigoDiccionario, resultado.Codigo);
        Assert.Equal("ES", resultado.Idioma);
    }

    [Fact]
    public void ExistePalabraEnDiccionario_PalabraExiste_DeberiaRetornarTrue()
    {
        // Arrange
        var codigoDiccionario = "ES_RAE";
        var palabra = "casa";
        var mockDiccionario = CreateMockDiccionario(codigoDiccionario, "ES");
        mockDiccionario.Setup(x => x.Existe(palabra)).Returns(true);
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo(codigoDiccionario)).Returns(mockDiccionario.Object);

        // Act
        var resultado = _servicio.ExistePalabraEnDiccionario(codigoDiccionario, palabra);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void ExistePalabraEnIdioma_PalabraExisteEnAlgunDiccionario_DeberiaRetornarTrue()
    {
        // Arrange
        var codigoIdioma = "ES";
        var palabra = "casa";
        var mockDiccionario1 = CreateMockDiccionario("ES_RAE", "ES");
        var mockDiccionario2 = CreateMockDiccionario("ES_LAROUSSE", "ES");
        
        mockDiccionario1.Setup(x => x.Existe(palabra)).Returns(false);
        mockDiccionario2.Setup(x => x.Existe(palabra)).Returns(true);
        
        var mockDiccionarios = new List<IDiccionario> { mockDiccionario1.Object, mockDiccionario2.Object };
        _mockSuministrador.Setup(x => x.GetDiccionarios(codigoIdioma)).Returns(mockDiccionarios);

        // Act
        var resultado = _servicio.ExistePalabraEnIdioma(codigoIdioma, palabra);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void GetSignificadosEnDiccionario_PalabraExiste_DeberiaRetornarSignificados()
    {
        // Arrange
        var codigoDiccionario = "ES_RAE";
        var palabra = "casa";
        var significados = new List<string> { "Edificio para habitar", "Hogar familiar" };
        
        var mockDiccionario = CreateMockDiccionario(codigoDiccionario, "ES");
        mockDiccionario.Setup(x => x.GetSignificados(palabra)).Returns(significados);
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo(codigoDiccionario)).Returns(mockDiccionario.Object);

        // Act
        var resultado = _servicio.GetSignificadosEnDiccionario(codigoDiccionario, palabra);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Edificio para habitar", resultado[0].Texto);
        Assert.Equal(codigoDiccionario, resultado[0].Diccionario);
    }

    [Fact]
    public void GetSignificadosEnIdioma_PalabraExisteEnMultiplesDiccionarios_DeberiaRetornarTodosLosSignificados()
    {
        // Arrange
        var codigoIdioma = "ES";
        var palabra = "casa";
        
        var mockDiccionario1 = CreateMockDiccionario("ES_RAE", "ES");
        var mockDiccionario2 = CreateMockDiccionario("ES_LAROUSSE", "ES");
        
        mockDiccionario1.Setup(x => x.GetSignificados(palabra)).Returns(new List<string> { "Edificio para habitar" });
        mockDiccionario2.Setup(x => x.GetSignificados(palabra)).Returns(new List<string> { "Hogar familiar" });
        
        var mockDiccionarios = new List<IDiccionario> { mockDiccionario1.Object, mockDiccionario2.Object };
        _mockSuministrador.Setup(x => x.GetDiccionarios(codigoIdioma)).Returns(mockDiccionarios);

        // Act
        var resultado = _servicio.GetSignificadosEnIdioma(codigoIdioma, palabra);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, s => s.Diccionario == "ES_RAE" && s.Texto == "Edificio para habitar");
        Assert.Contains(resultado, s => s.Diccionario == "ES_LAROUSSE" && s.Texto == "Hogar familiar");
    }

    // =========================================================================
    // Tests adicionales: entradas nulas/vacías y casos borde
    // =========================================================================

    [Fact]
    public void GetDiccionario_ConCodigoNull_DeberiaRetornarNull()
    {
        var resultado = _servicio.GetDiccionario(null!);
        Assert.Null(resultado);
    }

    [Fact]
    public void GetDiccionario_ConCodigoVacio_DeberiaRetornarNull()
    {
        var resultado = _servicio.GetDiccionario("  ");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetDiccionario_ConCodigoInexistente_DeberiaRetornarNull()
    {
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo("ELF")).Returns((IDiccionario?)null);
        var resultado = _servicio.GetDiccionario("ELF");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetDiccionarios_ConIdiomaVacio_DeberiaRetornarNull()
    {
        var resultado = _servicio.GetDiccionarios("  ");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnDiccionario_ConPalabraVacia_DeberiaRetornarNull()
    {
        var resultado = _servicio.GetSignificadosEnDiccionario("ES_RAE", "  ");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnDiccionario_DiccionarioNoExiste_DeberiaRetornarNull()
    {
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo("ELF")).Returns((IDiccionario?)null);
        var resultado = _servicio.GetSignificadosEnDiccionario("ELF", "casa");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnDiccionario_PalabraNoExiste_DeberiaRetornarNull()
    {
        var mockDiccionario = CreateMockDiccionario("ES_RAE", "ES");
        mockDiccionario.Setup(x => x.GetSignificados("zzz")).Returns((IList<string>?)null);
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo("ES_RAE")).Returns(mockDiccionario.Object);

        var resultado = _servicio.GetSignificadosEnDiccionario("ES_RAE", "zzz");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnIdioma_ConPalabraVacia_DeberiaRetornarNull()
    {
        var resultado = _servicio.GetSignificadosEnIdioma("ES", "  ");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnIdioma_IdiomaNoExiste_DeberiaRetornarNull()
    {
        _mockSuministrador.Setup(x => x.GetDiccionarios("ELF")).Returns((IList<IDiccionario>?)null);
        var resultado = _servicio.GetSignificadosEnIdioma("ELF", "casa");
        Assert.Null(resultado);
    }

    [Fact]
    public void GetSignificadosEnIdioma_PalabraNoExisteEnNingunDiccionario_DeberiaRetornarNull()
    {
        var mockDic1 = CreateMockDiccionario("ES_RAE", "ES");
        var mockDic2 = CreateMockDiccionario("ES_LAROUSSE", "ES");
        mockDic1.Setup(x => x.GetSignificados("zzz")).Returns((IList<string>?)null);
        mockDic2.Setup(x => x.GetSignificados("zzz")).Returns((IList<string>?)null);

        _mockSuministrador.Setup(x => x.GetDiccionarios("ES"))
            .Returns(new List<IDiccionario> { mockDic1.Object, mockDic2.Object });

        var resultado = _servicio.GetSignificadosEnIdioma("ES", "zzz");
        Assert.Null(resultado);
    }

    [Fact]
    public void ExistePalabraEnDiccionario_PalabraNoExiste_DeberiaRetornarFalse()
    {
        var mockDiccionario = CreateMockDiccionario("ES_RAE", "ES");
        mockDiccionario.Setup(x => x.Existe("zzz")).Returns(false);
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo("ES_RAE")).Returns(mockDiccionario.Object);

        var resultado = _servicio.ExistePalabraEnDiccionario("ES_RAE", "zzz");
        Assert.False(resultado);
    }

    [Fact]
    public void ExistePalabraEnDiccionario_DiccionarioNoExiste_DeberiaRetornarFalse()
    {
        _mockSuministrador.Setup(x => x.GetDiccionarioPorCodigo("ELF")).Returns((IDiccionario?)null);
        var resultado = _servicio.ExistePalabraEnDiccionario("ELF", "casa");
        Assert.False(resultado);
    }

    [Fact]
    public void ExistePalabraEnDiccionario_ConParametrosVacios_DeberiaRetornarFalse()
    {
        Assert.False(_servicio.ExistePalabraEnDiccionario("  ", "casa"));
        Assert.False(_servicio.ExistePalabraEnDiccionario("ES_RAE", "  "));
    }

    [Fact]
    public void ExistePalabraEnIdioma_PalabraNoExisteEnNinguno_DeberiaRetornarFalse()
    {
        var mockDic1 = CreateMockDiccionario("ES_RAE", "ES");
        var mockDic2 = CreateMockDiccionario("ES_LAROUSSE", "ES");
        mockDic1.Setup(x => x.Existe("zzz")).Returns(false);
        mockDic2.Setup(x => x.Existe("zzz")).Returns(false);

        _mockSuministrador.Setup(x => x.GetDiccionarios("ES"))
            .Returns(new List<IDiccionario> { mockDic1.Object, mockDic2.Object });

        var resultado = _servicio.ExistePalabraEnIdioma("ES", "zzz");
        Assert.False(resultado);
    }

    [Fact]
    public void ExistePalabraEnIdioma_IdiomaNoExiste_DeberiaRetornarFalse()
    {
        _mockSuministrador.Setup(x => x.GetDiccionarios("ELF")).Returns((IList<IDiccionario>?)null);
        var resultado = _servicio.ExistePalabraEnIdioma("ELF", "casa");
        Assert.False(resultado);
    }

    [Fact]
    public void ExistePalabraEnIdioma_ConParametrosVacios_DeberiaRetornarFalse()
    {
        Assert.False(_servicio.ExistePalabraEnIdioma("  ", "casa"));
        Assert.False(_servicio.ExistePalabraEnIdioma("ES", "  "));
    }

    private static IIdioma CreateMockIdioma(string codigo, string nombre)
    {
        var mock = new Mock<IIdioma>();
        mock.Setup(x => x.Codigo).Returns(codigo);
        mock.Setup(x => x.Nombre).Returns(nombre);
        return mock.Object;
    }

    private static Mock<IDiccionario> CreateMockDiccionario(string codigo, string idioma)
    {
        var mock = new Mock<IDiccionario>();
        mock.Setup(x => x.Codigo).Returns(codigo);
        mock.Setup(x => x.Idioma).Returns(idioma);
        return mock;
    }
}
