using PasajeYa.Application.Dtos;

namespace PasajeYa.Application.Interfaces;

public interface IViajeService
{
    Task<IReadOnlyList<ViajeDto>> BuscarAsync(BuscarViajesRequest request);
}