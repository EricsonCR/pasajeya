using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class RutaConfiguration : IEntityTypeConfiguration<Ruta>
{
    public void Configure(EntityTypeBuilder<Ruta> builder)
    {
        builder
            .HasOne(r => r.Origen)
            .WithMany()
            .HasForeignKey(r => r.OrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(r => r.Destino)
            .WithMany()
            .HasForeignKey(r => r.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(r => new { r.OrigenId, r.DestinoId })
            .IsUnique();
        
        builder
            .ToTable(t => t.HasCheckConstraint("CK_Rutas_OrigenDistintoDestino", "[OrigenId]<>[DestinoId]"));
    }
}