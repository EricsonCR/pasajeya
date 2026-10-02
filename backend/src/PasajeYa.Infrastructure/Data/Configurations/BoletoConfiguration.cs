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
            .WithMany()
            .HasForeignKey(b => b.ViajeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(b => b.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}