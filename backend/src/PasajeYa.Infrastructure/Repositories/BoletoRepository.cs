using Microsoft.EntityFrameworkCore;
using PasajeYa.Application.Exceptions;
using PasajeYa.Application.Interfaces;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;
using PasajeYa.Infrastructure.Data;

namespace PasajeYa.Infrastructure.Repositories;

public class BoletoRepository : IBoletoRepository
{
    private readonly PasajeYaDbContext _context;

    public BoletoRepository(PasajeYaDbContext context)
    {
        _context = context;
    }

    public async Task<Boleto?> ObtenerPorIdAsync(int boletoId)
    {
        var boleto = await _context.Boletos
               .AsNoTracking()
               .Include(b => b.Viaje).ThenInclude(v => v!.Ruta).ThenInclude(r => r!.Origen)
               .Include(b => b.Viaje).ThenInclude(v => v!.Ruta).ThenInclude(r => r!.Destino)
               .FirstOrDefaultAsync(x => x.Id == boletoId);

        return boleto;
    }

    public async Task RegistrarPagoAsync(Boleto boleto, Pago pago)
    {
        _context.Boletos.Attach(boleto);
        boleto.Estado = EstadoBoleto.Pagado;
        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();
    }

    public async Task ReservarAsync(Boleto boletoNuevo, Boleto? boletoVencido)
    {
        try
        {
            if (boletoVencido is not null)
            {
                _context.Boletos.Attach(boletoVencido);
                boletoVencido.Estado = EstadoBoleto.Anulado;
            }
            _context.Boletos.Add(boletoNuevo);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ConflictoException("El asiento ya fue reservado");
        }
    }
}