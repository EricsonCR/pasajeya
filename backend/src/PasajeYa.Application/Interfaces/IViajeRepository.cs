using PasajeYa.Domain.Entities;

namespace PasajeYa.Application.Interfaces;

public interface IViajeRepository
{
    Task<IReadOnlyList<Viaje>> BuscarAsync(int origenId, int destinoId, DateTime desde, DateTime hasta);
}