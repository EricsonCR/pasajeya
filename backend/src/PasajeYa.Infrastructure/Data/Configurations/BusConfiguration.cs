using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder
            .Property(b => b.Placa)
            .HasMaxLength(10);

        builder
            .HasIndex(b => b.Placa)
            .IsUnique();
    }
}