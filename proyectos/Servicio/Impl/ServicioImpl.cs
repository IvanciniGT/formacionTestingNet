using AutoMapper;
using Diccionarios.Api;
using ServicioDiccionarios.DTOs;

namespace ServicioDiccionarios.Implementacion;

/// <summary>
/// Implementación del servicio de diccionarios que encapsula la lógica de negocio
/// y actúa como facade hacia la API de diccionarios.
/// NOTA: El logging se maneja mediante el decorador ServicioLoggingDecorator (AOP).
/// </summary>
public class ServicioImpl : IServicio
{
    private readonly IProveedorDiccionarios _suministradorDiccionarios;
    private readonly IMapper _mapper;

    public ServicioImpl(
        IProveedorDiccionarios suministradorDiccionarios,
        IMapper mapper)
    {
        _suministradorDiccionarios = suministradorDiccionarios ?? throw new ArgumentNullException(nameof(suministradorDiccionarios));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public List<IdiomaDTO> GetIdiomas()
    {
        var idiomas = _suministradorDiccionarios.GetIdiomas();
        return _mapper.Map<List<IdiomaDTO>>(idiomas);
    }

    public List<DiccionarioDTO>? GetDiccionarios(string codigoIdioma)
    {
        if (string.IsNullOrWhiteSpace(codigoIdioma))
            return null;

        var diccionarios = _suministradorDiccionarios.GetDiccionarios(codigoIdioma);
        return diccionarios != null ? _mapper.Map<List<DiccionarioDTO>>(diccionarios) : null;
    }

    public DiccionarioDTO? GetDiccionario(string codigoDiccionario)
    {
        if (string.IsNullOrWhiteSpace(codigoDiccionario))
            return null;

        var diccionario = _suministradorDiccionarios.GetDiccionarioPorCodigo(codigoDiccionario);
        return diccionario != null ? _mapper.Map<DiccionarioDTO>(diccionario) : null;
    }

    public List<SignificadoDTO>? GetSignificadosEnDiccionario(string codigoDiccionario, string palabra)
    {
        if (string.IsNullOrWhiteSpace(codigoDiccionario) || string.IsNullOrWhiteSpace(palabra))
            return null;

        var diccionario = _suministradorDiccionarios.GetDiccionarioPorCodigo(codigoDiccionario);
        if (diccionario == null)
            return null;

        var significados = diccionario.GetSignificados(palabra);
        if (significados == null || !significados.Any())
            return null;

        return significados.Select(s => new SignificadoDTO 
        { 
            Texto = s, 
            Diccionario = diccionario.Codigo
        }).ToList();
    }

    public List<SignificadoDTO>? GetSignificadosEnIdioma(string codigoIdioma, string palabra)
    {
        if (string.IsNullOrWhiteSpace(codigoIdioma) || string.IsNullOrWhiteSpace(palabra))
            return null;

        var diccionarios = _suministradorDiccionarios.GetDiccionarios(codigoIdioma);
        if (diccionarios == null || !diccionarios.Any())
            return null;

        var todosLosSignificados = new List<SignificadoDTO>();

        foreach (var diccionario in diccionarios)
        {
            var significados = diccionario.GetSignificados(palabra);
            if (significados != null && significados.Any())
            {
                todosLosSignificados.AddRange(
                    significados.Select(s => new SignificadoDTO 
                    { 
                        Texto = s, 
                        Diccionario = diccionario.Codigo
                    })
                );
            }
        }

        return todosLosSignificados.Any() ? todosLosSignificados : null;
    }

    public bool ExistePalabraEnDiccionario(string codigoDiccionario, string palabra)
    {
        if (string.IsNullOrWhiteSpace(codigoDiccionario) || string.IsNullOrWhiteSpace(palabra))
            return false;

        var diccionario = _suministradorDiccionarios.GetDiccionarioPorCodigo(codigoDiccionario);
        return diccionario?.Existe(palabra) ?? false;
    }

    public bool ExistePalabraEnIdioma(string codigoIdioma, string palabra)
    {
        if (string.IsNullOrWhiteSpace(codigoIdioma) || string.IsNullOrWhiteSpace(palabra))
            return false;

        var diccionarios = _suministradorDiccionarios.GetDiccionarios(codigoIdioma);
        if (diccionarios == null || !diccionarios.Any())
            return false;

        return diccionarios.Any(d => d.Existe(palabra));
    }
}