using PasajeYa.Application.Dtos;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Application.Services;

public class ViajeService : IViajeService
{
    private readonly IViajeRepository _viajeRepository;

    public ViajeService(IViajeRepository viajeRepository)
    {
        _viajeRepository = viajeRepository;
    }

    public async Task<IReadOnlyList<ViajeDto>> BuscarAsync(BuscarViajesRequest request)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var inicioDelDia = request.Fecha.ToDateTime(TimeOnly.MinValue);

        var desde = request.Fecha == hoy ? DateTime.Now : inicioDelDia;
        var hasta = inicioDelDia.AddDays(1);

        var viajes = await _viajeRepository.BuscarAsync(request.Origen, request.Destino, desde, hasta);
        var viajesDto = viajes
            .Select(v => new ViajeDto(
                v.Id,
                    v.Salida,
                    v.Llegada,
                    v.Bus!.TipoServicio,
                    v.Precio,
                    v.AsientosDisponibles()
            ))
            .ToList();

        return viajesDto;
    }
}