using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PasajeYa.Domain.Entities;

namespace PasajeYa.Infrastructure.Data.Configurations;

public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder
            .Property(c => c.Nombre)
            .HasMaxLength(100);

        builder
            .HasIndex(c => c.Nombre)
            .IsUnique();

        builder.HasData(
            new Ciudad { Id = 1, Nombre = "Lima" },
            new Ciudad { Id = 2, Nombre = "Arequipa" },
            new Ciudad { Id = 3, Nombre = "Trujillo" },
            new Ciudad { Id = 4, Nombre = "Tacna" },
            new Ciudad { Id = 5, Nombre = "Piura" },
            new Ciudad { Id = 6, Nombre = "Cajamarca" },
            new Ciudad { Id = 7, Nombre = "Cusco" },
            new Ciudad { Id = 8, Nombre = "Huancayo" }
        );
    }
}