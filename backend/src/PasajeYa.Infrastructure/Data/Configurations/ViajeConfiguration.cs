using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class ViajeConfiguration : IEntityTypeConfiguration<Viaje>
{
    public void Configure(EntityTypeBuilder<Viaje> builder)
    {
        builder
            .Property(v => v.Precio)
            .HasPrecision(10, 2);

        builder
            .HasOne(v => v.Ruta)
            .WithMany()
            .HasForeignKey(v => v.RutaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Bus)
            .WithMany()
            .HasForeignKey(v => v.BusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}