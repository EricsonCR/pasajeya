using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder
            .HasOne(p => p.Boleto)
            .WithMany(b => b.Pagos)
            .HasForeignKey(p => p.BoletoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(p => p.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .Property(p => p.NumeroOperacion)
            .HasMaxLength(50);

        builder
            .Property(p => p.Monto)
            .HasPrecision(10, 2);

        builder
            .HasIndex(p => p.BoletoId)
            .IsUnique()
            .HasFilter("[Estado] = ('Aprobado')");
    }
}