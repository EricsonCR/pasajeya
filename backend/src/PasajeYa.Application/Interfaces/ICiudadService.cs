using PasajeYa.Application.Dtos;

namespace PasajeYa.Application.Interfaces;

public interface ICiudadService
{
    Task<IReadOnlyList<CiudadDto>> GetAllAsync();
}