using Microsoft.EntityFrameworkCore;
using PasajeYa.Application.Interfaces;
using PasajeYa.Domain.Entities;
using PasajeYa.Infrastructure.Data;

namespace PasajeYa.Infrastructure.Repositories;

public class ViajeRepository : IViajeRepository
{
    private readonly PasajeYaDbContext _context;

    public ViajeRepository(PasajeYaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Viaje>> BuscarAsync(int origenId, int destinoId, DateTime desde, DateTime hasta)
    {
        var viajes = await _context.Viajes
            .AsNoTracking()
            .Include(v => v.Bus)
            .Include(v => v.Boletos)
            .Where(v =>
                v.Ruta!.OrigenId == origenId
                && v.Ruta!.DestinoId == destinoId
                && v.Salida >= desde
                && v.Salida < hasta
            )
            .OrderBy(v => v.Salida)
            .ToListAsync();
        return viajes;
    }
}