using System.Net;
using System.Text.Json;
using DiccionariosRestV1Api;

namespace Rest.V1.SystemTests;

/// <summary>
/// Pruebas de SISTEMA del endpoint /api/v1/diccionarios
/// Aquí hacemos peticiones HTTP reales a la WebApi completa con MariaDB.
/// </summary>
public class DiccionariosSystemTests : IClassFixture<DiccionariosWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public DiccionariosSystemTests(DiccionariosWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    // =========================================================================
    // GetDiccionario: Happy Path - GET /api/v1/diccionarios/ES_RAE → 200
    // =========================================================================
    [Fact]
    public async Task GetDiccionario_DiccionarioExiste_Devuelve200()
    {
        // GIVEN: La BBDD tiene el diccionario ES_RAE (seedeado en el constructor)

        // WHEN: Hago una petición HTTP GET al endpoint
        var response = await _client.GetAsync("/api/v1/diccionarios/ES_RAE");

        // THEN: Me devuelve un 200 OK con el diccionario
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var diccionario = JsonSerializer.Deserialize<DiccionarioRestV1DTO>(content, _jsonOptions);

        Assert.NotNull(diccionario);
        Assert.Equal("ES_RAE", diccionario.Codigo);
        Assert.Equal("Diccionario RAE", diccionario.Nombre);
        Assert.Equal("ES", diccionario.Idioma);
    }

    // =========================================================================
    // GetDiccionario: Fail - GET /api/v1/diccionarios/ELF → 404
    // =========================================================================
    [Fact]
    public async Task GetDiccionario_DiccionarioNoExiste_Devuelve404()
    {
        // GIVEN: La BBDD NO tiene el diccionario de los elfos

        // WHEN: Hago una petición HTTP GET al endpoint de los elfos
        var response = await _client.GetAsync("/api/v1/diccionarios/ELF");

        // THEN: Me devuelve un 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // =========================================================================
    // GetSignificados: Happy Path - GET /api/v1/diccionarios/ES_RAE/significados?palabra=casa → 200
    // =========================================================================
    [Fact]
    public async Task GetSignificados_PalabraExiste_Devuelve200ConSignificados()
    {
        // GIVEN: La BBDD tiene la palabra "casa" con 2 significados en ES_RAE

        // WHEN: Hago una petición HTTP GET pidiendo los significados
        var response = await _client.GetAsync("/api/v1/diccionarios/ES_RAE/significados?palabra=casa");

        // THEN: Me devuelve un 200 OK con los significados
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var significados = JsonSerializer.Deserialize<List<SignificadoRestV1DTO>>(content, _jsonOptions);

        Assert.NotNull(significados);
        Assert.Equal(2, significados.Count);
        Assert.Contains(significados, s => s.Texto == "Edificio para habitar");
        Assert.Contains(significados, s => s.Texto == "Hogar familiar");
    }
}
