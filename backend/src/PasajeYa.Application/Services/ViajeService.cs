using PasajeYa.Application.Dtos;
using PasajeYa.Application.Exceptions;
using PasajeYa.Application.Interfaces;

namespace PasajeYa.Application.Services;

public class ViajeService : IViajeService
{
    private readonly IViajeRepository _viajeRepository;
    private readonly ICiudadRepository _ciudadRepository;

    public ViajeService(IViajeRepository viajeRepository, ICiudadRepository ciudadRepository)
    {
        _viajeRepository = viajeRepository;
        _ciudadRepository = ciudadRepository;
    }

    public async Task<IReadOnlyList<ViajeDto>> BuscarAsync(BuscarViajesRequest request)
    {
        var errores = new Dictionary<string, string[]>();
        var hoy = DateOnly.FromDateTime(DateTime.Today);

        if (request.Origen == request.Destino) { errores["destino"] = ["Origen y destino no pueden ser iguales"]; }

        var existeOrigen = await _ciudadRepository.ExisteAsync(request.Origen);
        if (!existeOrigen) { errores["origen"] = ["La ciudad de origen no existe"]; }

        var existeDestino = await _ciudadRepository.ExisteAsync(request.Destino);
        if (!existeDestino) { errores["destino"] = ["La ciudad de destino no existe"]; }

        if (request.Fecha > hoy.AddDays(30)) { errores["fecha"] = ["La fecha no puede ser mayor a 30 dias"]; }
        if (request.Fecha < hoy) { errores["fecha"] = ["La fecha no puede ser anterior a hoy"]; }

        if (errores.Count > 0) { throw new ValidacionException("Error al buscar viajes", errores); }

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