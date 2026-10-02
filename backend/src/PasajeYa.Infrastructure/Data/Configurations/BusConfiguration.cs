using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;
using PasajeYa.Domain.Enums;

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

        builder.HasData(
            new Bus { Id = 1, Placa = "BDS275", Capacidad = 32, TipoServicio = TipoServicio.Economico },
            new Bus { Id = 2, Placa = "KFC192", Capacidad = 48, TipoServicio = TipoServicio.Economico },
            new Bus { Id = 3, Placa = "PIL483", Capacidad = 32, TipoServicio = TipoServicio.Ejecutivo },
            new Bus { Id = 4, Placa = "JFK581", Capacidad = 28, TipoServicio = TipoServicio.Ejecutivo },
            new Bus { Id = 5, Placa = "MGM397", Capacidad = 16, TipoServicio = TipoServicio.Vip },
            new Bus { Id = 6, Placa = "FIT814", Capacidad = 14, TipoServicio = TipoServicio.Vip }
        );
    }
}