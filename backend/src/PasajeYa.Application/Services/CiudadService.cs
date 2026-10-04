using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Application.Services;

public class CiudadService : ICiudadService
{
    private readonly ICiudadRepository _ciudadRepository;

    public CiudadService(ICiudadRepository ciudadRepository)
    {
        _ciudadRepository = ciudadRepository;
    }

    public async Task<IReadOnlyList<CiudadDto>> GetAllAsync()
    {
        var ciudades = await _ciudadRepository.GetAllAsync();
        var ciudadesDto = ciudades.Select(c => new CiudadDto(c.Id, c.Nombre)).ToList();
        return ciudadesDto;
    }
}