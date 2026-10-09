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

        builder.HasData(
            new Ruta { Id = 1, OrigenId = 1, DestinoId = 2 },
            new Ruta { Id = 2, OrigenId = 1, DestinoId = 3 },
            new Ruta { Id = 3, OrigenId = 1, DestinoId = 4 },
            new Ruta { Id = 4, OrigenId = 1, DestinoId = 5 },
            new Ruta { Id = 5, OrigenId = 1, DestinoId = 6 },
            new Ruta { Id = 6, OrigenId = 2, DestinoId = 1 },
            new Ruta { Id = 7, OrigenId = 2, DestinoId = 3 },
            new Ruta { Id = 8, OrigenId = 2, DestinoId = 4 },
            new Ruta { Id = 9, OrigenId = 2, DestinoId = 5 },
            new Ruta { Id = 10, OrigenId = 3, DestinoId = 1 },
            new Ruta { Id = 11, OrigenId = 3, DestinoId = 7 },
            new Ruta { Id = 12, OrigenId = 3, DestinoId = 8 },
            new Ruta { Id = 13, OrigenId = 3, DestinoId = 4 },
            new Ruta { Id = 14, OrigenId = 5, DestinoId = 1 },
            new Ruta { Id = 15, OrigenId = 6, DestinoId = 3 }
        );
    }
}