using Microsoft.EntityFrameworkCore;
using PasajeYa.Application.Interfaces;
using PasajeYa.Domain.Entities;
using PasajeYa.Infrastructure.Data;

namespace PasajeYa.Infrastructure.Repositories;

public class CiudadRepository : ICiudadRepository
{
    private readonly PasajeYaDbContext _context;

    public CiudadRepository(PasajeYaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Ciudad>> GetAllAsync()
    {
        return await _context.Ciudades.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();
    }
}