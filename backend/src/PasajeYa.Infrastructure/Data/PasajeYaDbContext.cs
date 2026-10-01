using Microsoft.EntityFrameworkCore;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data;

public class PasajeYaDbContext : DbContext
{
    public PasajeYaDbContext(DbContextOptions<PasajeYaDbContext> options) : base(options) { }

    public DbSet<Ciudad> Ciudades { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Ruta> Rutas { get; set; }
    public DbSet<Viaje> Viajes { get; set; }
    public DbSet<Boleto> Boletos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PasajeYaDbContext).Assembly);
    }
}