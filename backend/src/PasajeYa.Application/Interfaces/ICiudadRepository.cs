using PasajeYa.Domain.Entities;

namespace PasajeYa.Application.Interfaces;

public interface ICiudadRepository
{
    Task<IReadOnlyList<Ciudad>> GetAllAsync();
}