using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class BoletoConfiguration : IEntityTypeConfiguration<Boleto>
{
    public void Configure(EntityTypeBuilder<Boleto> builder)
    {
        builder
            .HasOne(b => b.Viaje)
            .WithMany(v => v.Boletos)
            .HasForeignKey(b => b.ViajeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(b => b.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .Property(b => b.Dni)
            .IsFixedLength()
            .HasMaxLength(8);

        builder
            .Property(b => b.Nombres)
            .HasMaxLength(100);

        builder
            .Property(b => b.Apellidos)
            .HasMaxLength(100);

        builder
            .Property(b => b.Correo)
            .HasMaxLength(150);

        builder
            .Property(b => b.Celular)
            .HasMaxLength(9);

        builder
            .HasIndex(b => new { b.ViajeId, b.NumeroAsiento })
            .IsUnique()
            .HasFilter("[Estado] IN ('Reservado','Pagado')");
    }
}